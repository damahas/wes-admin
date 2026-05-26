<template>
  <el-radio-group
    v-model="radioValue"
    :class="{ 'radio-block': !props.isRow, 'is-readonly': readonly }"
    @change="readonly && (radioValue = props.value)"
  >
    <el-radio
      v-for="item in props.options"
      :key="item.key"
      :value="item.key"
      :class="{ 'radio-block': !props.isRow }"
    >
      {{ item.label }}
    </el-radio>
  </el-radio-group>
</template>

<script setup>
import { computed, ref, watch } from "vue";

const props = defineProps({
  value: String,
  isRow: {
    type: Boolean,
    default: false,
  },
  options: {
    type: Array,
    default: () => [],
  },
  readonly: {
    type: Boolean,
    default: false,
  },
});

const emit = defineEmits(["updateValue"]);

const radioValue = ref(props.value);

watch(
  () => props.value,
  (val) => {
    radioValue.value = val;
  }
);

watch(radioValue, (val) => {
  emit("updateValue", val);
});
</script>

<style lang="scss" scoped>
.radio-block {
  display: block;
}

// 只读状态样式
.is-readonly {
  :deep(.el-radio) {
    cursor: not-allowed;
  }

  :deep(.el-radio__input.is-checked .el-radio__inner) {
    background-color: #409eff;
    border-color: #409eff;
  }

  :deep(.el-radio__input.is-checked + .el-radio__label) {
    color: #303133;
  }
}
</style>
