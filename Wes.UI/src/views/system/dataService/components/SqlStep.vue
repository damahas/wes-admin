<template>
  <div class="step-wrapper">
    <div class="step-header">
      <div class="header-left">
        <div class="header-icon sql-icon">
          <svg
            width="18"
            height="18"
            viewBox="0 0 24 24"
            fill="none"
            stroke="currentColor"
            stroke-width="2"
          >
            <ellipse cx="12" cy="6" rx="8" ry="3" />
            <path d="M4 6v6c0 1.657 3.582 3 8 3s8-1.343 8-3V6" />
            <path d="M4 12v6c0 1.657 3.582 3 8 3s8-1.343 8-3v-6" />
          </svg>
        </div>
        <div class="header-text">
          <h3>SQL配置</h3>
          <p>配置SQL查询语句和变量参数</p>
        </div>
      </div>
      <el-button type="danger" plain @click="emit('delete')"> 删除 </el-button>
    </div>

    <div class="step-body">
      <div class="form-group flex-1 content-area">
        <!-- 左侧表结构树 -->
        <div class="table-tree">
          <div class="tree-header">
            <el-input
              v-model="searchText"
              placeholder="搜索表名"
              prefix-icon="Search"
              size="small"
              clearable
            />
          </div>
          <div class="tree-content">
            <el-tree
              ref="treeRef"
              :data="treeData"
              :props="treeProps"
              :expand-on-click-node="false"
              :filter-node-method="filterNode"
              :load="loadNode"
              lazy
              node-key="id"
            >
              <template #default="{ node, data }">
                <span
                  class="tree-node"
                  @click.stop="handleNodeClick(data, node)"
                >
                  <el-icon v-if="data.isLeaf"><Document /></el-icon>
                  <el-icon v-else><Folder /></el-icon>
                  <el-tooltip
                    :content="data.description || data.label"
                    placement="top"
                    :disabled="!data.description"
                  >
                    <span class="node-label">
                      {{ data.label }}
                      <span v-if="data.description" class="node-desc">({{ data.description }})</span>
                    </span>
                  </el-tooltip>
                  <el-tag
                    v-if="data.dbColumnName"
                    size="small"
                    type="info"
                    class="type-tag"
                  >
                    {{ data.dataType }}
                  </el-tag>
                </span>
              </template>
            </el-tree>
          </div>
        </div>

        <!-- 中间编辑器 -->
        <div class="editor-area">
          <div class="editor-header">
            <label class="form-label" style="margin-bottom:0">SQL语句</label>
            <el-dropdown trigger="click" @command="insertVariable" class="var-dropdown">
              <span class="var-dropdown-link">
                <el-icon><CirclePlus /></el-icon> 插入变量
              </span>
              <template #dropdown>
                <el-dropdown-menu>
                  <el-dropdown-item disabled class="dropdown-group-title">输入变量</el-dropdown-item>
                  <el-dropdown-item
                    v-for="v in inputVarList"
                    :key="v.key"
                    :command="`#{${v.key}}`"
                  >
                    <div class="var-item-row">
                      <code class="var-name">#{{ '{' + v.key + '}' }}</code>
                      <span class="var-type-tag">{{ typeLabel(v.type) }}</span>
                    </div>
                  </el-dropdown-item>
                  <el-dropdown-item v-if="inputVarList.length === 0" disabled class="dropdown-empty">
                    暂无，请在基础信息中配置
                  </el-dropdown-item>
                  <el-dropdown-item divided disabled class="dropdown-group-title">内置变量</el-dropdown-item>
                  <el-dropdown-item
                    v-for="v in builtinVars"
                    :key="v.key"
                    :command="`#{sys.${v.key}}`"
                  >
                    <div class="var-item-row">
                      <code class="var-name">#{{ '{sys.' + v.key + '}' }}</code>
                      <span class="var-desc">{{ v.desc }}</span>
                    </div>
                  </el-dropdown-item>
                </el-dropdown-menu>
              </template>
            </el-dropdown>
          </div>
          <CodeEditor
            ref="codeEditorRef"
            :model-value="localPartConfig"
            @update:model-value="updatePartConfig"
            language="sql"
            placeholder="请输入SQL语句，可以使用 #{paramName} 作为参数"
            height="calc(100vh - 243px)"
          />
        </div>

        <!-- 右侧面板：输出变量 + AI聊天 -->
        <div class="param-panel">
          <div class="param-section">
            <div class="section-header">
              <span class="form-label">输出变量</span>
            </div>
            <div class="output-var-group">
              <div class="output-var-row">
                <label class="var-label"><span class="required">*</span>变量名</label>
                <el-input
                  :model-value="props.node.varName"
                  @update:model-value="updateVarName"
                  placeholder="请输入变量名"
                  size="small"
                  clearable
                />
              </div>
              <div class="output-var-row">
                <label class="var-label"><span class="required">*</span>输出类型</label>
                <el-radio-group
                  :model-value="props.node.varType"
                  @update:model-value="updateVarType"
                  size="small"
                >
                  <el-radio :label="0">列表对象</el-radio>
                  <el-radio :label="1">对象</el-radio>
                  <el-radio :label="2">单个值</el-radio>
                  <el-radio :label="3">执行结果</el-radio>
                </el-radio-group>
              </div>
            </div>
          </div>

          <!-- AI 聊天区域 -->
          <div class="param-section flex-1 ai-section">
            <div class="section-divider"></div>
            <div class="section-header">
              <span class="form-label">AI 助手</span>
              <div class="section-header-actions">
                <el-dropdown trigger="click" @command="selectModelCmd" class="model-dropdown">
                  <span class="model-dropdown-link">
                    {{ modelLabel }} <i class="el-icon-arrow-down el-icon--right" style="font-size:11px" />
                  </span>
                  <template #dropdown>
                    <el-dropdown-menu>
                      <el-dropdown-item
                        v-for="m in models"
                        :key="m.modelId"
                        :command="m.modelId"
                        :class="{ 'is-active': m.modelId === selectedModel }"
                      >
                        {{ providers.find(p => p.name === m.provider)?.displayName || m.provider }} · {{ m.displayName }}
                      </el-dropdown-item>
                    </el-dropdown-menu>
                  </template>
                </el-dropdown>
                <el-button
                  size="small"
                  type="danger"
                  plain
                  :disabled="chatMessages.length === 0"
                  @click="clearChat"
                >清空</el-button>
              </div>
            </div>

            <!-- 消息列表 -->
            <div ref="chatMsgRef" class="chat-messages">
              <div v-if="chatMessages.length === 0" class="chat-empty">
                <svg viewBox="0 0 24 24" fill="none" width="36" height="36">
                  <path d="M20 2H4c-1.1 0-2 .9-2 2v18l4-4h14c1.1 0 2-.9 2-2V4c0-1.1-.9-2-2-2z" fill="var(--theme-color)" opacity="0.12"/>
                  <circle cx="8" cy="10" r="1.5" fill="var(--theme-color)" opacity="0.4"/>
                  <circle cx="12" cy="10" r="1.5" fill="var(--theme-color)" opacity="0.4"/>
                  <circle cx="16" cy="10" r="1.5" fill="var(--theme-color)" opacity="0.4"/>
                </svg>
                <p>输入需求，AI 帮你生成 SQL</p>
              </div>
              <div
                v-for="(msg, i) in chatMessages"
                :key="i"
                class="chat-msg"
                :class="`chat-msg--${msg.role}`"
              >
                <div class="chat-msg__content">
                  <div v-if="msg.role === 'assistant'" class="chat-msg__text" v-html="renderMarkdownText(msg.content)"></div>
                  <div v-else class="chat-msg__text">{{ msg.content }}</div>
                  <!-- 提取到的 SQL 操作按钮 -->
                  <div
                    v-if="msg.role === 'assistant' && msg.sqlBlocks && msg.sqlBlocks.length"
                    class="chat-sql-actions"
                  >
                    <div
                      v-for="(sql, si) in msg.sqlBlocks"
                      :key="si"
                      class="chat-sql-block"
                    >
                      <code class="chat-sql-preview">{{ sql }}</code>
                      <el-button
                        size="small"
                        type="success"
                        plain
                        @click="applySql(sql)"
                      >应用</el-button>
                    </div>
                  </div>
                </div>
              </div>
              <!-- 加载动画 -->
              <div v-if="chatLoading" class="chat-msg chat-msg--assistant">
                <div class="chat-msg__content">
                  <div class="chat-typing"><span /><span /><span /></div>
                </div>
              </div>
            </div>

            <!-- 输入区 -->
            <div class="chat-input-area">
              <el-input
                v-model="chatInput"
                placeholder="描述你想要的 SQL..."
                size="small"
                :disabled="chatLoading"
                @keydown.enter.exact="sendChat"
              >
                <template #append>
                  <el-button
                    :icon="chatLoading ? 'Close' : 'Promotion'"
                    :disabled="!chatLoading && !chatInput.trim()"
                    @click="chatLoading ? stopChat() : sendChat()"
                  />
                </template>
              </el-input>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, watch, nextTick, onMounted } from "vue";
import { getTables, getTableFields } from "@/api/system/dataService";
import { agentChatStream, getProviders, getModels } from "@/api/ai";
import CodeEditor from "@/components/CodeEditor";
import { Folder, Document, Search, CirclePlus } from "@element-plus/icons-vue";

const props = defineProps({
  node: {
    type: Object,
    required: true,
  },
  formData: {
    type: Object,
    default: () => ({}),
  },
});

const emit = defineEmits(["update:node", "update:formData"]);

// 本地副本用于编辑
const localPartConfig = ref(props.node.partConfig || "");
const treeData = ref([]);
const searchText = ref("");
const treeRef = ref(null);
const codeEditorRef = ref(null);

// ==================== AI 聊天 ====================
const chatMessages = ref([]);
const chatInput = ref("");
const chatLoading = ref(false);
const chatMsgRef = ref(null);
let chatAbort = null;

// 模型选择
const providers = ref([]);
const models = ref([]);
const selectedModel = ref("");

const modelLabel = computed(() => {
  const m = models.value.find(x => x.modelId === selectedModel.value);
  if (!m) return "选择模型";
  const p = providers.value.find(x => x.name === m.provider);
  return `${p?.displayName || m.provider} · ${m.displayName}`;
});

// 当前选中的 provider（根据 model 反查）
const selectedProvider = computed(() => {
  const m = models.value.find(x => x.modelId === selectedModel.value);
  return m?.provider || "";
});

onMounted(async () => {
  try {
    const pr = await getProviders();
    if (pr.code === 200 && pr.data) providers.value = pr.data;
  } catch { /* 静默 */ }
  try {
    const mr = await getModels();
    if (mr.code === 200 && mr.data) {
      models.value = mr.data;
      // 默认选中默认提供商的默认模型
      if (mr.data.length > 0) {
        const defProvider = providers.value.find(p => p.isDefault);
        const defModel = defProvider
          ? mr.data.find(m => m.provider === defProvider.name)
          : mr.data[0];
        selectedModel.value = defModel?.modelId || mr.data[0]?.modelId || "";
      }
    }
  } catch { /* 静默 */ }
});

function selectModelCmd(modelId) {
  selectedModel.value = modelId;
}

// 简单的 Markdown 渲染（代码块、行内代码、粗体）
function renderMarkdownText(text) {
  if (!text) return "";
  let html = text
    .replace(/&/g, "&amp;")
    .replace(/</g, "&lt;")
    .replace(/>/g, "&gt;");
  // 代码块
  html = html.replace(/```(\w*)\n?([\s\S]*?)```/g, (_, lang, code) => {
    const escaped = code.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
    return `<pre class="chat-code-block"><code>${escaped}</code></pre>`;
  });
  // 行内代码
  html = html.replace(/`([^`]+)`/g, "<code>$1</code>");
  // 粗体
  html = html.replace(/\*\*(.+?)\*\*/g, "<strong>$1</strong>");
  // 换行
  html = html.replace(/\n/g, "<br>");
  return html;
}

// 从文本中提取 SQL 代码块
function extractSqlBlocks(text) {
  const blocks = [];
  const regex = /```(?:sql)?\s*\n?([\s\S]*?)```/gi;
  let match;
  while ((match = regex.exec(text)) !== null) {
    const sql = match[1].trim();
    if (sql) blocks.push(sql);
  }
  return blocks;
}

function scrollChatBottom() {
  nextTick(() => {
    const el = chatMsgRef.value;
    if (el) el.scrollTop = el.scrollHeight;
  });
}

async function sendChat() {
  const text = chatInput.value.trim();
  if (!text || chatLoading.value) return;
  chatInput.value = "";

  chatMessages.value.push({ role: "user", content: text });
  scrollChatBottom();
  chatLoading.value = true;

  let content = "";
  const msgIdx = chatMessages.value.length;
  chatMessages.value.push({ role: "assistant", content: "", sqlBlocks: [] });

  // 收集当前可用表名，帮助 AI 生成准确 SQL
  const tableNames = [];
  function collectTables(nodes) {
    for (const n of nodes) {
      if (n.name) tableNames.push(n.name);
      if (n.children) collectTables(n.children);
    }
  }
  collectTables(treeData.value);

  const tableHint = tableNames.length > 0
    ? `\n数据库可用表：${tableNames.join(', ')}`
    : "";

  // 收集可用变量
  const varParts = [];
  const inputVars = inputVarList.value;
  if (inputVars.length > 0) {
    varParts.push(`输入变量（引用: #{变量名}）：${inputVars.map(v => `${v.key}(${typeLabel(v.type)})`).join(', ')}`);
  }
  varParts.push(`内置变量（引用: #{sys.变量名}）：${builtinVars.map(v => `sys.${v.key}(${v.desc})`).join(', ')}`);
  const varHint = varParts.length > 0 ? `\n可用的参数变量：\n${varParts.join('\n')}` : "";

  // 带入当前编辑器 SQL，便于 AI 基于现有语句修改
  const currentSql = localPartConfig.value?.trim();
  const sqlHint = currentSql
    ? `\n当前 SQL 语句：\n\`\`\`sql\n${currentSql}\n\`\`\``
    : "";

  chatAbort = agentChatStream(
    "sql_expert",
    {
      provider: selectedProvider.value,
      model: selectedModel.value,
      message: `请仅生成完整可执行的 SQL 查询语句，不要执行、不要解释，不要只返回修改片段。需求：${text}${tableHint}${varHint}${sqlHint}`,
      history: [],
    },
    {
      onChunk(chunk) {
        if (chunk.error) {
          content = `[错误] ${chunk.error}`;
          chatMessages.value[msgIdx].content = content;
          return;
        }
        if (chunk.content) {
          content += chunk.content;
          chatMessages.value[msgIdx].content = content;
        }
        scrollChatBottom();
      },
      onDone() {
        chatLoading.value = false;
        chatMessages.value[msgIdx].sqlBlocks = extractSqlBlocks(content);
        if (!content && !chatMessages.value[msgIdx].sqlBlocks.length) {
          chatMessages.value[msgIdx].content = "未收到有效回复，请重试。";
        }
        chatAbort = null;
      },
      onError(err) {
        chatLoading.value = false;
        chatMessages.value[msgIdx].content = content || `请求失败: ${err?.message || '请重试'}`;
        chatMessages.value[msgIdx].sqlBlocks = extractSqlBlocks(content);
        chatAbort = null;
      },
    }
  );
}

function stopChat() {
  chatAbort?.();
  chatLoading.value = false;
  chatAbort = null;
}

function clearChat() {
  chatMessages.value = [];
}

function applySql(sql) {
  localPartConfig.value = sql;
  emit("update:node", {
    ...props.node,
    partConfig: sql,
  });
}

// ==================== 输出变量 ====================

// 更新输出变量名
function updateVarName(value) {
  emit("update:node", {
    ...props.node,
    varName: value,
  });
}

// 更新输出类型
function updateVarType(value) {
  emit("update:node", {
    ...props.node,
    varType: value,
  });
}

// 树形组件配置
const treeProps = {
  children: "children",
  label: "label",
  isLeaf: "isLeaf",
};

// 监听搜索文本
watch(searchText, (val) => {
  treeRef.value?.filter(val);
});

// 过滤节点
function filterNode(value, data) {
  if (!value) return true;
  return data.label.toLowerCase().includes(value.toLowerCase());
}

// 懒加载节点
async function loadNode(node, resolve) {
  // 根节点加载表列表
  if (node.level === 0) {
    try {
      const res = await getTables();
      if (res.code === 200 && res.data) {
        const tables = res.data.map((item) => ({
          id: item.name,
          label: item.name,
          name: item.name,
          description: item.description,
          dbObjectType: item.dbObjectType,
          isLeaf: false,
        }));
        resolve(tables);
      } else {
        resolve([]);
      }
    } catch (error) {
      console.error("加载表列表失败:", error);
      resolve([]);
    }
  } else {
    // 子节点加载字段
    const tableName = node.data.name || node.data.label;
    try {
      const res = await getTableFields(tableName);
      if (res.code === 200 && res.data) {
        const fields = res.data.map((field) => ({
          id: `${tableName}.${field.dbColumnName}`,
          label: field.dbColumnName,
          name: field.dbColumnName,
          tableName: field.tableName,
          dbColumnName: field.dbColumnName,
          propertyName: field.propertyName,
          dataType: field.dataType,
          isPrimarykey: field.isPrimarykey,
          isNullable: field.isNullable,
          length: field.length,
          isLeaf: true,
        }));
        resolve(fields);
      } else {
        resolve([]);
      }
    } catch (error) {
      console.error("加载表字段失败:", error);
      resolve([]);
    }
  }
}

// 节点点击处理 - 插入到编辑器
function handleNodeClick(data, node) {
  // 如果是字段节点，插入字段名
  if (data.dbColumnName) {
    insertToEditor(data.label);
  } else if (data.name) {
    // 如果是表节点，插入表名
    insertToEditor(data.name);
  }
}

// 插入文本到编辑器
function insertToEditor(text) {
  if (codeEditorRef.value) {
    codeEditorRef.value.insertText(text);
  } else {
    // 备选方案：直接在当前光标位置追加
    localPartConfig.value += text;
  }
}

// ==================== 变量选择 ====================

// 内置系统变量
const builtinVars = [
  { key: 'userId', desc: '当前用户ID' },
  { key: 'userName', desc: '当前用户名' },
  { key: 'userAccount', desc: '当前用户账号' },
  { key: 'deptId', desc: '当前部门ID' },
  { key: 'deptName', desc: '当前部门名称' },
  { key: 'now', desc: '当前日期时间' },
  { key: 'today', desc: '当前日期' },
];

// 从 formData 解析输入变量列表
const inputVarList = computed(() => {
  try {
    const config = props.formData?.paramConfig;
    if (config) {
      const arr = typeof config === 'string' ? JSON.parse(config) : config;
      return Array.isArray(arr) ? arr.filter(v => v.key) : [];
    }
  } catch { /* 解析失败 */ }
  return [];
});

function typeLabel(type) {
  const map = { string: '字符串', number: '数字', boolean: '布尔', date: '日期', object: '对象' };
  return map[type] || type;
}

// 点击下拉项：将 :varName 插入编辑器光标位置
function insertVariable(varName) {
  insertToEditor(varName);
}

// 监听props.node.partConfig变化
watch(
  () => props.node.partConfig,
  (newPartConfig) => {
    if (newPartConfig !== localPartConfig.value) {
      localPartConfig.value = newPartConfig || "";
    }
  }
);

// 更新partConfig
function updatePartConfig(newValue) {
  localPartConfig.value = newValue;
  emit("update:node", {
    ...props.node,
    partConfig: newValue,
  });
}
</script>

<style scoped>
.step-wrapper {
  width: 100%;
  height: 100%;
  display: flex;
  flex-direction: column;
}

.step-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 16px 20px;
  background: var(--bg-hover);
  border-radius: 12px 12px 0 0;
}

.header-left {
  display: flex;
  align-items: center;
  gap: 12px;
}

.header-icon {
  width: 40px;
  height: 40px;
  border-radius: 10px;
  display: flex;
  align-items: center;
  justify-content: center;
}

.sql-icon {
  background: linear-gradient(135deg, #8BA095 0%, #6E8472 100%);
  color: #141614;
}

.header-text h3 {
  margin: 0;
  font-size: 16px;
  font-weight: 600;
  color: var(--el-text-color-primary);
}

.header-text p {
  margin: 2px 0 0;
  font-size: 12px;
  color: var(--el-text-color-secondary);
}

.delete-btn {
  color: var(--color-danger);
  border-color: var(--color-danger);
  background: transparent;
}

.delete-btn:hover {
  color: #141614;
  background: var(--color-danger);
  border-color: var(--color-danger);
}

.step-body {
  flex: 1;
  padding: 16px;
  display: flex;
  flex-direction: column;
  gap: 16px;
  background: var(--bg-card);
  border-radius: 0 0 12px 12px;
  border: 1px solid var(--border-color);
  border-top: none;
}

.form-group {
  display: flex;
  flex-direction: column;
}

.form-group.flex-1 {
  flex: 1;
  min-height: 0;
}

.content-area {
  flex-direction: row;
  gap: 16px;
  flex: 1;
}

.table-tree {
  width: 250px;
  flex-shrink: 0;
  display: flex;
  flex-direction: column;
  border: 1px solid var(--border-color);
  border-radius: 8px;
  overflow: hidden;
  max-height: calc(100vh - 194px);
  background: var(--bg-card);
}

.tree-header {
  padding: 12px;
  border-bottom: 1px solid var(--border-color);
  background: var(--bg-hover);
}

.tree-header .form-label {
  margin-bottom: 8px;
}

.tree-content {
  flex: 1;
  overflow: auto;
  padding: 8px;
}

.tree-node {
  display: flex;
  align-items: center;
  gap: 6px;
  flex: 1;
  min-width: 0;
}

.tree-node .el-icon {
  color: var(--el-color-primary);
  flex-shrink: 0;
}

.node-label {
  display: flex;
  align-items: center;
  gap: 4px;
  overflow: hidden;
  white-space: nowrap;
  min-width: 0;
}

.node-desc {
  color: var(--text-secondary);
  font-size: 12px;
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
  flex-shrink: 1;
  min-width: 0;
}

.type-tag {
  margin-left: auto;
  font-size: 10px;
}

.editor-area {
  flex: 1;
  display: flex;
  flex-direction: column;
  min-width: 0;
}

.form-label {
  margin-bottom: 8px;
  font-size: 13px;
  font-weight: 500;
  color: var(--el-text-color-regular);
}

.editor-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 8px;
}

.var-dropdown {
  line-height: 1;
}

.var-dropdown-link {
  display: inline-flex;
  align-items: center;
  gap: 4px;
  cursor: pointer;
  font-size: 12px;
  color: var(--theme-color);
  padding: 3px 8px;
  border-radius: 4px;
  border: 1px dashed var(--theme-color);
  user-select: none;
  transition: background 0.2s;
}

.var-dropdown-link:hover {
  background: var(--theme-color-light);
}

.var-dropdown-link .el-icon {
  font-size: 13px;
}

/* 下拉项 */
.var-item-row {
  display: flex;
  align-items: center;
  gap: 8px;
}

.var-name {
  font-family: 'JetBrains Mono', 'Consolas', monospace;
  font-size: 12px;
  color: var(--theme-color);
  background: var(--theme-color-light);
  padding: 1px 5px;
  border-radius: 3px;
}

.var-type-tag {
  font-size: 11px;
  color: var(--text-placeholder);
}

.var-desc {
  font-size: 11px;
  color: var(--text-secondary);
}

.dropdown-group-title {
  font-size: 11px;
  color: var(--text-placeholder);
  padding: 4px 12px 2px;
}

.dropdown-empty {
  font-size: 11px;
  color: var(--text-placeholder);
  padding: 4px 20px;
}

/* 右侧面板 */
.param-panel {
  width: 350px;
  flex-shrink: 0;
  display: flex;
  flex-direction: column;
}

.param-section {
  margin-bottom: 16px;
}

.param-section.flex-1 {
  flex: 1;
  display: flex;
  flex-direction: column;
  min-height: 0;
}

.param-section .form-label {
  margin-bottom: 8px;
}

.form-label .required {
  color: var(--color-danger);
  margin-right: 4px;
}

/* AI 聊天区域 */
.ai-section {
  margin-bottom: 0;
}

.chat-messages {
  flex: 1;
  overflow-y: auto;
  display: flex;
  flex-direction: column;
  gap: 8px;
  margin-bottom: 8px;
  min-height: 0;
}

.chat-empty {
  flex: 1;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  gap: 8px;
  color: var(--text-placeholder);
  font-size: 12px;
  padding: 16px 0;
}

.chat-msg {
  display: flex;
}

.chat-msg--user {
  justify-content: flex-end;
}

.chat-msg--user .chat-msg__content {
  background: var(--theme-color-light, rgba(107, 163, 104, 0.1));
  border-radius: 10px 10px 2px 10px;
  max-width: 100%;
}

.chat-msg--assistant .chat-msg__content {
  background: var(--bg-hover);
  border-radius: 10px 10px 10px 2px;
  max-width: 100%;
}

.chat-msg__content {
  padding: 8px 10px;
  min-width: 0;
}

.chat-msg__text {
  font-size: 13px;
  line-height: 1.55;
  word-break: break-word;
  color: var(--text-primary);
}

/* 聊天中的代码块 */
.chat-code-block {
  background: var(--bg-secondary, #f4f6f8);
  border-radius: 6px;
  padding: 8px 10px;
  margin: 4px 0;
  overflow-x: auto;
  font-size: 12px;
  font-family: 'JetBrains Mono', 'Consolas', monospace;
  line-height: 1.5;
}

.chat-code-block code {
  font-family: inherit;
  background: none;
  padding: 0;
  color: var(--text-primary);
}

/* SQL 操作按钮区域 */
.chat-sql-actions {
  margin-top: 6px;
  display: flex;
  flex-direction: column;
  gap: 5px;
}

.chat-sql-block {
  display: flex;
  flex-direction: column;
  gap: 3px;
  padding: 6px 8px;
  background: var(--bg-card);
  border: 1px solid var(--border-color);
  border-radius: 6px;
}

.chat-sql-preview {
  font-size: 11px;
  font-family: 'JetBrains Mono', 'Consolas', monospace;
  color: var(--text-secondary);
  white-space: pre-wrap;
  word-break: break-all;
  max-height: 80px;
  overflow-y: auto;
  display: block;
}

/* 加载动画 */
.chat-typing {
  display: flex;
  align-items: center;
  gap: 3px;
  padding: 4px 0;
}

.chat-typing span {
  width: 5px;
  height: 5px;
  border-radius: 50%;
  background: var(--text-placeholder);
  animation: chat-bounce 1.2s ease-in-out infinite;
}

.chat-typing span:nth-child(2) { animation-delay: 0.15s; }
.chat-typing span:nth-child(3) { animation-delay: 0.3s; }

@keyframes chat-bounce {
  0%, 60%, 100% { transform: translateY(0); opacity: 0.4; }
  30% { transform: translateY(-5px); opacity: 1; }
}

/* 输入区 */
.chat-input-area {
  flex-shrink: 0;
}

/* 输出变量组 */
.output-var-group {
  display: flex;
  flex-direction: column;
  gap: 10px;
}

.output-var-row {
  display: flex;
  flex-direction: column;
  gap: 4px;
}

.var-label {
  font-size: 12px;
  color: var(--el-text-color-secondary);
  display: flex;
  align-items: center;
  gap: 4px;
}

.var-label .required {
  color: var(--color-danger);
}

.section-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 12px;
}

.section-header-actions {
  display: flex;
  align-items: center;
  gap: 8px;
}

.model-dropdown {
  line-height: 1;
}

.model-dropdown-link {
  display: inline-flex;
  align-items: center;
  gap: 4px;
  cursor: pointer;
  font-size: 12px;
  color: var(--text-secondary);
  padding: 2px 6px;
  border-radius: 4px;
  user-select: none;
  transition: color 0.2s, background 0.2s;
}

.model-dropdown-link:hover {
  color: var(--theme-color);
  background: var(--theme-color-light);
}

.section-divider {
  height: 1px;
  background: var(--border-color);
  margin: 16px 0;
}

/* 暗色模式 */
html.dark .chat-code-block {
  background: rgba(255, 255, 255, 0.06);
}

html.dark .chat-msg--user .chat-msg__content {
  background: rgba(140, 196, 136, 0.14);
}

html.dark .chat-msg--assistant .chat-msg__content {
  background: rgba(140, 196, 136, 0.06);
}

html.dark .var-name {
  background: rgba(140, 196, 136, 0.12);
}

html.dark .var-dropdown-link:hover {
  background: rgba(140, 196, 136, 0.1);
}

/* 模型下拉激活态 */
:deep(.model-dropdown .el-dropdown-menu__item.is-active) {
  color: var(--theme-color);
  font-weight: 600;
}
</style>
