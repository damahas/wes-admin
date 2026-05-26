using System;
using System.Linq;
using Wes.DbModel;
using Wes.Business;
using Wes.Utils.Model;
using Wes.Utils.Extension;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Wes.WebApi.Areas.SystemManage
{
    [ApiController]
    [Route("flow/process")]
    public class FlowProcessController : ControllerBase
    {
        private IFlowProcessBiz _flowProcessBiz;

        public FlowProcessController(IFlowProcessBiz flowProcessBiz)
        {
            _flowProcessBiz = flowProcessBiz;
        }

        #region 流程管理

        [HttpGet]
        [Route("list")]
        public ReturnData GetList([FromQuery] ParamData<FlowProcessModel> param)
        {
            return _flowProcessBiz.GetList(param);
        }

        [HttpGet]
        [Route("all")]
        public ReturnData GetAll()
        {
            return _flowProcessBiz.GetAll();
        }

        [HttpGet]
        [Route("{id}")]
        public ReturnData GetById(long id)
        {
            return _flowProcessBiz.GetById(id);
        }

        [HttpPut]
        public ReturnData Update([FromBody] FlowProcessModel model)
        {
            return _flowProcessBiz.Save(model);
        }

        [HttpPost]
        public ReturnData Insert([FromBody] FlowProcessModel model)
        {
            return _flowProcessBiz.Save(model);
        }

        [HttpDelete]
        [Route("{ids}")]
        public ReturnData Delete(string ids)
        {
            return _flowProcessBiz.Delete(ids);
        }

        [HttpGet]
        [Route("version/list")]
        public ReturnData GetVersionList([FromQuery] ParamData<FlowProcessVersionModel> param)
        {
            return _flowProcessBiz.GetVersionList(param);
        }

        #endregion

        #region 流程版本管理

        [HttpGet]
        [Route("version/{id}")]
        public ReturnData GetVersionById(long id)
        {
            return _flowProcessBiz.GetVersionById(id);
        }

        [HttpPut]
        [Route("version")]
        public ReturnData UpdateVersion([FromBody] FlowProcessVersionModel model)
        {
            return _flowProcessBiz.SaveVersion(model);
        }

        [HttpPost]
        [Route("version")]
        public ReturnData InsertVersion([FromBody] FlowProcessVersionModel model)
        {
            return _flowProcessBiz.SaveVersion(model);
        }

        [HttpDelete]
        [Route("version/{id}")]
        public ReturnData DeleteVersion(long id)
        {
            return _flowProcessBiz.DeleteVersion(id);
        }

        [HttpGet]
        [Route("version/copy/{id}")]
        public ReturnData CopyVersion(long id)
        {
            return _flowProcessBiz.CopyVersion(id);
        }

        [HttpGet]
        [Route("version/use/{id}")]
        public ReturnData UseVersion(long id)
        {
            return _flowProcessBiz.UseVersion(id);
        }

        #endregion
    }
}
