<template lang="">
    <v-app-bar :elevation="2">
        <template v-slot:prepend>
            <v-app-bar-nav-icon></v-app-bar-nav-icon>
        </template>

        <v-app-bar-title>Application Bar</v-app-bar-title>

        <template v-slot:append>
            <v-btn icon="mdi-heart"></v-btn>

            <v-btn icon="mdi-magnify"></v-btn>

            <v-btn icon="mdi-dots-vertical"></v-btn>
        </template>
    </v-app-bar>
</template>
<script setup lang="ts">
  import { onMounted, onUnmounted, ref } from "vue";
  import { useAuthenticationStore } from "@/composables/auth/authenticationStore";
  
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
  