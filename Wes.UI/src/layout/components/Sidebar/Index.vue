<template>
  <el-aside :width="isCollapse ? '52px' : '264px'" class="sidebar">
    <div class="logo">
      <span v-if="!isCollapse">WesAdmin</span>
      <span v-else>WA</span>
    </div>
    <div class="menu-container">
      <el-menu
        :default-active="activeMenu"
        :collapse="isCollapse"
        :collapse-transition="false"
        router
        background-color="transparent"
        text-color="var(--text-primary)"
        active-text-color="var(--theme-color)"
        :class="{ 'menu-collapsed': isCollapse }"
        popper-class="sidebar-menu-popper"
      >
        <el-menu-item index="/home">
          <i class="menu-icon fa fa-fw fa-house"></i>
          <template #title>{{ t("menu.home") }}</template>
        </el-menu-item>
        <MenuItem
          v-for="item in menuList"
          :collapse="isCollapse"
          :key="item.path"
          :menu="item"
        />
      </el-menu>
    </div>
    <div class="collapse-btn" @click="toggleCollapse">
      <i v-if="!isCollapse" class="menu-icon fa fa-fw fa-outdent"></i>
      <i v-else class="menu-icon fa fa-fw fa-indent"></i>
      <span v-if="!isCollapse">{{ t("common.collapse") }}</span>
    </div>
  </el-aside>
</template>

<script setup>
import { computed } from "vue";
import { useRoute, useRouter } from "vue-router";
import { useI18n } from "vue-i18n";
import { useStore } from "vuex";
import { Fold, Expand, HomeFilled } from "@element-plus/icons-vue";
import MenuItem from "./MenuItem.vue";

defineProps({
  isCollapse: {
    type: Boolean,
    default: false,
  },
});

const emit = defineEmits(["toggle-collapse"]);

const route = useRoute();
const router = useRouter();
const { t } = useI18n();
const store = useStore();

const activeMenu = computed(() => route.path);

const menuList = computed(() => store.state.user.routes || []);

const toggleCollapse = () => {
  emit("toggle-collapse");
};
</script>

<style lang="scss" scoped>
.sidebar {
  border: none;
  box-shadow: none;
  transition: width 0.3s;
  display: flex;
  flex-direction: column;
  overflow-x: hidden;
}

.logo {
  height: 60px;
  display: flex;
  align-items: center;
  justify-content: center;
  color: var(--text-title);
  border: none;
  box-shadow: none;
  font-size: 20px;
  font-weight: 600;
  flex-shrink: 0;
}

.menu-container {
  height: calc(100vh - 60px - 40px);
  overflow-y: auto;
  overflow-x: hidden;

  /* 隐藏滚动条 */
  &::-webkit-scrollbar {
    width: 0;
    display: none;
  }

  /* Firefox */
  scrollbar-width: none;

  /* IE/Edge */
  -ms-overflow-style: none;
}

.collapse-btn {
  height: 40px;
  display: flex;
  align-items: center;
  cursor: pointer;
  color: var(--menu-item-color);
  font-size: 14px;
  font-weight: 500;

  .collapse-icon {
    margin-left: 18px;
  }
}

.sidebar :deep(.el-menu) {
  border: none;
  box-shadow: none;
  padding: 0 16px;
}

.sidebar :deep(.el-menu.menu-collapsed) {
  padding: 0 8px;
}

.sidebar :deep(.el-sub-menu .el-menu) {
  padding: 0;
  position: static;
}

.sidebar :deep(.el-menu-item) {
  height: var(--menu-item-height);
  line-height: var(--menu-item-height);
  margin-bottom: var(--menu-item-gap);
  border-radius: 4px;
  color: var(--menu-item-color);
  font-size: 14px;
  font-weight: 500;
  display: flex;
  align-items: center;
}

.sidebar :deep(.el-menu-item .menu-icon) {
  color: var(--menu-item-icon-color);
}

.sidebar :deep(.el-menu-item:hover) {
  background-color: var(--menu-item-active-bg);
}

.sidebar :deep(.el-menu-item.is-active) {
  background-color: var(--menu-item-active-bg);
  color: var(--menu-item-active-color);
}

.sidebar :deep(.el-menu-item.is-active .menu-icon) {
  color: var(--menu-item-active-icon-color);
}

.sidebar :deep(.el-sub-menu__title) {
  height: var(--menu-item-height);
  line-height: var(--menu-item-height);
  margin-bottom: var(--menu-item-gap);
  border-radius: 4px;
  color: var(--menu-item-color);
  font-size: 14px;
  font-weight: 500;
  position: relative;
}

.sidebar :deep(.el-sub-menu__title .el-sub-menu__icon-arrow) {
  position: absolute;
  right: 10px;
  top: 50%;
  transform: translateY(-50%);
}

.sidebar :deep(.el-sub-menu__title .menu-icon) {
  color: var(--menu-item-icon-color);
  margin-right: 5px;
  width: var(--el-menu-icon-width);
}

.sidebar :deep(.el-sub-menu__title:hover) {
  background-color: var(--menu-item-active-bg);
}

.sidebar :deep(.el-sub-menu.is-active > .el-sub-menu__title) {
  color: var(--menu-item-active-color);
}

.sidebar :deep(.el-sub-menu.is-active > .el-sub-menu__title .menu-icon) {
  color: var(--menu-item-active-icon-color);
}

// .sidebar :deep(.el-sub-menu .el-menu-item) {
//   padding-left: 48px !important;
//   padding-right: 0 !important;
//   width: 100%;
//   margin-right: -16px;
// }

// /* 三级菜单缩进 */
// .sidebar :deep(.el-sub-menu .el-sub-menu .el-menu-item) {
//   padding-left: 68px !important;
// }

// /* 四级菜单缩进 */
// .sidebar :deep(.el-sub-menu .el-sub-menu .el-sub-menu .el-menu-item) {
//   padding-left: 88px !important;
// }

.sidebar :deep(.menu-icon) {
  color: var(--menu-item-icon-color);
  font-size: 14px;
  margin-right: 5px;
  text-align: center;
  vertical-align: middle;
  width: var(--el-menu-icon-width);
}

.el-menu--collapse {
  width: 100%;
}

.sidebar :deep(.menu-collapsed .el-tooltip__trigger) {
  justify-content: center;
  align-items: center;
}
</style>

<style>
/* 缩起状态下的弹出子菜单样式 */
.sidebar-menu-popper .el-menu--popup {
  background-color: var(--bg-card) !important;
  border: none;
  /* box-shadow: 0 2px 12px rgba(0, 0, 0, 0.1); */
  border-radius: 4px;
  min-width: 200px;
  padding: 8px 0;
}

.sidebar-menu-popper .el-menu-item {
  height: var(--menu-item-height);
  line-height: var(--menu-item-height);
  margin: 0;
  border-radius: 4px;
  color: var(--menu-item-color);
  font-size: 14px;
  /* font-weight: 500; */
  padding: 0 20px;
}

.sidebar-menu-popper .el-menu-item .menu-icon {
  color: var(--menu-item-icon-color);
}

.sidebar-menu-popper .el-menu-item:hover {
  background-color: var(--menu-item-active-bg);
}

.sidebar-menu-popper .el-menu-item.is-active {
  background-color: var(--menu-item-active-bg);
  color: var(--menu-item-active-color);
}

.sidebar-menu-popper .el-menu-item.is-active .menu-icon {
  color: var(--menu-item-active-icon-color);
}
</style>
