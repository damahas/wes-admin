using SqlSugar;
using System;
using System.Collections.Generic;
using Wes.Entity;
using Wes.Utils.Model;
using Wes.ViewModel.FlowManage;

namespace Wes.Service
{
    public interface IFlowInstanceNodeService
    {
        public List<FlowInstanceNodeEntity> GetList(ParamData<FlowInstanceNodeParam> param, out int total);

        public List<FlowInstanceNodeEntity> GetAll();

        public FlowInstanceNodeEntity GetById(long id);

        public FlowInstanceNodeEntity GetByTaskId(long taskId);

        public List<FlowInstanceNodeEntity> GetResetNodes(long instanceId, long nodeId);

        public bool Save(FlowInstanceNodeEntity model, ISqlSugarClient sqlSugarClient = null);

        public bool Delete(List<long> ids);

        public bool DeleteByInstanceId(List<long> ids);

        public bool DeleteByNodeId(List<long> ids);
    }
}
