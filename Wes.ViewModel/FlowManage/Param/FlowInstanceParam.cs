using SqlSugar.DbConvert;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Wes.DbModel;

namespace Wes.ViewModel.FlowManage
{
    public class FlowInstanceParam
    {
        /// <summary>
        /// 业务编码
        /// </summary>
        public string BusinessCode { get; set; }

        /// <summary>
        /// 流程编码
        /// </summary>
        public string ProcessCode { get; set; }

        /// <summary>
        /// 流程名称
        /// </summary>
        public string ProcessName { get; set; }

        /// <summary>
        /// 当前节点
        /// <summary>
        public string CurrentNodeName { get; set; }

        /// <summary>
        /// 是否加急 1 加急 0整除
        /// <summary>
        public int? IsUrgent { get; set; }

        /// <summary>
        /// 状态 start：发起  doing：审批中  pass，通过  unpass，不通过  pending，挂起
        /// <summary>
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public FlowStatusEnum? InstanceStatus { get; set; }
    }
}
