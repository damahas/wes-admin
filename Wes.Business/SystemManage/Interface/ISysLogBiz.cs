using System;
using System.Collections.Generic;
using System.Text;
using Wes.DbModel;
using Wes.Utils.Model;
using Wes.ViewModel.SystemManage;

namespace Wes.Business
{
    public interface ISysLogBiz
    {
        public RowData<SysLoginLogModel> GetLoginList(ParamData<LoginLogParam> param);

        public ReturnData SaveLoginLog(SysLoginLogModel model);

        public ReturnData DeleteLoginLog(string ids);

        public RowData<SysOperLogModel> GetOperList(ParamData<OperLogParam> param);
    }
}

