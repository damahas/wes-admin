using SqlSugar;
using System;
using System.Collections.Generic;
using Wes.Entity;
using Wes.Utils.Model;
using Wes.ViewModel.FlowManage;

namespace Wes.Service
{
    public interface IFlowInstanceTaskService
    {
        public List<FlowInstanceTaskEntity> GetList(ParamData<FlowInstanceTaskParam> param, out int total);

        public List<FlowInstanceTaskEntity> GetAll();

        public FlowInstanceTaskEntity GetById(long id);

        public bool Save(FlowInstanceTaskEntity model, ISqlSugarClient sqlSugarClient = null);

        public bool AutoHandleNodeTask(long nodeId, long taskId = 0, ISqlSugarClient sqlSugarClient = null);

        public bool Delete(List<long> ids);

        public bool DeleteByInstanceId(List<long> ids);

        public FlowInstanceTaskEntity GetByUserId(long instanceId, long userId);

        public List<FlowInstanceTaskEntity> GetByNodeId(long nodeId);
    }
}
