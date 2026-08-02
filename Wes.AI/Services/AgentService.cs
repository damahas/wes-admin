using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.ChatCompletion;
using SKKernel = Microsoft.SemanticKernel.Kernel;
using Wes.AI.Kernel;
using Wes.AI.Models.Entity;
using Wes.AI.Models.ViewModel;
using Wes.AI.Plugins;
using SqlSugar;

namespace Wes.AI.Services;

/// <summary>
/// Agent 服务：使用 SK ChatCompletionAgent + Function Calling
///
/// 架构（多助手路由 + 子 agent 隔离）：
/// - 三个「一线子 agent」各自独立、可单独调用，且插件相互隔离：
///     · general   通用对话，Kernel 不挂载任何领域插件
///     · sql_expert 数据库查询，Kernel 仅挂载 sql + knowledge，并注入表名提示
///     · ticket    工单管理，Kernel 仅挂载 ticket + knowledge
///   （SQL 插件只在 sql_expert 的 Kernel 上，其他助手根本看不到 execute_sql，根治「什么都带 SQL」）
/// - 一个「总 agent」（auto）：把上述三个子 agent 注册成工具（Agent-as-Tool），
///   由模型按意图选择调用哪个子 agent，必要时组合多个子 agent 的结果后汇总返回。
///   前端默认走 auto，对用户而言仍是「单一助手」；同时每个子 agent 也可通过 /ai/agent/{type} 单独调用。
///
/// SK ChatCompletionAgent 内置自动 Function Calling 循环：产生 tool_calls 时由 Agent 负责
/// 调用插件/子 agent 并把结果回灌，直到产出最终文本。
/// </summary>
public class AgentService : IAgentService
{
    private readonly KernelProvider _kernelProvider;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<AgentService> _logger;

    // 助手类型元数据（含总 agent auto 与三个可独立调用的子 agent）
    private static readonly Dictionary<string, AgentTypeInfo> AgentTypeMeta = new()
    {
        ["auto"] = new() { Type = "auto", DisplayName = "智能助手", Description = "自动识别意图，路由到合适的子助手（数据库/图表/工单/通用）并汇总" },
        ["general"] = new() { Type = "general", DisplayName = "通用对话", Description = "通用对话 Agent，不挂载任何数据库/工单工具" },
        ["sql_expert"] = new() { Type = "sql_expert", DisplayName = "SQL 专家", Description = "辅助生成和执行只读 SQL 查询" },
        ["chart_expert"] = new() { Type = "chart_expert", DisplayName = "图表助手", Description = "查询数据并生成 ECharts 图表配置 JSON" },
        ["ticket"] = new() { Type = "ticket", DisplayName = "工单助手", Description = "辅助创建、查询和管理工单" }
    };

    public AgentService(
        KernelProvider kernelProvider,
        IServiceProvider serviceProvider,
        ILogger<AgentService> logger)
    {
        _kernelProvider = kernelProvider;
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    // ==================== 公开方法 ====================

    public async Task<AiAgentResponse> ExecuteAsync(
        string agentType, AiAgentRequest request, CancellationToken ct = default)
    {
        var sw = Stopwatch.StartNew();
        var provider = _kernelProvider.ResolveProviderName(request.Provider);
        var model = _kernelProvider.GetModelId(request.Provider, request.Model);

        var agent = BuildAgent(agentType, provider, model, request.Temperature, request.MaxTokens);
        var history = BuildHistory(request);

        _logger.LogInformation("Agent[{Type}] 开始调用模型: provider={Provider}, model={Model}, 消息总数={MsgCount}, 历史条数={HistoryCount}, temperature={Temp}, maxTokens={Max}",
            agentType, provider, model, history.Count, request.History?.Count ?? 0, request.Temperature, request.MaxTokens);

        var sb = new StringBuilder();
        Exception? callEx = null;
        try
        {
            await foreach (var resp in agent.InvokeAsync(history, cancellationToken: ct))
            {
                if (!string.IsNullOrEmpty(resp.Message.Content))
                    sb.Append(resp.Message.Content);
            }
            sw.Stop();
            _logger.LogInformation("Agent[{Type}] 调用成功: provider={Provider}, model={Model}, 耗时={Ms}ms, 返回长度={Len}",
                agentType, provider, model, sw.ElapsedMilliseconds, sb.Length);
            if (sb.Length > 0)
                _logger.LogDebug("Agent[{Type}] 返回内容预览: {Preview}", agentType, sb.Length <= 300 ? sb.ToString() : sb.ToString()[..300] + "...");
        }
        catch (Exception ex)
        {
            sw.Stop();
            callEx = ex;
            _logger.LogError(ex, "Agent[{Type}] 调用模型失败: provider={Provider}, model={Model}, 耗时={Ms}ms。InnerException 含 HTTP 状态码与响应体，可据此判断密钥/地址/模型名是否正确。",
                agentType, provider, model, sw.ElapsedMilliseconds);
        }

        if (callEx != null)
            throw callEx;

        return new AiAgentResponse
        {
            AgentType = agentType,
            Provider = provider,
            Model = model,
            Content = sb.ToString(),
            DurationMs = sw.ElapsedMilliseconds
        };
    }

    public async IAsyncEnumerable<StreamChunk> ExecuteStreamAsync(
        string agentType, AiAgentRequest request,
        [EnumeratorCancellation] CancellationToken ct = default)
    {
        var provider = _kernelProvider.ResolveProviderName(request.Provider);
        var model = _kernelProvider.GetModelId(request.Provider, request.Model);

        var agent = BuildAgent(agentType, provider, model, request.Temperature, request.MaxTokens);
        var history = BuildHistory(request);

        var sw = Stopwatch.StartNew();
        _logger.LogInformation("Agent[{Type}] 开始流式调用模型: provider={Provider}, model={Model}, 消息总数={MsgCount}, 历史条数={HistoryCount}",
            agentType, provider, model, history.Count, request.History?.Count ?? 0);

        // 先收集所有分块再统一 yield，避免 yield 出现在带 catch 的 try 块内（C# 不允许）
        var chunks = new List<StreamChunk>();
        Exception? streamEx = null;
        try
        {
            await foreach (var chunk in agent.InvokeStreamingAsync(history, cancellationToken: ct))
            {
                // 仅转发带有正文的流式片段；工具调用步骤的片段 Content 为空，跳过
                if (string.IsNullOrEmpty(chunk.Message.Content))
                    continue;

                chunks.Add(new StreamChunk
                {
                    Content = chunk.Message.Content,
                    FinishReason = chunk.Message.Metadata?.ContainsKey("FinishReason") == true
                        ? chunk.Message.Metadata["FinishReason"]?.ToString() : null
                });
            }
            sw.Stop();
            _logger.LogInformation("Agent[{Type}] 流式调用成功: provider={Provider}, model={Model}, 耗时={Ms}ms, 块数={Count}",
                agentType, provider, model, sw.ElapsedMilliseconds, chunks.Count);
        }
        catch (Exception ex)
        {
            sw.Stop();
            streamEx = ex;
            _logger.LogError(ex, "Agent[{Type}] 流式调用模型失败: provider={Provider}, model={Model}, 耗时={Ms}ms。InnerException 含 HTTP 状态码与响应体。",
                agentType, provider, model, sw.ElapsedMilliseconds);
        }

        if (streamEx != null)
        {
            yield return new StreamChunk
            {
                Error = $"调用模型失败: {streamEx.Message}",
                FinishReason = "error"
            };
            yield break;
        }

        foreach (var c in chunks)
            yield return c;
    }

    public List<AgentTypeInfo> GetAgentTypes() => AgentTypeMeta.Values.ToList();

    // ==================== 私有方法 ====================

    /// <summary>
    /// 根据 agent 类型构建要执行的 Agent：
    /// - auto：构建总 agent，把三个子 agent 注册为工具做路由+汇总
    /// - 其余：归一化后直接构建对应的「一线子 agent」（可独立调用）
    /// </summary>
    private Agent BuildAgent(string agentType, string provider, string modelId, double temperature, int maxTokens)
    {
        if (agentType == "auto")
        {
            // 三个子 agent 各自使用隔离的 Kernel（仅挂自己的插件）
            var subAgents = new Agent[]
            {
                CreateSubAgent("general", _kernelProvider.GetKernel(provider, "general"), modelId, temperature, maxTokens),
                CreateSubAgent("sql_expert", _kernelProvider.GetKernel(provider, "sql_expert"), modelId, temperature, maxTokens),
                CreateSubAgent("chart_expert", _kernelProvider.GetKernel(provider, "chart_expert"), modelId, temperature, maxTokens),
                CreateSubAgent("ticket", _kernelProvider.GetKernel(provider, "ticket"), modelId, temperature, maxTokens)
            };
            return CreateOrchestrator(_kernelProvider.GetKernel(provider, "auto"), modelId, temperature, maxTokens, subAgents);
        }

        var type = NormalizeSubType(agentType);
        return CreateSubAgent(type, _kernelProvider.GetKernel(provider, type), modelId, temperature, maxTokens);
    }

    /// <summary>
    /// 归一化子 agent 类型：legacy "chat" 与未知/空值统一视为 general。
    /// </summary>
    private static string NormalizeSubType(string agentType) =>
        string.IsNullOrWhiteSpace(agentType) || agentType == "chat" ? "general" : agentType;

    /// <summary>
    /// 创建「一线子 agent」：按类型注入专属指令，其 Kernel 上只挂载该类型所需插件（隔离）。
    /// </summary>
    private ChatCompletionAgent CreateSubAgent(string type, SKKernel kernel, string modelId, double temperature, int maxTokens)
    {
        string instructions = type switch
        {
            "sql_expert" =>
                "你是 Wes 管理系统的 SQL 专家助手，负责查询数据库中的数据或为用户编写 SQL。\n\n" +
                "【三种工作模式】\n" +
                "1. 数据查询模式（默认）：当用户询问系统中的数据、统计、列表（如「有哪些角色」「多少用户」「统计订单」）时：\n" +
                "   - 必要时调用 get_table_schema 查询表结构；\n" +
                "   - 编写规范的只读 SELECT 查询并调用 execute_sql 执行；\n" +
                "   - 用自然语言清晰回答结果，禁止编造数据，所有数字必须来自 execute_sql 的真实返回；\n" +
                "   - 输出要求：只返回结果的中文说明，不要展示 SQL 代码块、不要附上执行的语句。\n" +
                "2. SQL 生成模式：仅当用户明确要求「写 SQL」「生成查询语句」「给我 SQL」「SQL 怎么写」时：\n" +
                "   - 只返回 SQL 代码块（用 ```sql 包裹），不执行、不返回数据；\n" +
                "   - 必要时仍可调用 get_table_schema 确保表名/字段正确。\n" +
                "3. 详细模式：当用户要求「详细」「带 SQL」「附上 SQL」「显示 SQL」「详细模式」，或用于调试、教学时：\n" +
                "   - 流程同模式 1（必要时查表结构 → 写 SQL → 调用 execute_sql 执行）；\n" +
                "   - 输出要求：先用 ```sql 代码块展示执行的 SQL，再用自然语言说明查询结果；\n" +
                "   - 禁止编造数据，结果必须来自 execute_sql 的真实返回。\n\n" +
                "【SQL 规范】关键字大写、每个子句独立成行并缩进；仅允许 SELECT / WITH 查询；表名/字段名以 get_table_schema 返回为准，不要臆测。\n" +
                "【异常处理】若 execute_sql 返回错误，请展示执行的 SQL 与错误信息便于排查，并尝试修正后重试。\n" +
                "请用中文简洁回答。",

            "ticket" =>
                "你是 Wes 管理系统的工单管理助手。\n" +
                "你可以帮助用户：\n" +
                "1. 创建新工单\n" +
                "2. 查询工单状态和列表\n" +
                "3. 更新工单状态\n" +
                "4. 从知识库查找工单相关的解决方案\n\n" +
                "对于不明确的信息（如优先级、分配人），请主动询问用户。",

            "chart_expert" =>
                "你是 Wes 管理系统的数据可视化助手，负责查询数据库并以 ECharts 图表形式展示结果。\n\n" +
                "工作流程：\n" +
                "1. 必要时调用 get_table_schema 查询表结构；\n" +
                "2. 编写规范的只读 SELECT 查询并调用 execute_sql 执行；\n" +
                "3. 基于 execute_sql 返回的真实数据，构造 ECharts option JSON；\n" +
                "4. 将 ECharts option JSON 用 ```echart 代码块输出。\n\n" +
                "【强制格式要求】\n" +
                "- 代码块语言标签固定为 echart：```echart\n" +
                "- 代码块内必须是合法的 ECharts option JSON（可直接 echarts.setOption 使用）；\n" +
                "- series 必须指定 type（bar/line/pie 等）；\n" +
                "- 用户只要提到「图」「图表」「趋势」「分布」「对比」「占比」，你都必须输出 ECharts JSON，禁止使用 mermaid、plantuml、svg、vega-lite 等任何其它图表语法；\n" +
                "- 数据必须来自 execute_sql 的真实结果，禁止编造。\n\n" +
                "【图表类型选择】\n" +
                "- 折线图、趋势变化 → line\n" +
                "- 柱状图、分类对比 → bar\n" +
                "- 饼图、占比构成 → pie（data 为 [{name, value}, ...]）\n\n" +
                "【ECharts 折线图示例】\n" +
                "```echart\n" +
                "{\n" +
                "  \"title\": {\"text\": \"最近一个月登录趋势\"},\n" +
                "  \"tooltip\": {\"trigger\": \"axis\"},\n" +
                "  \"xAxis\": {\"type\": \"category\", \"data\": [\"6月1日\", \"6月2日\", \"6月3日\"], \"boundaryGap\": false},\n" +
                "  \"yAxis\": {\"type\": \"value\"},\n" +
                "  \"series\": [{\"type\": \"line\", \"data\": [12, 18, 15], \"smooth\": true}]\n" +
                "}\n" +
                "```\n\n" +
                "【ECharts 柱状图示例】\n" +
                "```echart\n" +
                "{\n" +
                "  \"title\": {\"text\": \"角色登录次数对比\"},\n" +
                "  \"tooltip\": {\"trigger\": \"axis\"},\n" +
                "  \"xAxis\": {\"type\": \"category\", \"data\": [\"管理员\", \"运营\", \"访客\"]},\n" +
                "  \"yAxis\": {\"type\": \"value\"},\n" +
                "  \"series\": [{\"type\": \"bar\", \"data\": [42, 30, 12]}]\n" +
                "}\n" +
                "```\n\n" +
                "【ECharts 饼图示例】\n" +
                "```echart\n" +
                "{\n" +
                "  \"title\": {\"text\": \"登录状态分布\"},\n" +
                "  \"tooltip\": {\"trigger\": \"item\"},\n" +
                "  \"legend\": {\"orient\": \"vertical\", \"left\": \"left\"},\n" +
                "  \"series\": [{\n" +
                "    \"name\": \"登录状态\",\n" +
                "    \"type\": \"pie\",\n" +
                "    \"radius\": \"55%\",\n" +
                "    \"data\": [\n" +
                "      {\"name\": \"登录成功\", \"value\": 22},\n" +
                "      {\"name\": \"登录失败\", \"value\": 3}\n" +
                "    ]\n" +
                "  }]\n" +
                "}\n" +
                "```\n\n" +
                "【最终检查】在输出前检查：代码块标签必须是 echart，series.type 必须与用户需求对应（折线图=line、柱状图=bar、饼图=pie），且 JSON 合法。\n" +
                "【SQL 规范】关键字大写、子句换行缩进；仅允许 SELECT / WITH 查询。\n" +
                "【异常处理】若 execute_sql 报错，展示 SQL 与错误信息，不要强行生成图表。\n" +
                "请用中文简洁回答。",

            _ =>
                "你是 Wes 管理系统的通用 AI 助手，可以回答用户关于系统使用的各类问题、闲聊或通用咨询。\n" +
                "你不负责查询数据库或操作工单；若用户的问题涉及数据统计或工单，请直接说明应由对应的专业助手处理，或引导用户重新提问。\n" +
                "请用中文回复，保持简洁专业。"
        };

        // 为 SQL 助手注入数据库可用表名，帮助模型写出正确的表名
        if (type == "sql_expert")
        {
            var tableHint = GetTableListHint();
            if (!string.IsNullOrWhiteSpace(tableHint))
                instructions += "\n\n数据库可用表（部分）：\n" + tableHint;
        }

        var settings = new PromptExecutionSettings
        {
            ModelId = modelId,
            FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(),
            ExtensionData = new Dictionary<string, object>
            {
                ["temperature"] = temperature,
                ["max_tokens"] = maxTokens
            }
        };

        return new ChatCompletionAgent
        {
            Kernel = kernel,
            Instructions = instructions,
            Name = type, // 作为 agent-as-tool 时的函数名（general / sql_expert / ticket）
            Arguments = new KernelArguments(settings)
        };
    }

    /// <summary>
    /// 创建「总 agent」：把多个子 agent 包装成工具插件（Agent-as-Tool），
    /// 由模型根据意图选择调用合适的子 agent，并汇总其返回结果。
    /// </summary>
    private ChatCompletionAgent CreateOrchestrator(SKKernel kernel, string modelId, double temperature, int maxTokens, Agent[] subAgents)
    {
        const string pluginName = "routing";
        if (!kernel.Plugins.Contains(pluginName))
        {
            var plugin = AgentKernelPluginFactory.CreateFromAgents(
                pluginName,
                "子助手集合。根据用户问题选择合适的子助手作答，必要时可组合多个子助手的结果后汇总。",
                subAgents);
            kernel.Plugins.Add(plugin);
        }

        var instructions =
            "你是 Wes 管理系统的智能路由助手。你本身不查询数据库、不创建工单，而是根据用户意图调用合适的子助手获取答案，再汇总后用中文清晰地回复用户。\n\n" +
            "子助手及其适用场景：\n" +
            "- general：通用对话、闲聊、系统使用咨询等一般性问题；\n" +
            "- sql_expert：涉及系统内的数据统计、列表查询、数据库内容（用户数、订单数等），以文字形式回答；\n" +
            "- chart_expert：需要以图表可视化展示数据（如「画个图」「柱状图」「趋势图」「分布」「占比」等）；\n" +
            "- ticket：涉及工单的创建、查询、更新与解决方案查找。\n\n" +
            "路由原则：\n" +
            "1. 仅当用户问题明确属于上述某一类时，才调用对应的子助手；\n" +
            "2. 用户要求图表/可视化时优先调用 chart_expert，而非 sql_expert；\n" +
            "3. 一个问题可能同时涉及多个领域，可在一次回复中组合多个子助手的结果；\n" +
            "4. 不要编造数据，所有事实必须来自子助手返回的真实内容；\n" +
            "5. 若子助手返回了错误信息或无法处理，如实告知用户。";

        var settings = new PromptExecutionSettings
        {
            ModelId = modelId,
            FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(),
            ExtensionData = new Dictionary<string, object>
            {
                ["temperature"] = temperature,
                ["max_tokens"] = maxTokens
            }
        };

        return new ChatCompletionAgent
        {
            Kernel = kernel,
            Instructions = instructions,
            Name = "WesAgent_auto",
            Arguments = new KernelArguments(settings)
        };
    }

    /// <summary>
    /// 获取数据库表名列表，注入到 SQL 子助手的系统提示中
    /// </summary>
    private string GetTableListHint()
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ISqlSugarClient>();
            var tables = db.DbMaintenance.GetTableInfoList()
                .Select(t => t.Name)
                .OrderBy(n => n)
                .ToList();
            return tables.Count == 0 ? "" : string.Join(", ", tables);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "获取数据库表列表失败，SQL 助手将缺少表名上下文");
            return "";
        }
    }

    /// <summary>
    /// 构建 ChatHistory（系统指令由 ChatCompletionAgent.Instructions 负责注入，这里不再重复添加）
    /// </summary>
    private static ChatHistory BuildHistory(AiAgentRequest request)
    {
        var history = new ChatHistory();

        if (request.History != null)
        {
            foreach (var msg in request.History)
            {
                AuthorRole? role = msg.Role?.ToLowerInvariant() switch
                {
                    "user" => AuthorRole.User,
                    "assistant" => AuthorRole.Assistant,
                    "system" => AuthorRole.System,
                    "tool" => AuthorRole.Tool,
                    _ => null
                };
                if (role.HasValue)
                    history.Add(new ChatMessageContent(role.Value, msg.Content));
            }
        }

        history.AddUserMessage(request.Message);
        return history;
    }
}
