using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Wes.Utils.Integration
{
    /// <summary>
    /// 飞书部门同步器（仅负责拉取 + 格式转换，不做数据持久化）。
    /// </summary>
    public class FeishuDeptSync
    {
        private readonly FeishuClient _client;

        public FeishuDeptSync(FeishuClient client)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }

        /// <summary>
        /// 拉取全部飞书部门并转换为统一格式
        /// </summary>
        public async Task<List<SyncDeptData>> GetAllAsync()
        {
            var feishuDepts = await _client.GetAllDepartmentsAsync();
            var result = new List<SyncDeptData>();

            foreach (var fd in feishuDepts)
            {
                result.Add(MapToSyncData(fd));
            }

            return result;
        }

        /// <summary>
        /// 将飞书部门数据映射为统一格式
        /// </summary>
        private static SyncDeptData MapToSyncData(FeishuDeptItem fd)
        {
            var orderNum = 0;
            int.TryParse(fd.Order, out orderNum);

            return new SyncDeptData
            {
                ThirdPartyDeptId = fd.DepartmentId,
                ThirdPartyParentId = string.IsNullOrEmpty(fd.ParentDepartmentId) ? "0" : fd.ParentDepartmentId,
                DeptName = fd.Name,
                OrderNum = orderNum,
                RawData = JsonConvert.SerializeObject(fd)
            };
        }
    }
}
