using System;
using System.Collections.Generic;
using System.Text;
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
    }
}
