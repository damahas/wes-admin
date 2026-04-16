<template>
  <div class="feature-container">
    <el-card>
      <template #header>
        <span>{{ t('feature.title') }}</span>
      </template>
      <div class="feature-content">
        <p>{{ t('feature.description') }}</p>
      </div>
    </el-card>
  </div>
</template>

<script setup>
import { useI18n } from 'vue-i18n'

const { t } = useI18n()
</script>

<style scoped>
.feature-container {
  width: 100%;
  height: 100%;
}

.feature-content {
  padding: 20px;
  color: var(--text-secondary);
}
</style>
