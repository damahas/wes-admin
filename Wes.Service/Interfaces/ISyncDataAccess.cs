using System.Threading.Tasks;
using Wes.Utils.Integration;

namespace Wes.Service
{
    /// <summary>
    /// 同步数据访问抽象接口。
    /// 将三方集成与具体数据库操作解耦，在 Wes.Service 层实现此接口。
    /// 所有方法使用统一模型，不依赖 Wes.DbModel。
    /// </summary>
    public interface ISyncDataAccess
    {
        /// <summary>根据三方 ID 查询本地部门</summary>
        Task<SyncDeptData> GetDeptByThirdPartyIdAsync(string thirdPartyId);

        /// <summary>根据三方 ID 查询本地用户</summary>
        Task<SyncUserData> GetUserByThirdPartyIdAsync(string thirdPartyId);

        /// <summary>
        /// 保存三方映射关系
        /// </summary>
        /// <param name="provider">平台标识：DingTalk / WeCom / Feishu</param>
        /// <param name="entityType">实体类型：Dept / User</param>
        /// <param name="thirdPartyId">三方 ID</param>
        /// <param name="localId">本地 ID</param>
        /// <param name="displayName">显示名称</param>
        Task<bool> SaveThirdPartyMappingAsync(string provider, string entityType, string thirdPartyId, string localId, string displayName);

        /// <summary>根据本地用户 ID 查询三方映射中的三方用户 ID</summary>
        Task<string> GetThirdPartyUserIdAsync(long localUserId);
    }
}
