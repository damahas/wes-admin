<template>
  <el-header class="header">
    <div class="header-right">
      <Message />

      <el-dropdown @command="handleLangChange">
        <span class="lang-btn">
          {{ langList.find((p) => p.langCode === currentLang)?.langName }}
          <el-icon><ArrowDown /></el-icon>
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

      <el-switch
        v-model="isDark"
        class="dark-switch"
        inline-prompt
        :active-icon="Moon"
        :inactive-icon="Sunny"
        @change="handleDarkChange"
      />

      <el-dropdown @command="handleCommand">
        <span class="user-info">
          <el-icon><User /></el-icon>
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
  justify-content: flex-end;
  align-items: center;
  padding: 0 20px;
  border: none;
  box-shadow: none;
}

.header-right {
  display: flex;
  align-items: center;
  gap: 20px;
}

.lang-btn {
  display: flex;
  align-items: center;
  gap: 4px;
  cursor: pointer;
  color: var(--text-primary);
}

.dark-switch {
  --el-switch-on-color: #302d39;
  --el-switch-off-color: var(--theme-color);
}

.user-info {
  display: flex;
  align-items: center;
  gap: 8px;
  cursor: pointer;
  color: var(--text-primary);
}
</style>
