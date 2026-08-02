using System;
using System.Collections.Generic;
using System.Text;
using Wes.Service;
using Wes.ViewModel;
using Wes.Utils.Extension;
using Wes.Entity;
using Wes.Utils.Model;
using Wes.Utils;
using System.Linq;
using Wes.ViewModel.SystemManage;

namespace Wes.Business
{
    public class SysFileBiz : ISysFileBiz
    {
        private ISysFileService _sysFileService;

        public SysFileBiz(ISysFileService sysFileService)
        {
            _sysFileService = sysFileService;
        }

        public ResultData<SysFileEntity> Save(SysFileEntity model)
        {
            var isSuccess = _sysFileService.Save(model);
            if (isSuccess)
            {
                return new ResultData<SysFileEntity>(model);
            }
            return new ResultData<SysFileEntity>(500, "保存失败");
        }
    }
}
