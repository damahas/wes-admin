<template>
  <div class="ai-history">
    <div class="ai-history__header">
      <el-button link class="ai-history__back" :title="t('ai.back')" @click="emit('back')">
        <el-icon><ArrowLeft /></el-icon>
      </el-button>
      <span class="ai-history__title-text">{{ t('ai.history') }}</span>
      <span class="ai-history__spacer"></span>
    </div>
    <div class="ai-history__list">
      <div
        v-for="s in sessions"
        :key="s.id"
        class="ai-history__item"
        :class="{ active: s.id === activeSessionId }"
        @click="emit('select', s.id)"
      >
        <el-icon><ChatLineRound /></el-icon>
        <span class="ai-history__title">{{ s.title }}</span>
        <span class="ai-history__time">{{ formatTime(s.updatedAt) }}</span>
        <el-button
          link
          size="small"
          class="ai-history__delete"
          @click.stop="emit('delete', s.id)"
        >
          <el-icon><Delete /></el-icon>
        </el-button>
      </div>
    </div>
  </div>
</template>

<script setup>
import { useI18n } from 'vue-i18n'
import { ArrowLeft, ChatLineRound, Delete } from '@element-plus/icons-vue'

const props = defineProps({
  sessions: { type: Array, default: () => [] },
  activeSessionId: { type: [String, Number], default: null }
})
const emit = defineEmits(['select', 'delete', 'back'])

const { t } = useI18n()

/** 友好的时间展示：今天显示时分，今年显示月日，往年显示完整日期 */
function formatTime(v) {
  if (!v) return ''
  const d = new Date(v)
  if (isNaN(d.getTime())) return ''
  const now = new Date()
  const pad = n => String(n).padStart(2, '0')
  const hm = `${pad(d.getHours())}:${pad(d.getMinutes())}`
  if (d.toDateString() === now.toDateString()) return hm
  if (d.getFullYear() === now.getFullYear()) return `${d.getMonth() + 1}月${d.getDate()}日`
  return `${d.getFullYear()}-${pad(d.getMonth() + 1)}-${pad(d.getDate())}`
}
</script>

<style scoped>
.ai-history {
  flex: 1;
  display: flex;
  flex-direction: column;
  overflow: hidden;
  min-height: 0;
  background: var(--bg-primary);
}
.ai-history__header {
  display: flex;
  align-items: center;
  padding: 12px 14px;
  border-bottom: 1px solid var(--border-color-light);
  font-size: 14px;
  font-weight: 600;
  color: var(--text-title);
}
.ai-history__back {
  color: var(--text-secondary);
  font-size: 16px;
}
.ai-history__back:hover {
  color: var(--theme-color);
}
.ai-history__title-text {
  flex: 1;
  text-align: center;
}
.ai-history__spacer {
  width: 28px;
  flex-shrink: 0;
}
.ai-history__list {
  flex: 1;
  overflow-y: auto;
  padding: 8px;
}
.ai-history__item {
  display: flex;
  align-items: center;
  gap: 8px;
  padding: 10px;
  border-radius: 8px;
  cursor: pointer;
  color: var(--text-primary);
  font-size: 14px;
  transition: background-color 0.2s;
}
.ai-history__item:hover {
  background: var(--bg-hover);
}
.ai-history__item.active {
  background: var(--theme-color-light, rgba(107, 163, 104, 0.12));
  color: var(--theme-color);
}
.ai-history__title {
  flex: 1;
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
}
.ai-history__time {
  flex-shrink: 0;
  font-size: 12px;
  color: var(--text-placeholder);
  margin-left: 4px;
}
.ai-history__delete {
  opacity: 0;
  color: var(--text-secondary);
}
.ai-history__item:hover .ai-history__delete {
  opacity: 1;
}
.ai-history__delete:hover {
  color: var(--danger-color, #f56c6c) !important;
}
</style>
