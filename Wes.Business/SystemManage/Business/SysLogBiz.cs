using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Wes.DbModel;
using Wes.Service;
using Wes.Utils.Extension;
using Wes.Utils.Model;
using Wes.ViewModel.SystemManage;

namespace Wes.Business
{
    public class SysLogBiz : ISysLogBiz
    {
        private ISysLoginLogService _sysLoginLogService;
        private ISysOperLogService _sysOperLogService;

        public SysLogBiz(ISysLoginLogService sysLoginLogService, ISysOperLogService sysOperLogService)
        {
            _sysLoginLogService = sysLoginLogService;
            _sysOperLogService = sysOperLogService;

        }

        public RowData<SysLoginLogModel> GetLoginList(ParamData<LoginLogParam> param)
        {
            int total = 0;
            RowData<SysLoginLogModel> result = new RowData<SysLoginLogModel>(_sysLoginLogService.GetList(param, out total));
            result.total = total;
            return result;
        }

        public ReturnData SaveLoginLog(SysLoginLogModel model)
        {
            if (_sysLoginLogService.Save(model))
            {
                return new ReturnData();
            }
            return new ReturnData(500, "保存失败！");
        }

        public ReturnData DeleteLoginLog(string ids)
        {
            var dicIds = ids.ToLongList();
            if (dicIds == null || dicIds.Count == 0)
            {
                return new ReturnData(500, "参数有误，请检查参数！");
            }
            if (_sysLoginLogService.Delete(dicIds))
            {
                return new ReturnData();
            }
            return new ReturnData(500, "删除失败！");
        }

        public RowData<SysOperLogModel> GetOperList(ParamData<OperLogParam> param)
        {
            int total = 0;
            RowData<SysOperLogModel> result = new RowData<SysOperLogModel>(_sysOperLogService.GetList(param, out total));
            result.total = total;
            return result;
        }

    }
}

