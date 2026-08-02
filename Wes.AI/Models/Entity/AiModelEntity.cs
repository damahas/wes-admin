using SqlSugar;
using System;

using Wes.Utils.Converter;
using System.Text.Json.Serialization;
namespace Wes.AI.Models.Entity;

/// <summary>
/// AI 模型配置（数据库管理，替代原 appsettings 的 AI 节点）
/// </summary>
[SugarTable("sys_ai_model", TableDescription = "AI 模型配置")]
public class AiModelEntity
{
    /// <summary>
    /// 主键 ID
    /// </summary>
    [JsonConverter(typeof(LongToStringConverter))]
    [SugarColumn(IsPrimaryKey = true, ColumnName = "id")]
    public long Id { get; set; }

    /// <summary>
    /// 提供商标识（如 deepseek / qwen / glm / openai）
    /// </summary>
    [SugarColumn(ColumnName = "provider", Length = 50)]
    public string Provider { get; set; } = "";

    /// <summary>
    /// 提供商显示名称
    /// </summary>
    [SugarColumn(ColumnName = "provider_name", Length = 100)]
    public string ProviderName { get; set; } = "";

    /// <summary>
    /// 模型 ID（调用接口时使用）
    /// </summary>
    [SugarColumn(ColumnName = "model_id", Length = 100)]
    public string ModelId { get; set; } = "";

    /// <summary>
    /// 模型显示名称
    /// </summary>
    [SugarColumn(ColumnName = "display_name", Length = 100)]
    public string DisplayName { get; set; } = "";

    /// <summary>
    /// API Key（为空时需用户在数据库补充）。禁止序列化到前端。
    /// </summary>
    [JsonIgnore]
    [SugarColumn(ColumnName = "api_key", Length = 200)]
    public string ApiKey { get; set; } = "";

    /// <summary>
    /// 接口地址（OpenAI 兼容）
    /// </summary>
    [SugarColumn(ColumnName = "base_url", Length = 300)]
    public string BaseUrl { get; set; } = "";

    /// <summary>
    /// 是否该提供商的默认模型（同提供商仅一个为 true）
    /// </summary>
    [SugarColumn(ColumnName = "is_default")]
    public bool IsDefault { get; set; }

    /// <summary>
    /// 是否全局默认提供商（前端预选、未指定时回退）
    /// </summary>
    [SugarColumn(ColumnName = "is_default_provider")]
    public bool IsDefaultProvider { get; set; }

    /// <summary>
    /// 排序号
    /// </summary>
    [SugarColumn(ColumnName = "sort")]
    public int Sort { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    [SugarColumn(ColumnName = "created_at")]
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// 更新时间
    /// </summary>
    [SugarColumn(ColumnName = "updated_at")]
    public DateTime UpdatedAt { get; set; }
}
