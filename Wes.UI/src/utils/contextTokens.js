/**
 * 上下文 Token 估算（与后端 Wes.AI/Context/ContextCompressor.cs 保持同一套启发式）：
 * CJK/全角按 1 token/字，其余按 4 字符/token。仅用于展示与预算判断，不要求精确。
 */

/** 系统提示与工具定义的粗略开销（后端会返回真实用量，前端先用它做初始值） */
export const SYSTEM_TOKENS = 1500

/** 模型未配置上下文窗口时的兜底值（与后端 ContextCompressor.DefaultMaxContext 一致） */
export const DEFAULT_MAX_CONTEXT = 128000

export function estimateTokens(text) {
  if (!text) return 0
  let ascii = 0
  let cjk = 0
  for (const ch of text) {
    // 0x2E80 之后基本都是 CJK/全角字符
    if (ch.codePointAt(0) >= 0x2e80) cjk++
    else ascii++
  }
  return cjk + Math.ceil(ascii / 4)
}

export function estimateMessagesTokens(messages) {
  return (messages || []).reduce((sum, m) => sum + estimateTokens(m && m.content) + 4, 0)
}

/** 12345 -> 12.3k；999 -> 999 */
export function formatTokens(n) {
  const v = Number(n) || 0
  if (v >= 10000) return Math.round(v / 1000) + 'k'
  if (v >= 1000) return (v / 1000).toFixed(1) + 'k'
  return String(v)
}
