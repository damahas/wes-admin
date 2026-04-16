using System;
using System.Collections.Generic;
using Wes.DbModel;
using Wes.Utils.Model;
using Wes.ViewModel.SystemManage;

namespace Wes.Business
{
    public interface ISysFlieBiz
    {
        public ResultData<SysFileModel> Save(SysFileModel model);
    }
}
