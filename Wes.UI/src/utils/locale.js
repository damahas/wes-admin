const LOCALE_KEY = 'locale'

const SUPPORTED = ['zh-CN', 'en-US']

/**
 * 获取初始语言：
 * 1. 优先读取 localStorage 缓存
 * 2. 其次取浏览器语言（中文 → zh-CN，其余 → en-US）
 */
export const getInitialLocale = () => {
  const cached = localStorage.getItem(LOCALE_KEY)
  if (cached && SUPPORTED.includes(cached)) return cached

  const browser = navigator.language || ''
  return browser.toLowerCase().startsWith('zh') ? 'zh-CN' : 'en-US'
}

export { LOCALE_KEY }
