using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Wes.Business;
using Wes.Utils.Model;
using Wes.Entity;
using Wes.ViewModel.SystemManage;

namespace Wes.WebApi.Areas.SystemManage
{
    [ApiController]
    [Route("system/i18n")]
    public class I18nController : ControllerBase
    {
        private ISysI18nBiz _sysI18nBiz;

        public I18nController(ISysI18nBiz sysI18nBiz)
        {
            this._sysI18nBiz = sysI18nBiz;
        }

        [HttpGet]
        [Route("list")]
        public ReturnData GetList([FromQuery] ParamData<I18nParam> param)
        {
            return _sysI18nBiz.GetList(param);
        }

        [HttpGet]
        [Route("{id}")]
        public ReturnData GetById(long id)
        {
            return _sysI18nBiz.GetById(id);
        }

        [HttpPost]
        public ReturnData Insert([FromBody] SysI18nEntity model)
        {
            return _sysI18nBiz.Save(model);
        }

        [HttpPut]
        public ReturnData Update([FromBody] SysI18nEntity model)
        {
            return _sysI18nBiz.Save(model);
        }

        [HttpDelete]
        [Route("{ids}")]
        public ReturnData Delete(string ids)
        {
            return _sysI18nBiz.Delete(ids);
        }

        [HttpPost]
        [Route("refresh")]
        public ReturnData RefreshCache()
        {
            return _sysI18nBiz.RefreshCache();
        }

        [HttpPost]
        [Route("seed")]
        public ReturnData Seed()
        {
            return _sysI18nBiz.Seed();
        }

        [HttpGet]
        [Route("frontend")]
        public ReturnData GetFrontendTranslations([FromQuery] string lang)
        {
            return _sysI18nBiz.GetFrontendTranslations(lang);
        }
    }
}
