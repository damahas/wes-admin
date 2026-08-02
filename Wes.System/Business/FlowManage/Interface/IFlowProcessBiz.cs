using System;
using System.Collections.Generic;
using Wes.Entity;
using Wes.Utils.Model;
using Wes.ViewModel.FlowManage;

namespace Wes.Business
{
    public interface IFlowProcessBiz
    {
        public RowData<FlowProcessEntity> GetList(ParamData<FlowProcessParam> param);

        public ResultData<List<FlowProcessEntity>> GetAll();

        public ResultData<FlowProcessEntity> GetById(long id);

        public ReturnData Save(FlowProcessEntity model);

        public ReturnData Delete(string ids);

        #region 流程版本

        public RowData<FlowProcessVersionEntity> GetVersionList(ParamData<FlowProcessVersionParam> param);

        public ResultData<List<FlowProcessVersionEntity>> GetVersionAll();

        public ResultData<FlowProcessVersionEntity> GetVersionById(long id);

        public ReturnData SaveVersion(FlowProcessVersionEntity model);

        public ReturnData DeleteVersion(long versionId);

        public ReturnData CopyVersion(long versionId);

        public ReturnData UseVersion(long versionId);

        #endregion
    }
}
