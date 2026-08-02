using SqlSugar;
using System;
using System.Collections.Generic;
using Wes.Entity;
using Wes.Utils.Model;
using Wes.ViewModel.FlowManage;

namespace Wes.Service
{
    public interface IFlowProcessVersionService
    {
        public List<FlowProcessVersionEntity> GetList(ParamData<FlowProcessVersionParam> param, out int total);

        public List<FlowProcessVersionEntity> GetListByProcessId(long proccessId);

        public FlowProcessVersionEntity GetLastVersion(long processId);

        public List<FlowProcessVersionEntity> GetAll();

        public FlowProcessVersionEntity GetById(long id);

        public FlowProcessVersionEntity GetByProcessCode(string processCode);

        public bool Save(FlowProcessVersionEntity model, ISqlSugarClient sugarClient = null);

        public bool Delete(List<long> ids);

    }
}
