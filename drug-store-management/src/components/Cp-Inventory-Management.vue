<template>
    <v-container fluid class="pa-0">
      <v-row>
        <!-- Sidebar chức năng (ẩn trên mobile, chỉ hiện trên md+) -->
        <v-col cols="12" md="3" class="d-none d-md-block">
          <v-card class="pa-4 mb-4" elevation="3">
            <v-list nav dense>
              <v-list-item
                v-for="option in sidebarOptions"
                :key="option.value"
                :active="currentTab === option.value"
                @click="currentTab = option.value"
                class="rounded-lg"
                color="primary"
                style="cursor:pointer"
              >
                <v-list-item-icon>
                  <v-icon>{{ option.icon }}</v-icon>
                </v-list-item-icon>
                <v-list-item-title>{{ option.title }}</v-list-item-title>
              </v-list-item>
            </v-list>
          </v-card>
          <!-- Thêm một card nhỏ hướng dẫn hoặc chú thích -->
          <v-alert type="info" variant="tonal" class="mt-2">
            <b>Lưu ý:</b> Kiểm tra kỹ tồn kho trước khi điều chỉnh hoặc cập nhật!
          </v-alert>
        </v-col>
  
        <!-- Main content -->
        <v-col cols="12" md="9">
          <v-card flat class="pa-2 pa-md-5">
            <!-- Tabs navigation cho mobile -->
            <v-tabs
              v-model="currentTab"
              class="d-md-none"
              stacked
              background-color="white"
              show-arrows
              slider-color="primary"
            >
              <v-tab v-for="option in sidebarOptions" :key="option.value" :value="option.value">
                <v-icon left>{{ option.icon }}</v-icon>
                {{ option.title }}
              </v-tab>
            </v-tabs>
  
            <!-- Nội dung từng chức năng -->
            <div v-if="currentTab === 'stock_check'">
              <StockCheck />
            </div>
            <div v-if="currentTab === 'stock_update'">
              <StockUpdate />
            </div>
            <div v-if="currentTab === 'stock_adjust'">
              <StockAdjust />
            </div>
            <div v-if="currentTab === 'stock_history'">
              <StockHistory />
            </div>
            <div v-if="currentTab === 'stock_warning'">
              <StockWarning />
            </div>
            <div v-if="currentTab === 'expired_warning'">
              <ExpiredWarning />
            </div>
          </v-card>
        </v-col>
      </v-row>
    </v-container>
  </template>
  
  <script setup lang="ts">
  import StockCheck from '@/components/inventory-components/StockCheck.vue'
  import StockUpdate from '@/components/inventory-components/StockUpdate.vue'
  import StockAdjust from '@/components/inventory-components/StockAdjust.vue'
  import StockHistory from '@/components/inventory-components/StockHistory.vue'
  import StockWarning from '@/components/inventory-components/StockWarning.vue'
  import ExpiredWarning from '@/components/inventory-components/ExpiredWarning.vue'
  
  import { ref } from 'vue'
  
  const sidebarOptions = [
    { value: 'stock_check', title: 'Kiểm tra tồn kho', icon: 'mdi-warehouse' },
    { value: 'stock_update', title: 'Cập nhật tồn kho', icon: 'mdi-refresh-circle' },
    { value: 'stock_adjust', title: 'Điều chỉnh tồn kho', icon: 'mdi-tune-variant' },
    { value: 'stock_history', title: 'Lịch sử giao dịch', icon: 'mdi-history' },
    { value: 'stock_warning', title: 'Cảnh báo tồn kho thấp', icon: 'mdi-alert-decagram' },
    { value: 'expired_warning', title: 'Cảnh báo thuốc hết hạn', icon: 'mdi-calendar-alert' }, // chức năng khuyên dùng thêm
  ]
  
  const currentTab = ref('stock_check')
  </script>