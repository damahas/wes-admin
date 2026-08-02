using System;
using SqlSugar;
using System.Collections.Generic;
using Wes.Entity;
using Wes.Utils;
using Wes.Utils.Model;
using Wes.ViewModel.FlowManage;

namespace Wes.Service
{
    public class FlowProcessVersionService : Repository<FlowProcessVersionEntity>, IFlowProcessVersionService
    {
        public FlowProcessVersionService(ISqlSugarClient db) : base(db) { }

        public List<FlowProcessVersionEntity> GetList(ParamData<FlowProcessVersionParam> param, out int total)
        {
            Expressionable<FlowProcessVersionEntity> express = Expressionable.Create<FlowProcessVersionEntity>();
            express.And(p => p.IsDel == 0);
            if (param.Params != null)
            {
                if (param.Params.ProcessId > 0)
                {
                    express.And(p => p.ProcessId == param.Params.ProcessId);
                }
                if (!string.IsNullOrWhiteSpace(param.Params.Version))
                {
                    express.And(p => p.Version.Contains(param.Params.Version.Trim()));
                }
            }
            total = 0;
            var query = Context.Queryable<FlowProcessVersionEntity>().Where(express.ToExpression())
                .Includes(p => p.Process)
                .OrderBy(p => p.VersionId, OrderByType.Desc);
            if (param.PageSize == 0)
                return query.ToList();
            return query.ToPageList(param.PageNum, param.PageSize, ref total);
        }

        public List<FlowProcessVersionEntity> GetListByProcessId(long proccessId)
        {
            return Context.Queryable<FlowProcessVersionEntity>()
                    .Where(p => p.ProcessId == proccessId && p.IsDel == 0)
                    .OrderBy(p => p.CreateTime, OrderByType.Desc).ToList();
        }

        public FlowProcessVersionEntity GetLastVersion(long processId)
        {
            return Context.Queryable<FlowProcessVersionEntity>()
                .Where(p => p.ProcessId == processId && p.IsDel == 0)
                .OrderBy(p => p.CreateTime, OrderByType.Desc).First();
        }

        public List<FlowProcessVersionEntity> GetAll()
        {
            return GetList(p => p.IsDel == 0);
        }

        public FlowProcessVersionEntity GetById(long id)
        {
            return Context.Queryable<FlowProcessVersionEntity>()
                .Where(p => p.VersionId == id && p.IsDel == 0)
                .Includes(p => p.Process).First();
        }

        public FlowProcessVersionEntity GetByProcessCode(string processCode)
        {
            if (string.IsNullOrWhiteSpace(processCode))
            {
                return null;
            }
            return Context.Queryable<FlowProcessVersionEntity>()
                .LeftJoin<FlowProcessEntity>((v, p) => v.ProcessId == p.ProcessId && p.CurVersionId == v.VersionId)
                .Where((v, p) => p.ProcessCode == processCode && p.IsDel == 0 && v.IsDel == 0).First();
        }

        public bool Save(FlowProcessVersionEntity model, ISqlSugarClient sqlSugarClient = null)
        {
            var context = sqlSugarClient ?? Context;
            if (model.VersionId > 0)
            {
                model.UpdateTime = DateTime.Now;
                model.UpdateBy = GlobalContext.CurrentUser.Account;
                return context.Updateable(model).ExecuteCommand() > 0;
            }
            model.IsDel = 0;
            model.CreateTime = DateTime.Now;
            model.CreateBy = GlobalContext.CurrentUser.Account;
            model.VersionId = Context.Insertable(model).ExecuteReturnSnowflakeId();
            return true;
        }

        public bool Delete(List<long> ids)
        {
            if (ids == null || ids.Count == 0) return false;
            return Context.Updateable<FlowProcessVersionEntity>().SetColumns(p => p.IsDel == 1)
                .Where(p => ids.Contains(p.VersionId) && p.IsLock == 0).ExecuteCommand() > 0;
        }

    }
}
