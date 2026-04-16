using System;
using System.Collections.Generic;
using Wes.DbModel;
using Wes.Utils.Model;
using Wes.ViewModel.SystemManage;

namespace Wes.Business
{
    public interface ISysCodeRuleBiz
    {
        public RowData<SysCodeRuleModel> GetList(ParamData<SysCodeRuleModel> param);

        public ResultData<List<SysCodeRuleModel>> GetAll();

        public ResultData<CodeRuleInfo> GetById(long id);

        public ReturnData Save(CodeRuleInfo model);

        public ReturnData Delete(string ids);

        public bool GetCode(long id, out string errMsg, out List<string> codes, int count);

        public string GetCode(string ruleCode, out string errMsg);

        public string GetCode(long id, out string errMsg);
    }
}
