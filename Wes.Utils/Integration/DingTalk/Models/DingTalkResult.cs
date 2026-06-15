using System.Collections.Generic;
using Newtonsoft.Json;

namespace Wes.Utils.Integration
{
    /// <summary>钉钉 API 通用响应基类</summary>
    public abstract class DingTalkApiResult
    {
        [JsonProperty("errcode")]
        public int ErrCode { get; set; }

        [JsonProperty("errmsg")]
        public string ErrMsg { get; set; }

        [JsonIgnore]
        public bool IsSuccess => ErrCode == 0;
    }

    /// <summary>钉钉 AccessToken 响应</summary>
    public class DingTalkTokenResult : DingTalkApiResult
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }

        [JsonProperty("expires_in")]
        public int ExpiresIn { get; set; }
    }

    /// <summary>钉钉子部门列表响应（只含 dept_id）</summary>
    public class DingTalkDeptIdListResult : DingTalkApiResult
    {
        [JsonProperty("result")]
        public DingTalkDeptIdListData Result { get; set; }

        public class DingTalkDeptIdListData
        {
            [JsonProperty("dept_id_list")]
            public List<long> DeptIdList { get; set; }
        }
    }

    /// <summary>钉钉部门详情响应</summary>
    public class DingTalkDeptDetailResult : DingTalkApiResult
    {
        [JsonProperty("result")]
        public DingTalkDeptItem Result { get; set; }
    }

    /// <summary>钉钉部门项（Sync 层使用）</summary>
    public class DingTalkDeptItem
    {
        [JsonProperty("dept_id")]
        public long DeptId { get; set; }

        [JsonProperty("parent_id")]
        public long ParentId { get; set; }

        public string Name { get; set; }

        public int Order { get; set; }
    }

    /// <summary>钉钉用户 ID 列表响应</summary>
    public class DingTalkUserIdListResult : DingTalkApiResult
    {
        [JsonProperty("result")]
        public DingTalkUserIdListData Result { get; set; }

        public class DingTalkUserIdListData
        {
            [JsonProperty("userid_list")]
            public List<string> UserIdList { get; set; }
        }
    }

    /// <summary>钉钉用户列表响应（v2，支持分页，含完整信息）</summary>
    public class DingTalkUserListResult : DingTalkApiResult
    {
        [JsonProperty("result")]
        public DingTalkUserListData Result { get; set; }

        public class DingTalkUserListData
        {
            [JsonProperty("has_more")]
            public bool HasMore { get; set; }

            [JsonProperty("next_cursor")]
            public long NextCursor { get; set; }

            [JsonProperty("list")]
            public List<DingTalkUserDetailResult> List { get; set; }
        }
    }

    /// <summary>钉钉用户详情响应</summary>
    public class DingTalkUserDetailResult
    {
        [JsonProperty("userid")]
        public string UserId { get; set; }

        [JsonProperty("unionid")]
        public string UnionId { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public string Mobile { get; set; }

        public string Avatar { get; set; }

        public string Title { get; set; }

        [JsonProperty("org_email")]
        public string OrgEmail { get; set; }

        [JsonProperty("job_number")]
        public string JobNumber { get; set; }

        [JsonProperty("dept_id_list")]
        public List<long> DeptIdList { get; set; }

        /// <summary>转为 Sync 层使用的 UserItem</summary>
        public DingTalkUserItem ToUserItem()
        {
            return new DingTalkUserItem
            {
                UserId = UserId,
                DeptIdList = DeptIdList ?? new List<long>(),
                Name = Name,
                Email = Email ?? OrgEmail,
                Mobile = Mobile,
                Avatar = Avatar,
                Title = Title,
                JobNumber = JobNumber
            };
        }
    }

    /// <summary>用户项（Sync 层使用）</summary>
    public class DingTalkUserItem
    {
        public string UserId { get; set; }
        public List<long> DeptIdList { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public string Avatar { get; set; }
        public string Title { get; set; }
        public string JobNumber { get; set; }
    }

    /// <summary>钉钉扫码登录响应</summary>
    public class DingTalkLoginResult : DingTalkApiResult
    {
        [JsonProperty("user_info")]
        public DingTalkLoginUserInfo UserInfo { get; set; }

        public new bool IsSuccess => ErrCode == 0;
    }

    public class DingTalkLoginUserInfo
    {
        public string UnionId { get; set; }
        public string OpenId { get; set; }
        public string Nick { get; set; }
    }
}
