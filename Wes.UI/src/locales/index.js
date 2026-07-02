import { createI18n } from 'vue-i18n'
import { getInitialLocale } from '../utils/locale'
import system from './zh/system'
import flow from './zh/flow'
import systemEn from './en/system'
import flowEn from './en/flow'

const messages = {
  'zh-CN': {
    ...system,
    ...flow
  },
  'en-US': {
    ...systemEn,
    ...flowEn
  }
}

const i18n = createI18n({
  legacy: false,
  locale: getInitialLocale(),
  fallbackLocale: 'en-US',
  messages
})

export default i18n
