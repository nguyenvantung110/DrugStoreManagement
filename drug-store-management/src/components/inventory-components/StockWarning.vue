<!-- StockWarning.vue -->
<template>
    <div>
      <div class="text-h6 font-weight-bold mb-4">Cảnh báo tồn kho tối thiểu</div>
      <v-data-table
        :headers="headers"
        :items="items"
        density="compact"
        class="elevation-1"
        :footer-props="{itemsPerPageOptions: [10, 20, 50]}"
      >
        <template #item.actions="{ item }">
          <v-btn icon="mdi-refresh" size="small" title="Cập nhật tồn kho" color="primary" variant="text"></v-btn>
        </template>
      </v-data-table>
      <v-alert v-if="items.length === 0" type="success" class="mt-4">
        Không có thuốc nào dưới mức cảnh báo!
      </v-alert>
    </div>
  </template>
  <script setup lang="ts">
  const headers = [
    { title: 'Mã thuốc', key: 'code' },
    { title: 'Tên thuốc', key: 'name' },
    { title: 'Tồn kho', key: 'quantity' },
    { title: 'Tồn tối thiểu', key: 'minStock' },
    { title: '', key: 'actions', sortable: false }
  ]
  const items = [
    { code: 'TH002', name: 'Vitamin C', quantity: 4, minStock: 5 },
  ]
  </script>