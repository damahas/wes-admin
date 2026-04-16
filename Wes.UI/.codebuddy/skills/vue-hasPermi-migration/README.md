# Vue v-hasPermi Directive Migration Skill

## 概述

这个 Skill 帮助将 Ruoyi 风格的 Vue 3 项目中的 `v-hasPermi` 自定义指令迁移到其他 Vue 3 项目中，实现基于权限的元素可见性控制。

## 功能

- 自动化权限指令迁移流程
- 提供完整的文件模板和示例代码
- 支持多种状态管理方案（Pinia/Vuex）
- 包含常见问题解决方案

## 使用场景

- 从 Ruoyi 框架迁移权限系统到新项目
- 在新项目中实现基于 RBAC 的 UI 元素权限控制
- 集成角色和权限管理功能

## 包含资源

- **SKILL.md**: 完整的迁移工作流程指导
- **assets/**:
  - `hasPermi.js`: 核心指令实现
  - `directive-index.js`: 指令注册文件
  - `user-store.js`: Pinia 用户状态管理模板
  - `main.js`: 入口文件集成示例
  - `usage-example.vue`: 使用示例组件

## 使用方法

1. 将 skill 放置到目标项目的 `.codebuddy/skills/` 目录
2. 在对话中提及需要迁移 `v-hasPermi` 指令
3. 按照工作流程逐步完成迁移

## 权限数据格式

权限应存储为字符串数组：
```javascript
permissions: [
  'system:user:add',
  'system:user:edit',
  'system:user:delete'
]
```

特殊权限：
- `*:*:*` - 超级管理员，匹配所有权限

## 技术要求

- Vue 3.x
- Pinia（推荐）或 Vuex
- Vite 或 Webpack
