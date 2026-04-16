using System;
using System.Collections.Generic;
using System.Text;

namespace Wes.Utils.CodeGenerator
{
    public class MysqlDataTypeMapping
    {
        public static string DbTypeMapping(string dbType, bool isNull)
        {
            switch (dbType.ToLower())
            {
                case "char":
                case "varchar":
                case "text":
                case "longtext":
                case "mediumtext":
                case "tinytext": return "string";
                case "bigint": return "long" + (!isNull ? "" : "?");
                case "date":
                case "time":
                case "timestamp":
                case "datetime": return "DateTime" + (!isNull ? "" : "?");
                case "tinyint":
                case "int": return "int" + (!isNull ? "" : "?");
                case "smallint": return "short" + (!isNull ? "" : "?");
                case "decimal": return "decimal" + (!isNull ? "" : "?");
                case "double": return "double" + (!isNull ? "" : "?");
                case "float": return "float" + (!isNull ? "" : "?");
                case "bit": return "bool" + (!isNull ? "" : "?");
                case "blob":
                case "longblob":
                case "varbinary":
                case "mediumblob": return "byte[]";
                default: return "";
            }
        }
    }
}
