import request from '@/utils/request'

// 查询子系统列表
export function listSubSystem(query) {
  return request({
    url: '/system/subsystem/list',
    method: 'get',
    params: query
  })
}

// 查询子系统详细
export function getSubSystem(systemId) {
  return request({
    url: '/system/subsystem/' + systemId,
    method: 'get'
  })
}

// 新增子系统
export function addSubSystem(data) {
  return request({
    url: '/system/subsystem',
    method: 'post',
    data: data
  })
}

// 修改子系统
export function updateSubSystem(data) {
  return request({
    url: '/system/subsystem',
    method: 'put',
    data: data
  })
}

// 删除子系统
export function delSubSystem(systemId) {
  return request({
    url: '/system/subsystem/' + systemId,
    method: 'delete'
  })
}