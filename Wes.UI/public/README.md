# Public 目录说明

本目录用于存放不需要经过 webpack/vite 处理的静态资源文件。

## 目录结构

```
public/
├── favicon.ico         # 网站图标
├── images/            # 静态图片资源
│   ├── logo/        # Logo图片
│   └── common/      # 通用图片
└── icons/           # 自定义图标资源
    └── svg/        # SVG图标
```

## 使用说明

### 访问方式

`public` 目录中的文件会直接复制到构建输出目录的根目录，可以通过根路径访问：

```html
<!-- 在模板中使用 -->
<img src="/images/logo/logo.png" alt="Logo" />

<!-- 或者 -->
<link rel="icon" href="/favicon.ico">
```

### 适合放置的文件

- 网站图标 (favicon.ico)
- robots.txt
- manifest.json (PWA配置)
- 大型静态资源（如PDF、视频等）
- 不需要编译处理的图片和图标

### 与 assets 目录的区别

| 目录 | 特点 | 适用场景 |
|------|------|----------|
| `public/` | 直接复制到输出目录，不经过处理 | favicon、robots.txt、大型静态资源 |
| `src/assets/` | 经过构建工具处理，支持优化 | 组件中使用的图片、字体、需要压缩的SVG |

## 示例

```vue
<template>
  <!-- 使用 public 目录资源 -->
  <img src="/images/common/placeholder.jpg" alt="占位图" />
  
  <!-- 使用 assets 目录资源 -->
  <img src="@/assets/images/logo/logo.png" alt="Logo" />
</template>
```
