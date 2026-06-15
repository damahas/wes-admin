using System;
using System.Collections.Generic;
using System.Net.Http;

namespace Wes.Utils.Integration
{
    /// <summary>
    /// 集成工厂。支持注册表式扩展，新增平台无需改源码。
    /// 使用方式：factory.Create(config)，config.Provider 自动路由到对应实现。
    /// </summary>
    public class IntegrationFactory
    {
        private readonly HttpClient _httpClient;

        /// <summary>平台标识 → 创建委托</summary>
        private readonly Dictionary<string, Func<ThirdPartyConfig, IThirdPartyIntegration>> _registry
            = new(StringComparer.OrdinalIgnoreCase);

        public IntegrationFactory(HttpClient httpClient = null)
        {
            _httpClient = httpClient;

            // 默认注册三大平台
            Register("DingTalk", cfg => new DingTalkIntegration((DingTalkConfig)cfg, _httpClient));
            Register("WeCom", cfg => new WeComIntegration((WeComConfig)cfg, _httpClient));
            Register("Feishu", cfg => new FeishuIntegration((FeishuConfig)cfg, _httpClient));
        }

        /// <summary>注册新平台（扩展入口）</summary>
        public void Register(string provider, Func<ThirdPartyConfig, IThirdPartyIntegration> factory)
        {
            if (string.IsNullOrWhiteSpace(provider))
                throw new ArgumentNullException(nameof(provider));
            _registry[provider] = factory ?? throw new ArgumentNullException(nameof(factory));
        }

        /// <summary>根据配置创建集成实例</summary>
        public IThirdPartyIntegration Create(ThirdPartyConfig config)
        {
            if (config == null || !config.Enabled)
                throw new InvalidOperationException("三方集成未启用或配置为空");

            if (string.IsNullOrWhiteSpace(config.Provider) || !_registry.TryGetValue(config.Provider, out var factory))
                throw new NotSupportedException($"不支持的三方平台: {config.Provider}");

            return factory(config);
        }
    }
}
