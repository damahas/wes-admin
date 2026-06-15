using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Wes.Utils.Integration
{
    /// <summary>
    /// {PlatformName} OpenAPI 客户端，继承 BaseThirdPartyClient。
    /// Token 缓存由基类自动管理，子类只需实现 FetchAccessTokenAsync。
    /// </summary>
    public class {Platform}Client : BaseThirdPartyClient
    {
        private readonly {Platform}Config _config;

        public {Platform}Client({Platform}Config config, HttpClient httpClient = null)
            : base(httpClient)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
        }

        #region Token

        protected override async Task<TokenResult> FetchAccessTokenAsync()
        {
            // TODO: 根据 {PlatformName} 的 Token 接口实现
            // 参考钉钉 GET 方式：
            //   var url = $"{_config.BaseUrl}/gettoken?appkey={_config.AppId}&appsecret={_config.AppSecret}";
            //   var response = await Http.GetStringAsync(url);
            //   var result = JsonConvert.DeserializeObject<{Platform}TokenResult>(response);
            //   if (!result.IsSuccess)
            //       throw new ThirdPartyException($"获取 AccessToken 失败: {result.ErrMsg}");
            //   return new TokenResult { Token = result.AccessToken, ExpiresIn = result.ExpiresIn };
            //
            // 参考飞书 POST 方式：
            //   var body = new { app_id = _config.AppId, app_secret = _config.AppSecret };
            //   var content = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json");
            //   var resp = await Http.PostAsync($"{_config.BaseUrl}/open-apis/auth/v3/tenant_access_token/internal", content);
            //   ...

            throw new NotImplementedException();
        }

        #endregion

        #region 部门

        // TODO: 添加 {PlatformName} 部门相关 API 方法
        // 示例：
        // public async Task<List<{Platform}DeptItem>> GetDepartmentListAsync(long? parentId = null)
        // {
        //     var token = await GetAccessTokenAsync();
        //     ...
        // }

        #endregion

        #region 用户

        // TODO: 添加 {PlatformName} 用户相关 API 方法

        #endregion

        #region 扫码登录

        // TODO: 添加 {PlatformName} 扫码登录相关 API 方法
        // 示例：public async Task<{Platform}LoginResult> GetLoginUserByCodeAsync(string authCode)

        #endregion
    }
}
