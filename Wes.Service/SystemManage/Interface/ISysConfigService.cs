using System;
using System.Collections.Generic;
using System.Text;
using Wes.DbModel;
using Wes.Utils.Model;
using Wes.ViewModel.SystemManage;

namespace Wes.Service
{
    public interface ISysConfigService
    {
        #region 参数操作

        public SysConfigModel GetById(long configId);

        public List<SysConfigModel> GetByIds(List<long> ids);

        public SysConfigModel GetByConfigKey(string configKey);

        public List<SysConfigModel> GetList(ParamData<ConfigParam> param, out int total);

        public List<SysConfigModel> GetAll();

        public bool Save(SysConfigModel config);

        public bool Delete(List<long> ids);

        #endregion
    }
}
