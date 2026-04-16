using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Wes.DbModel
{
    public enum NodeHandleByEnum
    {
        /// <summary>
        /// 流程发起人
        /// </summary>
        [EnumMember(Value = "author")]
        author,

        /// <summary>
        /// 部门领导
        /// </summary>
        [EnumMember(Value = "leader")]
        leader,

        /// <summary>
        /// 角色
        /// </summary>
        [EnumMember(Value = "role")]
        role,

        /// <summary>
        /// 部门
        /// </summary>
        [EnumMember(Value = "dept")]
        dept,

        /// <summary>
        /// 指定用户
        /// </summary>
        [EnumMember(Value = "user")]
        user,
    }
}
