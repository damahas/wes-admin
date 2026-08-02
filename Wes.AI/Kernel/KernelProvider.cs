using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using SqlSugar;
using Wes.AI.Models.Entity;
using Wes.AI.Models.ViewModel;
using Wes.AI.Plugins;
using SKKernel = Microsoft.SemanticKernel.Kernel;

namespace Wes.AI.Kernel;

/// <summary>
/// SK Kernel 管理器：按提供商从数据库（ai_model）创建并缓存 Kernel 实例。
/// 关键改造：Kernel 按 (提供商, 助手类型) 维度缓存，且只挂载该助手类型所需的插件，
/// 从而实现「不同助手插件相互隔离」——例如 SQL 插件仅存在于 sql_expert 的 Kernel 上，
/// 总 agent（auto）与通用助手（general）的 Kernel 上看不到任何数据库工具。
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
    /// 获取指定提供商的 Kernel（按 提供商:助手类型 缓存，线程安全）。
    /// agentType 为空时返回不带任何领域插件的纯对话 Kernel。
    /// </summary>
    public SKKernel GetKernel(string? providerName = null, string? agentType = null)
    {
        var name = ResolveProviderName(providerName);
        var key = string.IsNullOrEmpty(agentType) ? name : $"{name}:{agentType}";
        return _kernels.GetOrAdd(key, _ => BuildKernel(name, agentType));
    }

    /// <summary>
    /// 解析提供商名称（未指定则使用全局默认提供商）
    /// </summary>
    public string ResolveProviderName(string? providerName)
    {
        if (!string.IsNullOrWhiteSpace(providerName) && ProviderExists(providerName))
            return providerName;

        var def = _db.Queryable<AiModelEntity>().First(m => m.IsDefaultProvider);
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

        var name = ResolveProviderName(providerName);
        var row = _db.Queryable<AiModelEntity>()
            .Where(m => m.Provider == name && m.IsDefault)
            .First()
            ?? _db.Queryable<AiModelEntity>().First(m => m.Provider == name);
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
        var defaultProvider = rows.FirstOrDefault(m => m.IsDefaultProvider)?.Provider ?? "";
        return rows
            .GroupBy(m => m.Provider)
            .Select(g =>
            {
                var def = g.OrderBy(m => m.IsDefault ? 0 : 1).First();
                return new ProviderInfo
                {
                    Name = g.Key,
                    DisplayName = def.ProviderName,
                    DefaultModel = def.ModelId,
                    IsDefault = g.Key == defaultProvider
                };
            })
            .ToList();
    }

    /// <summary>
    /// 获取模型列表（可按 provider 过滤）
    /// </summary>
    public List<ModelInfo> GetModelList(string? provider)
    {
        // 仅返回已配置 ApiKey 的模型，未配置密钥的模型对前端不可用
        var rows = _db.Queryable<AiModelEntity>()
            .Where(m => !string.IsNullOrEmpty(m.ApiKey))
            .Where(m => provider == null || m.Provider == provider)
            .OrderBy(m => m.Sort)
            .ToList();
        return rows.Select(m => new ModelInfo
        {
            Provider = m.Provider,
            ModelId = m.ModelId,
            DisplayName = string.IsNullOrWhiteSpace(m.DisplayName) ? m.ModelId : m.DisplayName
        }).ToList();
    }

    /// <summary>
    /// 构建 Kernel 实例，并按助手类型挂载对应插件（插件隔离）
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
            providerName, config.ProviderName, config.ModelId, agentType);

        return kernel;
    }

    /// <summary>
    /// 仅挂载该助手类型所需的插件，保证不同助手之间工具相互隔离。
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
