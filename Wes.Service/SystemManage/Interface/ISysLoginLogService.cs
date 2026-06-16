using System;
using System.Collections.Generic;
using System.Text;
using Wes.DbModel;
using Wes.Utils.Model;
using Wes.ViewModel.SystemManage;

namespace Wes.Service
{
    public interface ISysLoginLogService
    {
        public List<SysLoginLogModel> GetList(ParamData<LoginLogParam> param, out int total);

        public bool Save(SysLoginLogModel model);

        public bool Delete(List<long> ids);

        public bool DeleteAll();
    }
}

