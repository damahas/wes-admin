using SqlSugar;
using System;
using System.Collections.Generic;
using Wes.DbModel;
using Wes.Utils.Model;
using Wes.ViewModel.FlowManage;

namespace Wes.Service
{
    public interface IFlowInstanceNodeService
    {
        public List<FlowInstanceNodeModel> GetList(ParamData<FlowInstanceNodeParam> param, out int total);

        public List<FlowInstanceNodeModel> GetAll();

        public FlowInstanceNodeModel GetById(long id);

        public FlowInstanceNodeModel GetByTaskId(long taskId);

        public List<FlowInstanceNodeModel> GetResetNodes(long instanceId, long nodeId);

        public bool Save(FlowInstanceNodeModel model, ISqlSugarClient sqlSugarClient = null);

        public bool Delete(List<long> ids);

        public bool DeleteByInstanceId(List<long> ids);

        public bool DeleteByNodeId(List<long> ids);
    }
}
