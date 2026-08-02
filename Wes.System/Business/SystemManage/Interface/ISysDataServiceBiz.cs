using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;
using Wes.Entity;
using Wes.Utils.Model;
using Wes.ViewModel.SystemManage;

namespace Wes.Business
{
    public interface ISysDataServiceBiz
    {
        public RowData<SysDataServiceEntity> GetList(ParamData<DataServiceParam> param);

        public ResultData<SysDataServiceEntity> GetById(long id);

        public ResultData<SysDataServiceEntity> GetByCode(string serviceCode);

        public ReturnData Save(SysDataServiceEntity model);

        public ReturnData Delete(string ids);

        public ResultData<Dictionary<string, object>> Exec(string serviceCode, Dictionary<string, object> param);

        #region ���ݿ����Ϣ
        public ResultData<List<DbTableInfo>> GetTables();

        public ResultData<List<DbColumnInfo>> GetTableColumns(string tableName);
        #endregion
    }
}
