namespace Wes.Utils.Integration
{
    /// <summary>
    /// {PlatformName} 开放平台配置
    /// </summary>
    public class {Platform}Config : ThirdPartyConfig
    {
        /// <summary>{PlatformName} App ID / App Key</summary>
        public string AppId { get; set; }

        /// <summary>{PlatformName} App Secret</summary>
        public string AppSecret { get; set; }

        // TODO: 添加平台特有字段（CorpId、AgentId 等）

        public {Platform}Config()
        {
            BaseUrl = "{BaseUrl}";
        }

        public override string Provider => "{Provider}";
    }
}
