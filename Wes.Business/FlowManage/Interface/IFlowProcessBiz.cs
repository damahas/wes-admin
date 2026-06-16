using System;
using System.Collections.Generic;
using Wes.DbModel;
using Wes.Utils.Model;

namespace Wes.Business
{
    public interface IFlowProcessBiz
    {
        public RowData<FlowProcessModel> GetList(ParamData<FlowProcessModel> param);

        public ResultData<List<FlowProcessModel>> GetAll();

        public ResultData<FlowProcessModel> GetById(long id);

        public ReturnData Save(FlowProcessModel model);

        public ReturnData Delete(string ids);

        #region 流程版本

        public RowData<FlowProcessVersionModel> GetVersionList(ParamData<FlowProcessVersionModel> param);

        public ResultData<List<FlowProcessVersionModel>> GetVersionAll();

        public ResultData<FlowProcessVersionModel> GetVersionById(long id);

        public ReturnData SaveVersion(FlowProcessVersionModel model);

        public ReturnData DeleteVersion(long versionId);

        public ReturnData CopyVersion(long versionId);

        public ReturnData UseVersion(long versionId);

        #endregion
    }
}
