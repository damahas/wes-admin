using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using SqlSugar;
using Wes.AI.Context;
using Wes.AI.Models.Entity;
using Wes.AI.Models.ViewModel;
using Wes.Utils.Extension;

namespace Wes.AI.Services;

/// <summary>
/// 会话持久化服务（基于 SqlSugar，按用户隔离）
/// </summary>
public class SessionService : ISessionService
{
    private readonly ISqlSugarClient _db;
    private readonly AiTurnStore _turnStore;

    public SessionService(ISqlSugarClient db, AiTurnStore turnStore)
    {
        _db = db;
        _turnStore = turnStore;
    }

    public async Task<List<AiSessionDto>> ListAsync(long userId)
    {
        var sessions = await _db.Queryable<AiSessionEntity>()
            .Where(s => s.UserId == userId)
            .OrderByDescending(s => s.UpdatedAt)
            .ToListAsync();
        return sessions.Select(s => s.ToEntityCopy<AiSessionEntity, AiSessionDto>()).ToList();
    }

    /// <summary>
    /// 由已落库字段反推 token 来源，避免为此新增列：
    /// model   = 有真实 token 且非估算（模型直返）
    /// probe   = 有真实输入 token 但整体标记为估算（输入实测、输出估算）
    /// estimate= 没有真实 token（全部本地估算）
    /// </summary>
    private static string? InferUsageSource(bool? tokensEstimated, int? promptTokens)
    {
        if (promptTokens == null) return tokensEstimated == null ? null : TokenUsageSource.Estimate;
        return tokensEstimated == true ? TokenUsageSource.Probe : TokenUsageSource.Model;
    }

    public async Task<AiSessionDto?> GetAsync(long userId, long id)
    {
        var s = await _db.Queryable<AiSessionEntity>()
            .Where(x => x.UserId == userId && x.Id == id)
            .FirstAsync();
        if (s == null) return null;

        var msgs = await _db.Queryable<AiMessageEntity>()
            .Where(m => m.SessionId == id && m.UserId == userId)
            .OrderBy(m => m.Sort)
            .ToListAsync();

        var dto = s.ToEntityCopy<AiSessionEntity, AiSessionDto>();
        dto.Messages = msgs.ConvertAll(m => new AiMessageDto
        {
            Role = m.Role,
            Content = m.Content ?? "",
            ToolCalls = string.IsNullOrWhiteSpace(m.ToolCalls)
                ? null
                : JsonSerializer.Deserialize<List<ToolCallInfo>>(m.ToolCalls),
            // 老消息没有落库 token 时，按与前端一致的规则现场补算，保证展示不为空
            Tokens = m.Tokens ?? ContextCompressor.EstimateTokens(m.Content),
            // 历史消息没有耗时记录时返回 null，前端不展示耗时徽章
            ElapsedMs = m.ElapsedMs,
            PromptTokens = m.PromptTokens,
            CompletionTokens = m.CompletionTokens,
            TokensEstimated = m.TokensEstimated,
            // 来源不单独落库：由已存的 prompt_tokens + tokens_estimated 推断
            UsageSource = InferUsageSource(m.TokensEstimated, m.PromptTokens)
        });
        return dto;
    }

    public async Task<AiSessionDto> CreateAsync(long userId, CreateSessionRequest req)
    {
        var now = DateTime.Now;
        var entity = new AiSessionEntity
        {
            Id = SnowFlakeSingle.Instance.NextId(),
            UserId = userId,
            Title = string.IsNullOrWhiteSpace(req.Title) ? "新会话" : req.Title,
            AgentType = req.AgentType,
            Provider = req.Provider,
            Model = req.Model,
            CreatedAt = now,
            UpdatedAt = now
        };
        await _db.Insertable(entity).ExecuteCommandAsync();
        return entity.ToEntityCopy<AiSessionEntity, AiSessionDto>();
    }

    public async Task<int> UpdateAsync(long userId, long id, UpdateSessionRequest req)
    {
        var entity = await _db.Queryable<AiSessionEntity>()
            .Where(x => x.UserId == userId && x.Id == id)
            .FirstAsync();
        if (entity == null) return 0;

        if (req.Title != null) entity.Title = req.Title;
        if (req.AgentType != null) entity.AgentType = req.AgentType;
        if (req.Provider != null) entity.Provider = req.Provider;
        if (req.Model != null) entity.Model = req.Model;
        entity.UpdatedAt = DateTime.Now;

        return await _db.Updateable(entity).ExecuteCommandAsync();
    }

    public async Task<int> DeleteAsync(long userId, long id)
    {
        await _db.Deleteable<AiMessageEntity>()
            .Where(m => m.SessionId == id && m.UserId == userId)
            .ExecuteCommandAsync();
        return await _db.Deleteable<AiSessionEntity>()
            .Where(s => s.UserId == userId && s.Id == id)
            .ExecuteCommandAsync();
    }

    public async Task<int> SaveMessagesAsync(long userId, long id, List<AiMessageDto> messages)
    {
        var exists = await _db.Queryable<AiSessionEntity>()
            .AnyAsync(s => s.UserId == userId && s.Id == id);
        if (!exists) return 0;

        // 全量覆盖会重建所有行，因此先取出旧消息：本轮只更新最后一条 assistant 的实测指标，
        // 其余消息沿用已落库的耗时/token，否则每问一次就会把历史消息的统计清空
        var oldMessages = await _db.Queryable<AiMessageEntity>()
            .Where(m => m.SessionId == id && m.UserId == userId)
            .OrderBy(m => m.Sort)
            .ToListAsync();

        // 全量覆盖该会话消息，保证幂等
        var rows = await _db.Deleteable<AiMessageEntity>()
            .Where(m => m.SessionId == id && m.UserId == userId)
            .ExecuteCommandAsync();

        if (messages.Count > 0)
        {
            var now = DateTime.Now;

            // 耗时与真实 token 以服务端实测为准：取本轮流式调用记录的指标，写到最后一条 assistant 消息上。
            // 前端不传这些字段（传了也不采信），避免时钟不一致或被篡改。
            var turn = _turnStore.Take(id);
            var assistantIdx = -1;
            for (var i = messages.Count - 1; i >= 0; i--)
            {
                if (string.Equals(messages[i].Role, "assistant", StringComparison.OrdinalIgnoreCase))
                {
                    assistantIdx = i;
                    break;
                }
            }

            var entities = new List<AiMessageEntity>(messages.Count);
            for (var i = 0; i < messages.Count; i++)
            {
                var m = messages[i];

                // 同位置且同角色的旧消息：沿用它已落库的统计值（历史消息的耗时/真实 token 不能被覆盖）
                var prev = i < oldMessages.Count
                           && string.Equals(oldMessages[i].Role, m.Role, StringComparison.OrdinalIgnoreCase)
                    ? oldMessages[i]
                    : null;
                // 本轮实测指标只作用于最后一条 assistant 消息
                var isCurrent = i == assistantIdx && turn != null;

                entities.Add(new AiMessageEntity
                {
                    Id = SnowFlakeSingle.Instance.NextId(),
                    SessionId = id,
                    UserId = userId,
                    Role = m.Role,
                    Content = m.Content,
                    ToolCalls = m.ToolCalls == null ? null : JsonSerializer.Serialize(m.ToolCalls),
                    // 本轮有真实 token 就用真实值（总数 = 输入 + 输出），否则沿用旧值，最后才回退本地估算
                    Tokens = isCurrent && turn!.PromptTokens != null
                        ? turn.PromptTokens + turn.CompletionTokens
                        : (prev?.Tokens ?? m.Tokens ?? ContextCompressor.EstimateTokens(m.Content)),
                    ElapsedMs = isCurrent ? (int)turn!.ElapsedMs : prev?.ElapsedMs,
                    PromptTokens = isCurrent ? turn!.PromptTokens : prev?.PromptTokens,
                    CompletionTokens = isCurrent ? turn!.CompletionTokens : prev?.CompletionTokens,
                    TokensEstimated = isCurrent ? turn!.Estimated : prev?.TokensEstimated,
                    Sort = i,
                    CreatedAt = now
                });
            }
            rows += await _db.Insertable(entities).ExecuteCommandAsync();
        }

        rows += await _db.Updateable<AiSessionEntity>()
            .SetColumns(s => s.UpdatedAt == DateTime.Now)
            .Where(s => s.Id == id)
            .ExecuteCommandAsync();

        return rows;
    }
}
