using System;
using System.Collections.Generic;
using System.Text;
using Wes.Service;
using Wes.ViewModel;
using Wes.Utils.Extension;
using Wes.DbModel;
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

        public ResultData<SysFileModel> Save(SysFileModel model)
        {
            var isSuccess = _sysFileService.Save(model);
            if (isSuccess)
            {
                return new ResultData<SysFileModel>(model);
            }
            return new ResultData<SysFileModel>(500, "保存失败");
        }
    }
}
