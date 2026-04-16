<template>
  <div class="basic-info-step">
    <el-card shadow="never" class="form-card">
      <template #header>
        <div class="card-header">
          <span>基础信息配置</span>
        </div>
      </template>
      <el-form :model="formData" :rules="rules" ref="formRef" label-width="100px">
        <el-form-item label="服务编码" prop="serviceCode">
          <el-input
            v-model="formData.serviceCode"
            placeholder="请输入服务编码"
            maxlength="50"
          />
        </el-form-item>
        <el-form-item label="服务名称" prop="serviceName">
          <el-input
            v-model="formData.serviceName"
            placeholder="请输入服务名称"
            maxlength="100"
          />
        </el-form-item>
        <el-form-item label="服务类型" prop="serviceType">
          <el-radio-group v-model="formData.serviceType">
            <el-radio label="1">查询</el-radio>
            <el-radio label="2">分页查询</el-radio>
            <el-radio label="3">增删改</el-radio>
          </el-radio-group>
        </el-form-item>
        <el-form-item label="分类" prop="category">
          <DictSelect
            v-model="formData.category"
            :options="categoryOptions"
            placeholder="请选择分类"
            style="width: 100%"
          />
        </el-form-item>
        <el-form-item label="状态">
          <el-radio-group v-model="formData.isEnabled">
            <el-radio
              v-for="item in statusOptions || []"
              :key="item.value"
              :value="item.value"
            >
              {{ item.label }}
            </el-radio>
          </el-radio-group>
        </el-form-item>
        <el-form-item label="描述">
          <el-input
            v-model="formData.remark"
            type="textarea"
            placeholder="请输入描述"
            maxlength="500"
            :rows="3"
          />
        </el-form-item>
      </el-form>
    </el-card>
  </div>
</template>

<script setup name="BasicInfoStep">
import { defineProps, defineEmits, ref } from 'vue'
import DictSelect from '@/components/DictSelect'

const props = defineProps({
  // 表单数据
  formData: {
    type: Object,
    required: true,
    default: () => ({
      serviceCode: '',
      serviceName: '',
      serviceType: '',
      category: '',
      isEnabled: 1,
      remark: ''
    })
  },
  // 分类选项
  categoryOptions: {
    type: Array,
    default: () => []
  },
  // 状态选项
  statusOptions: {
    type: Array,
    default: () => []
  },
  // 表单验证规则
  rules: {
    type: Object,
    default: () => ({
      serviceCode: [
        { required: true, message: '服务编码不能为空', trigger: 'blur' },
        {
          max: 50,
          message: '服务编码长度不能超过50个字符',
          trigger: 'blur'
        }
      ],
      serviceName: [
        { required: true, message: '服务名称不能为空', trigger: 'blur' },
        {
          max: 100,
          message: '服务名称长度不能超过100个字符',
          trigger: 'blur'
        }
      ],
      serviceType: [{ required: true, message: '请选择服务类型', trigger: 'change' }],
      category: [{ required: true, message: '请选择分类', trigger: 'change' }]
    })
  }
})

const emit = defineEmits(['validate'])

// 表单引用
const formRef = ref(null)

// 表单验证方法
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

// 重置表单
const resetForm = () => {
  if (formRef.value) {
    formRef.value.resetFields()
  }
}

// 获取表单引用，供父组件调用
const getFormRef = () => {
  return formRef.value
}

// 暴露方法给父组件
defineExpose({
  validateForm,
  resetForm,
  getFormRef
})
</script>

<style scoped>
.basic-info-step {
  width: 100%;
}

.form-card {
  min-height: 400px;
}

.card-header {
  font-weight: 600;
  color: var(--el-text-color-primary);
  font-size: 16px;
}
</style>