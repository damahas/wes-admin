using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using SqlSugar;
using Wes.AI.Context;
using Wes.AI.Models.Entity;
using Wes.AI.Models.Enum;
using Wes.AI.Models.ViewModel;
using Wes.AI.Plugins;
using SKKernel = Microsoft.SemanticKernel.Kernel;

namespace Wes.AI.Kernel;

/// <summary>
/// SK Kernel 管理器：按提供商从数据库（ai_model）创建并缓存 Kernel 实例。
/// 关键改造：Kernel 按 (提供商, agent 类型) 维度缓存，且只挂载该 agent 类型所需的插件，
/// 从而实现「不同 agent 插件相互隔离」——例如 SQL 插件仅存在于 sql_expert 的 Kernel 上，
/// 调度 agent（auto）与通用 agent（general）的 Kernel 上看不到任何数据库工具。
/// </summary>
public class KernelProvider
{
    private readonly ISqlSugarClient _db;
    private readonly ILoggerFactory _loggerFactory;
    private readonly IServiceProvider _services;
    private readonly ConcurrentDictionary<string, SKKernel> _kernels = new();

    public KernelProvider(ISqlSugarClient db, ILoggerFactory loggerFactory, IServiceProvider services)
    {
        _db = db;
        _loggerFactory = loggerFactory;
        _services = services;
    }

    /// <summary>
    /// 获取指定提供商的 Kernel（按 提供商:agent 类型 缓存，线程安全）。
    /// agentType 为空时返回不带任何领域插件的纯对话 Kernel。
    /// </summary>
    public SKKernel GetKernel(string? providerName = null, string? agentType = null)
    {
        var name = ResolveProviderName(providerName);
        var key = string.IsNullOrEmpty(agentType) ? name : $"{name}:{agentType}";
        return _kernels.GetOrAdd(key, _ => BuildKernel(name, agentType));
    }

    /// <summary>
    /// 失效 Kernel 缓存。
    /// ai_model 变更（modelId / baseUrl / apiKey / provider / 默认项）后必须调用：
    /// Kernel 在构建时即固化了 endpoint 与默认 modelId，不清缓存会导致「新模型名 + 旧地址」，
    /// 表现为调用失败 HTTP 404 model_not_found。
    /// providerName 为空时清空全部（删除、默认项变更等影响面不确定的场景用）。
    /// </summary>
    public void Invalidate(string? providerName = null)
    {
        if (string.IsNullOrWhiteSpace(providerName))
        {
            _kernels.Clear();
            return;
        }

        var prefix = providerName.Trim();
        var keys = _kernels.Keys
            .Where(k => k == prefix || k.StartsWith(prefix + ":", StringComparison.Ordinal))
            .ToList();

        foreach (var key in keys)
            _kernels.TryRemove(key, out _);
    }

    /// <summary>
    /// 解析提供商名称（未指定 / 不存在时，回退到全局默认模型所在的提供商）
    /// </summary>
    public string ResolveProviderName(string? providerName)
    {
        if (!string.IsNullOrWhiteSpace(providerName) && ProviderExists(providerName))
            return providerName;

        var def = _db.Queryable<AiModelEntity>().First(m => m.IsDefault);
        return def?.Provider ?? providerName ?? "";
    }

    private bool ProviderExists(string name) =>
        _db.Queryable<AiModelEntity>().Any(m => m.Provider == name);

    /// <summary>
    /// 获取提供商配置（返回该提供商的默认模型行，含 apiKey / baseUrl / modelId）
    /// </summary>
    public AiModelEntity GetProviderConfig(string? providerName = null)
    {
        var name = ResolveProviderName(providerName);
        // 优先取已配置 ApiKey 的行，避免默认行密钥为空导致调用 401
        var row = _db.Queryable<AiModelEntity>()
                .Where(m => m.Provider == name && !string.IsNullOrEmpty(m.ApiKey))
                .OrderBy(m => m.IsDefault ? 0 : 1)
                .First()
            ?? _db.Queryable<AiModelEntity>()
                .Where(m => m.Provider == name)
                .OrderBy(m => m.IsDefault ? 0 : 1)
                .First();
        return row ?? throw new InvalidOperationException($"Provider '{name}' not configured.");
    }

    /// <summary>
    /// 获取当前有效的模型 ID
    /// </summary>
    public string GetModelId(string? providerName = null, string? modelOverride = null)
    {
        if (!string.IsNullOrWhiteSpace(modelOverride))
            return modelOverride;

        // 未指定提供商：直接用全局默认模型（其 Provider 即为默认提供商）
        if (string.IsNullOrWhiteSpace(providerName))
        {
            var global = _db.Queryable<AiModelEntity>().First(m => m.IsDefault);
            if (global != null) return global.ModelId;
        }

        // 指定了提供商：该提供商下 IsDefault 优先，否则取排序最靠前的一条
        var name = ResolveProviderName(providerName);
        var row = _db.Queryable<AiModelEntity>()
            .Where(m => m.Provider == name)
            .OrderBy(m => m.IsDefault ? 0 : 1)
            .OrderBy(m => m.Sort)
            .First();
        return row?.ModelId ?? "";
    }

    /// <summary>
    /// 获取所有提供商列表（按 provider 分组）
    /// </summary>
    public List<ProviderInfo> GetProviderList()
    {
        // 仅返回已配置 ApiKey 的提供商，未配置密钥的模型对前端不可用
        var rows = _db.Queryable<AiModelEntity>()
            .Where(m => !string.IsNullOrEmpty(m.ApiKey))
            .ToList();
        // 默认提供商优先取全局默认模型所在的提供商；
        // 若默认行未配置密钥（不在 rows 中），则回退到排序最靠前的一家，
        // 保证列表里始终有一项 isDefault=true，前端不会静默 fallback 到第一条
        var defaultProvider = rows
            .OrderBy(m => m.IsDefault ? 0 : 1)
            .ThenBy(m => m.Sort)
            .FirstOrDefault()?.Provider ?? "";
        return rows
            .GroupBy(m => m.Provider)
            .Select(g =>
            {
                var def = g.OrderBy(m => m.IsDefault ? 0 : 1).ThenBy(m => m.Sort).First();
                return new ProviderInfo
                {
                    Name = g.Key,
                    DisplayName = def.Provider,
                    DefaultModel = def.ModelId,
                    IsDefault = g.Key == defaultProvider
                };
            })
            .ToList();
    }

    /// <summary>
    /// 获取模型列表（可按 provider、模型类型、能力过滤）
    /// </summary>
    /// <param name="provider">提供商，为空表示不限</param>
    /// <param name="modelType">模型类型（llm / vision，见 AiModelType），为空表示不限；兼容旧参数</param>
    /// <param name="require">需要具备的能力（位掩码），None 表示不限。
    /// 例：前端要发图片时传 InputImage，即可只拿到能看图的模型。</param>
    public List<ModelInfo> GetModelList(string? provider, string? modelType = null,
        AiModelCapability require = AiModelCapability.None)
    {
        var type = string.IsNullOrWhiteSpace(modelType) ? null : AiModelType.Normalize(modelType);

        // 仅返回已配置 ApiKey 的模型，未配置密钥的模型对前端不可用
        var rows = _db.Queryable<AiModelEntity>()
            .Where(m => !string.IsNullOrEmpty(m.ApiKey))
            .Where(m => provider == null || m.Provider == provider)
            .OrderBy(m => m.Sort)
            .ToList();

        // 类型过滤在内存进行：兼容历史数据 model_type 为空的情况（空值按 llm 处理）
        if (type != null)
            rows = rows.Where(m => AiModelType.Normalize(m.ModelType) == type).ToList();

        // 能力按位与过滤：capabilities 为 0 的历史数据按 model_type 推导，保证存量模型不被筛掉
        return rows
            .Select(m => new { Row = m, Cap = AiModelCapabilities.Normalize(m.Capabilities, m.ModelType) })
            .Where(x => AiModelCapabilities.HasAll(x.Cap, require))
            .Select(x => new ModelInfo
            {
                Provider = x.Row.Provider,
                ModelId = x.Row.ModelId,
                DisplayName = string.IsNullOrWhiteSpace(x.Row.DisplayName) ? x.Row.ModelId : x.Row.DisplayName,
                MaxContext = x.Row.MaxContext,
                Capabilities = (int)x.Cap,
                CapabilityKeys = AiModelCapabilities.ToKeys(x.Cap),
                CapabilitySummary = AiModelCapabilities.Summary(x.Cap),
                // 类型由能力派生，不再依赖手填的 model_type
                ModelType = AiModelCapabilities.IsMultimodal(x.Cap) ? AiModelType.Vision : AiModelType.Llm,
                ModelTypeName = AiModelCapabilities.TypeName(x.Cap)
            })
            .ToList();
    }

    /// <summary>
    /// 获取指定模型（未指定则用提供商默认模型）的最大上下文窗口。
    /// 数据库未配置（&lt;=0）时回退到 ContextCompressor.DefaultMaxContext。
    /// </summary>
    public int GetContextLimit(string? providerName, string? modelId)
    {
        var name = ResolveProviderName(providerName);
        var query = _db.Queryable<AiModelEntity>().Where(m => m.Provider == name);

        var row = string.IsNullOrWhiteSpace(modelId)
            ? null
            : query.First(m => m.ModelId == modelId);

        row ??= _db.Queryable<AiModelEntity>()
            .Where(m => m.Provider == name)
            .OrderBy(m => m.IsDefault ? 0 : 1)
            .OrderBy(m => m.Sort)
            .First();

        return row is { MaxContext: > 0 } ? row.MaxContext : ContextCompressor.DefaultMaxContext;
    }

    /// <summary>
    /// 构建 Kernel 实例，并按 agent 类型挂载对应插件（插件隔离）
    /// </summary>
    private SKKernel BuildKernel(string providerName, string? agentType)
    {
        var config = GetProviderConfig(providerName);

        var logger = _loggerFactory.CreateLogger($"Wes.AI.Kernel.{providerName}");

        // 明确提示密钥缺失，避免调用时只看到 401 却找不到原因
        if (string.IsNullOrWhiteSpace(config.ApiKey))
            logger.LogWarning("Provider '{Provider}' 的 ApiKey 为空，调用模型接口将返回 401。请检查 ai_model 表配置。",
                providerName);

        logger.LogInformation("Building Kernel: provider={Provider}, endpoint={Endpoint}, model={Model}, agentType={AgentType}",
            providerName, config.BaseUrl, config.ModelId, agentType);

        var builder = SKKernel.CreateBuilder();

        // 注册 OpenAI 兼容的 Chat Completion 服务
        builder.AddOpenAIChatCompletion(
            modelId: config.ModelId,
            apiKey: new(config.ApiKey),
            endpoint: new Uri(config.BaseUrl));

        // 注册日志
        builder.Services.AddSingleton<ILoggerFactory>(_loggerFactory);

        var kernel = builder.Build();

        RegisterPluginsForType(kernel, agentType, logger);

        logger.LogInformation("Kernel built for provider '{Provider}' ({DisplayName}), model={Model}, agentType={AgentType}",
            providerName, config.Provider, config.ModelId, agentType);

        return kernel;
    }

    /// <summary>
    /// 仅挂载该 agent 类型所需的插件，保证不同 agent 之间工具相互隔离。
    /// - sql_expert：sql + knowledge
    /// - ticket：ticket + knowledge
    /// - 其余（general / auto 等）：不挂载任何领域插件
    /// </summary>
    private void RegisterPluginsForType(SKKernel kernel, string? agentType, ILogger logger)
    {
        var needed = agentType switch
        {
            "sql_expert" => new[] { ("sql", typeof(SqlPlugin)), ("knowledge", typeof(KnowledgePlugin)) },
            "ticket" => new[] { ("ticket", typeof(TicketPlugin)), ("knowledge", typeof(KnowledgePlugin)) },
            // chart_expert 需要查数据生成图表，同样挂载 sql + knowledge
            "chart_expert" => new[] { ("sql", typeof(SqlPlugin)), ("knowledge", typeof(KnowledgePlugin)) },
            _ => System.Array.Empty<(string, System.Type)>()
        };

        foreach (var (name, type) in needed)
        {
            if (kernel.Plugins.Contains(name))
                continue;

            // 从应用根容器解析（插件已在 AddAiServices 中注册为单例）
            var plugin = _services.GetRequiredService(type);
            kernel.Plugins.AddFromObject(plugin, name);
            logger.LogInformation("Plugin '{Plugin}' registered to kernel (agentType={AgentType})", name, agentType);
        }
    }
}
