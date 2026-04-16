using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using SqlSugar;
using Wes.DbModel;

namespace Wes.ViewModel.FlowManage
{
    public class FlowRunModel
    {
        /// <summary>
        /// 流程编码
        /// <summary>
        public string ProcessCode { get; set; }

        /// <summary>
        /// 是否加急 1 加急 0整除
        /// <summary>
        public int IsUrgent { get; set; }

        /// <summary>
        /// 是否抓回 1抓回
        /// <summary>
        public int IsRecall { get; set; }

        /// <summary>
        /// 业务主键id
        /// <summary>
        public long BusinessId { get; set; }

        /// <summary>
        /// 业务编码
        /// <summary>
        public string BusinessCode { get; set; }

        /// <summary>
        /// 处理结果 pass通过  unpass不通过  pending挂起  delegate委托
        /// <summary>
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public FlowStatusEnum TaskResult { get; set; }

        /// <summary>
        /// 审批意见
        /// <summary>
        public string Comments { get; set; }

        /// <summary>
        /// 扩展信息
        /// <summary>
        public string ExtendInfo { get; set; }

        /// <summary>
        /// 实例状态 start：发起  doing：审批中  pass，通过  unpass，不通过  pending，挂起
        /// <summary>
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public FlowStatusEnum? InstanceStatus { get; set; }

        /// <summary>
        /// 任务id
        /// <summary>
        public long InstanceTaskId { get; set; }

        /// <summary>
        /// 节点信息
        /// </summary>
        public FlowInstanceNodeModel Node { set; get; }

        /// <summary>
        /// 选择人员
        /// </summary>
        public FlowSelectUserModel SelectUsers { set; get; } 

        /// <summary>
        /// 已选择用户节点，这里可能会有多个
        /// </summary>
        public Dictionary<string, long> SelectedUser { set; get; }
    }
}

