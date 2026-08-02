using System;
using SqlSugar;
using System.Collections.Generic;
using Wes.Entity;
using Wes.Utils;
using Wes.Utils.Model;
using Org.BouncyCastle.Crypto;
using Wes.ViewModel.FlowManage;

namespace Wes.Service
{
    public class FlowInstanceService : Repository<FlowInstanceEntity>, IFlowInstanceService
    {
        public FlowInstanceService(ISqlSugarClient db) : base(db) { }

        public List<FlowInstanceEntity> GetList(ParamData<FlowInstanceParam> param, out int total)
        {
            Expressionable<FlowInstanceEntity, FlowProcessEntity, FlowProcessVersionEntity> express =
                Expressionable.Create<FlowInstanceEntity, FlowProcessEntity, FlowProcessVersionEntity>();
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
            var query = Context.Queryable<FlowInstanceEntity, FlowProcessEntity, FlowProcessVersionEntity>(
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

        public List<FlowInstanceEntity> GetAll()
        {
            return GetList(p => p.IsDel == 0);
        }

        public FlowInstanceEntity GetById(long id)
        {
            return GetFirst(p => p.InstanceId == id && p.IsDel == 0);
        }

        public FlowInstanceEntity GeDetailById(long id)
        {
            return Context.Queryable<FlowInstanceEntity>().Where(p => p.IsDel == 0 && p.InstanceId == id)
                .Includes(p => p.Process).Includes(p => p.Version).Includes(p => p.CreateUser)
                .Includes(p => p.Nodes, n => n.Tasks, t => t.ActualUser).First();
        }

        public bool Save(FlowInstanceEntity model, ISqlSugarClient sqlSugarClient = null)
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
            return Context.Updateable<FlowInstanceEntity>().SetColumns(p => p.IsDel == 1)
                .Where(p => ids.Contains(p.InstanceId)).ExecuteCommand() > 0;
        }

        #region 流程

        public FlowInstanceEntity GetByBusinessId(string processCode, long businessId)
        {
            return this.Context.Queryable<FlowInstanceEntity>()
                .Includes(i => i.Process, i => i.Version)
                .InnerJoin<FlowProcessEntity>((i, p) => i.ProcessId == p.ProcessId)
                .Where((i, p) => i.IsDel == 0 && i.BusinessId == businessId && p.ProcessCode == processCode).First();
        }

        public bool FinishFlowInstance(long id)
        {
            Context.Updateable<FlowInstanceNodeEntity>().SetColumns(p => p.NodeResult == FlowStatusEnum.auto)
                .Where(p => p.InstanceId == id).ExecuteCommand();
            Context.Updateable<FlowInstanceTaskEntity>()
                .SetColumns(p => p.TaskResult == FlowStatusEnum.auto && p.Comments == "流程结束，节点自动处理")
                .Where(p => p.InstanceId == id).ExecuteCommand();
            return true;
        }

        #endregion
    }
}
