<template>
  <div class="app-container">
    <div class="main-panel p16">
      <div class="config-toolbar">
        <div class="config-toolbar__left">
          <input type="text" name="username" autocomplete="username" style="display:none" />
          <input type="password" name="password" autocomplete="current-password" style="display:none" />
          <el-input
            v-model="searchKeyword"
            placeholder="搜索配置名称或键名"
            clearable
            prefix-icon="Search"
            style="width: 260px"
            autocomplete="new-password"
            name="config-search"
            @clear="searchKeyword = ''"
          />
        </div>
        <div class="config-toolbar__right">
          <el-dropdown trigger="click" @command="handleAddFromTemplate" v-hasPermi="['system:config:add']">
            <el-button type="primary" plain icon="Plus">
              新增配置<el-icon class="el-icon--right"><ArrowDown /></el-icon>
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
                  <span v-if="tpl._exists" style="color:var(--el-color-danger);font-size:11px;margin-left:4px;">已存在</span>
                </el-dropdown-item>
                <el-dropdown-item divided command="__custom__">通用配置</el-dropdown-item>
              </el-dropdown-menu>
            </template>
          </el-dropdown>
          <el-button type="danger" plain icon="Refresh" @click="handleRefreshCache" v-hasPermi="['system:config:remove']">
            刷新缓存
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
              <span class="config-card__drag" title="拖拽排序">⠿</span>
              <span class="config-card__name">{{ item.configName }}</span>
              <el-tag size="small" :type="item.configType === 'Y' ? '' : 'info'">{{ item.configKey }}</el-tag>
            </div>
            <div class="config-card__head-right">
              <span v-if="item._saving" class="config-card__status-text"><el-icon class="is-loading"><Loading /></el-icon></span>
              <span v-else-if="item._saved" class="config-card__status-text config-card__status-text--ok">已保存</span>
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
                  <label class="config-card__label">{{ field.label }}</label>
                  <div class="config-card__input">
                    <el-switch v-if="field.type === 'switch'" v-model="item._fields[field.key]" @change="handleFieldChange(item)" />
                    <template v-else-if="field.button">
                      <el-input
                        v-model="item._fields[field.key]"
                        :type="field.type === 'password' ? 'password' : 'text'"
                        :show-password="field.type === 'password'"
                        @blur="handleFieldBlur(item)"
                      >
                        <template #append>
                          <el-button @click="handleFieldButton(item, field)">{{ field.button.title }}</el-button>
                        </template>
                      </el-input>
                    </template>
                    <el-input
                      v-else
                      v-model="item._fields[field.key]"
                      :type="field.type === 'password' ? 'password' : 'text'"
                      :show-password="field.type === 'password'"
                      @blur="handleFieldBlur(item)"
                    />
                  </div>
                </div>
              </div>
            </template>

            <template v-else-if="item._schema && item._schema.type === 'table'">
              <div class="config-card__table-top">
                <el-button size="small" type="primary" plain icon="Plus" @click="handleTableAdd(item)">新增行</el-button>
              </div>
              <el-table :data="item._fields" border size="small">
                <el-table-column v-for="col in item._schema.columns" :key="col.key" :label="col.label" show-overflow-tooltip>
                  <template #default="{ row }">
                    <el-input v-model="row[col.key]" size="small" @blur="handleFieldBlur(item)" />
                  </template>
                </el-table-column>
                <el-table-column label="操作" width="70" align="center">
                  <template #default="{ $index }">
                    <el-button link type="danger" icon="Delete" size="small" @click="handleTableRemove(item, $index)" />
                  </template>
                </el-table-column>
              </el-table>
            </template>

            <template v-else>
              <div class="config-card__fields">
                <div class="config-card__field">
                  <label class="config-card__label">名称</label>
                  <el-input v-model="item.configName" @blur="handleFieldBlur(item)" />
                </div>
                <div class="config-card__field">
                  <label class="config-card__label">键名</label>
                  <el-input v-model="item.configKey" @blur="handleFieldBlur(item)" />
                </div>
                <div class="config-card__field">
                  <label class="config-card__label">键值</label>
                  <el-input v-model="item.configValue" @blur="handleFieldBlur(item)" />
                </div>
              </div>
            </template>
          </div>
        </div>

        <el-empty v-if="!loading && filteredConfigs.length === 0" description="暂无配置，点击「新增配置」添加" :image-size="80" style="grid-column: 1 / -1;" />
      </div>
    </div>
  </div>
</template>

<script setup name="Config">
import { ref, reactive, computed, nextTick, onBeforeUnmount } from "vue";
import { ElMessage, ElMessageBox } from "element-plus";
import { ArrowDown, Loading } from "@element-plus/icons-vue";
import Sortable from "sortablejs";
import { getAllConfig, addConfig, updateConfig, delConfig, refreshCache, testMail, integrationSync, updateConfigSort } from "@/api/system/config";

// ==================== 模板库 ====================
const TEMPLATES = {
  "sys.integration.dingtalk": {
    configName: "钉钉集成", configType: "Y", _span: 1,
    schema: { fields: [
      { label: "AppId", key: "appId", type: "input", defaultValue: "" },
      { label: "CorpId", key: "corpId", type: "input", defaultValue: "" },
      { label: "ClientId", key: "clientId", type: "input", defaultValue: "" },
      { label: "ClientSecret", key: "clientSecret", type: "password", defaultValue: "" },
      { label: "接口地址", key: "dingPath", type: "input", defaultValue: "https://oapi.dingtalk.com", button: { title: "同步", action: "dingtalk" } },
    ]},
  },
  "sys.integration.wecom": {
    configName: "企业微信集成", configType: "Y", _span: 1,
    schema: { fields: [
      { label: "企业ID", key: "corpId", type: "input", defaultValue: "" },
      { label: "应用Secret", key: "corpSecret", type: "password", defaultValue: "" },
      { label: "AgentId", key: "agentId", type: "input", defaultValue: "" },
      { label: "接口地址", key: "baseUrl", type: "input", defaultValue: "https://qyapi.weixin.qq.com", button: { title: "同步", action: "wecom" } },
    ]},
  },
  "sys.integration.feishu": {
    configName: "飞书集成", configType: "Y", _span: 1,
    schema: { fields: [
      { label: "AppId", key: "appId", type: "input", defaultValue: "" },
      { label: "AppSecret", key: "appSecret", type: "password", defaultValue: "" },
      { label: "接口地址", key: "baseUrl", type: "input", defaultValue: "https://open.feishu.cn", button: { title: "同步", action: "feishu" } },
    ]},
  },
  "sys.integration.mail": {
    configName: "邮箱集成", configType: "Y", _span: 1,
    schema: { fields: [
      { label: "服务器地址", key: "mailHost", type: "input", defaultValue: "" },
      { label: "端口", key: "mailPort", type: "input", defaultValue: "" },
      { label: "账号", key: "mailAccount", type: "input", defaultValue: "" },
      { label: "密码", key: "mailPassword", type: "password", defaultValue: "" },
      { label: "测试邮箱", key: "testMail", type: "input", defaultValue: "", button: { title: "发送测试邮件", action: "testMail" } },
      { label: "SSL", key: "enableSsl", type: "switch", defaultValue: false },
    ]},
  },
  "sys.login.isCaptchaOn": {
    configName: "登录验证码", configType: "Y", _span: 1,
    schema: { fields: [
      { label: "验证码", key: "enabled", type: "switch", defaultValue: false },
    ]},
  },
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
    configName: TEMPLATES[key].configName,
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
    configName: tpl?.configName || dbItem.configName || "",
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

// ==================== 自动保存 ====================
function handleFieldBlur(item) {
  item._dirty = true;
  if (saveTimers[item._key]) clearTimeout(saveTimers[item._key]);
  saveTimers[item._key] = setTimeout(() => doSave(item), 600);
}
function handleFieldChange(item) {
  item._dirty = true;
  if (saveTimers[item._key]) clearTimeout(saveTimers[item._key]);
  saveTimers[item._key] = setTimeout(() => doSave(item), 300);
}
function doSave(item) {
  if (!item._dirty) return;
  item._saving = true; item._saved = false;
  const cv = item._schema ? serializeFields(item._schema, item._fields) : (item.configValue || "");
  const p = { configId: item.configId, configKey: item.configKey, configName: item.configName, configType: item.configType, configValue: cv, remark: item.remark };
  const api = item.configId ? updateConfig(p) : addConfig(p);
  api.then((res) => {
    if (!item.configId && res?.data?.configId) item.configId = res.data.configId;
    item._saving = false; item._saved = true; item._dirty = false;
    setTimeout(() => { item._saved = false; }, 2000);
  }).catch(() => { item._saving = false; });
}

// ==================== 按钮 ====================
async function handleFieldButton(item, field) {
  const a = field.button?.action;
  if (a === "testMail") { try { await testMail(item._fields); ElMessage.success("邮件已发送"); } catch {} }
  else if (a === "dingtalk") { try { await integrationSync("dingtalk"); ElMessage.success("钉钉同步完成"); } catch {} }
  else if (a === "wecom") { try { await integrationSync("wecom"); ElMessage.success("企微同步完成"); } catch {} }
  else if (a === "feishu") { try { await integrationSync("feishu"); ElMessage.success("飞书同步完成"); } catch {} }
}

function handleTableAdd(item) {
  if (!item._schema?.columns) return;
  const row = {};
  item._schema.columns.forEach((c) => (row[c.key] = ""));
  item._fields.push(row);
  handleFieldChange(item);
}
function handleTableRemove(item, i) { item._fields.splice(i, 1); handleFieldChange(item); }

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
      configKey: command.configKey, configName: tpl.configName,
      configType: tpl.configType, configValue: "", remark: "",
      _schema: tpl.schema, _fields: parseFields(tpl.schema, ""), _span: tpl._span,
      sortBy: null, _saving: false, _saved: false, _dirty: false,
    }));
  }
  nextTick(() => initSortable());
}

function handleDelete(item) {
  const name = item.configName || item.configKey || "未命名";
  ElMessageBox.confirm(`是否确认删除配置"${name}"？`, "提示", {
    confirmButtonText: "确定删除", cancelButtonText: "取消", confirmButtonType: "danger", type: "warning",
  }).then(() => {
    if (item.configId) return delConfig(item.configId);
    const i = configItems.value.findIndex((x) => x._key === item._key);
    if (i > -1) configItems.value.splice(i, 1);
    ElMessage.success("已移除");
  }).then((res) => {
    if (res) {
      const i = configItems.value.findIndex((x) => x._key === item._key);
      if (i > -1) configItems.value.splice(i, 1);
      ElMessage.success("删除成功");
    }
  }).catch(() => {});
}

function handleRefreshCache() { refreshCache().then(() => ElMessage.success("刷新缓存成功")); }

onBeforeUnmount(() => {
  Object.values(saveTimers).forEach((t) => clearTimeout(t));
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
  font-size: 13px; color: var(--el-text-color-secondary);
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
