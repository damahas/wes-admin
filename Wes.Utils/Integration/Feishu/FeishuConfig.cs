namespace Wes.Utils.Integration
{
    /// <summary>飞书集成配置</summary>
    public class FeishuConfig : ThirdPartyConfig
    {
        /// <summary>应用 AppId</summary>
        public string AppId { get; set; }

        /// <summary>应用 AppSecret</summary>
        public string AppSecret { get; set; }

        public FeishuConfig()
        {
            BaseUrl = "https://open.feishu.cn";
        }

        public override string Provider => "Feishu";
    }
}
