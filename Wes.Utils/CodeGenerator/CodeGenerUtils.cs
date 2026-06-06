using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Wes.Utils.Model;
using Wes.Utils.Extension;

namespace Wes.Utils.CodeGenerator
{
    public class CodeGenerUtils
    {
        public static string BuildDBModel(GenTableAttr tableAttr)
        {
            StringBuilder code = new StringBuilder();
            code.Append($"using System;\r\n");
            code.Append($"using SqlSugar;\r\n");
            code.Append($"using System.Collections.Generic;\r\n");
            code.Append($"using System.Text;\r\n");
            code.Append($"using Wes.Utils.Converter;\r\n");
            code.Append($"using System.Text.Json.Serialization;\r\n");
            code.Append($"\r\n");
            code.Append($"namespace Wes.DbModel\r\n");
            code.Append($"{{\r\n");
            code.Append($"    /// <summary>\r\n");
            code.Append($"    /// {tableAttr.FunctionName}\r\n");
            code.Append($"    /// <summary>\r\n");
            code.Append($"    [SugarTable(\"{tableAttr.TableName}\", \"{tableAttr.FunctionName}\", IsDisabledDelete = true)]\r\n");
            code.Append($"    public class {tableAttr.ClassName}Model\r\n");
            code.Append($"    {{\r\n");
            foreach (var item in tableAttr.Columns)
            {
                code.Append($"        /// <summary>\r\n");
                code.Append($"        /// {item.ColumnComment}\r\n");
                code.Append($"        /// <summary>\r\n");
                code.Append($"        [SugarColumn(ColumnName = \"{item.ColumnName}\"");
                if (item.IsPk == "1")
                {
                    code.Append($", IsPrimaryKey = true");
                }
                if (item.IsIncrement == "1")
                {
                    code.Append($", IsIdentity = true");
                }
                if (!"1".Equals(item.IsRequired))
                {
                    code.Append($", IsNullable = true");
                }
                switch (item.ColumnType)
                {
                    case "varchar":
                    case "char":
                    case "int":
                    case "bigint":
                        code.Append($", Length = {item.ColumnLength}");
                        break;
                    case "decimal":
                        code.Append($", Length = {item.ColumnLength}, DecimalDigits = {item.ColumnPrecision}");
                        break;
                    default: break;
                }
                code.Append($", ColumnDescription=\"{item.ColumnComment}\")]\r\n");
                if (item.ColumnType == "bigint")
                {
                    code.Append($"        [JsonConverter(typeof(LongToStringConverter))]\r\n");
                }
                code.Append($"        public {item.CType} {item.CField} {{ get; set; }}\r\n");
                code.Append($"\r\n");
            }
            code.Append($"    }}\r\n");
            code.Append($"}}\r\n");
            return code.ToString();
        }

        public static string BuildIService(GenTableAttr tableAttr)
        {
            var primaryKey = GetPrimaryKeyCol(tableAttr.Columns);
            StringBuilder code = new StringBuilder();
            string modelName = tableAttr.ClassName + "Model";
            code.Append($"using System;\r\n");
            code.Append($"using System.Collections.Generic;\r\n");
            code.Append($"using Wes.DbModel;\r\n");
            code.Append($"using Wes.Utils.Model;\r\n");
            code.Append($"\r\n");
            code.Append($"namespace Wes.Service\r\n");
            code.Append($"{{\r\n");
            code.Append($"    public interface I{tableAttr.ClassName}Service\r\n");
            code.Append($"    {{\r\n");
            //code.Append($"        /// <summary>\r\n");
            //code.Append($"        /// 查询{tableAttr.FunctionName}列表\r\n");
            //code.Append($"        /// <summary>\r\n");
            //code.Append($"        /// <param name=\"param\">查询条件</param>\r\n");
            //code.Append($"        /// <param name=\"total\">查询结果总条数</param>\r\n");
            //code.Append($"        /// <returns>查询结果</returns>\r\n");
            code.Append($"        public List<{modelName}> GetList(ParamData<{modelName}> param, out int total);\r\n");
            code.Append($"\r\n");
            code.Append($"        public List<{modelName}> GetAll();\r\n");
            code.Append($"\r\n");
            code.Append($"        public {modelName} GetById({primaryKey.CType} id);\r\n");
            code.Append($"\r\n");
            code.Append($"        public bool Save({modelName} model);\r\n");
            code.Append($"\r\n");
            code.Append($"        public bool Delete(List<{primaryKey.CType}> ids);\r\n");
            code.Append($"    }}\r\n");
            code.Append($"}}\r\n");
            return code.ToString();
        }

        public static string BuildService(GenTableAttr tableAttr)
        {
            var columns = tableAttr.Columns;
            var primaryKey = GetPrimaryKeyCol(tableAttr.Columns);
            var isDel = GetColByName(tableAttr.Columns, DbKey.IsDel);

            StringBuilder code = new StringBuilder();
            GenTableColumnAttr pkColumn = tableAttr.Columns.Where(p => p.IsPk == "1").FirstOrDefault();
            string modelName = tableAttr.ClassName + "Model";
            code.Append($"using System;\r\n");
            code.Append($"using SqlSugar;\r\n");
            code.Append($"using System.Collections.Generic;\r\n");
            code.Append($"using Wes.DbModel;\r\n");
            code.Append($"using Wes.Utils;\r\n");
            code.Append($"using Wes.Utils.Model;\r\n");
            code.Append($"\r\n");
            code.Append($"namespace Wes.Service\r\n");
            code.Append($"{{\r\n");
            code.Append($"    public class {tableAttr.ClassName}Service : Repository<{modelName}>, I{tableAttr.ClassName}Service\r\n");
            code.Append($"    {{\r\n");
            code.Append($"        public {tableAttr.ClassName}Service(ISqlSugarClient db) : base(db) {{ }}\r\n");
            code.Append($"\r\n");
            // GetList
            code.Append($"        public List<{modelName}> GetList(ParamData<{modelName}> param, out int total)\r\n");
            code.Append($"        {{\r\n");
            code.Append($"            Expressionable<{modelName}> express = Expressionable.Create<{modelName}>();\r\n");
            if (isDel != null)
            {
                code.Append($"            express.And(p => p.IsDel == 0);\r\n");
            }

            var queryColumns = tableAttr?.Columns?.Where(p => p.IsQuery == "1");
            if (queryColumns != null && queryColumns.Count() > 0)
            {
                code.Append($"            if (param.Params != null)\r\n");
                code.Append($"            {{\r\n");
                foreach (var queryItem in queryColumns)
                {
                    switch (queryItem.CType)
                    {
                        case "string":
                            code.Append($"                if (!string.IsNullOrWhiteSpace(param.Params.{queryItem.CField}))\r\n");
                            break;
                        case "long":
                        case "int":
                        case "short":
                        case "decimal":
                        case "double":
                        case "float":
                            code.Append($"                if (param.Params.{queryItem.CField} > 0)\r\n");
                            break;
                        default:
                            code.Append($"                if (param.Params.{queryItem.CField} != null)\r\n");
                            break;
                    }
                    code.Append($"                {{\r\n");
                    switch (queryItem.QueryType)
                    {
                        case "EQ":
                            code.Append($"                    express.And(p => p.{queryItem.CField} == param.Params.{queryItem.CField}{(queryItem.CType == "string" ? ".Trim()" : "")});");
                            break;
                        case "NE":
                            code.Append($"                    express.And(p => p.{queryItem.CField} != param.Params.{queryItem.CField}{(queryItem.CType == "string" ? ".Trim()" : "")});");
                            break;
                        case "GT":
                            code.Append($"                    express.And(p => p.{queryItem.CField} > param.Params.{queryItem.CField}{(queryItem.CType == "string" ? ".Trim()" : "")});");
                            break;
                        case "GTE":
                            code.Append($"                    express.And(p => p.{queryItem.CField} >= param.Params.{queryItem.CField}{(queryItem.CType == "string" ? ".Trim()" : "")});");
                            break;
                        case "LT":
                            code.Append($"                    express.And(p => p.{queryItem.CField} < param.Params.{queryItem.CField}{(queryItem.CType == "string" ? ".Trim()" : "")});");
                            break;
                        case "LTE":
                            code.Append($"                    express.And(p => p.{queryItem.CField} <= param.Params.{queryItem.CField}{(queryItem.CType == "string" ? ".Trim()" : "")});");
                            break;
                        case "LIKE":
                            code.Append($"                    express.And(p => p.{queryItem.CField}.Contains(param.Params.{queryItem.CField}{(queryItem.CType == "string" ? ".Trim()" : "")}));");
                            break;
                        default:
                            break;
                    }
                    code.Append($"\r\n");
                    code.Append($"                }}\r\n");
                }
                code.Append($"            }}\r\n");
            }

            code.Append($"            total = 0;\r\n");
            code.Append($"            var query = Context.Queryable<{modelName}>().Where(express.ToExpression());\r\n");
            code.Append($"            if (param.PageSize == 0)\r\n");
            code.Append($"                return query.ToList();\r\n");
            code.Append($"            return query.ToPageList(param.PageNum, param.PageSize, ref total);\r\n");
            code.Append($"        }}\r\n");
            code.Append($"\r\n");
            // GetAll
            code.Append($"        public List<{modelName}> GetAll()\r\n");
            code.Append($"        {{\r\n");
            code.Append($"            return GetList({(isDel != null ? "p => p.IsDel == 0" : "")});\r\n");
            code.Append($"        }}\r\n");
            code.Append($"\r\n");
            // GetById
            code.Append($"        public {modelName} GetById({primaryKey.CType} id)\r\n");
            code.Append($"        {{\r\n");
            code.Append($"            return GetFirst(p => p.{primaryKey.CField} == id{(isDel != null ? " && p.IsDel == 0" : "")});\r\n");
            code.Append($"        }}\r\n");
            code.Append($"\r\n");
            // Save
            code.Append($"        public bool Save({modelName} model)\r\n");
            code.Append($"        {{\r\n");
            code.Append($"            if (model.{primaryKey.CField} > 0)\r\n");
            code.Append($"            {{\r\n");
            if (columns.TryGetColByName(DbKey.UpdateTime, out var updateTime))
            {
                code.Append($"                model.{updateTime.CField} = DateTime.Now;\r\n");
            }
            if (columns.TryGetColByName(DbKey.UpdateBy, out var updateBy))
            {
                code.Append($"                model.{updateBy.CField} = GlobalContext.CurrentUser.Account;\r\n");
            }
            code.Append($"                return Update(model);\r\n");
            code.Append($"            }}\r\n");
            if (isDel != null)
            {
                code.Append($"            model.{isDel.CField} = 0;\r\n");
            }
            if (columns.TryGetColByName(DbKey.CreateTime, out var createTime))
            {
                code.Append($"            model.{createTime.CField} = DateTime.Now;\r\n");
            }
            if (columns.TryGetColByName(DbKey.CreateBy, out var createBy))
            {
                code.Append($"            model.{createBy.CField} = GlobalContext.CurrentUser.Account;\r\n");
            }
            code.Append($"            return InsertReturnSnowflakeId(model) > 0;\r\n");
            code.Append($"        }}\r\n");
            code.Append($"\r\n");
            // Delete
            code.Append($"        public bool Delete(List<{primaryKey.CType}> ids)\r\n");
            code.Append($"        {{\r\n");
            code.Append($"            if (ids == null || ids.Count == 0) return false;\r\n");
            if (isDel != null)
            {
                code.Append($"            return Context.Updateable<{modelName}>().SetColumns(p => p.IsDel == 1)\r\n");
                code.Append($"                .Where(p => ids.Contains(p.{primaryKey.CField})).ExecuteCommand() > 0;\r\n");
            }
            else
            {
                code.Append($"            return Context.Deleteable<{modelName}>().In(ids).ExecuteCommand() > 0;\r\n");
            }
            code.Append($"        }}\r\n");
            code.Append($"\r\n");
            code.Append($"    }}\r\n");
            code.Append($"}}\r\n");
            return code.ToString();
        }

        public static string BuildIBusiness(GenTableAttr tableAttr)
        {
            var primaryKey = GetPrimaryKeyCol(tableAttr.Columns);
            StringBuilder code = new StringBuilder();
            string modelName = tableAttr.ClassName + "Model";
            code.Append($"using System;\r\n");
            code.Append($"using System.Collections.Generic;\r\n");
            code.Append($"using Wes.DbModel;\r\n");
            code.Append($"using Wes.Utils.Model;\r\n");
            code.Append($"\r\n");
            code.Append($"namespace Wes.Business\r\n");
            code.Append($"{{\r\n");
            code.Append($"    public interface I{tableAttr.ClassName}Biz\n");
            code.Append($"    {{\r\n");
            code.Append($"        public RowData<{modelName}> GetList(ParamData<{modelName}> param);\r\n");
            code.Append($"\r\n");
            code.Append($"        public ResultData<List<{modelName}>> GetAll();\r\n");
            code.Append($"\r\n");
            code.Append($"        public ResultData<{modelName}> GetById({primaryKey.CType} id);\r\n");
            code.Append($"\r\n");
            code.Append($"        public ReturnData Save({modelName} model);\r\n");
            code.Append($"\r\n");
            code.Append($"        public ReturnData Delete(string ids);\r\n");
            code.Append($"    }}\r\n");
            code.Append($"}}\r\n");
            return code.ToString();
        }

        public static string BuildBusiness(GenTableAttr tableAttr)
        {
            StringBuilder code = new StringBuilder();
            var primaryKey = GetPrimaryKeyCol(tableAttr.Columns);
            string modelName = tableAttr.ClassName + "Model";
            string serviceName = tableAttr.ClassName.ToFirstCharLower() + "Service";
            string privateServiceName = "_" + serviceName;
            code.Append($"using System;\r\n");
            code.Append($"using System.Linq;\r\n");
            code.Append($"using Wes.Service;\r\n");
            code.Append($"using Wes.Utils.Extension;\r\n");
            code.Append($"using Wes.DbModel;\r\n");
            code.Append($"using Wes.Utils.Model;\r\n");
            code.Append($"using System.Collections.Generic;\r\n");
            code.Append($"\r\n");
            code.Append($"namespace Wes.Business\r\n");
            code.Append($"{{\r\n");
            code.Append($"    public class {tableAttr.ClassName}Biz : I{tableAttr.ClassName}Biz\r\n");
            code.Append($"    {{\r\n");
            code.Append($"        private I{tableAttr.ClassName}Service {privateServiceName};\r\n");
            code.Append($"\r\n");
            code.Append($"        public {tableAttr.ClassName}Biz(I{tableAttr.ClassName}Service {serviceName})\r\n");
            code.Append($"        {{\r\n");
            code.Append($"            {privateServiceName} = {serviceName};\r\n");
            code.Append($"        }}\r\n");
            code.Append($"\r\n");
            // GetById
            code.Append($"        public ResultData<{modelName}> GetById({primaryKey.CType} id)\r\n");
            code.Append($"        {{\r\n");
            code.Append($"            return new ResultData<{modelName}>({privateServiceName}.GetById(id));\r\n");
            code.Append($"        }}\r\n");
            code.Append($"\r\n");
            // GetAll
            code.Append($"        public ResultData<List<{modelName}>> GetAll()\r\n");
            code.Append($"        {{\r\n");
            code.Append($"            return new ResultData<List<{modelName}>>({privateServiceName}.GetAll());\r\n");
            code.Append($"        }}\r\n");
            code.Append($"\r\n");
            // GetList
            code.Append($"        public RowData<{modelName}> GetList(ParamData<{modelName}> param)\r\n");
            code.Append($"        {{\r\n");
            code.Append($"            int total = 0;\r\n");
            code.Append($"            RowData<{modelName}> result = new RowData<{modelName}>({privateServiceName}.GetList(param, out total));\r\n");
            code.Append($"            result.total = total;\r\n");
            code.Append($"            return result;\r\n");
            code.Append($"        }}\r\n");
            code.Append($"\r\n");
            // Save
            code.Append($"        public ReturnData Save({modelName} model)\r\n");
            code.Append($"        {{\r\n");
            code.Append($"            if ({privateServiceName}.Save(model))\r\n");
            code.Append($"            {{\r\n");
            code.Append($"                return new ReturnData();\r\n");
            code.Append($"            }}\r\n");
            code.Append($"            return new ReturnData(500, \"保存失败！\");\r\n");
            code.Append($"        }}\r\n");
            code.Append($"\r\n");
            // Delete
            code.Append($"        public ReturnData Delete(string ids)\r\n");
            code.Append($"        {{\r\n");
            //TODO 这里暂时主键支持string和long 后期可改成任意类型
            if (primaryKey.CType == "string")
            {
                code.Append($"            var delIds = ids.ToStringList();\r\n");
            }
            else
            {
                code.Append($"            var delIds = ids.ToLongList();\r\n");
            }
            code.Append($"            if (delIds == null || delIds.Count == 0)\r\n");
            code.Append($"            {{\r\n");
            code.Append($"                return new ReturnData(500, \"参数有误，请检查参数！\");\r\n");
            code.Append($"            }}\r\n");
            code.Append($"            if ({privateServiceName}.Delete(delIds))\r\n");
            code.Append($"            {{\r\n");
            code.Append($"                return new ReturnData();\r\n");
            code.Append($"            }}\r\n");
            code.Append($"            return new ReturnData(500, \"删除失败！\");\r\n");
            code.Append($"        }}\r\n");
            code.Append($"\r\n");
            code.Append($"    }}\r\n");
            code.Append($"}}\r\n");
            return code.ToString();
        }

        public static string BuildController(GenTableAttr tableAttr)
        {
            StringBuilder code = new StringBuilder();
            string modelName = tableAttr.ClassName + "Model";
            string bizName = tableAttr.ClassName.ToFirstCharLower() + "Biz";
            string privateBizName = "_" + bizName;
            code.Append($"using System;\r\n");
            code.Append($"using System.Linq;\r\n");
            code.Append($"using Wes.DbModel;\r\n");
            code.Append($"using Wes.Business;\r\n");
            code.Append($"using Wes.Utils.Model;\r\n");
            code.Append($"using Wes.Utils.Extension;\r\n");
            code.Append($"using Microsoft.AspNetCore.Mvc;\r\n");
            code.Append($"using System.Collections.Generic;\r\n");
            code.Append($"\r\n");
            code.Append($"namespace Wes.WebApi.Areas.{tableAttr.ModuleName.ToFirstCharUpper()}Manage\r\n");
            code.Append($"{{\r\n");
            code.Append($"    [ApiController]\r\n");
            code.Append($"    [Route(\"{tableAttr.ModuleName}/{tableAttr.BusinessName}\")]\r\n");
            code.Append($"    public class {tableAttr.ClassName}Controller : ControllerBase\r\n");
            code.Append($"    {{\r\n");
            code.Append($"        private I{tableAttr.ClassName}Biz {privateBizName};\r\n");
            code.Append($"\r\n");
            code.Append($"        public {tableAttr.ClassName}Controller(I{tableAttr.ClassName}Biz {bizName})\r\n");
            code.Append($"        {{\r\n");
            code.Append($"            {privateBizName} = {bizName};\r\n");
            code.Append($"        }}\r\n");
            code.Append($"\r\n");
            // GetList
            code.Append($"        [HttpGet]\r\n");
            code.Append($"        [Route(\"list\")]\r\n");
            code.Append($"        public ReturnData GetList([FromQuery] ParamData<{modelName}> param)\r\n");
            code.Append($"        {{\r\n");
            code.Append($"            return {privateBizName}.GetList(param);\r\n");
            code.Append($"        }}\r\n");
            code.Append($"\r\n");
            // GetById
            code.Append($"        [HttpGet]\r\n");
            code.Append($"        [Route(\"{{id}}\")]\r\n");
            code.Append($"        public ReturnData GetById(long id)\r\n");
            code.Append($"        {{\r\n");
            code.Append($"            return {privateBizName}.GetById(id);\r\n");
            code.Append($"        }}\r\n");
            code.Append($"\r\n");
            // Update
            code.Append($"        [HttpPut]\r\n");
            code.Append($"        public ReturnData Update([FromBody] {modelName} model)\r\n");
            code.Append($"        {{\r\n");
            code.Append($"            return {privateBizName}.Save(model);\r\n");
            code.Append($"        }}\r\n");
            code.Append($"\r\n");
            // Insert
            code.Append($"        [HttpPost]\r\n");
            code.Append($"        public ReturnData Insert([FromBody] {modelName} model)\r\n");
            code.Append($"        {{\r\n");
            code.Append($"            return {privateBizName}.Save(model);\r\n");
            code.Append($"        }}\r\n");
            code.Append($"\r\n");
            // Delete
            code.Append($"        [HttpDelete]\r\n");
            code.Append($"        [Route(\"{{ids}}\")]\r\n");
            code.Append($"        public ReturnData Delete(string ids)\r\n");
            code.Append($"        {{\r\n");
            code.Append($"            return {privateBizName}.Delete(ids);\r\n");
            code.Append($"        }}\r\n");
            code.Append($"\r\n");
            code.Append($"    }}\r\n");
            code.Append($"}}\r\n");

            return code.ToString();
        }

        public static string BuildApiJs(GenTableAttr tableAttr)
        {
            StringBuilder code = new StringBuilder();
            string businessName = tableAttr.BusinessName.ToFirstCharUpper();
            code.Append($"import request from '@/utils/request'\r\n");
            code.Append($"\r\n");
            code.Append($"// 获取{tableAttr.FunctionName}列表\r\n");
            code.Append($"export function list{businessName}(query) {{\r\n");
            code.Append($"  return request({{\r\n");
            code.Append($"    url: '/{tableAttr.ModuleName}/{tableAttr.BusinessName}/list',\r\n");
            code.Append($"    method: 'get',\r\n");
            code.Append($"    params: query\r\n");
            code.Append($"  }})\r\n");
            code.Append($"}}\r\n");
            code.Append($"\r\n");
            code.Append($"// 获取{tableAttr.FunctionName}\r\n");
            code.Append($"export function get{businessName}(id) {{\r\n");
            code.Append($"  return request({{\r\n");
            code.Append($"    url: '/{tableAttr.ModuleName}/{tableAttr.BusinessName}/' + id,\r\n");
            code.Append($"   method: 'get'\r\n");
            code.Append($"  }})\r\n");
            code.Append($"}}\r\n");
            code.Append($"\r\n");
            code.Append($"// 新增{tableAttr.FunctionName}\r\n");
            code.Append($"export function add{businessName}(data) {{\r\n");
            code.Append($"  return request({{\r\n");
            code.Append($"    url: '/{tableAttr.ModuleName}/{tableAttr.BusinessName}',\r\n");
            code.Append($"    method: 'post',\r\n");
            code.Append($"    data: data\r\n");
            code.Append($"  }})\r\n");
            code.Append($"}}\r\n");
            code.Append($"\r\n");
            code.Append($"// 更新{tableAttr.FunctionName}\r\n");
            code.Append($"export function update{businessName}(data) {{\r\n");
            code.Append($"  return request({{\r\n");
            code.Append($"    url: '/{tableAttr.ModuleName}/{tableAttr.BusinessName}',\r\n");
            code.Append($"    method: 'put',\r\n");
            code.Append($"    data: data\r\n");
            code.Append($"  }})\r\n");
            code.Append($"}}\r\n");
            code.Append($"\r\n");
            code.Append($"// 删除{tableAttr.FunctionName}\r\n");
            code.Append($"export function del{businessName}(ids) {{\r\n");
            code.Append($"  return request({{\r\n");
            code.Append($"    url: '/{tableAttr.ModuleName}/{tableAttr.BusinessName}/' + ids,\r\n");
            code.Append($"    method: 'delete',\r\n");
            code.Append($"  }})\r\n");
            code.Append($"}}\r\n");
            code.Append($"\r\n");
            return code.ToString();
        }

        public static string BuildIndex(GenTableAttr tableAttr)
        {
            StringBuilder code = new StringBuilder();
            var searchCols = tableAttr.Columns.Where(p => p.IsQuery == "1");
            var editCols = tableAttr.Columns.Where(p => p.IsInsert == "1");
            var showCols = tableAttr.Columns.Where(p => p.IsList == "1");
            var requiredCols = tableAttr.Columns.Where(p => p.IsRequired == "1");
            var dicList = tableAttr.Columns.Where(p => !string.IsNullOrWhiteSpace(p.DictType)).Select(p => $"\"{p.DictType}\"");
            string businessName = tableAttr.BusinessName.ToFirstCharUpper();
            var primaryKeyName = GetPrimaryKeyCol(tableAttr.Columns).CField.ToFirstCharLower();
            code.Append($@"<template>
  <div class=""app-container"">
    <el-form
      :model=""queryParams""
      ref=""queryForm""
      size=""small""
      :inline=""true""
      v-show=""showSearch""
      label-width=""68px""
    >");
            code.Append("\r\n");
            foreach (var item in searchCols)
            {
                if (!string.IsNullOrWhiteSpace(item.DictType))
                {
                    code.Append($@"      <el-form-item label=""{item.ColumnComment}"" prop=""{item.CField.ToFirstCharLower()}"">
        <el-select
          v-model=""queryParams.params.{item.CField.ToFirstCharLower()}""
          placeholder=""{item.ColumnComment}""
          style=""width: 100%""
          filterable
          clearable
        >
          <el-option
            v-for=""dict in dict.type.{item.DictType.Trim()}""
            :key=""dict.value""
            :label=""dict.label""
            :value=""dict.value""
          />
        </el-select>
      </el-form-item>");
                    code.Append("\r\n");
                }
                else
                {
                    // TODO 其他类型
                    switch (item.HtmlType)
                    {
                        case "input":
                        default:
                            code.Append($@"      <el-form-item label=""{item.ColumnComment}"" prop=""{item.CField.ToFirstCharLower()}"">
        <el-input
          v-model=""queryParams.params.{item.CField.ToFirstCharLower()}""
          placeholder=""请输入{item.ColumnComment}""
          clearable
          @keyup.enter.native=""handleQuery""
        />
      </el-form-item>");
                            code.Append("\r\n");
                            break;
                    }
                }
            }
            code.Append($@" <el-form-item>
        <el-button
          type=""primary""
          icon=""el-icon-search""
          size=""mini""
          @click=""handleQuery""
          >搜索</el-button
        >
        <el-button icon=""el-icon-refresh"" size=""mini"" @click=""resetQuery""
          >重置</el-button
        >
      </el-form-item>
    </el-form>

    <el-row :gutter=""10"" class=""mb8"">
      <el-col :span=""1.5"">
        <el-button
          type=""primary""
          plain
          icon=""el-icon-plus""
          size=""mini""
          @click=""handleAdd""
          v-hasPermi=""['system:post:add']""
          >新增</el-button
        >
      </el-col>
      <el-col :span=""1.5"">
        <el-button
          type=""success""
          plain
          icon=""el-icon-edit""
          size=""mini""
          :disabled=""single""
          @click=""handleUpdate""
          v-hasPermi=""['system:post:edit']""
          >修改</el-button
        >
      </el-col>
      <el-col :span=""1.5"">
        <el-button
          type=""danger""
          plain
          icon=""el-icon-delete""
          size=""mini""
          :disabled=""multiple""
          @click=""handleDelete""
          v-hasPermi=""['system:post:remove']""
          >删除</el-button
        >
      </el-col>
      <right-toolbar
        :showSearch.sync=""showSearch""
        @queryTable=""getList""
      ></right-toolbar>
    </el-row>

    <el-table
      v-loading=""loading""
      :data=""dataList""
      @selection-change=""handleSelectionChange""
    >
      <el-table-column type=""selection"" width=""55"" align=""center"" />");
            code.Append("\r\n");
            foreach (var item in showCols)
            {
                if (!string.IsNullOrWhiteSpace(item.DictType))
                {
                    code.Append($@"      <el-table-column label=""{item.ColumnComment}"" align=""center"" prop=""{item.CField.ToFirstCharLower()}"">
        <template slot-scope=""scope"">
          <dict-tag
            :options=""dict.type.{item.DictType.Trim()}""
            :value=""scope.row.{item.CField.ToFirstCharLower()}""
          />
        </template>
      </el-table-column>");
                    code.Append("\r\n");
                }
                else
                {
                    switch (item.CType)
                    {
                        case "DateTime":
                        case "DateTime?":
                            code.Append($@"      <el-table-column
        label=""{item.ColumnComment}""
        align=""center""
        prop=""{item.CField.ToFirstCharLower()}""
        width=""180""
      >
        <template slot-scope=""scope"">
          <span>{{{{ parseTime(scope.row.{item.CField.ToFirstCharLower()}) }}}}</span>
        </template>
      </el-table-column>");
                            code.Append("\r\n");
                            break;
                        default:
                            code.Append($@"      <el-table-column label=""{item.ColumnComment}"" align=""center"" prop=""{item.CField.ToFirstCharLower()}"" />");
                            code.Append("\r\n");
                            break;
                    }
                }
            }
            code.Append($@"<el-table-column
        label=""操作""
        align=""center""
        class-name=""small-padding fixed-width""
      >
        <template slot-scope=""scope"">
          <el-button
            size=""mini""
            type=""text""
            icon=""el-icon-edit""
            @click=""handleUpdate(scope.row)""
            v-hasPermi=""['system:post:edit']""
            >修改</el-button
          >
          <el-button
            size=""mini""
            type=""text""
            icon=""el-icon-delete""
            @click=""handleDelete(scope.row)""
            v-hasPermi=""['system:post:remove']""
            >删除</el-button
          >
        </template>
      </el-table-column>
    </el-table>

    <pagination
      v-show=""total > 0""
      :total=""total""
      :page.sync=""queryParams.pageNum""
      :limit.sync=""queryParams.pageSize""
      @pagination=""getList""
    />

    <!-- 添加或修改数据对话框 -->
    <el-dialog :title=""title"" :visible.sync=""open"" width=""500px"" append-to-body>
      <el-form ref=""form"" :model=""form"" :rules=""rules"" label-width=""80px"">");
            code.Append("\r\n");
            foreach (var item in editCols)
            {
                if (!string.IsNullOrWhiteSpace(item.DictType))
                {
                    code.Append($@"        <el-form-item label=""{item.ColumnComment}"" prop=""{item.CField.ToFirstCharLower()}"">
          <el-radio-group v-model=""form.{item.CField.ToFirstCharLower()}"">
            <el-radio
              v-for=""dict in dict.type.{item.DictType}""
              :key=""dict.value""
              :label=""dict.value""
              >{{{{ dict.label }}}}</el-radio
            >
          </el-radio-group>
        </el-form-item>");
                    code.Append("\r\n");
                }
                else
                {
                    switch (item.HtmlType)
                    {
                        case "textarea":
                            code.Append($@"        <el-form-item label=""{item.ColumnComment}"" prop=""{item.CField.ToFirstCharLower()}"">
          <el-input
            v-model=""form.{item.CField.ToFirstCharLower()}""
            type=""textarea""
            placeholder=""请输入{item.ColumnComment}""
          />
        </el-form-item>");
                            code.Append("\r\n");
                            break;
                        case "input":
                        default:
                            code.Append($@"        <el-form-item label=""{item.ColumnComment}"" prop=""{item.CField.ToFirstCharLower()}"">
          <el-input v-model=""form.{item.CField.ToFirstCharLower()}"" placeholder=""请输入{item.ColumnComment}"" />
        </el-form-item>");
                            code.Append("\r\n");
                            break;
                    }
                }
            }
            code.Append($@" </el-form>
      <div slot=""footer"" class=""dialog-footer"">
        <el-button type=""primary"" @click=""submitForm"">确 定</el-button>
        <el-button @click=""cancel"">取 消</el-button>
      </div>
    </el-dialog>
  </div>
</template>

<script>
import {{
  list{businessName},
  get{businessName},
  del{businessName},
  add{businessName},
  update{businessName},
}} from ""@/api/{tableAttr.ModuleName}/{tableAttr.BusinessName}"";


export default {{
  name: ""{tableAttr.ClassName}"",
  dicts: [{string.Join(", ", dicList)}],
  data() {{
    return {{
      // 遮罩层
      loading: true,
      // 选中数组
      ids: [],
      // 非单个禁用
      single: true,
      // 非多个禁用
      multiple: true,
      // 显示搜索条件
      showSearch: true,
      // 总条数
      total: 0,
      // 表格数据
      dataList: [],
      // 弹出层标题
      title: """",
      // 是否显示弹出层
      open: false,
      // 查询参数
      queryParams: {{
        pageNum: 1,
        pageSize: 10,
        params: {{}},
      }},
      // 表单参数
      form: {{}},
      // 表单校验
      rules: {{");
            code.Append("\r\n");
            foreach (var item in requiredCols)
            {
                code.Append($@"        {item.CField.ToFirstCharLower()}: [
          {{required: true, message: ""{item.ColumnComment}不能为空"", trigger: ""blur"" }},
        ],");
                code.Append("\r\n");
            }
            code.Append($@"}},
    }};
  }},
  created() {{
    this.getList();
  }},
  methods: {{
    /** 查询列表 */
    getList() {{
      this.loading = true;
      list{businessName}(this.queryParams).then((response) => {{
        this.dataList = response.rows;
        this.total = response.total;
        this.loading = false;
      }});
    }},
    // 取消按钮
    cancel() {{
      this.open = false;
      this.reset();
    }},
    // 表单重置
    reset() {{
      this.form = {{}};
      this.resetForm(""form"");
    }},
    /** 搜索按钮操作 */
    handleQuery() {{
      this.queryParams.pageNum = 1;
      this.getList();
    }},
    /** 重置按钮操作 */
    resetQuery() {{
      this.resetForm(""queryForm"");
      this.handleQuery();
    }},
    // 多选框选中数据
    handleSelectionChange(selection) {{
      this.ids = selection.map((item) => item.{primaryKeyName});
      this.single = selection.length != 1;
      this.multiple = !selection.length;
    }},
    /** 新增按钮操作 */
    handleAdd() {{
      this.reset();
      this.open = true;
      this.title = ""添加{tableAttr.FunctionName}"";
    }},
    /** 修改按钮操作 */
    handleUpdate(row) {{
      this.reset();
      const {primaryKeyName} = row.{primaryKeyName} || this.ids;
      get{businessName}({primaryKeyName}).then((response) => {{
        this.form = response.data;
        this.open = true;
        this.title = ""修改{tableAttr.FunctionName}"";
      }});
    }},
    /** 提交按钮 */
    submitForm: function () {{
      this.$refs[""form""].validate((valid) => {{
        if (valid) {{
          if (this.form.{primaryKeyName} != undefined) {{
            update{businessName}(this.form).then((response) => {{
              this.$modal.msgSuccess(""修改成功"");
              this.open = false;
              this.getList();
            }});
          }} else {{
            add{businessName}(this.form).then((response) => {{
              this.$modal.msgSuccess(""新增成功"");
              this.open = false;
              this.getList();
            }});
          }}
        }}
      }});
    }},
    /** 删除按钮操作 */
    handleDelete(row) {{
      const {primaryKeyName}s = row.{primaryKeyName} || this.ids;
      this.$modal
        .confirm('是否确认删除编号为""' + {primaryKeyName}s + '""的数据项？')
        .then(function () {{
          return del{businessName}({primaryKeyName}s);
        }})
        .then(() => {{
          this.getList();
          this.$modal.msgSuccess(""删除成功"");
        }})
        .catch(() => {{}});
    }},
  }},
}};
</script>");
            return code.ToString();
        }

        public static string BuildListIndex(GenTableAttr tableAttr)
        {
            StringBuilder code = new StringBuilder();
            var searchCols = tableAttr.Columns.Where(p => p.IsQuery == "1");
            var editCols = tableAttr.Columns.Where(p => p.IsInsert == "1");
            var showCols = tableAttr.Columns.Where(p => p.IsList == "1");
            var requiredCols = tableAttr.Columns.Where(p => p.IsRequired == "1");
            var dicList = tableAttr.Columns.Where(p => !string.IsNullOrWhiteSpace(p.DictType)).Select(p => $"\"{p.DictType}\"");
            string businessName = tableAttr.BusinessName.ToFirstCharUpper();
            var primaryKeyName = GetPrimaryKeyCol(tableAttr.Columns).CField.ToFirstCharLower();
            code.Append($@"<template>
  <div class=""app-container"">
    <el-form
      :model=""queryParams""
      ref=""queryForm""
      size=""small""
      :inline=""true""
      v-show=""showSearch""
      label-width=""68px""
    >");
            code.Append("\r\n");
            foreach (var item in searchCols)
            {
                if (!string.IsNullOrWhiteSpace(item.DictType))
                {
                    code.Append($@"      <el-form-item label=""{item.ColumnComment}"" prop=""{item.CField.ToFirstCharLower()}"">
        <el-select
          v-model=""queryParams.params.{item.CField.ToFirstCharLower()}""
          placeholder=""{item.ColumnComment}""
          style=""width: 100%""
          filterable
          clearable
        >
          <el-option
            v-for=""dict in dict.type.{item.DictType.Trim()}""
            :key=""dict.value""
            :label=""dict.label""
            :value=""dict.value""
          />
        </el-select>
      </el-form-item>");
                    code.Append("\r\n");
                }
                else
                {
                    // TODO 其他类型
                    switch (item.HtmlType)
                    {
                        case "input":
                        default:
                            code.Append($@"      <el-form-item label=""{item.ColumnComment}"" prop=""{item.CField.ToFirstCharLower()}"">
        <el-input
          v-model=""queryParams.params.{item.CField.ToFirstCharLower()}""
          placeholder=""请输入{item.ColumnComment}""
          clearable
          @keyup.enter.native=""handleQuery""
        />
      </el-form-item>");
                            code.Append("\r\n");
                            break;
                    }
                }
            }
            code.Append($@"      <el-form-item>
        <el-button
          type=""primary""
          icon=""el-icon-search""
          size=""mini""
          @click=""handleQuery""
          >搜索</el-button
        >
        <el-button icon=""el-icon-refresh"" size=""mini"" @click=""resetQuery""
          >重置</el-button
        >
      </el-form-item>
    </el-form>

    <el-row :gutter=""10"" class=""mb8"">
      <el-col :span=""1.5"">
        <el-button
          type=""primary""
          plain
          icon=""el-icon-plus""
          size=""mini""
          @click=""handleAdd""
          v-hasPermi=""['system:post:add']""
          >新增</el-button
        >
      </el-col>
      <el-col :span=""1.5"">
        <el-button
          type=""success""
          plain
          icon=""el-icon-edit""
          size=""mini""
          :disabled=""single""
          @click=""handleUpdate""
          v-hasPermi=""['system:post:edit']""
          >修改</el-button
        >
      </el-col>
      <el-col :span=""1.5"">
        <el-button
          type=""danger""
          plain
          icon=""el-icon-delete""
          size=""mini""
          :disabled=""multiple""
          @click=""handleDelete""
          v-hasPermi=""['system:post:remove']""
          >删除</el-button
        >
      </el-col>
      <right-toolbar
        :showSearch.sync=""showSearch""
        @queryTable=""getList""
      ></right-toolbar>
    </el-row>

    <el-table
      v-loading=""loading""
      :data=""dataList""
      @selection-change=""handleSelectionChange""
    >
      <el-table-column type=""selection"" width=""55"" align=""center"" />");
            code.Append("\r\n");
            foreach (var item in showCols)
            {
                if (!string.IsNullOrWhiteSpace(item.DictType))
                {
                    code.Append($@"      <el-table-column label=""{item.ColumnComment}"" align=""center"" prop=""{item.CField.ToFirstCharLower()}"">
        <template slot-scope=""scope"">
          <dict-tag
            :options=""dict.type.{item.DictType.Trim()}""
            :value=""scope.row.{item.CField.ToFirstCharLower()}""
          />
        </template>
      </el-table-column>");
                    code.Append("\r\n");
                }
                else
                {
                    switch (item.CType)
                    {
                        case "DateTime":
                        case "DateTime?":
                            code.Append($@"      <el-table-column
        label=""{item.ColumnComment}""
        align=""center""
        prop=""{item.CField.ToFirstCharLower()}""
        width=""180""
      >
        <template slot-scope=""scope"">
          <span>{{{{ parseTime(scope.row.{item.CField.ToFirstCharLower()}) }}}}</span>
        </template>
      </el-table-column>");
                            code.Append("\r\n");
                            break;
                        default:
                            code.Append($@"      <el-table-column label=""{item.ColumnComment}"" align=""center"" prop=""{item.CField.ToFirstCharLower()}"" />");
                            code.Append("\r\n");
                            break;
                    }
                }
            }
            code.Append($@"<el-table-column
        label=""操作""
        align=""center""
        class-name=""small-padding fixed-width""
      >
        <template slot-scope=""scope"">
          <el-button
            size=""mini""
            type=""text""
            icon=""el-icon-edit""
            @click=""handleUpdate(scope.row)""
            v-hasPermi=""['system:post:edit']""
            >修改</el-button
          >
          <el-button
            size=""mini""
            type=""text""
            icon=""el-icon-delete""
            @click=""handleDelete(scope.row)""
            v-hasPermi=""['system:post:remove']""
            >删除</el-button
          >
        </template>
      </el-table-column>
    </el-table>

    <pagination
      v-show=""total > 0""
      :total=""total""
      :page.sync=""queryParams.pageNum""
      :limit.sync=""queryParams.pageSize""
      @pagination=""getList""
    />

    <form-edit ref=""formEdit"" @change=""getList""></form-edit>

  </div>
</template>
");
            code.Append($@"<script>
import {{
  list{businessName},
  get{businessName},
  del{businessName},
}} from ""@/api/{tableAttr.ModuleName}/{tableAttr.BusinessName}"";
import formEdit from ""@/views/{tableAttr.ModuleName}/{tableAttr.BusinessName}/edit"";

export default {{
  name: ""{tableAttr.ClassName}"",
  dicts: [{string.Join(", ", dicList)}],
  components: {{ formEdit }},
  data() {{
    return {{
      // 遮罩层
      loading: true,
      // 选中数组
      ids: [],
      // 非单个禁用
      single: true,
      // 非多个禁用
      multiple: true,
      // 显示搜索条件
      showSearch: true,
      // 总条数
      total: 0,
      // 表格数据
      dataList: [],
      // 查询参数
      queryParams: {{
        pageNum: 1,
        pageSize: 10,
        params: {{}},
      }},
    }};
  }},
  created() {{
    this.getList();
  }},
  methods: {{
    /** 查询列表 */
    getList() {{
      this.loading = true;
      list{businessName}(this.queryParams).then((response) => {{
        this.dataList = response.rows;
        this.total = response.total;
        this.loading = false;
      }});
    }},
    /** 搜索按钮操作 */
    handleQuery() {{
      this.queryParams.pageNum = 1;
      this.getList();
    }},
    /** 重置按钮操作 */
    resetQuery() {{
      this.resetForm(""queryForm"");
      this.handleQuery();
    }},
    // 多选框选中数据
    handleSelectionChange(selection) {{
      this.ids = selection.map((item) => item.{primaryKeyName});
      this.single = selection.length != 1;
      this.multiple = !selection.length;
    }},
    /** 新增按钮操作 */
    handleAdd() {{
      this.$refs.formEdit.openDialog();
    }},
    /** 修改按钮操作 */
    handleUpdate(row) {{
      const {primaryKeyName} = row.{primaryKeyName} || this.ids;
      this.$refs.formEdit.openDialog({primaryKeyName});
    }},
    /** 删除按钮操作 */
    handleDelete(row) {{
      const {primaryKeyName}s = row.{primaryKeyName} || this.ids;
      this.$modal
        .confirm('是否确认删除编号为""' + {primaryKeyName}s + '""的数据项？')
        .then(function () {{
          return del{businessName}({primaryKeyName}s);
        }})
        .then(() => {{
          this.getList();
          this.$modal.msgSuccess(""删除成功"");
        }})
        .catch(() => {{}});
    }},
  }},
}};
</script>");
            return code.ToString();
        }

        public static string BuildDialogIndex(GenTableAttr tableAttr)
        {
            StringBuilder code = new StringBuilder();
            var searchCols = tableAttr.Columns.Where(p => p.IsQuery == "1");
            var editCols = tableAttr.Columns.Where(p => p.IsInsert == "1");
            var showCols = tableAttr.Columns.Where(p => p.IsList == "1");
            var requiredCols = tableAttr.Columns.Where(p => p.IsRequired == "1");
            var dicList = tableAttr.Columns.Where(p => !string.IsNullOrWhiteSpace(p.DictType)).Select(p => $"\"{p.DictType}\"");
            string businessName = tableAttr.BusinessName.ToFirstCharUpper();
            var primaryKeyName = GetPrimaryKeyCol(tableAttr.Columns).CField.ToFirstCharLower();
            code.Append($@"<template>
  <div class=""app-container"">");
            code.Append($@"
    <!-- 添加或修改数据对话框 -->
    <el-dialog :title=""title"" :visible.sync=""open"" width=""500px"" append-to-body>
      <el-form ref=""form"" :model=""form"" :rules=""rules"" label-width=""80px"">");
            code.Append("\r\n");
            foreach (var item in editCols)
            {
                if (!string.IsNullOrWhiteSpace(item.DictType))
                {
                    code.Append($@"        <el-form-item label=""{item.ColumnComment}"" prop=""{item.CField.ToFirstCharLower()}"">
          <el-radio-group v-model=""form.{item.CField.ToFirstCharLower()}"">
            <el-radio
              v-for=""dict in dict.type.{item.DictType}""
              :key=""dict.value""
              :label=""dict.value""
              >{{{{ dict.label }}}}</el-radio
            >
          </el-radio-group>
        </el-form-item>");
                    code.Append("\r\n");
                }
                else
                {
                    switch (item.HtmlType)
                    {
                        case "textarea":
                            code.Append($@"        <el-form-item label=""{item.ColumnComment}"" prop=""{item.CField.ToFirstCharLower()}"">
          <el-input
            v-model=""form.{item.CField.ToFirstCharLower()}""
            type=""textarea""
            placeholder=""请输入{item.ColumnComment}""
          />
        </el-form-item>");
                            code.Append("\r\n");
                            break;
                        case "input":
                        default:
                            code.Append($@"        <el-form-item label=""{item.ColumnComment}"" prop=""{item.CField.ToFirstCharLower()}"">
          <el-input v-model=""form.{item.CField.ToFirstCharLower()}"" placeholder=""请输入{item.ColumnComment}"" />
        </el-form-item>");
                            code.Append("\r\n");
                            break;
                    }
                }
            }
            code.Append($@" </el-form>
      <div slot=""footer"" class=""dialog-footer"">
        <el-button type=""primary"" @click=""submitForm"">确 定</el-button>
        <el-button @click=""cancel"">取 消</el-button>
      </div>
    </el-dialog>
  </div>
</template>

<script>
import {{
  get{businessName},
  add{businessName},
  update{businessName},
}} from ""@/api/{tableAttr.ModuleName}/{tableAttr.TableName}"";


export default {{
  name: ""{tableAttr.ClassName}"",
  dicts: [{string.Join(", ", dicList)}],
  data() {{
    return {{
      // 遮罩层
      loading: true,
      // 弹出层标题
      title: """",
      // 是否显示弹出层
      open: false,
      // 表单参数
      form: {{}},
      // 表单校验
      rules: {{");
            code.Append("\r\n");
            foreach (var item in requiredCols)
            {
                code.Append($@"        {item.CField.ToFirstCharLower()}: [
          {{required: true, message: ""{item.ColumnComment}不能为空"", trigger: ""blur"" }},
        ],");
                code.Append("\r\n");
            }
            code.Append($@"}},
    }};
  }},
  created() {{
  }},
  methods: {{
    // 取消按钮
    cancel() {{
      this.open = false;
      this.reset();
    }},
    // 表单重置
    reset() {{
      this.form = {{}};
      this.resetForm(""form"");
    }},
    /** 打开编辑页面 */
    openDialog(id) {{
      this.reset();
      this.title = ""添加{tableAttr.FunctionName}"";
      if (id) {{
        this.title = ""编辑{tableAttr.FunctionName}"";
        get{businessName}(id).then((response) => {{
          this.form = response.data;
          this.open = true;
        }});
      }} else {{
        this.open = true;
      }}
    }},
    /** 提交按钮 */
    submitForm: function () {{
      this.$refs[""form""].validate((valid) => {{
        if (valid) {{
          if (this.form.{primaryKeyName} != undefined) {{
            update{businessName}(this.form).then((response) => {{
              this.$modal.msgSuccess(""修改成功"");
              this.open = false;
              this.$emit(""change"");
            }});
          }} else {{
            add{businessName}(this.form).then((response) => {{
              this.$modal.msgSuccess(""新增成功"");
              this.open = false;
              this.$emit(""change"");
            }});
          }}
        }}
      }});
    }},
  }},
}};
</script>");
            return code.ToString();
        }

        #region

        private static GenTableColumnAttr GetPrimaryKeyCol(List<GenTableColumnAttr> cols)
        {
            return cols.Where(p => p.IsPk == "1").FirstOrDefault();
        }

        private static GenTableColumnAttr GetColByName(List<GenTableColumnAttr> cols, string name)
        {
            return cols.Where(p => p.ColumnName == name).FirstOrDefault();
        }

        #endregion
    }

    static class Utils
    {
        public static bool TryGetColByName(this List<GenTableColumnAttr> cols, string name, out GenTableColumnAttr col)
        {
            col = null;
            var find = cols.Where(p => p.ColumnName == name);
            if (find.Any())
            {
                col = find.First();
                return true;
            }
            return false;
        }
    }
}
