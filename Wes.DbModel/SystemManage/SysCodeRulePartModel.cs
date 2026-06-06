using System;
using SqlSugar;
using System.Collections.Generic;
using System.Text;
using Wes.Utils.Converter;
using System.Text.Json.Serialization;

namespace Wes.DbModel
{
    /// <summary>
    /// 序号生成规则片段
    /// <summary>
    [SugarTable("sys_code_rule_part", "序号生成规则片段", IsDisabledUpdateAll = true)]
    public class SysCodeRulePartModel
    {
        /// <summary>
        /// 编码片段规则
        /// <summary>
        [SugarColumn(ColumnName = "part_id", IsPrimaryKey = true, Length = 20, ColumnDescription = "编码片段规则")]
        [JsonConverter(typeof(LongToStringConverter))] 
        public long PartId { get; set; }

        /// <summary>
        /// 规则id
        /// <summary>
        [SugarColumn(ColumnName = "rule_id", Length = 20, ColumnDescription = "规则id")]
        [JsonConverter(typeof(LongToStringConverter))]
        public long RuleId { get; set; }

        /// <summary>
        /// 片段类型 string/calc/date
        /// <summary>
        [SugarColumn(ColumnName = "part_type", Length = 20, ColumnDescription = "片段类型 string/calc/date")]
        public string PartType { get; set; }

        /// <summary>
        /// 固定字符串/字符规则/日期格式
        /// <summary>
        [SugarColumn(ColumnName = "part_value", IsNullable = true, Length = 200, ColumnDescription = "固定字符串/字符规则/日期格式")]
        public string PartValue { get; set; }

        /// <summary>
        /// 重置类型 week/month/quarter/year
        /// <summary>
        [SugarColumn(ColumnName = "reset_type", IsNullable = true, Length = 255, ColumnDescription = "重置类型 week/month/quarter/year")]
        public string ResetType { get; set; }

        /// <summary>
        /// 周开始时间 0周日 1周一
        /// <summary>
        [SugarColumn(ColumnName = "week_start_day", Length = 2, ColumnDescription = "周开始时间 0周日 1周一")]
        public int WeekStartDay { get; set; }

        /// <summary>
        /// 下次重置日期
        /// <summary>
        [SugarColumn(ColumnName = "reset_time", IsNullable = true, ColumnDescription = "下次重置日期")]
        public DateTime? ResetTime { get; set; }

        /// <summary>
        /// 当前位置
        /// <summary>
        [SugarColumn(ColumnName = "current_index", IsNullable = true, Length = 800, ColumnDescription = "当前位置")]
        public string CurrentIndex { get; set; }

        /// <summary>
        /// 排序字段
        /// <summary>
        [SugarColumn(ColumnName = "sort", Length = 6, ColumnDescription = "排序字段")]
        public int Sort { get; set; }

        /// <summary>
        /// 删除标志（0代表存在 1代表删除）
        /// <summary>
        [SugarColumn(ColumnName = "is_del", Length = 1, ColumnDescription = "删除标志（0代表存在 1代表删除）")]
        public int IsDel { get; set; }

        /// <summary>
        /// 备注
        /// <summary>
        [SugarColumn(ColumnName = "remark", IsNullable = true, Length = 200, ColumnDescription = "备注")]
        public string Remark { get; set; }

        /// <summary>
        /// 创建者
        /// <summary>
        [SugarColumn(ColumnName = "create_by", IsNullable = true, Length = 64, ColumnDescription = "创建者")]
        public string CreateBy { get; set; }

        /// <summary>
        /// 创建时间
        /// <summary>
        [SugarColumn(ColumnName = "create_time", IsNullable = true, ColumnDescription = "创建时间")]
        public DateTime? CreateTime { get; set; }

        /// <summary>
        /// 更新者
        /// <summary>
        [SugarColumn(ColumnName = "update_by", IsNullable = true, Length = 64, ColumnDescription = "更新者")]
        public string UpdateBy { get; set; }

        /// <summary>
        /// 更新时间
        /// <summary>
        [SugarColumn(ColumnName = "update_time", IsNullable = true, ColumnDescription = "更新时间")]
        public DateTime? UpdateTime { get; set; }

        /// <summary>
        /// 是否跳过0
        /// <summary>
        [SugarColumn(ColumnName = "is_skip_zero", Length = 2, ColumnDescription = "是否跳过0")]
        public int IsSkipZero{ get; set; }

    }
}
