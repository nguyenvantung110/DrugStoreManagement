<template>
    <NavBarComponent />
    <router-view />
    <LoadingComponent />
    <FooterComponent />
</template>
<script setup lang="ts">
    import NavBarComponent from "@/components/common/navbar-component.vue";
    import LoadingComponent from "@/components/common/loading-component.vue";
    import FooterComponent from "@/components/common/footer-component.vue";
</script>
<style scoped></style>
  