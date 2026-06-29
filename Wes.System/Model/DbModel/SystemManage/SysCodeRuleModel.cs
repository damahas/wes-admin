using System;
using SqlSugar;
using System.Collections.Generic;
using System.Text;
using Wes.Utils.Converter;
using System.Text.Json.Serialization;
using Wes.Utils;

namespace Wes.DbModel
{
    /// <summary>
    /// 序号生成规则
    /// <summary>
    [SugarTable("sys_code_rule", "序号生成规则", IsDisabledUpdateAll = true)]
    public class SysCodeRuleModel
    {
        /// <summary>
        /// 规则id
        /// <summary>
        [SugarColumn(ColumnName = "rule_id", IsPrimaryKey = true, Length = 20, ColumnDescription = "规则id")]
        [JsonConverter(typeof(LongToStringConverter))]
        public long RuleId { get; set; }

        /// <summary>
        /// 规则编码
        /// <summary>
        [SugarColumn(ColumnName = "rule_code", Length = 50, ColumnDescription = "规则编码")]
        public string RuleCode { get; set; }

        /// <summary>
        /// 规则名称
        /// <summary>
        [SugarColumn(ColumnName = "rule_name", Length = 50, ColumnDescription = "规则名称")]
        public string RuleName { get; set; }

        /// <summary>
        /// 规则类型
        /// <summary>
        [SugarColumn(ColumnName = "rule_type", IsNullable = true, Length = 20, ColumnDescription = "规则类型")]
        public string RuleType { get; set; }

        /// <summary>
        /// 状态（0正常 1停用）
        /// <summary>
        [SugarColumn(ColumnName = "status", Length = 1, ColumnDescription = "状态（0正常 1停用）")]
        public string Status { get; set; }

        /// <summary>
        /// 删除标志（0代表存在 1代表删除）
        /// <summary>
        [SugarColumn(ColumnName = "is_del", IsNullable = true, Length = 1, ColumnDescription = "删除标志（0代表存在 1代表删除）")]
        public int? IsDel { get; set; }

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

    }
}
