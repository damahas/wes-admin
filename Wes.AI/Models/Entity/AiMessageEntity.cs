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
    [SugarColumn(IsPrimaryKey = true, ColumnName = "id")]
    public long Id { get; set; }

    /// <summary>
    /// 所属会话 ID
    /// </summary>
    [JsonConverter(typeof(LongToStringConverter))]
    [SugarColumn(ColumnName = "session_id")]
    public long SessionId { get; set; }

    /// <summary>
    /// 用户 ID
    /// </summary>
    [JsonConverter(typeof(LongToStringConverter))]
    [SugarColumn(ColumnName = "user_id")]
    public long UserId { get; set; }

    /// <summary>
    /// 消息角色（user / assistant / system）
    /// </summary>
    [SugarColumn(ColumnName = "role", Length = 20)]
    public string Role { get; set; } = "";

    /// <summary>
    /// 消息内容
    /// </summary>
    [SugarColumn(ColumnName = "content", ColumnDataType = "longtext", IsNullable = true)]
    public string? Content { get; set; }

    /// <summary>
    /// 工具调用信息（JSON 字符串）
    /// </summary>
    [SugarColumn(ColumnName = "tool_calls", ColumnDataType = "longtext", IsNullable = true)]
    public string? ToolCalls { get; set; }

    /// <summary>
    /// 排序号（对话先后顺序）
    /// </summary>
    [SugarColumn(ColumnName = "sort")]
    public int Sort { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    [SugarColumn(ColumnName = "created_at")]
    public DateTime CreatedAt { get; set; }
}
