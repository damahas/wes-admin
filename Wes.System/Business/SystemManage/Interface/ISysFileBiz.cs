using System;
using System.Collections.Generic;
using Wes.Entity;
using Wes.Utils.Model;
using Wes.ViewModel.SystemManage;

namespace Wes.Business
{
    public interface ISysFileBiz
    {
        public ResultData<SysFileEntity> Save(SysFileEntity model);
    }
}
