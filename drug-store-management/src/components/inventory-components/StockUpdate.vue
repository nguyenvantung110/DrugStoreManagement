<!-- StockUpdate.vue -->
<template>
    <div>
      <div class="text-h6 font-weight-bold mb-4">Cập nhật tồn kho</div>
      <v-alert type="info" variant="tonal" class="mb-3">
        Chức năng dùng để nhập thêm thuốc mới hoặc cập nhật số lượng hiện có.
      </v-alert>
      <v-form @submit.prevent="onSubmit" ref="formRef" class="mb-5">
        <v-row>
          <v-col cols="12" md="6">
            <v-text-field v-model="form.code" label="Mã thuốc (*)" required />
          </v-col>
          <v-col cols="12" md="6">
            <v-text-field v-model="form.name" label="Tên thuốc (*)" required />
          </v-col>
          <v-col cols="12" md="4">
            <v-text-field v-model="form.unit" label="Đơn vị" />
          </v-col>
          <v-col cols="12" md="4">
            <v-text-field v-model="form.quantity" label="Số lượng nhập" type="number" min="1" />
          </v-col>
          <v-col cols="12" md="4">
            <v-text-field v-model="form.minStock" label="Tồn kho tối thiểu" type="number" min="0" />
          </v-col>
          <v-col cols="12">
            <v-btn type="submit" color="primary" class="mt-2">Cập nhật</v-btn>
          </v-col>
        </v-row>
      </v-form>
      <v-snackbar v-model="showSnackbar" color="success">
        Đã cập nhật tồn kho thành công!
      </v-snackbar>
    </div>
  </template>
  <script setup lang="ts">
  import { ref } from 'vue'
  const showSnackbar = ref(false)
  const form = ref({
    code: '',
    name: '',
    unit: '',
    quantity: '',
    minStock: ''
  })
  const formRef = ref()
  function onSubmit() {
    showSnackbar.value = true
    // Xử lý cập nhật vào API hoặc store ở đây
    form.value = { code: '', name: '', unit: '', quantity: '', minStock: '' }
    formRef.value?.resetValidation()
  }
  </script>