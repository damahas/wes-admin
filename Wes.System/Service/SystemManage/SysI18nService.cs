using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using Wes.Entity;
using Wes.Utils;
using Wes.Utils.Model;
using Wes.ViewModel.SystemManage;

namespace Wes.Service
{
    public class SysI18nService : Repository<SysI18nEntity>, ISysI18nService
    {
        public SysI18nService(ISqlSugarClient db) : base(db) { }

        public List<SysI18nEntity> GetList(ParamData<I18nParam> param, out int total)
        {
            Expressionable<SysI18nEntity> express = Expressionable.Create<SysI18nEntity>();
            express.And(p => p.IsDel == 0);
            if (param.Params != null)
            {
                if (!string.IsNullOrWhiteSpace(param.Params.I18nKey))
                {
                    express.And(p => p.I18nKey.Contains(param.Params.I18nKey.Trim()));
                }
                if (!string.IsNullOrWhiteSpace(param.Params.Lang))
                {
                    express.And(p => p.Lang == param.Params.Lang.Trim());
                }
                if (!string.IsNullOrWhiteSpace(param.Params.I18nValue))
                {
                    express.And(p => p.I18nValue.Contains(param.Params.I18nValue.Trim()));
                }
            }
            total = 0;
            var query = Context.Queryable<SysI18nEntity>()
                .Where(express.ToExpression())
                .OrderBy(p => p.I18nKey)
                .OrderBy(p => p.Lang);
            if (param.PageSize == 0)
                return query.ToList();
            return query.ToPageList(param.PageNum, param.PageSize, ref total);
        }

        public SysI18nEntity GetById(long id)
        {
            return GetFirst(p => p.I18nId == id && p.IsDel == 0);
        }

        public bool Save(SysI18nEntity model)
        {
            if (model.I18nId > 0)
            {
                model.UpdateTime = DateTime.Now;
                model.UpdateBy = GlobalContext.CurrentUser.Account;
                return Update(model);
            }
            model.IsDel = 0;
            model.CreateTime = DateTime.Now;
            model.CreateBy = GlobalContext.CurrentUser.Account;
            return InsertReturnSnowflakeId(model) > 0;
        }

        public bool Delete(List<long> ids)
        {
            return Context.Updateable<SysI18nEntity>()
                .SetColumns(p => p.IsDel == 1)
                .Where(p => ids.Contains(p.I18nId))
                .ExecuteCommand() > 0;
        }

        public string GetTranslation(string key, string lang)
        {
            var item = GetFirst(p => p.I18nKey == key && p.Lang == lang && p.IsDel == 0);
            return item?.I18nValue;
        }

        public Dictionary<string, string> GetTranslationsByPrefix(string prefix, string lang)
        {
            var list = GetList(p => p.I18nKey.StartsWith(prefix) && p.Lang == lang && p.IsDel == 0);
            return list.ToDictionary(p => p.I18nKey, p => p.I18nValue);
        }

        public Dictionary<string, string> GetAllFrontendTranslations(string lang)
        {
            var list = GetList(p => p.I18nKey.StartsWith("frontend.") && p.Lang == lang && p.IsDel == 0);
            return list.ToDictionary(p => p.I18nKey.Substring(9), p => p.I18nValue);
        }

        public SysI18nEntity GetByKeyAndLang(string i18nKey, string lang)
        {
            return GetFirst(p => p.I18nKey == i18nKey && p.Lang == lang && p.IsDel == 0);
        }
    }
}
