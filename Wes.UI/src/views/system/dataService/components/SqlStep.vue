<template>
  <div class="sql-step-container">
    <el-card shadow="never" class="step-card">
      <template #header>
        <div class="card-header">
          <span>SQL配置步骤</span>
          <div class="header-actions">
            <el-tag type="success">SQL</el-tag>
          </div>
        </div>
      </template>
      
      <div class="sql-layout">
        <!-- 左侧：表字段树结构（预留） -->
        <div class="left-panel">
          <div class="panel-header">
            <h3>表字段结构</h3>
            <el-tag type="info">开发中</el-tag>
          </div>
          <div class="panel-content">
            <el-empty description="表字段树结构开发中" />
          </div>
        </div>
        
        <!-- 中间：SQL编辑器区域 -->
        <div class="center-panel">
          <div class="panel-header">
            <h3>SQL配置</h3>
          </div>
          <div class="panel-content">
            <!-- 变量名输入 -->
            <div class="var-name-input">
              <el-input
                :model-value="props.node.varName"
                @update:model-value="value => emit('update:node', { ...props.node, varName: value })"
                placeholder="请输入节点变量名称"
                maxlength="100"
              >
                <template #prepend>变量名</template>
              </el-input>
            </div>
            
            <!-- SQL编辑器 -->
            <div class="editor-container">
              <div class="editor-header">
                <span>SQL语句</span>
                <el-tag size="small">支持参数化查询</el-tag>
              </div>
              <CodeEditor
                :model-value="localPartConfig"
                @update:model-value="updatePartConfig"
                language="sql"
                placeholder="请输入SQL语句，可以使用 :paramName 作为参数"
                :height="300"
              />
              <div class="editor-tips">
                <el-alert title="SQL编写提示" type="info" :closable="false">
                  <ul>
                    <li>使用 :参数名 的方式定义参数，如：SELECT * FROM users WHERE id = :userId</li>
                    <li>支持多行SQL语句</li>
                    <li>复杂的SQL建议在右侧配置参数</li>
                    <li>支持SQL语法高亮和代码提示</li>
                  </ul>
                </el-alert>
              </div>
            </div>
          </div>
        </div>
        
        <!-- 右侧：变量配置和AI -->
        <div class="right-panel">
          <!-- 变量配置 -->
          <div class="variables-panel">
            <div class="panel-header">
              <h3>变量配置</h3>
              <el-button type="primary" plain icon="Plus" size="small" @click="addVariable">
                添加变量
              </el-button>
            </div>
            <div class="panel-content">
              <div v-if="variables.length === 0" class="empty-variables">
                <el-empty description="暂无变量配置" size="small" />
              </div>
              
              <div v-else class="variables-list">
                <div v-for="(variable, index) in variables" :key="index" class="variable-item">
                  <el-input
                    v-model="variable.param"
                    placeholder="变量名"
                    style="width: 120px; margin-right: 10px;"
                    @input="updateVariables"
                  />
                  <el-select
                    v-model="variable.type"
                    placeholder="类型"
                    style="width: 100px; margin-right: 10px;"
                    @change="updateVariables"
                  >
                    <el-option label="字符串" value="string" />
                    <el-option label="数字" value="number" />
                    <el-option label="布尔值" value="boolean" />
                    <el-option label="日期" value="date" />
                    <el-option label="数组" value="array" />
                    <el-option label="对象" value="object" />
                  </el-select>
                  <el-input
                    v-model="variable.description"
                    placeholder="描述"
                    style="flex: 1; margin-right: 10px;"
                    @input="updateVariables"
                  />
                  <el-button
                    link
                    type="danger"
                    icon="Delete"
                    @click="removeVariable(index)"
                  />
                </div>
              </div>
              
              <div class="variables-tips">
                <el-alert title="变量配置说明" type="warning" :closable="false">
                  <ul>
                    <li>变量名需与SQL中的 :参数名 保持一致</li>
                    <li>配置的变量会在调用时作为参数传入</li>
                    <li>变量配置会保存到 paramConfig 字段</li>
                  </ul>
                </el-alert>
              </div>
            </div>
          </div>
          
          <!-- AI对话区域（预留） -->
          <div class="ai-panel">
            <div class="panel-header">
              <h3>AI助手</h3>
              <el-tag type="info">开发中</el-tag>
            </div>
            <div class="panel-content">
              <el-empty description="AI对话功能开发中" />
            </div>
          </div>
        </div>
      </div>
    </el-card>
  </div>
</template>

<script setup>
import { ref, watch, computed } from 'vue'
import CodeEditor from '@/components/CodeEditor'

const props = defineProps({
  node: {
    type: Object,
    required: true
  },
  paramConfig: {
    type: Array,
    default: () => []
  }
})

const emit = defineEmits(['update:node', 'update:param-config'])

// 本地副本用于编辑
const localPartConfig = ref(props.node.partConfig || '')

// 变量配置 - 直接使用 props.paramConfig
const variables = ref([...props.paramConfig])

// 监听props.node.partConfig变化
watch(() => props.node.partConfig, (newPartConfig) => {
  if (newPartConfig !== localPartConfig.value) {
    localPartConfig.value = newPartConfig || ''
  }
})

// 监听paramConfig变化
watch(() => props.paramConfig, (newParamConfig) => {
  variables.value = [...newParamConfig]
}, { deep: true })

// 更新partConfig
function updatePartConfig(newValue) {
  localPartConfig.value = newValue
  emit('update:node', {
    ...props.node,
    partConfig: newValue
  })
}

// 添加变量
function addVariable() {
  variables.value.push({
    param: '',
    type: 'string',
    description: ''
  })
  updateVariables()
}

// 删除变量
function removeVariable(index) {
  variables.value.splice(index, 1)
  updateVariables()
}

// 更新变量配置
function updateVariables() {
  // 过滤掉空的变量
  const validVariables = variables.value.filter(v => v.param && v.type)
  emit('update:param-config', validVariables)
}
</script>

<style scoped>
.sql-step-container {
  height: 100%;
}

.step-card {
  height: 100%;
  min-height: 600px;
}

.card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.header-actions {
  display: flex;
  align-items: center;
  gap: 10px;
}

.sql-layout {
  display: flex;
  gap: 20px;
  height: calc(100% - 60px);
}

.left-panel,
.center-panel,
.right-panel {
  flex: 1;
  display: flex;
  flex-direction: column;
}

.right-panel {
  flex: 0.8;
  display: flex;
  flex-direction: column;
  gap: 20px;
}

.panel-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 15px;
  padding-bottom: 10px;
  border-bottom: 1px solid var(--el-border-color-light);
}

.panel-header h3 {
  margin: 0;
  font-size: 14px;
  font-weight: 600;
  color: var(--el-text-color-primary);
}

.panel-content {
  flex: 1;
  overflow-y: auto;
}

.var-name-input {
  margin-bottom: 20px;
}

.editor-container {
  flex: 1;
  display: flex;
  flex-direction: column;
}

.editor-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 10px;
}

.editor-tips {
  margin-top: 15px;
}

.editor-tips ul {
  margin: 5px 0;
  padding-left: 20px;
}

.editor-tips li {
  margin: 3px 0;
  font-size: 12px;
}

.variables-panel,
.ai-panel {
  flex: 1;
  display: flex;
  flex-direction: column;
  min-height: 250px;
}

.empty-variables {
  padding: 40px 0;
}

.variables-list {
  display: flex;
  flex-direction: column;
  gap: 10px;
}

.variable-item {
  display: flex;
  align-items: center;
  padding: 10px;
  background: #f8f9fa;
  border-radius: 4px;
}

.variables-tips {
  margin-top: 15px;
}

.variables-tips ul {
  margin: 5px 0;
  padding-left: 20px;
}

.variables-tips li {
  margin: 3px 0;
  font-size: 12px;
}

.ai-panel .panel-content {
  display: flex;
  align-items: center;
  justify-content: center;
}
</style>