using System;
using SqlSugar;
using System.Collections.Generic;
using System.Text;
using Org.BouncyCastle.Crypto;
using SqlSugar.DbConvert;
using Wes.Utils.JsonConverter;
using System.Text.Json.Serialization;

namespace Wes.DbModel
{
    /// <summary>
    /// 审批流实例
    /// <summary>
    [SugarTable("flow_instance", "审批流实例", IsDisabledUpdateAll = true)]
    public class FlowInstanceModel
    {
        /// <summary>
        /// 主键
        /// <summary>
        [SugarColumn(ColumnName = "instance_id", IsPrimaryKey = true, Length = 20, ColumnDescription = "主键")]
        [JsonConverter(typeof(LongToStringConverter))]
        public long InstanceId { get; set; }

        /// <summary>
        /// 流程id
        /// <summary>
        [SugarColumn(ColumnName = "process_id", Length = 20, ColumnDescription = "流程id")]
        [JsonConverter(typeof(LongToStringConverter))]
        public long ProcessId { get; set; }

        /// <summary>
        /// 流程版本
        /// <summary>
        [SugarColumn(ColumnName = "version_id", Length = 20, ColumnDescription = "流程版本")]
        [JsonConverter(typeof(LongToStringConverter))]
        public long VersionId { get; set; }

        /// <summary>
        /// 业务主键id
        /// <summary>
        [SugarColumn(ColumnName = "business_id", Length = 20, ColumnDescription = "业务主键id")]
        [JsonConverter(typeof(LongToStringConverter))]
        public long BusinessId { get; set; }

        /// <summary>
        /// 业务编码
        /// <summary>
        [SugarColumn(ColumnName = "business_code", IsNullable = true, Length = 100, ColumnDescription = "业务编码")]
        public string BusinessCode { get; set; }

        /// <summary>
        /// 当前节点
        /// <summary>
        [SugarColumn(ColumnName = "current_node_id", IsNullable = true, Length = 20, ColumnDescription = "当前节点")]
        [JsonConverter(typeof(LongToStringConverter))]
        public long CurrentNodeId { get; set; }

        /// <summary>
        /// 是否加急 1 加急 0整除
        /// <summary>
        [SugarColumn(ColumnName = "is_urgent", Length = 1, ColumnDescription = "是否加急 1 加急 0整除")]
        public int IsUrgent { get; set; }

        /// <summary>
        /// 审批状态
        /// <summary>
        [SugarColumn(ColumnName = "instance_status", Length = 20, ColumnDescription = "审批状态")]
        public FlowStatusEnum InstanceStatus { get; set; }

        /// <summary>
        /// 扩展信息
        /// <summary>
        [SugarColumn(ColumnName = "extend_info", IsNullable = true, Length = 1000, ColumnDescription = "扩展信息")]
        public string ExtendInfo { get; set; }

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
        /// 当前流程
        /// </summary>
        [Navigate(NavigateType.OneToOne, nameof(ProcessId))]
        public FlowProcessModel Process { set; get; }

        /// <summary>
        /// 当前版本
        /// </summary>
        [Navigate(NavigateType.OneToOne, nameof(VersionId))]
        public FlowProcessVersionModel Version { set; get; }

        /// <summary>
        /// 创建人
        /// </summary>
        [Navigate(NavigateType.OneToOne, nameof(CreateBy), nameof(SysUserModel.Account))]
        public SysUserModel CreateUser { set; get; }

        /// <summary>
        /// 当前节点
        /// </summary>
        [Navigate(NavigateType.OneToOne, nameof(CurrentNodeId))]
        public FlowInstanceNodeModel CurrentNode { set; get; }

        /// <summary>
        /// 所有节点
        /// </summary>
        [Navigate(NavigateType.OneToMany, nameof(InstanceId), nameof(FlowInstanceNodeModel.InstanceId))]
        public List<FlowInstanceNodeModel> Nodes { set; get; }
    }
}
