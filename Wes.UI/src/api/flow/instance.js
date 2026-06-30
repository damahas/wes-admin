import request from '@/utils/request'

// 获取审批流实例列表
export function listInstance(query) {
  return request({
    url: '/flow/instance/list',
    method: 'get',
    params: query
  })
}

// 获取审批流实例
export function getInstance(id) {
  return request({
    url: '/flow/instance/' + id,
    method: 'get'
  })
}

// 新增审批流实例
export function addInstance(data) {
  return request({
    url: '/flow/instance',
    method: 'post',
    data: data
  })
}

// 更新审批流实例
export function updateInstance(data) {
  return request({
    url: '/flow/instance',
    method: 'put',
    data: data
  })
}

// 删除审批流实例
export function delInstance(ids) {
  return request({
    url: '/flow/instance/' + ids,
    method: 'delete',
  })
}

// 删除审批流实例
export function delegateTask(taskId, userId) {
  return request({
    url: `/flow/instance/delegate/${taskId}/${userId}`,
    method: 'get',
  })
}

// 删除审批流实例
export function resetNode(nodeId) {
  return request({
    url: `/flow/instance/reset/node/${nodeId}`,
    method: 'get',
  })
}