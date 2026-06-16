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
    [Route("system/post")]
    public class PostController : ControllerBase
    {
        private ISysPostBiz _sysPostBiz;

        public PostController(ISysPostBiz sysPostBiz)
        {
            _sysPostBiz = sysPostBiz;
        }

        [HttpGet]
        [Route("list")]
        public ReturnData GetList([FromQuery] ParamData<PostParam> param)
        {
            return _sysPostBiz.GetList(param);
        }

        [HttpGet]
        [Route("{id}")]
        public ReturnData GetById(long id)
        {
            return _sysPostBiz.GetById(id);
        }

        [HttpPut]
        public ReturnData Update([FromBody] SysPostModel role)
        {
            return _sysPostBiz.Save(role);
        }

        [HttpPost]
        public ReturnData Insert([FromBody] SysPostModel role)
        {
            return _sysPostBiz.Save(role);
        }

        [HttpDelete]
        [Route("{ids}")]
        public ReturnData Delete(string ids)
        {
            return _sysPostBiz.Delete(ids);
        }

        [HttpPost]
        [Route("export")]
        public FileContentResult Export([FromForm] ParamData<PostParam> param)
        {
            return File(_sysPostBiz.Export(param), "application/ms-excel");
        }
    }
}