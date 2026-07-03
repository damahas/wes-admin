<template>
  <div class="app-container">
    <div class="main-panel p16">
      <div class="config-toolbar">
        <div class="config-toolbar__left">
          <el-input
            v-model="searchKeyword"
            :placeholder="t('config.placeholder.search')"
            clearable
            prefix-icon="Search"
            style="width: 260px"
          />
        </div>
        <div class="config-toolbar__right">
          <el-dropdown trigger="click" @command="handleAddFromTemplate" v-hasPermi="['system:config:add']">
            <el-button type="primary" plain icon="Plus">
              {{ t('config.addConfig') }}<el-icon class="el-icon--right"><ArrowDown /></el-icon>
            </el-button>
            <template #dropdown>
              <el-dropdown-menu>
                <el-dropdown-item
                  v-for="tpl in addTemplates"
                  :key="tpl.configKey"
                  :command="tpl"
                  :disabled="tpl._exists"
                >
                  {{ tpl.configName }}
                  <span v-if="tpl._exists" style="color:var(--el-color-danger);font-size:11px;margin-left:4px;">{{ t('config.existing') }}</span>
                </el-dropdown-item>
                <el-dropdown-item divided command="__custom__">{{ t('config.generalConfig') }}</el-dropdown-item>
              </el-dropdown-menu>
            </template>
          </el-dropdown>
          <el-button type="danger" plain icon="Refresh" @click="handleRefreshCache" v-hasPermi="['system:config:remove']">
            {{ t('config.refreshCache') }}
          </el-button>
        </div>
      </div>

      <div ref="cardsWrap" class="config-cards" v-loading="loading">
        <div
          v-for="item in filteredConfigs"
          :key="item._key"
          class="config-card"
        >
          <div class="config-card__head">
            <div class="config-card__head-left">
              <span class="config-card__drag" :title="t('config.dragSort')">⠿</span>
              <span class="config-card__name">{{ item.configName }}</span>
              <el-tag size="small" :type="item.configType === 'Y' ? '' : 'info'">{{ item.configKey }}</el-tag>
            </div>
            <div class="config-card__head-right">
              <span v-if="item._saving" class="config-card__status-text"><el-icon class="is-loading"><Loading /></el-icon></span>
              <span v-else-if="item._saved" class="config-card__status-text config-card__status-text--ok">{{ t('config.saved') }}</span>
              <el-button
                link type="danger" icon="Delete"
                @click="handleDelete(item)"
                v-hasPermi="['system:config:remove']"
              />
            </div>
          </div>

          <div class="config-card__body">
            <template v-if="item._schema && item._schema.fields">
              <div class="config-card__fields" :class="{ 'config-card__fields--cols': item._span >= 2 && item._schema.fields.length > 2 }">
                <div v-for="field in item._schema.fields" :key="field.key" class="config-card__field">
                  <label class="config-card__label">{{ t(field.label) }}</label>
                  <div class="config-card__input">
                    <el-switch v-if="field.type === 'switch'" v-model="item._fields[field.key]" @change="scheduleSave(item, 300)" />
                    <template v-else-if="field.button">
                      <el-input
                        v-model="item._fields[field.key]"
                        :type="field.type === 'password' ? 'password' : 'text'"
                        :show-password="field.type === 'password'"
                        @blur="scheduleSave(item, 600)"
                      >
                        <template #append>
                          <el-button @click="handleFieldButton(item, field)">{{ t(field.button.title) }}</el-button>
                        </template>
                      </el-input>
                    </template>
                    <el-input
                      v-else
                      v-model="item._fields[field.key]"
                      :type="field.type === 'password' ? 'password' : 'text'"
                      :show-password="field.type === 'password'"
                      @blur="scheduleSave(item, 600)"
                    />
                  </div>
                </div>
              </div>
            </template>

            <template v-else-if="item._schema && item._schema.type === 'table'">
              <div class="config-card__table-top">
                <el-button size="small" type="primary" plain icon="Plus" @click="handleTableAdd(item)">{{ t('config.addRow') }}</el-button>
              </div>
              <el-table :data="item._fields" border size="small">
                <el-table-column v-for="col in item._schema.columns" :key="col.key" :label="t(col.label)" show-overflow-tooltip>
                  <template #default="{ row }">
                    <el-input v-model="row[col.key]" size="small" @blur="scheduleSave(item, 600)" />
                  </template>
                </el-table-column>
                <el-table-column :label="t('common.actions')" width="70" align="center">
                  <template #default="{ $index }">
                    <el-button link type="danger" icon="Delete" size="small" @click="handleTableRemove(item, $index)" />
                  </template>
                </el-table-column>
              </el-table>
            </template>

            <template v-else>
              <div class="config-card__fields">
                <div class="config-card__field">
                  <label class="config-card__label">{{ t('config.name') }}</label>
                  <el-input v-model="item.configName" @blur="scheduleSave(item, 600)" />
                </div>
                <div class="config-card__field">
                  <label class="config-card__label">{{ t('config.keyName') }}</label>
                  <el-input v-model="item.configKey" @blur="scheduleSave(item, 600)" />
                </div>
                <div class="config-card__field">
                  <label class="config-card__label">{{ t('config.keyValue') }}</label>
                  <el-input v-model="item.configValue" @blur="scheduleSave(item, 600)" />
                </div>
              </div>
            </template>
          </div>
        </div>

        <el-empty v-if="!loading && filteredConfigs.length === 0" :description="t('config.empty')" :image-size="80" style="grid-column: 1 / -1;" />
      </div>
    </div>
  </div>
</template>

<script setup name="Config">
import { ref, reactive, computed, nextTick, onBeforeUnmount } from "vue";
import { useI18n } from "vue-i18n";
import { ElMessage, ElMessageBox } from "element-plus";
import { ArrowDown, Loading } from "@element-plus/icons-vue";
import Sortable from "sortablejs";
import { getAllConfig, addConfig, updateConfig, delConfig, refreshCache, testMail, integrationSync, updateConfigSort } from "@/api/system/config";

const { t } = useI18n();

// ==================== 模板库（label/button.title 使用 i18n key） ====================
const TEMPLATES = {
  "sys.integration.dingtalk": {
    configNameKey: "config.templateNames.dingtalk", configType: "Y", _span: 1,
    schema: { fields: [
      { label: "config.field.appId", key: "appId", type: "input", defaultValue: "" },
      { label: "config.field.corpId", key: "corpId", type: "input", defaultValue: "" },
      { label: "config.field.clientId", key: "clientId", type: "input", defaultValue: "" },
      { label: "config.field.clientSecret", key: "clientSecret", type: "password", defaultValue: "" },
      { label: "config.field.apiUrl", key: "dingPath", type: "input", defaultValue: "https://oapi.dingtalk.com", button: { title: "config.field.sync", action: "dingtalk" } },
    ]},
  },
  "sys.integration.wecom": {
    configNameKey: "config.templateNames.wecom", configType: "Y", _span: 1,
    schema: { fields: [
      { label: "config.field.corpIdLabel", key: "corpId", type: "input", defaultValue: "" },
      { label: "config.field.corpSecret", key: "corpSecret", type: "password", defaultValue: "" },
      { label: "config.field.agentId", key: "agentId", type: "input", defaultValue: "" },
      { label: "config.field.apiUrl", key: "baseUrl", type: "input", defaultValue: "https://qyapi.weixin.qq.com", button: { title: "config.field.sync", action: "wecom" } },
    ]},
  },
  "sys.integration.feishu": {
    configNameKey: "config.templateNames.feishu", configType: "Y", _span: 1,
    schema: { fields: [
      { label: "config.field.appId", key: "appId", type: "input", defaultValue: "" },
      { label: "config.field.appSecret", key: "appSecret", type: "password", defaultValue: "" },
      { label: "config.field.apiUrl", key: "baseUrl", type: "input", defaultValue: "https://open.feishu.cn", button: { title: "config.field.sync", action: "feishu" } },
    ]},
  },
  "sys.integration.mail": {
    configNameKey: "config.templateNames.mail", configType: "Y", _span: 1,
    schema: { fields: [
      { label: "config.field.serverAddress", key: "mailHost", type: "input", defaultValue: "" },
      { label: "config.field.port", key: "mailPort", type: "input", defaultValue: "" },
      { label: "config.field.username", key: "mailAccount", type: "input", defaultValue: "" },
      { label: "config.field.password", key: "mailPassword", type: "password", defaultValue: "" },
      { label: "config.field.testMail", key: "testMail", type: "input", defaultValue: "", button: { title: "config.field.sendTestMail", action: "testMail" } },
      { label: "config.field.ssl", key: "enableSsl", type: "switch", defaultValue: false },
    ]},
  },
  "sys.login.isCaptchaOn": {
    configNameKey: "config.templateNames.captcha", configType: "Y", _span: 1,
    schema: { fields: [
      { label: "config.field.captcha", key: "enabled", type: "switch", defaultValue: false },
    ]},
  },
};

// ==================== 同步按钮 action → message key 映射 ====================
const SYNC_ACTION_MAP = {
  testMail: { fn: testMail, msgKey: "config.mailSent" },
  dingtalk: { fn: integrationSync, msgKey: "config.dingtalkSyncDone", arg: "dingtalk" },
  wecom: { fn: integrationSync, msgKey: "config.wecomSyncDone", arg: "wecom" },
  feishu: { fn: integrationSync, msgKey: "config.feishuSyncDone", arg: "feishu" },
};

// ==================== 状态 ====================
const loading = ref(false);
const searchKeyword = ref("");
const configItems = ref([]);
const cardsWrap = ref(null);
const saveTimers = {};
let sortableInstance = null;

const filteredConfigs = computed(() => {
  if (!searchKeyword.value) return configItems.value;
  const kw = searchKeyword.value.toLowerCase();
  return configItems.value.filter(
    (i) => (i.configName && i.configName.toLowerCase().includes(kw))
        || (i.configKey && i.configKey.toLowerCase().includes(kw))
  );
});

const addTemplates = computed(() => {
  const existingKeys = new Set(configItems.value.map((i) => i.configKey));
  return Object.keys(TEMPLATES).map((key) => ({
    configKey: key,
    configName: t(TEMPLATES[key].configNameKey),
    _exists: existingKeys.has(key),
  }));
});

// ==================== 工具 ====================
function parseFields(schema, configValue) {
  if (!schema) return configValue || "";
  if (schema.type === "table") {
    if (configValue) { try { return JSON.parse(configValue); } catch { return []; } }
    return [];
  }
  const defaults = {};
  schema.fields.forEach((f) => (defaults[f.key] = f.defaultValue ?? ""));
  if (configValue) {
    try { return { ...defaults, ...JSON.parse(configValue) }; } catch { return { ...defaults }; }
  }
  return { ...defaults };
}

function serializeFields(schema, fields) {
  if (!schema) return fields;
  if (schema.type === "table") return JSON.stringify(fields);
  const data = {};
  schema.fields.forEach((f) => { data[f.key] = fields[f.key] ?? f.defaultValue ?? ""; });
  return JSON.stringify(data);
}

function buildItem(dbItem) {
  const tpl = TEMPLATES[dbItem.configKey];
  const schema = tpl?.schema || null;
  const fields = parseFields(schema, dbItem.configValue);
  return reactive({
    _key: dbItem.configKey || `custom_${dbItem.configId}_${Date.now()}`,
    configId: dbItem.configId,
    configKey: dbItem.configKey || "",
    configName: tpl?.configNameKey ? t(tpl.configNameKey) : (dbItem.configName || ""),
    configType: tpl?.configType || dbItem.configType || "N",
    configValue: dbItem.configValue ?? "",
    remark: dbItem.remark || "",
    _schema: schema,
    _fields: fields,
    _span: tpl?._span || 1,
    sortBy: dbItem.sortBy,
    _saving: false,
    _saved: false,
    _dirty: false,
  });
}

// ==================== 加载 ====================
function loadConfigs() {
  loading.value = true;
  getAllConfig().then((res) => {
    const rows = res?.rows || [];
    configItems.value = rows.map((r) => buildItem(r));
    nextTick(() => initSortable());
  }).finally(() => { loading.value = false; });
}

// ==================== 拖拽排序 ====================
function initSortable() {
  if (sortableInstance) sortableInstance.destroy();
  const el = cardsWrap.value;
  if (!el) return;
  sortableInstance = Sortable.create(el, {
    handle: ".config-card__drag",
    animation: 200,
    ghostClass: "config-card--ghost",
    onEnd(evt) {
      const moved = configItems.value.splice(evt.oldIndex, 1)[0];
      configItems.value.splice(evt.newIndex, 0, moved);
      const ids = configItems.value.filter((i) => i.configId).map((i) => i.configId);
      if (ids.length) updateConfigSort(ids).catch(() => {});
    },
  });
}

// ==================== 自动保存（合并 debounce） ====================
function scheduleSave(item, delay) {
  item._dirty = true;
  if (saveTimers[item._key]) clearTimeout(saveTimers[item._key]);
  saveTimers[item._key] = setTimeout(() => doSave(item), delay);
}

function doSave(item) {
  if (!item._dirty) return;
  item._saving = true;
  item._saved = false;
  const cv = item._schema ? serializeFields(item._schema, item._fields) : (item.configValue || "");
  const p = { configId: item.configId, configKey: item.configKey, configName: item.configName, configType: item.configType, configValue: cv, remark: item.remark };
  const api = item.configId ? updateConfig(p) : addConfig(p);
  api.then((res) => {
    if (!item.configId && res?.data?.configId) item.configId = res.data.configId;
    item._saving = false;
    item._saved = true;
    item._dirty = false;
    setTimeout(() => { item._saved = false; }, 2000);
  }).catch(() => { item._saving = false; });
}

// ==================== 按钮（action map 替代 if/else） ====================
async function handleFieldButton(item, field) {
  const action = field.button?.action;
  const mapping = SYNC_ACTION_MAP[action];
  if (!mapping) return;
  try {
    const args = action === "testMail" ? [item._fields] : [mapping.arg];
    await mapping.fn(...args);
    ElMessage.success(t(mapping.msgKey));
  } catch { /* silent */ }
}

function handleTableAdd(item) {
  if (!item._schema?.columns) return;
  const row = {};
  item._schema.columns.forEach((c) => (row[c.key] = ""));
  item._fields.push(row);
  scheduleSave(item, 300);
}
function handleTableRemove(item, i) { item._fields.splice(i, 1); scheduleSave(item, 300); }

function handleAddFromTemplate(command) {
  if (command === "__custom__") {
    configItems.value.unshift(reactive({
      _key: `custom_${Date.now()}`, configId: undefined, configKey: "", configName: "",
      configType: "N", configValue: "", remark: "",
      _schema: null, _fields: "", _span: 1,
      sortBy: null, _saving: false, _saved: false, _dirty: false,
    }));
  } else {
    const tpl = TEMPLATES[command.configKey];
    configItems.value.unshift(reactive({
      _key: command.configKey, configId: undefined,
      configKey: command.configKey, configName: t(tpl.configNameKey),
      configType: tpl.configType, configValue: "", remark: "",
      _schema: tpl.schema, _fields: parseFields(tpl.schema, ""), _span: tpl._span,
      sortBy: null, _saving: false, _saved: false, _dirty: false,
    }));
  }
  nextTick(() => initSortable());
}

async function handleDelete(item) {
  const name = item.configName || item.configKey || t("config.unnamed");
  try {
    await ElMessageBox.confirm(
      t("config.confirm.delete", { name }),
      t("common.confirmTitle"),
      { confirmButtonText: t("common.confirmDelete"), cancelButtonText: t("common.cancel"), confirmButtonType: "warning", type: "warning" }
    );
    if (item.configId) {
      await delConfig(item.configId);
      ElMessage.success(t("config.deleteSuccess"));
    }
    const i = configItems.value.findIndex((x) => x._key === item._key);
    if (i > -1) configItems.value.splice(i, 1);
    if (!item.configId) ElMessage.success(t("config.removed"));
  } catch { /* cancel */ }
}

function handleRefreshCache() {
  refreshCache().then(() => ElMessage.success(t("config.cacheRefreshed")));
}

onBeforeUnmount(() => {
  Object.values(saveTimers).forEach((timer) => clearTimeout(timer));
  if (sortableInstance) sortableInstance.destroy();
});
loadConfigs();
</script>

<style scoped>
.config-toolbar {
  display: flex; align-items: center; justify-content: space-between; margin-bottom: 18px;
}
.config-toolbar__right { display: flex; gap: 8px; }

/* ========== 3 列瀑布流 ========== */
.config-cards {
  column-count: 3;
  column-gap: 14px;
}

/* ========== 卡片 ========== */
.config-card {
  background: var(--el-fill-color-blank);
  border: 1px solid var(--el-border-color-dark);
  border-radius: 10px;
  overflow: hidden;
  box-shadow: 0 1px 4px rgba(0,0,0,0.04);
  transition: box-shadow 0.2s, border-color 0.2s;
  break-inside: avoid;
  margin-bottom: 14px;
}
.config-card:hover {
  border-color: var(--el-color-primary-light-3);
  box-shadow: 0 4px 16px rgba(0,0,0,0.08);
}
html.dark .config-card:hover { box-shadow: 0 4px 16px rgba(0,0,0,0.25); }

/* 拖拽中的鬼影 */
.config-card--ghost {
  opacity: 0.4;
  border-style: dashed;
}

/* ========== 拖拽把手 ========== */
.config-card__drag {
  cursor: grab;
  color: var(--el-text-color-placeholder);
  font-size: 16px;
  line-height: 1;
  user-select: none;
  flex-shrink: 0;
}
.config-card__drag:active {
  cursor: grabbing;
}

/* ========== 头部 ========== */
.config-card__head {
  display: flex; align-items: center; justify-content: space-between;
  padding: 10px 16px;
  background: var(--el-fill-color-lighter);
  border-bottom: 1px solid var(--el-border-color-lighter);
  gap: 8px;
}
.config-card__head-left { display: flex; align-items: center; gap: 8px; min-width: 0; }
.config-card__head-right { display: flex; align-items: center; gap: 6px; flex-shrink: 0; }
.config-card__name { font-size: 14px; font-weight: 600; color: var(--el-text-color-primary); white-space: nowrap; }
.config-card__status-text { font-size: 12px; color: var(--el-text-color-secondary); }
.config-card__status-text--ok { color: var(--el-color-success); }

/* ========== 内容 ========== */
.config-card__body { padding: 14px 16px; }

.config-card__fields { display: flex; flex-direction: column; gap: 10px; }
.config-card__fields--cols {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 8px 16px;
}

.config-card__field { display: flex; align-items: center; gap: 8px; }
.config-card__label {
  width: 70px; flex-shrink: 0;
  font-size: 14px; color: var(--el-text-color-secondary);
  text-align: right; white-space: nowrap;
}
.config-card__input { flex: 1; min-width: 0; }

.config-card__table-top { margin-bottom: 8px; }

/* ========== 响应式 ========== */
@media (max-width: 1100px) {
  .config-cards { column-count: 2; }
}
@media (max-width: 700px) {
  .config-cards { column-count: 1; }
  .config-card__fields--cols { grid-template-columns: 1fr; }
}
</style>
