namespace Wes.Utils.Integration
{
    /// <summary>钉钉集成配置</summary>
    public class DingTalkConfig : ThirdPartyConfig
    {
        /// <summary>应用 AppId</summary>
        public string AppId { get; set; }

        /// <summary>应用 ClientId（原 AppKey）</summary>
        public string ClientId { get; set; }

        /// <summary>应用 ClientSecret（原 AppSecret）</summary>
        public string ClientSecret { get; set; }

        /// <summary>企业 CorpId</summary>
        public string CorpId { get; set; }

        public DingTalkConfig()
        {
            BaseUrl = "https://oapi.dingtalk.com";
        }

        public override string Provider => "DingTalk";
    }
}
