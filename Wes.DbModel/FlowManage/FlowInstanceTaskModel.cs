using System;
using SqlSugar;
using System.Collections.Generic;
using System.Text;
using SqlSugar.DbConvert;
using Wes.Utils.Converter;
using System.Text.Json.Serialization;

namespace Wes.DbModel
{
    /// <summary>
    /// 节点任务表
    /// <summary>
    [SugarTable("flow_instance_task", "节点任务", IsDisabledUpdateAll = true)]
    public class FlowInstanceTaskModel
    {
        /// <summary>
        /// 主键id
        /// <summary>
        [SugarColumn(ColumnName = "instance_task_id", IsPrimaryKey = true, Length = 20, ColumnDescription = "主键id")]
        [JsonConverter(typeof(LongToStringConverter))]
        public long InstanceTaskId { get; set; }

        /// <summary>
        /// 实例id
        /// <summary>
        [SugarColumn(ColumnName = "instance_id", Length = 20, ColumnDescription = "实例id")]
        [JsonConverter(typeof(LongToStringConverter))]
        public long InstanceId { get; set; }

        /// <summary>
        /// 节点id
        /// <summary>
        [SugarColumn(ColumnName = "instance_node_id", Length = 20, ColumnDescription = "节点id")]
        [JsonConverter(typeof(LongToStringConverter))]
        public long InstanceNodeId { get; set; }

        /// <summary>
        /// 任务处理人
        /// <summary>
        [SugarColumn(ColumnName = "task_user_id", Length = 20, ColumnDescription = "任务处理人")]
        [JsonConverter(typeof(LongToStringConverter))]
        public long TaskUserId { get; set; }

        /// <summary>
        /// 实际处理人
        /// <summary>
        [SugarColumn(ColumnName = "actual_user_id", Length = 20, ColumnDescription = "实际处理人")]
        [JsonConverter(typeof(LongToStringConverter))]
        public long ActualUserId { get; set; }

        /// <summary>
        /// 处理结果 pass通过  unpass不通过  pending挂起  delegate委托
        /// <summary>
        [JsonConverter(typeof(JsonStringEnumConverter))]
        [SugarColumn(ColumnName = "task_result", Length = 20, ColumnDescription = "处理结果 pass通过  unpass不通过  pending挂起  delegate委托", SqlParameterDbType = typeof(EnumToStringConvert))]
        public FlowStatusEnum TaskResult { get; set; }

        /// <summary>
        /// 审批意见
        /// <summary>
        [SugarColumn(ColumnName = "comments", IsNullable = true, Length = 800, ColumnDescription = "审批意见")]
        public string Comments { get; set; }

        /// <summary>
        /// 是否抓回 1抓回
        /// <summary>
        [SugarColumn(ColumnName = "is_recall", Length = 1, ColumnDescription = "是否抓回 1抓回")]
        public int IsRecall { get; set; }

        /// <summary>
        /// 委托人员（用于抓回）
        /// <summary>
        [SugarColumn(ColumnName = "recall_task_id", IsNullable = true, Length = 20, ColumnDescription = "委托人员（用于抓回）")]
        [JsonConverter(typeof(LongToStringConverter))]
        public long? RecallTaskId { get; set; }

        /// <summary>
        /// 删除标志（0代表存在 1代表删除）
        /// <summary>
        [SugarColumn(ColumnName = "is_del", Length = 1, ColumnDescription = "删除标志（0代表存在 1代表删除）")]
        public int IsDel { get; set; }

        /// <summary>
        /// 处理时间
        /// <summary>
        [SugarColumn(ColumnName = "handle_time", IsNullable = true, ColumnDescription = "处理时间")]
        public DateTime? HandleTime { get; set; }

        /// <summary>
        /// 创建时间
        /// <summary>
        [SugarColumn(ColumnName = "create_time", IsNullable = true, ColumnDescription = "创建时间")]
        public DateTime? CreateTime { get; set; }

        /// <summary>
        /// 实际处理人
        /// </summary>
        [Navigate(NavigateType.OneToOne, nameof(ActualUserId))]
        public SysUserModel ActualUser { set; get; }
    }
}
