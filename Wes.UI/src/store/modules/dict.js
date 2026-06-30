export default {
  namespaced: true,
  state: {
    // 字典数据存储，key: dictType, value: dictArray
    dictData: {}
  },
  getters: {
    // 获取指定类型的字典
    getDict: state => dictType => {
      return state.dictData[dictType] || []
    },
    // 获取所有字典
    getAllDict: state => state.dictData
  },
  mutations: {
    // 设置字典（支持单个或批量）
    SET_DICT(state, payload) {
      if (Array.isArray(payload)) {
        // 批量设置
        payload.forEach(item => {
          state.dictData[item.dictType] = item.dictData
        })
      } else {
        // 单个设置
        state.dictData[payload.dictType] = payload.dictData
      }
    },
    // 删除字典
    DELETE_DICT(state, dictType) {
      delete state.dictData[dictType]
    },
    // 清空所有字典
    CLEAR_DICT(state) {
      state.dictData = {}
    }
  },
  actions: {
    // 设置单个字典
    setDict({ commit }, dictType, dictData) {
      commit('SET_DICT', { dictType, dictData })
    },
    // 批量设置字典
    setDictBatch({ commit }, dicts) {
      commit('SET_DICT', dicts)
    },
    // 删除字典
    deleteDict({ commit }, dictType) {
      commit('DELETE_DICT', dictType)
    },
    // 清空所有字典
    clearDict({ commit }) {
      commit('CLEAR_DICT')
    }
  }
}
