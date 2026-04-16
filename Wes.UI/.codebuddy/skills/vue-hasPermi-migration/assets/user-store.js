import { defineStore } from 'pinia'

const useUserStore = defineStore(
  'user',
  {
    state: () => ({
      token: '',
      id: '',
      name: '',
      avatar: '',
      roles: [],
      permissions: [] // 存储用户权限列表，如 ['system:user:add', 'system:user:edit']
    }),
    actions: {
      // 登录
      login(userInfo) {
        // 根据实际后端接口实现登录逻辑
        return new Promise((resolve, reject) => {
          // 登录成功后获取token并保存
          // 同时调用getInfo获取用户信息和权限
          resolve()
        })
      },
      // 获取用户信息
      getInfo() {
        return new Promise((resolve, reject) => {
          // 调用后端接口获取用户信息和权限列表
          // 将权限赋值给 this.permissions
          resolve()
        })
      },
      // 退出系统
      logOut() {
        return new Promise((resolve, reject) => {
          // 清除用户信息和权限
          this.permissions = []
          resolve()
        })
      }
    }
  })

export default useUserStore
