namespace Wes.Utils.Integration
{
    /// <summary>企业微信集成配置</summary>
    public class WeComConfig : ThirdPartyConfig
    {
        /// <summary>企业 CorpId</summary>
        public string CorpId { get; set; }

        /// <summary>应用 Secret（通讯录同步用）</summary>
        public string CorpSecret { get; set; }

        /// <summary>AgentId（应用 ID）</summary>
        public long? AgentId { get; set; }

        public WeComConfig()
        {
            BaseUrl = "https://qyapi.weixin.qq.com";
        }

        public override string Provider => "WeCom";
    }
}
