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
    [Route("flow/instance")]
    public class FlowInstanceController : ControllerBase
    {
        private IFlowInstanceBiz _flowInstanceBiz;

        public FlowInstanceController(IFlowInstanceBiz flowInstanceBiz)
        {
            _flowInstanceBiz = flowInstanceBiz;
        }

        [HttpGet]
        [Route("list")]
        public ReturnData GetList([FromQuery] ParamData<FlowInstanceParam> param)
        {
            return _flowInstanceBiz.GetList(param);
        }

        [HttpGet]
        [Route("{id}")]
        public ReturnData GetById(long id)
        {
            return _flowInstanceBiz.GetById(id);
        }

        [HttpPut]
        public ReturnData Update([FromBody] FlowInstanceModel model)
        {
            return _flowInstanceBiz.Save(model);
        }

        [HttpPost]
        public ReturnData Insert([FromBody] FlowInstanceModel model)
        {
            return _flowInstanceBiz.Save(model);
        }

        [HttpDelete]
        [Route("{ids}")]
        public ReturnData Delete(string ids)
        {
            return _flowInstanceBiz.Delete(ids);
        }


        [HttpGet]
        [Route("delegate/{taskId}/{userId}")]
        public ReturnData TaskDelegate(long taskId, long userId)
        {
            return _flowInstanceBiz.TaskDelegate(taskId, userId);
        }

        [HttpGet]
        [Route("reset/node/{nodeId}")]
        public ReturnData NodeReset(long nodeId)
        {
            return _flowInstanceBiz.NodeReset(nodeId);
        }
    }
}