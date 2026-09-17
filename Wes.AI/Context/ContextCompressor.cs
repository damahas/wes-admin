using System.Text;
using Wes.AI.Models.ViewModel;

namespace Wes.AI.Context;

/// <summary>
/// 上下文压缩模块：
/// 1. 估算 token（CJK 按 1 token/字、其余按 4 字符/token 的启发式，无需分词器）；
/// 2. 按「模型最大上下文 - 系统提示 - 输出预留」的预算裁剪历史，保留最近若干轮；
/// 3. 把被淘汰的早期对话折叠成一条 system 摘要，保持上下文连贯；
/// 4. 对超长单条消息（如 SQL 表格结果）做中段截断，保留头尾。
///
/// 说明：压缩只影响「送给模型的上下文」，不改动数据库中保存的完整会话历史，
/// 因此用户切换会话后仍能看到全部消息，只是模型不再重复接收过期内容。
/// </summary>
public static class ContextCompressor
{
    /// <summary>模型未配置 max_context 时的兜底上下文窗口</summary>
    public const int DefaultMaxContext = 128000;

    /// <summary>输入侧最多占用的上下文比例，其余留给模型输出</summary>
    private const double InputBudgetRatio = 0.8;

    /// <summary>无论如何至少保留的历史条数（含当前这一轮的上下文）</summary>
    private const int MinKeepMessages = 4;

    /// <summary>摘要中最多列出几条早期消息</summary>
    private const int SummaryMaxItems = 8;

    /// <summary>摘要中每条消息保留的字符数</summary>
    private const int SummaryItemChars = 60;

    /// <summary>单条消息超过该 token 数时做中段截断</summary>
    private const int LongMessageTokens = 2000;

    private const int LongMessageKeepHead = 700;
    private const int LongMessageKeepTail = 300;

    /// <summary>
    /// 估算文本的 token 数：CJK 约 1 token/字，其余约 4 字符/token。
    /// </summary>
    public static int EstimateTokens(string? text)
    {
        if (string.IsNullOrEmpty(text)) return 0;

        int ascii = 0, cjk = 0;
        foreach (var ch in text)
        {
            // 0x2E80 之后基本都是 CJK/全角字符，按 1 token/字 估算
            if (ch >= 0x2E80) cjk++;
            else ascii++;
        }
        return cjk + (int)Math.Ceiling(ascii / 4.0);
    }

    /// <summary>
    /// 估算消息列表的 token 数（每条额外计入角色与分隔的开销）。
    /// </summary>
    public static int EstimateMessages(IEnumerable<HistoryMessage>? messages)
    {
        if (messages == null) return 0;
        var total = 0;
        foreach (var m in messages)
            total += EstimateTokens(m?.Content) + 4;
        return total;
    }

    /// <summary>
    /// 压缩历史：按预算保留最近的消息，早期消息折叠为摘要，超长消息截断。
    /// </summary>
    /// <param name="history">完整历史（按时间正序，不含当前这条用户消息）</param>
    /// <param name="systemTokens">系统提示与工具定义占用的 token</param>
    /// <param name="reservedOutputTokens">为模型输出预留的 token（一般为 max_tokens）</param>
    /// <param name="maxContext">模型最大上下文窗口，&lt;=0 时使用兜底值</param>
    /// <param name="currentMessage">本次用户消息（保证不被裁掉）</param>
    public static CompressResult Compress(
        IReadOnlyList<HistoryMessage>? history,
        int systemTokens,
        int reservedOutputTokens,
        int maxContext,
        string? currentMessage = null)
    {
        var limit = maxContext > 0 ? maxContext : DefaultMaxContext;
        var budget = (int)(limit * InputBudgetRatio) - systemTokens - reservedOutputTokens;
        if (budget < 2000) budget = 2000;

        // 1) 先把超长消息截断，避免一条 SQL 结果吃掉整个窗口
        var normalized = new List<HistoryMessage>();
        var truncated = 0;
        if (history != null)
        {
            foreach (var m in history)
            {
                if (m != null && EstimateTokens(m.Content) > LongMessageTokens)
                {
                    normalized.Add(TruncateMiddle(m));
                    truncated++;
                }
                else
                {
                    normalized.Add(m!);
                }
            }
        }

        // 2) 从最新往回保留，直到超出预算（至少保留 MinKeepMessages 条）
        var currentTokens = EstimateTokens(currentMessage) + 4;
        var kept = new List<HistoryMessage>();
        var used = currentTokens;
        for (var i = normalized.Count - 1; i >= 0; i--)
        {
            if (kept.Count >= MinKeepMessages && used > budget) break;
            used += EstimateTokens(normalized[i].Content) + 4;
            kept.Add(normalized[i]);
        }
        kept.Reverse();

        var dropped = normalized.Count - kept.Count;

        // 3) 被淘汰的早期对话折叠为一条 system 摘要
        var result = new List<HistoryMessage>();
        var hasSummary = false;
        if (dropped >= 2)
        {
            result.Add(new HistoryMessage
            {
                Role = "system",
                Content = BuildSummary(normalized.Take(dropped).ToList(), dropped)
            });
            hasSummary = true;
        }
        result.AddRange(kept);

        var usedTokens = systemTokens + EstimateMessages(result) + EstimateTokens(currentMessage) + 4;
        return new CompressResult
        {
            Messages = result,
            Usage = new ContextUsage
            {
                UsedTokens = usedTokens,
                MaxContext = limit,
                Budget = budget,
                Ratio = limit > 0 ? Math.Min(1d, usedTokens / (double)limit) : 0,
                Compressed = dropped > 0 || truncated > 0,
                DroppedMessages = dropped,
                TruncatedMessages = truncated
            }
        };
    }

    /// <summary>
    /// 超长消息中段截断：保留开头与结尾，中间用省略提示替代。
    /// </summary>
    private static HistoryMessage TruncateMiddle(HistoryMessage m)
    {
        var content = m.Content ?? "";
        if (content.Length <= LongMessageKeepHead + LongMessageKeepTail)
            return m;

        var tokens = EstimateTokens(content);
        var head = content[..LongMessageKeepHead];
        var tail = content[^LongMessageKeepTail..];
        return new HistoryMessage
        {
            Role = m.Role,
            Name = m.Name,
            ToolCallId = m.ToolCallId,
            Content = $"{head}\n…（本条中间已省略，原约 {tokens} token）…\n{tail}"
        };
    }

    /// <summary>
    /// 生成早期对话摘要：只取每条消息的开头，压缩成要点列表。
    /// </summary>
    private static string BuildSummary(List<HistoryMessage> dropped, int count)
    {
        var sb = new StringBuilder();
        sb.AppendLine("【早期对话摘要】以下要点来自已被压缩的较早对话，仅用于保持上下文连贯，不得当作工具返回的真实数据：");

        var picked = dropped
            .Where(m => m is { Role: "user" or "assistant" } && !string.IsNullOrWhiteSpace(m.Content))
            .TakeLast(SummaryMaxItems)
            .ToList();

        foreach (var m in picked)
        {
            var role = m.Role == "user" ? "用户" : "小W";
            var text = OneLine(m.Content);
            if (text.Length > SummaryItemChars)
                text = text[..SummaryItemChars] + "…";
            sb.AppendLine($"- {role}：{text}");
        }

        var omitted = count - picked.Count;
        if (omitted > 0)
            sb.AppendLine($"- （另有 {omitted} 条更早的消息已省略）");

        return sb.ToString();
    }

    /// <summary>
    /// 折叠空白，保证摘要中每条消息只占一行。
    /// </summary>
    private static string OneLine(string text)
    {
        var sb = new StringBuilder(text.Length);
        var lastSpace = false;
        foreach (var ch in text)
        {
            if (char.IsWhiteSpace(ch))
            {
                if (!lastSpace) sb.Append(' ');
                lastSpace = true;
            }
            else
            {
                sb.Append(ch);
                lastSpace = false;
            }
        }
        return sb.ToString().Trim();
    }
}

/// <summary>
/// 压缩结果：压缩后的消息列表 + 本次上下文用量。
/// </summary>
public class CompressResult
{
    public List<HistoryMessage> Messages { get; set; } = new();
    public ContextUsage Usage { get; set; } = new();
}
