using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;
using Wes.Entity;
using Wes.Utils;
using Wes.Utils.Model;
using Wes.ViewModel.SystemManage;

namespace Wes.Service
{
    public class SysConfigService : Repository<SysConfigEntity>, ISysConfigService
    {
        public SysConfigService(ISqlSugarClient db) : base(db) { }

        #region 参数操作

        public bool Delete(List<long> ids)
        {
            return Context.Updateable<SysConfigEntity>().SetColumns(p => p.IsDel == 1)
                .Where(p => ids.Contains(p.ConfigId)).ExecuteCommand() > 0;
        }

        public List<SysConfigEntity> GetAll()
        {
            return Context.Queryable<SysConfigEntity>()
                .Where(p => p.IsDel == 0)
                .OrderBy(p => p.SortBy, OrderByType.Asc)
                .OrderBy(p => p.ConfigId, OrderByType.Asc)
                .ToList();
        }

        public SysConfigEntity GetByConfigKey(string configKey)
        {
            return GetFirst(p => p.ConfigKey == configKey && p.IsDel == 0);
        }

        public SysConfigEntity GetById(long configId)
        {
            return GetFirst(p => p.ConfigId == configId && p.IsDel == 0);
        }

        public List<SysConfigEntity> GetByIds(List<long> ids)
        {
            if (ids == null || ids.Count == 0) return new List<SysConfigEntity>();
            return GetList(p => p.IsDel == 0 && ids.Contains(p.ConfigId));
        }

        public List<SysConfigEntity> GetList(ParamData<ConfigParam> param, out int total)
        {
            Expressionable<SysConfigEntity> express = Expressionable.Create<SysConfigEntity>();
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
            var query = Context.Queryable<SysConfigEntity>().Where(express.ToExpression());
            if (param.PageSize == 0)
                return query.ToList();
            return query.ToPageList(param.PageNum, param.PageSize, ref total);
        }

        public bool Save(SysConfigEntity config)
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

        public void UpdateSort(List<long> configIds)
        {
            var updateList = configIds.Select((id, index) => new SysConfigEntity { ConfigId = id, SortBy = index }).ToList();
            Context.Updateable(updateList).UpdateColumns(p => new { p.SortBy }).ExecuteCommand();
        }

        #endregion
    }
}
