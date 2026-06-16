using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Wes.DbModel
{
    public enum NodeHandleTypeEnum
    {
        /// <summary>
        /// 一个人同意
        /// </summary>
        [EnumMember(Value = "one")]
        one,

        /// <summary>
        /// 所有人同意
        /// </summary>
        [EnumMember(Value = "all")]
        all,

        /// <summary>
        /// 上一节点最后处理人选择
        /// </summary>
        [EnumMember(Value = "select")] 
        select
    }
}
