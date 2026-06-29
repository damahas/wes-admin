using System;
using System.Linq;
using Wes.DbModel;
using Wes.Business;
using Wes.Utils.Model;
using Wes.Utils.Extension;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Wes.ViewModel.FlowManage;

namespace Wes.WebApi.Areas.FlowManage
{
    [ApiController]
    [Route("flow/run")]
    public class FlowRunController : ControllerBase
    {
        private IFlowInstanceBiz _flowInstanceBiz;

        public FlowRunController(IFlowInstanceBiz flowInstanceBiz)
        {
            this._flowInstanceBiz = flowInstanceBiz;
        }

        [HttpGet]
        [Route("img/{id}")]
        public ReturnData GetFlowImg()
        {
            return null;
        }

        [HttpGet]
        [Route("{processCode}/{businessId}")]
        public ReturnData GetRunIni(string processCode, long businessId)
        {
            return _flowInstanceBiz.GetRunIni(processCode, businessId);
        }

        [HttpPost]
        [Route("{processCode}/{businessId}")]
        public ReturnData RunFlow([FromBody] FlowRunModel flowRunModel)
        {
            return _flowInstanceBiz.Exec(flowRunModel);
        }
    }
}

