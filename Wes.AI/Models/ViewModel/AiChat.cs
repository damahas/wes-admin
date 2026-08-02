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
}
