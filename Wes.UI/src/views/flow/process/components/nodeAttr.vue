<template>
  <div class="node-attr">
    <div class="attr-header">
      <i class="fa fa-cog"></i>
      <span>{{ traitData.label }}</span>
    </div>
    <div class="attr-content">
      <div v-for="comp in shownComponents" :key="comp.label" class="form-item">
        <label class="form-label">{{ comp.label }}</label>
        <div class="form-control">
          <component
            :is="allComponents[comp.type]"
            :value="getValue(comp.param)"
            v-bind="{ ...comp.props, ...(comp.type === 'conditionSelect' || comp.type === 'conditions' ? { element, graphGetter } : {}) }"
            :readonly="readonly"
            @updateValue="(val) => !readonly && updateValue(comp.param, val)"
          />
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { computed, watch } from "vue";
import { useI18n } from "vue-i18n";
import allComponents from "./index.js";
import { useFlowConfig } from "../config/index.js";
import { get as _get, set as _set } from "lodash";

const { t } = useI18n();
const { getElementTrait } = useFlowConfig(t);

const props = defineProps({
  type: String,
  element: {
    type: [Object],
    default: () => ({}),
  },
  readonly: {
    type: Boolean,
    default: false,
  },
  // 获取 X6 Graph 实例（供 conditionSelect 查询分支节点条件 / 执行互换）
  graphGetter: {
    type: Function,
    default: () => null,
  },
});

const traitData = computed(() => {
  return getElementTrait(props.type);
});

// 仅展示当前元素适用的组件
// - 边：源节点非分支时隐藏 conditionSelect（需求：只有分支边才能选条件）
const shownComponents = computed(() => {
  const comps = traitData.value?.components || [];
  if (!isEdge(props.element)) return comps;
  const graph = props.graphGetter?.();
  const srcId = props.element.getSourceCellId?.();
  const src = srcId ? graph?.getCellById?.(srcId) : null;
  if (!src || src.getData?.()?.type !== "branch") {
    return comps.filter((c) => c.type !== "conditionSelect");
  }
  return comps;
});

// 判断是否为 X6 边：边的数据需通过 getData/setData 访问，且为扁平结构
function isEdge(el) {
  return el && typeof el.getData === "function" && typeof el.setData === "function";
}

function getValue(param) {
  if (isEdge(props.element)) {
    return _get(props.element.getData(), param);
  }
  return _get(props.element, param);
}

function updateValue(param, value) {
  if (isEdge(props.element)) {
    const data = props.element.getData() || {};
    _set(data, param, value);
    props.element.setData(data);
    refreshEdgeLabel(props.element);
    return;
  }
  _set(props.element, param, value);
}

// 连线标签：优先显示分支条件名，其次连线名称
function refreshEdgeLabel(edge) {
  const data = edge.getData() || {};
  let text = data.name || "";
  if (data.conditionId) {
    const graph = props.graphGetter?.();
    const src = graph?.getCellById?.(edge.getSourceCellId?.());
    const conds = src?.getData?.()?.meta?.conditions || [];
    const cond = conds.find((c) => c.id === data.conditionId);
    if (cond) text = cond.name;
  }
  if (text) edge.setLabels([{ attrs: { text: { text } } }]);
  else edge.setLabels([]);
}

defineOptions({
  name: "NodeAttr",
});
</script>

<style lang="scss" scoped>
.node-attr {
  height: 100%;
  background-color: var(--bg-card);
  border-left: 1px solid var(--border-color);
  display: flex;
  flex-direction: column;
}

.attr-header {
  display: flex;
  align-items: center;
  gap: 8px;
  padding: 12px 16px;
  font-size: 14px;
  font-weight: 700;
  color: var(--text-title);
  background-color: var(--bg-hover);
  border-bottom: 1px solid var(--border-color-light);

  > i {
    color: var(--theme-color);
  }
}

.attr-content {
  flex: 1;
  overflow-y: auto;
  padding: 12px 16px;
}

.form-item {
  margin-bottom: 19px;
  position: relative;

  &:last-child {
    margin-bottom: 0;
  }
}

.form-label {
  display: block;
  margin-bottom: 12px;
  font-size: 14px;
  color: var(--text-secondary);
}

.form-control {
  :deep(.el-input__inner) {
    font-size: 13px;
  }

  :deep(.el-select) {
    width: 100%;
  }
}
</style>
