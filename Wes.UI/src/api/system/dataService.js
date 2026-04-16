import request from '@/utils/request'

// 查询数据服务列表
export function listDataService(query) {
  return request({
    url: '/system/dataService/list',
    method: 'get',
    params: query
  })
}

// 查询数据服务详细
export function getDataService(serviceId) {
  return request({
    url: '/system/dataService/' + serviceId,
    method: 'get'
  })
}

// 新增数据服务
export function addDataService(data) {
  return request({
    url: '/system/dataService',
    method: 'post',
    data: data
  })
}

// 修改数据服务
export function updateDataService(data) {
  return request({
    url: '/system/dataService',
    method: 'put',
    data: data
  })
}

// 删除数据服务
export function delDataService(serviceId) {
  return request({
    url: '/system/dataService/' + serviceId,
    method: 'delete'
  })
}