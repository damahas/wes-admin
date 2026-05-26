import request from "@/utils/request";

// 查询
export function getLicense() {
  return request({
    url: "/system/license",
    method: "get",
  });
}

// 保存
export function saveLicense(data) {
  return request({
    url: "/system/license",
    method: "post",
    data: data,
  });
}

// 查询
export function getLicenseExpireTime() {
  return request({
    url: "/system/license/expireTime",
    method: "get",
  });
}