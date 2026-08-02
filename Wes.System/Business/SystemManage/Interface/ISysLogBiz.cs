using System;
using System.Collections.Generic;
using System.Text;
using Wes.Entity;
using Wes.Utils.Model;
using Wes.ViewModel.SystemManage;

namespace Wes.Business
{
    public interface ISysLogBiz
    {
        public RowData<SysLoginLogEntity> GetLoginList(ParamData<LoginLogParam> param);

        public ReturnData SaveLoginLog(SysLoginLogEntity model);

        public ReturnData DeleteLoginLog(string ids);

        public RowData<SysOperLogEntity> GetOperList(ParamData<OperLogParam> param);
    }
}

