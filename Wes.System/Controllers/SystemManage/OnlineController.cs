using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Wes.Business;
using Wes.DbModel;
using Wes.Utils.Model;
using Wes.ViewModel.SystemManage;

namespace Wes.WebApi.Areas.SystemManage
{
    [ApiController]
    [Route("system/online")]
    public class OnlineController : ControllerBase
    {
        private ISysOnlineBiz _onlineBiz;

        public OnlineController(ISysOnlineBiz onlineBiz)
        {
            _onlineBiz = onlineBiz;
        }

        [HttpGet]
        [Route("list")]
        public ReturnData GetList([FromQuery] ParamData<OnlineParam> param)
        {
            return _onlineBiz.GetList(param);
        }

        [HttpDelete]
        [Route("{id}")]
        public ReturnData Delete(long id)
        {
            return _onlineBiz.Delete(id);
        }
    }
}