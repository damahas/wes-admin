import request from '@/utils/request'

// 查询登录日志列表
export function loginList(query) {
  return request({
    url: '/system/log/login/list',
    method: 'get',
    params: query
  })
}

// 查询操作日志列表
export function operList(query) {
  return request({
    url: '/system/log/oper/list',
    method: 'get',
    params: query
  })
}