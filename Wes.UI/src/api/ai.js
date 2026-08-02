import request from '@/utils/request'

// ==================== Agent（聊天即 chat 类型，无独立纯对话路由） ====================

/** 非流式 Agent 对话 */
export function agentChat(type, data) {
  return request({ url: `/ai/agent/${type}`, method: 'post', data })
}

/** 获取提供商列表 */
export function getProviders() {
  return request({ url: '/ai/agent/providers', method: 'get' })
}

/** 获取模型列表 */
export function getModels(provider) {
  return request({ url: '/ai/agent/models', method: 'get', params: { provider } })
}

// ==================== SSE 流式 ====================

/**
 * 创建 SSE 流式请求（返回 abort 函数）
 * @param {string} url - 请求路径
 * @param {object} data - 请求 body
 * @param {object} callbacks - { onChunk, onDone, onError }
 * @returns {function} abort 函数，调用可中断请求
 */
export function sseRequest(url, data, callbacks) {
  const token = localStorage.getItem('accessToken') || ''
  const controller = new AbortController()

  fetch(`/api${url}`, {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${token}`
    },
    body: JSON.stringify(data),
    signal: controller.signal
  }).then(async response => {
    if (!response.ok) {
      callbacks.onError?.(new Error(`HTTP ${response.status}`))
      return
    }
    const reader = response.body.getReader()
    const decoder = new TextDecoder()
    let buffer = ''
    while (true) {
      const { done, value } = await reader.read()
      if (done) break
      buffer += decoder.decode(value, { stream: true })
      const lines = buffer.split('\n')
      buffer = lines.pop() || ''
      for (const line of lines) {
        if (line.startsWith('data: ')) {
          const payload = line.slice(6).trim()
          if (payload === '[DONE]') {
            callbacks.onDone?.()
            return
          }
          try {
            const chunk = JSON.parse(payload)
            callbacks.onChunk?.(chunk)
          } catch { /* 忽略解析错误 */ }
        }
      }
    }
    callbacks.onDone?.()
  }).catch(err => {
    if (err.name === 'AbortError') {
      callbacks.onDone?.()
    } else {
      callbacks.onError?.(err)
    }
  })

  return () => controller.abort()
}

/** 流式 Agent 对话（含 chat 类型），返回 abort 函数 */
export function agentChatStream(type, data, callbacks) {
  return sseRequest(`/ai/agent/${type}/stream`, data, callbacks)
}

// ==================== 会话持久化 ====================

/** 当前用户会话列表 */
export function listSessions() {
  return request({ url: '/ai/session', method: 'get' })
}

/** 创建会话 */
export function createSession(data) {
  return request({ url: '/ai/session', method: 'post', data })
}

/** 获取会话详情（含消息） */
export function getSession(id) {
  return request({ url: `/ai/session/${id}`, method: 'get' })
}

/** 更新会话（标题/场景/模型） */
export function updateSession(id, data) {
  return request({ url: `/ai/session/${id}`, method: 'put', data })
}

/** 删除会话 */
export function deleteSession(id) {
  return request({ url: `/ai/session/${id}`, method: 'delete' })
}

/** 全量保存会话消息 */
export function saveSessionMessages(id, messages) {
  return request({ url: `/ai/session/${id}/messages`, method: 'put', data: { messages } })
}
