using System;
using System.Collections.Generic;
using Wes.DbModel;
using Wes.Utils.Model;
using Wes.ViewModel.SystemManage;

namespace Wes.Service
{
    public interface ISysFileService
    {
        public List<SysFileModel> GetList(ParamData<FileParam> param, out int total);

        public List<SysFileModel> GetListByTableId(string tableName, long tableId);

        public List<SysFileModel> GetAll();

        public SysFileModel GetById(long id);

        public bool Save(SysFileModel model);

        public bool Delete(List<long> ids);
    }
}
