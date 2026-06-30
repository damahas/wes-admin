<template>
  <div>
    <!-- 右键触发区域 -->
    <div class="trigger-area" @contextmenu.prevent="handleRightClick($event)">
      <p>在此区域右键点击查看菜单</p>
    </div>

    <!-- 右键菜单组件 -->
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
      key: "view",
      label: "查看详情",
      icon: "fa fa-eye",
      action: () => console.log("查看详情"),
    },
    {
      key: "edit",
      label: "编辑",
      icon: "fa fa-edit",
      action: () => console.log("编辑"),
    },
    {
      key: "delete",
      label: "删除",
      icon: "fa fa-trash",
      action: () => console.log("删除"),
    },
  ];
  contextMenuVisible.value = true;
}

function handleContextSelect(item) {
  console.log("选中的菜单项:", item.key);
}
</script>

<style scoped>
.trigger-area {
  width: 300px;
  height: 200px;
  border: 1px dashed #ccc;
  display: flex;
  align-items: center;
  justify-content: center;
  cursor: context-menu;
}
</style>
