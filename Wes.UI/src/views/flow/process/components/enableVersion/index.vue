<template>
  <el-switch
    size="small"
    :model-value="switchValue"
    :disabled="switchValue"
    :title="switchValue ? '已启用版本，无法修改' : '启用版本'"
    @change="handleChange"
  >
  </el-switch>
</template>

<script setup>
import { ref, watch } from "vue";
import { useVersion } from "@/api/flow/process";

const props = defineProps({
  value: String,
});

const handleChange = (val) => {
  if (val) {
    useVersion(props.value).then((res) => {});
    emit("updateValue", '');
  }
};

const emit = defineEmits(["updateValue"]);

const switchValue = ref(false);

watch(
  () => props.value,
  (val) => {
    if (!val) {
      switchValue.value = true;
    } else {
      switchValue.value = false;
    }
  }
);

// watch(switchValue, (val) => {
//   emit("updateValue", val);
// });
</script>
