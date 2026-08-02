using System;
using SqlSugar;
using System.Collections.Generic;
using Wes.Entity;
using Wes.Utils;
using Wes.Utils.Model;
using Wes.ViewModel.FlowManage;
using Org.BouncyCastle.Crypto;

namespace Wes.Service
{
    public class FlowInstanceNodeService : Repository<FlowInstanceNodeEntity>, IFlowInstanceNodeService
    {
        public FlowInstanceNodeService(ISqlSugarClient db) : base(db) { }

        public List<FlowInstanceNodeEntity> GetList(ParamData<FlowInstanceNodeParam> param, out int total)
        {
            Expressionable<FlowInstanceNodeEntity> express = Expressionable.Create<FlowInstanceNodeEntity>();
            express.And(p => p.IsDel == 0);
            if (param.Params != null)
            {
                if (param.Params.InstanceId > 0)
                {
                    express.And(p => p.InstanceId == param.Params.InstanceId);
                }
                if (!string.IsNullOrWhiteSpace(param.Params.NodeId))
                {
                    express.And(p => p.NodeId == param.Params.NodeId.Trim());
                }
                if (!string.IsNullOrWhiteSpace(param.Params.NodeName))
                {
                    express.And(p => p.NodeName == param.Params.NodeName.Trim());
                }
                if (!string.IsNullOrWhiteSpace(param.Params.PreNodeId))
                {
                    express.And(p => p.PreNodeId == param.Params.PreNodeId.Trim());
                }
            }
            total = 0;
            var query = Context.Queryable<FlowInstanceNodeEntity>().Where(express.ToExpression());
            if (param.PageSize == 0)
                return query.ToList();
            return query.ToPageList(param.PageNum, param.PageSize, ref total);
        }

        public List<FlowInstanceNodeEntity> GetAll()
        {
            return GetList(p => p.IsDel == 0);
        }

        public FlowInstanceNodeEntity GetById(long id)
        {
            return GetFirst(p => p.InstanceNodeId == id && p.IsDel == 0);
        }

        public FlowInstanceNodeEntity GetByTaskId(long taskId)
        {
            return Context.Queryable<FlowInstanceNodeEntity>()
                .LeftJoin<FlowInstanceTaskEntity>((n, t) => n.InstanceNodeId == t.InstanceNodeId)
                .Where((n, t) => t.InstanceTaskId == taskId)
                .First();
        }

        public List<FlowInstanceNodeEntity> GetResetNodes(long instanceId, long nodeId)
        {
            return Context.Queryable<FlowInstanceNodeEntity>()
                .Where(p => p.InstanceId == instanceId && p.InstanceNodeId >= nodeId).ToList();
        }

        public bool Save(FlowInstanceNodeEntity model, ISqlSugarClient sqlSugarClient = null)
        {
            var context = sqlSugarClient ?? Context;
            if (model.InstanceNodeId > 0)
            {
                model.UpdateTime = DateTime.Now;
                model.UpdateBy = GlobalContext.CurrentUser.Account;
                return context.Updateable(model).ExecuteCommand() > 0;
            }
            model.IsDel = 0;
            model.CreateTime = DateTime.Now;
            model.CreateBy = GlobalContext.CurrentUser.Account;
            model.InstanceNodeId = context.Insertable(model).ExecuteReturnSnowflakeId();
            return true;
        }

        public bool Delete(List<long> ids)
        {
            if (ids == null || ids.Count == 0) return false;
            return Context.Updateable<FlowInstanceNodeEntity>().SetColumns(p => p.IsDel == 1)
                .Where(p => ids.Contains(p.InstanceNodeId)).ExecuteCommand() > 0;
        }

        public bool DeleteByInstanceId(List<long> ids)
        {
            if (ids == null || ids.Count == 0) return false;
            return Context.Updateable<FlowInstanceNodeEntity>().SetColumns(p => p.IsDel == 1)
                .Where(p => ids.Contains(p.InstanceId)).ExecuteCommand() > 0;
        }

        public bool DeleteByNodeId(List<long> ids)
        {
            if (ids == null || ids.Count == 0) return false;
            Context.Updateable<FlowInstanceNodeEntity>().SetColumns(p => p.IsDel == 1)
                .Where(p => ids.Contains(p.InstanceNodeId)).ExecuteCommand();
            Context.Updateable<FlowInstanceTaskEntity>().SetColumns(p => p.IsDel == 1)
                .Where(p => ids.Contains(p.InstanceNodeId)).ExecuteCommand();
            return true;
        }
    }
}
