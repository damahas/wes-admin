using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;
using Wes.DbModel;
using Wes.Utils.Model;
using Wes.ViewModel.SystemManage;

namespace Wes.Business
{
    public interface ISysDataServiceBiz
    {
        public RowData<SysDataServiceModel> GetList(ParamData<DataServiceParam> param);

        public ResultData<SysDataServiceModel> GetById(long id);

        public ResultData<SysDataServiceModel> GetByCode(string serviceCode);

        public ReturnData Save(SysDataServiceModel model);

        public ReturnData Delete(string ids);

        public ResultData<Dictionary<string, object>> Exec(string serviceCode, Dictionary<string, object> param);

        #region 数据库表信息
        public ResultData<List<DbTableInfo>> GetTables();

        public ResultData<List<DbColumnInfo>> GetTableColumns(string tableName);
        #endregion
    }
}
