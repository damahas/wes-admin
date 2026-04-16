import { createStore } from 'vuex'
import system from './modules/system'
import user from './modules/user'
import dict from './modules/dict'

export default createStore({
  modules: {
    system,
    user,
    dict
  }
})
