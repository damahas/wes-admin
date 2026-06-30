# Vue 组件使用指南

本文档收录项目中常用的 Vue 组件使用示例。

## 目录

- [ContextMenu 右键菜单](#contextmenu-右键菜单)

---

## ContextMenu 右键菜单

### 基本用法

```vue
<template>
  <div>
    <div @contextmenu.prevent="handleRightClick($event)">
      右键点击此处
    </div>
    
    <ContextMenu
      v-model:visible="contextMenuVisible"
      :x="contextMenuX"
      :y="contextMenuY"
      :menus="contextMenus"
      :min-width="200"
      @select="handleContextSelect"
    />
  </div>
</template>

<script setup>
import { ref } from "vue";
import ContextMenu from "@/components/ContextMenu/index.vue";

const contextMenuVisible = ref(false);
const contextMenuX = ref(0);
const contextMenuY = ref(0);
const contextMenus = ref([]);

function handleRightClick(e) {
  contextMenuX.value = e.clientX;
  contextMenuY.value = e.clientY;
  contextMenus.value = [
    {
      key: "edit",
      label: "编辑",
      icon: "fa fa-edit",
      action: () => handleEdit(),
    },
    {
      key: "delete",
      label: "删除",
      icon: "fa fa-trash",
      action: () => handleDelete(),
    },
  ];
  contextMenuVisible.value = true;
}

function handleContextSelect(item) {
  console.log("选中菜单:", item.key);
}
</script>
```

### Props

| 参数 | 说明 | 类型 | 默认值 |
|------|------|------|--------|
| visible | 菜单显示状态 | `boolean` | `false` |
| x | 菜单显示 X 坐标 | `number` | `0` |
| y | 菜单显示 Y 坐标 | `number` | `0` |
| menus | 菜单项配置 | `MenuItem[]` | `[]` |
| minWidth | 菜单最小宽度 | `number` | `200` |

### MenuItem 配置

| 参数 | 说明 | 类型 |
|------|------|------|
| key | 菜单唯一标识 | `string` |
| label | 菜单显示文字 | `string` |
| icon | 图标类名（FontAwesome） | `string` |
| disabled | 是否禁用 | `boolean` |
| action | 点击回调函数 | `Function` |

### Events

| 事件名 | 说明 | 回调参数 |
|--------|------|----------|
| select | 菜单选中事件 | `(item: MenuItem)` |
| update:visible | 菜单显示状态变更 | `(visible: boolean)` |

### 自定义菜单内容

```vue
<ContextMenu
  v-model:visible="contextMenuVisible"
  :x="contextMenuX"
  :y="contextMenuY"
  :min-width="200"
>
  <div class="custom-menu-item" @click="handleCopy">
    <i class="fa fa-copy"></i>
    <span>复制</span>
  </div>
  <div class="custom-menu-item" @click="handlePaste">
    <i class="fa fa-paste"></i>
    <span>粘贴</span>
  </div>
</ContextMenu>
```

### 与 Graph/X6 结合使用

```javascript
// X6 图形右键菜单
graph.on("node:contextmenu", ({ e, node }) => {
  contextMenuX.value = e.clientX;
  contextMenuY.value = e.clientY;
  contextMenus.value = [
    {
      key: "delete",
      label: "删除节点",
      icon: "fa fa-trash",
      action: () => node.remove(),
    },
    {
      key: "copy",
      label: "复制节点",
      icon: "fa fa-copy",
      action: () => handleCopyNode(node),
    },
  ];
  contextMenuVisible.value = true;
});
```

---

## 待添加组件

- [ ] ElTable 表格封装
- [ ] SearchForm 搜索表单
- [ ] DictSelect 数据字典下拉
- [ ] IconSelect 图标选择器
