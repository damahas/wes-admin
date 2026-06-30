<template>
  <el-header class="header">
    <div class="header-left">
      <div class="header-logo">
        <svg width="28" height="28" viewBox="0 0 28 28" fill="none">
          <path d="M14 2L24.5 7.5V18.5L14 24L3.5 18.5V7.5L14 2Z" fill="#8CC488" stroke="#6BA368" stroke-width="1.5"/>
          <path d="M14 8L20 11.2V17.6L14 20.8L8 17.6V11.2L14 8Z" fill="#D6E7D4" stroke="#6BA368" stroke-width="1"/>
        </svg>
      </div>
      <span class="header-title">WES管理系统</span>
    </div>
    <div class="header-right">
      <Message />

      <span class="header-icon-btn" @click="handleDarkChange(!isDark)">
        <el-icon v-if="isDark"><Sunny /></el-icon>
        <el-icon v-else><Moon /></el-icon>
      </span>

      <el-dropdown @command="handleLangChange">
        <span class="header-icon-btn">
          <i class="fa fa-globe"></i>
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

      <el-dropdown @command="handleCommand">
        <span class="user-info">
          {{ userInfo?.userName || t("common.user") }}
          <el-icon class="el-icon--right"><arrow-down /></el-icon>
        </span>
        <template #dropdown>
          <el-dropdown-menu>
            <el-dropdown-item command="logout">{{ t("common.logout") }}</el-dropdown-item>
          </el-dropdown-menu>
        </template>
      </el-dropdown>
    </div>
  </el-header>
</template>

<script setup>
import { computed } from "vue";
import { useRouter } from "vue-router";
import { useStore } from "vuex";
import { useI18n } from "vue-i18n";
import { Moon, Sunny, ArrowDown } from "@element-plus/icons-vue";
import Message from "@/layout/components/Message";

const router = useRouter();
const store = useStore();
const { t, locale } = useI18n();

const isDark = computed(() => store.getters["system/isDark"]);
const currentLang = computed(() => store.getters["system/locale"]);
const langList = computed(() => store.getters["system/langList"]);
const userInfo = computed(() => store.getters["user/userInfo"]);

const handleDarkChange = (value) => {
  store.dispatch("system/setDark", value);
};

const handleLangChange = (lang) => {
  store.dispatch("system/setLocale", lang);
  locale.value = lang;
  location.reload();
};

const handleCommand = (command) => {
  if (command === "logout") {
    store.dispatch("user/logout");
    router.push("/login");
  }
};
</script>

<style scoped>
.header {
  height: 48px;
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 0 20px;
  border: none;
  box-shadow: none;
  background-color: var(--bg-header);
  border-bottom: 1px solid var(--border-color);
  flex-shrink: 0;
}

.header-left {
  display: flex;
  align-items: center;
  gap: 10px;
}

.header-logo {
  display: flex;
  align-items: center;
  justify-content: center;
}

.header-title {
  font-size: 18px;
  font-weight: 500;
  color: var(--text-title);
  letter-spacing: 0.01em;
}

.header-right {
  display: flex;
  align-items: center;
  gap: 8px;
}

.header-icon-btn {
  display: flex;
  align-items: center;
  justify-content: center;
  width: 32px;
  height: 32px;
  border-radius: 50%;
  cursor: pointer;
  color: var(--text-secondary);
  font-size: 15px;
  transition: background-color 0.2s;
}

.header-icon-btn:hover {
  background-color: var(--bg-hover);
}

.user-info {
  display: flex;
  align-items: center;
  gap: 8px;
  padding-left: 6px;
  cursor: pointer;
  color: var(--text-primary);
}


</style>
