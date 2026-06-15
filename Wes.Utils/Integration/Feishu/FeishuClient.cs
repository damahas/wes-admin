using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Wes.Utils.Integration
{
    /// <summary>
    /// 飞书 OpenAPI 客户端
    /// </summary>
    public class FeishuClient : BaseThirdPartyClient
    {
        private readonly FeishuConfig _config;

        public FeishuClient(FeishuConfig config, HttpClient httpClient = null)
            : base(httpClient)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
        }

        #region Token

        protected override async Task<TokenResult> FetchAccessTokenAsync()
        {
            var url = $"{_config.BaseUrl}/open-apis/auth/v3/tenant_access_token/internal";
            var body = new { app_id = _config.AppId, app_secret = _config.AppSecret };
            var content = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json");

            var response = await Http.PostAsync(url, content);
            var json = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<FeishuTokenResult>(json);

            if (!result.IsSuccess)
                throw new ThirdPartyException($"获取 tenant_access_token 失败: {result.Msg}");

            return new TokenResult { Token = result.TenantAccessToken, ExpiresIn = result.Expire };
        }

        #endregion

        #region 部门

        public async Task<FeishuPageData<FeishuDeptItem>> GetDepartmentListAsync(string parentId = "0", string pageToken = null)
        {
            var token = await GetAccessTokenAsync();
            var url = $"{_config.BaseUrl}/open-apis/contact/v3/departments" +
                      $"?parent_department_id={parentId}&page_size=100";

            if (!string.IsNullOrEmpty(pageToken))
                url += $"&page_token={pageToken}";

            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("Authorization", $"Bearer {token}");

            var response = await Http.SendAsync(request);
            var json = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<FeishuApiResponse<FeishuPageData<FeishuDeptItem>>>(json);

            if (!result.IsSuccess)
                throw new ThirdPartyException($"获取部门列表失败: {result.Msg}");

            return result.Data ?? new FeishuPageData<FeishuDeptItem>();
        }

        public async Task<List<FeishuDeptItem>> GetAllDepartmentsAsync()
        {
            var allDepts = new List<FeishuDeptItem>();
            await CollectAllDeptsRecursive("0", allDepts);
            return allDepts;
        }

        private async Task CollectAllDeptsRecursive(string parentId, List<FeishuDeptItem> result)
        {
            string pageToken = null;
            do
            {
                var page = await GetDepartmentListAsync(parentId, pageToken);
                if (page.Items != null)
                    result.AddRange(page.Items);

                pageToken = page.HasMore ? page.PageToken : null;

                if (page.Items != null)
                {
                    foreach (var dept in page.Items)
                        await CollectAllDeptsRecursive(dept.DepartmentId, result);
                }
            } while (pageToken != null);
        }

        #endregion

        #region 用户

        public async Task<FeishuPageData<FeishuUserItem>> GetUserListByDeptAsync(string deptId, string pageToken = null)
        {
            var token = await GetAccessTokenAsync();
            var url = $"{_config.BaseUrl}/open-apis/contact/v3/users" +
                      $"?department_id={deptId}&page_size=100";

            if (!string.IsNullOrEmpty(pageToken))
                url += $"&page_token={pageToken}";

            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("Authorization", $"Bearer {token}");

            var response = await Http.SendAsync(request);
            var json = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<FeishuApiResponse<FeishuPageData<FeishuUserItem>>>(json);

            if (!result.IsSuccess)
                throw new ThirdPartyException($"获取用户列表失败: {result.Msg}");

            return result.Data ?? new FeishuPageData<FeishuUserItem>();
        }

        public async Task<List<FeishuUserItem>> GetAllUsersAsync()
        {
            var allDepts = await GetAllDepartmentsAsync();
            var allUsers = new List<FeishuUserItem>();
            var syncedIds = new HashSet<string>();

            foreach (var dept in allDepts)
            {
                try
                {
                    string pageToken = null;
                    do
                    {
                        var page = await GetUserListByDeptAsync(dept.DepartmentId, pageToken);
                        if (page.Items != null)
                        {
                            foreach (var user in page.Items)
                            {
                                if (syncedIds.Contains(user.OpenId))
                                    continue;
                                syncedIds.Add(user.OpenId);
                                allUsers.Add(user);
                            }
                        }
                        pageToken = page.HasMore ? page.PageToken : null;
                    } while (pageToken != null);
                }
                catch { /* 单个部门拉取失败不影响整体 */ }
            }

            return allUsers;
        }

        public async Task<FeishuUserItem> GetUserDetailAsync(string openId)
        {
            var token = await GetAccessTokenAsync();
            var url = $"{_config.BaseUrl}/open-apis/contact/v3/users/{openId}";

            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("Authorization", $"Bearer {token}");

            var response = await Http.SendAsync(request);
            var json = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<FeishuApiResponse<FeishuApiUserDetail>>(json);

            if (!result.IsSuccess)
                throw new ThirdPartyException($"获取用户详情失败: {result.Msg}");

            return result.Data?.User;
        }

        /// <summary>飞书用户详情返回结构（私有）</summary>
        private class FeishuApiUserDetail
        {
            [JsonProperty("user")]
            public FeishuUserItem User { get; set; }
        }

        #endregion

        #region 扫码登录

        public async Task<FeishuLoginAccessTokenResult> GetLoginAccessTokenAsync(string authCode)
        {
            var url = $"{_config.BaseUrl}/open-apis/authen/v1/access_token";
            var body = new { grant_type = "authorization_code", code = authCode };
            var content = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json");
            content.Headers.Add("Authorization", $"Bearer {await GetAccessTokenAsync()}");

            var response = await Http.PostAsync(url, content);
            var json = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<FeishuLoginAccessTokenResult>(json);

            if (!result.IsSuccess)
                throw new ThirdPartyException($"扫码登录获取 access_token 失败: {result.Msg}");

            return result;
        }

        public async Task<FeishuLoginUserInfoResult> GetLoginUserInfoAsync(string userAccessToken)
        {
            var url = $"{_config.BaseUrl}/open-apis/authen/v1/user_info";

            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("Authorization", $"Bearer {userAccessToken}");

            var response = await Http.SendAsync(request);
            var json = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<FeishuLoginUserInfoResult>(json);

            if (!result.IsSuccess)
                throw new ThirdPartyException($"扫码登录获取用户信息失败: {result.Msg}");

            return result;
        }

        #endregion
    }
}
