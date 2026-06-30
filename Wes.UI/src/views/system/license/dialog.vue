<template>
  <el-dialog
    v-model="visible"
    :title="t('license.dialogTitle')"
    width="520px"
    :close-on-click-modal="false"
    :close-on-press-escape="false"
    destroy-on-close
    @open="handleOpen"
  >
    <el-form ref="formRef" :model="form" :rules="rules" label-width="110px" v-loading="loading">
      <el-form-item :label="t('license.platformSystem')">
        <el-input :model-value="form.platformOS" disabled />
      </el-form-item>
      <el-form-item :label="t('license.platformCode')">
        <el-input :model-value="form.platformCode" disabled />
      </el-form-item>
      <el-form-item :label="t('license.licenseKey')" prop="licenseCode">
        <el-input
          type="textarea"
          :rows="6"
          :placeholder="t('license.licenseKeyPlaceholder')"
          v-model="form.licenseCode"
        />
      </el-form-item>
    </el-form>

    <template #footer>
      <el-button @click="handleClose">{{ t('common.cancel') }}</el-button>
      <el-button type="primary" :loading="submitting" @click="handleSave">
        {{ t('license.activate') }}
      </el-button>
    </template>
  </el-dialog>
</template>

<script setup>
import { ref, reactive, computed } from 'vue'
import { useStore } from 'vuex'
import { useI18n } from 'vue-i18n'
import { getLicense, saveLicense } from '@/api/system/license'
import { ElMessage } from 'element-plus'

const store = useStore()
const { t } = useI18n()

const visible = computed({
  get: () => store.state.system.licenseDialogVisible,
  set: v => store.dispatch('system/setLicenseDialog', v)
})

const formRef = ref(null)
const loading = ref(false)
const submitting = ref(false)

const form = reactive({
  platformOS: '',
  platformCode: '',
  licenseCode: ''
})

const rules = {
  licenseCode: [
    { required: true, message: () => t('license.licenseKeyPlaceholder'), trigger: 'blur' }
  ]
}

async function handleOpen() {
  loading.value = true
  try {
    const res = await getLicense()
    form.platformOS = res.data.platformOS || ''
    form.platformCode = res.data.platformCode || ''
    form.licenseCode = ''
  } catch {
    ElMessage.error(t('request.requestFailed'))
  } finally {
    loading.value = false
  }
}

function handleClose() {
  visible.value = false
}

async function handleSave() {
  const valid = await formRef.value.validate().catch(() => false)
  if (!valid) return

  submitting.value = true
  try {
    await saveLicense(form)
    ElMessage.success(t('license.activateSuccess') || '激活成功')
    visible.value = false
  } catch {
    // 错误由拦截器统一提示
  } finally {
    submitting.value = false
  }
}
</script>
