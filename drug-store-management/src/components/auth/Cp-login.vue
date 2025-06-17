<template>
    <div class="login-container h-100 w-100 flex-grow-1">
        This is login page
        <button @click="login">Login</button>
    </div>
</template>
<script setup lang="ts">
import { useAuthenticationStore } from '@/composables/auth/authenticationStore';
import { useRouter } from 'vue-router';

const router = useRouter();
const authenticationStore = useAuthenticationStore();

const login = async () => {
    await authenticationStore.signIn();
    router.push("/");
};
</script>
<style scoped></style>