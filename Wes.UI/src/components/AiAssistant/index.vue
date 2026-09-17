<template>
  <Teleport to="body">
    <el-drawer
      v-model="visible"
      direction="rtl"
      :size="'min(720px, 92vw)'"
      :before-close="onBeforeClose"
      :z-index="2000"
      :show-close="false"
      destroy-on-close
      custom-class="ai-drawer"
      @opened="onDrawerOpened"
    >
      <template #header>
        <div class="ai-drawer__header">
          <span class="ai-drawer__title">{{ t('ai.title') }}</span>
          <div class="ai-drawer__actions">
            <el-button link :title="t('ai.newSession')" @click="onNewSession">
              <i class="fa fa-plus"></i>
            </el-button>
            <el-button link :title="t('ai.history')" @click="openHistory">
              <i class="fa fa-history"></i>
            </el-button>
            <el-button link :title="t('ai.close')" @click="onBeforeClose">
              <i class="fa fa-times"></i>
            </el-button>
          </div>
        </div>
      </template>

      <div class="ai-drawer__body">
        <AiChat
          v-if="view === 'chat'"
          ref="aiChatRef"
          :model="model"
          :models="models"
          :messages="messages"
          :loading="loading"
          :providers="providers"
          :usage="contextUsage"
          :local-tokens="localTokens"
          :max-context="maxContext"
          @update:model="onModelChange"
          @send="onSend"
          @stop="onStop"
        />
        <AiHistory
          v-else
          :sessions="sessions"
          :active-session-id="activeSessionId"
          @select="onSelectSession"
          @delete="onDeleteSession"
          @back="backToChat"
        />
      </div>
    </el-drawer>
  </Teleport>
</template>

<script setup>
import { ref, nextTick } from 'vue'
import { useI18n } from 'vue-i18n'
import AiChat from './AiChat.vue'
import AiHistory from './AiHistory.vue'
import { useAiAssistant } from './useAiAssistant'

const { t } = useI18n()

const {
  loading, sessions, activeSessionId, messages,
  providers, models,
  model, contextUsage, localTokens, maxContext,
  init,
  onNewSession: createSession2, onSelectSession: selectSession2,
  onDeleteSession, onSend, onStop
} = useAiAssistant()

// ==================== 抽屉 / 视图切换 ====================
const visible = ref(false)
const view = ref('chat') // 'chat' | 'history'
const aiChatRef = ref(null)

// 抽屉打开动画结束后滚动到最新消息（此时容器高度已确定，滚动准确）
function onDrawerOpened() {
  nextTick(() => aiChatRef.value?.scrollToBottom(true))
}

async function openDrawer() {
  visible.value = true
  view.value = 'chat'
  await init() // 幂等：仅首次打开时加载列表，之后复用缓存
}
function closeDrawer() { visible.value = false }
defineExpose({ open: openDrawer, close: closeDrawer })

function openHistory() { view.value = 'history' }
function backToChat() { view.value = 'chat' }

function onBeforeClose() { visible.value = false }

// 新建会话后回到对话视图
async function onNewSession() {
  await createSession2()
  view.value = 'chat'
}

// 选择历史会话后回到对话视图
async function onSelectSession(id) {
  await selectSession2(id)
  view.value = 'chat'
}

function onModelChange(v) { model.value = v }
</script>

<style scoped>
:deep(.ai-drawer) {
  border-radius: 16px 0 0 16px;
}
:deep(.ai-drawer .el-drawer__header) {
  margin-bottom: 0;
  border-bottom: 1px solid var(--border-color);
}
:deep(.ai-drawer .el-drawer__body) {
  padding: 0;
  position: relative;
  flex: 1;
  min-height: 0;
  overflow: hidden;
}

/* 自定义内容容器：必须是 flex 列且占满 EP body 的高度，
   否则内部的 .ai-chat / .ai-messages 拿不到受限高度，消息区无法成为滚动容器 */
.ai-drawer__body {
  display: flex;
  flex-direction: column;
  height: 100%;
  min-height: 0;
  flex: 1;
  overflow: hidden;
}

.ai-drawer__header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  width: 100%;
}
.ai-drawer__title {
  font-size: 16px;
  font-weight: 600;
  color: var(--text-title);
}
.ai-drawer__actions {
  display: flex;
  align-items: center;
  gap: 2px;
}
.ai-drawer__actions .el-button {
  color: var(--text-secondary);
  font-size: 16px;
  padding: 2px 3px;
}
.ai-drawer__actions .el-button i {
  font-size: 16px;
  vertical-align: middle;
}
.ai-drawer__actions .el-button:hover {
  color: var(--theme-color);
  background: var(--theme-color-light);
}

.ai-drawer__body {
  display: flex;
  flex-direction: column;
  flex: 1;
  overflow: hidden;
}
</style>
