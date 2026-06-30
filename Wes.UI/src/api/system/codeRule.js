import request from '@/utils/request'

// 获取序号生成规则列表
export function listSysCodeRule(query) {
  return request({
    url: '/system/coderule/list',
    method: 'get',
    params: query
  })
}

// 获取所有序号生成规则
export function getAllCodeRule() {
    return request({
      url: '/system/coderule/all',
     method: 'get'
    })
  }


// 获取序号生成规则
export function getSysCodeRule(id) {
  return request({
    url: '/system/coderule/' + id,
   method: 'get'
  })
}

// 新增序号生成规则
export function addSysCodeRule(data) {
  return request({
    url: '/system/coderule',
    method: 'post',
    data: data
  })
}

// 更新序号生成规则
export function updateSysCodeRule(data) {
  return request({
    url: '/system/coderule',
    method: 'put',
    data: data
  })
}

// 删除序号生成规则
export function delSysCodeRule(ids) {
  return request({
    url: '/system/code_rule/' + ids,
    method: 'delete',
  })
}

