using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Wes.Utils.Integration
{
    /// <summary>
    /// 企业微信 OpenAPI 客户端
    /// </summary>
    public class WeComClient : BaseThirdPartyClient
    {
        private readonly WeComConfig _config;

        public WeComClient(WeComConfig config, HttpClient httpClient = null)
            : base(httpClient)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
        }

        #region Token

        protected override async Task<TokenResult> FetchAccessTokenAsync()
        {
            var url = $"{_config.BaseUrl}/cgi-bin/gettoken?corpid={_config.CorpId}&corpsecret={_config.CorpSecret}";
            var response = await Http.GetStringAsync(url);
            var result = JsonConvert.DeserializeObject<WeComTokenResult>(response);

            if (!result.IsSuccess)
                throw new ThirdPartyException($"获取 AccessToken 失败: {result.ErrMsg}");

            return new TokenResult { Token = result.AccessToken, ExpiresIn = result.ExpiresIn };
        }

        #endregion

        #region 部门

        public async Task<List<WeComDeptItem>> GetDepartmentListAsync(long? parentId = null)
        {
            var token = await GetAccessTokenAsync();
            var id = parentId ?? 0;
            var url = $"{_config.BaseUrl}/cgi-bin/department/list?access_token={token}&id={id}";

            var response = await Http.GetStringAsync(url);
            var result = JsonConvert.DeserializeObject<WeComDeptListResult>(response);

            if (!result.IsSuccess)
                throw new ThirdPartyException($"获取部门列表失败: {result.ErrMsg}");

            return result.Department ?? new List<WeComDeptItem>();
        }

        public async Task<List<WeComDeptItem>> GetAllDepartmentsAsync()
        {
            var allDepts = new List<WeComDeptItem>();
            await CollectAllDeptsRecursive(0, allDepts);
            return allDepts;
        }

        private async Task CollectAllDeptsRecursive(long parentId, List<WeComDeptItem> result)
        {
            var subDepts = await GetDepartmentListAsync(parentId);
            result.AddRange(subDepts);

            foreach (var dept in subDepts)
                await CollectAllDeptsRecursive(dept.Id, result);
        }

        #endregion

        #region 用户

        public async Task<List<WeComUserItem>> GetUserListByDeptAsync(long deptId)
        {
            var token = await GetAccessTokenAsync();
            var url = $"{_config.BaseUrl}/cgi-bin/user/list?access_token={token}&department_id={deptId}&fetch_child=0";

            var response = await Http.GetStringAsync(url);
            var result = JsonConvert.DeserializeObject<WeComUserListResult>(response);

            if (!result.IsSuccess)
                throw new ThirdPartyException($"获取用户列表失败: {result.ErrMsg}");

            return result.UserList ?? new List<WeComUserItem>();
        }

        public async Task<List<WeComUserItem>> GetAllUsersAsync()
        {
            var allDepts = await GetAllDepartmentsAsync();
            var allUsers = new List<WeComUserItem>();
            var syncedIds = new HashSet<string>();

            foreach (var dept in allDepts)
            {
                try
                {
                    var users = await GetUserListByDeptAsync(dept.Id);
                    foreach (var user in users)
                    {
                        if (syncedIds.Contains(user.UserId))
                            continue;
                        syncedIds.Add(user.UserId);
                        allUsers.Add(user);
                    }
                }
                catch { /* 单个部门拉取失败不影响整体 */ }
            }

            return allUsers;
        }

        public async Task<WeComUserItem> GetUserDetailAsync(string userId)
        {
            var token = await GetAccessTokenAsync();
            var url = $"{_config.BaseUrl}/cgi-bin/user/get?access_token={token}&userid={userId}";

            var response = await Http.GetStringAsync(url);
            var result = WeComUserDetailResult.FromJson(response);

            if (!result.IsSuccess)
                throw new ThirdPartyException($"获取用户详情失败: {result.ErrMsg}");

            return result.User;
        }

        #endregion

        #region 扫码登录

        public async Task<WeComLoginResult> GetLoginUserByCodeAsync(string authCode)
        {
            var token = await GetAccessTokenAsync();
            var url = $"{_config.BaseUrl}/cgi-bin/auth/getuserinfo?access_token={token}&code={authCode}";

            var response = await Http.GetStringAsync(url);
            var result = JsonConvert.DeserializeObject<WeComLoginResult>(response);

            if (!result.IsSuccess)
                throw new ThirdPartyException($"扫码登录获取用户信息失败: {result.ErrMsg}");

            return result;
        }

        #endregion
    }
}
