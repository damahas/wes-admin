<template>
  <div class="editor-demo">
    <div class="demo-header">
      <h1>代码编辑器组件演示</h1>
      <p class="demo-desc">这是一个集成的代码编辑器组件，支持SQL、JavaScript和JSON编辑</p>
    </div>
    
    <div class="demo-content">
      <div class="demo-section">
        <h2>1. 基础使用</h2>
        
        <div class="demo-item">
          <h3>SQL编辑器示例</h3>
          <CodeEditor
            v-model="sqlCode"
            language="sql"
            placeholder="请输入SQL语句..."
            :height="250"
            @change="handleChange"
          />
        </div>
        
        <div class="demo-item">
          <h3>JavaScript编辑器示例</h3>
          <CodeEditor
            v-model="jsCode"
            language="javascript"
            placeholder="请输入JavaScript代码..."
            :height="250"
            @change="handleChange"
          />
        </div>
        
        <div class="demo-item">
          <h3>JSON编辑器示例</h3>
          <CodeEditor
            v-model="jsonCode"
            language="json"
            placeholder="请输入JSON数据..."
            :height="200"
            @change="handleChange"
          />
          </div>
        </div>
        
        <div class="demo-section">
          <h2>2. 自定义代码联想演示</h2>
          
          <div class="demo-item">
            <h3>带自定义代码提示的SQL编辑器</h3>
            <p>你可以传入自定义的代码提示词，这些提示词将与内置提示词合并显示。</p>
            
            <div class="custom-suggestions-control">
              <el-button @click="addCustomSuggestion" type="primary" size="small">
                添加自定义提示词
              </el-button>
              <el-button @click="clearCustomSuggestions" size="small">
                清空自定义提示
              </el-button>
              <span style="margin-left: 10px; color: #666; font-size: 13px;">
                当前有 {{ customSuggestions.length }} 个自定义提示词
              </span>
            </div>
            
            <CodeEditor
              v-model="customSqlCode"
              language="sql"
              placeholder="输入 'user' 或 'table' 查看自定义提示..."
              :height="250"
              :custom-suggestions="customSuggestions"
              @change="handleChange"
            />
            
            <div class="suggestion-examples">
              <h4>自定义提示词示例：</h4>
              <el-tag 
                v-for="item in customSuggestions" 
                :key="item.label"
                size="small"
                style="margin-right: 8px; margin-bottom: 8px;"
              >
                {{ item.label }}
              </el-tag>
            </div>
          </div>
        </div>
        
        <div class="demo-section">
          <h2>3. 在数据服务中的实际应用</h2>
          
          <div class="demo-app">
          <h3>模拟数据服务配置界面</h3>
          
          <el-card class="service-card">
            <template #header>
              <div class="card-header">
                <span>数据服务配置</span>
                <el-tag type="primary">演示模式</el-tag>
              </div>
            </template>
            
            <el-tabs v-model="activeTab">
              <el-tab-pane label="SQL步骤" name="sql">
                <div class="service-step">
                  <div class="step-info">
                    <el-input
                      v-model="stepName"
                      placeholder="步骤名称"
                      style="width: 200px; margin-bottom: 15px"
                    />
                    <el-input
                      v-model="varName"
                      placeholder="变量名"
                      style="width: 200px; margin-bottom: 15px"
                    />
                  </div>
                  
                  <div class="step-editor">
                    <h4>SQL配置</h4>
                    <CodeEditor
                      v-model="serviceSqlCode"
                      language="sql"
                      placeholder="请输入数据服务SQL..."
                      :height="200"
                    />
                  </div>
                  
                  <div class="step-params">
                    <h4>参数配置</h4>
                    <el-table :data="params" border style="width: 100%">
                      <el-table-column prop="name" label="参数名" width="120" />
                      <el-table-column prop="type" label="类型" width="100">
                        <template #default="scope">
                          <el-tag size="small">{{ scope.row.type }}</el-tag>
                        </template>
                      </el-table-column>
                      <el-table-column prop="required" label="必填" width="80">
                        <template #default="scope">
                          <el-tag v-if="scope.row.required" type="success" size="small">是</el-tag>
                          <el-tag v-else type="info" size="small">否</el-tag>
                        </template>
                      </el-table-column>
                      <el-table-column prop="defaultValue" label="默认值" />
                      <el-table-column prop="description" label="描述" />
                    </el-table>
                  </div>
                </div>
              </el-tab-pane>
              
              <el-tab-pane label="JavaScript步骤" name="javascript">
                <div class="service-step">
                  <div class="step-info">
                    <el-input
                      v-model="stepName"
                      placeholder="步骤名称"
                      style="width: 200px; margin-bottom: 15px"
                    />
                    <el-input
                      v-model="varName"
                      placeholder="变量名"
                      style="width: 200px; margin-bottom: 15px"
                    />
                  </div>
                  
                  <div class="step-editor">
                    <h4>JavaScript代码</h4>
                    <CodeEditor
                      v-model="serviceJsCode"
                      language="javascript"
                      placeholder="请输入数据处理逻辑..."
                      :height="250"
                    />
                  </div>
                  
                  <div class="step-help">
                    <el-alert title="代码提示" type="info" :closable="false">
                      <ul>
                        <li>可以使用 <code>params</code> 对象访问传入参数</li>
                        <li>可以使用 <code>context</code> 对象访问执行上下文</li>
                        <li>支持异步操作，使用 <code>async/await</code></li>
                        <li>最后一行会自动作为返回值</li>
                      </ul>
                    </el-alert>
                  </div>
                </div>
              </el-tab-pane>
            </el-tabs>
            
            <div class="service-actions">
              <el-button type="primary" @click="saveService">保存服务</el-button>
              <el-button @click="resetService">重置</el-button>
              <el-button @click="testService">测试执行</el-button>
            </div>
          </el-card>
        </div>
      </div>
      
      <div class="demo-section">
        <h2>3. 组件特性</h2>
        
        <div class="features">
          <el-row :gutter="20">
            <el-col :span="8">
              <div class="feature-card">
                <div class="feature-icon">
                  <el-icon size="30"><MagicStick /></el-icon>
                </div>
                <h3>语法高亮</h3>
                <p>支持SQL、JavaScript、JSON语法高亮显示，提升代码可读性</p>
              </div>
            </el-col>
            
            <el-col :span="8">
              <div class="feature-card">
                <div class="feature-icon">
                  <el-icon size="30"><ChatDotRound /></el-icon>
                </div>
                <h3>智能提示</h3>
                <p>输入时提供关键词和函数提示，提高编码效率</p>
              </div>
            </el-col>
            
            <el-col :span="8">
              <div class="feature-card">
                <div class="feature-icon">
                  <el-icon size="30"><Sort /></el-icon>
                </div>
                <h3>代码格式化</h3>
                <p>一键格式化代码，保持代码风格统一</p>
              </div>
            </el-col>
            
            <el-col :span="8">
              <div class="feature-card">
                <div class="feature-icon">
                  <el-icon size="30"><Collection /></el-icon>
                </div>
                <h3>代码片段</h3>
                <p>内置常用代码片段，快速插入标准代码</p>
              </div>
            </el-col>
            
            <el-col :span="8">
              <div class="feature-card">
                <div class="feature-icon">
                  <el-icon size="30"><FullScreen /></el-icon>
                </div>
                <h3>全屏模式</h3>
                <p>支持全屏编辑，提供沉浸式编码体验</p>
              </div>
            </el-col>
            
            <el-col :span="8">
              <div class="feature-card">
                <div class="feature-icon">
                  <el-icon size="30"><DataAnalysis /></el-icon>
                </div>
                <h3>实时统计</h3>
                <p>显示字符数、行数、光标位置等实时信息</p>
              </div>
            </el-col>
          </el-row>
        </div>
      </div>
      
      <div class="demo-section">
        <h2>4. 使用说明</h2>
        
        <div class="usage">
          <h3>安装与使用</h3>
          <pre class="usage-code">
// 1. 引入组件
import CodeEditor from '@/components/CodeEditor'

// 2. 注册组件
components: {
  CodeEditor
}

// 3. 在模板中使用
&lt;template&gt;
  &lt;CodeEditor
    v-model="code"
    language="sql"  // 可选: 'sql', 'javascript', 'json'
    placeholder="请输入代码..."
    :height="400"
    @change="handleChange"
    @save="handleSave"
  /&gt;
&lt;/template&gt;

// 4. 组件属性说明
// - modelValue: 代码内容（双向绑定）
// - language: 语言类型
// - placeholder: 占位符文本
// - height: 编辑器高度（支持数字或字符串）
// - readonly: 是否只读
// - showLineNumbers: 是否显示行号

// 5. 组件事件
// - update:modelValue: 代码变化时触发
// - update:language: 语言切换时触发
// - change: 代码变化时触发
// - save: 点击保存按钮时触发
          </pre>
          
          <h3>在现有项目中的集成</h3>
          <p>本编辑器已集成到数据服务的SQL步骤和JavaScript步骤中，替换了原来的简单textarea。</p>
          
          <div class="integration-example">
            <h4>集成效果对比：</h4>
            <el-row :gutter="20">
              <el-col :span="12">
                <div class="comparison before">
                  <h5>之前：简单textarea</h5>
                  <div class="comparison-image">
                    <div class="mock-textarea">
                      <div class="mock-header">SQL配置</div>
                      <div class="mock-content">
                        <textarea placeholder="请输入SQL语句..." rows="6"></textarea>
                      </div>
                      <div class="mock-footer">支持参数化查询</div>
                    </div>
                  </div>
                  <p>功能简单，无语法高亮和代码提示</p>
                </div>
              </el-col>
              <el-col :span="12">
                <div class="comparison after">
                  <h5>现在：集成编辑器</h5>
                  <div class="comparison-image">
                    <div class="mock-editor">
                      <div class="mock-toolbar">
                        <span>SQL</span>
                        <div class="mock-buttons">
                          <button>格式化</button>
                          <button>片段</button>
                        </div>
                      </div>
                      <div class="mock-code">
                        <span class="keyword">SELECT</span> * <span class="keyword">FROM</span> users
                        <span class="keyword">WHERE</span> status = 1
                      </div>
                      <div class="mock-status">
                        <span>行 1, 列 10</span>
                        <span>字符数: 45</span>
                      </div>
                    </div>
                  </div>
                  <p>功能丰富，支持语法高亮、代码提示、格式化等</p>
                </div>
              </el-col>
            </el-row>
          </div>
        </div>
      </div>
    </div>
    
    <div class="demo-footer">
      <p>© 2024 代码编辑器组件 - 为WesAdmin数据服务优化</p>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import { ElMessage } from 'element-plus'
import {
  MagicStick,
  ChatDotRound,
  Sort,
  Collection,
  FullScreen,
  DataAnalysis
} from '@element-plus/icons-vue'
import CodeEditor from '@/components/CodeEditor'

// 基础示例代码
const sqlCode = ref(`SELECT * FROM users WHERE status = :status AND create_time >= :startDate`)
const jsCode = ref(`// 处理用户数据
function processUsers(users) {
  return users.map(user => ({
    id: user.id,
    name: user.username,
    email: user.email,
    status: user.status === 1 ? '启用' : '禁用'
  }))
}`)
const jsonCode = ref(`{
  "code": 200,
  "data": {
    "users": []
  }
}`)

// 自定义代码联想演示
const customSqlCode = ref(`-- 使用自定义提示词
-- 输入 'user' 可以看到用户相关的自定义提示
SELECT * FROM sys_user WHERE username LIKE :keyword`)

const customSuggestions = ref([
  { label: 'user_table', type: 'custom', description: '用户表' },
  { label: 'user_query', type: 'custom', description: '用户查询' },
  { label: 'user_analysis', type: 'custom', description: '用户分析' },
  { label: 'table_structure', type: 'custom', description: '表结构' },
  { label: 'table_data', type: 'custom', description: '表数据' },
  { label: 'data_export', type: 'custom', description: '数据导出' },
  { label: 'data_import', type: 'custom', description: '数据导入' },
  { label: 'sys_user', type: 'custom', description: '系统用户表' },
  { label: 'sys_role', type: 'custom', description: '系统角色表' },
  { label: 'sys_teest', type: 'custom', description: '系统部门表' },
  { label: 'sys_test', type: 'custom', description: '系统部门表' },
  { label: 'sys_tes', type: 'custom', description: '系统部门表' }
])

// 添加自定义提示词
function addCustomSuggestion() {
  const suggestions = [
    'custom_query_' + Date.now().toString().slice(-4),
    'business_logic_' + Date.now().toString().slice(-4),
    'data_transform_' + Date.now().toString().slice(-4),
    'report_' + Date.now().toString().slice(-4),
    'dashboard_' + Date.now().toString().slice(-4)
  ]
  
  const randomSuggestion = suggestions[Math.floor(Math.random() * suggestions.length)]
  customSuggestions.value.push({
    label: randomSuggestion,
    type: 'custom',
    description: '动态添加的自定义提示词'
  })
  ElMessage.success(`已添加自定义提示词: ${randomSuggestion}`)
}

// 清空自定义提示
function clearCustomSuggestions() {
  customSuggestions.value = []
  ElMessage.info('已清空所有自定义提示词')
}

// 数据服务模拟
const activeTab = ref('sql')
const stepName = ref('用户查询步骤')
const varName = ref('userQuery')
const serviceSqlCode = ref(`SELECT 
  u.id,
  u.username,
  u.email,
  r.role_name,
  d.dept_name
FROM 
  sys_user u
  LEFT JOIN sys_role r ON u.role_id = r.id
  LEFT JOIN sys_dept d ON u.dept_id = d.id
WHERE 
  u.status = :status
  AND u.dept_id = :deptId`)
const serviceJsCode = ref(`// 数据处理逻辑
function processData(data) {
  // 数据转换
  const processed = data.map(item => {
    return {
      ...item,
      // 添加处理时间
      processTime: new Date().toISOString(),
      // 状态转换
      statusText: item.status === 1 ? '正常' : '停用'
    }
  })
  
  // 返回结果
  return {
    success: true,
    data: processed,
    count: processed.length,
    message: '处理完成'
  }
}

// 主函数
return processData(params.data)`)

const params = ref([
  { name: 'status', type: 'number', required: true, defaultValue: '1', description: '用户状态' },
  { name: 'deptId', type: 'number', required: false, defaultValue: '', description: '部门ID' },
  { name: 'keyword', type: 'string', required: false, defaultValue: '', description: '搜索关键词' }
])

// 事件处理
const handleChange = (code) => {
  console.log('代码变化:', code)
}

const saveService = () => {
  ElMessage.success('数据服务保存成功')
}

const resetService = () => {
  ElMessage.info('已重置')
}

const testService = () => {
  ElMessage.info('测试执行功能需连接后端服务')
}
</script>

<style scoped>
.editor-demo {
  padding: 20px;
  background: #f5f7fa;
  min-height: 100vh;
}

.demo-header {
  text-align: center;
  margin-bottom: 40px;
  padding: 30px;
  background: white;
  border-radius: 8px;
  box-shadow: 0 2px 12px 0 rgba(0, 0, 0, 0.1);
}

.demo-header h1 {
  margin: 0 0 10px 0;
  color: #333;
  font-size: 28px;
}

.demo-desc {
  margin: 0;
  color: #666;
  font-size: 16px;
  line-height: 1.6;
}

.demo-content {
  max-width: 1200px;
  margin: 0 auto;
}

.demo-section {
  margin-bottom: 40px;
  padding: 25px;
  background: white;
  border-radius: 8px;
  box-shadow: 0 2px 12px 0 rgba(0, 0, 0, 0.1);
}

.demo-section h2 {
  margin: 0 0 20px 0;
  color: #409eff;
  font-size: 22px;
  border-bottom: 2px solid #e8f4ff;
  padding-bottom: 10px;
}

.demo-item {
  margin-bottom: 25px;
}

.demo-item h3 {
  margin: 0 0 12px 0;
  color: #333;
  font-size: 16px;
}

/* 数据服务模拟 */
.demo-app h3 {
  margin: 0 0 20px 0;
  color: #333;
  font-size: 18px;
}

.service-card {
  border: 1px solid #e8e8e8;
}

.card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  font-weight: 600;
}

.service-step {
  padding: 15px 0;
}

.step-info {
  margin-bottom: 20px;
}

.step-editor {
  margin-bottom: 20px;
}

.step-editor h4,
.step-params h4 {
  margin: 0 0 10px 0;
  color: #333;
  font-size: 15px;
}

.step-help {
  margin-top: 20px;
}

.step-help ul {
  margin: 10px 0 0 20px;
  padding: 0;
}

.step-help li {
  margin-bottom: 5px;
  color: #666;
}

.step-help code {
  background: #f0f0f0;
  padding: 2px 6px;
  border-radius: 3px;
  font-family: 'Courier New', monospace;
  font-size: 12px;
  color: #d14;
}

.service-actions {
  margin-top: 25px;
  padding-top: 20px;
  border-top: 1px solid #e8e8e8;
  display: flex;
  gap: 10px;
}

/* 特性卡片 */
.features {
  margin-top: 20px;
}

.feature-card {
  padding: 20px;
  text-align: center;
  border: 1px solid #e8e8e8;
  border-radius: 8px;
  transition: all 0.3s;
  height: 180px;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
}

.feature-card:hover {
  border-color: #409eff;
  box-shadow: 0 4px 12px rgba(64, 158, 255, 0.1);
  transform: translateY(-2px);
}

.feature-icon {
  margin-bottom: 15px;
  color: #409eff;
}

.feature-card h3 {
  margin: 0 0 10px 0;
  color: #333;
  font-size: 16px;
}

.feature-card p {
  margin: 0;
  color: #666;
  font-size: 13px;
  line-height: 1.5;
}

/* 使用说明 */
.usage h3 {
  margin: 20px 0 15px 0;
  color: #333;
  font-size: 18px;
}

.usage h3:first-child {
  margin-top: 0;
}

.usage-code {
  margin: 0 0 20px 0;
  padding: 20px;
  background: #2d2d2d;
  color: #f8f8f2;
  border-radius: 8px;
  font-family: 'Courier New', monospace;
  font-size: 13px;
  line-height: 1.5;
  overflow-x: auto;
  white-space: pre-wrap;
}

/* 集成示例 */
.integration-example h4 {
  margin: 20px 0 15px 0;
  color: #333;
  font-size: 16px;
}

.comparison {
  text-align: center;
  padding: 20px;
  border: 1px solid #e8e8e8;
  border-radius: 8px;
  height: 100%;
}

.comparison h5 {
  margin: 0 0 15px 0;
  color: #333;
  font-size: 15px;
}

.comparison-image {
  margin: 15px 0;
}

.mock-textarea,
.mock-editor {
  border: 1px solid #dcdfe6;
  border-radius: 4px;
  overflow: hidden;
  text-align: left;
}

.mock-textarea .mock-header {
  padding: 10px;
  background: #f5f7fa;
  border-bottom: 1px solid #dcdfe6;
  font-weight: 500;
}

.mock-textarea .mock-content textarea {
  width: 100%;
  border: none;
  padding: 10px;
  font-family: 'Courier New', monospace;
  font-size: 14px;
  resize: none;
  outline: none;
}

.mock-textarea .mock-footer {
  padding: 8px 10px;
  background: #f5f7fa;
  border-top: 1px solid #dcdfe6;
  font-size: 12px;
  color: #666;
}

.mock-editor .mock-toolbar {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 8px 12px;
  background: #f5f7fa;
  border-bottom: 1px solid #dcdfe6;
  font-weight: 500;
}

.mock-editor .mock-buttons {
  display: flex;
  gap: 5px;
}

.mock-editor .mock-buttons button {
  padding: 4px 8px;
  border: 1px solid #dcdfe6;
  border-radius: 3px;
  background: white;
  font-size: 12px;
  cursor: pointer;
}

.mock-editor .mock-code {
  padding: 12px;
  font-family: 'Courier New', monospace;
  font-size: 14px;
  line-height: 1.5;
  background: #fafafa;
  min-height: 100px;
}

.mock-editor .mock-code .keyword {
  color: #0077aa;
  font-weight: bold;
}

.mock-editor .mock-status {
  display: flex;
  justify-content: space-between;
  padding: 6px 12px;
  background: #f5f7fa;
  border-top: 1px solid #dcdfe6;
  font-size: 12px;
  color: #666;
}

.comparison p {
  margin: 15px 0 0 0;
  color: #666;
  font-size: 13px;
}

.before {
  border-color: #e8e8e8;
}

.after {
  border-color: #409eff;
  background: #f0f7ff;
}

/* 自定义代码联想演示样式 */
.custom-suggestions-control {
  margin-bottom: 15px;
  padding: 10px;
  background-color: #f5f7fa;
  border-radius: 4px;
  border: 1px solid #dcdfe6;
}

.suggestion-examples {
  margin-top: 15px;
  padding: 15px;
  background-color: #f8f9fa;
  border-radius: 4px;
  border: 1px solid #e8e8e8;
}

.suggestion-examples h4 {
  margin-top: 0;
  margin-bottom: 10px;
  color: #333;
  font-size: 14px;
}

/* 页脚 */
.demo-footer {
  text-align: center;
  padding: 20px;
  margin-top: 40px;
  color: #999;
  font-size: 14px;
  border-top: 1px solid #e8e8e8;
}

/* 响应式调整 */
@media (max-width: 768px) {
  .editor-demo {
    padding: 10px;
  }
  
  .demo-header {
    padding: 20px 15px;
  }
  
  .demo-section {
    padding: 15px;
  }
  
  .features .el-col {
    margin-bottom: 15px;
  }
  
  .feature-card {
    height: auto;
    min-height: 150px;
  }
  
  .integration-example .el-col {
    margin-bottom: 20px;
  }
}
</style>