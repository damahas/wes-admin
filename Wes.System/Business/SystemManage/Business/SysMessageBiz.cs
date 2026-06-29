using System;
using System.Linq;
using Wes.Service;
using Wes.Utils.Extension;
using Wes.DbModel;
using Wes.Utils.Model;
using Wes.ViewModel.SystemManage;
using System.Collections.Generic;
using Wes.Utils;

namespace Wes.Business
{
    public class SysMessageBiz : ISysMessageBiz
    {
        private ISysMessageService _sysMessageService;

        public SysMessageBiz(ISysMessageService sysMessageService)
        {
            _sysMessageService = sysMessageService;
        }

        public ResultData<SysMessageModel> GetById(long id)
        {
            return new ResultData<SysMessageModel>(_sysMessageService.GetById(id));
        }

        public ResultData<List<SysMessageModel>> GetAll()
        {
            return new ResultData<List<SysMessageModel>>(_sysMessageService.GetAll());
        }

        public RowData<SysMessageModel> GetList(ParamData<MessageParam> param)
        {
            int total = 0;
            RowData<SysMessageModel> result = new RowData<SysMessageModel>(_sysMessageService.GetList(param, out total));
            result.total = total;
            return result;
        }

        public ReturnData Save(SysMessageModel model)
        {
            if (_sysMessageService.Save(model))
            {
                return new ReturnData();
            }
            return new ReturnData(500, "保存失败！");
        }

        public ReturnData Delete(string ids)
        {
            var delIds = ids.ToLongList();
            if (delIds == null || delIds.Count == 0)
            {
                return new ReturnData(500, "参数有误，请检查参数！");
            }
            if (_sysMessageService.Delete(delIds))
            {
                return new ReturnData();
            }
            return new ReturnData(500, "删除失败！");
        }

        public ReturnData ReadAll()
        {
            _sysMessageService.ReadAll();
            return new ReturnData();
        }

        public ReturnData Read(long id)
        {
            var message = _sysMessageService.GetById(id);
            if (message == null)
            {
                return new ReturnData(500, "找不到消息");
            }
            if (message.UserId != GlobalContext.CurrentUser.UserId)
            {
                return new ReturnData(500, "请勿处理其他人消息");
            }
            if (message.IsRead == 0)
            {
                message.IsRead = 1;
                message.ReadTime = DateTime.Now;
                _sysMessageService.Save(message);
            }
            return new ReturnData();
        }

        public ReturnData Read(List<long> ids)
        {
            _sysMessageService.Read(ids);
            return new ReturnData();
        }
    }
}
