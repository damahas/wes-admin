using System;
using System.Collections.Generic;
using System.Text;
using Wes.DbModel;
using Wes.Utils.Model;
using Wes.ViewModel.SystemManage;

namespace Wes.Business
{
    public interface ISysOnlineBiz
    {
        public RowData<OnlineInfo> GetList(ParamData<OnlineParam> param);

        public ReturnData Delete(long id);
    }
}

