using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Wes.Utils.Integration
{
    #region Token

    /// <summary>企业微信 AccessToken 响应</summary>
    public class WeComTokenResult
    {
        [JsonProperty("errcode")]
        public int ErrCode { get; set; }

        [JsonProperty("errmsg")]
        public string ErrMsg { get; set; }

        [JsonProperty("access_token")]
        public string AccessToken { get; set; }

        [JsonProperty("expires_in")]
        public int ExpiresIn { get; set; }

        public bool IsSuccess => ErrCode == 0;
    }

    #endregion

    #region 部门

    /// <summary>企业微信部门列表响应</summary>
    public class WeComDeptListResult
    {
        [JsonProperty("errcode")]
        public int ErrCode { get; set; }

        [JsonProperty("errmsg")]
        public string ErrMsg { get; set; }

        [JsonProperty("department")]
        public List<WeComDeptItem> Department { get; set; }

        public bool IsSuccess => ErrCode == 0;
    }

    public class WeComDeptItem
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("parentid")]
        public long ParentId { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("order")]
        public long Order { get; set; }
    }

    #endregion

    #region 用户

    /// <summary>企业微信用户列表响应</summary>
    public class WeComUserListResult
    {
        [JsonProperty("errcode")]
        public int ErrCode { get; set; }

        [JsonProperty("errmsg")]
        public string ErrMsg { get; set; }

        [JsonProperty("userlist")]
        public List<WeComUserItem> UserList { get; set; }

        public bool IsSuccess => ErrCode == 0;
    }

    public class WeComUserItem
    {
        [JsonProperty("userid")]
        public string UserId { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("department")]
        public List<long> Department { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("mobile")]
        public string Mobile { get; set; }

        [JsonProperty("avatar")]
        public string Avatar { get; set; }

        [JsonProperty("position")]
        public string Position { get; set; }

        [JsonProperty("job_number")]
        public string JobNumber { get; set; }
    }

    /// <summary>
    /// 用户详情响应（企微 /user/get 接口返回根节点即用户属性，需手动绑定）
    /// </summary>
    public class WeComUserDetailResult
    {
        public int ErrCode { get; set; }
        public string ErrMsg { get; set; }
        public bool IsSuccess => ErrCode == 0;

        public WeComUserItem User { get; private set; }

        public static WeComUserDetailResult FromJson(string json)
        {
            var jObj = JObject.Parse(json);
            return new WeComUserDetailResult
            {
                ErrCode = jObj["errcode"]?.ToObject<int>() ?? -1,
                ErrMsg = jObj["errmsg"]?.ToString() ?? "unknown",
                User = new WeComUserItem
                {
                    UserId = jObj["userid"]?.ToString(),
                    Name = jObj["name"]?.ToString(),
                    Department = jObj["department"]?.ToObject<List<long>>() ?? new List<long>(),
                    Email = jObj["email"]?.ToString(),
                    Mobile = jObj["mobile"]?.ToString(),
                    Avatar = jObj["avatar"]?.ToString(),
                    Position = jObj["position"]?.ToString(),
                    JobNumber = jObj["job_number"]?.ToString()
                }
            };
        }
    }

    #endregion

    #region 扫码登录

    /// <summary>企业微信扫码登录：通过 code 获取用户标识</summary>
    public class WeComLoginResult
    {
        [JsonProperty("errcode")]
        public int ErrCode { get; set; }

        [JsonProperty("errmsg")]
        public string ErrMsg { get; set; }

        /// <summary>企业微信 userId</summary>
        [JsonProperty("UserId")]
        public string UserId { get; set; }

        /// <summary>用户票据（非敏感信息）</summary>
        [JsonProperty("user_ticket")]
        public string UserTicket { get; set; }

        public bool IsSuccess => ErrCode == 0;
    }

    #endregion
}
