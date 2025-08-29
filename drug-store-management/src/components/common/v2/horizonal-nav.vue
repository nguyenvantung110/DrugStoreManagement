<template>
    <div class="flex flex-row justify-between px-4 py-3 items-center sticky top-0 bg-white z-50">
        <div class="nav-button">
            <button @click="toggleLeftBarMenu()" class="2xl:hidden sm:block">
                <v-icon size="30" color="primary">mdi-menu</v-icon>
            </button>
        </div>
        <div class="horizonal-bar flex flex-row bg-white gap-5 items-center">
            <button>
                <v-icon size="20" color="#11c393">mdi-bell-badge-outline</v-icon>
            </button>
            <div class="account-btn relative">
                <button @click="toggleMenu($event)">
                    <v-icon size="30" color="#11c393">mdi-account-circle</v-icon>
                </button>
                <div
                    v-if="isShowAccountInfo"
                    class="account-info rounded-md border border-gray-400 absolute top-full right-0 bg-white z-50"
                >
                    <div class="info-header rounded-ss-md border-b border-gray-400 bg-slate-50 flex flex-row gap-3 py-5 px-7">
                        <div class="avatar rounded-full bg-[#3fbd9a] w-10 h-10 flex flex-col items-center justify-center">AB</div>
                        <div class="user-info  flex flex-col ggap-1">
                            <span class="text-sm font-medium">TungNV</span>
                            <span class="text-sm text-gray-400">tungnv@gmail.com</span>
                        </div>
                    </div>
                    <div class="inf-content py-5 px-7 flex flex-col gap-4">
                        <button class="flex flex-row gap-2 items-center hover:text-[#11c393]" @click="router.push('/personalinfo'); isShowAccountInfo = false">
                            <v-icon size="20" color="#11c393">mdi-account-outline</v-icon>
                            <span class="text-sm">Thông tin tài khoản</span>
                        </button>
                        <button class="flex flex-row gap-2 items-center hover:text-[#11c393]">
                            <v-icon size="20" color="#11c393">mdi-cog-outline</v-icon>
                            <span class="text-sm">Cài đặt tài khoản</span>
                        </button>
                    </div>
                    <div class="info-footer border-t border-gray-400 py-5 px-7">
                        <button class="flex flex-row gap-2 items-center hover:text-[#11c393]" @click="logout">
                            <v-icon size="20" color="#11c393">mdi-logout</v-icon>
                            <span class="text-sm">Đăng xuất</span>
                        </button>
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>
<script setup lang="ts">
import { onMounted, onUnmounted, ref } from 'vue';
import { useRouter } from 'vue-router';

const router = useRouter();
const isShowAccountInfo = ref<boolean>(false)

const closeContextMenu = (event: MouseEvent) => {
    event.stopPropagation();
    const contextMenu = document.querySelector(".account-info");
    if (contextMenu && !contextMenu.contains(event.target as Node)) {
        isShowAccountInfo.value = false;
    }
}

const isToggleLeftBar = ref<boolean>(true)
const emit = defineEmits<{
    toggleLeftBar: [toggleFlag: boolean]
}>();
  
const toggleLeftBarMenu = () => {
    isToggleLeftBar.value = !isToggleLeftBar.value
    emit("toggleLeftBar", isToggleLeftBar.value);
}

const toggleMenu = (event: MouseEvent) => {
  event.stopPropagation();
  isShowAccountInfo.value = !isShowAccountInfo.value;
};

const logout = () => {
    router.push('/login');
}

onMounted(() => {
    document.addEventListener('click', closeContextMenu)
})

onUnmounted(() => {
    document.removeEventListener('click', closeContextMenu)
})
</script>

<style scoped></style>
  