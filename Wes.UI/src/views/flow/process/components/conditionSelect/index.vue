<template>
  <div v-if="visible" class="condition-select">
    <el-select
      v-model="currentId"
      :placeholder="t('flow.designer.selectCondition')"
      :disabled="readonly || !conditions.length"
      style="width: 100%"
      @change="onChange"
    >
      <el-option :value="''" :label="t('flow.designer.noCondition')" />
      <el-option
        v-for="cond in conditions"
        :key="cond.id"
        :value="cond.id"
        :label="cond.name || cond.id"
      />
    </el-select>
  </div>
</template>

<script setup>
import { ref, computed, watch } from "vue";
import { useI18n } from "vue-i18n";

const { t } = useI18n();

const props = defineProps({
  // 当前选中的边（X6 Edge 对象）
  element: { type: Object, default: null },
  // 获取 X6 Graph 实例的函数
  graphGetter: { type: Function, default: () => null },
  readonly: { type: Boolean, default: false },
  // nodeAttr 透传的 value（边数据走 setData，此处不使用）
  value: { type: [String, Number], default: "" },
});

const emit = defineEmits(["updateValue"]);

// 读取边数据
function getEdgeData() {
  return (props.element?.getData?.() || {});
}

// 源节点
function getSourceNode() {
  const graph = props.graphGetter?.();
  if (!graph || !props.element) return null;
  const sourceId = props.element.getSourceCellId?.();
  return sourceId ? graph.getCellById(sourceId) : null;
}

// 源分支节点的条件列表
const conditions = computed(() => {
  const node = getSourceNode();
  if (!node) return [];
  const data = node.getData?.() || {};
  if (data.type !== "branch") return [];
  return data.meta?.conditions || [];
});

// 仅当源节点为分支节点时显示
const visible = computed(() => {
  const node = getSourceNode();
  if (!node) return false;
  return (node.getData?.() || {}).type === "branch";
});

const currentId = ref("");

function syncCurrent() {
  currentId.value = getEdgeData().conditionId || "";
}
syncCurrent();

// 切换到不同边时重新同步已选条件
watch(() => props.element, () => syncCurrent());

// 条件名称
function conditionName(id) {
  if (!id) return "";
  return conditions.value.find((c) => c.id === id)?.name || "";
}

// 刷新边标签：有条件显示条件名，否则显示连线名
function refreshLabel(edge) {
  if (!edge) return;
  const data = edge.getData?.() || {};
  const text = conditionName(data.conditionId) || data.name || "";
  if (text) {
    edge.setLabels([{ attrs: { text: { text } } }]);
  } else {
    edge.setLabels([]);
  }
}

function onChange(newId) {
  if (props.readonly) return;
  const graph = props.graphGetter?.();
  if (!graph || !props.element) return;

  const oldId = getEdgeData().conditionId || "";

  // 设置当前边条件
  props.element.setData({ ...getEdgeData(), conditionId: newId });
  refreshLabel(props.element);

  // 互斥：同一条件只能出现在一条出边上，冲突则互换
  if (newId && newId !== oldId) {
    const sourceId = props.element.getSourceCellId?.();
    const sibling = graph
      .getEdges()
      .find(
        (e) =>
          e.id !== props.element.id &&
          e.getSourceCellId?.() === sourceId &&
          (e.getData?.() || {}).conditionId === newId
      );
    if (sibling) {
      sibling.setData({ ...(sibling.getData?.() || {}), conditionId: oldId });
      refreshLabel(sibling);
    }
  }

  // 通知 nodeAttr（仅同步展示，持久化已由 setData 完成）
  emit("updateValue", newId);
}
</script>

<style lang="scss" scoped>
.condition-select {
  display: flex;
  flex-direction: column;
  gap: 4px;
}
</style>
