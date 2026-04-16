using SqlSugar;
using System;
using System.Collections.Generic;
using Wes.DbModel;
using Wes.Utils.Model;

namespace Wes.Service
{
    public interface ISysCodeRuleService
    {
        #region 编码主表操作

        public List<SysCodeRuleModel> GetList(ParamData<SysCodeRuleModel> param, out int total);

        public List<SysCodeRuleModel> GetAll();

        public SysCodeRuleModel GetById(long id);

        public SysCodeRuleModel GetByRuleCode(string ruleCode);

        public bool Save(SysCodeRuleModel model, ISqlSugarClient client);

        public bool Delete(List<long> ids);

        #endregion

        #region 片段操作

        public List<SysCodeRulePartModel> GetPartListByRuleId(long ruleId);

        public bool SavePart(SysCodeRulePartModel model, ISqlSugarClient client);

        public bool DeletePart(List<long> ids, ISqlSugarClient client);

        #endregion
    }
}
