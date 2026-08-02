using SqlSugar;
using System;

using Wes.Utils.Converter;
using System.Text.Json.Serialization;
namespace Wes.AI.Models.Entity;

/// <summary>
/// AI 会话（按用户隔离）
/// </summary>
[SugarTable("sys_ai_session", TableDescription = "AI 会话")]
public class AiSessionEntity
{
    /// <summary>
    /// 主键 ID
    /// </summary>
    [JsonConverter(typeof(LongToStringConverter))]
    [SugarColumn(IsPrimaryKey = true, ColumnName = "id")]
    public long Id { get; set; }

    /// <summary>
    /// 用户 ID（会话按用户隔离）
    /// </summary>
    [JsonConverter(typeof(LongToStringConverter))]
    [SugarColumn(ColumnName = "user_id")]
    public long UserId { get; set; }

    /// <summary>
    /// 会话标题
    /// </summary>
    [SugarColumn(ColumnName = "title", Length = 200)]
    public string Title { get; set; } = "新会话";

    /// <summary>
    /// 智能体（助手）类型，如 chat
    /// </summary>
    [SugarColumn(ColumnName = "agent_type", Length = 50)]
    public string AgentType { get; set; } = "chat";

    /// <summary>
    /// 模型提供商名称
    /// </summary>
    [SugarColumn(ColumnName = "provider", Length = 100)]
    public string Provider { get; set; } = "";

    /// <summary>
    /// 模型 ID（对应 ai_model 表的 model_id，从数据库读取）
    /// </summary>
    [SugarColumn(ColumnName = "model", Length = 100)]
    public string Model { get; set; } = "";

    /// <summary>
    /// 创建时间
    /// </summary>
    [SugarColumn(ColumnName = "created_at")]
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// 更新时间（最近一次消息交互）
    /// </summary>
    [SugarColumn(ColumnName = "updated_at")]
    public DateTime UpdatedAt { get; set; }
}
