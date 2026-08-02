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
