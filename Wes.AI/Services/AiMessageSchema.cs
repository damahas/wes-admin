using SqlSugar;
using System;
using System.Linq;

namespace Wes.AI.Services;

/// <summary>
/// sys_ai_message 表结构补齐。
///
/// 启动时的 CodeFirst.InitTables 只会创建不存在的表，不会给已存在的表补列，
/// 因此后续新增的统计列（耗时 / 真实 token）需要在这里补，避免老库升级时手工执行 DDL。
/// </summary>
public static class AiMessageSchema
{
    private const string Table = "sys_ai_message";

    public static void EnsureColumns(ISqlSugarClient db)
    {
        try
        {
            if (!db.DbMaintenance.IsAnyTable(Table, false)) return;

            var cols = db.DbMaintenance.GetColumnInfosByTableName(Table, false);
            static bool Same(string? a, string b) => string.Equals(a, b, StringComparison.OrdinalIgnoreCase);
            bool Has(string name) => cols.Any(c => Same(c.DbColumnName, name));

            if (!Has("elapsed_ms"))
                db.DbMaintenance.AddColumn(Table, new DbColumnInfo
                {
                    DbColumnName = "elapsed_ms",
                    DataType = "int",
                    IsNullable = true,
                    ColumnDescription = "回答耗时（毫秒，服务端实测）"
                });

            if (!Has("tokens_estimated"))
                db.DbMaintenance.AddColumn(Table, new DbColumnInfo
                {
                    DbColumnName = "tokens_estimated",
                    DataType = "tinyint(1)",
                    IsNullable = true,
                    ColumnDescription = "token是否为估算值（1=估算 0=真实）"
                });
        }
        catch
        {
            // 表结构补齐失败不阻断启动（例如账号无 DDL 权限），此时统计列缺失仅影响展示
        }
    }
}
