# Integration 模块架构参考

## 目录结构

```
Integration/
├── Base/
│   ├── BaseThirdPartyClient.cs      # Token 缓存 + HttpClient 生命周期
│   └── BaseIntegration.cs           # 接口骨架（扫码登录→子类实现，数据拉取→委托 Sync 组件）
├── Models/
│   ├── SyncModels.cs                # SyncDeptData / SyncUserData（统一返回格式）
│   └── ThirdPartyModels.cs          # ThirdPartyUserInfo / ThirdPartyDeptInfo
├── ThirdPartyConfig.cs              # 配置基类
├── ThirdPartyException.cs           # 统一异常类
├── IThirdPartyIntegration.cs        # 统一接口定义
├── IntegrationFactory.cs            # 工厂（注册表模式）
├── DingTalk/                        # 钉钉实现
│   ├── Models/
│   │   └── DingTalkResult.cs
│   ├── DingTalkConfig.cs
│   ├── DingTalkClient.cs
│   ├── DingTalkDeptSync.cs
│   ├── DingTalkUserSync.cs
│   └── DingTalkIntegration.cs
├── WeCom/                           # 企业微信实现
└── Feishu/                          # 飞书实现
```

## 类层次关系

### ThirdPartyConfig（配置基类）

```csharp
public abstract class ThirdPartyConfig
{
    public bool Enabled { get; set; }
    public string RedirectUri { get; set; }   // 扫码登录回调地址
    public string BaseUrl { get; set; }        // OpenAPI 基础地址
    public abstract string Provider { get; }   // 平台标识
}
```

子类添加平台特有字段（AppId/AppKey、AppSecret 等），构造函数设 `BaseUrl` 默认值。

### BaseThirdPartyClient（HTTP 调用层）

```csharp
public abstract class BaseThirdPartyClient : IDisposable
{
    protected readonly HttpClient Http;
    protected BaseThirdPartyClient(HttpClient httpClient = null);

    // 自动缓存 Token（提前 5 分钟刷新）
    public async Task<string> GetAccessTokenAsync();

    // 子类只需实现这个
    protected abstract Task<TokenResult> FetchAccessTokenAsync();

    // TokenResult: { string Token, int ExpiresIn }
    protected struct TokenResult;
}
```

### BaseIntegration（业务编排层）

```csharp
public abstract class BaseIntegration : IThirdPartyIntegration, IDisposable
{
    public abstract string Provider { get; }

    // 扫码登录（子类实现）
    public abstract Task<string> GetQrCodeLoginUrl(string state);
    public abstract Task<ThirdPartyUserInfo> GetUserInfoByAuthCode(string authCode);

    // 数据拉取（子类委托 Sync 组件）
    public abstract Task<List<SyncDeptData>> GetDepartments();
    public abstract Task<SyncDeptData> GetDepartmentById(string thirdPartyDeptId);
    public abstract Task<List<SyncUserData>> GetUsers();
    public abstract Task<SyncUserData> GetUserById(string thirdPartyUserId);
}
```

### Sync 组件（数据转换层）

负责将平台原始响应转换为统一格式：

**SyncDeptData 字段**：
- `ThirdPartyDeptId` — 三方部门 ID
- `ThirdPartyParentId` — 三方父部门 ID（"0" = 根）
- `DeptName` — 部门名称
- `OrderNum` — 排序号
- `RawData` — 原始 JSON

**SyncUserData 字段**：
- `ThirdPartyUserId` — 三方用户 ID
- `ThirdPartyDeptIds` — 所在部门 ID 列表
- `UserName` — 用户名称
- `Account` — 账号
- `Email` — 邮箱
- `Mobile` — 手机号
- `Avatar` — 头像
- `Title` — 职位
- `JobNumber` — 工号
- `RawData` — 原始 JSON

**ThirdPartyUserInfo 字段**（扫码登录返回）：
- `ThirdPartyUserId`
- `ThirdPartyDeptId`
- `Name`
- `Email`
- `Mobile`
- `Avatar`
- `Title`
- `RawData`

### ThirdPartyException（统一异常）

```csharp
public class ThirdPartyException : Exception
{
    public ThirdPartyException(string message) : base(message) { }
    public ThirdPartyException(string message, Exception inner) : base(message, inner) { }
}
```

### IntegrationFactory（工厂）

```csharp
public class IntegrationFactory
{
    // 注册表：provider → 创建委托
    private readonly Dictionary<string, Func<ThirdPartyConfig, IThirdPartyIntegration>> _registry;

    // 扩展入口
    public void Register(string provider, Func<ThirdPartyConfig, IThirdPartyIntegration> factory);

    // 创建实例（按 config.Provider 路由）
    public IThirdPartyIntegration Create(ThirdPartyConfig config);
}
```

## 已有平台 API 端点参考

### 钉钉
| 用途 | 方式 | URL |
|------|------|-----|
| Token | GET | `{BaseUrl}/gettoken?appkey=...&appsecret=...` |
| 子部门列表 | POST | `{BaseUrl}/topapi/v2/department/listsub` |
| 部门详情 | POST | `{BaseUrl}/topapi/v2/department/get` |
| 用户列表 | POST | `{BaseUrl}/topapi/v2/user/list` |
| 用户详情 | POST | `{BaseUrl}/topapi/v2/user/get` |
| 扫码登录 | - | `https://login.dingtalk.com/oauth2/auth` |

### 企业微信
| 用途 | 方式 | URL |
|------|------|-----|
| Token | GET | `{BaseUrl}/cgi-bin/gettoken` |
| 部门列表 | GET | `{BaseUrl}/cgi-bin/department/list` |
| 用户列表 | GET | `{BaseUrl}/cgi-bin/user/list` |

### 飞书
| 用途 | 方式 | URL |
|------|------|-----|
| Token | POST | `{BaseUrl}/open-apis/auth/v3/tenant_access_token/internal` |
| 子部门列表 | GET | `{BaseUrl}/open-apis/contact/v3/departments/{parent_id}/children` |
| 部门用户 | GET | `{BaseUrl}/open-apis/contact/v3/users/find_by_department` |
