import { createRouter, createWebHistory, RouteRecordRaw } from 'vue-router'
import HomeView from '@/views/HomeView.vue'
import LoginView from '@/views/LoginView.vue'
import MainLayout from '@/layouts/MainLayout.vue'
import { useAuthenticationStore } from '@/composables/auth/authenticationStore'

const routes: Array<RouteRecordRaw> = [
  {
    path: "/login",
    name: "login",
    component: LoginView,
    meta: {
      requiresAuth: false,
    },
  },
  {
    path: "/",
    component: MainLayout,
    meta: {
      requiresAuth: true,
    },
    children: [
      {
        path: "/",
        name: "home",
        component: HomeView,
        meta: {
          requiresAuth: true,
        },
      },
      {
        path: "/about",
        name: "about",
        component: () => import(/* webpackChunkName: "about" */ '../views/AboutView.vue'),
        meta: {
          requiresAuth: true,
        },
      },
    ],
  },
]

const router = createRouter({
  history: createWebHistory(),
  routes
})

// Call beforeEach on the router instance
router.beforeEach((to, from, next) => {
  const authenticationStore = useAuthenticationStore();
  to.meta.previousRouteName = from.name;
  if (to.meta.requiresAuth && !authenticationStore.isAuthenticated) {
    next("/login");
  } else {
    next();
  }
});

export default router
