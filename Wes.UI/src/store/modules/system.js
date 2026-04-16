const THEME_KEY = 'theme'
const LOCALE_KEY = 'locale'

const getInitialTheme = () => {
  const saved = localStorage.getItem(THEME_KEY)
  if (saved === 'dark') {
    document.documentElement.classList.add('dark')
    return true
  }
  return false
}

export default {
  namespaced: true,
  state: {
    isDark: getInitialTheme(),
    locale: localStorage.getItem(LOCALE_KEY) || 'zh-CN',
    // 支持的语言列表
    langList: [
      { langCode: 'zh-CN', langName: '简体中文' },
      { langCode: 'en-US', langName: 'English' }
    ]
  },
  getters: {
    isDark: state => state.isDark,
    locale: state => state.locale,
    langList: state => state.langList
  },
  mutations: {
    TOGGLE_DARK(state) {
      state.isDark = !state.isDark
      localStorage.setItem(THEME_KEY, state.isDark ? 'dark' : 'light')
      if (state.isDark) {
        document.documentElement.classList.add('dark')
      } else {
        document.documentElement.classList.remove('dark')
      }
    },
    SET_DARK(state, isDark) {
      state.isDark = isDark
      localStorage.setItem(THEME_KEY, isDark ? 'dark' : 'light')
      if (isDark) {
        document.documentElement.classList.add('dark')
      } else {
        document.documentElement.classList.remove('dark')
      }
    },
    SET_LOCALE(state, locale) {
      state.locale = locale
      localStorage.setItem(LOCALE_KEY, locale)
    },
    SET_LANG_LIST(state, languages) {
      state.langList = languages
    }
  },
  actions: {
    toggleDark({ commit }) {
      commit('TOGGLE_DARK')
    },
    setDark({ commit }, isDark) {
      commit('SET_DARK', isDark)
    },
    setLocale({ commit }, locale) {
      commit('SET_LOCALE', locale)
    },
    setLangList({ commit }, languages) {
      commit('SET_LANG_LIST', languages)
    }
  }
}
