<template lang="">
    <div v-if="loading.isShow" class="loading-spinner">
      <div class="spinner"></div>
      <!-- <span>Loading...</span> -->
    </div>
</template>
<script setup lang="ts">
    import { useLoadingStore } from "@/composables/common/loadingStore";
    const loading = useLoadingStore();
</script>

<style scoped>
    .loading-spinner {
    position: absolute;
    top: 0;
    left: 0;
    background-image: radial-gradient(
        circle farthest-corner at center,
        #2d373f6e 5%,
        #151d2075 100%
    );
    display: flex;
    flex-direction: column;
    align-items: center;
    justify-content: center;
    height: 100%;
    width: 100vw;
    z-index: 9999;
    }

    .spinner {
    border: 4px solid #f3f3f3;
    border-top: 4px solid #90c6cf;
    border-radius: 50%;
    width: 40px;
    height: 40px;
    animation: spin 1s linear infinite;
    }

    @keyframes spin {
    0% {
        transform: rotate(0deg);
    }
    100% {
        transform: rotate(360deg);
    }
    }
</style>
  