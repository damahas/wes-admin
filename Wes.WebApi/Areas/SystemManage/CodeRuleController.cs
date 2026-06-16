using System;
using System.Linq;
using Wes.DbModel;
using Wes.Business;
using Wes.Utils.Model;
using Wes.Utils.Extension;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Wes.ViewModel.SystemManage;

namespace Wes.WebApi.Areas.SystemManage
{
    [ApiController]
    [Route("system/coderule")]
    public class CodeRuleController : ControllerBase
    {
        private ISysCodeRuleBiz _sysCodeRuleBiz;

        public CodeRuleController(ISysCodeRuleBiz sysCodeRuleBiz)
        {
            _sysCodeRuleBiz = sysCodeRuleBiz;
        }

        [HttpGet]
        [Route("list")]
        public ReturnData GetList([FromQuery] ParamData<SysCodeRuleModel> param)
        {
            return _sysCodeRuleBiz.GetList(param);
        }

        [HttpGet]
        [Route("all")]
        public ReturnData GetAll()
        {
            return _sysCodeRuleBiz.GetAll();
        }

        [HttpGet]
        [Route("{id}")]
        public ReturnData GetById(long id)
        {
            return _sysCodeRuleBiz.GetById(id);
        }

        [HttpPut]
        public ReturnData Update([FromBody] CodeRuleInfo model)
        {
            return _sysCodeRuleBiz.Save(model);
        }

        [HttpPost]
        public ReturnData Insert([FromBody] CodeRuleInfo model)
        {
            return _sysCodeRuleBiz.Save(model);
        }

        [HttpDelete]
        [Route("{ids}")]
        public ReturnData Delete(string ids)
        {
            return _sysCodeRuleBiz.Delete(ids);
        }

        [HttpGet]
        [Route("gen/{ruleCode}")]
        public ReturnData Gen(string ruleCode)
        {
            string errorMsg = string.Empty;
            string genCode = _sysCodeRuleBiz.GetCode(ruleCode, out errorMsg);
            if (!string.IsNullOrWhiteSpace(errorMsg))
            {
                return new ReturnData(400, errorMsg);
            }
            return new ResultData<string>(genCode);
        }
    }
}