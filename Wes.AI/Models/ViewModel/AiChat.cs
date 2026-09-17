using Wes.AI.Models.Enum;
namespace Wes.AI.Models.ViewModel;

// ============ 消息模型 ============

/// <summary>
/// 历史消息
/// </summary>
public class HistoryMessage
{
    public string Role { get; set; } = "";   // user / assistant / system / tool
    public string Content { get; set; } = "";
    public string? Name { get; set; }
    public string? ToolCallId { get; set; }
}

/// <summary>
/// 工具调用信息
/// </summary>
public class ToolCallInfo
{
    public string Id { get; set; } = "";
    public string Name { get; set; } = "";
    public string Arguments { get; set; } = "";
}

// ============ 聊天请求/响应 ============

/// <summary>
/// 聊天请求（直接对话，无 Function Calling）
/// </summary>
public class AiChatRequest
{
    public string? Provider { get; set; }
    public string? Model { get; set; }
    public string Message { get; set; } = "";
    public List<HistoryMessage>? History { get; set; }
    public double Temperature { get; set; } = 0.7;
    public int MaxTokens { get; set; } = 4096;
}

/// <summary>
/// 上下文用量（用于前端展示「上下文占用圆环」与压缩提示）
/// </summary>
public class ContextUsage
{
    /// <summary>本次请求估算的输入 token（系统提示 + 历史 + 当前消息）</summary>
    public int UsedTokens { get; set; }
    /// <summary>模型最大上下文窗口</summary>
    public int MaxContext { get; set; }
    /// <summary>本次留给历史的预算（已扣除系统提示与输出预留）</summary>
    public int Budget { get; set; }
    /// <summary>占用比例 0~1</summary>
    public double Ratio { get; set; }
    /// <summary>是否发生过压缩（裁剪/折叠/截断）</summary>
    public bool Compressed { get; set; }
    /// <summary>被折叠为摘要的历史条数</summary>
    public int DroppedMessages { get; set; }
    /// <summary>被中段截断的超长消息条数</summary>
    public int TruncatedMessages { get; set; }
}

/// <summary>
/// 流式 SSE 数据块
/// </summary>
public class StreamChunk
{
    public string? Content { get; set; }
    public string? FinishReason { get; set; }
    public List<ToolCallInfo>? ToolCalls { get; set; }
    /// <summary>
    /// 调用模型失败时填充的错误信息（前端可据此提示用户）
    /// </summary>
    public string? Error { get; set; }
    /// <summary>
    /// 上下文用量（仅在本次请求的最后一个块中返回）
    /// </summary>
    public ContextUsage? Usage { get; set; }

    /// <summary>
    /// 本次回答耗时（毫秒，服务端 Stopwatch 实测）。
    /// 仅在最后一个块中返回，供前端展示；落库由服务端在保存消息时写入，不依赖前端回传。
    /// </summary>
    public long? ElapsedMs { get; set; }

    /// <summary>
    /// 模型返回的输入 token（真实消耗）。仅在最后一个块中返回；为 null 表示模型未返回 usage。
    /// </summary>
    public int? PromptTokens { get; set; }

    /// <summary>
    /// 模型返回的输出 token（真实消耗）。仅在最后一个块中返回；为 null 表示模型未返回 usage。
    /// </summary>
    public int? CompletionTokens { get; set; }

    /// <summary>
    /// true=模型未返回 usage，前端展示的 token 为本地估算值（展示时加 ~ 前缀）
    /// </summary>
    public bool TokensEstimated { get; set; }

    /// <summary>
    /// token 来源：model=模型直返真实值 / probe=输入为探测实测、输出为估算 / estimate=全部估算。见 TokenUsageSource。
    /// </summary>
    public string? UsageSource { get; set; }
}

// ============ Agent 请求/响应 ============

/// <summary>
/// Agent 对话请求
/// </summary>
public class AiAgentRequest
{
    public string? Provider { get; set; }
    public string? Model { get; set; }
    public string Message { get; set; } = "";
    /// <summary>
    /// 所属会话 ID（可选）。传入后服务端会把本次回答耗时与该会话关联，
    /// 前端保存消息时自动落库到该条 assistant 消息上，无需前端回传耗时。
    /// </summary>
    public long? SessionId { get; set; }
    public List<HistoryMessage>? History { get; set; }
    public Dictionary<string, object?>? Context { get; set; }
    public double Temperature { get; set; } = 0.7;
    public int MaxTokens { get; set; } = 4096;
}

/// <summary>
/// Agent 对话响应
/// </summary>
public class AiAgentResponse
{
    public string AgentType { get; set; } = "";
    public string Provider { get; set; } = "";
    public string Model { get; set; } = "";
    public string Content { get; set; } = "";
    public List<ToolCallInfo>? ToolCalls { get; set; }
    public int? TotalTokens { get; set; }
    public long DurationMs { get; set; }
    /// <summary>模型返回的输入 token（真实消耗），未返回时为 null</summary>
    public int? PromptTokens { get; set; }
    /// <summary>模型返回的输出 token（真实消耗），未返回时为 null</summary>
    public int? CompletionTokens { get; set; }
    /// <summary>true=模型未返回 usage，展示的 token 为本地估算值</summary>
    public bool TokensEstimated { get; set; }
    /// <summary>token 来源：model / probe / estimate，见 TokenUsageSource</summary>
    public string? UsageSource { get; set; }
    /// <summary>
    /// 本次请求的上下文用量（含是否触发压缩），前端据此刷新上下文圆环
    /// </summary>
    public ContextUsage? Usage { get; set; }
}

// ============ 管理查询模型 ============

/// <summary>
/// 提供商列表项
/// </summary>
public class ProviderInfo
{
    public string Name { get; set; } = "";
    public string DisplayName { get; set; } = "";
    public string DefaultModel { get; set; } = "";
    public bool IsDefault { get; set; }
}

/// <summary>
/// Agent 类型列表项
/// </summary>
public class AgentTypeInfo
{
    public string Type { get; set; } = "";
    public string DisplayName { get; set; } = "";
    public string Description { get; set; } = "";
}

/// <summary>
/// 模型列表项
/// </summary>
public class ModelInfo
{
    public string Provider { get; set; } = "";
    public string ModelId { get; set; } = "";
    public string DisplayName { get; set; } = "";
    /// <summary>模型最大上下文窗口（token，未知时为 0，前端回退默认值）</summary>
    public int MaxContext { get; set; }
    /// <summary>能力位掩码（见 AiModelCapability）</summary>
    public int Capabilities { get; set; }
    /// <summary>能力 key 列表（如 input_text / input_image），前端据此判断模型能否用于某场景</summary>
    public List<string> CapabilityKeys { get; set; } = new();
    /// <summary>能力摘要（如「文+图 → 文」），用于下拉与列表展示</summary>
    public string CapabilitySummary { get; set; } = "";
    /// <summary>模型类型（由能力派生，兼容旧字段）：llm / vision</summary>
    public string ModelType { get; set; } = AiModelType.Llm;
    /// <summary>模型类型的中文显示名称（大语言模型 / 多模态模型）</summary>
    public string ModelTypeName { get; set; } = AiModelType.DisplayName(AiModelType.Llm);
}
