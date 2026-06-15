using System.Collections.Generic;
using System.Threading.Tasks;
using Wes.Utils.Integration;

namespace Wes.Service
{
    /// <summary>
    /// 三方集成数据保存接口。
    /// 接收 Utils 层返回的统一格式部门/用户数据，执行入库及三方映射关系建立。
    /// </summary>
    public interface ISyncDataSaveService
    {
        /// <summary>
        /// 保存部门数据（自动按层级排序、解析 ParentId/Ancestors、建立三方映射）
        /// </summary>
        /// <param name="provider">平台标识：DingTalk / WeCom / Feishu</param>
        /// <param name="depts">统一格式的部门数据</param>
        /// <returns>同步结果</returns>
        Task<SyncSaveResult> SaveDeptsAsync(string provider, List<SyncDeptData> depts);

        /// <summary>
        /// 保存用户数据（自动关联部门、建立三方映射）
        /// </summary>
        /// <param name="provider">平台标识：DingTalk / WeCom / Feishu</param>
        /// <param name="users">统一格式的用户数据</param>
        /// <returns>同步结果</returns>
        Task<SyncSaveResult> SaveUsersAsync(string provider, List<SyncUserData> users);
    }

    /// <summary>
    /// 同步保存结果
    /// </summary>
    public class SyncSaveResult
    {
        public bool Success { get; set; }
        public int CreatedCount { get; set; }
        public int UpdatedCount { get; set; }
        public int FailedCount { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
    }
}
