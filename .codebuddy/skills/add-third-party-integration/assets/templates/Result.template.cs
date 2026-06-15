using Newtonsoft.Json;

namespace Wes.Utils.Integration
{
    /// <summary>
    /// {PlatformName} OpenAPI 原始响应模型
    /// </summary>

    /// <summary>
    /// Token 接口返回
    /// </summary>
    public class {Platform}TokenResult
    {
        // TODO: 根据实际 API 响应定义字段
        // 示例：
        // [JsonProperty("errcode")]
        // public int ErrCode { get; set; }
        //
        // [JsonProperty("errmsg")]
        // public string ErrMsg { get; set; }
        //
        // [JsonProperty("access_token")]
        // public string AccessToken { get; set; }
        //
        // [JsonProperty("expires_in")]
        // public int ExpiresIn { get; set; }
        //
        // public bool IsSuccess => ErrCode == 0;
    }

    // TODO: 添加 {PlatformName} 其他接口的响应模型，例如：
    // - {Platform}DeptListResult    — 部门列表
    // - {Platform}UserListResult    — 用户列表
    // - {Platform}LoginResult       — 扫码登录
    // - {Platform}DeptItem          — 单个部门
    // - {Platform}UserItem          — 单个用户
}
