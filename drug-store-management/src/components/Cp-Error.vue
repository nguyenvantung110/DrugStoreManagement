<template>
    <div class="fill-height d-flex flex-column align-center justify-center text-center" style="background: linear-gradient(135deg, #e3f0ff 0%, #f9fafb 100%);">
      <div class="mb-4">
        <v-avatar size="90" class="mx-auto bg-[#11c3941c]">
          <v-icon size="70" color="#11c393">mdi-pill</v-icon>
        </v-avatar>
      </div>
      <h1 class="font-weight-bold mb-2 text-5xl text-[#11c393]">Có lỗi xảy ra!</h1>
      <p class="mb-6 text-lg text-[#757575]">
        Xin lỗi, hệ thống Quản lý Nhà thuốc gặp sự cố.<br>
        Vui lòng thử lại sau hoặc liên hệ bộ phận hỗ trợ kỹ thuật.
      </p>
      <v-btn class="text-white" color="#11c393" @click="goHome" size="large" elevation="2" prepend-icon="mdi-home">
        Trở về trang chính
      </v-btn>
      <div class="mt-8 text-[#b0bec5] text-md">
        <v-icon size="20" color="#b0bec5" class="mr-1">mdi-headset</v-icon>
        Hỗ trợ: 1900 1234 &nbsp;|&nbsp; Email: support@nhathuocspa.vn
      </div>
    </div>
  </template>
  
  <script setup lang="ts">
  import { useRouter } from 'vue-router'
  
  const router = useRouter()
  function goHome() {
    router.push({ name: 'home' })
  }
  </script>