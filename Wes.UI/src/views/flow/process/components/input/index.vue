<template>
  <el-input
    v-model="inputValue"
    :type="props.type"
    :rows="props.rows"
    :readonly="readonly"
    :disabled="readonly"
  ></el-input>
</template>

<script setup>
import { ref, watch } from "vue";

const props = defineProps({
  value: String,
  type: {
    type: String,
    default: "text",
  },
  rows: {
    type: Number,
    default: 3,
  },
  readonly: {
    type: Boolean,
    default: false,
  },
});

const emit = defineEmits(["updateValue"]);

const inputValue = ref(props.value);

watch(
  () => props.value,
  (val) => {
    inputValue.value = val;
  }
);

watch(inputValue, (val) => {
  emit("updateValue", val);
});
</script>
