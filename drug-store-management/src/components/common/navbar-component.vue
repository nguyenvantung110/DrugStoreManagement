<template lang="">
    <v-app-bar :elevation="2">
        <template v-slot:prepend>
            <!-- <v-app-bar-nav-icon></v-app-bar-nav-icon> -->
            <v-icon size="30" color="primary">mdi-pill</v-icon>
        </template>

        <v-app-bar-title>Nhà thuốc Xanh</v-app-bar-title>
    
        <v-btn>
          <router-link class="text-primary" to="/">Trang chủ</router-link>
        </v-btn>
        <v-btn>
          <router-link class="text-primary" to="/sales">Bán hàng</router-link>
        </v-btn>
        <v-menu>
          <template v-slot:activator="{ props }">
            <v-btn color="primary" v-bind="props">Đơn hàng</v-btn>
          </template>
          <v-list>
            <v-list-item :key="1" :value="1">
              <v-list-item-title>
                <router-link class="text-primary" to="/purchase-request">Đơn đặt hàng</router-link>
              </v-list-item-title>
            </v-list-item>
            <v-list-item :key="2" :value="2">
              <v-list-item-title>
                <router-link class="text-primary" to="/purchase-order">Nhập hàng</router-link>
              </v-list-item-title>
            </v-list-item>
          </v-list>
        </v-menu>
        <v-btn>
          <router-link class="text-primary" to="/inventorymanagement">Quản lý kho</router-link>
        </v-btn>
        <v-btn>
          <router-link class="text-primary" to="/suppliermanagement">Nhà cung cấp</router-link>
        </v-btn>
        <v-btn>
          <router-link class="text-primary" to="/test">Test</router-link>
        </v-btn>
        
        <template v-slot:append>
            <v-btn color="primary" @click="isReaded = !isReaded">
              <v-icon v-if="!isReaded" size="20" color="red">mdi-bell-badge</v-icon>
              <v-icon v-else size="20" color="primary">mdi-bell</v-icon>
            </v-btn>
            <!-- <v-btn v-else color="primary" icon="mdi-bell" @click="isReaded = !isReaded"></v-btn> -->
            <!-- <v-btn icon="mdi-account"></v-btn> -->
            <v-menu>
              <template v-slot:activator="{ props }">
                <v-btn icon="mdi-account" color="primary" v-bind="props"></v-btn>
              </template>
              <v-list>
                <v-list-item :key="1" :value="1">
                  <v-list-item-title class="text-primary"  @click="router.push('/userinfo')">
                    Tài khoản
                  </v-list-item-title>
                </v-list-item>
                <v-list-item :key="2" :value="2">
                  <v-list-item-title class="text-primary" @click="router.push('/login')">
                    Đăng xuất
                  </v-list-item-title>
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
  const isReaded = ref(false);

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
  