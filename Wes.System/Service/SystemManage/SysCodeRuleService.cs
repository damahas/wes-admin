using System;
using SqlSugar;
using System.Collections.Generic;
using Wes.Entity;
using Wes.Utils;
using Wes.Utils.Model;
using Wes.ViewModel.SystemManage;

namespace Wes.Service
{
    public class SysCodeRuleService : Repository<SysCodeRuleEntity>, ISysCodeRuleService
    {
        public SysCodeRuleService(ISqlSugarClient db) : base(db) { }

        #region 编码主表操作

        public List<SysCodeRuleEntity> GetList(ParamData<CodeRuleParam> param, out int total)
        {
            Expressionable<SysCodeRuleEntity> express = Expressionable.Create<SysCodeRuleEntity>();
            express.And(p => p.IsDel == 0);
            if (param.Params != null)
            {
                if (!string.IsNullOrWhiteSpace(param.Params.RuleCode))
                {
                    express.And(p => p.RuleCode.Contains(param.Params.RuleCode.Trim()));
                }
                if (!string.IsNullOrWhiteSpace(param.Params.RuleName))
                {
                    express.And(p => p.RuleName.Contains(param.Params.RuleName.Trim()));
                }
                if (!string.IsNullOrWhiteSpace(param.Params.RuleType))
                {
                    express.And(p => p.RuleType == param.Params.RuleType.Trim());
                }
                if (!string.IsNullOrWhiteSpace(param.Params.Status))
                {
                    express.And(p => p.Status == param.Params.Status.Trim());
                }
            }
            total = 0;
            var query = Context.Queryable<SysCodeRuleEntity>().OrderBy(p => p.RuleId, OrderByType.Desc).Where(express.ToExpression());
            if (param.PageSize == 0)
                return query.ToList();
            return query.ToPageList(param.PageNum, param.PageSize, ref total);
        }

        public List<SysCodeRuleEntity> GetAll()
        {
            return GetList(p => p.IsDel == 0);
        }

        public SysCodeRuleEntity GetById(long id)
        {
            return GetFirst(p => p.RuleId == id && p.IsDel == 0);
        }

        public SysCodeRuleEntity GetByRuleCode(string ruleCode)
        {
            return GetFirst(p => p.RuleCode == ruleCode && p.IsDel == 0);
        }

        public bool Save(SysCodeRuleEntity model, ISqlSugarClient client)
        {
            if (model.RuleId > 0)
            {
                model.UpdateTime = DateTime.Now;
                model.UpdateBy = GlobalContext.CurrentUser.UserName;
                return client.Updateable(model).ExecuteCommand() > 0;
            }
            model.IsDel = 0;
            model.CreateTime = DateTime.Now;
            model.CreateBy = GlobalContext.CurrentUser.UserName;
            model.RuleId = client.Insertable(model).ExecuteReturnSnowflakeId();
            return model.RuleId > 0;
        }

        public bool Delete(List<long> ids)
        {
            if (ids == null || ids.Count == 0) return false;
            return Context.Updateable<SysCodeRuleEntity>().SetColumns(p => p.IsDel == 1)
                .Where(p => ids.Contains(p.RuleId)).ExecuteCommand() > 0;
        }

        #endregion

        #region 片段操作

        public List<SysCodeRulePartEntity> GetPartListByRuleId(long ruleId)
        {
            return Context.Queryable<SysCodeRulePartEntity>().Where(p => p.IsDel == 0 && p.RuleId == ruleId).ToList();
        }

        public bool SavePart(SysCodeRulePartEntity model, ISqlSugarClient client)
        {
            if (model.PartId > 0)
            {
                model.UpdateTime = DateTime.Now;
                model.UpdateBy = GlobalContext.CurrentUser.UserName;
                return client.Updateable(model).ExecuteCommand() > 0;
            }
            model.IsDel = 0;
            model.CreateTime = DateTime.Now;
            model.CreateBy = GlobalContext.CurrentUser.UserName;
            return client.Insertable(model).ExecuteReturnSnowflakeId() > 0;
        }

        public bool DeletePart(List<long> ids, ISqlSugarClient client)
        {
            if (ids == null || ids.Count == 0) return false;
            return client.Updateable<SysCodeRulePartEntity>().SetColumns(p => p.IsDel == 1)
                .Where(p => ids.Contains(p.PartId)).ExecuteCommand() > 0;
        }

        #endregion

    }
}
