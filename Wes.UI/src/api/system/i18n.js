import request from '@/utils/request'

/**
 * 国际化翻译管理
 */

// 查询翻译列表
export function listI18n(query) {
  return request({
    url: '/system/i18n/list',
    method: 'get',
    params: query
  })
}

// 查询翻译详细
export function getI18n(id) {
  return request({
    url: '/system/i18n/' + id,
    method: 'get'
  })
}

// 新增翻译
export function addI18n(data) {
  return request({
    url: '/system/i18n',
    method: 'post',
    data: data
  })
}

// 修改翻译
export function updateI18n(data) {
  return request({
    url: '/system/i18n',
    method: 'put',
    data: data
  })
}

// 删除翻译
export function delI18n(ids) {
  return request({
    url: '/system/i18n/' + ids,
    method: 'delete'
  })
}

// 刷新国际化缓存
export function refreshI18nCache() {
  return request({
    url: '/system/i18n/refresh',
    method: 'post'
  })
}

// 获取前端翻译数据
export function getFrontendTranslations(lang) {
  return request({
    url: '/system/i18n/frontend',
    method: 'get',
    params: { lang }
  })
}
