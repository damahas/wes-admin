using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Wes.Utils.Integration
{
    /// <summary>企业微信集成实现</summary>
    public class WeComIntegration : BaseIntegration
    {
        private readonly WeComClient _client;
        private readonly WeComDeptSync _deptSync;
        private readonly WeComUserSync _userSync;
        private readonly WeComConfig _config;

        public override string Provider => "WeCom";

        public WeComIntegration(WeComConfig config, HttpClient httpClient = null)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _client = new WeComClient(config, httpClient);
            _deptSync = new WeComDeptSync(_client);
            _userSync = new WeComUserSync(_client);
        }

        #region 扫码登录

        public override async Task<string> GetQrCodeLoginUrl(string state)
        {
            var redirectUri = _config.RedirectUri ?? "";
            var url = $"https://open.weixin.qq.com/connect/oauth2/authorize" +
                      $"?appid={_config.CorpId}" +
                      $"&redirect_uri={Uri.EscapeDataString(redirectUri)}" +
                      $"&response_type=code" +
                      $"&scope=snsapi_base" +
                      $"&state={state}" +
                      $"#wechat_redirect";
            return await Task.FromResult(url);
        }

        public override async Task<ThirdPartyUserInfo> GetUserInfoByAuthCode(string authCode)
        {
            var loginResult = await _client.GetLoginUserByCodeAsync(authCode);
            if (!loginResult.IsSuccess)
                throw new ThirdPartyException($"获取登录用户失败: {loginResult.ErrMsg}");

            return new ThirdPartyUserInfo
            {
                ThirdPartyUserId = loginResult.UserId,
                Name = loginResult.UserId
            };
        }

        #endregion

        #region 数据拉取

        public override async Task<List<SyncDeptData>> GetDepartments()
            => await _deptSync.GetAllAsync();

        public override async Task<SyncDeptData> GetDepartmentById(string thirdPartyDeptId)
        {
            // 企微无单部门查询接口，走全量拉取后筛选
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
