using SqlSugar;
using System;

using Wes.Utils.Converter;
using System.Text.Json.Serialization;
using Wes.AI.Models.Enum;
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
    [SugarColumn(IsPrimaryKey = true, ColumnName = "id", ColumnDescription = "主键ID")]
    public long Id { get; set; }

    /// <summary>
    /// 提供商名称（如 DeepSeek / 通义千问 / 智谱 GLM）。
    /// 同时作为分组键：同一名称下的模型共享同一条连接（base_url + api_key）。
    /// 若同一厂商有多个不同接口地址，请用不同名称区分（如「通义千问」「通义千问-中转」）。
    /// </summary>
    [SugarColumn(ColumnName = "provider", Length = 100, ColumnDescription = "提供商名称（分组键，同一名称共享同一接口地址）")]
    public string Provider { get; set; } = "";

    /// <summary>
    /// 模型 ID（调用接口时使用，如 deepseek-chat / qwen-plus）
    /// </summary>
    [SugarColumn(ColumnName = "model_id", Length = 100, ColumnDescription = "模型ID")]
    public string ModelId { get; set; } = "";

    /// <summary>
    /// 模型显示名称（仅前端展示用，不参与接口调用）
    /// </summary>
    [SugarColumn(ColumnName = "display_name", Length = 100, ColumnDescription = "模型显示名称")]
    public string DisplayName { get; set; } = "";

    /// <summary>
    /// API Key（为空时需用户在数据库补充）。禁止序列化到前端。
    /// 同一提供商下优先取已配置密钥的行，见 KernelProvider.GetProviderConfig。
    /// </summary>
    [JsonIgnore]
    [SugarColumn(ColumnName = "api_key", Length = 200, ColumnDescription = "API密钥")]
    public string ApiKey { get; set; } = "";

    /// <summary>
    /// 接口地址（OpenAI 兼容，如 https://api.openai.com/v1）
    /// </summary>
    [SugarColumn(ColumnName = "base_url", Length = 300, ColumnDescription = "接口地址（OpenAI兼容）")]
    public string BaseUrl { get; set; } = "";

    /// <summary>
    /// 模型最大上下文窗口（token）。
    /// 用于上下文压缩预算与前端「上下文占用」展示；小于等于 0 时回退为默认值。
    /// </summary>
    [SugarColumn(ColumnName = "max_context", ColumnDescription = "最大上下文窗口（token）")]
    public int MaxContext { get; set; } = 128000;

    /// <summary>
    /// 能力位掩码（见 AiModelCapability）：输入模态 + 输出模态 + 附加能力。
    /// 运行时以本字段为准；为 0（历史数据）时按 model_type 推导，见 AiModelCapabilities.Normalize。
    /// </summary>
    [SugarColumn(ColumnName = "capabilities", ColumnDescription = "能力位掩码（见 AiModelCapability）")]
    public int Capabilities { get; set; }

    /// <summary>
    /// 模型类型：llm=大语言模型（默认），vision=视觉模型（见 AiModelType）。
    /// 已降级为兼容/展示字段：能力统一由 Capabilities 描述，本字段仅在 Capabilities 为 0 时用于推导。
    /// </summary>
    [SugarColumn(ColumnName = "model_type", Length = 20, ColumnDescription = "模型类型（llm=大语言模型 vision=视觉模型）")]
    public string ModelType { get; set; } = AiModelType.Llm;

    /// <summary>
    /// 是否全局默认模型（全表仅一个为 true）。
    /// 未指定提供商 / 未指定模型时回退到该行，其 Provider 同时作为默认提供商。
    /// </summary>
    [SugarColumn(ColumnName = "is_default", ColumnDescription = "是否默认模型（全表唯一，1是 0否）")]
    public bool IsDefault { get; set; }

    /// <summary>
    /// 排序号（列表与模型下拉的展示顺序）
    /// </summary>
    [SugarColumn(ColumnName = "sort", ColumnDescription = "排序号")]
    public int Sort { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    [SugarColumn(ColumnName = "created_at", ColumnDescription = "创建时间")]
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// 更新时间
    /// </summary>
    [SugarColumn(ColumnName = "updated_at", ColumnDescription = "更新时间")]
    public DateTime UpdatedAt { get; set; }
}
