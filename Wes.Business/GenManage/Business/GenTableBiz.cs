using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Wes.DbModel;
using Wes.Service;
using Wes.Utils.Extension;
using Wes.Utils.Model;
using Wes.ViewModel.SystemManage;
using Wes.Utils.Cache;
using Wes.Utils;
using Wes.Utils.CodeGenerator;
using System.IO;
using Wes.ViewModel.GenManage;

namespace Wes.Business
{
    public class GenTableBiz : IGenTableBiz
    {
        private IGenTableService _genTableService;

        public GenTableBiz(IGenTableService genTableService)
        {
            _genTableService = genTableService;
        }

        #region 表基础操作

        public ResultData<GenTableInfo> GetById(long id)
        {
            ResultData<GenTableInfo> result = new ResultData<GenTableInfo>(new GenTableInfo()
            {
                Info = _genTableService.GetById(id),
                Rows = _genTableService.GetColumnByTableId(id),
                Tables = _genTableService.GetAll()
            });
            return result;
        }

        public RowData<GenTableModel> GetList(ParamData<DbTableParam> param)
        {
            int total = 0;
            RowData<GenTableModel> result = new RowData<GenTableModel>(_genTableService.GetList(param, out total));
            result.total = total;
            return result;
        }

        public ReturnData Save(GenTableModel table)
        {
            _genTableService.Save(table);
            _genTableService.SaveColumn(table.Columns);
            return new ReturnData(200, "保存成功！");
        }

        public ReturnData Delete(string ids)
        {
            var tableIds = ids.ToLongList();
            if (tableIds == null || tableIds.Count == 0)
            {
                return new ReturnData(500, "参数有误，请检查参数！");
            }
            _genTableService.Delete(tableIds);
            _genTableService.DeleteColumnByTableIds(tableIds);
            return new ReturnData();
        }

        #endregion

        public RowData<GenTableModel> GetDbTableList(ParamData<DbTableParam> param)
        {
            int total = 0;
            RowData<GenTableModel> result = new RowData<GenTableModel>(_genTableService.GetDbTableList(param, out total));
            result.total = total;
            return result;
        }

        public ReturnData ImportTable(string tables)
        {
            if (string.IsNullOrWhiteSpace(tables))
            {
                return new ReturnData(500, "参数错误！");
            }
            var tableList = tables.Split(',').Where(p => !string.IsNullOrWhiteSpace(p));
            foreach (var table in tableList)
            {
                var curTable = _genTableService.GetDbTable(table);
                if (curTable == null) continue;
                // 保存表数据
                curTable.FunctionName = curTable.TableComment.TrimEnd('表');
                curTable.FunctionAuthor = GlobalContext.CurrentUser.Account;
                curTable.TplCategory = "crud";
                curTable.ClassName = curTable.TableName.ToWordFirstCharUpper('_');
                curTable.PackageName = "Wes";
                curTable.ModuleName = "system";
                curTable.GenType = "1";
                curTable.GenPath = "/";
                curTable.BusinessName = curTable.TableName;
                _genTableService.Save(curTable);
                SaveTableColumn(curTable.TableId, curTable.TableName);
            }
            return new ReturnData();
        }

        public ReturnData RefreshDbTable(string tableName)
        {
            if (string.IsNullOrWhiteSpace(tableName))
            {
                return new ReturnData(500, "参数错误！");
            }
            var table = _genTableService.GetByTableName(tableName);
            if (table == null)
            {
                return new ReturnData(500, "找不到表数据！");
            }
            var dbTable = _genTableService.GetDbTable(tableName);
            if (dbTable == null)
            {
                return new ReturnData(500, "找不到数据库表，表已删除！");
            }
            table.FunctionName = dbTable.TableComment.TrimEnd('表');
            table.TableComment = dbTable.TableComment;
            _genTableService.Save(table);
            SaveTableColumn(table.TableId, table.TableName);
            return new ReturnData();
        }

        public ResultData<Dictionary<string, string>> Preview(long id)
        {
            ResultData<Dictionary<string, string>> result = new ResultData<Dictionary<string, string>>()
            {
                Data = new Dictionary<string, string>()
            };
            var tableInfo = _genTableService.GetById(id);
            tableInfo.Columns = _genTableService.GetColumnByTableId(id);
            var tableAttr = tableInfo.ToEntityCopy<GenTableModel, GenTableAttr>();
            result.Data.Add("vm/c#/Model.cs.vm", CodeGenerUtils.BuildDBModel(tableAttr));
            result.Data.Add("vm/c#/IService.cs.vm", CodeGenerUtils.BuildIService(tableAttr));
            result.Data.Add("vm/c#/Service.cs.vm", CodeGenerUtils.BuildService(tableAttr));
            result.Data.Add("vm/c#/IBusiness.cs.vm", CodeGenerUtils.BuildIBusiness(tableAttr));
            result.Data.Add("vm/c#/Business.cs.vm", CodeGenerUtils.BuildBusiness(tableAttr));
            result.Data.Add("vm/c#/Controller.cs.vm", CodeGenerUtils.BuildController(tableAttr));
            result.Data.Add("vm/js/api.js.vm", CodeGenerUtils.BuildApiJs(tableAttr));
            result.Data.Add("vm/vue/index.vue.vm", CodeGenerUtils.BuildIndex(tableAttr));
            result.Data.Add("vm/vue/indexList.vue.vm", CodeGenerUtils.BuildListIndex(tableAttr));
            result.Data.Add("vm/vue/editDialog.vue.vm", CodeGenerUtils.BuildDialogIndex(tableAttr));
            return result;
        }

        public ReturnData GenDbTable(string tableName)
        {
#if DEBUG
            var tableInfo = _genTableService.GetByTableName(tableName);
            tableInfo.Columns = _genTableService.GetColumnByTableId(tableInfo.TableId);
            var tableAttr = tableInfo.ToEntityCopy<GenTableModel, GenTableAttr>();

            string baseDir = Directory.GetCurrentDirectory().Replace("\\Wes.WebApi", "");

            $"{baseDir}/Wes.DbModel/{tableInfo.ModuleName.ToFirstCharUpper()}Manage"
                .WriteToFile($"{tableInfo.ClassName}Model.cs", CodeGenerUtils.BuildDBModel(tableAttr));

            $"{baseDir}/Wes.Service/{tableInfo.ModuleName.ToFirstCharUpper()}Manage/Interface"
                .WriteToFile($"I{tableInfo.ClassName}Service.cs", CodeGenerUtils.BuildIService(tableAttr));
            $"{baseDir}/Wes.Service/{tableInfo.ModuleName.ToFirstCharUpper()}Manage/Service"
                .WriteToFile($"{tableInfo.ClassName}Service.cs", CodeGenerUtils.BuildService(tableAttr));

            $"{baseDir}/Wes.Business/{tableInfo.ModuleName.ToFirstCharUpper()}Manage/Interface"
                .WriteToFile($"I{tableInfo.ClassName}Biz.cs", CodeGenerUtils.BuildIBusiness(tableAttr));
            $"{baseDir}/Wes.Business/{tableInfo.ModuleName.ToFirstCharUpper()}Manage/Business"
                .WriteToFile($"{tableInfo.ClassName}Biz.cs", CodeGenerUtils.BuildBusiness(tableAttr));

            $"{baseDir}/Wes.WebApi/Areas/{tableInfo.ModuleName.ToFirstCharUpper()}Manage"
                .WriteToFile($"{tableInfo.ClassName}Controller.cs", CodeGenerUtils.BuildController(tableAttr));

            $"{baseDir}/Wes.UI/src/api/{tableInfo.ModuleName}"
                .WriteToFile($"{tableInfo.BusinessName}.js", CodeGenerUtils.BuildApiJs(tableAttr));
            $"{baseDir}/Wes.UI/src/views/{tableInfo.ModuleName}/{tableInfo.BusinessName}"
                .WriteToFile($"index.vue", CodeGenerUtils.BuildIndex(tableAttr));
            $"{baseDir}/Wes.UI/src/views/{tableInfo.ModuleName}/{tableInfo.BusinessName}"
                .WriteToFile($"index.bak.vue", CodeGenerUtils.BuildListIndex(tableAttr));
            $"{baseDir}/Wes.UI/src/views/{tableInfo.ModuleName}/{tableInfo.BusinessName}"
                .WriteToFile($"edit.vue", CodeGenerUtils.BuildDialogIndex(tableAttr));

            return new ReturnData(200, Directory.GetCurrentDirectory().Replace("\\Wes.WebApi", ""));
#else
            return new ReturnData(500,"debug模式下才能使用代码生成");
#endif
        }

        #region 私有方法

        /// <summary>
        /// 保存表字段对象
        /// </summary>
        /// <param name="tableId"></param>
        /// <param name="tableName"></param>
        private void SaveTableColumn(long tableId, string tableName)
        {
            List<GenTableColumnModel> saveModels = new List<GenTableColumnModel>();
            var columnDic = _genTableService.GetColumnByTableId(tableId).ToDictionary(p => p.ColumnName, p => p);
            var tableColumns = _genTableService.GetDbTableColumn(tableName);
            foreach (var item in tableColumns)
            {
                GenTableColumnModel column;
                if (columnDic.ContainsKey(item.DbColumnName))
                {
                    column = columnDic[item.DbColumnName];
                    columnDic.Remove(item.DbColumnName);
                    column.UpdateBy = GlobalContext.CurrentUser.Account;
                    column.UpdateTime = DateTime.Now;
                }
                else
                {
                    column = new GenTableColumnModel()
                    {
                        ColumnName = item.DbColumnName,
                        CField = item.DbColumnName.ToWordFirstCharUpper('_'),
                        CreateBy = GlobalContext.CurrentUser.Account,
                        ColumnType = item.DataType,
                        CreateTime = DateTime.Now
                    };
                    InitColumn(column, item.IsPrimarykey);
                }
                column.TableId = tableId;
                column.CType = MysqlDataTypeMapping.DbTypeMapping(item.DataType, item.IsNullable);
                column.ColumnComment = item.ColumnDescription;
                column.ColumnType = item.DataType;
                column.IsPk = item.IsPrimarykey ? "1" : "0";
                column.IsIncrement = item.IsIdentity ? "1" : "0";
                column.IsRequired = item.IsNullable ? "0" : "1";
                column.ColumnLength = item.Length;
                column.ColumnPrecision = item.DecimalDigits;
                saveModels.Add(column);
            }
            _genTableService.SaveColumn(saveModels);
            _genTableService.DeleteColumn(columnDic.Values.Select(p => p.ColumnId).ToList());
        }

        /// <summary>
        /// 初始化表字段对象
        /// </summary>
        /// <param name="column"></param>
        /// <param name="IsPrimarykey"></param>
        private void InitColumn(GenTableColumnModel column, bool IsPrimarykey)
        {
            column.IsInsert = "1";
            column.QueryType = "EQ";
            column.HtmlType = "input";
            // 给默认值
            column.IsEdit = "1";
            column.IsList = "0";
            column.IsQuery = "0";
            switch (column.ColumnType)
            {
                case "date":
                    column.HtmlType = "date";
                    break;
                case "time":
                    column.HtmlType = "time";
                    break;
                case "timestamp":
                case "datetime":
                    column.HtmlType = "datetime";
                    break;
                default: break;
            }

            if (IsPrimarykey || column.ColumnName.Contains("create_by") || column.ColumnName.Contains("create_time"))
            {
                column.IsEdit = "0";
                return;
            }
            if (column.ColumnName.Contains("update_by") || column.ColumnName.Contains("update_time") || column.ColumnName.Contains("is_del"))
            {
                return;
            }
            if (column.ColumnName.Contains("remark"))
            {
                column.IsList = "1";
                return;
            }
            column.IsList = "1";
            column.IsQuery = "1";
        }

        #endregion
    }
}
