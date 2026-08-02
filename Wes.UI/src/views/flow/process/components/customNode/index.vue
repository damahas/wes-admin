<template>
  <!-- 分支节点：黄色菱形 -->
  <div
    v-if="isBranch"
    class="flow-node-branch"
    :style="{
      width: size.width + 'px',
      height: size.height + 'px',
    }"
    :title="data?.meta?.name || t('flow.designer.node.branch')"
  >
    <div class="branch-diamond" :style="{ backgroundColor: data?.color || '#f0b04a' }">
      <i :class="'fa ' + (data?.icon || 'fa-code-fork')"></i>
    </div>
  </div>
  <!-- 普通节点：矩形 -->
  <div
    v-else
    class="flow-node-vue"
    :style="{
      width: size.width + 'px',
      height: size.height + 'px',
    }"
    :title="data?.meta?.name || t('flow.designer.node.general')"
  >
    <div class="flow-node-icon" :style="{ backgroundColor: data?.color || '#409eff' }">
      <i :class="'fa ' + (data?.icon || 'fa-tasks')"></i>
    </div>
    <span class="flow-node-text">{{ data?.meta?.name || t('flow.designer.node.general') }}</span>
  </div>
</template>

<script setup>
import { computed, reactive } from "vue";
import i18n from "@/locales";

// X6 的 vue-shape 用独立 createApp 挂载节点组件，不继承主 app 的 i18n 注入，
// 因此这里不能使用 useI18n()，改用全局 i18n 实例的 t。
const t = i18n.global.t;

const props = defineProps({
  node: {
    type: Object,
  },
  graph: {
    type: Object,
  },
  options: {
    type: Object,
  },
});

const size = computed(() => props.node?.getSize() || { width: 120, height: 40 });
const data = computed(() => reactive(props.node?.getData()));
const isBranch = computed(() => data.value?.type === "branch");
</script>

<style scoped>
.flow-node-vue {
  display: flex;
  align-items: center;
  gap: 6px;
  padding: 0 10px;
  border-radius: 6px;
  box-sizing: border-box;
  cursor: move;
  background-color: var(--bg-card);
  border: 1px solid var(--border-color);
  box-shadow: 0 2px 4px var(--shadow-color);
}

.flow-node-icon {
  display: flex;
  align-items: center;
  justify-content: center;
  width: 20px;
  height: 20px;
  border-radius: 4px;
  font-size: 12px;
  color: #fff;
}

.flow-node-text {
  flex: 1;
  font-size: 12px;
  color: var(--text-title);
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
}

/* 分支节点菱形：旋转方块 + 圆角，缩小尺寸 */
.flow-node-branch {
  display: flex;
  align-items: center;
  justify-content: center;
  box-sizing: border-box;
  cursor: move;
}

.branch-diamond {
  width: 70%;
  height: 70%;
  display: flex;
  align-items: center;
  justify-content: center;
  border-radius: 6px;
  transform: rotate(45deg);
  filter: drop-shadow(0 2px 4px var(--shadow-color));
}

.branch-diamond > i {
  color: #fff;
  font-size: 18px;
  transform: rotate(-45deg);
  text-shadow: 0 1px 2px rgba(0, 0, 0, 0.35);
}
</style>
