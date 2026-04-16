using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;
using Wes.DbModel;
using Wes.Utils;
using Wes.Utils.Model;
using Wes.ViewModel.SystemManage;
using System.Linq;
using Wes.Utils.Extension;

namespace Wes.Service
{
    public class GenTableService : Repository<GenTableModel>, IGenTableService
    {
        public GenTableService(ISqlSugarClient db) : base(db) { }

        #region 表基本操作

        public GenTableModel GetById(long tableId)
        {
            return GetFirst(p => p.TableId == tableId);
        }

        public GenTableModel GetByTableName(string tableName)
        {
            return GetFirst(p => p.TableName == tableName);
        }

        public List<GenTableModel> GetAll()
        {
            return Context.Queryable<GenTableModel>().Includes(p => p.Columns).ToList();
        }


        public List<GenTableModel> GetList(ParamData<DbTableParam> param, out int total)
        {
            Expressionable<GenTableModel> express = Expressionable.Create<GenTableModel>();
            if (param.Params != null)
            {
                if (!string.IsNullOrWhiteSpace(param.Params.TableName))
                {
                    express.And(p => p.TableName.Contains(param.Params.TableName.Trim()));
                }
                if (!string.IsNullOrWhiteSpace(param.Params.TableComment))
                {
                    express.And(p => p.TableComment == param.Params.TableComment);
                }
                if (param.Params.BeginTime != null)
                {
                    express.And(p => p.CreateTime >= param.Params.BeginTime);
                }
                if (param.Params.EndTime != null)
                {
                    express.And(p => p.CreateTime <= param.Params.EndTime);
                }
            }
            total = 0;
            var query = Context.Queryable<GenTableModel>().OrderBy(p => p.TableId, OrderByType.Desc).Where(express.ToExpression());
            if (param.PageNum == 0)
                return query.ToList();
            return query.ToPageList(param.PageNum, param.PageSize, ref total);
        }

        public bool Delete(List<long> ids)
        {
            if (ids == null || ids.Count == 0) return false;
            return Context.Deleteable<GenTableModel>().In(ids).ExecuteCommand() > 0;
        }

        public bool Save(GenTableModel model)
        {
            if (model.TableId > 0)
            {
                model.UpdateTime = DateTime.Now;
                model.UpdateBy = GlobalContext.CurrentUser.Account;
                return Update(model);
            }
            model.CreateTime = DateTime.Now;
            model.CreateBy = GlobalContext.CurrentUser.Account;
            model.TableId = InsertReturnSnowflakeId(model);
            return model.TableId > 0;
        }

        #endregion

        #region 字段基本操作

        public List<GenTableColumnModel> GetColumnByTableId(long id)
        {
            return Context.Queryable<GenTableColumnModel>().Where(p => p.TableId == id).ToList();
        }

        public bool SaveColumn(List<GenTableColumnModel> models)
        {
            List<GenTableColumnModel> saveModels = models.Where(p => p.ColumnId == 0).ToList();
            List<GenTableColumnModel> updateModels = models.Where(p => p.ColumnId > 0).ToList();
            if (saveModels.Any()) { 
                Context.Insertable(saveModels).ExecuteReturnSnowflakeIdList();
            }
            if (updateModels.Any())
            {
                Context.Updateable(updateModels).ExecuteCommand();
            }
            return true;
        }

        public bool DeleteColumn(List<long> ids)
        {
            if (ids == null || ids.Count == 0) return false;
            return Context.Deleteable<GenTableColumnModel>().In(ids).ExecuteCommand() > 0;
        }

        public bool DeleteColumnByTableIds(List<long> ids)
        {
            if (ids == null || ids.Count == 0) return false;
            return Context.Deleteable<GenTableColumnModel>().Where(p => ids.Contains(p.TableId)).ExecuteCommand() > 0;
        }

        #endregion

        public List<GenTableModel> GetDbTableList(ParamData<DbTableParam> param, out int total)
        {
            string dbName = Context.CurrentConnectionConfig.ConnectionString.ToDbName();
            List<SugarParameter> sugarParameter = new List<SugarParameter>();
            StringBuilder sql = new StringBuilder($@"SELECT
	                table_name TableName,
	                table_comment TableComment,
	                create_time CreateTime,
	                update_time UpdateTime 
                FROM
	                information_schema.TABLES 
                WHERE
	                table_schema = '{dbName}' 
	                AND table_name NOT IN ( SELECT table_name FROM {dbName}.gen_table ) 
                    AND table_name NOT IN ( 'gen_table', 'gen_table_column' )
                ");
            if (param.Params != null)
            {
                if (!string.IsNullOrWhiteSpace(param.Params.TableName))
                {
                    sql.Append($" AND table_name like @TableName ");
                    sugarParameter.Add(new SugarParameter("@TableName", $"%{param.Params.TableName.Trim()}%"));
                }
                if (!string.IsNullOrWhiteSpace(param.Params.TableComment))
                {
                    sql.Append($" AND table_comment like @TableComment ");
                    sugarParameter.Add(new SugarParameter("@TableComment", $"%{param.Params.TableComment.Trim()}%"));
                }
            }
            total = 0;
            return Context.SqlQueryable<GenTableModel>(sql.ToString()).AddParameters(sugarParameter)
                .OrderBy("t.CreateTime DESC").ToPageList(param.PageNum, param.PageSize, ref total);
        }

        public GenTableModel GetDbTable(string tableName)
        {
            string sql = $@"SELECT
	                            table_name TableName,
	                            table_comment TableComment
                            FROM
	                            information_schema.TABLES 
                            WHERE
	                            table_name = @tableName ";
            return Context.SqlQueryable<GenTableModel>(sql.ToString())
                .AddParameters(new SugarParameter[] { new SugarParameter("@tableName", tableName) }).First();
        }

        public List<DbColumnInfo> GetDbTableColumn(string tableName)
        {
            return Context.DbMaintenance.GetColumnInfosByTableName(tableName);
        }
    }
}
