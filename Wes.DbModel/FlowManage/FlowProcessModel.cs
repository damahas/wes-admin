using System;
using SqlSugar;
using System.Collections.Generic;
using System.Text;
using Wes.Utils.Converter;
using System.Text.Json.Serialization;

namespace Wes.DbModel
{
    /// <summary>
    /// 审批流
    /// <summary>
    [SugarTable("flow_process", "审批流", IsDisabledUpdateAll = true)]
    public class FlowProcessModel
    {
        /// <summary>
        /// 主键
        /// <summary>
        [SugarColumn(ColumnName = "process_id", IsPrimaryKey = true, Length = 20, ColumnDescription = "主键")]
        [JsonConverter(typeof(LongToStringConverter))]
        public long ProcessId { get; set; }

        /// <summary>
        /// 流程编码
        /// <summary>
        [SugarColumn(ColumnName = "process_code", Length = 100, ColumnDescription = "流程编码")]
        public string ProcessCode { get; set; }

        /// <summary>
        /// 流程名称
        /// <summary>
        [SugarColumn(ColumnName = "process_name", Length = 100, ColumnDescription = "流程名称")]
        public string ProcessName { get; set; }

        /// <summary>
        /// 父级id
        /// <summary>
        [SugarColumn(ColumnName = "parent_id", Length = 20, ColumnDescription = "父级id")]
        [JsonConverter(typeof(LongToStringConverter))]
        public long ParentId { get; set; }

        /// <summary>
        /// 当前流程图
        /// <summary>
        [SugarColumn(ColumnName = "cur_version_id", Length = 20, ColumnDescription = "当前流程图")]
        [JsonConverter(typeof(LongToStringConverter))]
        public long CurVersionId { get; set; }

        /// <summary>
        /// 业务模块
        /// <summary>
        [SugarColumn(ColumnName = "business_field", Length = 100, ColumnDescription = "业务模块")]
        public string BusinessField { get; set; }

        /// <summary>
        /// 描述
        /// <summary>
        [SugarColumn(ColumnName = "remark", IsNullable = true, Length = 50, ColumnDescription = "描述")]
        public string Remark { get; set; }

        /// <summary>
        /// 删除标志（0代表存在 1代表删除）
        /// <summary>
        [SugarColumn(ColumnName = "is_del", Length = 1, ColumnDescription = "删除标志（0代表存在 1代表删除）")]
        public int IsDel { get; set; }

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
        /// 页面地址{id}占位符
        /// <summary>
        [SugarColumn(ColumnName = "form_url", IsNullable = true, Length = 500, ColumnDescription = "页面地址{id}占位符")]
        public string FormUrl { get; set; }

        /// <summary>
        /// 回调地址
        /// <summary>
        [SugarColumn(ColumnName = "back_url", IsNullable = true, Length = 500, ColumnDescription = "回调地址")]
        public string BackUrl { get; set; }

        /// <summary>
        /// 当前版本
        /// </summary>
        [Navigate(NavigateType.OneToOne, nameof(CurVersionId))]
        public FlowProcessVersionModel Version { set; get; }

    }
}
