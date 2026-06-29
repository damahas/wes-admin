using System;
using SqlSugar;
using System.Collections.Generic;
using Wes.DbModel;
using Wes.Utils;
using Wes.Utils.Model;
using Wes.ViewModel.FlowManage;
using Org.BouncyCastle.Crypto;

namespace Wes.Service
{
    public class FlowInstanceTaskService : Repository<FlowInstanceTaskModel>, IFlowInstanceTaskService
    {
        public FlowInstanceTaskService(ISqlSugarClient db) : base(db) { }

        public List<FlowInstanceTaskModel> GetList(ParamData<FlowInstanceTaskParam> param, out int total)
        {
            Expressionable<FlowInstanceTaskModel> express = Expressionable.Create<FlowInstanceTaskModel>();
            express.And(p => p.IsDel == 0);
            if (param.Params != null)
            {
                if (param.Params.InstanceId > 0)
                {
                    express.And(p => p.InstanceId == param.Params.InstanceId);
                }
                if (param.Params.TaskUserId > 0)
                {
                    express.And(p => p.TaskUserId == param.Params.TaskUserId);
                }
                if (param.Params.ActualUserId > 0)
                {
                    express.And(p => p.ActualUserId == param.Params.ActualUserId);
                }
                if (!string.IsNullOrWhiteSpace(param.Params.Comments))
                {
                    express.And(p => p.Comments == param.Params.Comments.Trim());
                }
                if (param.Params.HandleTime != null)
                {
                    express.And(p => p.HandleTime == param.Params.HandleTime);
                }
                if (param.Params.IsRecall > 0)
                {
                    express.And(p => p.IsRecall == param.Params.IsRecall);
                }
            }
            total = 0;
            var query = Context.Queryable<FlowInstanceTaskModel>().Where(express.ToExpression());
            if (param.PageSize == 0)
                return query.ToList();
            return query.ToPageList(param.PageNum, param.PageSize, ref total);
        }

        public List<FlowInstanceTaskModel> GetAll()
        {
            return GetList();
        }

        public FlowInstanceTaskModel GetById(long id)
        {
            return GetFirst(p => p.InstanceTaskId == id);
        }

        public bool Save(FlowInstanceTaskModel model, ISqlSugarClient sqlSugarClient = null)
        {
            var context = sqlSugarClient ?? Context;
            if (model.InstanceTaskId > 0)
            {
                return context.Updateable(model).ExecuteCommand() > 0;
            }
            model.CreateTime = DateTime.Now;
            model.InstanceTaskId = context.Insertable(model).ExecuteReturnSnowflakeId();
            return true;
        }

        public bool AutoHandleNodeTask(long nodeId, long taskId = 0, ISqlSugarClient sqlSugarClient = null)
        {
            var context = sqlSugarClient ?? Context;
            return context.Updateable<FlowInstanceTaskModel>()
                .SetColumns(p => p.TaskResult == FlowStatusEnum.auto)
                .SetColumns(p => p.Comments == "根据审批策略自动处理")
               .Where(p => p.InstanceNodeId == nodeId && p.TaskResult == FlowStatusEnum.start && p.InstanceTaskId != taskId)
               .ExecuteCommand() > 0;
        }

        public bool Delete(List<long> ids)
        {
            if (ids == null || ids.Count == 0) return false;
            return Context.Updateable<FlowInstanceTaskModel>().SetColumns(p => p.IsDel == 1)
                .Where(p => ids.Contains(p.InstanceTaskId)).ExecuteCommand() > 0;
        }

        public bool DeleteByInstanceId(List<long> ids)
        {
            if (ids == null || ids.Count == 0) return false;
            return Context.Updateable<FlowInstanceTaskModel>().SetColumns(p => p.IsDel == 1)
                .Where(p => ids.Contains(p.InstanceId)).ExecuteCommand() > 0;
        }

        public FlowInstanceTaskModel GetByUserId(long instanceId, long userId)
        {
            return GetFirst(p => p.InstanceId == instanceId && p.ActualUserId == userId && p.IsDel == 0 && p.TaskResult == FlowStatusEnum.start);
        }

        public List<FlowInstanceTaskModel> GetByNodeId(long nodeId)
        {
            return GetList(p => p.InstanceNodeId == nodeId && p.IsDel == 0);
        }
    }
}
