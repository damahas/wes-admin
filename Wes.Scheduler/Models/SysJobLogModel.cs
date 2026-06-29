using SqlSugar;
using Wes.Utils.Converter;
using System.Text.Json.Serialization;

namespace Wes.DbModel
{
    /// <summary>
    /// 定时任务调度日志表
    /// </summary>
    [SugarTable("sys_job_log", "定时任务调度日志表", IsDisabledUpdateAll = true)]
    public class SysJobLogModel
    {
        [SugarColumn(ColumnName = "job_log_id", IsPrimaryKey = true, Length = 20, ColumnDescription = "任务日志ID")]
        [JsonConverter(typeof(LongToStringConverter))]
        public long JobLogId { get; set; }

        [SugarColumn(ColumnName = "job_name", Length = 64, ColumnDescription = "任务名称")]
        public string JobName { get; set; } = string.Empty;

        [SugarColumn(ColumnName = "job_group", Length = 64, ColumnDescription = "任务组名")]
        public string JobGroup { get; set; } = string.Empty;

        [SugarColumn(ColumnName = "invoke_target", Length = 500, ColumnDescription = "调用目标字符串")]
        public string InvokeTarget { get; set; } = string.Empty;

        [SugarColumn(ColumnName = "job_message", Length = 500, IsNullable = true, ColumnDescription = "日志信息")]
        public string? JobMessage { get; set; }

        [SugarColumn(ColumnName = "status", Length = 1, ColumnDescription = "执行状态（0正常 1失败）")]
        public string Status { get; set; } = "0";

        [SugarColumn(ColumnName = "exception_info", Length = 2000, IsNullable = true, ColumnDescription = "异常信息")]
        public string? ExceptionInfo { get; set; }

        [SugarColumn(ColumnName = "elapsed_time", ColumnDescription = "执行耗时（毫秒）")]
        public long ElapsedTime { get; set; }

        [SugarColumn(ColumnName = "create_time", IsNullable = true, ColumnDescription = "创建时间")]
        public DateTime? CreateTime { get; set; }
    }
}
