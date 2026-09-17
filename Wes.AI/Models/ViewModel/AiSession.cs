using Wes.Utils.Converter;
using System.Text.Json.Serialization;
namespace Wes.AI.Models.ViewModel;

/// <summary>
/// 消息 DTO（前后端传输）
/// </summary>
public class AiMessageDto
{
    public string Role { get; set; } = "";
    public string Content { get; set; } = "";
    public List<ToolCallInfo>? ToolCalls { get; set; }
    /// <summary>
    /// 该条消息的 token 数（用户消息为输入估算，助手消息为输出估算；为空时后端会自动补算）
    /// </summary>
    public int? Tokens { get; set; }

    /// <summary>
    /// 回答耗时（毫秒）。仅助手消息有值，由前端计时后提交，为空表示无计时记录（历史消息）。
    /// </summary>
    public int? ElapsedMs { get; set; }

    /// <summary>
    /// 模型返回的输入 token（真实消耗）。仅助手消息有值，为空表示模型未返回 usage。
    /// </summary>
    public int? PromptTokens { get; set; }

    /// <summary>
    /// 模型返回的输出 token（真实消耗）。仅助手消息有值，为空表示模型未返回 usage。
    /// </summary>
    public int? CompletionTokens { get; set; }

    /// <summary>
    /// true=tokens 为本地估算值（模型未返回 usage），false=真实消耗，null=未知（历史数据）
    /// </summary>
    public bool? TokensEstimated { get; set; }

    /// <summary>
    /// token 来源：model=模型直返真实值 / probe=输入实测+输出估算 / estimate=全部估算，null=未知（历史数据）。
    /// </summary>
    public string? UsageSource { get; set; }
}

/// <summary>
/// 会话 DTO（列表/详情）
/// </summary>
public class AiSessionDto
{
    [JsonConverter(typeof(LongToStringConverter))]
    public long Id { get; set; }
    public string Title { get; set; } = "";
    public string AgentType { get; set; } = "";
    public string Provider { get; set; } = "";
    public string Model { get; set; } = "";
    public DateTime UpdatedAt { get; set; }
    public List<AiMessageDto> Messages { get; set; } = new();
}

/// <summary>
/// 创建会话请求
/// </summary>
public class CreateSessionRequest
{
    public string Title { get; set; } = "新会话";
    public string AgentType { get; set; } = "chat";
    public string Provider { get; set; } = "";
    public string Model { get; set; } = "";
}

/// <summary>
/// 更新会话请求
/// </summary>
public class UpdateSessionRequest
{
    public string? Title { get; set; }
    public string? AgentType { get; set; }
    public string? Provider { get; set; }
    public string? Model { get; set; }
}

/// <summary>
/// 保存消息请求
/// </summary>
public class SaveMessagesRequest
{
    public List<AiMessageDto> Messages { get; set; } = new();
}
