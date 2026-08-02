using System;
using System.Collections.Generic;
using System.Text;
using Wes.Entity;
using Wes.Utils.Model;
using Wes.ViewModel.SystemManage;

namespace Wes.Service
{
    public interface ISysDicDataService
    {
        public List<SysDicDataEntity> GetList(ParamData<DicDataParam> param, out int total);

        public List<SysDicDataEntity> GetAll();

        public SysDicDataEntity GetById(long id);

        public List<SysDicDataEntity> GetByIds(List<long> ids);

        public bool Save(SysDicDataEntity dic);

        public bool Delete(List<long> ids);

        public List<SysDicDataEntity> GetListByDicType(string dicType);

        public List<SysDicDataEntity> GetAllListByDicType(string dicType);

        public SysDicDataEntity GetByDictType(string dictType, string dictValue);
    }
}

