using SqlSugar;
using System;

using Wes.Utils.Converter;
using System.Text.Json.Serialization;
namespace Wes.AI.Models.Entity;

/// <summary>
/// AI 会话消息
/// </summary>
[SugarTable("sys_ai_message", TableDescription = "AI 会话消息")]
public class AiMessageEntity
{
    /// <summary>
    /// 主键 ID
    /// </summary>
    [JsonConverter(typeof(LongToStringConverter))]
    [SugarColumn(IsPrimaryKey = true, ColumnName = "id", ColumnDescription = "主键ID")]
    public long Id { get; set; }

    /// <summary>
    /// 所属会话 ID（sys_ai_session.id）
    /// </summary>
    [JsonConverter(typeof(LongToStringConverter))]
    [SugarColumn(ColumnName = "session_id", ColumnDescription = "会话ID")]
    public long SessionId { get; set; }

    /// <summary>
    /// 用户 ID（消息按用户冗余，便于直接按用户清理与统计）
    /// </summary>
    [JsonConverter(typeof(LongToStringConverter))]
    [SugarColumn(ColumnName = "user_id", ColumnDescription = "用户ID")]
    public long UserId { get; set; }

    /// <summary>
    /// 消息角色：user=用户 / assistant=AI / system=系统。
    /// 回放给模型时工具消息（tool）会被丢弃，见 AgentService.BuildChatHistory。
    /// </summary>
    [SugarColumn(ColumnName = "role", Length = 20, ColumnDescription = "消息角色（user=用户 assistant=AI system=系统）")]
    public string Role { get; set; } = "";

    /// <summary>
    /// 消息正文（Markdown）。可为空：纯工具调用步骤的消息没有正文。
    /// </summary>
    [SugarColumn(ColumnName = "content", ColumnDataType = "longtext", IsNullable = true, ColumnDescription = "消息内容")]
    public string? Content { get; set; }

    /// <summary>
    /// 工具调用信息（JSON 字符串，记录本条 assistant 消息触发的函数名与入参）。
    /// 回放时缺少 tool_call_id 会被丢弃，因此仅用于审计展示，不参与上下文重建。
    /// </summary>
    [SugarColumn(ColumnName = "tool_calls", ColumnDataType = "longtext", IsNullable = true, ColumnDescription = "工具调用信息")]
    public string? ToolCalls { get; set; }

    /// <summary>
    /// 该条消息的 token 数（本地估算值，用于前端逐条展示消耗，非厂商计费值）
    /// </summary>
    [SugarColumn(ColumnName = "tokens", IsNullable = true, ColumnDescription = "消息token数（估算）")]
    public int? Tokens { get; set; }

    /// <summary>
    /// 回答耗时（毫秒）。仅 assistant 消息有值，用户消息为 null。
    /// 由前端在流式开始/结束时计时后随消息一并保存，用于前端在回答下方展示耗时。
    /// </summary>
    [SugarColumn(ColumnName = "elapsed_ms", IsNullable = true, ColumnDescription = "回答耗时（毫秒）")]
    public int? ElapsedMs { get; set; }

    /// <summary>
    /// 模型返回的输入 token（真实值）。仅 assistant 消息有值，模型未返回 usage 时为 null。
    /// </summary>
    [SugarColumn(ColumnName = "prompt_tokens", IsNullable = true, ColumnDescription = "模型返回的输入token")]
    public int? PromptTokens { get; set; }

    /// <summary>
    /// 模型返回的输出 token（真实值）。仅 assistant 消息有值，模型未返回 usage 时为 null。
    /// </summary>
    [SugarColumn(ColumnName = "completion_tokens", IsNullable = true, ColumnDescription = "模型返回的输出token")]
    public int? CompletionTokens { get; set; }

    /// <summary>
    /// tokens 字段是否为估算值：true=本地估算（模型未返回 usage），false=模型返回的真实消耗，null=未知（历史数据）
    /// </summary>
    [SugarColumn(ColumnName = "tokens_estimated", IsNullable = true, ColumnDescription = "token是否为估算值")]
    public bool? TokensEstimated { get; set; }



    /// <summary>
    /// 排序号（同一会话内按此升序回放）
    /// </summary>
    [SugarColumn(ColumnName = "sort", ColumnDescription = "排序号")]
    public int Sort { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    [SugarColumn(ColumnName = "created_at", ColumnDescription = "创建时间")]
    public DateTime CreatedAt { get; set; }
}
