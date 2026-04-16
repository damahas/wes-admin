<template>
  <div class="js-step-container">
    <el-card shadow="never" class="step-card">
      <template #header>
        <div class="card-header">
          <span>JavaScript配置步骤</span>
          <div class="header-actions">
            <el-tag type="warning">JavaScript</el-tag>
          </div>
        </div>
      </template>
      
      <div class="js-layout">
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
        
        <!-- JavaScript编辑器 -->
        <div class="editor-container">
          <div class="editor-header">
            <span>JavaScript代码</span>
            <el-tag size="small">支持ES6+语法</el-tag>
          </div>
          
          <CodeEditor
            :model-value="localPartConfig"
            @update:model-value="updatePartConfig"
            language="javascript"
            placeholder="请输入JavaScript代码"
            :height="300"
          />
          
          <div class="editor-help">
            <h4>代码提示：</h4>
            <ul>
              <li>可以使用 <code>params</code> 对象访问传入的参数</li>
              <li>可以使用 <code>context</code> 对象访问执行上下文</li>
              <li>最后一行会自动作为返回值</li>
              <li>支持异步操作，使用 <code>async/await</code></li>
              <li>可以使用 <code>console.log()</code> 进行调试</li>
            </ul>
            
            <div class="code-examples">
              <h4>代码示例：</h4>
              <el-collapse>
                <el-collapse-item title="简单数据处理">
                  <pre class="example-code">// 示例：数据处理
const data = params.data || [];
const result = data.map(item => ({
  id: item.id,
  name: item.name.toUpperCase(),
  timestamp: Date.now()
}));

// 返回处理后的数据
return result;</pre>
                </el-collapse-item>
                
                <el-collapse-item title="条件判断">
                  <pre class="example-code">// 示例：条件判断
if (params.type === 'user') {
  // 用户数据处理
  const users = await fetchUsers(params.userIds);
  return users.map(u => ({
    ...u,
    fullName: `${u.firstName} ${u.lastName}`
  }));
} else if (params.type === 'order') {
  // 订单数据处理
  const orders = await fetchOrders(params.orderIds);
  return orders.filter(o => o.status === 'completed');
} else {
  // 默认返回
  return { error: '未知类型', params };
}</pre>
                </el-collapse-item>
                
                <el-collapse-item title="循环处理">
                  <pre class="example-code">// 示例：循环处理数组
const items = params.items || [];
const processedItems = [];

for (const item of items) {
  // 对每个项目进行处理
  const processed = {
    ...item,
    processedAt: new Date().toISOString(),
    // 添加计算字段
    total: item.price * item.quantity,
    status: item.quantity > 0 ? '有库存' : '缺货'
  };
  
  processedItems.push(processed);
}

// 返回处理结果
return {
  count: processedItems.length,
  items: processedItems,
  summary: {
    total: processedItems.reduce((sum, item) => sum + item.total, 0),
    average: processedItems.length > 0 
      ? processedItems.reduce((sum, item) => sum + item.total, 0) / processedItems.length 
      : 0
  }
};</pre>
                </el-collapse-item>
                
                <el-collapse-item title="函数调用">
                  <pre class="example-code">// 示例：调用外部函数
async function processUserData(userId) {
  // 这里可以调用其他服务或API
  const userInfo = await getUserInfo(userId);
  const userOrders = await getUserOrders(userId);
  
  return {
    user: userInfo,
    orders: userOrders,
    statistics: {
      orderCount: userOrders.length,
      totalAmount: userOrders.reduce((sum, order) => sum + order.amount, 0),
      lastOrderDate: userOrders.length > 0 
        ? userOrders[userOrders.length - 1].orderDate 
        : null
    }
  };
}

// 执行处理函数
const result = await processUserData(params.userId);
return result;</pre>
                </el-collapse-item>
              </el-collapse>
            </div>
          </div>
        </div>
      </div>
    </el-card>
  </div>
</template>

<script setup>
import { ref, watch } from 'vue'
import CodeEditor from '@/components/CodeEditor'

const props = defineProps({
  node: {
    type: Object,
    required: true
  }
})

const emit = defineEmits(['update:node'])

// 本地副本用于编辑
const localPartConfig = ref(props.node.partConfig || '')

// 监听props.node.partConfig变化
watch(() => props.node.partConfig, (newPartConfig) => {
  if (newPartConfig !== localPartConfig.value) {
    localPartConfig.value = newPartConfig || ''
  }
})

// 更新partConfig
function updatePartConfig(newValue) {
  localPartConfig.value = newValue
  emit('update:node', {
    ...props.node,
    partConfig: newValue
  })
}
</script>

<style scoped>
.js-step-container {
  height: 100%;
}

.step-card {
  height: 100%;
  display: flex;
  flex-direction: column;
}

.step-card .card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  font-weight: 600;
  color: var(--el-text-color-primary);
}

.js-layout {
  flex: 1;
  display: flex;
  flex-direction: column;
  gap: 20px;
}

.var-name-input {
  margin-bottom: 10px;
}

.editor-container {
  flex: 1;
  display: flex;
  flex-direction: column;
  min-height: 0;
}

.editor-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 10px;
  padding-bottom: 10px;
  border-bottom: 1px solid var(--el-border-color);
}

.editor-header span {
  font-weight: 600;
  color: var(--el-text-color-primary);
}

.code-editor {
  flex: 1;
  min-height: 300px;
}

.code-editor :deep(.el-textarea__inner) {
  font-family: 'Courier New', monospace;
  font-size: 14px;
  line-height: 1.5;
  background-color: #f8f9fa;
}

.editor-help {
  margin-top: 20px;
  padding: 15px;
  background-color: #f8f9fa;
  border-radius: 4px;
  border: 1px solid var(--el-border-color);
}

.editor-help h4 {
  margin-top: 0;
  margin-bottom: 10px;
  color: var(--el-text-color-primary);
}

.editor-help ul {
  margin: 0 0 15px 20px;
  padding: 0;
}

.editor-help li {
  margin-bottom: 5px;
  color: var(--el-text-color-regular);
  line-height: 1.5;
}

.editor-help code {
  background-color: #e8e8e8;
  padding: 2px 6px;
  border-radius: 3px;
  font-family: 'Courier New', monospace;
  font-size: 13px;
  color: #d63384;
}

.code-examples {
  margin-top: 15px;
}

.code-examples h4 {
  margin-bottom: 10px;
}

.example-code {
  margin: 0;
  padding: 15px;
  background-color: #1e1e1e;
  color: #d4d4d4;
  border-radius: 4px;
  font-family: 'Courier New', monospace;
  font-size: 13px;
  line-height: 1.5;
  white-space: pre-wrap;
  word-break: break-all;
  overflow-x: auto;
}

/* 语法高亮示例 */
.example-code .comment {
  color: #6a9955;
}

.example-code .keyword {
  color: #569cd6;
}

.example-code .string {
  color: #ce9178;
}

.example-code .number {
  color: #b5cea8;
}

.example-code .function {
  color: #dcdcaa;
}
</style>