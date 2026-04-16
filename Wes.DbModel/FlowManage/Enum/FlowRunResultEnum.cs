using System;
using System.Runtime.Serialization;

namespace Wes.DbModel
{
    public enum FlowRunResultEnum
    {
        /// <summary>
        /// 成功
        /// </summary>
        [EnumMember(Value = "success")]
        success,
        /// <summary>
        /// 失败
        /// </summary>
        [EnumMember(Value = "error")]
        error,
        /// <summary>
        /// 选择人
        /// </summary>
        [EnumMember(Value = "select")]
        select,
        /// <summary>
        /// 继续
        /// </summary>
        [EnumMember(Value = "next")]
        next,
    }
}

