<template>
  <router-view />
  <LicenseDialog />
  <AiAssistant ref="aiRef" />
</template>

<script setup>
import { ref, provide } from 'vue'
import LicenseDialog from '@/views/system/license/dialog.vue'
import AiAssistant from '@/components/AiAssistant/index.vue'

const aiRef = ref(null)

// 提供给 Header 右上角 AI 按钮调用，打开 AI 抽屉
provide('openAi', () => aiRef.value?.open())
</script>

<style>
* {
  margin: 0;
  padding: 0;
  box-sizing: border-box;
}

html, body, #app {
  width: 100%;
  height: 100%;
}

#app {
  font-family: 'Inter', 'PingFang SC', 'Microsoft YaHei', 'Noto Sans SC',
    system-ui, -apple-system, sans-serif;
}
</style>
