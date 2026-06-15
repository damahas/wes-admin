---
name: add-third-party-integration
description: 在 Wes.Utils/Integration 目录下新增三方平台集成。当用户说"新增一个 XXX 三方集成"、"加个 XXX 平台支持"、"帮我接入 XXX 作为第三方登录"时触发。按照现有架构规范自动生成所有必需文件，并在 IntegrationFactory 中注册。
---

# 新增三方平台集成

## 概述

在 `Wes.Utils/Integration` 目录下，按照项目现有架构规范新增一个三方平台集成。

生成以下 6 个文件（设平台名为 `{Platform}`）：

```
Integration/
└── {Platform}/
    ├── {Platform}Config.cs
    ├── {Platform}Client.cs
    ├── {Platform}Integration.cs
    ├── {Platform}DeptSync.cs
    ├── {Platform}UserSync.cs
    └── Models/
        └── {Platform}Result.cs
```

并在 `IntegrationFactory.cs` 构造函数中注册一行。

## 执行流程

### 第 1 步：确认平台信息

向用户确认：
1. 平台名称（英文标识，如 `Lark`，用于 Provider 属性、目录名和 Factory 注册）
2. 平台中文名（用于注释）
3. OpenAPI 基础地址（如 `https://open.larksuite.com`）
4. 认证字段名（通常为 AppId / AppSecret，也可能是 AppKey）
5. Token 获取接口（URL 路径 + 请求方式 GET/POST）

特别说明：
- 如果用户未提供 API 细节或只说了平台名而你不了解该平台 API，直接告知用户并提供通用模板。不要编造不存在的接口地址。
- 如果用户提供了 API 文档链接或具体端点信息，按实际端点在模板 TODO 中标注。

### 第 2 步：读取现有参考代码

**必须先读取以下文件**以确保生成的代码与现有架构完全一致：

| 文件 | 作用 |
|------|------|
| `Wes.Utils/Integration/ThirdPartyConfig.cs` | 配置基类 |
| `Wes.Utils/Integration/Base/BaseThirdPartyClient.cs` | Client 基类 |
| `Wes.Utils/Integration/Base/BaseIntegration.cs` | Integration 基类 |
| `Wes.Utils/Integration/Models/SyncModels.cs` | 统一返回模型 |
| `Wes.Utils/Integration/Models/ThirdPartyModels.cs` | 用户/部门统一模型 |
| `Wes.Utils/Integration/IThirdPartyIntegration.cs` | 统一接口 |
| `Wes.Utils/Integration/IntegrationFactory.cs` | 工厂（需修改） |
| `Wes.Utils/Integration/ThirdPartyException.cs` | 统一异常类 |
| `Wes.Utils/Integration/DingTalk/DingTalkConfig.cs` | Config 参考 |
| `Wes.Utils/Integration/DingTalk/DingTalkClient.cs` | Client 参考 |
| `Wes.Utils/Integration/DingTalk/DingTalkIntegration.cs` | Integration 参考 |
| `Wes.Utils/Integration/DingTalk/DingTalkDeptSync.cs` | DeptSync 参考 |
| `Wes.Utils/Integration/DingTalk/DingTalkUserSync.cs` | UserSync 参考 |

### 第 3 步：生成 6 个文件

按照 `assets/templates/` 中的模板，结合第 1 步收集的平台信息，生成所有文件。

**关键规范（必须遵守）**：
- 命名空间统一用 `Wes.Utils.Integration`
- Config 继承 `ThirdPartyConfig`，`Provider` 属性返回平台标识，构造函数设 `BaseUrl`
- Client 继承 `BaseThirdPartyClient`，**只需实现 `FetchAccessTokenAsync()`**，Token 缓存基类已实现
- Integration 继承 `BaseIntegration`，**保存 `_config` 字段**（用于拼 OAuth URL），组合 Client + DeptSync + UserSync
- DeptSync / UserSync 提供 `GetAllAsync()` 和 `GetByIdAsync(...)` 方法
- 平台原始响应模型放在 `Models/{Platform}Result.cs`
- **务必使用 `ThirdPartyException` 抛出异常**，不要创建平台专属异常
- `ThirdPartyException` 构造函数仅支持 `(string message)` 和 `(string message, Exception inner)`

### 第 4 步：注册 Factory

在 `IntegrationFactory.cs` 构造函数中添加一行：

```csharp
Register("{Provider}", cfg => new {Platform}Integration(({Platform}Config)cfg, _httpClient));
```

### 第 5 步：验证

- 检查所有文件命名空间、类名正确
- 确认 Factory 注册的 Provider 字符串与 Config.Provider 属性一致
- 确认目录结构正确

## 架构关系图

```
IntegrationFactory.Create(config) ──→ config.Provider 路由
   │
   └── {Platform}Integration : BaseIntegration
          ├── GetQrCodeLoginUrl(state)      ── 子类实现（拼 OAuth URL）
          ├── GetUserInfoByAuthCode(authCode)── 子类实现（调 Client 换用户信息）
          ├── GetDepartments()               ── 委托 _deptSync.GetAllAsync()
          ├── GetDepartmentById(id)          ── 委托 _deptSync.GetByIdAsync(id)
          ├── GetUsers()                     ── 委托 _userSync.GetAllAsync()
          └── GetUserById(id)                ── 委托 _userSync.GetByIdAsync(id)
                │
                ├── {Platform}Client : BaseThirdPartyClient
                │      └── FetchAccessTokenAsync()  ← 唯一需实现的方法
                ├── {Platform}DeptSync ── 调 Client → 转 SyncDeptData
                └── {Platform}UserSync ── 调 Client → 转 SyncUserData
```

## 注意事项

- Token 缓存逻辑已在 `BaseThirdPartyClient` 实现，子类不要重复实现
- 部门/用户同步返回统一格式 `SyncDeptData` / `SyncUserData`，映射逻辑写在 `*Sync.cs` 中
- 扫码登录的两个方法（`GetQrCodeLoginUrl` / `GetUserInfoByAuthCode`）在 Integration 中实现
- 如果用户只提供平台名但无法提供具体 API 细节，模板中的 TODO 标记需要保留让它作为待填写项
