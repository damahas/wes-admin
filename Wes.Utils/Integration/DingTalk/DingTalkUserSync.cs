using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Wes.Utils.Integration
{
    /// <summary>
    /// 钉钉用户同步器（仅负责拉取 + 格式转换，不做数据持久化）。
    /// 持久化逻辑由 Wes.Service 层的 SyncDataSaveService 负责。
    /// </summary>
    public class DingTalkUserSync
    {
        private readonly DingTalkClient _client;

        public DingTalkUserSync(DingTalkClient client)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }

        /// <summary>
        /// 拉取全部部门下的全部用户（自动去重），转换为统一格式
        /// </summary>
        public async Task<List<SyncUserData>> GetAllAsync()
        {
            var allDepts = await _client.GetAllDepartmentsAsync();
            var syncedUserIds = new HashSet<string>();
            var result = new List<SyncUserData>();

            foreach (var dept in allDepts)
            {
                try
                {
                    var users = await _client.GetAllUsersByDeptAsync(dept.DeptId);
                    foreach (var du in users)
                    {
                        if (syncedUserIds.Contains(du.UserId))
                            continue;

                        syncedUserIds.Add(du.UserId);
                        result.Add(MapToSyncData(du));
                    }
                }
                catch
                {
                    // 单个部门拉取失败不影响整体
                }
            }

            return result;
        }

        /// <summary>
        /// 按钉钉用户 ID 拉取单个用户并转换为统一格式
        /// </summary>
        public async Task<SyncUserData> GetByIdAsync(string userId)
        {
            var du = await _client.GetUserDetailAsync(userId);
            return MapToSyncData(du);
        }

        /// <summary>
        /// 将钉钉用户数据映射为统一格式
        /// </summary>
        private static SyncUserData MapToSyncData(DingTalkUserItem du)
        {
            return new SyncUserData
            {
                ThirdPartyUserId = du.UserId,
                ThirdPartyDeptIds = du.DeptIdList?.Select(d => d.ToString()).ToList() ?? new List<string>(),
                UserName = du.Name ?? du.UserId,
                Account = !string.IsNullOrEmpty(du.JobNumber) ? du.JobNumber : du.UserId,
                Email = du.Email,
                Mobile = du.Mobile,
                Avatar = du.Avatar,
                Title = du.Title,
                JobNumber = du.JobNumber,
                RawData = JsonConvert.SerializeObject(du)
            };
        }
    }
}
