<template>
  <div class="conditions-editor">
    <div v-if="!items.length && !readonly" class="cond-empty">
      {{ t('flow.designer.condition') }}
    </div>
    <div v-for="(cond, idx) in items" :key="cond.id" class="condition-item">
      <div class="condition-row">
        <el-input
          v-model="cond.name"
          :placeholder="t('flow.designer.conditionName')"
          :readonly="readonly"
          :disabled="readonly"
          :class="{ 'is-error': isDuplicate(idx) }"
          @input="emitChange"
        />
        <el-button
          v-if="!readonly"
          link
          type="danger"
          icon="Delete"
          @click="remove(idx)"
        />
      </div>
      <el-input
        v-model="cond.expr"
        type="textarea"
        :rows="2"
        :placeholder="t('flow.designer.conditionExprPlaceholder')"
        :readonly="readonly"
        :disabled="readonly"
        @input="emitChange"
      />
      <div v-if="isDuplicate(idx)" class="cond-error">
        {{ t('flow.designer.conditionNameDuplicate') }}
      </div>
    </div>
    <el-button
      v-if="!readonly"
      type="primary"
      link
      icon="Plus"
      @click="add"
    >
      {{ t('flow.designer.addCondition') }}
    </el-button>
  </div>
</template>

<script setup>
import { ref, watch } from "vue";
import { useI18n } from "vue-i18n";

const { t } = useI18n();

const props = defineProps({
  value: {
    type: Array,
    default: () => [],
  },
  readonly: {
    type: Boolean,
    default: false,
  },
  // 当前分支节点（X6 Node 对象），用于删除条件时重置引用该条件的出边
  element: { type: Object, default: null },
  // 获取 X6 Graph 实例的函数
  graphGetter: { type: Function, default: () => null },
});

const emit = defineEmits(["updateValue"]);

const clone = (arr) => (arr || []).map((c) => ({ ...c }));
const items = ref(clone(props.value));

// 仅当外部引用变化（切换节点/版本）时才重置，避免输入时重建对象导致失焦
watch(
  () => props.value,
  (v) => {
    if (v !== items.value) items.value = clone(v);
  }
);

let _seq = 0;
const genId = () => "c" + Date.now().toString(36) + (_seq++).toString(36) + Math.random().toString(36).slice(2, 5);

function add() {
  items.value.push({ id: genId(), name: "", expr: "" });
  emitChange();
}

function remove(idx) {
  // 重置引用此条件的所有出边
  const removedId = items.value[idx]?.id;
  if (removedId) {
    const graph = props.graphGetter?.();
    if (graph && props.element) {
      const edges = graph.getEdges();
      edges.forEach((e) => {
        const data = e.getData?.() || {};
        if (e.getSourceCellId?.() === props.element?.id && data.conditionId === removedId) {
          const newData = { ...data, conditionId: "" };
          e.setData(newData);
          // 边标签回退为连线名
          const text = newData.name || "";
          e.setLabels(text ? [{ attrs: { text: { text } } }] : []);
        }
      });
    }
  }
  items.value.splice(idx, 1);
  emitChange();
}

function emitChange() {
  // 直接回传内部数组引用，避免每次按键克隆导致输入框失焦
  emit("updateValue", items.value);
}

// 名称唯一性校验：返回某个下标是否为重复名称
function isDuplicate(idx) {
  const name = (items.value[idx]?.name || "").trim();
  if (!name) return false;
  return items.value.findIndex((c) => (c.name || "").trim() === name) !== idx;
}
</script>

<style lang="scss" scoped>
.conditions-editor {
  display: flex;
  flex-direction: column;
  gap: 10px;
}

.cond-empty {
  font-size: 13px;
  color: var(--text-secondary);
}

.condition-item {
  display: flex;
  flex-direction: column;
  gap: 6px;
  padding: 8px;
  border: 1px solid var(--border-color-light);
  border-radius: 4px;
  background-color: var(--bg-hover);
}

.condition-row {
  display: flex;
  align-items: center;
  gap: 6px;
}

.cond-error {
  font-size: 12px;
  color: var(--el-color-danger, #f56c6c);
}

:deep(.is-error .el-input__inner) {
  border-color: var(--el-color-danger, #f56c6c);
}
</style>
