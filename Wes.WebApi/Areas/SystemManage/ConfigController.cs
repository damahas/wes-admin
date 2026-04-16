using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wes.Business;
using Wes.DbModel;
using Wes.Utils.Model;
using Wes.ViewModel.SystemManage;
using static Org.BouncyCastle.Math.EC.ECCurve;

namespace Wes.WebApi.Areas.SystemManage
{
    [ApiController]
    [Route("system/config")]
    public class ConfigController : ControllerBase
    {
        private ISysConfigBiz _sysConfigBiz;

        public ConfigController(ISysConfigBiz sysConfigBiz)
        {
            _sysConfigBiz = sysConfigBiz;
        }

        [HttpGet]
        [Route("list")]
        public ReturnData GetList([FromQuery] ParamData<ConfigParam> param)
        {
            return _sysConfigBiz.GetList(param);
        }

        [HttpGet]
        [Route("{id}")]
        public ReturnData GetById(long id)
        {
            return _sysConfigBiz.GetById(id);
        }

        [HttpPut]
        public ReturnData Update([FromBody] SysConfigModel config)
        {
            return _sysConfigBiz.Save(config);
        }

        [HttpPost]
        public ReturnData Insert([FromBody] SysConfigModel config)
        {
            return _sysConfigBiz.Save(config);
        }

        [HttpDelete]
        [Route("{ids}")]
        public ReturnData Delete(string ids)
        {
            return _sysConfigBiz.Delete(ids);
        }

        [HttpPost]
        [Route("export")]
        public FileContentResult Export([FromForm] ParamData<ConfigParam> param)
        {
            return File(_sysConfigBiz.Export(param), "application/ms-excel");
        }

        [HttpGet]
        [Route("configKey/{configKey}")]
        public ReturnData GetByConfigKey(string configKey)
        {
            return new ReturnData(200, _sysConfigBiz.GetByConfigKey(configKey));
        }

        [HttpDelete]
        [Route("refreshCache")]
        public ReturnData RefreshCache()
        {
            return _sysConfigBiz.Refresh();
        }
    }
}