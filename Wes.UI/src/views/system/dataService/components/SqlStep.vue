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
          <label class="form-label">SQL语句</label>
          <CodeEditor
            ref="codeEditorRef"
            :model-value="localPartConfig"
            @update:model-value="updatePartConfig"
            language="sql"
            placeholder="请输入SQL语句，可以使用 :paramName 作为参数"
            height="calc(100vh - 219px)"
          />
        </div>

        <!-- 右侧参数编辑 -->
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
                  <el-radio :label="3">增删改</el-radio>
                </el-radio-group>
              </div>
            </div>
          </div>
          <div class="param-section flex-1">
            <div class="section-divider"></div>
            <div class="section-header">
              <span class="form-label">输入变量</span>
              <el-button size="small" type="primary" plain @click="addParam">+ 添加</el-button>
            </div>
            <div class="param-list">
              <div v-for="(param, index) in params" :key="index" class="param-item">
                <el-input
                  v-model="param.key"
                  placeholder="变量名"
                  size="small"
                  @change="updateParams"
                />
                <el-select v-model="param.type" size="small" @change="updateParams">
                  <el-option label="字符串" value="string" />
                  <el-option label="数字" value="number" />
                  <el-option label="布尔" value="boolean" />
                  <el-option label="日期" value="date" />
                  <el-option label="对象" value="object" />
                </el-select>
                <el-button
                  size="small"
                  type="danger"
                  plain
                  :icon="Delete"
                  @click="removeParam(index)"
                />
              </div>
              <div v-if="params.length === 0" class="param-empty">
                暂无参数，点击上方添加
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, watch, onMounted } from "vue";
import { getTables, getTableFields } from "@/api/system/dataService";
import CodeEditor from "@/components/CodeEditor";
import { Folder, Document, Search, Delete, QuestionFilled } from "@element-plus/icons-vue";

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

// 请求参数列表
const params = ref([]);

// 初始化参数
function initParams() {
  try {
    const config = props.formData.paramConfig;
    if (config && typeof config === "string" && config.trim()) {
      params.value = JSON.parse(config);
    } else if (Array.isArray(config)) {
      params.value = config;
    } else {
      params.value = [];
    }
  } catch (e) {
    params.value = [];
  }
}

// 更新参数到父组件（存储在 formData.paramConfig）
function updateParams() {
  emit("update:formData", {
    ...props.formData,
    paramConfig: JSON.stringify(params.value),
  });
}

// 添加参数
function addParam() {
  params.value.push({
    key: "",
    type: "string",
  });
  updateParams();
}

// 删除参数
function removeParam(index) {
  params.value.splice(index, 1);
  updateParams();
}

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

// 组件挂载时初始化参数
onMounted(() => {
  initParams();
});

// 监听 formData 变化，重新初始化参数
watch(
  () => props.formData.paramConfig,
  () => {
    initParams();
  },
  { deep: true }
);

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

/* 右侧参数面板 */
.param-panel {
  width: 250px;
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

.param-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 8px;
}

.param-header .form-label {
  margin-bottom: 0;
}

.param-list {
  flex: 1;
  overflow-y: auto;
  display: flex;
  flex-direction: column;
  gap: 8px;
}

.param-item {
  display: flex;
  align-items: center;
  gap: 6px;
}

.param-item .el-input {
  flex: 1;
  min-width: 0;
}

.param-item .el-select {
  width: 90px;
  flex-shrink: 0;
}

.param-empty {
  text-align: center;
  color: var(--text-secondary);
  font-size: 12px;
  padding: 20px 0;
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

.help-icon {
  color: var(--text-secondary);
  cursor: pointer;
  font-size: 12px;
}

.inline-help {
  color: var(--text-secondary);
  cursor: pointer;
  font-size: 10px;
  margin-left: 4px;
}

.section-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 12px;
}

.section-divider {
  height: 1px;
  background: var(--border-color);
  margin: 16px 0;
}
</style>
