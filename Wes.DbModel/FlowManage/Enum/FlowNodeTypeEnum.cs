using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

namespace Wes.DbModel
{
    public enum FlowNodeTypeEnum
    {
        /// <summary>
        /// 开始节点
        /// </summary>
        [EnumMember(Value = "start")]
        start,
        /// <summary>
        /// 结束节点
        /// </summary>
        [EnumMember(Value = "end")]
        end,
        /// <summary>
        /// 处理节点
        /// </summary>
        [EnumMember(Value = "task")]
        task,
        /// <summary>
        /// 通知节点
        /// </summary>
        [EnumMember(Value = "notice")] 
        notice
    }
}
