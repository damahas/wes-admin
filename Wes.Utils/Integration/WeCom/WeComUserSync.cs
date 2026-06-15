using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Wes.Utils.Integration
{
    /// <summary>
    /// 企业微信用户同步器（仅负责拉取 + 格式转换，不做数据持久化）。
    /// </summary>
    public class WeComUserSync
    {
        private readonly WeComClient _client;

        public WeComUserSync(WeComClient client)
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
        /// 按企微用户 ID 拉取单个用户并转换为统一格式
        /// </summary>
        public async Task<SyncUserData> GetByIdAsync(string userId)
        {
            var wu = await _client.GetUserDetailAsync(userId);
            return MapToSyncData(wu);
        }

        /// <summary>
        /// 将企微用户数据映射为统一格式
        /// </summary>
        private static SyncUserData MapToSyncData(WeComUserItem wu)
        {
            return new SyncUserData
            {
                ThirdPartyUserId = wu.UserId,
                ThirdPartyDeptIds = wu.Department?.Select(d => d.ToString()).ToList() ?? new List<string>(),
                UserName = wu.Name ?? wu.UserId,
                Account = !string.IsNullOrEmpty(wu.JobNumber) ? wu.JobNumber : wu.UserId,
                Email = wu.Email,
                Mobile = wu.Mobile,
                Avatar = wu.Avatar,
                Title = wu.Position,
                JobNumber = wu.JobNumber,
                RawData = JsonConvert.SerializeObject(wu)
            };
        }
    }
}
