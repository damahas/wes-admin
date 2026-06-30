<template>
  <div class="node-attr">
    <div class="attr-header">
      <i class="fa fa-cog"></i>
      <span>{{ traitData.label }}</span>
    </div>
    <div class="attr-content">
      <div v-for="comp in traitData.components" :key="comp.label" class="form-item">
        <label class="form-label">{{ comp.label }}</label>
        <div class="form-control">
          <component
            :is="allComponents[comp.type]"
            :value="getValue(comp.param)"
            v-bind="comp.props"
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
import allComponents from "./index.js";
import { getElementTrait } from "../config/index.js";
import { get as _get, set as _set } from "lodash";

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
});

const traitData = computed(() => {
  return getElementTrait(props.type);
});

function getValue(param) {
  return _get(props.element, param);
}

function updateValue(param, value) {
  _set(props.element, param, value);
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
