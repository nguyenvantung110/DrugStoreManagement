<template>
  <aside
  :class="{
    'h-full bg-white shadow-md w-72 flex flex-col px-6 py-8 z-[999] fixed top-0 duration-500': true,
    'sm:left-0': props.isToggleLeftBar,
    'sm:-left-72': !props.isToggleLeftBar
  }">
    <!-- Logo -->
    <div class="flex items-center gap-3 mb-10">
      <v-icon :color="highlightColor" class="text-3xl">mdi-pharmacy</v-icon>
      <span class="text-2xl font-extrabold text-gray-700">Pharmacy</span>
    </div>
    <ul class="flex flex-col gap-2">
      <li>
        <router-link
          to="/"
          class="flex items-center gap-4 px-5 py-3 rounded-xl cursor-pointer group transition text-sm font-medium"
          :class="colorClassExact('/')"
        >
          <v-icon :color="iconColorExact('/')" size="20" class="mr-1 transition-colors">mdi-home</v-icon>
          <span>Trang chủ</span>
        </router-link>
      </li>
      <li>
        <router-link
          to="/sales"
          class="flex items-center gap-4 px-5 py-3 rounded-xl cursor-pointer group transition text-sm font-medium"
          :class="colorClassExact('/sales')"
        >
          <v-icon :color="iconColorExact('/sales')" size="20" class="mr-1 transition-colors">mdi-cart</v-icon>
          <span>Bán hàng</span>
        </router-link>
      </li>
      <!-- Đơn hàng - Collapse -->
      <li>
        <button
          class="flex items-center gap-4 px-5 py-3 rounded-xl cursor-pointer group transition text-sm font-medium w-full select-none"
          :class="colorClassCollapse"
          @click="toggleOrderMenu"
        >
          <v-icon :color="iconColorCollapse" size="20" class="mr-1 transition-colors">mdi-file-document-outline</v-icon>
          <span>Đơn hàng</span>
          <v-icon class="ml-auto transition-transform" :class="{'rotate-90': orderMenuOpen}" color="grey">
            mdi-chevron-right
          </v-icon>
        </button>
        <transition name="fade">
          <ul v-show="orderMenuOpen" class="pl-15 flex flex-col gap-1">
            <li>
              <router-link
                to="/purchase-request"
                class="block py-2 text-sm rounded-lg transition font-medium"
                :class="colorClassExact('/purchase-request')"
              >Đơn đặt hàng</router-link>
            </li>
            <li>
              <router-link
                to="/purchase-order"
                class="block py-2 text-sm rounded-lg transition font-medium"
                :class="colorClassExact('/purchase-order')"
              >Đơn nhập hàng</router-link>
            </li>
          </ul>
        </transition>
      </li>
      <li>
        <router-link
          to="/inventorymanagement"
          class="flex items-center gap-4 px-5 py-3 rounded-xl cursor-pointer group transition text-sm font-medium"
          :class="colorClassExact('/inventorymanagement')"
        >
          <v-icon :color="iconColorExact('/inventorymanagement')" size="20" class="mr-1 transition-colors">mdi-home-plus-outline</v-icon>
          <span>Quản lý kho</span>
        </router-link>
      </li>
      <li>
        <router-link
          to="/suppliermanagement"
          class="flex items-center gap-4 px-5 py-3 rounded-xl cursor-pointer group transition text-sm font-medium"
          :class="colorClassExact('/suppliermanagement')"
        >
          <v-icon :color="iconColorExact('/suppliermanagement')" size="20" class=" mr-1 transition-colors">mdi-truck-fast</v-icon>
          <span>Nhà cung cấp</span>
        </router-link>
      </li>
      <li>
        <router-link
          to="/customermanagement"
          class="flex items-center gap-4 px-5 py-3 rounded-xl cursor-pointer group transition text-sm font-medium"
          :class="colorClassExact('/customermanagement')"
        >
          <v-icon :color="iconColorExact('/customermanagement')" size="20" class=" mr-1 transition-colors">mdi-account-cash-outline</v-icon>
          <span>Khách mua hàng</span>
        </router-link>
      </li>
      <li>
        <router-link
          to="/usermanagement"
          class="flex items-center gap-4 px-5 py-3 rounded-xl cursor-pointer group transition text-sm font-medium"
          :class="colorClassExact('/usermanagement')"
        >
          <v-icon :color="iconColorExact('/usermanagement')" size="20" class=" mr-1 transition-colors">mdi-account-group</v-icon>
          <span>Quản lý nhân viên</span>
        </router-link>
      </li>
    </ul>
    <!-- Avatar - icon vuetify -->
    <div class="mt-auto flex items-center gap-4 pt-10">
      <v-icon :color="highlightColor" class="text-3xl">mdi-account-circle</v-icon>
      <span class="text-lg text-gray-600">Nguyen Van A</span>
    </div>
  </aside>
</template>

<script setup lang="ts">
import { useRoute } from 'vue-router'
import { ref, computed } from 'vue'

const highlightColor = "#11c393"
const route = useRoute()
const props = defineProps<{
  isToggleLeftBar: boolean
}>()
// Collapse state
const orderMenuOpen = ref(
  ['/purchase-request', '/purchase-order'].includes(route.path)
)

function toggleOrderMenu() {
  orderMenuOpen.value = !orderMenuOpen.value
}

// Xác định active cho collapse
const orderMenuActive = computed(() =>
  ['/purchase-request', '/purchase-order'].includes(route.path)
)
const colorClassCollapse = computed(() => orderMenuActive.value ? 'text-[##11c393]' : 'text-gray-600 hover:text-[#11c393]')
const iconColorCollapse = computed(() => orderMenuActive.value ? highlightColor : undefined)

// Xác định active/hover cho từng route
function colorClassExact(path: any) {
  return route.path === path
    ? 'text-[#11c393]'
    : 'text-gray-600 hover:text-[#11c393]'
}

function iconColorExact(path: any) {
  return route.path === path ? highlightColor : undefined
}
</script>

<style scoped>
/* Hiệu ứng collapse cho submenu */
.fade-enter-active, .fade-leave-active {
  transition: all 0.2s cubic-bezier(.55,0,.1,1);
}
.fade-enter-from, .fade-leave-to {
  opacity: 0;
  max-height: 0;
  transform: translateY(-8px);
}
</style>