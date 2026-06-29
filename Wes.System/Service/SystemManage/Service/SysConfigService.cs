using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;
using Wes.DbModel;
using Wes.Utils;
using Wes.Utils.Model;
using Wes.ViewModel.SystemManage;

namespace Wes.Service
{
    public class SysConfigService : Repository<SysConfigModel>, ISysConfigService
    {
        public SysConfigService(ISqlSugarClient db) : base(db) { }

        #region 参数操作

        public bool Delete(List<long> ids)
        {
            return Context.Updateable<SysConfigModel>().SetColumns(p => p.IsDel == 1)
                .Where(p => ids.Contains(p.ConfigId)).ExecuteCommand() > 0;
        }

        public List<SysConfigModel> GetAll()
        {
            return GetList(p => p.IsDel == 0);
        }

        public SysConfigModel GetByConfigKey(string configKey)
        {
            return GetFirst(p => p.ConfigKey == configKey && p.IsDel == 0);
        }

        public SysConfigModel GetById(long configId)
        {
            return GetFirst(p => p.ConfigId == configId && p.IsDel == 0);
        }

        public List<SysConfigModel> GetByIds(List<long> ids)
        {
            if (ids == null || ids.Count == 0) return new List<SysConfigModel>();
            return GetList(p => p.IsDel == 0 && ids.Contains(p.ConfigId));
        }

        public List<SysConfigModel> GetList(ParamData<ConfigParam> param, out int total)
        {
            Expressionable<SysConfigModel> express = Expressionable.Create<SysConfigModel>();
            express.And(p => p.IsDel == 0);
            if (param.Params != null)
            {
                if (!string.IsNullOrWhiteSpace(param.Params.ConfigKey))
                {
                    express.And(p => p.ConfigKey.Contains(param.Params.ConfigKey.Trim()));
                }
                if (!string.IsNullOrWhiteSpace(param.Params.ConfigName))
                {
                    express.And(p => p.ConfigName.Contains(param.Params.ConfigName.Trim()));
                }
                if (!string.IsNullOrWhiteSpace(param.Params.ConfigType))
                {
                    express.And(p => p.ConfigType == param.Params.ConfigType);
                }
                if (param.Params.BeginTime != null)
                {
                    express.And(p => p.CreateTime >= param.Params.BeginTime);
                }
                if (param.Params.EndTime != null)
                {
                    express.And(p => p.CreateTime <= param.Params.EndTime);
                }
            }
            total = 0;
            var query = Context.Queryable<SysConfigModel>().Where(express.ToExpression());
            if (param.PageSize == 0)
                return query.ToList();
            return query.ToPageList(param.PageNum, param.PageSize, ref total);
        }

        public bool Save(SysConfigModel config)
        {
            if (config.ConfigId > 0)
            {
                config.UpdateTime = DateTime.Now;
                config.UpdateBy = GlobalContext.CurrentUser.Account;
                return Update(config);
            }
            config.IsDel = 0;
            config.CreateTime = DateTime.Now;
            config.CreateBy = GlobalContext.CurrentUser.Account;
            return InsertReturnSnowflakeId(config) > 0;
        }

        #endregion
    }
}
