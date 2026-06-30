import { createApp } from "vue";
import App from "./App.vue";
import router from "./router";
import store from "./store";
import ElementPlus from "element-plus";
import zhCn from "element-plus/dist/locale/zh-cn.mjs";
import en from "element-plus/dist/locale/en.mjs";
import "element-plus/dist/index.css";
import "element-plus/theme-chalk/dark/css-vars.css";
import * as ElementPlusIconsVue from "@element-plus/icons-vue";
// 国际化
import i18n from "./locales";
// 本地字体（无需依赖外部 CDN）
import "@fontsource/inter/400.css";
import "@fontsource/inter/500.css";
import "@fontsource/inter/600.css";
import "@fontsource/jetbrains-mono/400.css";
import "@fontsource/jetbrains-mono/500.css";
// 通用样式
import "./styles/light.scss";
import "./styles/dark.scss";
import "./styles/common.scss";
// axios封装
import "./utils/request";
// 通用js
import { formatTime } from "@/utils";
import DictTag from '@/components/DictTag';
import Pagination from '@/components/Pagination';
import RightToolbar from '@/components/RightToolbar';
// 权限指令
import directive from './directive';

const app = createApp(App);
for (const [key, component] of Object.entries(ElementPlusIconsVue)) {
  app.component(key, component);
}
// 全局方法挂载
app.config.globalProperties.formatTime = formatTime
// 全局组件挂载
app.component('DictTag', DictTag)
app.component('Pagination', Pagination)
app.component('RightToolbar', RightToolbar)

app.use(store);
app.use(router);
app.use(ElementPlus, {
  locale: store.state.system.locale === "zh-CN" ? zhCn : en,
});
app.use(i18n);
app.use(directive);
app.mount("#app");
