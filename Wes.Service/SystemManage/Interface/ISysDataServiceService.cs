using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;
using Wes.DbModel;
using Wes.Utils.Model;
using Wes.ViewModel.SystemManage;

namespace Wes.Service
{
    public interface ISysDataServiceService
    {
        public List<SysDataServiceModel> GetList(ParamData<DataServiceParam> param, out int total);

        public List<SysDataServiceModel> GetAll();

        public SysDataServiceModel GetById(long id);

        public List<SysDataServiceModel> GetByIds(List<long> ids);

        public bool Save(SysDataServiceModel model);

        public bool Delete(List<long> ids);

        #region 数据库表信息
        public List<DbTableInfo> GetTables();

        public List<DbColumnInfo> GetTableColumns(string tableName);
        #endregion
    }
}
