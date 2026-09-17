import request from '@/utils/request'

// 查询任务列表
export function listJob(query) {
  return request({
    url: '/scheduler/job/list',
    method: 'get',
    params: query
  })
}

// 查询任务详情
export function getJob(jobId) {
  return request({
    url: '/scheduler/job/' + jobId,
    method: 'get'
  })
}

// 新增任务
export function addJob(data) {
  return request({
    url: '/scheduler/job',
    method: 'post',
    data: data
  })
}

// 修改任务
export function updateJob(data) {
  return request({
    url: '/scheduler/job',
    method: 'put',
    data: data
  })
}

// 删除任务
export function delJob(jobIds) {
  return request({
    url: '/scheduler/job/' + jobIds,
    method: 'delete'
  })
}

// 修改任务状态
export function changeJobStatus(data) {
  return request({
    url: '/scheduler/job/changeStatus',
    method: 'put',
    data: data
  })
}

// 执行一次
export function runJob(jobId) {
  return request({
    url: '/scheduler/job/run/' + jobId,
    method: 'post'
  })
}

// 查询任务日志列表
export function listJobLog(query) {
  return request({
    url: '/scheduler/job/log/list',
    method: 'get',
    params: query
  })
}

// 查询任务日志详情
export function getJobLog(jobLogId) {
  return request({
    url: '/scheduler/job/log/' + jobLogId,
    method: 'get'
  })
}

// 删除任务日志
export function delJobLog(jobLogIds) {
  return request({
    url: '/scheduler/job/log/' + jobLogIds,
    method: 'delete'
  })
}

// 清空任务日志
export function cleanJobLog() {
  return request({
    url: '/scheduler/job/log/clean',
    method: 'delete'
  })
}
