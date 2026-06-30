<template>
  <div v-if="!menu.hidden">
    <template
      v-if="
        hasOneShowingChild(menu.children, menu) &&
        (!onlyOneChild.children || onlyOneChild.noShowingChildren) &&
        !menu.alwaysShow
      "
    >
      <el-menu-item v-if="onlyOneChild.meta" :index="resolvePath(onlyOneChild.path)">
        <i :class="'menu-icon fa fa-fw fa-' + menu.meta.icon"></i>
        <!-- <el-icon>
          <component
            :is="getIconComponent(onlyOneChild.meta.icon || (menu.meta && menu.meta.icon))"
          />
        </el-icon> -->
        <template #title>
          <span>{{ onlyOneChild.meta.title }}</span>
        </template>
      </el-menu-item>
    </template>

    <el-sub-menu v-else ref="subMenu" :index="resolvePath(menu.path)" teleported>
      <template v-if="menu.meta" #title>
        <i :class="'menu-icon fa fa-fw fa-' + menu.meta.icon"></i>
        <!-- <el-icon>
          <component :is="getIconComponent(menu.meta.icon)" />
        </el-icon> -->
        <span>{{ menu.meta.title }}</span>
      </template>
      <menu-item
        v-for="(child, index) in menu.children"
        :key="child.path + index"
        :is-nest="true"
        :menu="child"
        :collapse="collapse"
        :base-path="resolvePath(menu.path)"
      />
    </el-sub-menu>
  </div>
</template>

<script setup>
import { ref } from "vue";
import * as ElementPlusIconsVue from "@element-plus/icons-vue";

const props = defineProps({
  menu: {
    type: Object,
    required: true,
  },
  isNest: {
    type: Boolean,
    default: false,
  },
  basePath: {
    type: String,
    default: "",
  },
  collapse: {
    type: Boolean,
    default: false,
  },
});

const onlyOneChild = ref({});

function hasOneShowingChild(children = [], parent) {
  if (!children) {
    children = [];
  }
  const showingChildren = children.filter((item) => {
    if (item.hidden) {
      return false;
    } else {
      onlyOneChild.value = item;
      return true;
    }
  });

  if (showingChildren.length === 1) {
    return true;
  }

  if (showingChildren.length === 0) {
    onlyOneChild.value = { ...parent, noShowingChildren: true };
    return true;
  }

  return false;
}

function resolvePath(routePath) {
  if (routePath?.[0] == "/") {
    return routePath;
  }
  if (props.basePath) {
    return props.basePath + "/" + routePath;
  }
  return routePath;
}
</script>
