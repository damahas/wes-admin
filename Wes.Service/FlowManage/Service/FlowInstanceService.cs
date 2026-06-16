using System;
using SqlSugar;
using System.Collections.Generic;
using Wes.DbModel;
using Wes.Utils;
using Wes.Utils.Model;
using Org.BouncyCastle.Crypto;
using Wes.ViewModel.FlowManage;

namespace Wes.Service
{
    public class FlowInstanceService : Repository<FlowInstanceModel>, IFlowInstanceService
    {
        public FlowInstanceService(ISqlSugarClient db) : base(db) { }

        public List<FlowInstanceModel> GetList(ParamData<FlowInstanceParam> param, out int total)
        {
            Expressionable<FlowInstanceModel, FlowProcessModel, FlowProcessVersionModel> express =
                Expressionable.Create<FlowInstanceModel, FlowProcessModel, FlowProcessVersionModel>();
            express.And((i, p, v) => i.IsDel == 0);
            if (param.Params != null)
            {
                if (!string.IsNullOrWhiteSpace(param.Params.BusinessCode))
                {
                    express.And((i, p, v) => i.BusinessCode.Contains(param.Params.BusinessCode.Trim()));
                }
                if (!string.IsNullOrWhiteSpace(param.Params.ProcessCode))
                {
                    express.And((i, p, v) => p.ProcessCode.Contains(param.Params.ProcessCode.Trim()));
                }
                if (!string.IsNullOrWhiteSpace(param.Params.ProcessName))
                {
                    express.And((i, p, v) => p.ProcessName.Contains(param.Params.ProcessName.Trim()));
                }
                if (param.Params.IsUrgent != null || param.Params.IsUrgent > 0)
                {
                    express.And((i, p, v) => i.IsUrgent == param.Params.IsUrgent);
                }
                if (param.Params.InstanceStatus != null)
                {
                    express.And((i, p, v) => i.InstanceStatus == param.Params.InstanceStatus);
                }
            }
            total = 0;
            var query = Context.Queryable<FlowInstanceModel, FlowProcessModel, FlowProcessVersionModel>(
                (i, p, v) => new JoinQueryInfos(
                    JoinType.Left, i.ProcessId == p.ProcessId,
                    JoinType.Left, i.VersionId == v.VersionId
                    )
                ).Where(express.ToExpression())
                .OrderBy((i, p, v) => i.CreateTime, OrderByType.Desc)
                .Includes(p => p.Process).Includes(p => p.Version).Includes(p => p.CurrentNode);
            if (param.PageSize == 0)
                return query.ToList();
            return query.ToPageList(param.PageNum, param.PageSize, ref total);
        }

        public List<FlowInstanceModel> GetAll()
        {
            return GetList(p => p.IsDel == 0);
        }

        public FlowInstanceModel GetById(long id)
        {
            return GetFirst(p => p.InstanceId == id && p.IsDel == 0);
        }

        public FlowInstanceModel GeDetailById(long id)
        {
            return Context.Queryable<FlowInstanceModel>().Where(p => p.IsDel == 0 && p.InstanceId == id)
                .Includes(p => p.Process).Includes(p => p.Version).Includes(p => p.CreateUser)
                .Includes(p => p.Nodes, n => n.Tasks, t => t.ActualUser).First();
        }

        public bool Save(FlowInstanceModel model, ISqlSugarClient sqlSugarClient = null)
        {
            var context = sqlSugarClient ?? Context;
            if (model.InstanceId > 0)
            {
                model.UpdateTime = DateTime.Now;
                model.UpdateBy = GlobalContext.CurrentUser.Account;
                return context.Updateable(model).ExecuteCommand() > 0;
            }
            model.IsDel = 0;
            model.CreateTime = DateTime.Now;
            model.CreateBy = GlobalContext.CurrentUser.Account;
            model.InstanceId = context.Insertable(model).ExecuteReturnSnowflakeId();
            return true;
        }

        public bool Delete(List<long> ids)
        {
            if (ids == null || ids.Count == 0) return false;
            return Context.Updateable<FlowInstanceModel>().SetColumns(p => p.IsDel == 1)
                .Where(p => ids.Contains(p.InstanceId)).ExecuteCommand() > 0;
        }

        #region 流程

        public FlowInstanceModel GetByBusinessId(string processCode, long businessId)
        {
            return this.Context.Queryable<FlowInstanceModel>()
                .Includes(i => i.Process, i => i.Version)
                .InnerJoin<FlowProcessModel>((i, p) => i.ProcessId == p.ProcessId)
                .Where((i, p) => i.IsDel == 0 && i.BusinessId == businessId && p.ProcessCode == processCode).First();
        }

        public bool FinishFlowInstance(long id)
        {
            Context.Updateable<FlowInstanceNodeModel>().SetColumns(p => p.NodeResult == FlowStatusEnum.auto)
                .Where(p => p.InstanceId == id).ExecuteCommand();
            Context.Updateable<FlowInstanceTaskModel>()
                .SetColumns(p => p.TaskResult == FlowStatusEnum.auto && p.Comments == "流程结束，节点自动处理")
                .Where(p => p.InstanceId == id).ExecuteCommand();
            return true;
        }

        #endregion
    }
}
