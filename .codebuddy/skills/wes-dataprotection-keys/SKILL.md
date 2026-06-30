---
name: wes-dataprotection-keys
description: >
  生成 ASP.NET Core DataProtection 密钥文件，用于多实例 Docker 部署时共享加密密钥。
  当用户提到 "生成密钥"、"DataProtection 密钥"、"Docker 密钥共享"、"预生成密钥" 时触发。
---

## 用途

为 WesAdmin 项目生成可复用的 DataProtection 密钥 XML 文件，确保 Docker 多实例部署时加密票据互通。

## 使用方式

### 生成密钥

执行 `scripts/generate-keys.ps1`：

```powershell
powershell -ExecutionPolicy Bypass -File scripts/generate-keys.ps1
```

密钥文件输出到 `Wes.WebApi/DataProtection/key-{guid}.xml`。

### 默认行为（无预生成密钥）

如果未预先生成密钥，ASP.NET Core 启动时会在 `DataProtection/` 目录自动创建密钥。
系统**不依赖**预生成密钥，条件不满足时静默回退到运行时自动生成。

## 多实例部署

所有实例共享同一份密钥文件：

```bash
docker run -v /host/DataProtection:/app/DataProtection wes-admin
```

或预置到镜像（Dockerfile 已包含 `COPY Wes.WebApi/DataProtection /app/DataProtection/`）。

## 密钥轮换

默认 90 天有效期，ASP.NET Core 自动轮换。旧密钥保留用于解密历史数据。
