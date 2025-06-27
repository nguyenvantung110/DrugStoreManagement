<!-- StockCheck.vue -->
<template>
    <div>
      <div class="text-h6 font-weight-bold mb-4">Kiểm tra tồn kho</div>
      <v-text-field v-model="search" prepend-inner-icon="mdi-magnify" label="Tìm theo tên thuốc, mã vạch..." clearable class="mb-3" />
      <v-data-table
        :headers="headers"
        :items="filteredItems"
        :items-per-page="10"
        item-key="id"
        class="elevation-1"
        density="compact"
        :footer-props="{itemsPerPageOptions: [10, 20, 50]}"
      >
        <template #item.status="{ item }">
          <v-chip :color="item.quantity <= item.minStock ? 'red' : 'success'" size="small">
            {{ item.quantity <= item.minStock ? 'Cần nhập' : 'Đủ tồn kho' }}
          </v-chip>
        </template>
      </v-data-table>
    </div>
  </template>
  
  <script setup lang="ts">
  import { ref, computed } from 'vue'
  const search = ref('')
  const headers = [
    { title: 'Mã thuốc', key: 'code' },
    { title: 'Tên thuốc', key: 'name' },
    { title: 'Đơn vị', key: 'unit' },
    { title: 'Tồn kho', key: 'quantity' },
    { title: 'Tồn tối thiểu', key: 'minStock' },
    { title: 'Trạng thái', key: 'status' }
  ]
  const items = ref([
    { id: 1, code: 'TH001', name: 'Paracetamol 500mg', unit: 'Hộp', quantity: 23, minStock: 10 },
    { id: 2, code: 'TH002', name: 'Vitamin C', unit: 'Lọ', quantity: 4, minStock: 5 },
    { id: 3, code: 'TH003', name: 'Amoxicillin 500mg', unit: 'Vỉ', quantity: 52, minStock: 20 },
  ])
  const filteredItems = computed(() =>
    items.value.filter(
      i => i.name.toLowerCase().includes(search.value.toLowerCase()) ||
        i.code.toLowerCase().includes(search.value.toLowerCase())
    )
  )
  </script>