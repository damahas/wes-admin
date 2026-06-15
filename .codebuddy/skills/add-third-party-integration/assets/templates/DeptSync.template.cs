using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Wes.Utils.Integration
{
    /// <summary>
    /// {PlatformName} 部门同步器（仅负责拉取 + 格式转换，不做数据持久化）。
    /// 持久化逻辑由 Wes.Service 层的 SyncDataSaveService 负责。
    /// </summary>
    public class {Platform}DeptSync
    {
        private readonly {Platform}Client _client;

        public {Platform}DeptSync({Platform}Client client)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }

        /// <summary>
        /// 拉取全部 {PlatformName} 部门并转换为统一格式
        /// </summary>
        public async Task<List<SyncDeptData>> GetAllAsync()
        {
            // TODO: 调 _client 获取 {PlatformName} 原始部门数据
            // 然后映射为 SyncDeptData 列表
            var result = new List<SyncDeptData>();
            // var rawDepts = await _client.GetAllDepartmentsAsync();
            // foreach (var dd in rawDepts)
            // {
            //     result.Add(new SyncDeptData
            //     {
            //         ThirdPartyDeptId = dd.Id.ToString(),
            //         ThirdPartyParentId = dd.ParentId?.ToString() ?? "0",
            //         DeptName = dd.Name,
            //         OrderNum = dd.Order,
            //         RawData = JsonConvert.SerializeObject(dd)
            //     });
            // }
            return result;
        }

        /// <summary>
        /// 按 {PlatformName} 部门 ID 拉取单个部门并转换为统一格式
        /// </summary>
        public async Task<SyncDeptData> GetByIdAsync(string thirdPartyDeptId)
        {
            // TODO: 调 _client 获取单个部门，映射为 SyncDeptData
            throw new NotImplementedException();
        }
    }
}
