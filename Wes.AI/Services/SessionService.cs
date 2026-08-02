using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using SqlSugar;
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

    public SessionService(ISqlSugarClient db)
    {
        _db = db;
    }

    public async Task<List<AiSessionDto>> ListAsync(long userId)
    {
        var sessions = await _db.Queryable<AiSessionEntity>()
            .Where(s => s.UserId == userId)
            .OrderByDescending(s => s.UpdatedAt)
            .ToListAsync();
        return sessions.Select(s => s.ToEntityCopy<AiSessionEntity, AiSessionDto>()).ToList();
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
                : JsonSerializer.Deserialize<List<ToolCallInfo>>(m.ToolCalls)
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

        // 全量覆盖该会话消息，保证幂等
        var rows = await _db.Deleteable<AiMessageEntity>()
            .Where(m => m.SessionId == id && m.UserId == userId)
            .ExecuteCommandAsync();

        if (messages.Count > 0)
        {
            var now = DateTime.Now;
            var entities = new List<AiMessageEntity>(messages.Count);
            for (var i = 0; i < messages.Count; i++)
            {
                var m = messages[i];
                entities.Add(new AiMessageEntity
                {
                    Id = SnowFlakeSingle.Instance.NextId(),
                    SessionId = id,
                    UserId = userId,
                    Role = m.Role,
                    Content = m.Content,
                    ToolCalls = m.ToolCalls == null ? null : JsonSerializer.Serialize(m.ToolCalls),
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
