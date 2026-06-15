using System.Collections.Generic;

namespace Wes.Utils.Integration
{
    /// <summary>
    /// 统一部门数据模型。
    /// 各三方平台（钉钉/企微/飞书）的部门数据均映射为此格式，由调用方决定如何持久化。
    /// </summary>
    public class SyncDeptData
    {
        /// <summary>三方平台部门唯一标识</summary>
        public string ThirdPartyDeptId { get; set; }

        /// <summary>三方平台父部门唯一标识（"0" 表示根）</summary>
        public string ThirdPartyParentId { get; set; }

        /// <summary>部门名称</summary>
        public string DeptName { get; set; }

        /// <summary>排序号</summary>
        public int OrderNum { get; set; }

        /// <summary>原始 JSON 数据（保留三方平台完整返回，方便扩展溯源）</summary>
        public string RawData { get; set; }
    }

    /// <summary>
    /// 统一用户数据模型。
    /// 各三方平台的用户数据均映射为此格式，由调用方决定如何持久化。
    /// </summary>
    public class SyncUserData
    {
        /// <summary>三方平台用户唯一标识</summary>
        public string ThirdPartyUserId { get; set; }

        /// <summary>三方平台所属部门 ID 列表（用户可能属于多个部门）</summary>
        public List<string> ThirdPartyDeptIds { get; set; } = new List<string>();

        /// <summary>用户名称</summary>
        public string UserName { get; set; }

        /// <summary>用户账号（工号）</summary>
        public string Account { get; set; }

        /// <summary>邮箱</summary>
        public string Email { get; set; }

        /// <summary>手机号</summary>
        public string Mobile { get; set; }

        /// <summary>头像地址</summary>
        public string Avatar { get; set; }

        /// <summary>职位</summary>
        public string Title { get; set; }

        /// <summary>工号</summary>
        public string JobNumber { get; set; }

        /// <summary>原始 JSON 数据（保留三方平台完整返回）</summary>
        public string RawData { get; set; }
    }
}
