<template>
  <el-switch
    style="position: absolute; right: 0; top: -2px"
    size="small"
    :model-value="value"
    :disabled="readonly"
    @change="!readonly && $emit('updateValue', $event)"
  >
  </el-switch>
</template>

<script setup>
import { ref, watch } from "vue";

const props = defineProps({
  value: Boolean,
  props: {
    type: Object,
    default: () => ({}),
  },
  readonly: {
    type: Boolean,
    default: false,
  },
});

const emit = defineEmits(["updateValue"]);

const switchValue = ref(props.value);

watch(
  () => props.value,
  (val) => {
    switchValue.value = val;
  }
);

watch(switchValue, (val) => {
  emit("updateValue", val);
});
</script>
