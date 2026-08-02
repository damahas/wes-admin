using System;
using System.Collections.Generic;
using System.Text;
using Wes.Entity;
using Wes.Utils.Model;
using Wes.ViewModel.SystemManage;

namespace Wes.Service
{
    public interface ISysConfigService
    {
        #region 参数操作

        public SysConfigEntity GetById(long configId);

        public List<SysConfigEntity> GetByIds(List<long> ids);

        public SysConfigEntity GetByConfigKey(string configKey);

        public List<SysConfigEntity> GetList(ParamData<ConfigParam> param, out int total);

        public List<SysConfigEntity> GetAll();

        public bool Save(SysConfigEntity config);

        public bool Delete(List<long> ids);

        public void UpdateSort(List<long> configIds);

        #endregion
    }
}
