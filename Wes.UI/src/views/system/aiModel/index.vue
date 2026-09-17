<template>
  <div class="app-container">
    <div class="main-panel p16">
      <query-form
        :config="queryConfig"
        v-model:visible="showSearch"
        v-model="queryParams.params"
        @query="handleQuery"
        @reset="resetQuery"
      />

      <el-row :gutter="10" class="mb8">
        <el-col :span="1.5">
          <el-button
            type="primary"
            plain
            icon="Plus"
            @click="handleAdd"
            v-hasPermi="['system:aiModel:add']"
            >{{ t('common.add') }}</el-button
          >
        </el-col>
        <el-col :span="1.5">
          <el-button
            type="success"
            plain
            icon="Edit"
            :disabled="single"
            @click="handleUpdate()"
            v-hasPermi="['system:aiModel:edit']"
            >{{ t('common.edit') }}</el-button
          >
        </el-col>
        <el-col :span="1.5">
          <el-button
            type="danger"
            plain
            icon="Delete"
            :disabled="multiple"
            @click="handleDelete()"
            v-hasPermi="['system:aiModel:remove']"
            >{{ t('common.delete') }}</el-button
          >
        </el-col>
        <right-toolbar v-model:showSearch="showSearch" @queryTable="getList"></right-toolbar>
      </el-row>

      <el-table
        v-loading="loading"
        :data="tableList"
        @selection-change="handleSelectionChange"
      >
        <el-table-column type="selection" width="55" align="center" />
        <el-table-column :label="t('aiModel.index')" type="index" width="80" align="center" />
        <el-table-column :label="t('aiModel.provider')" prop="provider" min-width="150">
          <template #default="scope">
            <div class="ai-model__nowrap">{{ scope.row.provider }}</div>
          </template>
        </el-table-column>
        <el-table-column :label="t('aiModel.model')" prop="displayName" min-width="240">
          <template #default="scope">
            <div
              class="ai-model__nowrap ai-model__ellipsis"
              :title="`${scope.row.displayName} (${scope.row.modelId})`"
            >
              {{ scope.row.displayName }}
              <span class="ai-model__sub">({{ scope.row.modelId }})</span>
            </div>
          </template>
        </el-table-column>
        <el-table-column :label="t('aiModel.capability')" min-width="240">
          <template #default="scope">
            <div class="ai-model__capline">
              <span class="ai-model__summary">{{ scope.row.capabilitySummary }}</span>
              <el-tag
                v-for="k in extraCaps(scope.row.capabilityKeys)"
                :key="k"
                size="small"
                class="ai-model__cap"
                :type="capType(k)"
              >{{ capName(k) }}</el-tag>
            </div>
          </template>
        </el-table-column>
        <el-table-column :label="t('aiModel.maxContext')" min-width="110" align="right">
          <template #default="scope">
            <span class="ai-model__nowrap">{{ fmtContext(scope.row.maxContext) }}</span>
          </template>
        </el-table-column>
        <el-table-column :label="t('aiModel.default')" min-width="110" align="center">
          <template #default="scope">
            <el-switch
              :model-value="scope.row.isDefault"
              :before-change="() => beforeDefaultChange(scope.row)"
              @change="(val) => handleDefaultChange(scope.row, val)"
            />
          </template>
        </el-table-column>
        <el-table-column :label="t('aiModel.apiKey')" min-width="100" align="center">
          <template #default="scope">
            <el-tag size="small" :type="scope.row.hasApiKey ? 'success' : 'info'">
              {{ scope.row.hasApiKey ? t('aiModel.keyConfigured') : t('aiModel.keyMissing') }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column
          :label="t('common.actions')"
          align="center"
          min-width="140"
          class-name="small-padding fixed-width"
        >
          <template #default="scope">
            <div class="ai-model__nowrap">
              <el-button
                link
                type="primary"
                icon="Edit"
                @click="handleUpdate(scope.row)"
                v-hasPermi="['system:aiModel:edit']"
              >{{ t('common.edit') }}</el-button>
              <el-button
                link
                type="danger"
                icon="Delete"
                @click="handleDelete(scope.row)"
                v-hasPermi="['system:aiModel:remove']"
              >{{ t('common.delete') }}</el-button>
            </div>
          </template>
        </el-table-column>
      </el-table>
    </div>

    <!-- 新增/编辑模型对话框 -->
    <el-dialog :title="dialogTitle" v-model="dialogOpen" width="760px" append-to-body>
      <el-form ref="formRef" :model="form" :rules="rules" label-width="auto">
        <el-row>
          <el-col :span="12">
            <el-form-item :label="t('aiModel.provider')" prop="provider">
              <el-input v-model="form.provider" :placeholder="t('aiModel.placeholder.provider')" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item :label="t('aiModel.defaultModel')" prop="isDefault">
              <el-switch v-model="form.isDefault" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item :label="t('aiModel.modelId')" prop="modelId">
              <el-input v-model="form.modelId" :placeholder="t('aiModel.placeholder.modelId')" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item prop="displayName">
              <template #label>
                <span>
                  <el-tooltip :content="t('aiModel.displayNameTip')" placement="top">
                    <el-icon><question-filled /></el-icon>
                  </el-tooltip>
                  {{ t('aiModel.displayName') }}
                </span>
              </template>
              <el-input v-model="form.displayName" :placeholder="t('aiModel.placeholder.displayName')" />
            </el-form-item>
          </el-col>
          <el-col :span="24">
            <el-form-item :label="t('aiModel.baseUrl')" prop="baseUrl">
              <el-input v-model="form.baseUrl" :placeholder="t('aiModel.placeholder.baseUrl')" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item :label="t('aiModel.maxContext')" prop="maxContext">
              <el-input-number v-model="form.maxContext" :min="0" :step="1024" style="width: 100%" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item :label="t('common.sort')" prop="sort">
              <el-input-number v-model="form.sort" :min="0" style="width: 100%" />
            </el-form-item>
          </el-col>
          <el-col :span="24">
            <el-form-item :label="t('aiModel.apiKey')" prop="apiKey">
              <el-input
                v-model="form.apiKey"
                type="password"
                show-password
                :placeholder="form.id ? t('aiModel.placeholder.apiKeyKeep') : t('aiModel.placeholder.apiKey')"
              />
              <div class="ai-model__tip">{{ t('aiModel.apiKeyTip') }}</div>
            </el-form-item>
          </el-col>
        </el-row>

        <!-- 能力：按输入 / 输出 / 附加分组勾选 -->
        <el-divider content-position="left">{{ t('aiModel.capability') }}</el-divider>
        <div v-for="g in capGroups" :key="g.group" class="ai-model__capgroup">
          <span class="ai-model__capgroup-title">{{ g.label }}</span>
          <el-checkbox-group v-model="form.capabilityKeys">
            <el-checkbox v-for="item in g.items" :key="item.key" :value="item.key">{{ item.name }}</el-checkbox>
          </el-checkbox-group>
        </div>
        <div class="ai-model__tip">{{ t('aiModel.capabilityTip') }}：{{ capabilitySummary }}</div>
      </el-form>
      <template #footer>
        <el-button type="primary" @click="submitForm">{{ t('common.confirm') }}</el-button>
        <el-button @click="dialogOpen = false">{{ t('common.cancel') }}</el-button>
      </template>
    </el-dialog>
  </div>
</template>

<script setup name="AiModel">
import { ref, computed, nextTick } from "vue";
import { ElMessage, ElMessageBox } from "element-plus";
import { useI18n } from "vue-i18n";
import QueryForm from "@/components/QueryForm/index.vue";
import {
  listAiModels,
  getAiModel,
  addAiModel,
  updateAiModel,
  delAiModel,
  getAiCapabilities,
} from "@/api/ai";

const { t } = useI18n();

// 搜索
const showSearch = ref(true);
const loading = ref(false);
const tableList = ref([]);
const ids = ref([]);
const single = ref(true);
const multiple = ref(true);

// 查询参数
const queryParams = ref({
  params: {
    keyword: undefined,
    provider: undefined,
  },
});

// 提供商下拉：取自列表数据（值=名称，显示=名称）
const providerOptions = computed(() => {
  const set = new Set();
  tableList.value.forEach((row) => {
    if (row.provider) set.add(row.provider);
  });
  return Array.from(set, (v) => ({ value: v, label: v }));
});

// 查询表单配置
const queryConfig = computed(() => [
  {
    label: t('aiModel.keyword'),
    prop: "keyword",
    type: "input",
    placeholder: t('aiModel.keywordPlaceholder'),
  },
  {
    label: t('aiModel.provider'),
    prop: "provider",
    type: "select",
    placeholder: t('common.pleaseSelect'),
    options: providerOptions,
  },
]);

// 对话框
const dialogOpen = ref(false);
const dialogTitle = ref("");
const formRef = ref(null);
// 编辑前该行是否为默认模型：用于阻止把唯一默认模型关掉
const originalIsDefault = ref(false);

// 能力选项与默认能力（文本对话 + 工具 + 流式）
const capOptions = ref([]);
const DEFAULT_KEYS = ["input_text", "output_text", "tool_call", "streaming"];

const form = ref({
  id: 0,
  provider: "",
  modelId: "",
  displayName: "",
  baseUrl: "",
  maxContext: 128000,
  apiKey: "",
  capabilityKeys: [...DEFAULT_KEYS],
  isDefault: false,
  sort: 10,
});

const rules = {
  provider: [{ required: true, message: computed(() => t('aiModel.rules.providerRequired')), trigger: "blur" }],
  modelId:  [{ required: true, message: computed(() => t('aiModel.rules.modelIdRequired')), trigger: "blur" }],
  displayName: [{ required: true, message: computed(() => t('aiModel.rules.displayNameRequired')), trigger: "blur" }],
  baseUrl: [{ required: true, message: computed(() => t('aiModel.rules.baseUrlRequired')), trigger: "blur" }],
};

// 能力分组（输入 / 输出 / 附加）
const capGroups = computed(() => {
  const groupLabel = {
    input: t('aiModel.capInput'),
    output: t('aiModel.capOutput'),
    extra: t('aiModel.capExtra'),
  };
  return ["input", "output", "extra"]
    .map((group) => ({
      group,
      label: groupLabel[group],
      items: capOptions.value.filter((o) => o.group === group),
    }))
    .filter((g) => g.items.length > 0);
});

const capNameMap = computed(() =>
  Object.fromEntries(capOptions.value.map((o) => [o.key, o.name]))
);

function capName(key) {
  return capNameMap.value[key] || key;
}

function capType(key) {
  if (key.startsWith("input_")) return "success";
  if (key.startsWith("output_")) return "warning";
  return "info";
}

// 普通对话模型都有的「文本输入/文本输出」不再重复展示，只显示附加能力
const TEXT_CAPS = ["input_text", "output_text"];

function extraCaps(keys) {
  return (keys || []).filter((k) => !TEXT_CAPS.includes(k));
}

// 上下文格式化：65536 → 64K，未设置 → -
function fmtContext(v) {
  if (!v || v <= 0) return "-";
  return v % 1024 === 0 ? `${v / 1024}K` : String(v);
}

// 能力摘要预览：「文+图 → 文」
const capabilitySummary = computed(() => {
  const keys = new Set(form.value.capabilityKeys);
  const input = [];
  const output = [];
  if (keys.has("input_text")) input.push(t('aiModel.abbr.text'));
  if (keys.has("input_image")) input.push(t('aiModel.abbr.image'));
  if (keys.has("input_file")) input.push(t('aiModel.abbr.file'));
  if (keys.has("input_audio")) input.push(t('aiModel.abbr.audio'));
  if (keys.has("output_text")) output.push(t('aiModel.abbr.text'));
  if (keys.has("output_image")) output.push(t('aiModel.abbr.image'));
  if (keys.has("output_audio")) output.push(t('aiModel.abbr.audio'));
  return `${input.join("+") || "-"} → ${output.join("+") || "-"}`;
});

/** 获取列表 */
function getList() {
  loading.value = true;
  listAiModels({
    keyword: queryParams.value.params.keyword,
    provider: queryParams.value.params.provider,
  })
    .then((res) => {
      tableList.value = res?.data || [];
    })
    .catch(() => {
      tableList.value = [];
    })
    .finally(() => {
      loading.value = false;
    });
}

/** 能力字典 */
function getCapabilities() {
  getAiCapabilities().then((res) => {
    capOptions.value = res?.data || [];
  });
}

/** 搜索 */
function handleQuery() {
  getList();
}

/** 重置 */
function resetQuery() {
  queryParams.value.params.keyword = undefined;
  queryParams.value.params.provider = undefined;
  getList();
}

/** 选择 */
function handleSelectionChange(selection) {
  ids.value = selection.map((item) => item.id);
  single.value = selection.length !== 1;
  multiple.value = !selection.length;
}

/** 新增 */
function handleAdd() {
  form.value = {
    id: 0,
    provider: "",
    modelId: "",
    displayName: "",
    baseUrl: "",
    maxContext: 128000,
    apiKey: "",
    capabilityKeys: [...DEFAULT_KEYS],
    isDefault: false,
    sort: 10,
  };
  originalIsDefault.value = false;
  dialogTitle.value = t('aiModel.addTitle');
  dialogOpen.value = true;
  nextTick(() => formRef.value?.clearValidate());
}

/** 修改 */
function handleUpdate(row) {
  const id = row ? row.id : ids.value[0];
  if (!id) return;
  getAiModel(id).then((res) => {
    const d = res?.data;
    if (!d) return;
    form.value = {
      id: d.id,
      provider: d.provider,
      modelId: d.modelId,
      displayName: d.displayName,
      baseUrl: d.baseUrl,
      maxContext: d.maxContext,
      // 密钥只回显掩码，这里留空表示不修改
      apiKey: "",
      capabilityKeys: d.capabilityKeys?.length ? [...d.capabilityKeys] : [...DEFAULT_KEYS],
      isDefault: d.isDefault,
      sort: d.sort,
    };
    originalIsDefault.value = !!d.isDefault;
    dialogTitle.value = t('aiModel.editTitle');
    dialogOpen.value = true;
    nextTick(() => formRef.value?.clearValidate());
  }).catch(() => {});
}

/** 删除 */
function handleDelete(row) {
  const modelIds = row ? [row.id] : ids.value;
  if (!modelIds.length) return;
  ElMessageBox.confirm(
    t('aiModel.confirmDelete', {
      name: row
        ? row.displayName || row.modelId
        : tableList.value
            .filter((item) => modelIds.includes(item.id))
            .map((item) => item.displayName || item.modelId)
            .join("、"),
    }),
    t('common.confirmTitle'),
    {
      confirmButtonText: t('common.confirmDelete'),
      cancelButtonText: t('common.cancel'),
      confirmButtonType: "danger",
      type: "warning",
    }
  )
    .then(() => delAiModel(modelIds.join(",")))
    .then(() => {
      ElMessage.success(t('common.deleteSuccess'));
      getList();
    })
    .catch(() => {});
}

/** 列表开关：关闭当前默认模型时拦截，保证始终存在一个默认模型 */
function beforeDefaultChange(row) {
  if (!row.isDefault) return true;
  ElMessage.warning(t('aiModel.defaultKeepTip'));
  return false;
}

/** 列表开关：直接把该行设为默认模型（后端会自动取消其他行） */
function handleDefaultChange(row, val) {
  const payload = {
    id: row.id,
    provider: row.provider,
    modelId: row.modelId,
    displayName: row.displayName,
    baseUrl: row.baseUrl,
    maxContext: row.maxContext,
    capabilityKeys: [...(row.capabilityKeys || [])],
    isDefault: val,
    sort: row.sort,
  };
  updateAiModel(row.id, payload)
    .then(() => {
      ElMessage.success(t('common.editSuccess'));
      getList();
    })
    .catch(() => {
      row.isDefault = !val;
    });
}

/** 提交 */
function submitForm() {
  formRef.value?.validate((valid) => {
    if (!valid) return;
    if (!form.value.isDefault && originalIsDefault.value) {
      ElMessage.warning(t('aiModel.defaultKeepTip'));
      return;
    }
    const data = { ...form.value, capabilityKeys: [...form.value.capabilityKeys] };
    const apiCall = data.id ? updateAiModel(data.id, data) : addAiModel(data);
    apiCall.then(() => {
      ElMessage.success(data.id ? t('common.editSuccess') : t('common.addSuccess'));
      dialogOpen.value = false;
      getList();
    });
  });
}

// 初始化
getCapabilities();
getList();
</script>

<style scoped>
.ai-model__nowrap {
  white-space: nowrap;
}
.ai-model__sub {
  color: var(--el-text-color-secondary, #909399);
  font-size: 14px;
}
.ai-model__ellipsis {
  max-width: 300px;
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
}
.ai-model__capline {
  display: flex;
  align-items: center;
  white-space: nowrap;
}
.ai-model__summary {
  font-weight: 600;
  margin-right: 8px;
}
.ai-model__cap {
  margin: 2px 4px 2px 0;
}
.ai-model__capgroup {
  display: flex;
  align-items: center;
  gap: 12px;
  margin-bottom: 8px;
}
.ai-model__capgroup-title {
  width: 72px;
  flex-shrink: 0;
  color: var(--el-text-color-secondary, #606266);
}
.ai-model__tip {
  color: var(--el-text-color-secondary, #909399);
  font-size: 14px;
  line-height: 1.5;
}
</style>
