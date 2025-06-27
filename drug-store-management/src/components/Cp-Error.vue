<template>
    <div class="fill-height d-flex flex-column align-center justify-center text-center" style="background: linear-gradient(135deg, #e3f0ff 0%, #f9fafb 100%);">
      <div class="mb-4">
        <v-avatar size="90" class="mx-auto" style="background: #e3f0ff;">
          <v-icon size="70" color="primary">mdi-pill</v-icon>
        </v-avatar>
      </div>
      <h1 class="font-weight-bold mb-2" style="font-size: 2.8rem; color: #1976d2;">Có lỗi xảy ra!</h1>
      <p class="mb-6" style="font-size: 1.25rem; color: #757575;">
        Xin lỗi, hệ thống Quản lý Nhà thuốc gặp sự cố.<br>
        Vui lòng thử lại sau hoặc liên hệ bộ phận hỗ trợ kỹ thuật.
      </p>
      <v-btn color="primary" @click="goHome" size="large" elevation="2" prepend-icon="mdi-home">
        Trở về trang chính
      </v-btn>
      <div class="mt-8" style="color: #b0bec5; font-size: 1rem;">
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