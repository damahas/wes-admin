using System;
using System.Collections.Generic;
using Wes.Entity;
using Wes.Utils.Model;
using Wes.ViewModel.SystemManage;

namespace Wes.Service
{
    public interface ISysFileService
    {
        public List<SysFileEntity> GetList(ParamData<FileParam> param, out int total);

        public List<SysFileEntity> GetListByTableId(string tableName, long tableId);

        public List<SysFileEntity> GetAll();

        public SysFileEntity GetById(long id);

        public bool Save(SysFileEntity model);

        public bool Delete(List<long> ids);
    }
}
