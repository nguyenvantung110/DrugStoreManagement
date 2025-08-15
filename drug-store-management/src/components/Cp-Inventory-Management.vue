<template>
  <v-container class="px-10 md:px-8 py-4">
    <!-- Header Section - Consistent with Cp-Home -->
    <div class="mb-6">
      <div class="flex items-center justify-between mb-4">
        <div class="flex items-center gap-4">
          <div class="p-3 bg-[#11c393] rounded-2xl shadow-lg">
            <v-icon size="25" color="white">mdi-home-plus-outline</v-icon>
          </div>
          <div>
            <h1 class="text-lg md:text-lg font-bold text-gray-800">Quản lý kho thuốc</h1>
            <p class="text-gray-500 text-sm md:text-base">Kiểm soát và theo dõi tồn kho hiệu quả</p>
          </div>
        </div>
        
        <!-- Quick Stats - Consistent spacing -->
        <div class="hidden md:flex gap-4">
          <div class="text-center p-2 bg-white rounded-lg shadow-sm border border-gray-100 hover:shadow-md transition-shadow">
            <p class="text-2xl font-bold text-primary">1,247</p>
            <p class="text-xs text-gray-500">Tổng sản phẩm</p>
          </div>
          <div class="text-center p-2 bg-white rounded-lg shadow-sm border border-gray-100 hover:shadow-md transition-shadow">
            <p class="text-2xl font-bold text-warning">23</p>
            <p class="text-xs text-gray-500">Sắp hết hàng</p>
          </div>
          <div class="text-center p-2 bg-white rounded-lg shadow-sm border border-gray-100 hover:shadow-md transition-shadow">
            <p class="text-2xl font-bold text-error">5</p>
            <p class="text-xs text-gray-500">Hết hạn sớm</p>
          </div>
        </div>
      </div>
    </div>

    <!-- Navigation Tabs-->
    <div class="mb-6">
      <v-card class="nav-card" elevation="0">
        <v-card-text class="pa-2">
          <!-- Desktop Navigation -->
          <div class="hidden md:flex gap-3 flex-wrap">
            <v-btn
              v-for="(tab, index) in inventoryTabs"
              :key="tab.key"
              :variant="currentTab === tab.key ? 'flat' : 'text'"
              :color="currentTab === tab.key ? '#11c393' : 'grey-darken-1'"
              :class="[
                'nav-tab-btn',
                currentTab === tab.key ? 'active-tab text-white' : 'inactive-tab'
              ]"
              :style="{ animationDelay: `${index * 0.1}s` }"
              @click="currentTab = tab.key"
              size="large"
              rounded="lg"
            >
              <template #prepend>
                <v-icon :size="18">{{ tab.icon }}</v-icon>
              </template>
              <span class="ml-2 text-sm font-medium">{{ tab.title }}</span>
            </v-btn>
          </div>

          <!-- Mobile Navigation -->
          <div class="md:hidden">
            <v-select
              v-model="currentTab"
              :items="mobileSelectItems"
              item-title="title"
              item-value="value"
              variant="outlined"
              density="comfortable"
              hide-details
              class="mobile-nav-select"
              rounded="lg"
            >
              <template #prepend-inner>
                <v-icon color="primary" size="18">
                  {{ currentTabData.icon }}
                </v-icon>
              </template>
            </v-select>
          </div>
        </v-card-text>
      </v-card>
    </div>

    <!-- Main Content Area - Match dashboard structure -->
    <div class="content-wrapper">
      <v-card class="main-content-card" elevation="0">
        <!-- Dynamic Content -->
        <v-card-text class="content-body">
          <div class="min-h-[500px]">
            <!-- Loading State -->
            <div v-if="isLoading" class="loading-container">
              <v-progress-circular
                indeterminate
                color="primary"
                size="48"
                width="4"
              />
              <p class="text-gray-500 mt-4">Đang tải dữ liệu...</p>
            </div>

            <!-- Content Components -->
            <div v-else>
              <Transition name="fade" mode="out-in">
                <div :key="currentTab" class="tab-content">
                  <component :is="currentComponent" />
                </div>
              </Transition>
            </div>
          </div>
        </v-card-text>
      </v-card>
    </div>

    <!-- Alert Notification - Consistent positioning -->
    <v-snackbar
      v-model="showAlert"
      :color="alert.type"
      location="bottom right"
      timeout="3000"
      variant="elevated"
      rounded="lg"
    >
      <div class="flex items-center gap-2">
        <v-icon size="20">
          {{ getAlertIcon(alert.type) }}
        </v-icon>
        <div>
          <div class="font-medium">{{ alert.title }}</div>
          <div class="text-sm opacity-90">{{ alert.message }}</div>
        </div>
      </div>
      
      <template #actions>
        <v-btn
          icon="mdi-close"
          size="small"
          variant="text"
          @click="showAlert = false"
        />
      </template>
    </v-snackbar>

    <!-- Help Dialog - Consistent design -->
    <v-dialog v-model="showHelpDialog" max-width="700" persistent>
      <v-card rounded="xl">
        <v-card-title class="help-dialog-header">
          <v-icon color="#11c393" size="24">mdi-help-circle</v-icon>
          <span class="ml-2 font-bold">Hướng dẫn sử dụng</span>
        </v-card-title>
        
        <v-card-text class="px-6 py-4">
          <div class="space-y-4">
            <v-expansion-panels variant="accordion" rounded="lg">
              <v-expansion-panel
                v-for="tab in inventoryTabs"
                :key="tab.key"
                rounded="lg"
              >
                <v-expansion-panel-title>
                  <div class="flex items-center gap-3">
                    <v-icon :color="tab.color" size="20">{{ tab.icon }}</v-icon>
                    <span class="font-medium">{{ tab.title }}</span>
                  </div>
                </v-expansion-panel-title>
                <v-expansion-panel-text>
                  <p class="text-gray-600">{{ tab.description }}</p>
                </v-expansion-panel-text>
              </v-expansion-panel>
            </v-expansion-panels>
          </div>
        </v-card-text>
        
        <v-card-actions class="px-6 pb-6">
          <v-spacer />
          <v-btn
            variant="text"
            color="grey-darken-1"
            @click="showHelpDialog = false"
            rounded="lg"
          >
            Đóng
          </v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>

    <!-- Floating Action Button - Consistent with other components -->
    <v-fab
      class="fab-help"
      color="primary"
      icon="mdi-help-circle"
      size="large"
      location="bottom end"
      @click="showHelpDialog = true"
    />
  </v-container>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, watch, defineAsyncComponent } from 'vue'

// Types - Following project conventions
interface InventoryTab {
  key: string
  title: string
  icon: string
  color: string
  description: string
  component: string
}

interface TabAction {
  key: string
  label: string
  icon?: string
  variant?: 'text' | 'outlined' | 'flat'
  color?: string
}

interface Alert {
  type: 'success' | 'info' | 'warning' | 'error'
  title: string
  message: string
}

// Constants - Inventory tabs configuration
const inventoryTabs: InventoryTab[] = [
  {
    key: 'stock_check',
    title: 'Kiểm tra tồn kho',
    icon: 'mdi-home-lightning-bolt-outline',
    color: 'primary',
    description: 'Xem và tra cứu thông tin tồn kho hiện tại, số lượng và vị trí lưu trữ.',
    component: 'StockCheck'
  },
  {
    key: 'stock_adjust',
    title: 'Điều chỉnh tồn kho',
    icon: 'mdi-tune-variant',
    color: 'secondary',
    description: 'Điều chỉnh số liệu tồn kho khi có sai lệch hoặc cần chỉnh sửa thông tin.',
    component: 'StockAdjust'
  },
  {
    key: 'stock_history',
    title: 'Lịch sử giao dịch',
    icon: 'mdi-history',
    color: 'warning',
    description: 'Xem lại toàn bộ lịch sử nhập xuất kho và các giao dịch đã thực hiện.',
    component: 'StockHistory'
  },
  {
    key: 'stock_warning',
    title: 'Cảnh báo tồn kho',
    icon: 'mdi-alert-decagram',
    color: 'error',
    description: 'Theo dõi các sản phẩm có số lượng tồn kho thấp cần được nhập thêm.',
    component: 'StockWarning'
  },
  {
    key: 'expired_warning',
    title: 'Cảnh báo hết hạn',
    icon: 'mdi-calendar-alert',
    color: 'error',
    description: 'Quản lý các thuốc sắp hết hạn hoặc đã hết hạn để xử lý kịp thời.',
    component: 'ExpiredWarning'
  }
]

// Tab actions configuration
const tabActions: Record<string, TabAction[]> = {
  stock_check: [
    { key: 'refresh', label: 'Làm mới', icon: 'mdi-refresh', variant: 'text', color: 'primary' },
    { key: 'search', label: 'Tìm kiếm', icon: 'mdi-magnify', variant: 'outlined', color: 'primary' }
  ],
  stock_update: [
    { key: 'bulk_update', label: 'Cập nhật hàng loạt', icon: 'mdi-upload', variant: 'outlined', color: 'info' },
    { key: 'import', label: 'Import', icon: 'mdi-file-import', variant: 'text', color: 'info' }
  ],
  stock_adjust: [
    { key: 'save_changes', label: 'Lưu thay đổi', icon: 'mdi-content-save', variant: 'flat', color: 'success' }
  ],
  stock_history: [
    { key: 'export', label: 'Xuất báo cáo', icon: 'mdi-download', variant: 'outlined', color: 'warning' },
    { key: 'filter', label: 'Lọc', icon: 'mdi-filter', variant: 'text', color: 'warning' }
  ],
  stock_warning: [
    { key: 'mark_resolved', label: 'Đánh dấu đã xử lý', icon: 'mdi-check', variant: 'outlined', color: 'success' }
  ],
  expired_warning: [
    { key: 'remove_expired', label: 'Xử lý hết hạn', icon: 'mdi-delete', variant: 'outlined', color: 'error' }
  ]
}

// Component lazy loading - Following project structure
const componentMap = {
  StockCheck: defineAsyncComponent(() => import('@/components/inventory-components/StockCheck.vue')),
  StockAdjust: defineAsyncComponent(() => import('@/components/inventory-components/StockAdjust.vue')),
  StockHistory: defineAsyncComponent(() => import('@/components/inventory-components/StockHistory.vue')),
  StockWarning: defineAsyncComponent(() => import('@/components/inventory-components/StockWarning.vue')),
  ExpiredWarning: defineAsyncComponent(() => import('@/components/inventory-components/ExpiredWarning.vue'))
}

// Reactive state - Using setup-style API
const currentTab = ref<string>('stock_check')
const isLoading = ref<boolean>(false)
const showAlert = ref<boolean>(false)
const showHelpDialog = ref<boolean>(false)

const alert = ref<Alert>({
  type: 'info',
  title: '',
  message: ''
})

// Computed properties
const currentTabData = computed(() => {
  return inventoryTabs.find(tab => tab.key === currentTab.value) || inventoryTabs[0]
})

const currentComponent = computed(() => {
  const tab = currentTabData.value
  return componentMap[tab.component as keyof typeof componentMap] || componentMap.StockCheck
})

const currentTabActions = computed(() => {
  return tabActions[currentTab.value] || []
})

const mobileSelectItems = computed(() => {
  return inventoryTabs.map(tab => ({
    title: tab.title,
    value: tab.key
  }))
})

// Utility methods
const getAlertIcon = (type: string): string => {
  const icons = {
    success: 'mdi-check-circle',
    info: 'mdi-information',
    warning: 'mdi-alert',
    error: 'mdi-alert-circle'
  }
  return icons[type as keyof typeof icons] || 'mdi-information'
}

// Event handlers
const handleQuickAction = (actionKey: string): void => {
  const actions = {
    add_stock: { type: 'info', title: 'Nhập kho', message: 'Chức năng nhập kho đang được phát triển' },
    scan_barcode: { type: 'info', title: 'Quét mã vạch', message: 'Tính năng quét mã vạch sẽ được tích hợp' },
    export_report: { type: 'success', title: 'Xuất báo cáo', message: 'Đang chuẩn bị báo cáo tồn kho...' },
    check_alerts: { type: 'warning', title: 'Cảnh báo', message: 'Có 28 cảnh báo cần xử lý' }
  }

  const action = actions[actionKey as keyof typeof actions]
  if (action) {
    alert.value = action as Alert
    showAlert.value = true
  }
}

const handleTabAction = (actionKey: string): void => {
  alert.value = {
    type: 'success',
    title: 'Thành công',
    message: `Đã thực hiện hành động: ${actionKey}`
  }
  showAlert.value = true
}

const simulateLoading = (): void => {
  isLoading.value = true
  setTimeout(() => {
    isLoading.value = false
  }, 600)
}

// Lifecycle hooks
onMounted(() => {
  simulateLoading()
})

// Watchers
watch(currentTab, () => {
  simulateLoading()
})
</script>

<style scoped>
/* Component-specific styles following project conventions */
.nav-card {
  @apply bg-white border border-gray-100 shadow-sm;
  border-radius: 8px;
}

.nav-tab-btn {
  @apply transition-all duration-300 ease-out;
  min-width: 120px;
  height: 48px;
  text-transform: none;
  font-weight: 500;
  animation: slideInTab 0.6s ease-out both;
}

.active-tab {
  @apply shadow-md;
  transform: translateY(-2px);
}

.inactive-tab {
  @apply hover:shadow-sm;
}

.actions-card {
  @apply bg-white border border-gray-100 shadow-sm;
  border-radius: 16px;
}

.action-btn {
  @apply transition-all duration-200 hover:shadow-sm;
  text-transform: none;
  font-weight: 500;
}

.main-content-card {
  @apply bg-white border border-gray-100 shadow-sm;
  min-height: 600px;
}

.content-header {
  @apply px-6 py-5 border-b border-gray-100 bg-gray-50/50;
  border-radius: 16px 16px 0 0;
}

.content-body {
  @apply px-6 py-6;
}

.tab-action-btn {
  @apply transition-all duration-200;
  text-transform: none;
  font-weight: 500;
}

.loading-container {
  @apply flex flex-col items-center justify-center h-64;
}

.tab-content {
  animation: fadeInContent 0.4s ease-out;
}

/* .help-dialog-header {
  @apply px-6 py-5 bg-primary/5 border-b border-gray-100;
  border-radius: 24px 24px 0 0;
} */

.fab-help {
  position: fixed;
  bottom: 32px;
  right: 32px;
  z-index: 1000;
}

.mobile-nav-select :deep(.v-field) {
  border-radius: 12px;
}

/* Animations */
@keyframes slideInTab {
  from {
    opacity: 0;
    transform: translateY(20px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

@keyframes fadeInContent {
  from {
    opacity: 0;
    transform: translateY(10px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

/* Transitions */
.fade-enter-active,
.fade-leave-active {
  transition: all 0.3s ease;
}

.fade-enter-from {
  opacity: 0;
  transform: translateY(10px);
}

.fade-leave-to {
  opacity: 0;
  transform: translateY(-10px);
}

/* Responsive design */
@media (max-width: 768px) {
  .content-header {
    @apply px-4 py-4 text-sm;
  }
  
  .content-body {
    @apply px-4 py-4;
  }
  
  .nav-tab-btn {
    min-width: 100px;
    height: 44px;
  }
  
  .fab-help {
    bottom: 20px;
    right: 20px;
  }
}

/* Custom scrollbar - consistent across components */
:deep(.v-card-text::-webkit-scrollbar) {
  width: 6px;
  height: 6px;
}

:deep(.v-card-text::-webkit-scrollbar-track) {
  @apply bg-gray-100 rounded-full;
}

:deep(.v-card-text::-webkit-scrollbar-thumb) {
  @apply bg-gray-300 rounded-full hover:bg-gray-400;
}
</style>