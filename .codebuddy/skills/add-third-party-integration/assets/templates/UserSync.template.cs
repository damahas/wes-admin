using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Wes.Utils.Integration
{
    /// <summary>
    /// {PlatformName} 用户同步器（仅负责拉取 + 格式转换，不做数据持久化）。
    /// 持久化逻辑由 Wes.Service 层的 SyncDataSaveService 负责。
    /// </summary>
    public class {Platform}UserSync
    {
        private readonly {Platform}Client _client;

        public {Platform}UserSync({Platform}Client client)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }

        /// <summary>
        /// 拉取全部部门下的全部用户并转换为统一格式
        /// </summary>
        public async Task<List<SyncUserData>> GetAllAsync()
        {
            // TODO: 调 _client 获取 {PlatformName} 原始用户数据
            // 然后映射为 SyncUserData 列表
            var result = new List<SyncUserData>();
            // var rawUsers = await _client.GetAllUsersAsync();
            // foreach (var du in rawUsers)
            // {
            //     result.Add(new SyncUserData
            //     {
            //         ThirdPartyUserId = du.UserId,
            //         ThirdPartyDeptIds = du.DeptIdList ?? new List<string>(),
            //         UserName = du.Name ?? du.UserId,
            //         Account = du.Account ?? du.UserId,
            //         Email = du.Email,
            //         Mobile = du.Mobile,
            //         Avatar = du.Avatar,
            //         Title = du.Title,
            //         JobNumber = du.JobNumber,
            //         RawData = JsonConvert.SerializeObject(du)
            //     });
            // }
            return result;
        }

        /// <summary>
        /// 按 {PlatformName} 用户 ID 拉取单个用户并转换为统一格式
        /// </summary>
        public async Task<SyncUserData> GetByIdAsync(string userId)
        {
            // TODO: 调 _client 获取单个用户，映射为 SyncUserData
            throw new NotImplementedException();
        }
    }
}
