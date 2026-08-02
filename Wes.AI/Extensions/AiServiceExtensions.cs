using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Wes.AI.Kernel;
using Wes.AI.Plugins;
using Wes.AI.Services;

namespace Wes.AI.Extensions;

/// <summary>
/// Wes.AI 模块 DI 注册扩展
/// </summary>
public static class AiServiceExtensions
{
    /// <summary>
    /// 注册 AI 模块的所有服务到 DI 容器
    /// </summary>
    public static IServiceCollection AddAiServices(this IServiceCollection services, IConfiguration configuration)
    {
        // SK Kernel 提供者（单例，管理多提供商 Kernel，配置来自数据库 ai_model）
        services.AddSingleton<KernelProvider>();

        // AI 聊天场景统一复用 Agent 框架的 "chat" 类型
        services.AddScoped<IAgentService, AgentService>();

        // AI 会话持久化服务
        services.AddScoped<ISessionService, SessionService>();

        // AI 模型配置种子（启动时空表则插入默认数据，apikey 为空）
        services.AddHostedService<ModelConfigSeeder>();

        // AI 助手插件：无状态，注册为单例。
        // 注意：AgentService 把插件实例解析后注册到【单例缓存的 Kernel】上，
        // 因此插件本身不能是 Scoped。插件内部如需 Scoped 服务（如 ISqlSugarClient），
        // 应在方法内自行 CreateScope 解析（SqlPlugin 已这样做）。
        services.AddSingleton<SqlPlugin>();
        services.AddSingleton<TicketPlugin>();
        services.AddSingleton<KnowledgePlugin>();

        return services;
    }
}
