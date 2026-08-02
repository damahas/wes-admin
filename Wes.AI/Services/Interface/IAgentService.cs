using Wes.AI.Models.Entity;
using Wes.AI.Models.ViewModel;

namespace Wes.AI.Services;

/// <summary>
/// Agent 服务接口（支持 Function Calling）
/// </summary>
public interface IAgentService
{
    /// <summary>
    /// 执行 Agent 对话（非流式）
    /// </summary>
    Task<AiAgentResponse> ExecuteAsync(string agentType, AiAgentRequest request, CancellationToken ct = default);

    /// <summary>
    /// 执行 Agent 对话（流式 SSE）
    /// </summary>
    IAsyncEnumerable<StreamChunk> ExecuteStreamAsync(string agentType, AiAgentRequest request, CancellationToken ct = default);

    /// <summary>
    /// 获取所有可用的 Agent 类型
    /// </summary>
    List<AgentTypeInfo> GetAgentTypes();
}
