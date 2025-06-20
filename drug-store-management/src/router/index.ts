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
          requiresAuth: false,
        },
      },
      {
        path: "/about",
        name: "about",
        component: () => import('@/views/AboutView.vue'),
        meta: {
          requiresAuth: false,
        },
      },
      {
        path: "/inventorymanagement",
        name: "inventorymanagement",
        component: () => import('@/views/InventoryManagement.vue'),
        meta: {
          requiresAuth: false,
        },
      },
      {
        path: "/ordermanagement",
        name: "ordermanagement",
        component: () => import('@/views/OrderManagement.vue'),
        meta: {
          requiresAuth: false,
        },
      },
      {
        path: "/purchasemanagement",
        name: "purchasemanagement",
        component: () => import('@/views/PurchaseManagement.vue'),
        meta: {
          requiresAuth: false,
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
