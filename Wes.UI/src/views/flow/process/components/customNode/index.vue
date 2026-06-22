<template>
  <div
    class="flow-node-vue"
    :style="{
      width: size.width + 'px',
      height: size.height + 'px',
    }"
    :title="data?.meta?.name || `节点`"
  >
    <div class="flow-node-icon" :style="{ backgroundColor: data?.color || '#409eff' }">
      <i :class="'fa ' + (data?.icon || 'fa-tasks')"></i>
    </div>
    <span class="flow-node-text">{{ data?.meta?.name || "节点" }}</span>
  </div>
</template>

<script setup>
import { computed, reactive } from "vue";

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
</style>
