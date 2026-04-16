<template>
  <el-dialog
    v-model="dialogVisible"
    title="添加步骤"
    width="500px"
    :close-on-click-modal="false"
    :close-on-press-escape="false"
  >
    <el-form :model="formData" label-width="100px">
      <el-form-item label="步骤名称" required>
        <el-input
          v-model="formData.partName"
          placeholder="请输入步骤名称"
          maxlength="50"
          show-word-limit
        />
      </el-form-item>
      
      <el-form-item label="输出变量名" required>
        <el-input
          v-model="formData.varName"
          placeholder="请输入输出变量名（用于后续步骤引用）"
          maxlength="50"
          show-word-limit
        />
      </el-form-item>
      
      <el-form-item label="节点类型" required>
        <el-radio-group v-model="formData.partType">
          <el-radio :value="0">SQL节点</el-radio>
          <el-radio :value="1">JS节点</el-radio>
        </el-radio-group>
      </el-form-item>
    </el-form>
    
    <template #footer>
      <span class="dialog-footer">
        <el-button @click="handleCancel">取消</el-button>
        <el-button type="primary" @click="handleConfirm">确定</el-button>
      </span>
    </template>
  </el-dialog>
</template>

<script setup>
import { ref, watch } from 'vue'
import { ElMessage } from 'element-plus'

const props = defineProps({
  visible: {
    type: Boolean,
    default: false
  },
  defaultValues: {
    type: Object,
    default: () => ({
      partName: '',
      varName: '',
      partType: 0
    })
  }
})

const emit = defineEmits(['update:visible', 'confirm', 'cancel'])

const dialogVisible = ref(false)
const formData = ref({
  partName: '',
  varName: '',
  partType: 0
})

// 监听visible变化
watch(() => props.visible, (newVal) => {
  dialogVisible.value = newVal
})

// 监听dialogVisible变化
watch(() => dialogVisible.value, (newVal) => {
  emit('update:visible', newVal)
  if (!newVal) {
    emit('cancel')
  }
})

// 监听defaultValues变化
watch(() => props.defaultValues, (newVal) => {
  formData.value = { ...newVal }
}, { immediate: true })

function handleConfirm() {
  if (!formData.value.partName.trim()) {
    ElMessage.error('步骤名称不能为空')
    return
  }
  
  if (!formData.value.varName.trim()) {
    ElMessage.error('输出变量名不能为空')
    return
  }
  
  emit('confirm', {
    partName: formData.value.partName.trim(),
    varName: formData.value.varName.trim(),
    partType: formData.value.partType
  })
  
  dialogVisible.value = false
}

function handleCancel() {
  dialogVisible.value = false
  emit('cancel')
}
</script>

<style scoped>
.dialog-footer {
  display: flex;
  justify-content: flex-end;
  gap: 10px;
}
</style>