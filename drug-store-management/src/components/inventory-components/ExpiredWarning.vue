<!-- ExpiredWarning.vue -->
<template>
    <div>
      <div class="text-h6 font-weight-bold mb-4">Cảnh báo thuốc hết hạn</div>
      <v-data-table
        :headers="headers"
        :items="items"
        density="compact"
        class="elevation-1"
        :footer-props="{itemsPerPageOptions: [10, 20, 50]}"
      >
        <template #item.exp="{ item }">
          <v-chip :color="new Date(item.exp) < new Date() ? 'red' : 'orange'" size="small">
            {{ item.exp }}
          </v-chip>
        </template>
      </v-data-table>
      <v-alert v-if="items.length === 0" type="success" class="mt-4">
        Không có thuốc nào sắp hết hạn!
      </v-alert>
    </div>
  </template>
  <script setup lang="ts">
  const headers = [
    { title: 'Mã thuốc', key: 'code' },
    { title: 'Tên thuốc', key: 'name' },
    { title: 'Hạn sử dụng', key: 'exp' },
    { title: 'Tồn kho', key: 'quantity' }
  ]
  const items = [
    { code: 'TH003', name: 'Amoxicillin', exp: '2025-07-01', quantity: 30 },
  ]
  </script>