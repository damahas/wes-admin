import { getInfo, logout } from "@/api/login";
import { getRouters } from "@/api/system/menu";
import router from "@/router";
import Layout from "@/layout/Index.vue";
import InnerLink from "@/layout/components/InnerLink";
import ParentView from "@/layout/components/ParentView";

// 匹配views里面所有的.vue文件
const modules = import.meta.glob("../../views/**/*.vue");

function loadView(view) {
  let res;
  for (const path in modules) {
    const dir = path.split("views/")[1].split(".vue")[0];
    if (dir === view) {
      res = () => modules[path]();
    }
  }
  return res;
}

function filterChildren(childrenMap, lastRouter = false) {
  var children = [];
  childrenMap.forEach((el, index) => {
    if (el.children && el.children.length) {
      if (el.component === "ParentView" && !lastRouter) {
        el.children.forEach((c) => {
          c.path = el.path + "/" + c.path;
          if (c.children && c.children.length) {
            children = children.concat(filterChildren(c.children, c));
            return;
          }
          children.push(c);
        });
        return;
      }
    }
    if (lastRouter) {
      el.path = lastRouter.path + "/" + el.path;
      if (el.children && el.children.length) {
        children = children.concat(filterChildren(el.children, el));
        return;
      }
    }
    children = children.concat(el);
  });
  return children;
}

// 遍历后台传来的路由字符串，转换为组件对象
function filterAsyncRouter(asyncRouterMap) {
  return asyncRouterMap.filter((route) => {
    if (route.children) {
      route.children = filterChildren(route.children);
    }
    if (route.component) {
      // Layout 组件特殊处理
      if (route.component === "Layout") {
        route.component = Layout;
      } else if (route.component === "ParentView") {
        route.component = ParentView;
      } else if (route.component === "InnerLink") {
        route.component = InnerLink;
      } else {
        route.component = loadView(route.component);
      }
    }
    if (route?.children?.length) {
      route.children = filterAsyncRouter(route.children);
    } else {
      delete route["children"];
      delete route["redirect"];
    }
    return true;
  });
}

export default {
  namespaced: true,
  state: {
    userInfo: null,
    permissions: null,
    roles: null,
    routes: null,
    accessToken: localStorage.getItem("accessToken") || null,
  },
  getters: {
    userInfo: (state) => state.userInfo,
    accessToken: (state) => state.accessToken,
  },
  mutations: {
    SET_USER_INFO(state, userInfo) {
      state.userInfo = userInfo;
    },
    SET_PERMISSIONS(state, permissions) {
      state.permissions = permissions;
    },
    SET_ROLES(state, roles) {
      state.roles = roles;
    },
    SET_ROUTES(state, routes) {
      state.routes = routes;
    },
    SET_ACCESS_TOKEN(state, token) {
      state.accessToken = token;
      localStorage.removeItem("accessToken");
      if (token) {
        localStorage.setItem("accessToken", token);
      }
    },
  },
  actions: {
    setAccessToken({ commit }, token) {
      commit("SET_ACCESS_TOKEN", token);
    },
    async getUserInfo({ commit, state }) {
      if (state.userInfo) {
        return;
      }
      try {
        const info = await getInfo();
        commit("SET_USER_INFO", info.user);
        commit("SET_PERMISSIONS", info.permissions);
        commit("SET_ROLES", info.roles);
      } catch (error) {
        commit("SET_USER_INFO", null);
        throw error;
      }
    },
    logout({ commit }) {
      commit("SET_USER_INFO", null);
      commit("SET_ACCESS_TOKEN", null);
      // logout();
    },
    async getRoutes({ commit, state }) {
      if (state.routes) {
        return;
      }
      const r = await getRouters();
      const sdata = JSON.parse(JSON.stringify(r.data));
      const sidebarRoutes = filterAsyncRouter(sdata);
      sidebarRoutes.forEach((route) => {
        router.addRoute(route);
        // router.addRoute("Layout", route);
      });
      commit("SET_ROUTES", r.data);
    },
  },
};
