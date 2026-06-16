using SqlSugar;
using System;
using System.Collections.Generic;
using Wes.DbModel;
using Wes.Utils.Model;

namespace Wes.Service
{
    public interface IFlowProcessVersionService
    {
        public List<FlowProcessVersionModel> GetList(ParamData<FlowProcessVersionModel> param, out int total);

        public List<FlowProcessVersionModel> GetListByProcessId(long proccessId);

        public FlowProcessVersionModel GetLastVersion(long processId);

        public List<FlowProcessVersionModel> GetAll();

        public FlowProcessVersionModel GetById(long id);

        public FlowProcessVersionModel GetByProcessCode(string processCode);

        public bool Save(FlowProcessVersionModel model, ISqlSugarClient sugarClient = null);

        public bool Delete(List<long> ids);

    }
}
