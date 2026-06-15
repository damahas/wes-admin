using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Wes.Utils.Integration
{
    /// <summary>
    /// {PlatformName} 集成实现，继承 BaseIntegration。
    /// 扫码登录由子类实现，数据拉取委托给 DeptSync/UserSync 组件。
    /// </summary>
    public class {Platform}Integration : BaseIntegration
    {
        private readonly {Platform}Client _client;
        private readonly {Platform}DeptSync _deptSync;
        private readonly {Platform}UserSync _userSync;
        private readonly {Platform}Config _config;

        public override string Provider => "{Provider}";

        public {Platform}Integration({Platform}Config config, HttpClient httpClient = null)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _client = new {Platform}Client(config, httpClient);
            _deptSync = new {Platform}DeptSync(_client);
            _userSync = new {Platform}UserSync(_client);
        }

        #region 扫码登录

        public override async Task<string> GetQrCodeLoginUrl(string state)
        {
            // TODO: 拼接 {PlatformName} OAuth 授权地址
            // 参考钉钉实现：
            // var redirectUri = _config.RedirectUri ?? "";
            // var url = $"https://login.example.com/oauth2/auth" +
            //           $"?redirect_uri={Uri.EscapeDataString(redirectUri)}" +
            //           $"&response_type=code" +
            //           $"&client_id={_config.AppId}" +
            //           $"&scope=openid" +
            //           $"&state={state}" +
            //           $"&prompt=consent";
            // return await Task.FromResult(url);

            throw new NotImplementedException();
        }

        public override async Task<ThirdPartyUserInfo> GetUserInfoByAuthCode(string authCode)
        {
            // TODO: 用 authCode 换用户信息 → 转成 ThirdPartyUserInfo
            // 参考钉钉实现：
            // var loginResult = await _client.GetLoginUserByCodeAsync(authCode);
            // if (!loginResult.IsSuccess)
            //     throw new ThirdPartyException($"获取登录用户失败: {loginResult.ErrMsg}");
            // var userInfo = loginResult.UserInfo;
            // return new ThirdPartyUserInfo
            // {
            //     ThirdPartyUserId = userInfo.UserId,
            //     Name = userInfo.Nick,
            // };

            throw new NotImplementedException();
        }

        #endregion

        #region 数据拉取

        public override async Task<List<SyncDeptData>> GetDepartments()
            => await _deptSync.GetAllAsync();

        public override async Task<SyncDeptData> GetDepartmentById(string thirdPartyDeptId)
            => await _deptSync.GetByIdAsync(thirdPartyDeptId);

        public override async Task<List<SyncUserData>> GetUsers()
            => await _userSync.GetAllAsync();

        public override async Task<SyncUserData> GetUserById(string thirdPartyUserId)
            => await _userSync.GetByIdAsync(thirdPartyUserId);

        #endregion

        public override void Dispose()
        {
            _client?.Dispose();
        }
    }
}
