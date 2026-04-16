<template>
  <el-dialog
    v-model="dialogVisible"
    :title="title"
    width="80%"
    :fullscreen="isFullscreen"
    :show-close="false"
    :close-on-click-modal="false"
    :close-on-press-escape="false"
  >
    <template #header>
      <div class="dialog-header">
        <span>{{ title }}</span>
        <div class="header-actions">
          <el-button link @click="toggleFullscreen">
            <el-icon>
              <FullScreen v-if="!isFullscreen" />
              <CopyDocument v-else />
            </el-icon>
          </el-button>
          <el-button link @click="handleClose">
            <el-icon><Close /></el-icon>
          </el-button>
        </div>
      </div>
    </template>
    
    <div v-if="loading" class="loading-container">
      <el-skeleton :rows="10" animated />
    </div>
    
    <div v-else-if="serviceInfo">
      <el-descriptions title="服务基本信息" border>
        <el-descriptions-item label="服务编码">
          {{ serviceInfo.serviceCode }}
        </el-descriptions-item>
        <el-descriptions-item label="服务名称">
          {{ serviceInfo.serviceName }}
        </el-descriptions-item>
        <el-descriptions-item label="服务类型">
          <dict-tag :options="serviceTypeOptions" :value="serviceInfo.serviceType" />
        </el-descriptions-item>
        <el-descriptions-item label="分类">
          <dict-tag :options="sys_data_service_category" :value="serviceInfo.category" />
        </el-descriptions-item>
        <el-descriptions-item label="状态">
          <dict-tag :options="sys_normal_disable" :value="serviceInfo.isEnabled" />
        </el-descriptions-item>
        <el-descriptions-item label="创建时间">
          {{ formatTime(serviceInfo.createTime) }}
        </el-descriptions-item>
        <el-descriptions-item label="更新时间">
          {{ formatTime(serviceInfo.updateTime) }}
        </el-descriptions-item>
      </el-descriptions>
      
      <div class="preview-content">
        <h3>服务配置详情</h3>
        <div class="detail-section">
          <h4>SQL配置</h4>
          <pre class="sql-content">{{ serviceInfo.sqlContent || '暂无SQL配置' }}</pre>
        </div>
        
        <div class="detail-section">
          <h4>参数配置</h4>
          <div v-if="serviceInfo.paramConfig && serviceInfo.paramConfig.length > 0">
            <el-table :data="serviceInfo.paramConfig" border>
              <el-table-column prop="paramName" label="参数名称" width="120" />
              <el-table-column prop="paramType" label="参数类型" width="100" />
              <el-table-column prop="required" label="是否必填" width="80">
                <template #default="scope">
                  <dict-tag :options="yesNoOptions" :value="scope.row.required" />
                </template>
              </el-table-column>
              <el-table-column prop="defaultValue" label="默认值" />
              <el-table-column prop="description" label="描述" />
            </el-table>
          </div>
          <div v-else>
            <p>暂无参数配置</p>
          </div>
        </div>
      </div>
    </div>
    
    <div v-else class="empty-container">
      <el-empty description="未找到数据服务信息" />
    </div>
    
    <template #footer>
      <span class="dialog-footer">
        <el-button @click="handleClose">关闭</el-button>
      </span>
    </template>
  </el-dialog>
</template>

<script setup name="DataServicePreview">
import { ref, watch, defineProps, defineEmits, computed } from 'vue'
import { ElMessage } from 'element-plus'
import { getDict } from '@/utils'
import DictTag from '@/components/DictTag/index.vue'
import { getDataService } from '@/api/system/dataService'

const props = defineProps({
  // 是否显示弹窗
  visible: {
    type: Boolean,
    default: false
  },
  // 数据服务ID
  dsId: {
    type: [String, Number],
    default: ''
  }
})

const emit = defineEmits(['update:visible', 'close'])

// 弹窗显示控制
const dialogVisible = computed({
  get: () => props.visible,
  set: (val) => emit('update:visible', val)
})

const loading = ref(true)
const serviceInfo = ref(null)
const isFullscreen = ref(false)
const title = computed(() => {
  return serviceInfo.value 
    ? `数据服务预览 - ${serviceInfo.value.serviceName}`
    : '数据服务预览'
})

const dicts = getDict('sys_data_service_category', 'sys_normal_disable')
const sys_data_service_category = dicts.sys_data_service_category
const sys_normal_disable = dicts.sys_normal_disable

const serviceTypeOptions = ref([
  { value: '1', label: '查询' },
  { value: '2', label: '分页查询' },
  { value: '3', label: '增删改' }
])

const yesNoOptions = ref([
  { value: '0', label: '否' },
  { value: '1', label: '是' }
])

const handleClose = () => {
  dialogVisible.value = false
  emit('close')
}

const toggleFullscreen = () => {
  isFullscreen.value = !isFullscreen.value
}

const formatTime = (time) => {
  if (!time) return ''
  return new Date(time).toLocaleString()
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

// 监听 visible 变化，显示时加载数据
watch(
  () => props.visible,
  (newVal) => {
    if (newVal && props.dsId) {
      fetchServiceInfo()
    } else {
      // 关闭时重置数据
      serviceInfo.value = null
      loading.value = true
      isFullscreen.value = false
    }
  },
  { immediate: true }
)
</script>

<style scoped>
.dialog-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  width: 100%;
}

.header-actions {
  display: flex;
  align-items: center;
  gap: 8px;
}

.loading-container {
  padding: 40px 0;
  text-align: center;
}

.preview-content {
  margin-top: 30px;
}

.detail-section {
  margin-top: 20px;
  padding: 20px;
  background-color: #f8f9fa;
  border-radius: 4px;
}

.detail-section h4 {
  margin-bottom: 15px;
  color: #333;
}

.sql-content {
  background-color: #f5f5f5;
  padding: 15px;
  border-radius: 4px;
  font-family: 'Courier New', monospace;
  white-space: pre-wrap;
  word-break: break-all;
  max-height: 300px;
  overflow-y: auto;
}

.empty-container {
  padding: 60px 0;
  text-align: center;
}

.dialog-footer {
  display: flex;
  justify-content: flex-end;
}
</style>