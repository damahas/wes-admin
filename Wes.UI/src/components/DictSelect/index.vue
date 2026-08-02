<template>
  <el-tree-select
    v-model="selectedValue"
    :data="treeOptions"
    :props="{ label: 'label', value: 'value', children: 'children' }"
    node-key="dictValue"
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
  // 深拷贝切断响应式引用，防止 handleTree 的 in-place children 赋值触发 Vue 循环更新
  const safe = JSON.parse(JSON.stringify(props.options));
  return handleTree(safe);
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
