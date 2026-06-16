using System;
using SqlSugar;
using System.Collections.Generic;
using Wes.DbModel;
using Wes.Utils;
using Wes.Utils.Model;

namespace Wes.Service
{
    public class FlowProcessVersionService : Repository<FlowProcessVersionModel>, IFlowProcessVersionService
    {
        public FlowProcessVersionService(ISqlSugarClient db) : base(db) { }

        public List<FlowProcessVersionModel> GetList(ParamData<FlowProcessVersionModel> param, out int total)
        {
            Expressionable<FlowProcessVersionModel> express = Expressionable.Create<FlowProcessVersionModel>();
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
            var query = Context.Queryable<FlowProcessVersionModel>().Where(express.ToExpression())
                .Includes(p => p.Process)
                .OrderBy(p => p.VersionId, OrderByType.Desc);
            if (param.PageSize == 0)
                return query.ToList();
            return query.ToPageList(param.PageNum, param.PageSize, ref total);
        }

        public List<FlowProcessVersionModel> GetListByProcessId(long proccessId)
        {
            return Context.Queryable<FlowProcessVersionModel>()
                    .Where(p => p.ProcessId == proccessId && p.IsDel == 0)
                    .OrderBy(p => p.CreateTime, OrderByType.Desc).ToList();
        }

        public FlowProcessVersionModel GetLastVersion(long processId)
        {
            return Context.Queryable<FlowProcessVersionModel>()
                .Where(p => p.ProcessId == processId && p.IsDel == 0)
                .OrderBy(p => p.CreateTime, OrderByType.Desc).First();
        }

        public List<FlowProcessVersionModel> GetAll()
        {
            return GetList(p => p.IsDel == 0);
        }

        public FlowProcessVersionModel GetById(long id)
        {
            return Context.Queryable<FlowProcessVersionModel>()
                .Where(p => p.VersionId == id && p.IsDel == 0)
                .Includes(p => p.Process).First();
        }

        public FlowProcessVersionModel GetByProcessCode(string processCode)
        {
            if (string.IsNullOrWhiteSpace(processCode))
            {
                return null;
            }
            return Context.Queryable<FlowProcessVersionModel>()
                .LeftJoin<FlowProcessModel>((v, p) => v.ProcessId == p.ProcessId && p.CurVersionId == v.VersionId)
                .Where((v, p) => p.ProcessCode == processCode && p.IsDel == 0 && v.IsDel == 0).First();
        }

        public bool Save(FlowProcessVersionModel model, ISqlSugarClient sqlSugarClient = null)
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
            return Context.Updateable<FlowProcessVersionModel>().SetColumns(p => p.IsDel == 1)
                .Where(p => ids.Contains(p.VersionId) && p.IsLock == 0).ExecuteCommand() > 0;
        }

    }
}
