using System;
using System.Collections.Generic;
using System.Text;
using Wes.DbModel;
using Wes.Utils.Model;
using Wes.ViewModel.SystemManage;

namespace Wes.Service
{
    public interface ISysDicDataService
    {
        public List<SysDicDataModel> GetList(ParamData<DicDataParam> param, out int total);

        public List<SysDicDataModel> GetAll();

        public SysDicDataModel GetById(long id);

        public List<SysDicDataModel> GetByIds(List<long> ids);

        public bool Save(SysDicDataModel dic);

        public bool Delete(List<long> ids);

        public List<SysDicDataModel> GetListByDicType(string dicType);

        public List<SysDicDataModel> GetAllListByDicType(string dicType);

        public SysDicDataModel GetByDictType(string dictType, string dictValue);
    }
}

