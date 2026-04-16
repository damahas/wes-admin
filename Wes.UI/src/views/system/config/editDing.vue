<template>
  <el-form-item label="钉钉接口地址" required>
    <el-input v-model="form.dingPath" @change="handleChange"></el-input>
  </el-form-item>
  <el-form-item label="AgentId" required>
    <el-input v-model="form.agentId" @change="handleChange"></el-input>
  </el-form-item>
  <el-form-item label="AppKey" required>
    <el-input v-model="form.appKey" @change="handleChange"></el-input>
  </el-form-item>
  <el-form-item label="AppSecret" required>
    <el-input v-model="form.appSecret" @change="handleChange"></el-input>
  </el-form-item>
</template>

<script setup>
import { ref, computed, watch } from "vue";
const props = defineProps({
  modelValue: String,
});
const emits = defineEmits(["update:modelValue"]);
const form = ref({
  dingPath: "https://api.dingtalk.com",
  agentId: "",
  appKey: "",
  appSecret: "",
});

watch(
  () => props.modelValue,
  () => {
    if (!props.modelValue) {
      form.value = {
        dingPath: "https://api.dingtalk.com",
        agentId: "",
        appKey: "",
        appSecret: "",
      };
      return;
    }
    form.value = JSON.parse(props.modelValue);
  },
  { deep: true, immediate: true }
);

const handleChange = () => {
  emits("update:modelValue", JSON.stringify(form.value));
};
</script>

