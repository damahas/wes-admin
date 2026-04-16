using System;
using System.Collections.Generic;
using System.Text;
using SqlSugar;
using Wes.DbModel;
using Wes.Utils.Model;
using Wes.ViewModel.SystemManage;

namespace Wes.Service
{
    public interface IGenTableService
    {
        #region 表基本操作

        public GenTableModel GetById(long tableId);

        public GenTableModel GetByTableName(string tableName);

        public List<GenTableModel> GetAll();

        public List<GenTableModel> GetList(ParamData<DbTableParam> param, out int total);

        public bool Delete(List<long> ids);

        public bool Save(GenTableModel model);

        #endregion

        #region 字段基本操作

        public List<GenTableColumnModel> GetColumnByTableId(long id);

        public bool SaveColumn(List<GenTableColumnModel> models);

        public bool DeleteColumn(List<long> ids);

        public bool DeleteColumnByTableIds(List<long> ids);

        #endregion

        public List<GenTableModel> GetDbTableList(ParamData<DbTableParam> param, out int total);

        public GenTableModel GetDbTable(string tableName);

        public List<DbColumnInfo> GetDbTableColumn(string tableName);
    }
}
