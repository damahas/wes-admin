using System;
using System.Collections.Generic;
using System.Text;
using Wes.Entity;
using Wes.Utils.Model;
using Wes.ViewModel.SystemManage;

namespace Wes.Service
{
    public interface ISysLoginLogService
    {
        public List<SysLoginLogEntity> GetList(ParamData<LoginLogParam> param, out int total);

        public bool Save(SysLoginLogEntity model);

        public bool Delete(List<long> ids);

        public bool DeleteAll();
    }
}

