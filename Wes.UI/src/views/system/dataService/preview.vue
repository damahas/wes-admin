<template>
  <el-dialog
    v-model="dialogVisible"
    :title="title"
    :fullscreen="true"
    :show-close="false"
    :close-on-click-modal="false"
    :close-on-press-escape="false"
  >
    <template #header>
      <div class="dialog-header">
        <div class="header-left">
          <el-icon class="header-icon"><Document /></el-icon>
          <span>{{ title }}</span>
        </div>
        <div class="header-actions">
          <el-button type="primary" @click="handleExec">
            <el-icon><Promotion /></el-icon>
            发送请求
          </el-button>
          <el-button @click="handleClose">
            <el-icon><Close /></el-icon>
            关闭
          </el-button>
        </div>
      </div>
    </template>

    <div v-if="serviceInfo" class="preview-container">
      <el-row :gutter="20">
        <!-- 左侧：请求配置 -->
        <el-col :span="10">
          <div class="panel request-panel">
            <div class="panel-header">
              <div class="panel-title">
                <el-icon><Link /></el-icon>
                <span>请求配置</span>
              </div>
            </div>
            <div class="panel-body">
              <!-- 接口地址 -->
              <div class="form-group">
                <label class="form-label">接口地址</label>
                <div class="url-box">
                  <span class="url-text">{{ apiUrl }}</span>
                  <el-button class="copy-btn" @click="copyUrl">
                    <el-icon><CopyDocument /></el-icon>
                    <span>复制</span>
                  </el-button>
                </div>
              </div>

              <!-- 请求参数 -->
              <div class="form-group">
                <label class="form-label">请求参数</label>
                <div class="params-card">
                  <div v-if="paramsList.length > 0" class="params-table">
                    <div class="params-header">
                      <span class="col-key">参数名</span>
                      <span class="col-type">类型</span>
                      <span class="col-value">参数值</span>
                    </div>
                    <div v-for="(param, index) in paramsList" :key="index" class="params-row">
                      <span class="col-key text-display">{{ param.key }}</span>
                      <span class="col-type text-display">{{ getTypeName(param.type) }}</span>
                      <el-input
                        v-model="param.value"
                        :placeholder="getParamPlaceholder(param.type)"
                        size="small"
                        class="col-value"
                      />
                    </div>
                  </div>
                  <div v-else class="params-empty">
                    <el-icon><DocumentDelete /></el-icon>
                    <span>暂无请求参数</span>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </el-col>

        <!-- 右侧：响应结果 -->
        <el-col :span="14">
          <div class="panel response-panel">
            <div class="panel-header">
              <div class="panel-title">
                <el-icon><DataBoard /></el-icon>
                <span>响应结果</span>
              </div>
              <div class="panel-status">
                <el-tag v-if="responseData" :type="responseSuccess ? 'success' : 'danger'" size="large">
                  {{ responseSuccess ? '请求成功' : '请求失败' }}
                </el-tag>
              </div>
            </div>
            <div class="panel-body">
              <div class="response-box">
                <div v-if="responseLoading" class="response-loading">
                  <el-icon class="loading-spinner"><Loading /></el-icon>
                  <span>请求中...</span>
                </div>
                <template v-else-if="responseData">
                  <div class="response-toolbar">
                    <el-button size="small" @click="copyResponse">
                      <el-icon><CopyDocument /></el-icon>
                      复制结果
                    </el-button>
                  </div>
                  <pre class="response-json">{{ JSON.stringify(responseData, null, 2) }}</pre>
                </template>
                <div v-else class="response-empty">
                  <el-icon class="empty-icon"><VideoPause /></el-icon>
                  <p>点击「发送请求」按钮查看响应结果</p>
                </div>
              </div>
            </div>
          </div>
        </el-col>
      </el-row>
    </div>

    <div v-else class="empty-container">
      <el-empty description="未找到数据服务信息" />
    </div>
  </el-dialog>
</template>

<script setup name="DataServicePreview">
import { ref, computed, watch } from 'vue'
import { ElMessage } from 'element-plus'
import { Link, CopyDocument, Document, Promotion, Close, DataBoard, DocumentDelete, VideoPause, Loading } from '@element-plus/icons-vue'
import { getDataService, execDataService } from '@/api/system/dataService'

const props = defineProps({
  visible: {
    type: Boolean,
    default: false
  },
  dsId: {
    type: [String, Number],
    default: ''
  }
})

const emit = defineEmits(['update:visible', 'close'])

const dialogVisible = computed({
  get: () => props.visible,
  set: (val) => emit('update:visible', val)
})

const loading = ref(false)
const responseLoading = ref(false)
const serviceInfo = ref(null)
const title = computed(() => {
  return serviceInfo.value
    ? `数据服务预览 - ${serviceInfo.value.serviceName}`
    : '数据服务预览'
})

// 接口地址
const apiUrl = computed(() => {
  if (!serviceInfo.value?.serviceCode) return ''
  const baseUrl = window.location.origin
  return `${baseUrl}/api/system/dataService/${serviceInfo.value.serviceCode}/exec`
})

// 复制地址
const copyUrl = () => {
  navigator.clipboard.writeText(apiUrl.value).then(() => {
    ElMessage.success('接口地址已复制到剪贴板')
  }).catch(() => {
    ElMessage.error('复制失败')
  })
}

// 复制响应结果
const copyResponse = () => {
  const text = JSON.stringify(responseData.value, null, 2)
  navigator.clipboard.writeText(text).then(() => {
    ElMessage.success('响应结果已复制到剪贴板')
  }).catch(() => {
    ElMessage.error('复制失败')
  })
}

// 请求参数列表
const paramsList = ref([])

// 获取参数placeholder
const getParamPlaceholder = (type) => {
  if (type === 'object') {
    return 'JSON字符串，如: {"key":"value"}'
  }
  return '参数值'
}

// 获取类型中文名称
const getTypeName = (type) => {
  const typeMap = {
    string: '字符串',
    number: '数字',
    boolean: '布尔',
    date: '日期',
    object: '对象'
  }
  return typeMap[type] || type
}

// 请求参数解析
const parseParams = () => {
  // 保存用户已输入的值
  const existingParams = {}
  paramsList.value.forEach(p => {
    if (p.key) {
      existingParams[p.key] = p.value
    }
  })

  // 只在有配置时才解析，否则保持用户添加的参数
  const paramConfig = serviceInfo.value?.paramConfig
  if (!paramConfig) return

  try {
    let parsed = []
    if (typeof paramConfig === 'string') {
      parsed = JSON.parse(paramConfig)
    } else if (Array.isArray(paramConfig)) {
      parsed = paramConfig
    }

    if (Array.isArray(parsed)) {
      paramsList.value = parsed.map(p => {
        const key = p.key || p.paramName || ''
        return {
          key,
          type: p.type || 'string',
          value: existingParams[key] || ''
        }
      })
    }
  } catch (e) {
    console.error('解析参数失败:', e)
  }
}

// 响应数据
const responseData = ref(null)
const responseSuccess = ref(false)

// 发送请求
const handleExec = async () => {
  if (!serviceInfo.value?.serviceCode) {
    ElMessage.warning('服务编码不存在')
    return
  }

  const data = {}
  paramsList.value.forEach(param => {
    if (param.key) {
      let value = param.value
      if (param.type === 'number') {
        value = Number(param.value) || 0
      } else if (param.type === 'boolean') {
        value = param.value === 'true' || param.value === true
      } else if (param.type === 'object' && param.value) {
        try {
          value = JSON.parse(param.value)
        } catch (e) {
          ElMessage.error(`参数 "${param.key}" 的值不是有效的JSON字符串`)
          return
        }
      }
      data[param.key] = value
    }
  })

  try {
    responseLoading.value = true
    const res = await execDataService(serviceInfo.value.serviceCode, data)
    responseData.value = res
    responseSuccess.value = res.code === 200
    if (responseSuccess.value) {
      ElMessage.success('请求成功')
    } else {
      ElMessage.error(res.msg || '请求失败')
    }
  } catch (error) {
    responseData.value = error
    responseSuccess.value = false
    ElMessage.error('请求失败')
  } finally {
    responseLoading.value = false
  }
}

const handleClose = () => {
  dialogVisible.value = false
  emit('close')
}

const fetchServiceInfo = async () => {
  if (!props.dsId) {
    ElMessage.warning('未指定数据服务ID')
    return
  }

  try {
    loading.value = true
    const response = await getDataService(props.dsId)
    if (response.code === 200) {
      serviceInfo.value = response.data
      parseParams()
    } else {
      ElMessage.error('获取数据服务信息失败')
      serviceInfo.value = null
    }
  } catch (error) {
    console.error('获取数据服务信息失败:', error)
    ElMessage.error('获取数据服务信息失败')
    serviceInfo.value = null
  } finally {
    loading.value = false
  }
}

watch(
  () => props.visible,
  (newVal) => {
    if (newVal && props.dsId) {
      fetchServiceInfo()
      responseData.value = null
    } else {
      serviceInfo.value = null
      paramsList.value = []
      responseData.value = null
    }
  },
  { immediate: true }
)
</script>

<style scoped>
/* Dialog Header */
.dialog-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  width: 100%;
  padding: 0 4px;
}

.header-left {
  display: flex;
  align-items: center;
  gap: 10px;
}

.header-icon {
  font-size: 20px;
  color: var(--el-color-primary);
}

.header-actions {
  display: flex;
  align-items: center;
  gap: 12px;
}

/* Preview Container */
.preview-container {
  height: 100%;
  padding: 16px;
}

/* Panel */
.panel {
  background: var(--el-bg-color);
  border-radius: 8px;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.04);
  border: 1px solid var(--el-border-color-lighter);
  height: calc(100vh - 116px);
  display: flex;
  flex-direction: column;
}

.panel-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 10px 18px;
  border-bottom: 1px solid var(--el-border-color-lighter);
  background: var(--el-fill-color-light);
  border-radius: 8px 8px 0 0;
  height: 48px;
  box-sizing: border-box;
}

.panel-title {
  display: flex;
  align-items: center;
  gap: 8px;
  font-size: 14px;
  font-weight: 600;
  color: var(--el-text-color-primary);
}

.panel-title .el-icon {
  font-size: 16px;
  color: var(--el-color-primary);
}

.panel-status {
  display: flex;
  align-items: center;
}

.panel-body {
  padding: 16px;
  flex: 1;
  overflow-y: auto;
}

/* Form */
.form-group {
  margin-bottom: 16px;
}

.form-label {
  display: block;
  font-size: 14px;
  font-weight: 500;
  color: var(--el-text-color-regular);
  margin-bottom: 10px;
}

.url-box {
  display: flex;
  align-items: center;
  gap: 10px;
  padding: 10px 14px;
  background: var(--el-fill-color-light);
  border-radius: 6px;
}

.url-text {
  flex: 1;
  font-family: 'Fira Code', 'Consolas', monospace;
  font-size: 14px;
  color: var(--el-text-color-primary);
  word-break: break-all;
}

.copy-btn {
  display: flex;
  align-items: center;
  gap: 5px;
  padding: 6px 12px;
  font-size: 14px;
  border-radius: 4px;
  flex-shrink: 0;
}

/* Params Card */
.params-card {
  background: var(--el-fill-color-light);
  border: 1px solid var(--el-border-color-lighter);
  border-radius: 6px;
  padding: 12px;
  min-height: 80px;
}

.params-table {
  display: flex;
  flex-direction: column;
  gap: 8px;
}

.params-header {
  display: flex;
  gap: 8px;
  padding: 0 4px 6px;
  border-bottom: 1px solid var(--el-border-color-lighter);
  font-size: 14px;
  font-weight: 600;
  color: var(--el-text-color-secondary);
}

.params-row {
  display: flex;
  gap: 8px;
  align-items: center;
  padding: 2px 0;
}

.col-key {
  flex: 0.6;
}

.col-type {
  width: 70px;
  flex-shrink: 0;
}

.text-display {
  font-size: 14px;
  color: var(--el-text-color-regular);
  padding: 6px 0;
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
}

.col-value {
  flex: 1;
}

.params-empty {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 16px;
  color: var(--el-text-color-secondary);
  gap: 8px;
}

.params-empty .el-icon {
  font-size: 24px;
  color: var(--el-border-color);
}

/* Response Box */
.response-box {
  display: flex;
  flex-direction: column;
  background: #1e1e1e;
  border-radius: 6px;
  height: 100%;
  min-height: 300px;
  overflow: hidden;
}

.response-loading {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  height: 100%;
  gap: 12px;
  color: #888;
}

.response-loading .loading-spinner {
  font-size: 28px;
  color: var(--el-color-primary);
  animation: rotate 1s linear infinite;
}

.response-empty {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  height: 100%;
  color: #888;
}

.empty-icon {
  font-size: 36px;
  color: #555;
  margin-bottom: 12px;
}

.response-empty p {
  margin: 0;
  font-size: 14px;
}

.response-toolbar {
  padding: 8px 12px;
  border-bottom: 1px solid #333;
  flex-shrink: 0;
}

.response-toolbar .el-button {
  background: transparent;
  border-color: #555;
  color: #ccc;
}

.response-toolbar .el-button:hover {
  background: #333;
  border-color: #666;
}

.response-json {
  margin: 0;
  flex: 1;
  overflow: auto;
  padding: 12px;
  font-family: 'Fira Code', 'Consolas', monospace;
  font-size: 14px;
  color: #d4d4d4;
  line-height: 1.6;
  white-space: pre-wrap;
  word-break: break-all;
}

/* Empty Container */
.empty-container {
  padding: 60px 0;
  text-align: center;
}

@keyframes rotate {
  from { transform: rotate(0deg); }
  to { transform: rotate(360deg); }
}
</style>
