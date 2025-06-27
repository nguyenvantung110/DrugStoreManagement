<!-- StockAdjust.vue -->
<template>
    <div>
      <div class="text-h6 font-weight-bold mb-4">Điều chỉnh tồn kho</div>
      <v-alert type="warning" variant="tonal" class="mb-3">
        Dùng cho các trường hợp kiểm kê thực tế, sai lệch, thất thoát, hủy thuốc, nhập nhầm...
      </v-alert>
      <v-form @submit.prevent="onAdjust" class="mb-5">
        <v-row>
          <v-col cols="12" md="6">
            <v-text-field v-model="adjust.code" label="Mã thuốc" />
          </v-col>
          <v-col cols="12" md="6">
            <v-text-field v-model="adjust.name" label="Tên thuốc" />
          </v-col>
          <v-col cols="12" md="4">
            <v-text-field v-model="adjust.oldQuantity" label="Tồn kho cũ" type="number" />
          </v-col>
          <v-col cols="12" md="4">
            <v-text-field v-model="adjust.newQuantity" label="Tồn kho sau điều chỉnh" type="number" />
          </v-col>
          <v-col cols="12" md="4">
            <v-text-field v-model="adjust.reason" label="Lý do điều chỉnh" />
          </v-col>
          <v-col cols="12">
            <v-btn type="submit" color="orange" class="mt-2">Xác nhận điều chỉnh</v-btn>
          </v-col>
        </v-row>
      </v-form>
      <v-snackbar v-model="showSnackbar" color="success">
        Đã điều chỉnh tồn kho!
      </v-snackbar>
    </div>
  </template>
  <script setup lang="ts">
  import { ref } from 'vue'
  const adjust = ref({
    code: '',
    name: '',
    oldQuantity: '',
    newQuantity: '',
    reason: ''
  })
  const showSnackbar = ref(false)
  function onAdjust() {
    showSnackbar.value = true
    // Xử lý lưu điều chỉnh
    adjust.value = { code: '', name: '', oldQuantity: '', newQuantity: '', reason: '' }
  }
  </script>