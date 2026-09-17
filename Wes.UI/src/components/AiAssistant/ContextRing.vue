<template>
  <el-tooltip placement="top" :content="tip" effect="dark">
    <span class="ctx-ring" :class="`ctx-ring--${level}`" :title="tip">
      <svg :width="size" :height="size" viewBox="0 0 24 24">
        <circle
          class="ctx-ring__bg"
          cx="12" cy="12" :r="radius" :stroke-width="stroke" fill="none"
        />
        <circle
          class="ctx-ring__fg"
          cx="12" cy="12" :r="radius" :stroke-width="stroke" fill="none"
          :stroke-dasharray="circumference"
          :stroke-dashoffset="offset"
          stroke-linecap="round"
          transform="rotate(-90 12 12)"
        />
      </svg>
      <em v-if="compressed" class="ctx-ring__dot" />
    </span>
  </el-tooltip>
</template>

<script setup>
import { computed } from 'vue'
import { formatTokens } from '@/utils/contextTokens'

/**
 * 上下文占用圆环：展示已用 token / 模型最大上下文。
 * 占用 <60% 绿色，60%~85% 橙色，>85% 红色；发生过压缩时右下角显示一个小圆点。
 */
const props = defineProps({
  used: { type: Number, default: 0 },
  max: { type: Number, default: 0 },
  compressed: { type: Boolean, default: false },
  size: { type: Number, default: 16 }
})

const stroke = 3
const radius = computed(() => 12 - stroke / 2 - 0.5)
const circumference = computed(() => 2 * Math.PI * radius.value)

const ratio = computed(() => {
  const m = Number(props.max) || 0
  if (m <= 0) return 0
  return Math.min(1, Math.max(0, (Number(props.used) || 0) / m))
})

const offset = computed(() => circumference.value * (1 - ratio.value))

const level = computed(() => {
  const r = ratio.value
  if (r >= 0.85) return 'danger'
  if (r >= 0.6) return 'warn'
  return 'ok'
})

const tip = computed(() => {
  const pct = Math.round(ratio.value * 100)
  const base = `${formatTokens(props.used)} / ${formatTokens(props.max)} (${pct}%)`
  return props.compressed ? `${base} · 已压缩` : base
})
</script>

<style scoped>
.ctx-ring {
  position: relative;
  display: inline-flex;
  align-items: center;
  justify-content: center;
  vertical-align: middle;
}
.ctx-ring__bg {
  stroke: var(--border-color, #e5e5e5);
}
.ctx-ring--ok .ctx-ring__fg {
  stroke: var(--theme-color, #6ba368);
}
.ctx-ring--warn .ctx-ring__fg {
  stroke: #e6a23c;
}
.ctx-ring--danger .ctx-ring__fg {
  stroke: #f56c6c;
}
.ctx-ring__dot {
  position: absolute;
  right: -1px;
  bottom: -1px;
  width: 5px;
  height: 5px;
  border-radius: 50%;
  background: #e6a23c;
  border: 1px solid var(--bg-color, #fff);
}
</style>
