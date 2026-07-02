<template>
   <el-form ref="pwdRef" :model="user" :rules="rules" label-width="80px">
      <el-form-item :label="t('user.oldPassword')" prop="oldPassword">
         <el-input v-model="user.oldPassword" :placeholder="t('user.placeholder.oldPassword')" type="password" show-password />
      </el-form-item>
      <el-form-item :label="t('user.newPassword')" prop="newPassword">
         <el-input v-model="user.newPassword" :placeholder="t('user.placeholder.newPassword')" type="password" show-password />
      </el-form-item>
      <el-form-item :label="t('user.confirmPassword')" prop="confirmPassword">
         <el-input v-model="user.confirmPassword" :placeholder="t('user.placeholder.confirmPassword')" type="password" show-password/>
      </el-form-item>
      <el-form-item>
      <el-button type="primary" @click="submit">{{ t('common.save') }}</el-button>
      <el-button type="danger" @click="close">{{ t('common.close') }}</el-button>
      </el-form-item>
   </el-form>
</template>

<script setup>
import { useI18n } from 'vue-i18n'
import { updateUserPwd } from "@/api/system/user";

const { t } = useI18n()
const { proxy } = getCurrentInstance();

const user = reactive({
  oldPassword: undefined,
  newPassword: undefined,
  confirmPassword: undefined
});

const equalToPassword = (rule, value, callback) => {
  if (user.newPassword !== value) {
    callback(new Error(t("user.rules.passwordMismatch")));
  } else {
    callback();
  }
};

const rules = ref({
  oldPassword: [{ required: true, message: t("user.rules.oldPasswordRequired"), trigger: "blur" }],
  newPassword: [{ required: true, message: t("user.rules.newPasswordRequired"), trigger: "blur" }, { min: 6, max: 20, message: t("user.rules.passwordLengthHint"), trigger: "blur" }, { pattern: /^[^<>"'|\\]+$/, message: t("user.rules.invalidChars"), trigger: "blur" }],
  confirmPassword: [{ required: true, message: t("user.rules.confirmPasswordRequired"), trigger: "blur" }, { required: true, validator: equalToPassword, trigger: "blur" }]
});

/** 提交按钮 */
function submit() {
  proxy.$refs.pwdRef.validate(valid => {
    if (valid) {
      updateUserPwd(user.oldPassword, user.newPassword).then(response => {
        proxy.$modal.msgSuccess(t("common.editSuccess"));
      });
    }
  });
};

/** 关闭按钮 */
function close() {
  proxy.$tab.closePage();
};
</script>
