using SqlSugar;
using System;
using System.Collections.Generic;
using Wes.Entity;
using Wes.Utils.Model;
using Wes.ViewModel.FlowManage;

namespace Wes.Service
{
    public interface IFlowInstanceService
    {
        public List<FlowInstanceEntity> GetList(ParamData<FlowInstanceParam> param, out int total);

        public List<FlowInstanceEntity> GetAll();

        public FlowInstanceEntity GetById(long id);

        public FlowInstanceEntity GeDetailById(long id);

        public bool Save(FlowInstanceEntity model, ISqlSugarClient sqlSugarClient = null);

        public bool Delete(List<long> ids);

        #region 流程

        public FlowInstanceEntity GetByBusinessId(string processCode, long businessId);

        public bool FinishFlowInstance(long id);

        #endregion
    }
}
