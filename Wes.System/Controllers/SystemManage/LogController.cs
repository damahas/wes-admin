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
    [Route("system/log")]
    public class LogController : ControllerBase
    {
        private ISysLogBiz _sysLogBiz;

        public LogController(ISysLogBiz sysLogBiz)
        {
            _sysLogBiz = sysLogBiz;
        }

        [HttpGet]
        [Route("login/list")]
        public ReturnData GetLoginLogList([FromQuery]ParamData<LoginLogParam> param)
        {
            return _sysLogBiz.GetLoginList(param);
        }


        [HttpGet]
        [Route("oper/list")]
        public ReturnData GetOperLogList([FromQuery] ParamData<OperLogParam> param)
        {
            return _sysLogBiz.GetOperList(param);
        }
    }
}