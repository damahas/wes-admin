<template>
  <div class="home-container">
    <!-- <el-card class="welcome-card">
      <h1 class="welcome-title">{{ t('home.welcome') }}</h1>
      <p class="welcome-desc">{{ t('home.description') }}</p>
    </el-card> -->
  </div>
</template>

<script setup>
import { useI18n } from 'vue-i18n'

const { t } = useI18n()
</script>

<style scoped>
.home-container {
  width: 100%;
  height: 100%;
}

.welcome-card {
  max-width: 800px;
  margin: 0 auto;
  margin-top: 100px;
}

.welcome-title {
  text-align: center;
  color: var(--text-primary);
  font-size: 32px;
  margin-bottom: 20px;
}

.welcome-desc {
  text-align: center;
  color: var(--text-secondary);
  font-size: 16px;
}
</style>
