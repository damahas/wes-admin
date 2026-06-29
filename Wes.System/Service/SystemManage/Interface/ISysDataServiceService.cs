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

        public SysDataServiceModel GetByCode(string serviceCode);

        public List<SysDataServiceModel> GetByIds(List<long> ids);

        public bool Save(SysDataServiceModel model);

        public bool Delete(List<long> ids);

        #region 数据库表信息
        public List<DbTableInfo> GetTables();

        public List<DbColumnInfo> GetTableColumns(string tableName);
        #endregion

        #region 查询数据
        public dynamic GetDataFirstCell(string sql, Dictionary<string, object> param);

        public dynamic GetDataSingle(string sql, Dictionary<string, object> param);

        public List<dynamic> GetDataList(string sql, Dictionary<string, object> param);

        public int ExecCommand(string sql, Dictionary<string, object> param);
        #endregion
    }
}
