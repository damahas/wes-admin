<template>
  <el-dialog :title="title" v-model="open" :width="dialogWidth" append-to-body>
    <el-form ref="configRef" :model="form" :rules="rules" label-width="128px">
      <el-form-item label="参数名称" prop="configName">
        <el-input v-model="form.configName" placeholder="请输入参数名称" />
      </el-form-item>
      <el-form-item label="参数键名" prop="configKey">
        <el-autocomplete
          v-model="form.configKey"
          popper-class="config-autocomplete"
          :fetch-suggestions="querySearch"
          placeholder="请输入参数键名"
        >
          <template #default="{ item }">
            <span class="value">{{ item.value }}</span>
            <span class="label">（{{ item.label }}）</span>
          </template>
        </el-autocomplete>
      </el-form-item>
      <el-form-item label="参数键值" prop="configValue" v-if="!configKey">
        <el-input v-model="form.configValue" placeholder="请输入参数键值" />
      </el-form-item>
      <component :is="configKey" v-if="configKey" v-model="form.configValue"></component>
      <el-form-item label="系统内置" prop="configType">
        <el-radio-group v-model="form.configType">
          <el-radio v-for="dict in sys_yes_no" :key="dict.value" :value="dict.value">
            {{ dict.label }}
          </el-radio>
        </el-radio-group>
      </el-form-item>
      <el-form-item label="备注" prop="remark">
        <el-input v-model="form.remark" type="textarea" placeholder="请输入内容" />
      </el-form-item>
    </el-form>
    <template #footer>
      <div class="dialog-footer">
        <el-button type="primary" @click="submitForm">确 定</el-button>
        <el-button @click="cancel">取 消</el-button>
      </div>
    </template>
  </el-dialog>
</template>

<script setup>
import { ref, defineExpose, computed } from "vue";
import { ElMessage } from "element-plus";
import { getDict } from "@/utils";
import { getConfig, addConfig, updateConfig } from "@/api/system/config";
import editDing from "./editDing.vue";
import editSystem from "./editSystem.vue";

const { sys_yes_no } = getDict("sys_yes_no");
const emit = defineEmits(["change"]);

const title = ref("");
const open = ref(false);
const configRef = ref(null);
const form = ref({});
const rules = ref({
  configName: [{ required: true, message: "参数名称不能为空", trigger: "blur" }],
  configKey: [{ required: true, message: "参数键名不能为空", trigger: "blur" }],
  configValue: [{ required: true, message: "参数键值不能为空", trigger: "blur" }],
});

const configKey = computed(() => {
  switch (form.value.configKey) {
    case "sys.integration.dingtalk":
      return editDing;
    case "sys.subSystem":
      return editSystem;
    default:
      return "";
  }
});

const dialogWidth = computed(() => {
  if (form.value.configKey === "sys.subSystem") {
    return "68%";
  }
  return "600px";
});

const configKeys = [
  { value: "sys.integration.dingtalk", label: "钉钉集成" },
  { value: "sys.subSystem", label: "子系统管理" },
];

const querySearch = (str) => {
  return configKeys.filter((p) => p.value.includes(str));
};

/** 取消按钮 */
function cancel() {
  open.value = false;
  reset();
}

function openDialog(configId) {
  reset();
  if (configId) {
    getConfig(configId).then((response) => {
      form.value = response.data;
      open.value = true;
      title.value = "修改参数";
    });
    return;
  }
  open.value = true;
}

/** 表单重置 */
function reset() {
  title.value = "添加参数";
  form.value = {
    configId: undefined,
    configName: undefined,
    configKey: undefined,
    configValue: undefined,
    configType: "Y",
    remark: undefined,
  };
  if (configRef.value) configRef.value.resetFields();
}

/** 提交按钮 */
function submitForm() {
  configRef.value?.validate((valid) => {
    if (valid) {
      const apiCall = form.value.configId
        ? updateConfig(form.value)
        : addConfig(form.value);
      apiCall.then(() => {
        ElMessage.success(form.value.configId ? "修改成功" : "新增成功");
        open.value = false;
        emit("change");
      });
    }
  });
}

defineExpose({
  openDialog,
});
</script>

<style>
.config-autocomplete .el-autocomplete-suggestion li {
  line-height: 24px;
}
.config-autocomplete .value {
  font-size: 13px;
}
.config-autocomplete .label {
  margin-top: -8px;
  font-size: 12px;
  color: rgb(164, 163, 163);
}
</style>
