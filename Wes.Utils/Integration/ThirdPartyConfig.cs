namespace Wes.Utils.Integration
{
    /// <summary>
    /// 三方平台配置基类。
    /// 各平台（钉钉/企微/飞书）的配置继承此类，添加平台特有字段。
    /// </summary>
    public abstract class ThirdPartyConfig
    {
        /// <summary>是否启用该平台集成</summary>
        public bool Enabled { get; set; }

        /// <summary>扫码登录回调地址</summary>
        public string RedirectUri { get; set; }

        /// <summary>OpenAPI 基础地址</summary>
        public string BaseUrl { get; set; }

        /// <summary>平台标识（如 "DingTalk"、"WeCom"、"Feishu"）</summary>
        public abstract string Provider { get; }
    }
}
