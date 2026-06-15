using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Wes.Utils.Integration
{
    /// <summary>钉钉集成实现</summary>
    public class DingTalkIntegration : BaseIntegration
    {
        private readonly DingTalkClient _client;
        private readonly DingTalkDeptSync _deptSync;
        private readonly DingTalkUserSync _userSync;
        private readonly DingTalkConfig _config;

        public override string Provider => "DingTalk";

        public DingTalkIntegration(DingTalkConfig config, HttpClient httpClient = null)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _client = new DingTalkClient(config, httpClient);
            _deptSync = new DingTalkDeptSync(_client);
            _userSync = new DingTalkUserSync(_client);
        }

        #region 扫码登录

        public override async Task<string> GetQrCodeLoginUrl(string state)
        {
            var redirectUri = _config.RedirectUri ?? "";
            var url = $"https://login.dingtalk.com/oauth2/auth" +
                      $"?redirect_uri={Uri.EscapeDataString(redirectUri)}" +
                      $"&response_type=code" +
                      $"&client_id={_config.AppId}" +
                      $"&scope=openid+corpid" +
                      $"&state={state}" +
                      $"&prompt=consent";
            return await Task.FromResult(url);
        }

        public override async Task<ThirdPartyUserInfo> GetUserInfoByAuthCode(string authCode)
        {
            var loginResult = await _client.GetLoginUserByCodeAsync(authCode);
            if (!loginResult.IsSuccess)
                throw new ThirdPartyException($"获取登录用户失败: {loginResult.ErrMsg}");

            var userInfo = loginResult.UserInfo
                ?? throw new ThirdPartyException("获取登录用户信息为空");

            return new ThirdPartyUserInfo
            {
                ThirdPartyUserId = userInfo.UnionId,
                Name = userInfo.Nick,
            };
        }

        #endregion

        #region 数据拉取

        public override async Task<List<SyncDeptData>> GetDepartments()
            => await _deptSync.GetAllAsync();

        public override async Task<SyncDeptData> GetDepartmentById(string thirdPartyDeptId)
        {
            if (!long.TryParse(thirdPartyDeptId, out var deptId))
                throw new ArgumentException($"无效的部门 ID: {thirdPartyDeptId}");
            return await _deptSync.GetByIdAsync(deptId);
        }

        public override async Task<List<SyncUserData>> GetUsers()
            => await _userSync.GetAllAsync();

        public override async Task<SyncUserData> GetUserById(string thirdPartyUserId)
            => await _userSync.GetByIdAsync(thirdPartyUserId);

        #endregion

        public override void Dispose() => _client?.Dispose();
    }
}
