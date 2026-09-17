<template>
  <div class="ai-chat">
    <!-- 消息列表 -->
    <div ref="msgRef" class="ai-messages" @click="onMessagesClick" @scroll="onScroll">
      <div v-if="messages.length === 0" class="ai-empty">
        <div class="ai-empty__icon">
          <svg viewBox="0 0 24 24" fill="none" width="56" height="56">
            <path d="M20 2H4c-1.1 0-2 .9-2 2v18l4-4h14c1.1 0 2-.9 2-2V4c0-1.1-.9-2-2-2z" fill="var(--theme-color)" opacity="0.15"/>
            <circle cx="8" cy="10" r="1.5" fill="var(--theme-color)" opacity="0.5"/>
            <circle cx="12" cy="10" r="1.5" fill="var(--theme-color)" opacity="0.5"/>
            <circle cx="16" cy="10" r="1.5" fill="var(--theme-color)" opacity="0.5"/>
          </svg>
        </div>
        <p>{{ t('ai.empty') }}</p>
      </div>
      <div v-if="hasMore" class="ai-load-more">
        <el-button link size="small" @click="loadMore">加载更早的 {{ hiddenCount }} 条消息</el-button>
      </div>
      <div
        v-for="(msg, i) in visibleMessages"
        :key="start + i"
        class="ai-msg"
        :class="`ai-msg--${msg.role}`"
        v-memo="[msg.content, msg.role, msg.toolCalls, msg.tokens, msg.elapsedMs, msg.promptTokens, msg.completionTokens, msg.tokensEstimated, msg.usageSource]"
      >
        <div class="ai-msg__avatar">
          <span v-if="msg.role === 'user'">U</span>
          <span v-else class="ai-msg__avatar-ai">AI</span>
        </div>
        <div class="ai-msg__body">
          <!-- 正在生成中的空助手消息：在气泡内显示打字动画，而不是额外再出现一个加载气泡 -->
          <div
            class="ai-msg__content"
            v-html="renderMarkdown(msg.content)"
            v-if="!(msg.role === 'assistant' && !msg.content && loading && start + i === messages.length - 1)"
          ></div>
          <div class="ai-typing" v-else><span /><span /><span /></div>
          <div v-if="msg.toolCalls?.length" class="ai-msg__tools">
            <span
              v-for="tc in msg.toolCalls"
              :key="tc.id"
              class="ai-tool-badge"
            >{{ t('ai.toolCalling') }}: {{ tc.name }}</span>
          </div>
          <!-- 回答统计徽章：token 消耗 + 耗时（提问不展示统计） -->
          <div v-if="msg.role === 'assistant'" class="ai-msg__meta">
            <span v-if="tokensOf(msg) > 0" class="ai-msg__badge" :title="tokensTitle(msg)">
              <i class="fa-solid fa-database"></i>
              Tokens: {{ msg.tokensEstimated ? '~' : '' }}{{ formatTokens(tokensOf(msg)) }}
            </span>
            <span v-if="msg.elapsedMs > 0" class="ai-msg__badge">
              <i class="fa-solid fa-clock"></i>
              {{ t('ai.elapsed') }}: {{ formatElapsed(msg.elapsedMs) }}
            </span>
          </div>
        </div>
      </div>
    </div>

    <!-- 输入区：圆角聊天框 -->
    <div class="ai-input-card">
      <div class="ai-input">
        <el-input
          v-model="input"
          type="textarea"
          :autosize="{ minRows: 1, maxRows: 4 }"
          :placeholder="t('ai.placeholder')"
          resize="none"
          @keydown.enter.exact="onEnter"
        />
      </div>
      <div class="ai-input-toolbar">
        <div class="ai-toolbar-left">
          <ContextRing
            :used="contextUsed"
            :max="maxContext"
            :compressed="!!usage?.compressed"
            class="ai-context-ring"
          />
          <el-dropdown trigger="click" @command="v => emit('update:model', v)" class="ai-dropdown">
            <span class="ai-dropdown-link">
              {{ modelLabel }} <i class="fa fa-chevron-down"></i>
            </span>
            <template #dropdown>
              <el-dropdown-menu>
                <el-dropdown-item
                  v-for="m in models"
                  :key="m.modelId"
                  :command="m.modelId"
                  :class="{ 'is-active': m.modelId === model }"
                >{{ providerName(m.provider) }} · {{ m.displayName }}</el-dropdown-item>
              </el-dropdown-menu>
            </template>
          </el-dropdown>
        </div>
        <el-button
          type="primary"
          :icon="loading ? VideoPause : Promotion"
          circle
          class="ai-send-btn"
          :disabled="!loading && !input.trim()"
          @click="loading ? emit('stop') : onEnter()"
        />
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, watch, nextTick, onMounted, onUnmounted } from 'vue'
import { useI18n } from 'vue-i18n'
import { Promotion, VideoPause } from '@element-plus/icons-vue'
import { renderMarkdown } from './markdown'
import * as echarts from 'echarts'
import ContextRing from './ContextRing.vue'
import { estimateTokens, formatTokens } from '@/utils/contextTokens'

const props = defineProps({
  messages: { type: Array, default: () => [] },
  loading: { type: Boolean, default: false },
  model: { type: String, default: '' },
  models: { type: Array, default: () => [] },
  providers: { type: Array, default: () => [] },
  // 后端返回的最近一次上下文用量（含是否压缩），为空时用本地估算
  usage: { type: Object, default: null },
  localTokens: { type: Number, default: 0 },
  maxContext: { type: Number, default: 0 }
})
const emit = defineEmits(['update:model', 'send', 'stop'])

const { t } = useI18n()
const input = ref('')
const msgRef = ref(null)

// 上下文占用：优先用后端返回的真实用量，未返回时用本地估算；两者都叠加当前输入框的占用
const contextUsed = computed(() => {
  const base = props.usage?.usedTokens ?? props.localTokens
  return base + estimateTokens(input.value)
})

/**
 * 单条消息的 token 数：优先用落库/流式结束后写入的 msg.tokens，否则本地实时估算。
 * 内容为空（正在生成）时返回 0 不展示，避免每来一个字就重算一遍长文本；
 * 这里不依赖 loading，避免 loading 未复位导致最后一条始终不显示徽章。
 */
function tokensOf(msg) {
  if (msg.role === 'assistant' && !msg.content) return 0
  return msg.tokens ?? estimateTokens(msg.content)
}

/** token 徽章悬浮说明：区分「模型真实返回」「输入实测+输出估算」「全部本地估算」 */
function tokensTitle(msg) {
  if (msg.usageSource === 'model') return t('ai.tokensReal')
  if (msg.usageSource === 'probe') return t('ai.tokensProbe')
  if (msg.tokensEstimated === true) return t('ai.tokensEstimated')
  if (msg.promptTokens != null) return t('ai.tokensReal')
  return t('ai.outputTokens')
}

/** 回答耗时格式化：<1分钟显示秒（1 位小数），超过则显示 x m y s */
function formatElapsed(ms) {
  if (ms < 60000) return `${(ms / 1000).toFixed(1)}s`
  const m = Math.floor(ms / 60000)
  const s = Math.round((ms % 60000) / 1000)
  return `${m}m ${s}s`
}

const providerName = (name) =>
  props.providers.find(p => p.name === name)?.displayName || name
const modelLabel = computed(() => {
  const m = props.models.find(x => x.modelId === props.model)
  if (!m) return t('ai.model')
  return `${providerName(m.provider)} · ${m.displayName}`
})

function onEnter() {
  const text = input.value.trim()
  if (!text || props.loading) return
  emit('send', text)
  input.value = ''
}

function scrollToBottom(force = false) {
  nextTick(() => {
    const el = msgRef.value
    if (!el) return
    // 非强制模式（流式增量）：仅当用户已在底部附近时才自动跟随，避免打断查看历史
    if (!force) {
      const distanceToBottom = el.scrollHeight - el.scrollTop - el.clientHeight
      if (distanceToBottom > 120) return
    }
    el.scrollTop = el.scrollHeight
  })
}

// 窗口化渲染：只渲染最近 renderCount 条消息，向上滚动时增量加载更早消息，
// 避免长会话（数百条）一次性渲染导致卡顿。历史消息在内存中完整保留（useAiAssistant 已全量加载）。
const renderCount = ref(40)
const start = computed(() => Math.max(0, props.messages.length - renderCount.value))
const visibleMessages = computed(() => props.messages.slice(start.value))
const hasMore = computed(() => start.value > 0)
const hiddenCount = computed(() => start.value)
let loadingMore = false

function loadMore() {
  if (loadingMore || !hasMore.value) return
  const el = msgRef.value
  const prevHeight = el?.scrollHeight ?? 0
  const prevTop = el?.scrollTop ?? 0
  loadingMore = true
  renderCount.value += 40
  nextTick(() => {
    const el2 = msgRef.value
    if (el2) el2.scrollTop = el2.scrollHeight - prevHeight + prevTop // 保持视图位置不跳动
    loadingMore = false
  })
}

function onScroll() {
  const el = msgRef.value
  if (!el || loadingMore || !hasMore.value) return
  if (el.scrollTop < 60) loadMore()
}

// 代码块复制按钮（事件委托，避免 v-html 内联脚本）
function onMessagesClick(e) {
  const btn = e.target.closest('.ai-copy-btn')
  if (!btn) return
  const code = btn.closest('.ai-code-block')?.querySelector('code')?.textContent || ''
  if (!code) return
  const done = () => {
    const old = btn.textContent
    btn.textContent = '已复制'
    setTimeout(() => (btn.textContent = old), 1500)
  }
  if (navigator.clipboard?.writeText) {
    navigator.clipboard.writeText(code).then(done).catch(done)
  } else {
    done()
  }
}

// ==================== ECharts 图表挂载 ====================
// markdown.js 把 ```echart 代码块渲染成 <div class="ai-chart" data-option="..."> 占位，
// 这里在消息渲染后扫描占位元素，初始化 echarts 实例并 setOption。
const chartInstances = new Map() // el -> { inst, option }
let chartResizeObserver = null // 兜底：容器尺寸变化（气泡撑开、抽屉动画）时自动 resize

// 根据当前亮/暗模式生成 ECharts 全局基础样式，统一配色与坐标轴/文字颜色
function getChartBaseOption() {
  const isDark = document.documentElement.classList.contains('dark')
  const axisLine = { lineStyle: { color: isDark ? '#3E403E' : '#DCDCD6' } }
  const axisLabel = { color: isDark ? '#B0B0A6' : '#5E5E58' }
  const splitLine = { lineStyle: { color: isDark ? '#2C2E2C' : '#EDEDE7' } }
  return {
    // 与项目主题（绿色主色）协调的配色板
    color: ['#6BA368', '#4C9AFF', '#F2A93B', '#E06C75', '#9B7EDE', '#56C2C6', '#E0A458'],
    backgroundColor: 'transparent',
    textStyle: { color: isDark ? '#B0B0A6' : '#5E5E58' },
    title: {
      textStyle: { color: isDark ? '#FAFAF8' : '#141412', fontSize: 15 },
      left: 'center',
      top: 14
    },
    // 图例位置由具体图表决定，这里只统一文字颜色；无位置时默认顶部
    legend: { textStyle: { color: isDark ? '#B0B0A6' : '#5E5E58' } },
    tooltip: {
      backgroundColor: isDark ? '#1E201E' : '#FFFFFF',
      borderColor: isDark ? '#3E403E' : '#EDEDE7',
      borderWidth: 1,
      textStyle: { color: isDark ? '#EBEBE5' : '#1C1C1A' }
    },
    // 顶部留出足够间距，避免标题与 yAxis 名称/图例/图表贴紧
    grid: { left: 12, right: 16, top: 68, bottom: 8, containLabel: true },
    xAxis: { axisLine, axisLabel, splitLine: { show: false } },
    yAxis: { axisLine, axisLabel, splitLine }
  }
}

// 轻量深合并：把基础样式与模型返回的配置合并，模型配置优先于基础样式
function deepMerge(target, source) {
  const out = Array.isArray(target) ? target.slice() : { ...target }
  for (const k in source) {
    const sv = source[k]
    const tv = target ? target[k] : undefined
    if (sv && typeof sv === 'object' && !Array.isArray(sv) && tv && typeof tv === 'object' && !Array.isArray(tv)) {
      out[k] = deepMerge(tv, sv)
    } else {
      out[k] = sv
    }
  }
  return out
}

function renderCharts() {
  const root = msgRef.value
  if (!root) return
  // 清理已脱离 DOM 的旧实例，避免内存泄漏
  for (const [el, rec] of chartInstances) {
    if (!el.isConnected) {
      rec.inst.dispose()
      chartResizeObserver?.unobserve(el)
      chartInstances.delete(el)
    }
  }
  // 初始化新的图表占位
  root.querySelectorAll('.ai-chart').forEach(el => {
    if (chartInstances.has(el)) return
    const raw = el.dataset.option
    if (!raw) return
    let option
    try {
      option = JSON.parse(decodeURIComponent(raw))
    } catch {
      const ph = el.querySelector('.ai-chart-loading')
      if (ph) ph.textContent = '图表配置解析失败'
      return
    }
    // 饼图不需要坐标轴/网格，否则会出现多余的轴线和空白
    const base = getChartBaseOption()
    const isPie = option.series?.some(s => s.type === 'pie')
    if (isPie) {
      delete base.grid
      delete base.xAxis
      delete base.yAxis
      el.style.height = '260px'
    } else {
      el.style.height = ''
    }
    let merged = deepMerge(base, option)
    // 若图例没有指定位置，默认放到标题下方，避免与标题/图表贴紧
    if (merged.legend && !merged.legend.top && !merged.legend.bottom && !merged.legend.left && !merged.legend.right) {
      merged.legend.top = 44
    }
    const inst = echarts.init(el)
    inst.setOption(merged)
    inst.resize()
    chartInstances.set(el, { inst, option })
    chartResizeObserver?.observe(el)
  })
}

// 主题切换（亮/暗）时，用新的基础样式重渲染所有图表，保证配色同步
function rerenderChartsTheme() {
  chartInstances.forEach(rec => {
    const base = getChartBaseOption()
    const isPie = rec.option.series?.some(s => s.type === 'pie')
    if (isPie) {
      delete base.grid
      delete base.xAxis
      delete base.yAxis
    }
    let merged = deepMerge(base, rec.option)
    if (merged.legend && !merged.legend.top && !merged.legend.bottom && !merged.legend.left && !merged.legend.right) {
      merged.legend.top = 44
    }
    rec.inst.setOption(merged)
  })
}

function resizeCharts() {
  chartInstances.forEach(rec => rec.inst.resize())
}

let themeObserver = null

onMounted(() => {
  window.addEventListener('resize', resizeCharts)
  // 容器尺寸变化时同步图表：即使初始化时宽度不准（气泡尚未撑开），也会被自动修正
  chartResizeObserver = new ResizeObserver(entries => {
    for (const entry of entries) {
      const rec = chartInstances.get(entry.target)
      if (rec && entry.contentRect.width > 0) rec.inst.resize()
    }
  })
  // 亮/暗主题切换时，重渲染所有图表以同步配色
  themeObserver = new MutationObserver(() => rerenderChartsTheme())
  themeObserver.observe(document.documentElement, { attributes: true, attributeFilter: ['class'] })
  // 抽屉每次打开都会重新挂载（destroy-on-close）；消息异步加载与抽屉打开动画的时序不确定，
  // 多次延迟滚动确保滚到最新消息
  nextTick(() => scrollToBottom(true))
  setTimeout(() => scrollToBottom(true), 120)
  setTimeout(() => scrollToBottom(true), 380)
})
onUnmounted(() => {
  window.removeEventListener('resize', resizeCharts)
  chartResizeObserver?.disconnect()
  if (themeObserver) themeObserver.disconnect()
  chartInstances.forEach(rec => rec.inst.dispose())
  chartInstances.clear()
})

// 暴露滚动方法，供父组件在抽屉打开动画结束后调用
defineExpose({ scrollToBottom })

watch(() => props.messages, () => {
  scrollToBottom(!props.loading)
  nextTick(renderCharts)
}, { deep: true, flush: 'post', immediate: true })
watch(() => props.loading, () => { if (!props.loading) { scrollToBottom(true); nextTick(renderCharts) } })
</script>

<style scoped>
.ai-chat {
  flex: 1;
  display: flex;
  flex-direction: column;
  overflow: hidden;
  min-height: 0;
}

/* ==================== 消息列表 ==================== */
.ai-messages {
  flex: 1;
  overflow-y: auto;
  padding: 16px 20px;
  display: flex;
  flex-direction: column;
  gap: 16px;
}
.ai-empty {
  flex: 1;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  gap: 12px;
  color: var(--text-placeholder);
  font-size: 14px;
  padding: 40px 0;
}
.ai-empty__icon {
  opacity: 0.5;
}
.ai-load-more {
  display: flex;
  justify-content: center;
  padding: 4px 0 8px;
}
.ai-load-more .el-button {
  font-size: 13px;
  color: var(--text-secondary);
}
.ai-load-more .el-button:hover {
  color: var(--theme-color);
}

/* ==================== 消息气泡 ==================== */
.ai-msg {
  display: flex;
  gap: 10px;
  max-width: 100%;
}
.ai-msg--user { flex-direction: row-reverse; }
.ai-msg--assistant { flex-direction: row; }
.ai-msg__avatar {
  width: 30px;
  height: 30px;
  border-radius: 8px;
  display: flex;
  align-items: center;
  justify-content: center;
  flex-shrink: 0;
  font-size: 12px;
  font-weight: 600;
}
.ai-msg--user .ai-msg__avatar {
  background: var(--gradient-morning-mist, linear-gradient(135deg, #6BA368, #8CC488));
  color: #fff;
}
.ai-msg--assistant .ai-msg__avatar {
  background: var(--theme-color-light, rgba(107, 163, 104, 0.12));
  color: var(--theme-color);
}
.ai-msg__body {
  max-width: calc(100% - 48px);
}
.ai-msg__content {
  padding: 10px 14px;
  border-radius: 14px;
  font-size: 14px;
  line-height: 1.65;
  word-break: break-word;
}
.ai-msg--user .ai-msg__content {
  background: var(--theme-color-light, rgba(107, 163, 104, 0.1));
  color: var(--text-primary);
  border-bottom-right-radius: 4px;
}
.ai-msg--assistant .ai-msg__content {
  background: var(--bg-hover, #f5f5f5);
  color: var(--text-primary);
  border-bottom-left-radius: 4px;
}
.ai-msg__tools {
  margin-top: 6px;
  display: flex;
  flex-wrap: wrap;
  gap: 4px;
  padding-left: 4px;
}
/* 回答统计徽章（token 消耗 / 耗时），风格跟随系统主题变量 */
.ai-msg__meta {
  margin-top: 6px;
  display: flex;
  flex-wrap: wrap;
  gap: 6px;
  user-select: none;
}
.ai-msg__badge {
  display: inline-flex;
  align-items: center;
  gap: 5px;
  font-size: 12px;
  line-height: 1;
  padding: 4px 10px;
  border-radius: 12px;
  background: var(--theme-color-light, rgba(107, 163, 104, 0.08));
  color: var(--text-secondary, #8a8a8a);
  border: 1px solid var(--border-color, #e5e5e5);
}
.ai-msg__badge i {
  font-size: 12px;
  color: var(--theme-color);
}
.ai-tool-badge {
  font-size: 12px;
  padding: 2px 8px;
  border-radius: 6px;
  background: rgba(107, 163, 104, 0.1);
  color: var(--theme-color);
  border: 1px solid rgba(107, 163, 104, 0.2);
}

/* 代码块容器（亮色主题：浅色背景） */
:deep(.ai-code-block) {
  border: 1px solid #d0d7de;
  border-radius: 10px;
  overflow: hidden;
  margin: 8px 0;
  background: #f6f8fa;
}
:deep(.ai-code-head) {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 6px 10px;
  background: #eaeef2;
  border-bottom: 1px solid #d0d7de;
}
:deep(.ai-code-lang) {
  font-size: 12px;
  color: #57606a;
  font-family: 'JetBrains Mono', 'Consolas', monospace;
  text-transform: lowercase;
}
:deep(.ai-copy-btn) {
  font-size: 12px;
  color: #57606a;
  background: transparent;
  border: 1px solid #d0d7de;
  border-radius: 6px;
  padding: 2px 8px;
  cursor: pointer;
  transition: all 0.15s;
}
:deep(.ai-copy-btn:hover) {
  color: #24292e;
  border-color: #afb8c1;
}
:deep(.ai-code) {
  margin: 0;
  padding: 12px 14px;
  background: transparent;
  color: #24292e;
  font-size: 13px;
  line-height: 1.6;
  font-family: 'JetBrains Mono', 'Consolas', monospace;
  overflow-x: auto;
}
:deep(.ai-code code) {
  font-family: inherit;
  background: none;
  padding: 0;
}
:deep(.ai-inline-code) {
  background: rgba(175, 184, 193, 0.25);
  padding: 1px 5px;
  border-radius: 4px;
  font-size: 13px;
  font-family: 'JetBrains Mono', 'Consolas', monospace;
  color: #cf222e;
}

/* ECharts 图表容器 */
:deep(.ai-chart) {
  width: 100%;
  height: 320px;
  margin: 10px 0;
  border-radius: 12px;
  background: var(--bg-card);
}
:deep(.ai-chart-loading) {
  display: flex;
  align-items: center;
  justify-content: center;
  height: 100%;
  color: var(--text-secondary);
  font-size: 13px;
}
/* 含图表的消息撑满可用宽度：
   气泡默认 shrink-to-fit，图表占位出现时气泡仅有"图表渲染中…"几个字的宽度，
   echarts 此时初始化会把 canvas 宽度固化成该窄值，导致图表塌陷 */
.ai-msg__body:has(.ai-chart) {
  width: calc(100% - 48px);
}
.ai-msg__body:has(.ai-chart) .ai-msg__content {
  width: 100%;
}

/* ==================== 语法高亮配色（亮色：github 风格） ==================== */
:deep(.hljs) { color: #24292e; background: transparent; }
:deep(.hljs-comment), :deep(.hljs-quote) { color: #6a737d; font-style: italic; }
:deep(.hljs-keyword), :deep(.hljs-selector-tag), :deep(.hljs-literal),
:deep(.hljs-section), :deep(.hljs-doctag) { color: #d73a49; }
:deep(.hljs-string), :deep(.hljs-regexp), :deep(.hljs-addition),
:deep(.hljs-meta .hljs-string) { color: #032f62; }
:deep(.hljs-number), :deep(.hljs-meta) { color: #005cc5; }
:deep(.hljs-title), :deep(.hljs-title.function_), :deep(.hljs-title.class_) { color: #6f42c1; }
:deep(.hljs-attr), :deep(.hljs-attribute), :deep(.hljs-name), :deep(.hljs-type),
:deep(.hljs-built_in), :deep(.hljs-selector-id), :deep(.hljs-selector-class) { color: #e36209; }
:deep(.hljs-symbol), :deep(.hljs-bullet), :deep(.hljs-link),
:deep(.hljs-variable), :deep(.hljs-template-variable) { color: #e36209; }
:deep(.hljs-deletion) { color: #b31d28; }
:deep(.hljs-emphasis) { font-style: italic; }
:deep(.hljs-strong) { font-weight: 600; }
/* 表格 */
:deep(.ai-table-wrap) {
  overflow-x: auto;
  margin: 8px 0;
}
:deep(.ai-table) {
  border-collapse: collapse;
  width: 100%;
  font-size: 13px;
}
:deep(.ai-table th),
:deep(.ai-table td) {
  border: 1px solid var(--border-color, #e5e7eb);
  padding: 6px 10px;
}
:deep(.ai-table th) {
  background: var(--bg-hover, #f5f5f5);
  font-weight: 600;
}
/* 标题 / 链接 */
:deep(.ai-h) {
  margin: 8px 0 4px;
  font-weight: 600;
  line-height: 1.4;
}
:deep(.ai-h2) { font-size: 18px; }
:deep(.ai-h3) { font-size: 16px; }
:deep(.ai-h4) { font-size: 14px; }
:deep(.ai-link) {
  color: var(--theme-color);
  text-decoration: underline;
  word-break: break-all;
}
:deep(.ai-msg__content strong) { font-weight: 600; }
:deep(.ai-msg__content em) { font-style: italic; }

/* ==================== 加载动画 ==================== */
.ai-typing {
  display: flex;
  align-items: center;
  gap: 4px;
  padding: 10px 14px;
}
.ai-typing span {
  width: 6px;
  height: 6px;
  border-radius: 50%;
  background: var(--text-placeholder);
  animation: ai-bounce 1.2s ease-in-out infinite;
}
.ai-typing span:nth-child(2) { animation-delay: 0.15s; }
.ai-typing span:nth-child(3) { animation-delay: 0.3s; }
@keyframes ai-bounce {
  0%, 60%, 100% { transform: translateY(0); opacity: 0.4; }
  30% { transform: translateY(-6px); opacity: 1; }
}

/* ==================== 输入区（圆角聊天框卡片） ==================== */
.ai-input-card {
  flex-shrink: 0;
  background: var(--bg-secondary, #f4f6f8);
  border-radius: 18px;
  box-shadow: 0 2px 10px rgba(0, 0, 0, 0.06);
  padding: 10px 14px 8px;
  display: flex;
  flex-direction: column;
  gap: 6px;
  transition: box-shadow 0.2s;
}
.ai-input-card:focus-within {
  box-shadow: 0 0 0 2px rgba(107, 163, 104, 0.18);
}
.ai-input :deep(.el-textarea__inner) {
  padding: 2px 2px 8px;
  font-size: 14px;
  line-height: 1.5;
  resize: none;
  box-shadow: none;
  background: transparent;
  border: none;
  border-radius: 0;
  color: var(--text-primary);
}
.ai-input :deep(.el-textarea__inner::placeholder) {
  color: var(--text-placeholder);
}
.ai-input-toolbar {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 10px;
}
.ai-toolbar-left {
  display: flex;
  align-items: center;
  gap: 8px;
  flex-wrap: wrap;
}
.ai-context-ring {
  flex-shrink: 0;
  margin-right: 2px;
  cursor: help;
}
.ai-dropdown { line-height: 1; }
.ai-dropdown-link {
  display: inline-flex;
  align-items: center;
  gap: 4px;
  cursor: pointer;
  font-size: 13px;
  color: var(--text-secondary);
  padding: 3px 6px;
  border-radius: 6px;
  user-select: none;
  transition: color 0.2s, background-color 0.2s;
}
.ai-dropdown-link:hover {
  color: var(--theme-color);
  background: var(--theme-color-light);
}
.ai-dropdown-link i { font-size: 11px; }
.ai-toolbar-left :deep(.el-dropdown-menu__item.is-active) {
  color: var(--theme-color);
  font-weight: 600;
}
.ai-send-btn {
  flex-shrink: 0;
  width: 34px;
  height: 34px;
}

/* ==================== 暗色模式 ==================== */
html.dark .ai-msg--assistant .ai-msg__content {
  background: rgba(140, 196, 136, 0.06);
}
html.dark .ai-msg--user .ai-msg__content {
  background: rgba(140, 196, 136, 0.14);
}
html.dark .ai-input-card {
  background: rgba(255, 255, 255, 0.05);
}

/* 代码块（暗色主题：深色背景） */
html.dark :deep(.ai-code-block) {
  background: #0d1117;
  border-color: rgba(255, 255, 255, 0.12);
}
html.dark :deep(.ai-code-head) {
  background: #161b22;
  border-bottom-color: rgba(255, 255, 255, 0.08);
}
html.dark :deep(.ai-code-lang) {
  color: #8b949e;
}
html.dark :deep(.ai-copy-btn) {
  color: #8b949e;
  border-color: rgba(255, 255, 255, 0.15);
}
html.dark :deep(.ai-copy-btn:hover) {
  color: #fff;
  border-color: rgba(255, 255, 255, 0.35);
}
html.dark :deep(.ai-code) {
  color: #c9d1d9;
}
html.dark :deep(.ai-inline-code) {
  background: rgba(255, 255, 255, 0.08);
  color: #ffa198;
}

/* 语法高亮配色（暗色：github-dark 风格） */
html.dark :deep(.hljs) { color: #c9d1d9; }
html.dark :deep(.hljs-comment), html.dark :deep(.hljs-quote) { color: #8b949e; }
html.dark :deep(.hljs-keyword), html.dark :deep(.hljs-selector-tag), html.dark :deep(.hljs-literal),
html.dark :deep(.hljs-section), html.dark :deep(.hljs-doctag) { color: #ff7b72; }
html.dark :deep(.hljs-string), html.dark :deep(.hljs-regexp), html.dark :deep(.hljs-addition),
html.dark :deep(.hljs-meta .hljs-string) { color: #a5d6ff; }
html.dark :deep(.hljs-number), html.dark :deep(.hljs-meta) { color: #79c0ff; }
html.dark :deep(.hljs-title), html.dark :deep(.hljs-title.function_), html.dark :deep(.hljs-title.class_) { color: #d2a8ff; }
html.dark :deep(.hljs-attr), html.dark :deep(.hljs-attribute), html.dark :deep(.hljs-name), html.dark :deep(.hljs-type),
html.dark :deep(.hljs-built_in), html.dark :deep(.hljs-selector-id), html.dark :deep(.hljs-selector-class) { color: #79c0ff; }
html.dark :deep(.hljs-symbol), html.dark :deep(.hljs-bullet), html.dark :deep(.hljs-link),
html.dark :deep(.hljs-variable), html.dark :deep(.hljs-template-variable) { color: #ffa657; }
html.dark :deep(.hljs-deletion) { color: #ffa198; }
</style>
