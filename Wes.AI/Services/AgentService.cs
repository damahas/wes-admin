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
using Wes.AI.Context;
using Wes.AI.Kernel;
using Wes.AI.Models.Entity;
using Wes.AI.Models.ViewModel;
using Wes.AI.Plugins;
using SqlSugar;

namespace Wes.AI.Services;

/// <summary>
/// Agent 服务：使用 SK ChatCompletionAgent + Function Calling
///
/// 架构（调度 agent + 功能 agent，对外统一身份「小W」）：
/// - 四个「功能 agent」各自独立、可单独调用，且插件相互隔离：
///     · general   通用对话，Kernel 不挂载任何领域插件
///     · sql_expert 数据库查询，Kernel 仅挂载 sql + knowledge，并注入表名提示
///     · ticket    工单管理，Kernel 仅挂载 ticket + knowledge
///   （SQL 插件只在 sql_expert 的 Kernel 上，其他 agent 根本看不到 execute_sql，根治「什么都带 SQL」）
/// - 一个「调度 agent」（auto）：把上述功能 agent 注册成工具（Agent-as-Tool），
///   由模型按意图选择调用哪个功能 agent，必要时组合多个功能 agent 的结果后汇总返回。
///   前端默认走 auto，对用户而言始终是同一个「小W」；同时每个功能 agent 也可通过 /ai/agent/{type} 单独调用。
///
/// 命名约定：内部仍按 type 区分子 agent，但对外（DisplayName 与系统提示中的自称）一律为「小W」，
/// 回复中不出现「XX助手/子助手/专家」等称谓，也不暴露内部分工与调度过程。
///
/// SK ChatCompletionAgent 内置自动 Function Calling 循环：产生 tool_calls 时由 Agent 负责
/// 调用插件/子 agent 并把结果回灌，直到产出最终文本。
/// </summary>
public class AgentService : IAgentService
{
    private readonly KernelProvider _kernelProvider;
    private readonly AiTurnStore _turnStore;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<AgentService> _logger;

    /// <summary>
    /// 对外统一身份名：调度 agent 与所有功能 agent 都叫「小W」，用户不感知内部分工。
    /// </summary>
    private const string AgentName = "小W";

    /// <summary>
    /// 所有 agent 共用的身份与对外口径声明（拼在具体角色指令之前）。
    /// </summary>
    private const string PersonaInstruction =
        "你是 Wes 管理系统的 AI 助理，代号「" + AgentName + "」，用中文回答，简洁专业。\n" +
        "【统一身份】任何回复都只代表「" + AgentName + "」，自称「" + AgentName + "」或「我」；" +
        "禁止出现「XX助手/专家/子助手」等称谓，禁止透露内部角色代号与调度分工过程；" +
        "被问身份时答：我是 " + AgentName + "，Wes 管理系统的 AI 助理。\n";

    /// <summary>
    /// 调度 agent（auto）的职责指令（不含统一身份前缀）。
    /// </summary>
    private const string OrchestratorRoleInstruction =
        "【职责】调度（内部代号 auto，对用户不可见）：不直接查库、不直接建单，按意图调用内部能力，再以「小W」身份用中文汇总。\n" +
        "【能力映射】general=通用问答/闲聊/使用咨询；sql_expert=统计、列表、查数据（文字回答）；" +
        "chart_expert=图表可视化（图/趋势/占比/对比）；ticket=工单创建、查询、更新与方案查找。\n" +
        "【原则】①仅当意图明确时调用，一次可组合多个能力；②要图表优先 chart_expert，而非 sql_expert；" +
        "③所有数据必须来自能力返回，禁止编造；④能力报错可换能力或修正后重试 1 次，仍失败则如实告知；" +
        "⑤回复中不出现能力名、工具名与调用过程。\n" +
        "【图表保真】能力返回的代码块若含 ```echart 标签，你必须原样、完整地放进最终回复：标签保持 echart，"
        + "块内 JSON 一个字符都不得改写（不得增删字段、不得重排），代码块标签不得改成 json / mermaid / chart 等任何其它形式；"
        + "并禁止把图表数据改用文字、表格或图片描述重述一遍——那样用户会看不到图。输出前自检：回复中必须存在且仅存在该 "
        + "```echart 代码块；若自检发现丢失或被改写，必须重新输出一次，直接输出原代码块。\n" +
        "【上下文】历史中可能包含一条「早期对话摘要」，它是较早对话的压缩结果，只能用于保持连贯，不要当作工具返回的真实数据。\n";

    // Agent 类型元数据（调度 agent auto 与各功能 agent）；
    // 对外展示名统一为「小W」，用 Description 区分各自能力
    private static readonly Dictionary<string, AgentTypeInfo> AgentTypeMeta = new()
    {
        ["auto"] = new() { Type = "auto", DisplayName = AgentName, Description = "默认入口：自动识别意图（数据查询/图表/工单/通用）并作答" },
        ["general"] = new() { Type = "general", DisplayName = AgentName, Description = "通用对话：系统使用咨询与闲聊，不挂载任何数据库/工单工具" },
        ["sql_expert"] = new() { Type = "sql_expert", DisplayName = AgentName, Description = "数据查询：生成并执行只读 SQL，以文字形式回答" },
        ["chart_expert"] = new() { Type = "chart_expert", DisplayName = AgentName, Description = "数据可视化：查询数据并输出 ECharts 图表配置 JSON" },
        ["ticket"] = new() { Type = "ticket", DisplayName = AgentName, Description = "工单服务：创建、查询、更新工单与解决方案查找" }
    };

    public AgentService(
        KernelProvider kernelProvider,
        AiTurnStore turnStore,
        IServiceProvider serviceProvider,
        ILogger<AgentService> logger)
    {
        _kernelProvider = kernelProvider;
        _turnStore = turnStore;
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
        var (history, usage) = BuildChatHistory(agentType, provider, model, request);

        _logger.LogInformation("Agent[{Type}] 开始调用模型: provider={Provider}, model={Model}, 消息总数={MsgCount}, 历史条数={HistoryCount}, temperature={Temp}, maxTokens={Max}, 上下文用量={Used}/{MaxCtx}",
            agentType, provider, model, history.Count, request.History?.Count ?? 0, request.Temperature, request.MaxTokens,
            usage.UsedTokens, usage.MaxContext);

        var sb = new StringBuilder();
        // 模型真实 token：一轮提问可能因工具调用产生多次模型请求，这里累加所有响应的 usage
        var promptTokens = 0;
        var completionTokens = 0;
        var hasUsage = false;
        Exception? callEx = null;
        try
        {
            await foreach (var resp in agent.InvokeAsync(history, cancellationToken: ct))
            {
                var (input, output) = ReadTokenUsage(resp.Message.Metadata);
                if (input.HasValue || output.HasValue)
                {
                    hasUsage = true;
                    promptTokens += input ?? 0;
                    completionTokens += output ?? 0;
                }

                if (!string.IsNullOrEmpty(resp.Message.Content))
                    sb.Append(resp.Message.Content);
            }
            sw.Stop();
            _logger.LogInformation("Agent[{Type}] 调用成功: provider={Provider}, model={Model}, 耗时={Ms}ms, 返回长度={Len}, token(入/出)={In}/{Out}{Estimated}",
                agentType, provider, model, sw.ElapsedMilliseconds, sb.Length, promptTokens, completionTokens,
                hasUsage ? "" : "（模型未返回，展示用估算）");
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

        // 记录本轮实测指标，供前端保存消息时落库（非流式接口同样生效）
        if (request.SessionId is > 0)
            _turnStore.Set(request.SessionId.Value, new TurnMetrics
            {
                ElapsedMs = sw.ElapsedMilliseconds,
                PromptTokens = hasUsage ? promptTokens : null,
                CompletionTokens = hasUsage ? completionTokens : null,
                Estimated = !hasUsage,
                UsageSource = hasUsage ? TokenUsageSource.Model : TokenUsageSource.Estimate
            });

        return new AiAgentResponse
        {
            AgentType = agentType,
            Provider = provider,
            Model = model,
            Content = sb.ToString(),
            DurationMs = sw.ElapsedMilliseconds,
            PromptTokens = hasUsage ? promptTokens : null,
            CompletionTokens = hasUsage ? completionTokens : null,
            TokensEstimated = !hasUsage,
            UsageSource = hasUsage ? TokenUsageSource.Model : TokenUsageSource.Estimate,
            Usage = usage
        };
    }

    public async IAsyncEnumerable<StreamChunk> ExecuteStreamAsync(
        string agentType, AiAgentRequest request,
        [EnumeratorCancellation] CancellationToken ct = default)
    {
        var provider = _kernelProvider.ResolveProviderName(request.Provider);
        var model = _kernelProvider.GetModelId(request.Provider, request.Model);

        var agent = BuildAgent(agentType, provider, model, request.Temperature, request.MaxTokens);
        var (history, usage) = BuildChatHistory(agentType, provider, model, request);

        var sw = Stopwatch.StartNew();
        _logger.LogInformation("Agent[{Type}] 开始流式调用模型: provider={Provider}, model={Model}, 消息总数={MsgCount}, 历史条数={HistoryCount}, 会话={SessionId}, 上下文用量={Used}/{MaxCtx}",
            agentType, provider, model, history.Count, request.History?.Count ?? 0, request.SessionId, usage.UsedTokens, usage.MaxContext);

        // 先收集所有分块再统一 yield，避免 yield 出现在带 catch 的 try 块内（C# 不允许）
        var chunks = new List<StreamChunk>();
        // 模型真实 token：部分连接器会在流末尾（或每个片段）附带 usage，能拿到就累加
        var promptTokens = 0;
        var completionTokens = 0;
        var hasUsage = false;
        // 首字延迟（收到模型第一个片段的时间）与 token 探测耗时，用于定位慢在哪一段
        long? firstTokenMs = null;
        long probeMs = 0;
        Exception? streamEx = null;
        try
        {
            await foreach (var chunk in agent.InvokeStreamingAsync(history, cancellationToken: ct))
            {
                firstTokenMs ??= sw.ElapsedMilliseconds;

                var (input, output) = ReadTokenUsage(chunk.Message.Metadata);
                if (input.HasValue || output.HasValue)
                {
                    hasUsage = true;
                    promptTokens += input ?? 0;
                    completionTokens += output ?? 0;
                }

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
            _logger.LogInformation("Agent[{Type}] 流式调用成功: provider={Provider}, model={Model}, 耗时={Ms}ms, 块数={Count}, token(入/出)={In}/{Out}{Estimated}",
                agentType, provider, model, sw.ElapsedMilliseconds, chunks.Count, promptTokens, completionTokens,
                hasUsage ? "" : "（模型未返回，展示用估算）");
        }
        catch (Exception ex)
        {
            sw.Stop();
            streamEx = ex;
            _logger.LogError(ex, "Agent[{Type}] 流式调用模型失败: provider={Provider}, model={Model}, 耗时={Ms}ms。InnerException 含 HTTP 状态码与响应体。",
                agentType, provider, model, sw.ElapsedMilliseconds);
        }

        var elapsed = sw.ElapsedMilliseconds;

        if (streamEx != null)
        {
            // 失败轮次也暂存耗时，前端 onError 保存消息时一并落库
            if (request.SessionId is > 0)
                _turnStore.Set(request.SessionId.Value, new TurnMetrics
                {
                    ElapsedMs = elapsed,
                    Estimated = true,
                    UsageSource = TokenUsageSource.Estimate
                });

            yield return new StreamChunk
            {
                Error = $"调用模型失败: {streamEx.Message}",
                FinishReason = "error",
                ElapsedMs = elapsed
            };
            yield break;
        }

        // 先把正文全量推送给前端，后续的 token 探测不占用首字时间
        foreach (var c in chunks)
            yield return c;

        // 流式拿不到 usage 时，用一次 max_tokens=1 的非流式探测请求取回真实输入 token
        // （SK 的 OpenAI 连接器未实现 stream_options.include_usage，流式块里没有 usage）。
        // 探测成功：输入为实测值、输出仍为本地估算（半真实）；失败：入/出都回退估算。
        var usageSource = TokenUsageSource.Estimate;
        if (hasUsage)
        {
            usageSource = TokenUsageSource.Model;
        }
        else
        {
            var probeSw = Stopwatch.StartNew();
            var probed = await ProbeInputTokensAsync(agent, provider, agentType, history, ct);
            probeMs = probeSw.ElapsedMilliseconds;
            if (probed is > 0)
            {
                promptTokens = probed.Value;
                completionTokens = ContextCompressor.EstimateTokens(AnswerOf(chunks));
                usageSource = TokenUsageSource.Probe;
            }
        }

        // 记录本轮实测指标：前端保存消息时由 SessionService 取走落库，前端无需回传
        if (request.SessionId is > 0)
        {
            _turnStore.Set(request.SessionId.Value, new TurnMetrics
            {
                ElapsedMs = elapsed,
                PromptTokens = usageSource == TokenUsageSource.Estimate ? null : promptTokens,
                CompletionTokens = usageSource == TokenUsageSource.Estimate ? null : completionTokens,
                Estimated = usageSource != TokenUsageSource.Model,
                UsageSource = usageSource
            });
            _logger.LogInformation("Agent[{Type}] 指标已暂存: 会话={SessionId}, 耗时={Elapsed}ms, 来源={Source}",
                agentType, request.SessionId.Value, elapsed, usageSource);
        }
        else
        {
            _logger.LogWarning("Agent[{Type}] 未拿到会话ID（前端未传 sessionId），本轮耗时不会落库", agentType);
        }

        _logger.LogInformation("Agent[{Type}] 本轮耗时分布: 总计={Total}ms, 首字={First}ms, 生成={Gen}ms, token探测={Probe}ms, 块数={Count}",
            agentType, elapsed, firstTokenMs, firstTokenMs == null ? 0 : elapsed - firstTokenMs, probeMs, chunks.Count);

        // 末尾回传本次上下文用量（含是否压缩）、服务端实测耗时与真实 token，供前端展示
        yield return new StreamChunk
        {
            FinishReason = "stop",
            Usage = usage,
            ElapsedMs = elapsed,
            PromptTokens = usageSource == TokenUsageSource.Estimate ? null : promptTokens,
            CompletionTokens = usageSource == TokenUsageSource.Estimate ? null : completionTokens,
            TokensEstimated = usageSource != TokenUsageSource.Model,
            UsageSource = usageSource
        };
    }

    /// <summary>拼接流式分块得到完整回答文本</summary>
    private static string AnswerOf(List<StreamChunk> chunks)
    {
        var sb = new StringBuilder();
        foreach (var c in chunks)
            if (!string.IsNullOrEmpty(c.Content)) sb.Append(c.Content);
        return sb.ToString();
    }

    /// <summary>
    /// 探测真实输入 token：流式拿不到 usage 时，补发一次 max_tokens=1 的非流式请求，
    /// 从响应 usage 中取模型/网关实际计算的 prompt_tokens（输出只生成 1 个 token，几乎不产生费用）。
    ///
    /// 局限：探测只会带上系统指令与本次 history，auto 模式下子 agent 的调用发生在各自独立的
    /// ChatHistory 里，这部分轮次不计入，因此探测值 ≤ 真实累计输入（无工具调用时基本相等）。
    /// </summary>
    private async Task<int?> ProbeInputTokensAsync(
        ChatCompletionAgent agent, string provider, string agentType, ChatHistory history, CancellationToken ct)
    {
        try
        {
            var kernel = agent.Kernel ?? _kernelProvider.GetKernel(provider, NormalizeSubType(agentType));

            // SK 可能把 chat service 按 serviceId 注册为 keyed service，非 keyed 解析会失败，
            // 这里先按常规解析，失败再取全部注册项中的第一个
            IChatCompletionService? chat;
            try
            {
                chat = kernel.GetRequiredService<IChatCompletionService>();
            }
            catch (Exception)
            {
                chat = kernel.GetAllServices<IChatCompletionService>().FirstOrDefault();
            }
            if (chat == null)
            {
                _logger.LogWarning("Agent[{Type}] token 探测跳过：kernel 上未找到 IChatCompletionService", agentType);
                return null;
            }

            // 系统指令由 Agent 持有（不在 history 里），探测时必须补上，否则漏算指令与工具定义部分
            var probe = new ChatHistory();
            if (!string.IsNullOrWhiteSpace(agent.Instructions))
                probe.AddSystemMessage(agent.Instructions);
            foreach (var m in history) probe.Add(m);

            // 与 Agent 保持一致开启工具（工具 schema 会计入 prompt），但只允许生成 1 个 token
            var settings = new PromptExecutionSettings
            {
                FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(),
                ExtensionData = new Dictionary<string, object>
                {
                    ["max_tokens"] = 1,
                    ["temperature"] = 0
                }
            };

            var result = await chat.GetChatMessageContentsAsync(probe, settings, kernel, ct);
            var meta = result.Count > 0 ? result[0].Metadata : null;
            var (input, output) = ReadTokenUsage(meta);
            if (input is > 0 || output is > 0)
            {
                _logger.LogInformation("Agent[{Type}] token 探测成功: provider={Provider}, 真实输入 token={In}, 输出={Out}",
                    agentType, provider, input, output);
                return input;
            }

            _logger.LogWarning("Agent[{Type}] token 探测未拿到 usage: provider={Provider}, 消息数={Count}, metadataKeys={Keys}",
                agentType, provider, result.Count, meta == null ? "(null)" : string.Join(",", meta.Keys));
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Agent[{Type}] token 探测失败，回退本地估算", agentType);
            return null;
        }
    }

    /// <summary>
    /// 从 SK 消息的 Metadata["Usage"] 中读取模型返回的真实 token（输入 / 输出）。
    /// usage 类型随连接器与 SDK 版本变化（OpenAI SDK 2.x 为 ChatTokenUsage：InputTokens/OutputTokens/TotalTokens，
    /// 早期版本为 PromptTokens/CompletionTokens），这里按属性名反射取值，避免强依赖具体包类型。
    /// 模型/网关未返回 usage 时返回 (null, null)，调用方回退本地估算。
    /// </summary>
    private static (int? input, int? output) ReadTokenUsage(IReadOnlyDictionary<string, object?>? metadata)
    {
        if (metadata == null || metadata.Count == 0)
            return (null, null);

        // 主流连接器把 usage 放在 "Usage" 键下
        if (metadata.TryGetValue("Usage", out var usageObj) && usageObj != null)
        {
            var r = ReadFrom(usageObj);
            if (r.input is > 0 || r.output is > 0) return r;
        }

        // 兜底：部分连接器/网关把 token 对象放在其它键（或键名不同），扫描所有 metadata 值
        foreach (var v in metadata.Values)
        {
            if (v == null || ReferenceEquals(v, usageObj)) continue;
            var r = ReadFrom(v);
            if (r.input is > 0 || r.output is > 0) return r;
        }

        return (null, null);
    }

    /// <summary>从 usage 对象上按属性名反射取输入/输出 token（兼容 ChatTokenUsage 与各厂商命名）</summary>
    private static (int? input, int? output) ReadFrom(object usageObj)
    {
        var type = usageObj.GetType();

        int? Read(params string[] names)
        {
            foreach (var name in names)
            {
                var prop = type.GetProperty(name);
                if (prop == null) continue;
                var v = prop.GetValue(usageObj);
                if (v is int i) return i;
                if (v is long l) return (int)l;
            }
            return null;
        }

        var input = Read("InputTokens", "PromptTokens", "InputTokenCount", "PromptTokenCount");
        var output = Read("OutputTokens", "CompletionTokens", "OutputTokenCount", "CompletionTokenCount");
        return (input, output);
    }

    public List<AgentTypeInfo> GetAgentTypes() => AgentTypeMeta.Values.ToList();

    // ==================== 私有方法 ====================

    /// <summary>
    /// 根据 agent 类型构建要执行的 Agent：
    /// - auto：构建调度 agent，把各功能 agent 注册为工具做路由+汇总
    /// - 其余：归一化后直接构建对应的「功能 agent」（可独立调用，对外同样是「小W」）
    /// </summary>
    private ChatCompletionAgent BuildAgent(string agentType, string provider, string modelId, double temperature, int maxTokens)
    {
        if (agentType == "auto")
        {
            // 各功能 agent 各自使用隔离的 Kernel（仅挂自己的插件）
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
    /// 各功能 agent 的角色职责指令（不含统一身份前缀，便于单独估算 token 开销）。
    /// </summary>
    private static string RoleInstructions(string type) => type switch
    {
            "sql_expert" =>
                "【职责】数据查询与 SQL 编写（内部代号 sql_expert，对用户不可见）。\n" +
                "【工具】get_table_schema(表名) 查列结构；execute_sql(sql) 执行只读查询（仅 SELECT/WITH，最多返回 50 行）；search_knowledge 查知识库文档。\n" +
                "【流程】表名或字段不确定 → 先 get_table_schema → 写 SQL → execute_sql → 用中文说明结果；表名以系统提示中的可用表为准，禁止臆测。\n" +
                "【输出】默认只给中文结论，不展示 SQL；用户明确要 SQL 时只给 ```sql 代码块（不执行）；用户要「详细/带 SQL」时先给 ```sql 再给结论。\n" +
                "【SQL 规范】关键字大写、子句换行缩进；只查必要字段，能聚合就聚合，大表加 LIMIT；金额/数量保留原始单位。\n" +
                "【禁止】编造任何数字，所有数字必须来自 execute_sql 的返回；禁止写操作与 DDL。\n" +
                "【报错】展示 SQL 与错误信息，修正后最多重试 1 次；结果为空时说明可能原因（条件过严、无数据、表名不符）。",

            "ticket" =>
                "【职责】工单服务（内部代号 ticket，对用户不可见）。\n" +
                "【工具】create_ticket(标题, 描述, 优先级 low|medium|high|urgent, 受理人ID)、query_tickets(过滤条件, 条数)、update_ticket_status(工单ID, 状态 pending|in_progress|resolved|closed, 备注)、search_knowledge(查知识库方案)。\n" +
                "【流程】创建前确认标题、描述、优先级、受理人，缺失先向用户确认；查询/更新后给出工单号、状态与关键信息的中文摘要。\n" +
                "【约束】状态流转 pending→in_progress→resolved→closed；结论以工具返回为准，禁止编造工单号与结果；用户要解决方案时先查知识库。\n" +
                "【报错】工具失败时如实说明原因，并给出下一步建议。",

            "chart_expert" =>
                "【职责】数据可视化（内部代号 chart_expert，对用户不可见）：查数据并输出 ECharts option。\n" +
                "【工具】get_table_schema(表名)、execute_sql(只读 SELECT/WITH，最多 50 行)、search_knowledge。\n" +
                "【流程】不确定表/字段 → get_table_schema（表名以系统提示中的可用表为准，禁止臆测）→ execute_sql 取数 → 基于真实数据构造 option → 以 ```echart 代码块输出（块外只写一句中文结论）。\n" +
                "【格式】标签固定为 echart；内容必须是合法 JSON，可直接 echarts.setOption；series 必填 type；一次只输出一个代码块。\n" +
                "【选型】趋势/变化→line；分类对比→bar；占比构成→pie（data=[{name,value}]）；多系列→同图多个 series。\n" +
                "【禁止】mermaid、plantuml、svg、vega-lite 等其它图表语法；编造数据；无数据时强行出图。\n" +
                "【示例】```echart\n{\"title\":{\"text\":\"最近登录趋势\"},\"tooltip\":{\"trigger\":\"axis\"},\"xAxis\":{\"type\":\"category\",\"data\":[\"6/1\",\"6/2\"],\"boundaryGap\":false},\"yAxis\":{\"type\":\"value\"},\"series\":[{\"type\":\"line\",\"data\":[12,18],\"smooth\":true}]}\n```\n" +
                "【报错】execute_sql 出错时展示 SQL 与错误信息，不要强行生成图表。",

            _ =>
                "【职责】通用问答（内部代号 general，对用户不可见）：系统功能与使用方法咨询、概念解释、闲聊；不挂载数据库与工单工具。\n" +
                "【边界】涉及数据统计、列表查询、工单操作时，以「小W」的身份直接说明暂未处理该请求，并引导用户重新提问。\n" +
                "【要求】中文、简洁、必要时分点；不确定的内容不编造，说明还需要哪些信息才能回答。"
    };

    /// <summary>
    /// 创建「功能 agent」：统一身份 + 按类型注入专属职责指令，其 Kernel 上只挂载该类型所需插件（隔离）。
    /// </summary>
    private ChatCompletionAgent CreateSubAgent(string type, SKKernel kernel, string modelId, double temperature, int maxTokens)
    {
        string instructions = PersonaInstruction + RoleInstructions(type);

        // 为需要查库的 agent 注入数据库可用表名，帮助模型写出正确的表名
        if (type is "sql_expert" or "chart_expert")
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
    /// 创建「调度 agent」：把各功能 agent 包装成工具插件（Agent-as-Tool），
    /// 由模型根据意图选择调用合适的功能 agent，并汇总其返回结果；对外统一以「小W」身份作答。
    /// </summary>
    private ChatCompletionAgent CreateOrchestrator(SKKernel kernel, string modelId, double temperature, int maxTokens, Agent[] subAgents)
    {
        const string pluginName = "routing";
        if (!kernel.Plugins.Contains(pluginName))
        {
            var plugin = AgentKernelPluginFactory.CreateFromAgents(
                pluginName,
                "内部能力集合（对用户不可见）。根据用户问题选择合适的内部能力作答，必要时可组合多个能力的结果后汇总。",
                subAgents);
            kernel.Plugins.Add(plugin);
        }

        var instructions = PersonaInstruction + OrchestratorRoleInstruction;

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
    /// 获取数据库表名列表，注入到数据查询 Agent 的系统提示中
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
            _logger.LogWarning(ex, "获取数据库表列表失败，数据查询 Agent 将缺少表名上下文");
            return "";
        }
    }

    /// <summary>
    /// 构建 ChatHistory（系统指令由 ChatCompletionAgent.Instructions 负责注入，这里不再重复添加）
    /// </summary>
    /// <summary>
    /// 构建送入模型的 ChatHistory：
    /// 先按「模型最大上下文 - 系统提示 - 输出预留」的预算压缩历史，
    /// 再拼上本次用户消息。返回压缩后的用量，供接口回传给前端展示。
    /// </summary>
    private (ChatHistory History, ContextUsage Usage) BuildChatHistory(
        string agentType, string provider, string model, AiAgentRequest request)
    {
        var maxContext = _kernelProvider.GetContextLimit(provider, model);
        var systemTokens = EstimateSystemTokens(agentType);
        var result = ContextCompressor.Compress(
            request.History, systemTokens, request.MaxTokens, maxContext, request.Message);

        var history = new ChatHistory();
        foreach (var msg in result.Messages)
        {
            AuthorRole? role = msg.Role?.ToLowerInvariant() switch
            {
                "user" => AuthorRole.User,
                "assistant" => AuthorRole.Assistant,
                "system" => AuthorRole.System,
                // 工具消息缺少 tool_call_id，回放会变成孤儿消息导致接口报错，直接丢弃
                _ => null
            };
            if (role.HasValue && !string.IsNullOrWhiteSpace(msg.Content))
                history.Add(new ChatMessageContent(role.Value, msg.Content));
        }

        history.AddUserMessage(request.Message);

        if (result.Usage.Compressed)
            _logger.LogInformation("上下文已压缩: agent={Type}, 丢弃/折叠={Dropped}条, 截断={Truncated}条, 用量={Used}/{Max}",
                agentType, result.Usage.DroppedMessages, result.Usage.TruncatedMessages,
                result.Usage.UsedTokens, result.Usage.MaxContext);

        return (history, result.Usage);
    }

    /// <summary>
    /// 估算系统提示与工具定义的 token 开销（用于计算历史可用预算）。
    /// </summary>
    private static int EstimateSystemTokens(string agentType)
    {
        if (agentType == "auto")
        {
            // 调度 agent：自身指令 + 4 个功能 agent 的指令 + 路由插件与工具 schema
            var tokens = ContextCompressor.EstimateTokens(PersonaInstruction) * 5
                         + ContextCompressor.EstimateTokens(OrchestratorRoleInstruction);
            foreach (var sub in new[] { "general", "sql_expert", "chart_expert", "ticket" })
                tokens += ContextCompressor.EstimateTokens(RoleInstructions(sub));
            return tokens + 2000;
        }

        var type = NormalizeSubType(agentType);
        var total = ContextCompressor.EstimateTokens(PersonaInstruction)
                    + ContextCompressor.EstimateTokens(RoleInstructions(type))
                    + 600; // 工具定义 schema
        if (type is "sql_expert" or "chart_expert")
            total += 800; // 注入的数据库表名列表
        return total;
    }
}
