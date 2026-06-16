import { createRouter, createWebHistory } from "vue-router";
import store from "@/store";

const routes = [
  {
    path: "/login",
    name: "Login",
    meta: { noAuth: true },
    /**
     * 动态导入登录组件视图
     * @returns {Promise<Component>} 返回一个解析为Vue组件的Promise对象
     */
    component: () => import("@/views/login.vue"),
  },
  {
    path: "/",
    name: "Layout",
    component: () => import("@/layout/Index.vue"),
    redirect: "/home",
    children: [
      {
        path: "/home",
        name: "Home",
        component: () => import("@/views/Home.vue"),
      },
      {
        path: "/404",
        name: "404",
        component: () => import("@/views/error/404.vue"),
      },
      {
        path: '/system/user-auth',
        hidden: true,
        permissions: ['system:user:edit'],
        children: [
          {
            path: 'role/:userId(\\d+)',
            component: () => import('@/views/system/user/authRole'),
            name: 'AuthRole',
            meta: { title: '分配角色', activeMenu: '/system/user' }
          }
        ]
      },
      {
        path: '/system/role-auth',
        hidden: true,
        permissions: ['system:role:edit'],
        children: [
          {
            path: 'user/:roleId(\\d+)',
            component: () => import('@/views/system/role/authUser'),
            name: 'AuthUser',
            meta: { title: '分配用户', activeMenu: '/system/role' }
          }
        ]
      },
      {
        path: '/system/dict-data',
        hidden: true,
        permissions: ['system:dict:list'],
        children: [
          {
            path: 'index/:dictId(\\d+)',
            component: () => import('@/views/system/dict/data'),
            name: 'Data',
            meta: { title: '字典数据', activeMenu: '/system/dict' }
          }
        ]
      },
      // {
      //   path: '/monitor/job-log',
      //   component: Layout,
      //   hidden: true,
      //   permissions: ['monitor:job:list'],
      //   children: [
      //     {
      //       path: 'index/:jobId(\\d+)',
      //       component: () => import('@/views/monitor/job/log'),
      //       name: 'JobLog',
      //       meta: { title: '调度日志', activeMenu: '/monitor/job' }
      //     }
      //   ]
      // },
    ],
  },
  // {
  //   path: "/:pathMatch(.*)*",
  //   redirect: "/404",
  // },
];

const router = createRouter({
  history: createWebHistory(),
  routes,
});
let isLoadRoute = false;
router.beforeEach(async (to, from, next) => {
  const token = store.getters["user/accessToken"];
  if (isLoadRoute && !to.matched?.length) {
    next("/404");
    return;
  }
  if (to.path === "/login") {
    next();
    return;
  }

  if (!isLoadRoute) {
    await store.dispatch("user/getRoutes");
    isLoadRoute = true;
  }
  if (to.meta?.noAuth) {
    if (!to.matched.length) {
      next({ ...to, replace: true });
    }
    next();
  } else {
    if (!token) {
      next({ path: "/login", query: { redirect: to.fullPath } });
      return;
    }
    // if (to.path === "/login" && token) {
    //   next("/");
    //   return;
    // }
    await store.dispatch("user/getUserInfo");
    if (!to.matched.length) {
      next({ ...to, replace: true });
    }
    next();
  }
});

export default router;
