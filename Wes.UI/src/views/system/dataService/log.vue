<template>
  <el-dialog
    v-model="dialogVisible"
    :title="title"
    width="90%"
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
    
    <!-- 查询区域 -->
    <query-form
      :config="queryConfig"
      v-model:visible="showSearch"
      v-model="queryParams.params"
      @query="handleQuery"
      @reset="resetQuery"
    />
    
    <!-- 操作按钮 -->
    <el-row :gutter="10" class="mb8">
      <el-col :span="1.5">
        <el-button
          type="danger"
          plain
          icon="Delete"
          @click="handleBatchDelete"
          :disabled="multiple"
        >
          批量删除
        </el-button>
      </el-col>
      <right-toolbar
        v-model:showSearch="showSearch"
        @queryTable="getList"
        :columns="columns"
      />
    </el-row>
    
    <!-- 日志表格 -->
    <div class="table-container">
      <el-table
        v-loading="loading"
        :data="logList"
        @selection-change="handleSelectionChange"
      >
        <el-table-column type="selection" width="50" align="center" />
        <el-table-column
          label="服务编码"
          align="center"
          prop="serviceCode"
          v-if="columns.serviceCode && columns.serviceCode.visible"
          :show-overflow-tooltip="true"
        />
        <el-table-column
          label="服务名称"
          align="center"
          prop="serviceName"
          v-if="columns.serviceName && columns.serviceName.visible"
          :show-overflow-tooltip="true"
        />
        <el-table-column
          label="调用者"
          align="center"
          prop="caller"
          v-if="columns.caller && columns.caller.visible"
          :show-overflow-tooltip="true"
          width="120"
        />
        <el-table-column
          label="调用时间"
          align="center"
          prop="callTime"
          v-if="columns.callTime && columns.callTime.visible"
          width="160"
        >
          <template #default="scope">
            <span>{{ formatTime(scope.row.callTime) }}</span>
          </template>
        </el-table-column>
        <el-table-column
          label="执行状态"
          align="center"
          prop="status"
          v-if="columns.status && columns.status.visible"
          width="100"
        >
          <template #default="scope">
            <dict-tag
              :options="logStatusOptions"
              :value="scope.row.status"
            />
          </template>
        </el-table-column>
        <el-table-column
          label="执行耗时(ms)"
          align="center"
          prop="executionTime"
          v-if="columns.executionTime && columns.executionTime.visible"
          width="120"
        />
        <el-table-column
          label="参数"
          align="center"
          v-if="columns.params && columns.params.visible"
          width="100"
        >
          <template #default="scope">
            <el-button
              link
              type="primary"
              @click="viewParams(scope.row)"
            >
              查看
            </el-button>
          </template>
        </el-table-column>
        <el-table-column
          label="结果"
          align="center"
          v-if="columns.result && columns.result.visible"
          width="100"
        >
          <template #default="scope">
            <el-button
              link
              type="primary"
              @click="viewResult(scope.row)"
            >
              查看
            </el-button>
          </template>
        </el-table-column>
        <el-table-column
          label="错误信息"
          align="center"
          prop="errorMessage"
          v-if="columns.errorMessage && columns.errorMessage.visible"
          :show-overflow-tooltip="true"
        />
        <el-table-column
          label="操作"
          align="center"
          width="120"
          class-name="small-padding fixed-width"
        >
          <template #default="scope">
            <el-tooltip content="查看详情" placement="top">
              <el-button
                link
                type="primary"
                icon="View"
                @click="handleViewDetail(scope.row)"
              />
            </el-tooltip>
            <el-tooltip content="删除" placement="top">
              <el-button
                link
                type="danger"
                icon="Delete"
                @click="handleDelete(scope.row)"
              />
            </el-tooltip>
          </template>
        </el-table-column>
      </el-table>
      
      <pagination
        v-show="total > 0"
        :total="total"
        v-model:page="queryParams.pageNum"
        v-model:limit="queryParams.pageSize"
        @pagination="getList"
      />
    </div>
    
    <template #footer>
      <span class="dialog-footer">
        <el-button @click="handleClose">关闭</el-button>
      </span>
    </template>
    
    <!-- 参数查看对话框 -->
    <el-dialog
      v-model="paramsDialogVisible"
      title="调用参数"
      width="600px"
    >
      <div class="params-container">
        <pre v-if="currentParams">{{ JSON.stringify(currentParams, null, 2) }}</pre>
        <p v-else>无参数</p>
      </div>
      <template #footer>
        <span class="dialog-footer">
          <el-button @click="paramsDialogVisible = false">关闭</el-button>
        </span>
      </template>
    </el-dialog>
    
    <!-- 结果查看对话框 -->
    <el-dialog
      v-model="resultDialogVisible"
      title="调用结果"
      width="800px"
      fullscreen
    >
      <div class="result-container">
        <pre v-if="currentResult">{{ JSON.stringify(currentResult, null, 2) }}</pre>
        <p v-else>无结果</p>
      </div>
      <template #footer>
        <span class="dialog-footer">
          <el-button @click="resultDialogVisible = false">关闭</el-button>
        </span>
      </template>
    </el-dialog>
  </el-dialog>
</template>

<script setup name="DataServiceLog">
import { ref, reactive, toRefs, computed, watch, defineProps, defineEmits } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import QueryForm from '@/components/QueryForm/index.vue'
import DictTag from '@/components/DictTag/index.vue'
import RightToolbar from '@/components/RightToolbar/index.vue'
import Pagination from '@/components/Pagination/index.vue'

const props = defineProps({
  // 是否显示弹窗
  visible: {
    type: Boolean,
    default: false
  },
  // 数据服务ID（可选，用于筛选特定服务的日志）
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
const showSearch = ref(true)
const logList = ref([])
const ids = ref([])
const single = ref(true)
const multiple = ref(true)
const total = ref(0)
const isFullscreen = ref(false)
const title = ref('数据服务调用日志')

// 对话框控制
const paramsDialogVisible = ref(false)
const resultDialogVisible = ref(false)
const currentParams = ref(null)
const currentResult = ref(null)

// 日志状态选项
const logStatusOptions = ref([
  { value: 'success', label: '成功' },
  { value: 'failure', label: '失败' },
  { value: 'error', label: '错误' }
])

// 查询条件配置
const queryConfig = computed(() => [
  {
    label: '服务编码',
    prop: 'serviceCode',
    type: 'input',
    placeholder: '请输入服务编码'
  },
  {
    label: '服务名称',
    prop: 'serviceName',
    type: 'input',
    placeholder: '请输入服务名称'
  },
  {
    label: '调用者',
    prop: 'caller',
    type: 'input',
    placeholder: '请输入调用者'
  },
  {
    label: '执行状态',
    prop: 'status',
    type: 'select',
    placeholder: '请选择执行状态',
    options: logStatusOptions.value
  },
  {
    label: '调用时间',
    prop: 'dateRange',
    type: 'daterange',
    placeholder: '请选择调用时间范围'
  }
])

// 列显隐信息
const columns = ref({
  serviceCode: { label: '服务编码', visible: true },
  serviceName: { label: '服务名称', visible: true },
  caller: { label: '调用者', visible: true },
  callTime: { label: '调用时间', visible: true },
  status: { label: '执行状态', visible: true },
  executionTime: { label: '执行耗时', visible: true },
  params: { label: '参数', visible: true },
  result: { label: '结果', visible: true },
  errorMessage: { label: '错误信息', visible: false }
})

const data = reactive({
  queryParams: {
    pageNum: 1,
    pageSize: 10,
    params: {
      serviceCode: undefined,
      serviceName: undefined,
      caller: undefined,
      status: undefined,
      dateRange: undefined
    }
  }
})

const { queryParams } = toRefs(data)

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

/** 查询日志列表 */
function getList() {
  loading.value = true
  // 模拟数据 - 实际项目中需要对接API
  setTimeout(() => {
    loading.value = false
    logList.value = [
      {
        id: '1',
        serviceCode: 'DS001',
        serviceName: '用户查询服务',
        caller: 'admin',
        callTime: new Date().toISOString(),
        status: 'success',
        executionTime: 120,
        params: { userId: '123' },
        result: { user: { id: '123', name: '张三' } },
        errorMessage: ''
      },
      {
        id: '2',
        serviceCode: 'DS002',
        serviceName: '订单统计服务',
        caller: 'user01',
        callTime: new Date(Date.now() - 3600000).toISOString(),
        status: 'failure',
        executionTime: 250,
        params: { date: '2024-01-01' },
        result: null,
        errorMessage: '参数格式不正确'
      }
    ]
    total.value = logList.value.length
  }, 500)
}

/** 搜索按钮操作 */
function handleQuery() {
  queryParams.value.pageNum = 1
  getList()
}

/** 重置按钮操作 */
function resetQuery() {
  queryParams.value.params = {
    serviceCode: undefined,
    serviceName: undefined,
    caller: undefined,
    status: undefined,
    dateRange: undefined
  }
  handleQuery()
}

/** 查看参数 */
function viewParams(row) {
  currentParams.value = row.params
  paramsDialogVisible.value = true
}

/** 查看结果 */
function viewResult(row) {
  currentResult.value = row.result
  resultDialogVisible.value = true
}

/** 查看详情 */
function handleViewDetail() {
  ElMessage.info('查看详情功能开发中')
}

/** 删除日志 */
function handleDelete(row) {
  const logId = row.id
  ElMessageBox.confirm(
    `是否确认删除该条调用日志？`,
    '提示',
    {
      confirmButtonText: '确定',
      cancelButtonText: '取消',
      type: 'warning'
    }
  )
    .then(() => {
      // 模拟删除操作
      logList.value = logList.value.filter(item => item.id !== logId)
      total.value = logList.value.length
      ElMessage.success('删除成功')
    })
    .catch(() => {})
}

/** 批量删除 */
function handleBatchDelete() {
  if (ids.value.length === 0) {
    ElMessage.warning('请选择要删除的日志')
    return
  }
  
  ElMessageBox.confirm(
    `是否确认删除选中的${ids.value.length}条日志？`,
    '提示',
    {
      confirmButtonText: '确定',
      cancelButtonText: '取消',
      type: 'warning'
    }
  )
    .then(() => {
      // 模拟批量删除
      logList.value = logList.value.filter(item => !ids.value.includes(item.id))
      total.value = logList.value.length
      ids.value = []
      ElMessage.success('删除成功')
    })
    .catch(() => {})
}

/** 选择条数 */
function handleSelectionChange(selection) {
  ids.value = selection.map(item => item.id)
  single.value = selection.length !== 1
  multiple.value = !selection.length
}

// 监听 visible 变化，显示时加载数据
watch(
  () => props.visible,
  (newVal) => {
    if (newVal) {
      getList()
      // 如果有 dsId，则自动填充到查询条件
      if (props.dsId) {
        // 这里可以根据实际情况处理，比如查询特定服务的日志
        queryParams.value.params.serviceCode = props.dsId
      }
    } else {
      // 关闭时重置数据
      logList.value = []
      ids.value = []
      total.value = 0
      loading.value = true
      isFullscreen.value = false
      paramsDialogVisible.value = false
      resultDialogVisible.value = false
      currentParams.value = null
      currentResult.value = null
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

.table-container {
  margin-top: 20px;
}

.params-container,
.result-container {
  max-height: 500px;
  overflow-y: auto;
  background-color: #f5f5f5;
  padding: 15px;
  border-radius: 4px;
}

.params-container pre,
.result-container pre {
  margin: 0;
  font-family: 'Courier New', monospace;
  white-space: pre-wrap;
  word-break: break-all;
}

.dialog-footer {
  display: flex;
  justify-content: flex-end;
}

.mb8 {
  margin-bottom: 8px;
}
</style>