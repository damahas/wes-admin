import request from "@/utils/request";

// 获取验证码图片
export function getCodeImg(width, height) {
  return request({
    url: `/captchaImage?width=${width}&height=${height}`,
    headers: {
      isToken: false,
    },
    method: "get",
    timeout: 20000,
  });
}

// 验证验证码
export function validCodeImg(code, sliderPositionX) {
  return request({
    url: `/validCaptchaImage?code=${code}&sliderPositionX=${sliderPositionX}`,
    headers: {
      isToken: false,
    },
    method: "get",
    timeout: 20000,
  });
}

// 登录
export function login(data) {
  // {
  //   username,
  //   password,
  //   code,
  //   positionX
  // }
  return request({
    url: "/login",
    method: "post",
    data,
  });
}

// 获取用户详细信息
export function getInfo() {
  return request({
    url: "/getInfo",
    method: "get",
  });
}

// 退出方法
export function logout() {
  return request({
    url: '/logout',
    method: 'post'
  })
}
export function isCaptchaOn() {
  return request({
    url: "/isCaptchaOn",
    method: "get",
  });
}