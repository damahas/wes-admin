using SqlSugar;
using System;
using System.Collections.Generic;
using Wes.DbModel;
using Wes.Utils.Model;
using Wes.ViewModel.FlowManage;

namespace Wes.Service
{
    public interface IFlowInstanceService
    {
        public List<FlowInstanceModel> GetList(ParamData<FlowInstanceParam> param, out int total);

        public List<FlowInstanceModel> GetAll();

        public FlowInstanceModel GetById(long id);

        public FlowInstanceModel GeDetailById(long id);

        public bool Save(FlowInstanceModel model, ISqlSugarClient sqlSugarClient = null);

        public bool Delete(List<long> ids);

        #region 流程

        public FlowInstanceModel GetByBusinessId(string processCode, long businessId);

        public bool FinishFlowInstance(long id);

        #endregion
    }
}
