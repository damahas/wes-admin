using System;
using SqlSugar;
using System.Collections.Generic;
using System.Text;
using Wes.Utils.Converter;
using System.Text.Json.Serialization;

namespace Wes.DbModel
{
    /// <summary>
    /// 审批流定义
    /// <summary>
    [SugarTable("flow_process_version", "审批流定义", IsDisabledUpdateAll = true)]
    public class FlowProcessVersionModel
    {
        /// <summary>
        /// 主键id
        /// <summary>
        [SugarColumn(ColumnName = "version_id", IsPrimaryKey = true, Length = 20, ColumnDescription = "主键id")]
        [JsonConverter(typeof(LongToStringConverter))]
        public long VersionId { get; set; }

        /// <summary>
        /// 流程id
        /// <summary>
        [SugarColumn(ColumnName = "process_id", Length = 20, ColumnDescription = "流程id")]
        [JsonConverter(typeof(LongToStringConverter))]
        public long ProcessId { get; set; }

        /// <summary>
        /// 流程版本
        /// <summary>
        [SugarColumn(ColumnName = "version", Length = 50, ColumnDescription = "流程版本")]
        public string Version { get; set; }

        /// <summary>
        /// 流程内容
        /// <summary>
        [SugarColumn(ColumnName = "content", Length = 3000, ColumnDescription = "流程内容")]
        public string Content { get; set; }

        /// <summary>
        /// 是否锁定
        /// <summary>
        [SugarColumn(ColumnName = "is_lock", Length = 1, ColumnDescription = "是否锁定")]
        public int IsLock { get; set; }

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
        /// 当前版本
        /// </summary>
        [Navigate(NavigateType.OneToOne, nameof(ProcessId))]
        public FlowProcessModel Process { set; get; }

    }
}
