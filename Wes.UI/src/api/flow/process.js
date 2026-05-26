import request from "@/utils/request";

// 获取审批流列表
export function listProcess(query) {
  return request({
    url: "/flow/process/list",
    method: "get",
    params: query,
  });
}

// 获取所有审批流列表
export function allProcess() {
  return request({
    url: "/flow/process/all",
    method: "get",
  });
}

// 获取审批流
export function getProcess(id) {
  return request({
    url: "/flow/process/" + id,
    method: "get",
  });
}

// 新增审批流
export function addProcess(data) {
  return request({
    url: "/flow/process",
    method: "post",
    data: data,
  });
}

// 更新审批流
export function updateProcess(data) {
  return request({
    url: "/flow/process",
    method: "put",
    data: data,
  });
}

// 删除审批流
export function delProcess(ids) {
  return request({
    url: "/flow/process/" + ids,
    method: "delete",
  });
}

// 获取审批流定义列表
export function listVersion(query) {
  return request({
    url: "/flow/process/version/list",
    method: "get",
    params: query,
  });
}

// 获取审批流定义
export function getVersion(id) {
  return request({
    url: "/flow/process/version/" + id,
    method: "get",
  });
}

// 新增审批流定义
export function addVersion(data) {
  return request({
    url: "/flow/process/version",
    method: "post",
    data: data,
  });
}

// 更新审批流定义
export function updateVersion(data) {
  return request({
    url: "/flow/process/version",
    method: "put",
    data: data,
  });
}

// 删除审批流定义
export function delVersion(ids) {
  return request({
    url: "/flow/process/version/" + ids,
    method: "delete",
  });
}

// 复制审批流定义
export function copyVersion(id) {
  return request({
    url: "/flow/process/version/copy/" + id,
    method: "get",
  });
}

// 使用审批流定义
export function useVersion(id) {
  return request({
    url: "/flow/process/version/use/" + id,
    method: "get",
  });
}