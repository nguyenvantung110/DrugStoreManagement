<!-- StockHistory.vue -->
<template>
    <div>
      <div class="text-h6 font-weight-bold mb-4">Lịch sử giao dịch kho</div>
      <v-data-table
        :headers="headers"
        :items="items"
        :items-per-page="10"
        density="compact"
        class="elevation-1"
        :footer-props="{itemsPerPageOptions: [10, 20, 50]}"
      >
        <template #item.type="{ item }">
          <v-chip :color="item.type === 'Nhập' ? 'green' : (item.type === 'Điều chỉnh' ? 'orange' : 'blue')" size="small">
            {{ item.type }}
          </v-chip>
        </template>
      </v-data-table>
    </div>
  </template>
  <script setup lang="ts">
  const headers = [
    { title: 'Thời gian', key: 'ts' },
    { title: 'Mã thuốc', key: 'code' },
    { title: 'Tên thuốc', key: 'name' },
    { title: 'Loại giao dịch', key: 'type' },
    { title: 'Số lượng', key: 'quantity' },
    { title: 'Người thực hiện', key: 'by' }
  ]
  const items = [
    { ts: '2025-06-22 17:00', code: 'TH001', name: 'Paracetamol', type: 'Nhập', quantity: 100, by: 'Admin' },
    { ts: '2025-06-21 10:21', code: 'TH002', name: 'Vitamin C', type: 'Điều chỉnh', quantity: -5, by: 'Thủ kho' },
    { ts: '2025-06-18 15:43', code: 'TH001', name: 'Paracetamol', type: 'Xuất', quantity: -20, by: 'Nhân viên' },
  ]
  </script>