using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Wes.Utils.Integration
{
    /// <summary>
    /// 企业微信部门同步器（仅负责拉取 + 格式转换，不做数据持久化）。
    /// </summary>
    public class WeComDeptSync
    {
        private readonly WeComClient _client;

        public WeComDeptSync(WeComClient client)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }

        /// <summary>
        /// 拉取全部企微部门并转换为统一格式
        /// </summary>
        public async Task<List<SyncDeptData>> GetAllAsync()
        {
            var wecomDepts = await _client.GetAllDepartmentsAsync();
            var result = new List<SyncDeptData>();

            foreach (var wd in wecomDepts)
            {
                result.Add(MapToSyncData(wd));
            }

            return result;
        }

        /// <summary>
        /// 将企微部门数据映射为统一格式
        /// </summary>
        private static SyncDeptData MapToSyncData(WeComDeptItem wd)
        {
            return new SyncDeptData
            {
                ThirdPartyDeptId = wd.Id.ToString(),
                ThirdPartyParentId = wd.ParentId > 0 ? wd.ParentId.ToString() : "0",
                DeptName = wd.Name,
                OrderNum = (int)wd.Order,
                RawData = JsonConvert.SerializeObject(wd)
            };
        }
    }
}
