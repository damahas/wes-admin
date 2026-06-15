namespace Wes.Utils.Integration
{
    /// <summary>
    /// 三方用户信息（各平台统一模型，用于扫码登录）
    /// </summary>
    public class ThirdPartyUserInfo
    {
        /// <summary>三方平台用户唯一标识</summary>
        public string ThirdPartyUserId { get; set; }

        /// <summary>三方平台部门唯一标识</summary>
        public string ThirdPartyDeptId { get; set; }

        /// <summary>用户名称</summary>
        public string Name { get; set; }

        /// <summary>用户邮箱</summary>
        public string Email { get; set; }

        /// <summary>手机号</summary>
        public string Mobile { get; set; }

        /// <summary>头像地址</summary>
        public string Avatar { get; set; }

        /// <summary>职位</summary>
        public string Title { get; set; }

        /// <summary>原始数据（JSON，方便扩展）</summary>
        public string RawData { get; set; }
    }

    /// <summary>
    /// 三方部门信息（各平台统一模型，用于扫码登录回调等场景）
    /// </summary>
    public class ThirdPartyDeptInfo
    {
        /// <summary>三方平台部门唯一标识</summary>
        public string ThirdPartyDeptId { get; set; }

        /// <summary>三方平台父部门唯一标识</summary>
        public string ThirdPartyParentId { get; set; }

        /// <summary>部门名称</summary>
        public string Name { get; set; }

        /// <summary>排序</summary>
        public int Order { get; set; }

        /// <summary>原始数据（JSON）</summary>
        public string RawData { get; set; }
    }
}
