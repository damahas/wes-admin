using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Wes.Utils.Integration
{
    /// <summary>
    /// 钉钉部门同步器（仅负责拉取 + 格式转换，不做数据持久化）。
    /// 持久化逻辑由 Wes.Service 层的 SyncDataSaveService 负责。
    /// </summary>
    public class DingTalkDeptSync
    {
        private readonly DingTalkClient _client;

        public DingTalkDeptSync(DingTalkClient client)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }

        /// <summary>
        /// 拉取全部钉钉部门并转换为统一格式
        /// </summary>
        public async Task<List<SyncDeptData>> GetAllAsync()
        {
            var dingDepts = await _client.GetAllDepartmentsAsync();
            var result = new List<SyncDeptData>();

            foreach (var dd in dingDepts)
            {
                result.Add(MapToSyncData(dd));
            }

            return result;
        }

        /// <summary>
        /// 按钉钉部门 ID 拉取单个部门并转换为统一格式
        /// </summary>
        public async Task<SyncDeptData> GetByIdAsync(long deptId)
        {
            var dd = await _client.GetDepartmentDetailAsync(deptId);
            return MapToSyncData(dd);
        }

        /// <summary>
        /// 将钉钉部门数据映射为统一格式
        /// </summary>
        private static SyncDeptData MapToSyncData(DingTalkDeptItem dd)
        {
            return new SyncDeptData
            {
                ThirdPartyDeptId = dd.DeptId.ToString(),
                ThirdPartyParentId = dd.ParentId > 0 ? dd.ParentId.ToString() : "0",
                DeptName = dd.Name,
                OrderNum = dd.Order,
                RawData = JsonConvert.SerializeObject(dd)
            };
        }
    }
}
