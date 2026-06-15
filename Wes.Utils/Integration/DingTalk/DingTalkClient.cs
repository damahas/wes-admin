using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Wes.Utils.Integration
{
    /// <summary>
    /// 钉钉 OpenAPI 客户端（使用 topapi/v2 接口，域名 oapi.dingtalk.com）
    /// </summary>
    public class DingTalkClient : BaseThirdPartyClient
    {
        private readonly DingTalkConfig _config;

        public DingTalkClient(DingTalkConfig config, HttpClient httpClient = null)
            : base(httpClient)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
        }

        #region Token

        protected override async Task<TokenResult> FetchAccessTokenAsync()
        {
            var url = $"{_config.BaseUrl}/gettoken?appkey={_config.ClientId}&appsecret={_config.ClientSecret}";
            var response = await Http.GetAsync(url);
            var json = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new ThirdPartyException($"获取 AccessToken 失败: {json}");

            var result = JsonConvert.DeserializeObject<DingTalkTokenResult>(json);
            if (!result.IsSuccess)
                throw new ThirdPartyException($"获取 AccessToken 失败: {result.ErrMsg}");

            return new TokenResult { Token = result.AccessToken, ExpiresIn = result.ExpiresIn };
        }

        #endregion

        #region 部门

        /// <summary>获取子部门 ID 列表</summary>
        private async Task<List<long>> GetSubDeptIdsAsync(long parentId)
        {
            var token = await GetAccessTokenAsync();
            var url = $"{_config.BaseUrl}/topapi/v2/department/listsubid?access_token={token}";
            var body = new { dept_id = parentId };
            var content = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json");
            var response = await Http.PostAsync(url, content);
            var json = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new ThirdPartyException($"获取部门列表失败: {json}");

            var result = JsonConvert.DeserializeObject<DingTalkDeptIdListResult>(json);
            if (!result.IsSuccess)
                throw new ThirdPartyException($"获取部门列表失败: {result.ErrMsg}");

            return result.Result?.DeptIdList ?? new List<long>();
        }

        /// <summary>获取部门详情</summary>
        public async Task<DingTalkDeptItem> GetDepartmentDetailAsync(long deptId)
        {
            var token = await GetAccessTokenAsync();
            var url = $"{_config.BaseUrl}/topapi/v2/department/get?access_token={token}";
            var body = new { dept_id = deptId };
            var content = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json");
            var response = await Http.PostAsync(url, content);
            var json = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new ThirdPartyException($"获取部门详情失败: {json}");

            var result = JsonConvert.DeserializeObject<DingTalkDeptDetailResult>(json);
            if (!result.IsSuccess)
                throw new ThirdPartyException($"获取部门详情失败: {result.ErrMsg}");

            return result.Result;
        }

        /// <summary>获取子部门列表（含详情）</summary>
        public async Task<List<DingTalkDeptItem>> GetDepartmentListAsync(long parentId)
        {
            var ids = await GetSubDeptIdsAsync(parentId);
            if (ids.Count == 0) return new List<DingTalkDeptItem>();

            var items = new List<DingTalkDeptItem>();
            foreach (var id in ids)
            {
                try
                {
                    items.Add(await GetDepartmentDetailAsync(id));
                }
                catch
                {
                    items.Add(new DingTalkDeptItem { DeptId = id, ParentId = parentId, Name = id.ToString() });
                }
            }
            return items;
        }

        /// <summary>递归拉取全部部门（含根部门 1，确保挂在根节点的人也能拉到）</summary>
        public async Task<List<DingTalkDeptItem>> GetAllDepartmentsAsync()
        {
            var allDepts = new List<DingTalkDeptItem>();
            try
            {
                allDepts.Add(await GetDepartmentDetailAsync(1));
            }
            catch
            {
                allDepts.Add(new DingTalkDeptItem { DeptId = 1, ParentId = 0, Name = "根部门" });
            }
            await CollectAllDeptsRecursive(1, allDepts);
            return allDepts;
        }

        private async Task CollectAllDeptsRecursive(long parentId, List<DingTalkDeptItem> result)
        {
            var subDepts = await GetDepartmentListAsync(parentId);
            result.AddRange(subDepts);
            foreach (var dept in subDepts)
                await CollectAllDeptsRecursive(dept.DeptId, result);
        }

        #endregion

        #region 用户

        /// <summary>获取部门下用户列表（v2，含完整信息，支持分页）</summary>
        public async Task<List<DingTalkUserItem>> GetAllUsersByDeptAsync(long deptId)
        {
            var token = await GetAccessTokenAsync();
            var allUsers = new List<DingTalkUserItem>();
            long cursor = 0;

            do
            {
                var url = $"{_config.BaseUrl}/topapi/v2/user/list?access_token={token}";
                var body = new { dept_id = deptId, cursor, size = 100 };
                var content = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json");
                var response = await Http.PostAsync(url, content);
                var json = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                    throw new ThirdPartyException($"获取用户列表失败: {json}");

                var result = JsonConvert.DeserializeObject<DingTalkUserListResult>(json);
                if (!result.IsSuccess)
                    throw new ThirdPartyException($"获取用户列表失败: {result.ErrMsg}");

                var list = result.Result?.List ?? new List<DingTalkUserDetailResult>();
                allUsers.AddRange(list.Select(u => u.ToUserItem()));

                cursor = result.Result?.HasMore == true ? result.Result.NextCursor : -1;
            } while (cursor > 0);

            return allUsers;
        }

        /// <summary>获取用户详情</summary>
        public async Task<DingTalkUserItem> GetUserDetailAsync(string userId)
        {
            var token = await GetAccessTokenAsync();
            var url = $"{_config.BaseUrl}/topapi/v2/user/get?access_token={token}";
            var body = new { userid = userId };
            var content = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json");
            var response = await Http.PostAsync(url, content);
            var json = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new ThirdPartyException($"获取用户详情失败: {json}");

            var result = JsonConvert.DeserializeObject<DingTalkDeptDetailUserResult>(json);
            if (!result.IsSuccess)
                throw new ThirdPartyException($"获取用户详情失败: {result.ErrMsg}");

            return result.Result?.ToUserItem();
        }

        /// <summary>用户详情响应包装（v2）</summary>
        private class DingTalkDeptDetailUserResult : DingTalkApiResult
        {
            [JsonProperty("result")]
            public DingTalkUserDetailResult Result { get; set; }
        }

        #endregion

        #region 扫码登录

        public async Task<DingTalkLoginResult> GetLoginUserByCodeAsync(string authCode)
        {
            var token = await GetAccessTokenAsync();
            var url = $"{_config.BaseUrl}/topapi/v2/user/getuserinfo?access_token={token}";
            var body = new { code = authCode };
            var content = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json");
            var response = await Http.PostAsync(url, content);
            var json = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new ThirdPartyException($"获取登录用户失败: {json}");

            var result = JsonConvert.DeserializeObject<DingTalkLoginResult>(json);
            if (!result.IsSuccess)
                throw new ThirdPartyException($"获取登录用户失败: {result.ErrMsg}");

            return result;
        }

        #endregion
    }
}
