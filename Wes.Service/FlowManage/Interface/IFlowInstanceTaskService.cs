using SqlSugar;
using System;
using System.Collections.Generic;
using Wes.DbModel;
using Wes.Utils.Model;

namespace Wes.Service
{
    public interface IFlowInstanceTaskService
    {
        public List<FlowInstanceTaskModel> GetList(ParamData<FlowInstanceTaskModel> param, out int total);

        public List<FlowInstanceTaskModel> GetAll();

        public FlowInstanceTaskModel GetById(long id);

        public bool Save(FlowInstanceTaskModel model, ISqlSugarClient sqlSugarClient = null);

        public bool AutoHandleNodeTask(long nodeId, long taskId = 0, ISqlSugarClient sqlSugarClient = null);

        public bool Delete(List<long> ids);

        public bool DeleteByInstanceId(List<long> ids);

        public FlowInstanceTaskModel GetByUserId(long instanceId, long userId);

        public List<FlowInstanceTaskModel> GetByNodeId(long nodeId);
    }
}
