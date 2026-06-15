using System;

namespace Wes.Utils.Integration
{
    /// <summary>
    /// 三方集成统一异常，替换各平台各自定义的异常类。
    /// </summary>
    public class ThirdPartyException : Exception
    {
        public ThirdPartyException(string message) : base(message) { }
        public ThirdPartyException(string message, Exception inner) : base(message, inner) { }
    }
}
