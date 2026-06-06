using System;
using SqlSugar;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using SqlSugar.DbConvert;
using Wes.Utils.Converter;
using System.Text.Json.Serialization;

namespace Wes.DbModel
{
    /// <summary>
    /// 实例节点表
    /// <summary>
    [SugarTable("flow_instance_node", "实例节点", IsDisabledUpdateAll = true)]
    public class FlowInstanceNodeModel
    {
        /// <summary>
        /// 主键id
        /// <summary>
        [SugarColumn(ColumnName = "instance_node_id", IsPrimaryKey = true)]
        [JsonConverter(typeof(LongToStringConverter))]
        public long InstanceNodeId { get; set; }

        /// <summary>
        /// 实例id
        /// <summary>
        [SugarColumn(ColumnName = "instance_id")]
        [JsonConverter(typeof(LongToStringConverter))]
        public long InstanceId { get; set; }

        /// <summary>
        /// 节点id
        /// <summary>
        [SugarColumn(ColumnName = "node_id")]
        public string NodeId { get; set; }

        /// <summary>
        /// 节点类型 start end task notice
        /// <summary>
        [JsonConverter(typeof(JsonStringEnumConverter))]
        [SugarColumn(ColumnName = "node_type", IsNullable = true, Length = 11, ColumnDescription = "节点类型 start end task notice", SqlParameterDbType = typeof(EnumToStringConvert))]
        public FlowNodeTypeEnum NodeType { get; set; }

        /// <summary>
        /// 前一节点id
        /// <summary>
        [SugarColumn(ColumnName = "pre_node_id", IsNullable = true, Length = 100, ColumnDescription = "前一节点id")]
        public string PreNodeId { get; set; }

        /// <summary>
        /// 节点名称
        /// <summary>
        [SugarColumn(ColumnName = "node_name", IsNullable = true, Length = 50, ColumnDescription = "节点名称")]
        public string NodeName { get; set; }

        /// <summary>
        /// 处理结果 pass通过  unpass不通过  pending挂起
        /// <summary>
        [JsonConverter(typeof(JsonStringEnumConverter))]
        [SugarColumn(ColumnName = "node_result", IsNullable = true, Length = 20, ColumnDescription = "处理结果 pass通过  unpass不通过  pending挂起", SqlParameterDbType = typeof(EnumToStringConvert))]
        public FlowStatusEnum NodeResult { get; set; }

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
        /// 所有节点
        /// </summary>
        [Navigate(NavigateType.OneToMany, nameof(InstanceNodeId), nameof(FlowInstanceTaskModel.InstanceNodeId))]
        public List<FlowInstanceTaskModel> Tasks { set; get; }
    }
}
