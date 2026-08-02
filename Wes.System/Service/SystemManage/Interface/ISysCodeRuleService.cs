using SqlSugar;
using System;
using System.Collections.Generic;
using Wes.Entity;
using Wes.Utils.Model;
using Wes.ViewModel.SystemManage;

namespace Wes.Service
{
    public interface ISysCodeRuleService
    {
        #region 编码主表操作

        public List<SysCodeRuleEntity> GetList(ParamData<CodeRuleParam> param, out int total);

        public List<SysCodeRuleEntity> GetAll();

        public SysCodeRuleEntity GetById(long id);

        public SysCodeRuleEntity GetByRuleCode(string ruleCode);

        public bool Save(SysCodeRuleEntity model, ISqlSugarClient client);

        public bool Delete(List<long> ids);

        #endregion

        #region 片段操作

        public List<SysCodeRulePartEntity> GetPartListByRuleId(long ruleId);

        public bool SavePart(SysCodeRulePartEntity model, ISqlSugarClient client);

        public bool DeletePart(List<long> ids, ISqlSugarClient client);

        #endregion
    }
}
