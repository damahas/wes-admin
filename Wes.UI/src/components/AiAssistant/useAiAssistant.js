import { ref, computed } from 'vue'
import { useI18n } from 'vue-i18n'
import { ElMessage } from 'element-plus'
import {
  getProviders,
  getModels,
  agentChatStream,
  listSessions,
  createSession,
  getSession,
  updateSession,
  deleteSession,
  saveSessionMessages
} from '@/api/ai'
import { estimateMessagesTokens, estimateTokens, DEFAULT_MAX_CONTEXT } from '@/utils/contextTokens'

/**
 * AI 助手核心业务逻辑（与 UI 解耦）。
 * 负责：配置加载、会话列表、消息读写、流式发送、持久化、上下文用量。
 * 视图切换（chat / history）交由容器组件控制。
 */
export function useAiAssistant() {
  const { t } = useI18n()

  const loading = ref(false)
  const sessions = ref([])          // [{ id, title, agentType, provider }]
  const activeSessionId = ref(null)
  const messages = ref([])
  const providers = ref([])
  const models = ref([])            // 全部模型列表（跨提供商，来自数据库 ai_model 表）
  const messageCache = {}            // sessionId -> messages[]，避免重复拉取
  const contextUsage = ref(null)     // 后端返回的最近一次上下文用量 { usedTokens, maxContext, compressed, ... }
  let compressedNotified = false     // 当前会话是否已提示过「上下文已压缩」
  let abort = null

  const activeSession = computed(
    () => sessions.value.find(s => s.id === activeSessionId.value) || null
  )

  // 某提供商的默认模型（来自 providers 列表的 defaultModel）
  function defaultModelFor(prov) {
    return providers.value.find(p => p.name === prov)?.defaultModel || ''
  }

  // 当前模型的上下文窗口（来自 ai_model.max_context，未配置时用兜底值）
  const maxContext = computed(() => {
    const m = models.value.find(x => x.modelId === model.value)
    return m?.maxContext || DEFAULT_MAX_CONTEXT
  })

  // 未拿到后端用量前的本地估算：历史 + 系统提示开销
  const localTokens = computed(() => estimateMessagesTokens(messages.value) + 1500)

  // 助手 / 模型按当前会话存储，修改后即时落库
  const agentType = computed({
    get: () => activeSession.value?.agentType || 'auto',
    set: v => { if (activeSession.value) { activeSession.value.agentType = v; persistSession() } }
  })
  const provider = computed({
    get: () => activeSession.value?.provider || '',
    set: v => { if (activeSession.value) { activeSession.value.provider = v; persistSession() } }
  })
  // 选中模型：优先用会话已保存的，否则回退到该提供商的默认模型（均来自数据库）
  // 模型隐含所属提供商，选择模型时同步 provider
  const model = computed({
    get: () => {
      const s = activeSession.value
      if (s?.model) return s.model
      return defaultModelFor(s?.provider || '')
    },
    set: v => {
      if (!activeSession.value) return
      activeSession.value.model = v
      const m = models.value.find(x => x.modelId === v)
      if (m) activeSession.value.provider = m.provider
      persistSession()
    }
  })

  // ==================== 配置 ====================
  async function loadConfig() {
    try {
      const pr = await getProviders()
      if (pr.code === 200 && pr.data) providers.value = pr.data
      // 预拉取全部模型（跨提供商），用于统一选择器
      await loadModels()
    } catch { /* 静默失败 */ }
  }

  // 从数据库（ai_model 表）拉取全部模型（跨提供商），用于统一选择器
  async function loadModels() {
    try {
      const res = await getModels()
      if (res.code === 200 && res.data) models.value = res.data
    } catch { /* 静默失败 */ }
  }

  // ==================== 会话初始化 ====================
  async function ensureSessions() {
    try {
      const res = await listSessions()
      if (res.code === 200 && res.data) {
        sessions.value = res.data.map(s => ({
          id: s.id, title: s.title, agentType: s.agentType, provider: s.provider, model: s.model,
          updatedAt: s.updatedAt ?? s.UpdatedAt
        }))
      }
    } catch { /* 静默失败 */ }

    if (sessions.value.length === 0) {
      const defProvider = providers.value.find(p => p.isDefault)?.name || providers.value[0]?.name || ''
      const defModel = defaultModelFor(defProvider)
      await createNewSession('新会话', 'auto', defProvider, defModel, false)
    }
    if (sessions.value.length > 0) {
      activeSessionId.value = sessions.value[0].id
      await loadMessages(activeSessionId.value)
    }
  }

  // ==================== 会话持久化 ====================
  async function persistSession() {
    if (!activeSession.value) return
    try {
      await updateSession(activeSession.value.id, {
        agentType: activeSession.value.agentType,
        provider: activeSession.value.provider,
        model: activeSession.value.model
      })
    } catch { /* 静默失败 */ }
  }

  async function createNewSession(title, type, prov, model, switchTo = true) {
    try {
      const res = await createSession({ title, agentType: type, provider: prov, model })
      if (res.code === 200 && res.data) {
        const s = res.data
        sessions.value.push({ id: s.id, title: s.title, agentType: s.agentType, provider: s.provider, model: s.model, updatedAt: s.updatedAt ?? s.UpdatedAt })
        if (switchTo) { activeSessionId.value = s.id; setActiveMessages([]) }
        return s
      }
    } catch { /* 静默失败 */ }
    return null
  }

  async function onNewSession() {
    const defProvider = activeSession.value?.provider
      || providers.value.find(p => p.isDefault)?.name
      || providers.value[0]?.name || ''
    const defModel = defaultModelFor(defProvider)
    await createNewSession('新会话', 'auto', defProvider, defModel, true)
  }

  /** 设置当前会话消息，并写入缓存（同一引用，发送时自动同步） */
  function setActiveMessages(arr) {
    messages.value = arr
    contextUsage.value = null
    compressedNotified = false
    if (activeSessionId.value != null) messageCache[activeSessionId.value] = arr
  }

  async function loadMessages(id) {
    // 已缓存则直接命中，避免每次打开会话都调接口
    if (messageCache[id]) {
      setActiveMessages(messageCache[id])
      return
    }
    try {
      const res = await getSession(id)
      const arr = res.code === 200 && res.data
        ? (res.data.messages || []).map(m => ({
            role: m.role,
            content: m.content,
            toolCalls: m.toolCalls || [],
            // 后端已按同一规则补算；为空时本地再兜一次
            tokens: m.tokens ?? estimateTokens(m.content),
            // 历史消息可能没有耗时记录（旧数据或后端未存），为空则不展示
            elapsedMs: m.elapsedMs ?? null,
            // 真实 token（模型返回的入/出）；为空表示模型未返回 usage，展示为估算值
            promptTokens: m.promptTokens ?? null,
            completionTokens: m.completionTokens ?? null,
            tokensEstimated: m.tokensEstimated ?? null,
            // token 来源：model=模型直返 / probe=输入实测+输出估算 / estimate=全部估算
            usageSource: m.usageSource ?? null
          }))
        : []
      setActiveMessages(arr)
    } catch {
      setActiveMessages([])
    }
  }

  async function switchSession(id) {
    if (id === activeSessionId.value) return
    activeSessionId.value = id
    await loadMessages(id)
  }

  async function onSelectSession(id) {
    await switchSession(id)
  }

  async function onDeleteSession(id) {
    if (sessions.value.length <= 1) {
      ElMessage.warning(t('ai.cannotDeleteLast'))
      return
    }
    try {
      const res = await deleteSession(id)
      if (res.code !== 200) {
        ElMessage.error(t('ai.deleteFailed'))
        return
      }
    } catch {
      ElMessage.error(t('ai.deleteFailed'))
      return
    }
    sessions.value = sessions.value.filter(s => s.id !== id)
    delete messageCache[id]
    if (activeSessionId.value === id) {
      activeSessionId.value = sessions.value[0].id
      await loadMessages(activeSessionId.value)
    }
  }

  async function saveMessages() {
    if (!activeSession.value) return
    // 耗时由后端 Stopwatch 实测并在保存时自行落库，前端不回传该字段
    const payload = messages.value.map(m => ({
      role: m.role,
      content: m.content,
      toolCalls: m.toolCalls || [],
      tokens: m.tokens ?? estimateTokens(m.content)
    }))
    try {
      await saveSessionMessages(activeSession.value.id, payload)
    } catch { /* 静默失败 */ }
  }

  // ==================== 发送（统一走 Agent 框架） ====================
  async function onSend(text) {
    if (!text || loading.value) return
    if (!provider.value) {
      ElMessage.warning(t('ai.noProvider'))
      return
    }
    if (!activeSession.value) {
      await onNewSession()
    }
    const sid = activeSessionId.value

    messages.value.push({ role: 'user', content: text, tokens: estimateTokens(text) })
    if (activeSession.value && activeSession.value.title === '新会话') {
      activeSession.value.title = text.slice(0, 20)
      try { await updateSession(sid, { title: activeSession.value.title }) } catch { /* 忽略 */ }
    }
    loading.value = true

    let content = ''
    const idx = messages.value.length
    // 本地计时兜底：后端返回 elapsedMs 时以服务端实测为准；
    // 未返回（旧版本后端 / 未重新编译）时用本地时间差补算，保证耗时徽章始终可见
    const startAt = Date.now()
    let serverElapsed = false
    // 服务端是否给出过 token（model 直返 或 probe 实测），有则不回退本地估算
    let tokensFromServer = false
    messages.value.push({ role: 'assistant', content: '', toolCalls: [] })

    abort = agentChatStream(
      agentType.value,
      {
        provider: provider.value,
        model: model.value,
        message: text,
        // 带上会话 ID：服务端据此把实测耗时关联到本会话，保存消息时自动落库
        sessionId: sid,
        history: messages.value.slice(0, -1).map(m => ({ role: m.role, content: m.content }))
      },
      {
        onChunk(chunk) {
          if (chunk.error) {
            content += chunk.error
            messages.value[idx].content = content
            return
          }
          if (chunk.content) {
            content += chunk.content
            messages.value[idx].content = content
          }
          if (chunk.toolCalls) {
            messages.value[idx].toolCalls = chunk.toolCalls
          }
          if (chunk.usage) {
            contextUsage.value = chunk.usage
            if (chunk.usage.compressed && !compressedNotified) {
              compressedNotified = true
              ElMessage.info(t('ai.contextCompressed'))
            }
          }
          // 服务端实测耗时（最后一个块携带），仅用于展示，保存由后端落库
          if (chunk.elapsedMs > 0) {
            messages.value[idx].elapsedMs = chunk.elapsedMs
            serverElapsed = true
          }
          // 模型 token（最后一个块携带）：真实值 / 探测实测值优先，未返回（estimate）时回退本地估算
          if (chunk.promptTokens != null || chunk.completionTokens != null || chunk.tokensEstimated) {
            const p = chunk.promptTokens ?? 0
            const c = chunk.completionTokens ?? 0
            messages.value[idx].promptTokens = chunk.promptTokens ?? null
            messages.value[idx].completionTokens = chunk.completionTokens ?? null
            messages.value[idx].tokensEstimated = chunk.tokensEstimated === true
            messages.value[idx].usageSource = chunk.usageSource ?? null
            if (p + c > 0) {
              messages.value[idx].tokens = p + c
              tokensFromServer = true
            }
          }
        },
        async onDone() {
          if (!serverElapsed) messages.value[idx].elapsedMs = Date.now() - startAt
          // 服务端没给 token（estimate 来源）才用本地估算，避免覆盖真实/实测值
          if (!tokensFromServer) messages.value[idx].tokens = estimateTokens(content)
          loading.value = false
          await saveMessages()
        },
        async onError() {
          if (!serverElapsed) messages.value[idx].elapsedMs = Date.now() - startAt
          loading.value = false
          messages.value[idx].content = content || t('ai.networkError')
          messages.value[idx].tokens = estimateTokens(messages.value[idx].content)
          await saveMessages()
        }
      }
    )
  }

  function onStop() {
    abort?.()
    loading.value = false
  }

  let initialized = false
  // 幂等初始化：会话列表只在首次加载，之后复用缓存，避免重复查询
  async function init() {
    if (initialized) return
    initialized = true
    await loadConfig()
    await ensureSessions()
  }

  return {
    // 状态
    loading, sessions, activeSessionId, messages,
    providers, models,
    activeSession, agentType, provider, model,
    contextUsage, maxContext, localTokens,
    // 行为
    init,
    onNewSession, onSelectSession, onDeleteSession,
    onSend, onStop
  }
}
