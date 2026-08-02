using System;
using System.Collections.Generic;
using System.Text;
using Wes.Entity;
using Wes.Utils.Model;
using Wes.ViewModel.SystemManage;

namespace Wes.Service
{
    public interface ISysDicTypeService
    {
        public List<SysDicTypeEntity> GetList(ParamData<DicTypeParam> param, out int total);

        public List<SysDicTypeEntity> GetAll();

        public SysDicTypeEntity GetById(long id);

        public List<SysDicTypeEntity> GetByIds(List<long> ids);

        public bool Save(SysDicTypeEntity dic);

        public bool Delete(List<long> ids);

        public SysDicTypeEntity GetByDictType(string dictType);
    }
}

