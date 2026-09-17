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
    [SugarColumn(IsPrimaryKey = true, ColumnName = "id", ColumnDescription = "主键ID")]
    public long Id { get; set; }

    /// <summary>
    /// 用户 ID（会话按用户隔离，只能看到自己的会话）
    /// </summary>
    [JsonConverter(typeof(LongToStringConverter))]
    [SugarColumn(ColumnName = "user_id", ColumnDescription = "用户ID")]
    public long UserId { get; set; }

    /// <summary>
    /// 会话标题（新建时默认「新会话」，后续可用首条消息改写）
    /// </summary>
    [SugarColumn(ColumnName = "title", Length = 200, ColumnDescription = "会话标题")]
    public string Title { get; set; } = "新会话";

    /// <summary>
    /// 智能体类型：auto=自动调度（默认入口）/ general / sql_expert / chart_expert / ticket。
    /// 对外统一身份为「小W」，该字段仅用于后端路由，不暴露给用户。
    /// </summary>
    [SugarColumn(ColumnName = "agent_type", Length = 50, ColumnDescription = "智能体类型（auto=自动调度 general=通用 sql_expert=数据查询 chart_expert=图表 ticket=工单）")]
    public string AgentType { get; set; } = "chat";

    /// <summary>
    /// 模型提供商名称（对应 sys_ai_model.provider，如「通义千问」）。
    /// 仅用于回显与新建会话时的默认值，实际调用由 Model 决定。
    /// </summary>
    [SugarColumn(ColumnName = "provider", Length = 100, ColumnDescription = "模型提供商名称")]
    public string Provider { get; set; } = "";

    /// <summary>
    /// 模型 ID（对应 sys_ai_model.model_id）。会话内实际调用的模型，可中途切换。
    /// </summary>
    [SugarColumn(ColumnName = "model", Length = 100, ColumnDescription = "模型ID")]
    public string Model { get; set; } = "";

    /// <summary>
    /// 创建时间
    /// </summary>
    [SugarColumn(ColumnName = "created_at", ColumnDescription = "创建时间")]
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// 更新时间（最近一次消息交互，用于会话列表排序）
    /// </summary>
    [SugarColumn(ColumnName = "updated_at", ColumnDescription = "更新时间")]
    public DateTime UpdatedAt { get; set; }
}
