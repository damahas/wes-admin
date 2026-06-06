<template>
  <div class="login-container">
    <div class="login-panel">
      <el-dropdown class="lang-dropdown" @command="handleLangChange">
        <span class="lang-text">
          <i class="fa fa-language" style="font-size: 12px"></i>
          <span>{{ currentLangDisplay }}</span>
          <i class="fa-solid fa-angle-down"></i>
        </span>
        <template #dropdown>
          <el-dropdown-menu>
            <el-dropdown-item
              v-for="item in langList"
              :key="item.langCode"
              :command="item.langCode"
            >
              {{ item.langName }}
            </el-dropdown-item>
          </el-dropdown-menu>
        </template>
      </el-dropdown>

      <div class="login-title">{{ t("login.title") }}</div>

      <div class="form-item">
        <div class="form-label">
          <span class="required">*</span>
          {{ t("login.userName") }}
        </div>
        <el-input
          v-model="loginForm.userName"
          class="form-input"
          :placeholder="t('login.userNamePlaceholder')"
          @change="isUserNameError = !loginForm.userName"
        />
        <div class="form-error" v-if="isUserNameError">
          {{ t("login.userNamePlaceholder") }}
        </div>
      </div>

      <div class="form-item">
        <div class="form-label">
          <span class="required">*</span>
          {{ t("login.password") }}
        </div>
        <el-input
          v-model="loginForm.password"
          class="form-input"
          :placeholder="t('login.passwordPlaceholder')"
          type="password"
          @change="isPasswordError = !loginForm.password"
        />
        <div class="form-error" v-if="isPasswordError">
          {{ t("login.passwordPlaceholder") }}
        </div>
      </div>

      <div class="form-item" style="margin-top: 50px">
        <slider-code
          ref="sliderCodeRef"
          style="width: 100%"
          @handleOk="handleSliderSuccess"
        ></slider-code>
      </div>

      <div class="login-btn" @click="handleLogin">{{ t("login.login") }}</div>
    </div>
  </div>
</template>

<script setup>
import { reactive, ref, computed } from "vue";
import { useRouter, useRoute } from "vue-router";
import { useStore } from "vuex";
import { useI18n } from "vue-i18n";
import { ElMessage } from "element-plus";
import SliderCode from "@/components/SliderCode";
import { login } from "@/api/login";

const router = useRouter();
const route = useRoute();
const store = useStore();
const { t, locale } = useI18n();

const loginForm = reactive({
  userName: "",
  password: "",
  code: "",
  positionX: 0,
});
const sliderCodeRef = ref();

const isUserNameError = ref(false);
const isPasswordError = ref(false);

const langList = computed(() => store.getters["system/langList"]);
const currentLangDisplay = computed(() => {
  const lang = store.getters["system/locale"] || "en";
  return langList.value.find((p) => p.langCode === lang)?.langName;
});

const handleLangChange = (lang) => {
  store.dispatch("system/setLocale", lang);
  locale.value = lang;
};

const handleSliderSuccess = (data) => {
  if (data) {
    loginForm.code = data.code;
    loginForm.positionX = data.positionX;
    if (loginForm.userName && loginForm.password) {
      handleLogin();
    } else {
      sliderCodeRef.$message({
        message: "验证成功！",
        type: "success",
      });
    }
    return;
  }
  loginForm.code = "";
  loginForm.positionX = 0;
};

const handleLogin = () => {
  if (!loginForm.userName) {
    isUserNameError.value = true;
  }
  if (!loginForm.password) {
    isPasswordError.value = true;
  }
  if (!loginForm.userName || !loginForm.password) {
    return;
  }

  isUserNameError.value = false;
  isPasswordError.value = false;

  login(loginForm)
    .then((res) => {
      ElMessage.success(t("login.success"));
      store.dispatch("user/setAccessToken", res.token);
      // store.dispatch("user/login", {
      //   username: loginForm.userName,
      // });
      router.push(route.query.redirect || "/");
    })
    .catch(() => {
      // 重新获取验证码
        sliderCodeRef.value.reset();
    });
};
</script>

<style scoped>
.login-container {
  width: 100%;
  height: 100vh;
  background-size: 100% 100%;
  background-repeat: no-repeat;
  background-position: center;
  background-image: url(/images/login/login-bg.jpg);
  display: flex;
  align-items: center;
  justify-content: flex-end;
  position: relative;
}

.login-logo {
  position: absolute;
  top: 48px;
  left: 63px;
}

.login-panel {
  position: relative;
  width: 480px;
  height: 580px;
  border-radius: 12px;
  background-color: var(--bg-card);
  margin-right: 145px;
  padding: 0 69px;
  box-shadow: 0 2px 10px rgba(0, 0, 0, 0.1);
}

.lang-dropdown {
  position: absolute;
  top: 29px;
  right: 27px;
}

.lang-text {
  color: #999999;
  display: flex;
  align-items: center;
  gap: 3px;
  cursor: pointer;
}

.lang-text span {
  font-size: 14px;
  margin-left: 3px;
  margin-right: 9px;
}

.login-title {
  font-size: 24px;
  font-weight: 700;
  color: #6c5fb1;
  text-align: center;
  margin-top: 103px;
}

.form-item {
  margin-top: 30px;
  position: relative;
}

.form-item:first-of-type {
  margin-top: 41px;
}

.form-label {
  font-size: 16px;
  font-weight: 500;
  color: #5d5a70;
}

.required {
  color: #ff0000;
  margin-right: 3px;
}

.form-input {
  margin-top: 15px;
}

.form-error {
  color: #ff0000;
  font-size: 12px;
  margin-top: 3px;
}

.login-btn {
  background: #6c5fb1;
  border-radius: 7.28px;
  color: white;
  font-size: 18px;
  font-weight: 400;
  /* margin-top: 89px; */
  margin-top: 59px;
  height: 36px;
  width: 341px;
  display: flex;
  align-items: center;
  justify-content: center;
  cursor: pointer;
}

.login-btn:hover {
  background: #5d4fa3;
}

.login-btn:active {
  background: #4e4295;
}
</style>
