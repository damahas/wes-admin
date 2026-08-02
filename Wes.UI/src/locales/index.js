import { createI18n } from 'vue-i18n'
import { getInitialLocale } from '../utils/locale'
import system from './zh/system'
import flow from './zh/flow'
import ai from './zh/ai'
import home from './zh/home'
import systemEn from './en/system'
import flowEn from './en/flow'
import aiEn from './en/ai'
import homeEn from './en/home'

const messages = {
  'zh-CN': {
    ...system,
    ...flow,
    ...ai,
    ...home
  },
  'en-US': {
    ...systemEn,
    ...flowEn,
    ...aiEn,
    ...homeEn
  }
}

const i18n = createI18n({
  legacy: false,
  locale: getInitialLocale(),
  fallbackLocale: 'en-US',
  messages
})

export default i18n
