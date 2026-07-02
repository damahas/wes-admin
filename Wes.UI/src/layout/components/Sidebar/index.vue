<template>
  <el-aside :width="isCollapse ? '52px' : '264px'" class="sidebar">
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
    <div class="collapse-btn" :class="{ collapsed: isCollapse }" @click="toggleCollapse">
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
  background-color: var(--bg-sidebar);
  border-right: 1px solid var(--border-color);
}

.menu-container {
  flex: 1;
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
  height: 44px;
  display: flex;
  align-items: center;
  cursor: pointer;
  color: var(--menu-item-color);
  font-size: 14px;
  font-weight: 500;
  padding-left: 20px;
  border-top: 1px solid var(--border-color);

  .collapse-icon {
    margin-left: 18px;
  }
}

.collapse-btn.collapsed {
  padding-left: 0;
  justify-content: center;
}

.sidebar :deep(.el-menu) {
  border: none;
  box-shadow: none;
  padding: 5px 16px 0;
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
  border-radius: 8px;
  color: var(--menu-item-color);
  font-size: 14px;
  font-weight: 500;
  display: flex;
  align-items: center;
  padding-left: 20px !important;
}

.sidebar :deep(.el-menu-item .menu-icon) {
  color: var(--menu-item-icon-color);
  font-size: 16px;
}

.sidebar :deep(.el-menu-item:hover) {
  background-color: var(--menu-item-active-bg);
  color: var(--menu-item-active-color);
}

.sidebar :deep(.el-menu-item.is-active) {
  background-color: var(--menu-item-active-bg);
  color: var(--menu-item-active-color);
  font-weight: 600;
}

.sidebar :deep(.el-menu-item.is-active .menu-icon) {
  color: var(--menu-item-active-icon-color);
}

.sidebar :deep(.el-sub-menu__title) {
  height: var(--menu-item-height);
  line-height: var(--menu-item-height);
  margin-bottom: var(--menu-item-gap);
  border-radius: 8px;
  color: var(--menu-item-color);
  font-size: 14px;
  font-weight: 500;
  position: relative;
  padding-left: 20px !important;
}

.sidebar :deep(.el-sub-menu__title .el-sub-menu__icon-arrow) {
  position: absolute;
  right: 16px;
  top: 50%;
  transform: translateY(-50%);
  color: var(--menu-item-icon-color);
}

.sidebar :deep(.el-sub-menu__title .menu-icon) {
  color: var(--menu-item-icon-color);
  margin-right: 5px;
  width: var(--el-menu-icon-width);
  font-size: 16px;
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

/* 二级菜单缩进 */
.sidebar :deep(.el-sub-menu .el-menu-item) {
  padding-left: 36px !important;
  height: 40px;
  line-height: 40px;
  font-weight: 400;
}

.sidebar :deep(.el-sub-menu .el-menu .el-sub-menu__title) {
  padding-left: 36px !important;
}

/* 三级菜单缩进 */
.sidebar :deep(.el-sub-menu .el-sub-menu .el-menu-item) {
  padding-left: 52px !important;
}

.sidebar :deep(.el-sub-menu .el-menu .el-sub-menu .el-menu .el-sub-menu__title) {
  padding-left: 52px !important;
}

/* 四级菜单缩进 */
.sidebar :deep(.el-sub-menu .el-sub-menu .el-sub-menu .el-menu-item) {
  padding-left: 68px !important;
}

.sidebar :deep(.menu-icon) {
  color: var(--menu-item-icon-color);
  font-size: 16px;
  margin-right: 5px;
  text-align: center;
  vertical-align: middle;
  width: var(--el-menu-icon-width);
}

.el-menu--collapse {
  width: 100%;
}

/* 收起状态：只显示图标，居中 */
.sidebar :deep(.el-menu--collapse .el-menu-item),
.sidebar :deep(.menu-collapsed .el-menu-item) {
  padding: 0 !important;
  justify-content: center !important;
}

.sidebar :deep(.el-menu--collapse .el-menu-item .el-tooltip__trigger),
.sidebar :deep(.menu-collapsed .el-menu-item .el-tooltip__trigger) {
  display: flex;
  justify-content: center;
  align-items: center;
  width: 100%;
}

.sidebar :deep(.el-menu--collapse .el-menu-item > *),
.sidebar :deep(.menu-collapsed .el-menu-item > *) {
  margin-left: 0 !important;
  margin-right: 0 !important;
}

.sidebar :deep(.el-menu--collapse .el-menu-item span),
.sidebar :deep(.menu-collapsed .el-menu-item span) {
  display: none;
}

.sidebar :deep(.el-menu--collapse .el-sub-menu__title),
.sidebar :deep(.menu-collapsed .el-sub-menu__title) {
  padding: 0 !important;
  justify-content: center !important;
}

.sidebar :deep(.el-menu--collapse .el-sub-menu__title > *),
.sidebar :deep(.menu-collapsed .el-sub-menu__title > *) {
  margin-left: 0 !important;
  margin-right: 0 !important;
}

.sidebar :deep(.el-menu--collapse .el-sub-menu__title span),
.sidebar :deep(.menu-collapsed .el-sub-menu__title span) {
  display: none;
}

.sidebar :deep(.el-menu--collapse .el-sub-menu__icon-arrow),
.sidebar :deep(.menu-collapsed .el-sub-menu__icon-arrow) {
  display: none;
}

.sidebar :deep(.el-menu--collapse .menu-icon),
.sidebar :deep(.menu-collapsed .menu-icon) {
  margin-right: 0;
  width: auto;
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

.sidebar-menu-popper .el-sub-menu__title {
  height: var(--menu-item-height);
  line-height: var(--menu-item-height);
  margin: 0;
  border-radius: 4px;
  color: var(--menu-item-color);
  font-size: 14px;
  padding: 0 20px;
}

.sidebar-menu-popper .el-sub-menu__title .menu-icon {
  color: var(--menu-item-icon-color);
}

.sidebar-menu-popper .el-sub-menu__title:hover {
  background-color: var(--menu-item-active-bg);
  color: var(--menu-item-active-color);
}

.sidebar-menu-popper .el-sub-menu.is-active > .el-sub-menu__title {
  color: var(--menu-item-active-color);
}

.sidebar-menu-popper .el-sub-menu .el-menu {
  background-color: transparent;
}

.sidebar-menu-popper .el-sub-menu__icon-arrow {
  color: var(--menu-item-icon-color);
}
</style>
