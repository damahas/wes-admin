<template>
  <div class="step-wrapper">
    <div class="step-card">
      <div class="step-card-header">
        <div class="header-left">
          <div class="header-icon info-icon">
            <svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
              <path d="M12 2L2 7l10 5 10-5-10-5z"/>
              <path d="M2 17l10 5 10-5"/>
              <path d="M2 12l10 5 10-5"/>
            </svg>
          </div>
          <div class="header-text">
            <h3>基础信息配置</h3>
            <p>填写数据服务的基本信息和配置</p>
          </div>
        </div>
      </div>

      <div class="step-card-body">
        <el-form :model="formData" :rules="rules" ref="formRef" label-width="auto" class="service-form">
          <div class="form-layout">
            <!-- 左侧：基本信息 + 备注 -->
            <div class="form-left">
              <div class="form-section">
                <div class="section-title">
                  <el-icon><OfficeBuilding /></el-icon>
                  <span>基本信息</span>
                </div>
                <div class="form-grid">
                  <el-form-item label="服务编码" prop="serviceCode">
                    <el-input
                      v-model="formData.serviceCode"
                      placeholder="请输入服务编码"
                      maxlength="50"
                      clearable
                    >
                      <template #prefix>
                        <el-icon><Key /></el-icon>
                      </template>
                    </el-input>
                  </el-form-item>

                  <el-form-item label="服务名称" prop="serviceName">
                    <el-input
                      v-model="formData.serviceName"
                      placeholder="请输入服务名称"
                      maxlength="100"
                      clearable
                    >
                      <template #prefix>
                        <el-icon><Edit /></el-icon>
                      </template>
                    </el-input>
                  </el-form-item>

                  <el-form-item label="分类" prop="category">
                    <DictSelect
                      v-model="formData.category"
                      :options="categoryOptions"
                      placeholder="请选择分类"
                      style="width: 100%"
                    />
                  </el-form-item>

                  <el-form-item label="服务状态">
                    <el-switch
                      v-model="formData.status"
                      active-value="0"
                      inactive-value="1"
                      active-text="启用"
                      inactive-text="停用"
                      inline-prompt
                    />
                  </el-form-item>
                </div>

                <div class="inner-section template-section">
                  <div class="section-title">
                    <el-icon><Grid /></el-icon>
                    <span>加载模板</span>
                  </div>
                  <div class="template-grid">
                    <div
                      v-for="template in templateList"
                      :key="template.value"
                      class="template-item"
                      @click="selectTemplate(template)"
                    >
                      <el-icon><component :is="template.icon" /></el-icon>
                      <span>{{ template.label }}</span>
                    </div>
                  </div>
                </div>
              </div>

              <div class="form-section">
                <div class="section-title">
                  <el-icon><Document /></el-icon>
                  <span>备注信息</span>
                </div>
                <el-form-item label="服务描述">
                  <el-input
                    v-model="formData.remark"
                    type="textarea"
                    placeholder="请输入服务描述信息，用于说明服务的用途和使用方法..."
                    maxlength="500"
                    :rows="4"
                    show-word-limit
                  />
                </el-form-item>
              </div>
            </div>

            <!-- 右侧：输入变量 -->
            <div class="form-right">
              <div class="form-section param-section">
                <div class="section-title">
                  <el-icon><Setting /></el-icon>
                  <span>输入变量</span>
                </div>
                <div class="param-config-area">
                  <div class="param-list">
                    <div v-for="(param, index) in inputParams" :key="index" class="param-item">
                      <el-input
                        v-model="param.key"
                        placeholder="变量名"
                        size="small"
                        @change="onParamChange"
                      />
                      <el-select
                        v-model="param.type"
                        size="small"
                        @change="onParamChange"
                      >
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
                        @click="removeInputParam(index)"
                      />
                    </div>
                    <div v-if="inputParams.length === 0" class="param-empty">
                      暂未配置输入变量
                    </div>
                  </div>
                  <el-button
                    size="small"
                    type="primary"
                    plain
                    style="margin-top: 12px"
                    @click="addInputParam"
                  >
                    + 添加变量
                  </el-button>
                </div>
              </div>
            </div>
          </div>
        </el-form>
      </div>
    </div>
  </div>
</template>

<script setup name="BasicInfoStep">
import { defineProps, defineEmits, ref, reactive, watch } from 'vue'
import { Key, Edit, Document, OfficeBuilding, Grid, Check, Search, List, Operation, Connection, Setting, Delete } from '@element-plus/icons-vue'
import { ElMessageBox } from 'element-plus'
import DictSelect from '@/components/DictSelect'

const props = defineProps({
  formData: {
    type: Object,
    required: true,
    default: () => ({
      serviceCode: '',
      serviceName: '',
      category: '',
      status: 0,
      remark: ''
    })
  },
  categoryOptions: {
    type: Array,
    default: () => []
  },
  statusOptions: {
    type: Array,
    default: () => []
  },
  rules: {
    type: Object,
    default: () => ({
      serviceCode: [
        { required: true, message: '服务编码不能为空', trigger: 'blur' },
        { max: 50, message: '服务编码长度不能超过50个字符', trigger: 'blur' }
      ],
      serviceName: [
        { required: true, message: '服务名称不能为空', trigger: 'blur' },
        { max: 100, message: '服务名称长度不能超过100个字符', trigger: 'blur' }
      ],
      category: [{ required: true, message: '请选择分类', trigger: 'change' }]
    })
  }
})

const emit = defineEmits(['validate', 'template-select'])

const formRef = ref(null)

// ==================== 输入变量管理 ====================
const inputParams = ref([])

function loadInputParams() {
  try {
    const config = props.formData.paramConfig
    if (config && typeof config === 'string' && config.trim()) {
      inputParams.value = JSON.parse(config)
    } else if (Array.isArray(config)) {
      inputParams.value = config
    } else {
      inputParams.value = []
    }
  } catch {
    inputParams.value = []
  }
}

function syncParamConfig() {
  props.formData.paramConfig = JSON.stringify(inputParams.value)
}

function onParamChange() {
  syncParamConfig()
}

function addInputParam() {
  inputParams.value.push({ key: '', type: 'string' })
  syncParamConfig()
}

function removeInputParam(index) {
  inputParams.value.splice(index, 1)
  syncParamConfig()
}

// 初始化及监听外部变化
loadInputParams()
watch(() => props.formData.paramConfig, () => {
  loadInputParams()
})

// 模板列表
const templateList = [
  {
    value: 'query',
    label: '查询',
    icon: Search,
    description: '单表或多表查询，返回列表数据',
    steps: [
      { partName: '查询数据', partType: 0, varName: 'dataList', varType: 0 }
    ]
  },
  {
    value: 'pageQuery',
    label: '分页查询',
    icon: List,
    description: '带分页和排序的列表查询',
    steps: [
      { partName: '查询数据', partType: 0, varName: 'dataList', varType: 0 },
      { partName: '查询总条数', partType: 0, varName: 'total', varType: 2 }
    ]
  },
  {
    value: 'crud',
    label: '增删改',
    icon: Operation,
    description: '新增、修改、删除数据操作',
    steps: [
      { partName: '新增/更新/删除数据', partType: 0, varName: 'total', varType: 2 }
    ]
  },
  {
    value: 'tree',
    label: '树结构',
    icon: Connection,
    description: '树形结构的查询和操作',
    steps: [
      { partName: '查询数据', partType: 0, varName: 'dataList', varType: 0 },
      { partName: '数据处理', partType: 1, varName: '', varType: 1 }
    ]
  }
]

// 选择模板
async function selectTemplate(template) {
  try {
    await ElMessageBox.confirm(
      `加载「${template.label}」模板将清空当前已配置的节点，确定要继续吗？`,
      '提示',
      {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'warning',
      }
    )
    emit('template-select', template.steps)
  } catch {
    // 用户取消
  }
}

const validateForm = () => {
  if (!formRef.value) return Promise.resolve(false)

  return new Promise((resolve) => {
    formRef.value.validate((valid) => {
      if (valid) {
        emit('validate', true)
        resolve(true)
      } else {
        emit('validate', false)
        resolve(false)
      }
    })
  })
}

const resetForm = () => {
  if (formRef.value) {
    formRef.value.resetFields()
  }
}

const getFormRef = () => {
  return formRef.value
}

defineExpose({
  validateForm,
  resetForm,
  getFormRef
})
</script>

<style scoped>
.step-wrapper {
  width: 100%;
  height: 100%;
  padding: 0 4px;
}

.step-card {
  background: var(--bg-card);
  border-radius: 12px;
  border: 1px solid var(--border-color);
  box-shadow: 0 2px 12px rgba(0, 0, 0, 0.04);
  overflow: hidden;
}

.step-card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 16px 20px;
  background: var(--bg-hover);
  border-bottom: 1px solid var(--border-color);
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

.info-icon {
  background: linear-gradient(135deg, #6BA368 0%, #8CC488 100%);
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

.step-badge {
  padding: 4px 12px;
  border-radius: 20px;
  font-size: 12px;
  font-weight: 500;
}

.info-badge {
  background: linear-gradient(135deg, #6BA368 0%, #8CC488 100%);
  color: #141614;
}

.step-card-body {
  padding: 24px 40px;
  display: flex;
  justify-content: center;
}

.service-form {
}

/* 左右两栏布局 */
.form-layout {
  display: flex;
  gap: 20px;
  align-items: flex-start;
}

.form-left {
  width: 520px;
  flex-shrink: 0;
  display: flex;
  flex-direction: column;
  gap: 16px;
}

.form-right {
  width: 340px;
  flex-shrink: 0;
}

.form-section {
  padding: 20px;
  background: var(--bg-hover);
  border-radius: 10px;
  border: 1px solid var(--border-color);
}

/* 右侧变量面板占满高度 */
.param-section {
  height: 100%;
  display: flex;
  flex-direction: column;
  min-height: 360px;
}

.param-section .param-config-area {
  flex: 1;
  display: flex;
  flex-direction: column;
}

.param-section .param-list {
  flex: 1;
}

.section-title {
  display: flex;
  align-items: center;
  gap: 8px;
  margin-bottom: 18px;
  padding-bottom: 12px;
  border-bottom: 1px dashed var(--border-color);
  font-size: 14px;
  font-weight: 600;
  color: var(--text-primary);
}

.form-grid {
  display: grid;
  grid-template-columns: 1fr;
  gap: 16px;
}

.inner-section {
  margin-top: 16px;
  padding: 0;
  background: none;
  border: none;
}

:deep(.el-form-item) {
  margin-bottom: 0;
}

:deep(.el-form-item__label) {
  font-weight: 500;
  color: var(--el-text-color-regular);
}





:deep(.el-textarea__inner) {
  border-radius: 8px;
  resize: none;
}

:deep(.el-input__wrapper) {
  border-radius: 6px;
}

.template-grid {
  display: flex;
  gap: 12px;
  flex-wrap: wrap;
}

.template-item {
  display: flex;
  align-items: center;
  gap: 6px;
  padding: 8px 14px;
  border: 1px solid var(--border-color);
  border-radius: 6px;
  cursor: pointer;
  font-size: 13px;
  color: var(--text-primary);
  transition: all 0.2s;
  background: var(--bg-card);
}

.template-item:hover {
  border-color: var(--theme-color);
  color: var(--theme-color);
  background: var(--bg-hover);
}

.template-item .el-icon {
  font-size: 14px;
}

/* 输入变量配置区域 */
.param-config-area {
  display: flex;
  flex-direction: column;
}

.param-list {
  display: flex;
  flex-direction: column;
  gap: 8px;
}

.param-item {
  display: flex;
  align-items: center;
  gap: 8px;
}

.param-item .el-input {
  flex: 1;
  min-width: 0;
}

.param-item .el-select {
  width: 100px;
  flex-shrink: 0;
}

.param-empty {
  text-align: center;
  color: var(--text-secondary);
  font-size: 12px;
  padding: 16px 0;
}
</style>