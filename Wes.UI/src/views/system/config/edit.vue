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

      <FormRenderer
        v-if="currentSchema"
        v-model="schemaData"
        :schema="currentSchema"
        @update:model-value="handleSchemaChange"
      />

      <el-form-item label="参数键值" prop="configValue" v-if="isCustomKey">
        <el-input v-model="form.configValue" placeholder="请输入参数键值" />
      </el-form-item>
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
import { ref, computed, watch } from "vue";
import { ElMessage } from "element-plus";
import { getDict } from "@/utils";
import { getConfig, addConfig, updateConfig, testMail } from "@/api/system/config";
import FormRenderer from "./formRenderer.vue";

const { sys_yes_no } = getDict("sys_yes_no");
const emit = defineEmits(["change"]);

const configKeys = [
  { value: "sys.integration.dingtalk", label: "钉钉集成" },
  { value: "sys.integration.mail", label: "邮箱集成" },
  { value: "sys.subSystem", label: "子系统管理" },
];

const configSchema = {
  "sys.integration.dingtalk": {
    fields: [
      {
        label: "钉钉接口地址",
        key: "dingPath",
        type: "input",
        defaultValue: "https://api.dingtalk.com",
        required: true,
      },
      {
        label: "AgentId",
        key: "agentId",
        type: "input",
        defaultValue: "",
        required: true,
      },
      { label: "AppKey", key: "appKey", type: "input", defaultValue: "", required: true },
      {
        label: "AppSecret",
        key: "appSecret",
        type: "password",
        defaultValue: "",
        required: true,
      },
    ],
  },
  "sys.integration.mail": {
    fields: [
      {
        label: "邮箱服务器地址",
        key: "mailHost",
        type: "input",
        defaultValue: "",
        required: true,
      },
      {
        label: "邮箱端口",
        key: "mailPort",
        type: "input",
        defaultValue: "",
        required: true,
      },
      {
        label: "邮箱账号",
        key: "mailAccount",
        type: "input",
        defaultValue: "",
        required: true,
      },
      {
        label: "邮箱密码",
        key: "mailPassword",
        type: "password",
        defaultValue: "",
        required: true,
      },
      {
        label: "测试邮箱",
        key: "testMail",
        type: "input",
        defaultValue: "",
        button: {
          title: "发送邮件",
          action: handleTestMail,
        },
      },
      { label: "支持SSL", key: "enableSsl", type: "switch", defaultValue: false },
    ],
  },
  "sys.subSystem": {
    type: "table",
    columns: [
      { label: "ID", key: "systemId" },
      { label: "系统名称", key: "systemName" },
      { label: "访问地址", key: "systemUrl" },
    ],
  },
};

const title = ref("");
const open = ref(false);
const configRef = ref(null);
const form = ref({});
const rules = {
  configName: [{ required: true, message: "参数名称不能为空", trigger: "blur" }],
  configKey: [{ required: true, message: "参数键名不能为空", trigger: "blur" }],
  configValue: [{ required: true, message: "参数键值不能为空", trigger: "blur" }],
};

const schemaData = ref({});

const configKey = computed(() => form.value.configKey);

const isCustomKey = computed(() => configKey.value && !currentSchema.value);

const currentSchema = computed(() => configSchema[configKey.value] ?? null);

const dialogWidth = computed(() => {
  if (configKey.value === "sys.subSystem") return "68%";
  return "600px";
});

const initSchemaData = (schema, val) => {
  if (val) {
    try {
      return JSON.parse(val);
    } catch {
      return schema.type === "table" ? [] : {};
    }
  }
  if (schema.type === "table") return [];
  const d = {};
  schema.fields.forEach((f) => (d[f.key] = f.defaultValue ?? ""));
  return d;
};

watch(configKey, (key) => {
  schemaData.value = configSchema[key] ? initSchemaData(configSchema[key], null) : {};
});

watch(
  () => form.value.configValue,
  (val) => {
    if (!configKey.value || !currentSchema.value) return;
    schemaData.value = initSchemaData(currentSchema.value, val);
  }
);

// 从 schemaData 同步到 configValue
function handleSchemaChange() {
  form.value.configValue = JSON.stringify(schemaData.value);
}

async function handleTestMail() {
  await testMail(schemaData.value);
  ElMessage.success("邮件发送成功，请检查收件箱");
}

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
  schemaData.value = {};
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
