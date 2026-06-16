using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Wes.DbModel;
using Wes.Service;
using Wes.Utils.Extension;
using Wes.Utils.Model;
using Wes.ViewModel.SystemManage;
using Wes.Utils.Cache;
using Wes.Utils;

namespace Wes.Business
{
    public class SysOnlineBiz : ISysOnlineBiz
    {
        private ISysUserService _sysUserService;

        public SysOnlineBiz(ISysUserService sysUserService)
        {
            _sysUserService = sysUserService;
        }

        public RowData<OnlineInfo> GetList(ParamData<OnlineParam> param)
        {
            int total = 0;
            RowData<OnlineInfo> result = new RowData<OnlineInfo>(_sysUserService.GetOnlineList(param, out total));
            result.total = total;
            return result;
        }

        public ReturnData Delete(long id)
        {
            var token = _sysUserService.InvalidToken(id);
            if (token != null)
            {
                CacheFactory.Cache.RemoveCache($"{CacheKey.AccessToken}{token.Token}");
            }
            return new ReturnData();
        }
    }
}

