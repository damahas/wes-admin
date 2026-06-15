# WesAdmin

<p align="center">
  <strong>一个基于 Vue 3 + .NET 10 的企业级后台管理系统</strong>
</p>

<p align="center">
  <img src="https://img.shields.io/badge/Vue-3.4-brightgreen" alt="Vue">
  <img src="https://img.shields.io/badge/.NET-10.0-blue" alt=".NET">
  <img src="https://img.shields.io/badge/Element--Plus-2.13-blue" alt="Element Plus">
  <img src="https://img.shields.io/badge/MySQL-8.0-orange" alt="MySQL">
  <img src="https://img.shields.io/badge/license-MIT-green" alt="license">
</p>

---

## 简介

WesAdmin 是一套基于若依（RuoYi）的全新 UI 全栈企业级后台管理系统，提供用户管理、RBAC 权限控制、可视化工作流引擎、代码生成器、数据服务编排等核心功能，帮助开发者快速搭建和交付企业级后台应用。

> **开发状态**: 持续迭代中，欢迎 Star / Fork / PR。

---

## 技术栈

| 层级 | 技术 | 说明 |
|------|------|------|
| **前端框架** | Vue 3 + Vite 5 | Composition API, SCSS |
| **UI 组件库** | Element Plus 2.13 | 企业级 UI 组件 |
| **前端状态管理** | Vuex 4 | 全局状态管理 |
| **前端路由** | Vue Router 4 | 动态路由, 权限路由 |
| **流程图** | @antv/x6 | 工作流可视化设计 |
| **国际化** | vue-i18n | 简体中文 / English |
| **后端框架** | ASP.NET Core Web API | .NET 10.0 |
| **ORM** | SqlSugarCore 5.1 | Code-First, MySQL |
| **DI 容器** | Autofac 9.1 | 依赖注入 |
| **认证授权** | JWT Bearer | Token 认证 |
| **API 文档** | Swagger (Swashbuckle) | 自动 API 文档 |
| **缓存** | MemoryCache / Redis | 可选 Redis 支持 |

---

## 功能概览

### 系统管理
- **用户管理** — 用户 CRUD、状态管理、角色分配、岗位分配、密码重置、Excel 导出
- **角色管理** — 角色 CRUD、菜单权限分配、数据权限（部门隔离）、用户分配
- **菜单管理** — 菜单树 CRUD、前端动态路由自动生成
- **部门管理** — 部门树 CRUD、部门选择器
- **岗位管理** — 岗位 CRUD
- **字典管理** — 字典类型 + 字典数据维护，支持树结构
- **参数配置** — 系统参数 CRUD、缓存刷新、Excel 导出、钉钉/企微/飞书/邮件集成配置
- **编号规则** — 自定义编码规则生成器
- **数据服务** — 可视化 API 编排（SQL + JavaScript 步骤组合），对外暴露 API
- **许可证** — 设备指纹绑定、许可证激活/验证、企业版/试用版
- **文件管理** — 文件上传下载
- **日志管理** — 登录日志 + 操作日志查询
- **站内信** — 消息中心（已读/未读）
- **在线用户** — 在线用户监控

### 工作流管理
- **流程定义** — 基于 @antv/x6 的可视化流程设计器，拖拽节点和连线
- **流程版本** — 版本管理、版本复制、版本启用/停用
- **流程实例** — 发起流程、审批任务处理、流程跟踪
- **节点类型** — 开始、结束、任务（审批）、通知
- **审批方式** — 按用户、按角色、按部门
- **防死循环** — 流程引擎内置深度保护机制

---

## 项目架构

```
WesAdmin.sln
├── Wes.UI/                  # 前端 SPA (Vue3 + Vite + Element Plus)
├── Wes.WebApi/              # 表示层 / Web API 入口
│   ├── Controllers/         #   公共控制器
│   ├── Areas/               #   区域控制器
│   │   ├── SystemManage/    #     系统管理
│   │   ├── FlowManage/      #     流程管理
│   └── Extensions/          #   中间件扩展
├── Wes.Business/            # 业务逻辑层 (Biz)
├── Wes.Service/             # 数据访问层 (Service/Repository)
├── Wes.DbModel/             # 数据库模型层 (ORM 实体)
├── Wes.ViewModel/           # 视图模型 (DTO/Param/Result)
└── Wes.Utils/               # 工具层 (缓存、加密、三方集成等)
    └── Integration/          #   钉钉/企微/飞书/邮件集成
```

**依赖关系**: `WebApi` → `Business` → `Service` → `DbModel`，所有层均依赖 `Utils` 和 `ViewModel`。

---

## 快速开始

### 环境要求

- **后端**: [.NET 10.0 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- **前端**: [Node.js](https://nodejs.org/) 18+
- **数据库**: [MySQL](https://www.mysql.com/) 8.0+

### 后端启动

1. 克隆仓库
```bash
git clone https://github.com/wes-admin/wes-admin.git
cd wes-admin
```

2. 配置数据库连接

编辑 `Wes.WebApi/appsettings.Development.json`（或 `appsettings.json`）：
```json
{
  "ConnectionStrings": {
    "WesConnectionString": "DataBase=wes_admin;Data Source=127.0.0.1;User Id=root;Password=your_password;CharSet=utf8;port=3306"
  }
}
```

3. 运行后端
```bash
cd Wes.WebApi
dotnet run
```

> 应用启动后会自动建表（Code-First），无需手动执行 SQL。

后端启动在 `http://localhost:5113`，Swagger 文档在 `http://localhost:5113/swagger`。

### 前端启动

1. 安装依赖
```bash
cd Wes.UI
npm install
```

2. 启动开发服务器
```bash
npm run dev
```

前端启动在 `http://localhost:3000`，API 请求自动代理到后端 `http://localhost:5113`。

### 生产构建

```bash
# 前端构建
cd Wes.UI && npm run build

# 后端构建
cd Wes.WebApi && dotnet publish -c Release
```

### Docker 部署

项目提供 Dockerfile，将前后端打包到一个镜像中：
```bash
# 先完成前端 build 和后端 publish
docker build -t wes-admin ./Wes.WebApi
docker run -d -p 80:80 wes-admin
```

---

## 默认账户

账号：admin
密码：123456

---

## 界面预览

> TODO: 补充界面截图

---

## 开发计划

- [x] RBAC 权限管理
- [x] 可视化工作流引擎
- [x] 代码生成器（已升级为 AI 智能开发）
- [x] 国际化（中/英）
- [x] 暗黑模式
- [x] 滑块验证码
- [x] 许可证授权机制
- [x] 数据服务 API 编排
- [x] 钉钉/企微/飞书三方集成（登录 + 组织同步）
- [x] 邮件集成
- [ ] 数据服务请求日志功能
- [ ] TODO: 后续计划补充

---

## AI 智能开发

推荐使用 AI 编程助手（如 [CodeBuddy](https://www.codebuddy.ai)）替代传统代码生成器，开发效率大幅提升：

- **智能编码** — AI 理解项目上下文，自动生成 Model / Service / Biz / Controller / Vue 组件
- **代码重构** — 一键重构、代码审查、性能优化建议
- **Bug 修复** — 智能定位问题，快速修复
- **知识问答** — 项目架构、技术选型随时答疑
- **自然语言驱动** — 用自然语言描述需求，AI 即可完成开发任务

---

## 贡献指南

欢迎提交 Issue 和 Pull Request。

---

## 鸣谢

本项目基于 [RuoYi](https://gitee.com/y_project/RuoYi-Vue) 开发，感谢若依团队的开源贡献。

---

## 许可证

本项目基于 [MIT License](LICENSE) 开源协议发布。
