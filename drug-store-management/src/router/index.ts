import { createRouter, createWebHistory, RouteRecordRaw } from 'vue-router'
import HomeView from '@/views/HomeView.vue'
import LoginView from '@/views/LoginView.vue'
import ErrorView from '@/views/ErrorView.vue'
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
    path: "/error",
    name: "error",
    component: ErrorView,
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
        component: () => import('@/views/AboutView.vue'),
        meta: {
          requiresAuth: true,
        },
      },
      {
        path: "/inventorymanagement",
        name: "inventorymanagement",
        component: () => import('@/views/InventoryManagement.vue'),
        meta: {
          requiresAuth: true,
        },
      },
      {
        path: "/purchase-order",
        name: "purchase-order",
        component: () => import('@/views/purchase-management/PurchaseOrderView.vue'),
        meta: {
          requiresAuth: true,
        },
      },
      {
        path: "/purchase-request",
        name: "purchase-request",
        component: () => import('@/views/purchase-management/PurchaseRequestView.vue'),
        meta: {
          requiresAuth: true,
        },
      },
      {
        path: "/suppliermanagement",
        name: "suppliermanagement",
        component: () => import('@/views/SupplierManagement.vue'),
        meta: {
          requiresAuth: true,
        },
      },
      {
        path: "/userinfo",
        name: "userinfo",
        component: () => import('@/views/UserInfoView.vue'),
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
