using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;
using Wes.Entity;
using Wes.Utils.Model;
using Wes.ViewModel.SystemManage;

namespace Wes.Service
{
    public interface ISysDataServiceService
    {
        public List<SysDataServiceEntity> GetList(ParamData<DataServiceParam> param, out int total);

        public List<SysDataServiceEntity> GetAll();

        public SysDataServiceEntity GetById(long id);

        public SysDataServiceEntity GetByCode(string serviceCode);

        public List<SysDataServiceEntity> GetByIds(List<long> ids);

        public bool Save(SysDataServiceEntity model);

        public bool Delete(List<long> ids);

        #region ���ݿ����Ϣ
        public List<DbTableInfo> GetTables();

        public List<DbColumnInfo> GetTableColumns(string tableName);
        #endregion

        #region ��ѯ����
        public dynamic GetDataFirstCell(string sql, Dictionary<string, object> param);

        public dynamic GetDataSingle(string sql, Dictionary<string, object> param);

        public List<dynamic> GetDataList(string sql, Dictionary<string, object> param);

        public int ExecCommand(string sql, Dictionary<string, object> param);
        #endregion
    }
}
