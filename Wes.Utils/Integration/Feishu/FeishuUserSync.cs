using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Wes.Utils.Integration
{
    /// <summary>
    /// 飞书用户同步器（仅负责拉取 + 格式转换，不做数据持久化）。
    /// </summary>
    public class FeishuUserSync
    {
        private readonly FeishuClient _client;

        public FeishuUserSync(FeishuClient client)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }

        /// <summary>
        /// 拉取全部部门下的全部用户（自动去重），转换为统一格式
        /// </summary>
        public async Task<List<SyncUserData>> GetAllAsync()
        {
            var allUsers = await _client.GetAllUsersAsync();
            return allUsers.Select(MapToSyncData).ToList();
        }

        /// <summary>
        /// 按飞书用户 OpenId 拉取单个用户并转换为统一格式
        /// </summary>
        public async Task<SyncUserData> GetByIdAsync(string openId)
        {
            var fu = await _client.GetUserDetailAsync(openId);
            return MapToSyncData(fu);
        }

        /// <summary>
        /// 将飞书用户数据映射为统一格式
        /// </summary>
        private static SyncUserData MapToSyncData(FeishuUserItem fu)
        {
            return new SyncUserData
            {
                ThirdPartyUserId = fu.OpenId,
                ThirdPartyDeptIds = fu.DepartmentIds ?? new List<string>(),
                UserName = fu.Name ?? fu.OpenId,
                Account = !string.IsNullOrEmpty(fu.EmployeeNo) ? fu.EmployeeNo : fu.OpenId,
                Email = fu.Email,
                Mobile = fu.Mobile,
                Avatar = fu.AvatarUrl,
                Title = fu.JobTitle,
                JobNumber = fu.EmployeeNo,
                RawData = JsonConvert.SerializeObject(fu)
            };
        }
    }
}
