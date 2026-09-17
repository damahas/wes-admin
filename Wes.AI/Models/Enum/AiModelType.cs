namespace Wes.AI.Models.Enum;

/// <summary>
/// AI 模型类型（按主要能力/用途分类）
/// 说明：多数视觉模型本质也是多模态大语言模型，此处分类用于「该模型主要拿来做文本对话还是图像理解」，
/// 不代表其能否看图（如 GPT-4o、qwen-vl 同时具备两种能力）。
/// </summary>
public static class AiModelType
{
    /// <summary>
    /// 大语言模型（文本对话 / Function Calling），默认值
    /// </summary>
    public const string Llm = "llm";

    /// <summary>
    /// 视觉模型（图像理解 / 多模态）
    /// </summary>
    public const string Vision = "vision";

    /// <summary>
    /// 数据库默认值
    /// </summary>
    public const string Default = Llm;

    /// <summary>
    /// 归一化：空值或未知值回退为大语言模型
    /// </summary>
    public static string Normalize(string? type) =>
        string.Equals(type, Vision, System.StringComparison.OrdinalIgnoreCase) ? Vision : Llm;

    /// <summary>
    /// 模型类型的中文显示名称
    /// </summary>
    public static string DisplayName(string? type) =>
        Normalize(type) == Vision ? "视觉模型" : "大语言模型";
}
