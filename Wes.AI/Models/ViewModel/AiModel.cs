using System.Collections.Generic;
using System.Text.Json.Serialization;
using Wes.Utils.Converter;

namespace Wes.AI.Models.ViewModel;

/// <summary>
/// AI 模型配置项（列表 / 详情）
/// 安全：ApiKey 不明文下发，只给 HasApiKey 与掩码形式。
/// </summary>
public class AiModelDto
{
    /// <summary>
    /// 主键。雪花 ID 超过 JS 安全整数范围，统一按字符串下发，避免前端精度丢失
    /// </summary>
    [JsonConverter(typeof(LongToStringConverter))]
    public long Id { get; set; }

    /// <summary>提供商名称（分组键，同一名称共享一条连接）</summary>
    public string Provider { get; set; } = "";

    /// <summary>模型 ID（调用接口时使用）</summary>
    public string ModelId { get; set; } = "";

    /// <summary>模型显示名称</summary>
    public string DisplayName { get; set; } = "";

    /// <summary>接口地址（OpenAI 兼容）</summary>
    public string BaseUrl { get; set; } = "";

    /// <summary>最大上下文窗口</summary>
    public int MaxContext { get; set; }

    /// <summary>能力位掩码（见 AiModelCapability）</summary>
    public int Capabilities { get; set; }

    /// <summary>能力 key 列表（回显勾选用）</summary>
    public List<string> CapabilityKeys { get; set; } = new();

    /// <summary>能力摘要（如「文+图 → 文」）</summary>
    public string CapabilitySummary { get; set; } = "";

    /// <summary>展示用类型名（多模态模型 / 大语言模型）</summary>
    public string ModelTypeName { get; set; } = "";

    /// <summary>是否全局默认模型（全表唯一）</summary>
    public bool IsDefault { get; set; }

    public int Sort { get; set; }

    /// <summary>是否已配置密钥</summary>
    public bool HasApiKey { get; set; }

    /// <summary>密钥掩码（如 sk-****1a2b），仅供回显，提交时留空表示不修改</summary>
    public string ApiKeyMasked { get; set; } = "";
}

/// <summary>
/// 新增 / 编辑模型配置
/// </summary>
public class SaveAiModelRequest
{
    /// <summary>主键，0 表示新增。前端按字符串回传，这里兼容数字与字符串</summary>
    [JsonConverter(typeof(LongToStringConverter))]
    public long Id { get; set; }

    /// <summary>提供商名称</summary>
    public string Provider { get; set; } = "";
    public string ModelId { get; set; } = "";
    public string DisplayName { get; set; } = "";
    public string BaseUrl { get; set; } = "";
    public int MaxContext { get; set; }

    /// <summary>
    /// API Key。留空表示【不修改】（编辑场景）；新增时留空则该模型不可用（列表会被过滤掉）。
    /// </summary>
    public string? ApiKey { get; set; }

    /// <summary>能力 key 列表（见 AiModelCapabilities.Options），为空则使用默认能力</summary>
    public List<string>? CapabilityKeys { get; set; }

    /// <summary>是否全局默认模型（全表唯一，开启后其余行自动取消）</summary>
    public bool IsDefault { get; set; }
    public int Sort { get; set; }
}
