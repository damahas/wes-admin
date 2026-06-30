import axios from "axios";
import { ElMessage } from "element-plus";
import store from "@/store";
import router from "@/router";
import i18n from "@/locales";
import { tansParams } from "./index";

const { t } = i18n.global;

// 创建axios实例
const service = axios.create({
  baseURL: "/api",
  timeout: 60000,
});

// 请求拦截器
service.interceptors.request.use(
  (config) => {
    // 从store获取accessToken
    const accessToken = store.state.user.accessToken;
    if (accessToken) {
      config.headers["Authorization"] = `Bearer ${accessToken}`;
    }
    // get请求映射params参数
    if (config.method === 'get' && config.params) {
      const queryString = tansParams(config.params);
      if (queryString) {
        config.url = `${config.url}?${queryString}`;
        config.params = {};
      }
    }
    return config;
  },
  (error) => {
    console.error("Request error:", error);
    return Promise.reject(error);
  },
);

// 响应拦截器
service.interceptors.response.use(
  (response) => {
    const { code, msg } = response.data;
    // 二进制数据则直接返回
    if (response.request.responseType === 'blob' || response.request.responseType === 'arraybuffer') {
      return response.data
    }

    // code为200表示成功
    if (code === 200) {
      return response.data;
    }

    // code为401表示未授权，跳转登录页
    if (code === 401) {
      store.dispatch("user/logout");
      router.push('/login');
      ElMessage.error(msg || t("request.loginExpired"));
      return Promise.reject(new Error(msg || t("request.loginExpired")));
    }

    // code为402表示许可证过期，弹出维护弹窗
    if (code === 402) {
      store.dispatch("system/setLicenseDialog", true);
      return Promise.reject(new Error(msg));
    }

    // 其他错误，根据配置决定是否提示
    if (!response.config?.hideError) {
      ElMessage.error(msg || t("request.requestFailed"));
    }

    return Promise.reject(new Error(msg || t("request.requestFailed")));
  },
  (error) => {
    // 网络错误或超时
    if (error.code === 'ECONNABORTED' && error.message.includes('timeout')) {
      ElMessage.error(t("request.timeout"));
      return Promise.reject(error);
    }

    // 401表示未授权，跳转登录页
    if (error.response?.status === 401) {
      store.dispatch("user/logout");
      router.push('/login');
      return Promise.reject(error);
    }

    if (error.config?.showError !== false) {
      ElMessage.error(error.message || t("request.networkError"));
    }
    return Promise.reject(error);
  },
);

export default service;
