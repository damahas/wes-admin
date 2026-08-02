using System;
using System.Collections.Generic;
using Wes.Entity;
using Wes.Utils.Model;
using Wes.ViewModel.FlowManage;

namespace Wes.Business
{
    public interface IFlowInstanceBiz
    {
        public RowData<FlowInstanceEntity> GetList(ParamData<FlowInstanceParam> param);

        public ResultData<List<FlowInstanceEntity>> GetAll();

        public ResultData<FlowInstanceEntity> GetById(long id);

        public ReturnData Save(FlowInstanceEntity model);

        public ReturnData Delete(string ids);

        #region 流程流转

        public ResultData<FlowRunModel> GetRunIni(string processCode, long businessId);

        public ResultData<FlowRunModel> Exec(FlowRunModel flowRunModel);

        public ReturnData TaskDelegate(long taskId, long userId);

        public ReturnData NodeReset(long nodeId);

        //public ResultData<FlowRunModel> Run(FlowRunModel flowRunModel);

        #endregion
    }
}
