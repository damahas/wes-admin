import request from '@/utils/request'

// 查询在线用户列表
export function listOnline(query) {
  return request({
    url: '/system/online/list',
    method: 'get',
    params: query
  })
}

// 强退在线用户
export function delOnline(tokenId) {
  return request({
    url: '/system/online/' + tokenId,
    method: 'delete'
  })
}
