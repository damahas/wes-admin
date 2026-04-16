<template>
  <el-tree-select
    v-model="selectedValue"
    :data="treeOptions"
    node-key="value"
    default-expand-all
    @change="handleChange"
  />
</template>

<script setup>
import { computed } from "vue";
import { handleTree } from "@/utils";

const props = defineProps({
  options: {
    type: Array,
    default: () => [],
  },
  modelValue: {
    type: [String, Number, Array],
    default: "",
  },
});

const emit = defineEmits(["update:modelValue", "change"]);

const treeOptions = computed(() => {
  return handleTree(props.options);
});

const selectedValue = computed({
  get() {
    return props.modelValue;
  },
  set(value) {
    emit("update:modelValue", value);
    emit("change", value);
  },
});

function handleChange(value) {
  emit("change", value);
}
</script>

<style scoped>
.el-tag + .el-tag,
.el-tag + span,
span + .el-tag {
  margin-left: 8px;
}
</style>
