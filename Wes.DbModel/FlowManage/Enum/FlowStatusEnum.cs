using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Wes.DbModel
{
    public enum FlowStatusEnum
    {
        /// <summary>
        /// 开始
        /// </summary>
        [EnumMember(Value = "start")]
        start = 0,
        /// <summary>
        /// 审批中，只有flow_instance主表使用
        /// </summary>
        [EnumMember(Value = "doing")]
        doing = 10,
        /// <summary>
        /// 通过
        /// </summary>
        [EnumMember(Value = "pass")]
        pass = 100,
        /// <summary>
        /// 不通过
        /// </summary>
        [EnumMember(Value = "unpass")] 
        unpass = 101,
        /// <summary>
        /// 挂起
        /// </summary>
        [EnumMember(Value = "pending")]
        pending = 200,
        /// <summary>
        /// 委托
        /// </summary>
        [EnumMember(Value = "delegation")]
        delegation = 201,
        /// <summary>
        /// 系统自动处理
        /// </summary>
        [EnumMember(Value = "auto")] 
        auto = 9999,
    }
}
