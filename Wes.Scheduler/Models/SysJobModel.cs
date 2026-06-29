using SqlSugar;
using Wes.Utils.Converter;
using System.Text.Json.Serialization;

namespace Wes.DbModel
{
    /// <summary>
    /// 定时任务调度表
    /// </summary>
    [SugarTable("sys_job", "定时任务调度表", IsDisabledUpdateAll = true)]
    public class SysJobModel
    {
        [SugarColumn(ColumnName = "job_id", IsPrimaryKey = true, Length = 20, ColumnDescription = "任务ID")]
        [JsonConverter(typeof(LongToStringConverter))]
        public long JobId { get; set; }

        [SugarColumn(ColumnName = "job_name", Length = 64, ColumnDescription = "任务名称")]
        public string JobName { get; set; } = string.Empty;

        [SugarColumn(ColumnName = "job_group", Length = 64, ColumnDescription = "任务组名")]
        public string JobGroup { get; set; } = "DEFAULT";

        [SugarColumn(ColumnName = "invoke_target", Length = 500, ColumnDescription = "调用目标字符串")]
        public string InvokeTarget { get; set; } = string.Empty;

        [SugarColumn(ColumnName = "cron_expression", Length = 255, IsNullable = true, ColumnDescription = "cron执行表达式")]
        public string? CronExpression { get; set; }

        [SugarColumn(ColumnName = "misfire_policy", Length = 20, ColumnDescription = "计划执行错误策略")]
        public string MisfirePolicy { get; set; } = "3";

        [SugarColumn(ColumnName = "concurrent", Length = 1, ColumnDescription = "是否并发执行（0允许 1禁止）")]
        public string Concurrent { get; set; } = "1";

        [SugarColumn(ColumnName = "status", Length = 1, ColumnDescription = "状态（0正常 1暂停）")]
        public string Status { get; set; } = "0";

        [SugarColumn(ColumnName = "create_by", IsNullable = true, Length = 64, ColumnDescription = "创建者")]
        public string? CreateBy { get; set; }

        [SugarColumn(ColumnName = "create_time", IsNullable = true, ColumnDescription = "创建时间")]
        public DateTime? CreateTime { get; set; }

        [SugarColumn(ColumnName = "update_by", IsNullable = true, Length = 64, ColumnDescription = "更新者")]
        public string? UpdateBy { get; set; }

        [SugarColumn(ColumnName = "update_time", IsNullable = true, ColumnDescription = "更新时间")]
        public DateTime? UpdateTime { get; set; }

        [SugarColumn(ColumnName = "remark", Length = 500, IsNullable = true, ColumnDescription = "备注信息")]
        public string? Remark { get; set; }
    }
}
