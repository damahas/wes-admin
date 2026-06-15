using System.Collections.Generic;
using Newtonsoft.Json;

namespace Wes.Utils.Integration
{
    /// <summary>
    /// 飞书 tenant_access_token 响应
    /// </summary>
    public class FeishuTokenResult
    {
        [JsonProperty("code")]
        public int Code { get; set; }

        [JsonProperty("msg")]
        public string Msg { get; set; }

        [JsonProperty("tenant_access_token")]
        public string TenantAccessToken { get; set; }

        [JsonProperty("expire")]
        public int Expire { get; set; }

        public bool IsSuccess => Code == 0;
    }

    /// <summary>
    /// 飞书分页列表通用响应结构
    /// </summary>
    public class FeishuPageData<T>
    {
        [JsonProperty("has_more")]
        public bool HasMore { get; set; }

        [JsonProperty("page_token")]
        public string PageToken { get; set; }

        [JsonProperty("items")]
        public List<T> Items { get; set; }
    }

    public class FeishuApiResponse<T>
    {
        [JsonProperty("code")]
        public int Code { get; set; }

        [JsonProperty("msg")]
        public string Msg { get; set; }

        [JsonProperty("data")]
        public T Data { get; set; }

        public bool IsSuccess => Code == 0;
    }

    /// <summary>
    /// 飞书部门数据
    /// </summary>
    public class FeishuDeptItem
    {
        /// <summary>部门名称</summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>部门 OpenId</summary>
        [JsonProperty("department_id")]
        public string DepartmentId { get; set; }

        /// <summary>父部门 OpenId</summary>
        [JsonProperty("parent_department_id")]
        public string ParentDepartmentId { get; set; }

        /// <summary>排序号</summary>
        [JsonProperty("order")]
        public string Order { get; set; }
    }

    /// <summary>
    /// 飞书用户数据
    /// </summary>
    public class FeishuUserItem
    {
        /// <summary>用户 OpenId（同一应用下唯一）</summary>
        [JsonProperty("open_id")]
        public string OpenId { get; set; }

        /// <summary>用户 UnionId（同一租户下唯一）</summary>
        [JsonProperty("union_id")]
        public string UnionId { get; set; }

        /// <summary>用户姓名</summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>邮箱</summary>
        [JsonProperty("email")]
        public string Email { get; set; }

        /// <summary>手机号</summary>
        [JsonProperty("mobile")]
        public string Mobile { get; set; }

        /// <summary>头像</summary>
        [JsonProperty("avatar_url")]
        public string AvatarUrl { get; set; }

        /// <summary>工号</summary>
        [JsonProperty("employee_no")]
        public string EmployeeNo { get; set; }

        /// <summary>职位</summary>
        [JsonProperty("job_title")]
        public string JobTitle { get; set; }

        /// <summary>所属部门 OpenId 列表</summary>
        [JsonProperty("department_ids")]
        public List<string> DepartmentIds { get; set; }
    }

    /// <summary>
    /// 飞书扫码登录：通过 code 获取登录用户信息
    /// </summary>
    public class FeishuLoginAccessTokenResult
    {
        [JsonProperty("code")]
        public int Code { get; set; }

        [JsonProperty("msg")]
        public string Msg { get; set; }

        [JsonProperty("data")]
        public FeishuLoginAccessTokenData Data { get; set; }

        public bool IsSuccess => Code == 0;
    }

    public class FeishuLoginAccessTokenData
    {
        /// <summary>登录用户的 access_token</summary>
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }
    }

    public class FeishuLoginUserInfoResult
    {
        [JsonProperty("code")]
        public int Code { get; set; }

        [JsonProperty("msg")]
        public string Msg { get; set; }

        [JsonProperty("data")]
        public FeishuLoginUserInfoData Data { get; set; }

        public bool IsSuccess => Code == 0;
    }

    public class FeishuLoginUserInfoData
    {
        /// <summary>用户 OpenId</summary>
        [JsonProperty("open_id")]
        public string OpenId { get; set; }

        /// <summary>用户 UnionId</summary>
        [JsonProperty("union_id")]
        public string UnionId { get; set; }

        /// <summary>用户姓名</summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>邮箱</summary>
        [JsonProperty("email")]
        public string Email { get; set; }

        /// <summary>手机号</summary>
        [JsonProperty("mobile")]
        public string Mobile { get; set; }
    }
}
