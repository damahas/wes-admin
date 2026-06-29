using System;
using System.Linq;
using Wes.DbModel;
using Wes.Business;
using Wes.Utils.Model;
using Wes.Utils.Extension;
using Wes.ViewModel.SystemManage;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Wes.WebApi.Areas.SystemManage
{
    [ApiController]
    [Route("system/message")]
    public class MessageController : ControllerBase
    {
        private ISysMessageBiz _sysMessageBiz;

        public MessageController(ISysMessageBiz sysMessageBiz)
        {
            _sysMessageBiz = sysMessageBiz;
        }

        [HttpGet]
        [Route("list")]
        public ReturnData GetList([FromQuery] ParamData<MessageParam> param)
        {
            return _sysMessageBiz.GetList(param);
        }

        [HttpGet]
        [Route("{id}")]
        public ReturnData GetById(long id)
        {
            return _sysMessageBiz.GetById(id);
        }

        [HttpPost]
        [Route("read")]
        public ReturnData Reads(List<long> ids)
        {
            return _sysMessageBiz.Read(ids);
        }

        [HttpPost]
        [Route("read/{id}")]
        public ReturnData Read(long id)
        {
            return _sysMessageBiz.Read(id);
        }

        [HttpPost]
        [Route("read/all")]
        public ReturnData ReadAll()
        {
            return _sysMessageBiz.ReadAll();
        }

        [HttpDelete]
        [Route("{ids}")]
        public ReturnData Delete(string ids)
        {
            return _sysMessageBiz.Delete(ids);
        }

    }
}
