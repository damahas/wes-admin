using System;
using System.Collections.Generic;
using System.Linq;

namespace Wes.AI.Models.Enum;

/// <summary>
/// AI 模型能力（位掩码，可组合）。
///
/// 设计说明：模型不再用单一的「类型」描述，而是用能力集合描述——
///   能力 = 输入模态 + 输出模态 + 附加能力。
/// 例如：
///   纯文本对话     = InputText | OutputText | ToolCall | Streaming
///   多模态（看图）  = 上述 + InputImage
///   文生图         = InputText | OutputImage
/// 新增能力只需在此加一个枚举值（左移一位），无需改表结构。
/// </summary>
[Flags]
public enum AiModelCapability
{
    /// <summary>无能力</summary>
    None = 0,

    // ==================== 输入模态 ====================
    /// <summary>文本输入</summary>
    InputText = 1 << 0,
    /// <summary>图片输入（图像理解 / 看图）</summary>
    InputImage = 1 << 1,
    /// <summary>文件输入（文档解析，预留）</summary>
    InputFile = 1 << 2,
    /// <summary>语音输入（预留）</summary>
    InputAudio = 1 << 3,

    // ==================== 输出模态 ====================
    /// <summary>文本输出</summary>
    OutputText = 1 << 4,
    /// <summary>图片输出（文生图 / 图生图）</summary>
    OutputImage = 1 << 5,
    /// <summary>语音输出（预留）</summary>
    OutputAudio = 1 << 6,

    // ==================== 附加能力 ====================
    /// <summary>函数调用（Function Calling / Tools）</summary>
    ToolCall = 1 << 7,
    /// <summary>流式输出（SSE）</summary>
    Streaming = 1 << 8,
    /// <summary>JSON 结构化输出</summary>
    JsonMode = 1 << 9,
    /// <summary>思考型模型（会输出推理过程，如 deepseek-reasoner）</summary>
    Reasoning = 1 << 10
}

/// <summary>
/// 能力位运算与展示的辅助方法。
/// </summary>
public static class AiModelCapabilities
{
    /// <summary>纯文本对话（最保守的默认能力）</summary>
    public const AiModelCapability TextChat = AiModelCapability.InputText | AiModelCapability.OutputText;

    /// <summary>文本对话 + 工具调用 + 流式（Agent 场景所需的最小能力集）</summary>
    public const AiModelCapability AgentChat = TextChat | AiModelCapability.ToolCall | AiModelCapability.Streaming;

    /// <summary>新建模型时的默认能力</summary>
    public const AiModelCapability Default = AgentChat;

    /// <summary>
    /// 能力字典（供前端配置页渲染勾选框，key 为稳定标识，不要随意改名）
    /// </summary>
    public static List<CapabilityOption> Options()
    {
        return new List<CapabilityOption>
        {
            new("input_text", "文本输入", "input", AiModelCapability.InputText),
            new("input_image", "图片输入", "input", AiModelCapability.InputImage),
            new("input_file", "文件输入", "input", AiModelCapability.InputFile),
            new("input_audio", "语音输入", "input", AiModelCapability.InputAudio),
            new("output_text", "文本输出", "output", AiModelCapability.OutputText),
            new("output_image", "图片输出", "output", AiModelCapability.OutputImage),
            new("output_audio", "语音输出", "output", AiModelCapability.OutputAudio),
            new("tool_call", "工具调用", "extra", AiModelCapability.ToolCall),
            new("streaming", "流式输出", "extra", AiModelCapability.Streaming),
            new("json_mode", "JSON 输出", "extra", AiModelCapability.JsonMode),
            new("reasoning", "推理型", "extra", AiModelCapability.Reasoning)
        };
    }

    /// <summary>是否具备某能力</summary>
    public static bool Has(AiModelCapability value, AiModelCapability flag) => (value & flag) == flag;

    /// <summary>是否具备某能力（整数形态，便于直接判断数据库值）</summary>
    public static bool Has(int value, AiModelCapability flag) => ((AiModelCapability)value & flag) == flag;

    /// <summary>是否具备全部指定能力（用于按场景筛选模型）</summary>
    public static bool HasAll(AiModelCapability value, AiModelCapability required) =>
        required == AiModelCapability.None || (value & required) == required;

    /// <summary>
    /// 归一化：数据库值为 0（历史数据/未配置）时，按旧 model_type 推导，避免存量模型能力丢失。
    /// llm → 文本对话 + 工具 + 流式；vision → 再加图片输入。
    /// </summary>
    public static AiModelCapability Normalize(int value, string? modelType = null) =>
        value > 0 ? (AiModelCapability)value : FromModelType(modelType);

    /// <summary>由旧 model_type 推导能力（兼容迁移）</summary>
    public static AiModelCapability FromModelType(string? modelType) =>
        string.Equals(modelType, AiModelType.Vision, StringComparison.OrdinalIgnoreCase)
            ? AgentChat | AiModelCapability.InputImage
            : AgentChat;

    /// <summary>能力集合 → key 列表（给前端回显勾选）</summary>
    public static List<string> ToKeys(AiModelCapability value) =>
        Options().Where(o => Has(value, o.Flag)).Select(o => o.Key).ToList();

    /// <summary>key 列表 → 能力集合</summary>
    public static AiModelCapability FromKeys(IEnumerable<string>? keys)
    {
        if (keys == null) return AiModelCapability.None;
        var set = keys.Where(k => !string.IsNullOrWhiteSpace(k)).ToHashSet(StringComparer.OrdinalIgnoreCase);
        var result = AiModelCapability.None;
        foreach (var o in Options())
        {
            if (set.Contains(o.Key))
                result |= o.Flag;
        }
        return result;
    }

    /// <summary>
    /// 能力摘要（如「文+图 → 文」），用于列表展示与下拉后缀。
    /// </summary>
    public static string Summary(AiModelCapability value)
    {
        var input = new List<string>();
        var output = new List<string>();

        if (Has(value, AiModelCapability.InputText)) input.Add("文");
        if (Has(value, AiModelCapability.InputImage)) input.Add("图");
        if (Has(value, AiModelCapability.InputFile)) input.Add("档");
        if (Has(value, AiModelCapability.InputAudio)) input.Add("音");

        if (Has(value, AiModelCapability.OutputText)) output.Add("文");
        if (Has(value, AiModelCapability.OutputImage)) output.Add("图");
        if (Has(value, AiModelCapability.OutputAudio)) output.Add("音");

        var left = input.Count > 0 ? string.Join("+", input) : "—";
        var right = output.Count > 0 ? string.Join("+", output) : "—";
        return $"{left} → {right}";
    }

    /// <summary>是否为多模态（能接收文本以外的输入）</summary>
    public static bool IsMultimodal(AiModelCapability value) =>
        Has(value, AiModelCapability.InputImage | AiModelCapability.InputFile | AiModelCapability.InputAudio);

    /// <summary>
    /// 展示用类型名（由能力派生，替代手填的 model_type）：多模态模型 / 大语言模型
    /// </summary>
    public static string TypeName(AiModelCapability value) => IsMultimodal(value) ? "多模态模型" : "大语言模型";
}

/// <summary>
/// 能力字典项（返回给前端渲染配置页）
/// </summary>
public class CapabilityOption
{
    public CapabilityOption(string key, string name, string group, AiModelCapability flag)
    {
        Key = key;
        Name = name;
        Group = group;
        Flag = flag;
    }

    /// <summary>稳定标识（存 key，不存枚举数值，避免位调整后错乱）</summary>
    public string Key { get; set; }

    /// <summary>中文名</summary>
    public string Name { get; set; }

    /// <summary>分组：input / output / extra</summary>
    public string Group { get; set; }

    /// <summary>枚举值（仅后端使用，不序列化给前端）</summary>
    [System.Text.Json.Serialization.JsonIgnore]
    public AiModelCapability Flag { get; set; }
}
