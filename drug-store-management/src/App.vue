<template>
  <v-app>
    <LoadingComponent />
    <router-view />
  </v-app>
</template>
<script setup lang="ts">
  import LoadingComponent from "@/components/common/loading-component.vue";
</script>
<style></style>
