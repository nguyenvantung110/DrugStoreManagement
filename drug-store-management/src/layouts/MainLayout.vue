<template>
    <!-- Template v1 -->
    <!-- <NavBarComponent />
    <div class="mt-16 flex-grow-1">
        <router-view />
    </div>
    <LoadingComponent /> -->


    <!-- Template v2 -->
    <div class="flex flex-row h-full">
        <MainLeftBar :isToggleLeftBar = "isToggleLeftBar"/>
        <div :class="{'flex-grow-1 flex flex-col f-full duration-500': true, 'pl-72': isToggleLeftBar || !isSmallScreen}">
            <HorizonalNav @toggle-left-bar="toggleLeftBar"/>
            <router-view />
        </div>
    </div>
    <LoadingComponent />
</template>
<script setup lang="ts">
import NavBarComponent from "@/components/common/navbar-component.vue";
import LoadingComponent from "@/components/common/loading-component.vue";
import FooterComponent from "@/components/common/footer-component.vue";
import MainLeftBar from '@/components/common/v2/main-left-bar.vue'
import HorizonalNav from '@/components/common/v2/horizonal-nav.vue'
import { onMounted, onUnmounted, ref } from "vue";

const isToggleLeftBar = ref<boolean>(true)
const isSmallScreen = ref<boolean>(false)
const toggleLeftBar = (val: any) => {
    isToggleLeftBar.value = val
}

// 2xl with in tailwind
const breakpoint = 1536;

const showLeftBar = () => {
  isSmallScreen.value = window.innerWidth < breakpoint;
  isToggleLeftBar.value = window.innerWidth > breakpoint;
};

onMounted(() => {
  showLeftBar();
  window.addEventListener('resize', showLeftBar);
});

onUnmounted(() => {
  window.removeEventListener('resize', showLeftBar);
});
</script>
<style scoped></style>
  