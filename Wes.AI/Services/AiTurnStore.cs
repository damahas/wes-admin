using System.Collections.Concurrent;

namespace Wes.AI.Services;

/// <summary>
/// token 用量来源：模型直返 > 探测实测 > 本地估算。
/// SK 1.x 的 OpenAI 连接器未实现 stream_options.include_usage，流式拿不到 usage，
/// 因此引入 probe（流式结束后用一次 max_tokens=1 的非流式探测请求取回真实输入 token）。
/// </summary>
public static class TokenUsageSource
{
    /// <summary>模型直接返回的真实入/出 token</summary>
    public const string Model = "model";

    /// <summary>输入 token 为探测实测值，输出 token 仍为本地估算（半真实）</summary>
    public const string Probe = "probe";

    /// <summary>入/出均为本地估算（模型与探测都没拿到 usage）</summary>
    public const string Estimate = "estimate";
}

/// <summary>
/// 本轮对话指标（服务端实测，保存消息时落库）
/// </summary>
public sealed class TurnMetrics
{
    /// <summary>本轮回答耗时（毫秒，Stopwatch 实测）</summary>
    public long ElapsedMs { get; init; }

    /// <summary>模型返回的输入 token（prompt_tokens）；模型未返回 usage 时为 null</summary>
    public int? PromptTokens { get; init; }

    /// <summary>模型返回的输出 token（completion_tokens）；模型未返回 usage 时为 null</summary>
    public int? CompletionTokens { get; init; }

    /// <summary>true=模型未返回 usage，展示的 token 为本地估算值；false=模型返回的真实消耗</summary>
    public bool Estimated { get; init; }

    /// <summary>token 来源，取值见 TokenUsageSource</summary>
    public string? UsageSource { get; init; }
}

/// <summary>
/// 本轮对话指标暂存（sessionId -> 耗时 + 真实 token）。
/// 由 AgentService 在模型调用结束后写入，SessionService 保存消息时取出落库，
/// 避免前端上传不可信（或时钟不一致）的数据。
/// 仅在进程内暂存：取出即消费，服务重启后旧值丢失（对应消息回退为估算展示，不影响其它数据）。
/// </summary>
public class AiTurnStore
{
    private readonly ConcurrentDictionary<long, TurnMetrics> _items = new();

    /// <summary>记录某会话最近一轮的实测指标</summary>
    public void Set(long sessionId, TurnMetrics metrics)
    {
        if (sessionId > 0) _items[sessionId] = metrics;
    }

    /// <summary>取出并清除某会话的指标；无记录返回 null</summary>
    public TurnMetrics? Take(long sessionId) =>
        sessionId > 0 && _items.TryRemove(sessionId, out var m) ? m : null;
}
