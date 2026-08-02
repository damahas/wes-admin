using System.Collections.Generic;
using System.Threading.Tasks;
using Wes.AI.Models.Entity;
using Wes.AI.Models.ViewModel;

namespace Wes.AI.Services;

/// <summary>
/// 会话持久化服务（按用户隔离）
/// </summary>
public interface ISessionService
{
    Task<List<AiSessionDto>> ListAsync(long userId);
    Task<AiSessionDto?> GetAsync(long userId, long id);
    Task<AiSessionDto> CreateAsync(long userId, CreateSessionRequest req);
    Task<int> UpdateAsync(long userId, long id, UpdateSessionRequest req);
    Task<int> DeleteAsync(long userId, long id);
    Task<int> SaveMessagesAsync(long userId, long id, List<AiMessageDto> messages);
}
