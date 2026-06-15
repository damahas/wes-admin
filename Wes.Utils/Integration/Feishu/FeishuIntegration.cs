using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Wes.Utils.Integration
{
    /// <summary>飞书集成实现</summary>
    public class FeishuIntegration : BaseIntegration
    {
        private readonly FeishuClient _client;
        private readonly FeishuDeptSync _deptSync;
        private readonly FeishuUserSync _userSync;
        private readonly FeishuConfig _config;

        public override string Provider => "Feishu";

        public FeishuIntegration(FeishuConfig config, HttpClient httpClient = null)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _client = new FeishuClient(config, httpClient);
            _deptSync = new FeishuDeptSync(_client);
            _userSync = new FeishuUserSync(_client);
        }

        #region 扫码登录

        public override async Task<string> GetQrCodeLoginUrl(string state)
        {
            var redirectUri = _config.RedirectUri ?? "";
            var url = $"https://open.feishu.cn/open-apis/authen/v1/authorize" +
                      $"?app_id={_config.AppId}" +
                      $"&redirect_uri={Uri.EscapeDataString(redirectUri)}" +
                      $"&state={state}";
            return await Task.FromResult(url);
        }

        public override async Task<ThirdPartyUserInfo> GetUserInfoByAuthCode(string authCode)
        {
            var tokenResult = await _client.GetLoginAccessTokenAsync(authCode);
            if (!tokenResult.IsSuccess || string.IsNullOrEmpty(tokenResult.Data?.AccessToken))
                throw new ThirdPartyException("扫码登录获取 access_token 失败");

            var userResult = await _client.GetLoginUserInfoAsync(tokenResult.Data.AccessToken);
            if (!userResult.IsSuccess || userResult.Data == null)
                throw new ThirdPartyException("扫码登录获取用户信息失败");

            var userData = userResult.Data;
            return new ThirdPartyUserInfo
            {
                ThirdPartyUserId = userData.OpenId,
                Name = userData.Name,
                Email = userData.Email,
                Mobile = userData.Mobile
            };
        }

        #endregion

        #region 数据拉取

        public override async Task<List<SyncDeptData>> GetDepartments()
            => await _deptSync.GetAllAsync();

        public override async Task<SyncDeptData> GetDepartmentById(string thirdPartyDeptId)
        {
            var all = await GetDepartments();
            return all.Find(d => d.ThirdPartyDeptId == thirdPartyDeptId);
        }

        public override async Task<List<SyncUserData>> GetUsers()
            => await _userSync.GetAllAsync();

        public override async Task<SyncUserData> GetUserById(string thirdPartyUserId)
            => await _userSync.GetByIdAsync(thirdPartyUserId);

        #endregion

        public override void Dispose() => _client?.Dispose();
    }
}
