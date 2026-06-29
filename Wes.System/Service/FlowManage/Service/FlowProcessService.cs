using System;
using SqlSugar;
using System.Collections.Generic;
using Wes.DbModel;
using Wes.Utils;
using Wes.Utils.Model;
using Wes.ViewModel.FlowManage;

namespace Wes.Service
{
    public class FlowProcessService : Repository<FlowProcessModel>, IFlowProcessService
    {
        public FlowProcessService(ISqlSugarClient db) : base(db) { }

        public List<FlowProcessModel> GetList(ParamData<FlowProcessParam> param, out int total)
        {
            Expressionable<FlowProcessModel> express = Expressionable.Create<FlowProcessModel>();
            express.And(p => p.IsDel == 0);
            if (param.Params != null)
            {
                if (!string.IsNullOrWhiteSpace(param.Params.ProcessCode))
                {
                    express.And(p => p.ProcessCode.Contains(param.Params.ProcessCode.Trim()));
                }
                if (!string.IsNullOrWhiteSpace(param.Params.ProcessName))
                {
                    express.And(p => p.ProcessName.Contains(param.Params.ProcessName.Trim()));
                }
                if (param.Params.ParentId > 0)
                {
                    express.And(p => p.ParentId == param.Params.ParentId);
                }
                if (param.Params.CurVersionId > 0)
                {
                    express.And(p => p.CurVersionId == param.Params.CurVersionId);
                }
                if (!string.IsNullOrWhiteSpace(param.Params.BusinessField))
                {
                    express.And(p => p.BusinessField == param.Params.BusinessField.Trim());
                }
                if (!string.IsNullOrWhiteSpace(param.Params.FormUrl))
                {
                    express.And(p => p.FormUrl == param.Params.FormUrl.Trim());
                }
                if (!string.IsNullOrWhiteSpace(param.Params.BackUrl))
                {
                    express.And(p => p.BackUrl == param.Params.BackUrl.Trim());
                }
            }
            total = 0;
            var query = Context.Queryable<FlowProcessModel>().Where(express.ToExpression()).Includes(p => p.Version)
                .OrderBy(p => p.ProcessId, OrderByType.Desc);
            if (param.PageSize == 0)
                return query.ToList();
            return query.ToPageList(param.PageNum, param.PageSize, ref total);
        }

        public List<FlowProcessModel> GetAll()
        {
            return GetList(p => p.IsDel == 0);
        }

        public FlowProcessModel GetById(long id)
        {
            return GetFirst(p => p.ProcessId == id && p.IsDel == 0);
        }

        public FlowProcessModel GetByCode(string processCode)
        {
            return GetFirst(p=>p.ProcessCode == processCode && p.IsDel == 0);
        }

        public bool Save(FlowProcessModel model)
        {
            if (model.ProcessId > 0)
            {
                model.UpdateTime = DateTime.Now;
                model.UpdateBy = GlobalContext.CurrentUser.Account;
                return Update(model);
            }
            model.IsDel = 0;
            model.CreateTime = DateTime.Now;
            model.CreateBy = GlobalContext.CurrentUser.Account;
            model.ProcessId = Context.Insertable(model).ExecuteReturnSnowflakeId();
            return true;
        }

        public bool Delete(List<long> ids)
        {
            if (ids == null || ids.Count == 0) return false;
            return Context.Updateable<FlowProcessModel>().SetColumns(p => p.IsDel == 1)
                .Where(p => ids.Contains(p.ProcessId)).ExecuteCommand() > 0;
        }

    }
}
