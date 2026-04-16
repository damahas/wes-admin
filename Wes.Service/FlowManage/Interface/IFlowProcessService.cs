using System;
using System.Collections.Generic;
using Wes.DbModel;
using Wes.Utils.Model;

namespace Wes.Service
{
    public interface IFlowProcessService
    {
        public List<FlowProcessModel> GetList(ParamData<FlowProcessModel> param, out int total);

        public List<FlowProcessModel> GetAll();

        public FlowProcessModel GetById(long id);

        public FlowProcessModel GetByCode(string processCode);

        public bool Save(FlowProcessModel model);

        public bool Delete(List<long> ids);
    }
}
