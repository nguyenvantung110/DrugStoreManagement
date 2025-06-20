<template lang="">
    <v-app-bar :elevation="2">
        <template v-slot:prepend>
            <v-app-bar-nav-icon></v-app-bar-nav-icon>
        </template>

        <v-app-bar-title>Nhà thuốc Xanh</v-app-bar-title>
    
        <v-btn>
          <router-link to="/">Trang chủ</router-link>
        </v-btn>
        <v-btn>
          <router-link to="/ordermanagement">Đơn đặt hàng</router-link>
        </v-btn>
        <v-btn>
          <router-link to="/purchasemanagement">Nhập hàng</router-link>
        </v-btn>
        <v-btn>
          <router-link to="/inventorymanagement">Quản lý kho</router-link>
        </v-btn>
        
        <template v-slot:append>
            <v-btn icon="mdi-bell"></v-btn>
            <!-- <v-btn icon="mdi-account"></v-btn> -->
            <v-menu>
              <template v-slot:activator="{ props }">
                <v-btn icon="mdi-account" color="primary" v-bind="props"></v-btn>
              </template>
              <v-list>
                <v-list-item :key="1" :value="1">
                  <v-list-item-title>Tài khoản</v-list-item-title>
                </v-list-item>
                <v-list-item :key="2" :value="2">
                  <v-list-item-title @click="router.push('/login')">Đăng xuất</v-list-item-title>
                </v-list-item>
              </v-list>
          </v-menu>
        </template>
    </v-app-bar>
</template>
<script setup lang="ts">
  import { onMounted, onUnmounted, ref } from "vue";
  import { useAuthenticationStore } from "@/composables/auth/authenticationStore";
  import { useRouter } from "vue-router";
  const router = useRouter();
  const authenticationStore = useAuthenticationStore();
  const isShowMenu = ref(false);
  
  /**
   * Hook onMounted
   */
  onMounted(() => {
    document.addEventListener("click", closeContextMenu);
  });
  
  const logout = () => {
    // logout logic
  };
  
  /**
   * Close menu when clicking outside
   */
  const closeContextMenu = (event: MouseEvent) => {
    event.stopPropagation();
    const contextMenu = document.querySelector(".options-menu");
    if (contextMenu && !contextMenu.contains(event.target as Node)) {
      isShowMenu.value = false;
    }
  };
  
  const toggleMenu = (event: MouseEvent) => {
    event.stopPropagation();
    isShowMenu.value = !isShowMenu.value;
  };
  
  /**
   * Hook onUnmounted
   */
  onUnmounted(async () => {
    document.removeEventListener("click", closeContextMenu);
  });
</script>
 <style scoped></style>
  