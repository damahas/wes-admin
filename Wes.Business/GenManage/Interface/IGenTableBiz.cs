using System;
using System.Collections.Generic;
using System.Text;
using Wes.DbModel;
using Wes.Utils.Model;
using Wes.ViewModel.GenManage;
using Wes.ViewModel.SystemManage;

namespace Wes.Business
{
    public interface IGenTableBiz
    {
        #region 表基础操作

        public RowData<GenTableModel> GetList(ParamData<DbTableParam> param);

        public ResultData<GenTableInfo> GetById(long id);

        public ReturnData Delete(string ids);

        public ReturnData Save(GenTableModel table);

        #endregion

        public RowData<GenTableModel> GetDbTableList(ParamData<DbTableParam> param);

        public ReturnData ImportTable(string tables);

        public ReturnData RefreshDbTable(string tableName);

        public ResultData<Dictionary<string, string>> Preview(long id);

        public ReturnData GenDbTable(string tableName);
    }
}
