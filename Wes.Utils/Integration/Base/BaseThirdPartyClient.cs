using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Wes.Utils.Integration
{
    /// <summary>
    /// 三方平台 API 客户端基类，封装 HttpClient 生命周期、Token 缓存框架。
    /// 子类只需实现 <see cref="FetchAccessTokenAsync"/> 提供各平台自己的 Token 换取逻辑。
    /// </summary>
    public abstract class BaseThirdPartyClient : IDisposable
    {
        protected readonly HttpClient Http;
        private string _accessToken;
        private DateTime _tokenExpireTime = DateTime.MinValue;

        protected BaseThirdPartyClient(HttpClient httpClient = null)
        {
            Http = httpClient ?? new HttpClient { Timeout = TimeSpan.FromSeconds(30) };
        }

        /// <summary>
        /// 获取或刷新 AccessToken（自动缓存，提前 5 分钟刷新）
        /// </summary>
        public async Task<string> GetAccessTokenAsync()
        {
            if (!string.IsNullOrEmpty(_accessToken) && DateTime.Now < _tokenExpireTime)
                return _accessToken;

            var result = await FetchAccessTokenAsync();
            _accessToken = result.Token;
            _tokenExpireTime = DateTime.Now.AddSeconds(result.ExpiresIn - 300);
            return _accessToken;
        }

        /// <summary>
        /// 向三方平台发起获取 Token 的请求，子类实现。
        /// </summary>
        protected abstract Task<TokenResult> FetchAccessTokenAsync();

        /// <summary>
        /// Token 刷新结果
        /// </summary>
        protected struct TokenResult
        {
            public string Token;
            public int ExpiresIn;
        }

        public virtual void Dispose()
        {
            Http?.Dispose();
        }
    }
}
