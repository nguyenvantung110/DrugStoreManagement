<template>
    <v-container fluid class="pa-0 h-full">
      <v-row class="h-full relative">
        <!-- Sidebar chức năng (ẩn trên mobile, chỉ hiện trên md+) -->
         <button
            color="primary"
            @click="isShowMenu = !isShowMenu"
            :class="{
              'absolute top-0 left-0 z-50 rounded-e-3xl bg-neutral-100 transition duration-300': true,
              'w-1/6 text-right pr-2': isShowMenu,
              'pl-3': !isShowMenu
            }"
          >
            <v-icon size="30" color="primary" :class="{'transition-transform duration-300': true, 'rotate-180': isShowMenu}">mdi-chevron-right</v-icon>
          </button>
        <v-col
          v-if="isShowMenu"
          cols="12"
          md="2"
          class="d-none d-md-block h-full flex flex-col justify-between transition-transform duration-300"
        >
          <v-card class="pa-4 mb-4 relative">
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
                <!-- <v-list-item-icon>
                  <v-icon>{{ option.icon }}</v-icon>
                </v-list-item-icon> -->
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
        <v-col class="grow">
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
  const isShowMenu = ref<boolean>(false)
  </script>