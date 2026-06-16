using System;
using System.Collections.Generic;
using System.Text;
using Wes.DbModel;
using Wes.Utils.Model;
using Wes.ViewModel.SystemManage;

namespace Wes.Service
{
    public interface ISysDicTypeService
    {
        public List<SysDicTypeModel> GetList(ParamData<DicTypeParam> param, out int total);

        public List<SysDicTypeModel> GetAll();

        public SysDicTypeModel GetById(long id);

        public List<SysDicTypeModel> GetByIds(List<long> ids);

        public bool Save(SysDicTypeModel dic);

        public bool Delete(List<long> ids);

        public SysDicTypeModel GetByDictType(string dictType);
    }
}

