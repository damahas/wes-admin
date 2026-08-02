using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using SqlSugar;

namespace Wes.AI.Plugins;

/// <summary>
/// SQL 助手插件：生成并执行只读 SQL，支持查询表结构。
/// 仅允许 SELECT / WITH 查询，禁止写操作与 DDL，避免破坏数据。
/// 插件自身无状态，每次调用时在方法内创建 DI Scope 解析 Scoped 的 ISqlSugarClient，
/// 以便安全地注册到单例缓存的 Kernel 中。
/// </summary>
public class SqlPlugin
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<SqlPlugin> _logger;

    public SqlPlugin(IServiceProvider serviceProvider, ILogger<SqlPlugin> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    /// <summary>
    /// 执行只读 SQL 查询并返回结果（用于回答用户关于数据库数据的提问）
    /// </summary>
    [KernelFunction("execute_sql")]
    [Description("执行一条只读 SQL 查询（仅允许 SELECT 或 WITH 语句），返回查询结果（表格形式）。" +
                  "用于回答用户关于数据库数据的提问，例如统计用户数、查询列表等。禁止执行 INSERT/UPDATE/DELETE/DROP 等写操作。")]
    public async Task<string> ExecuteSqlAsync(
        [Description("要执行的 SELECT 或 WITH 查询语句")] string sql)
    {
        if (string.IsNullOrWhiteSpace(sql))
            return "错误：SQL 为空。";

        if (!IsReadOnly(sql))
            return "错误：仅允许执行只读查询（SELECT / WITH），已拒绝写操作或 DDL。";

        _logger.LogInformation("SqlPlugin.ExecuteSql: {Sql}", sql);

        try
        {
            using var scope = _serviceProvider.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ISqlSugarClient>();
            var dt = await db.Ado.GetDataTableAsync(sql);
            return FormatDataTable(dt);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "SqlPlugin.ExecuteSql failed: {Sql}", sql);
            return $"执行 SQL 出错：{ex.Message}";
        }
    }

    /// <summary>
    /// 获取指定表的结构（列名与类型），帮助模型生成正确的 SQL
    /// </summary>
    [KernelFunction("get_table_schema")]
    [Description("查询指定数据库表的列结构（列名、类型、是否可空），帮助生成正确的 SQL。若不确定表名，可先查看系统提示中给出的可用表列表。")]
    public async Task<string> GetTableSchemaAsync(
        [Description("表名")] string tableName)
    {
        if (string.IsNullOrWhiteSpace(tableName))
            return "错误：表名为空。";

        try
        {
            using var scope = _serviceProvider.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ISqlSugarClient>();
            var cols = await Task.Run(() => db.DbMaintenance.GetColumnInfosByTableName(tableName, false));
            if (cols == null || cols.Count == 0)
                return $"未找到表 {tableName} 的结构信息（表可能不存在，请核对可用表列表）。";

            var sb = new StringBuilder();
            sb.AppendLine($"表 {tableName} 结构：");
            foreach (var c in cols)
                sb.AppendLine($"- {c.DbColumnName} {c.DataType}{(c.IsNullable ? " NULL" : " NOT NULL")}");
            return sb.ToString();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "SqlPlugin.GetTableSchema failed: {Table}", tableName);
            return $"查询表结构出错：{ex.Message}";
        }
    }

    /// <summary>
    /// 仅允许以 SELECT / WITH 开头的只读语句
    /// </summary>
    private static bool IsReadOnly(string sql)
    {
        var s = sql.Trim().TrimEnd(';').Trim();
        var first = s.Split(new[] { ' ', '\t', '\n', '\r', '(' }, StringSplitOptions.RemoveEmptyEntries)
            .FirstOrDefault()?.ToLowerInvariant();
        return first is "select" or "with";
    }

    private static string FormatDataTable(DataTable dt)
    {
        if (dt.Rows.Count == 0)
            return "查询成功，但没有返回任何行。";

        const int maxRows = 50;
        var sb = new StringBuilder();
        sb.AppendLine($"查询返回 {dt.Rows.Count} 行（最多展示 {maxRows} 行）：");
        var headers = dt.Columns.Cast<DataColumn>().Select(c => c.ColumnName).ToArray();
        sb.AppendLine(string.Join(" | ", headers));
        var rows = Math.Min(dt.Rows.Count, maxRows);
        for (var i = 0; i < rows; i++)
        {
            var vals = dt.Rows[i].ItemArray.Select(v => v?.ToString() ?? "NULL");
            sb.AppendLine(string.Join(" | ", vals));
        }
        return sb.ToString();
    }
}
