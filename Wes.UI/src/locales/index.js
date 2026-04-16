import { createI18n } from 'vue-i18n'
import system from './zh/system'
import systemEn from './en/system'

const messages = {
  'zh-CN': {
    ...system
  },
  'en-US': {
    ...systemEn
  }
}

const i18n = createI18n({
  legacy: false,
  locale: localStorage.getItem('locale') || 'en-US',
  fallbackLocale: 'en-US',
  messages
})

export default i18n
