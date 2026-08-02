using System;
using System.Collections.Generic;
using Wes.Entity;
using Wes.Utils.Model;
using Wes.ViewModel.FlowManage;

namespace Wes.Service
{
    public interface IFlowProcessService
    {
        public List<FlowProcessEntity> GetList(ParamData<FlowProcessParam> param, out int total);

        public List<FlowProcessEntity> GetAll();

        public FlowProcessEntity GetById(long id);

        public FlowProcessEntity GetByCode(string processCode);

        public bool Save(FlowProcessEntity model);

        public bool Delete(List<long> ids);
    }
}
