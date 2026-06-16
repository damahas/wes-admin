using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Wes.DbModel;
using Wes.Utils.Model;
using Wes.ViewModel.SystemManage;

namespace Wes.Business
{
    public interface ISysConfigBiz
    {
        #region 配置操作

        public RowData<SysConfigModel> GetList(ParamData<ConfigParam> param);

        public ResultData<SysConfigModel> GetById(long id);

        public ReturnData Save(SysConfigModel config);

        public ReturnData Delete(string ids);

        public byte[] Export(ParamData<ConfigParam> param);
        #endregion

        public ReturnData Refresh();

        public string GetByConfigKey(string configKey);

        #region 调用三方接口

        /// <summary>
        /// 同步指定三方平台的组织架构（部门 + 用户）
        /// </summary>
        /// <param name="provider">平台标识：dingtalk / feishu / wecom</param>
        Task<ReturnData> SyncThirdPartyAsync(string provider);

        #endregion
    }
}
