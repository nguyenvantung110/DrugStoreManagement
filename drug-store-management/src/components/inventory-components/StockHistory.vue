<template>
  <div class="stock-history-container">
    <!-- Header Section -->
    <div class="flex items-center justify-between mb-6">
      <div class="flex items-center gap-3">
        <v-icon color="warning" size="24">mdi-history</v-icon>
        <div>
          <h2 class="text-xl font-bold text-gray-800">Lịch sử giao dịch</h2>
          <p class="text-gray-500 text-sm">Xem lại toàn bộ lịch sử nhập xuất kho và các giao dịch đã thực hiện</p>
        </div>
      </div>

      <!-- Action Buttons -->
      <div class="flex gap-2">
        <v-btn
          variant="outlined"
          color="primary"
          size="small"
          prepend-icon="mdi-refresh"
          @click="refreshData"
        >
          Làm mới
        </v-btn>
        <v-btn
          variant="outlined"
          color="success"
          size="small"
          prepend-icon="mdi-download"
          @click="exportHistory"
        >
          Xuất báo cáo
        </v-btn>
      </div>
    </div>

    <!-- Filter & Search Section -->
    <v-card class="mb-6 filter-card" elevation="0">
      <v-card-text class="pb-4">
        <div class="grid grid-cols-1 md:grid-cols-4 gap-4 items-center">
          <!-- Search Input -->
          <v-text-field
            v-model="searchQuery"
            label="Tìm kiếm giao dịch..."
            prepend-inner-icon="mdi-magnify"
            variant="outlined"
            density="comfortable"
            hide-details
            clearable
            placeholder="Nhập mã thuốc hoặc tên thuốc..."
          />

          <!-- Transaction Type Filter -->
          <v-select
            v-model="typeFilter"
            :items="transactionTypes"
            label="Loại giao dịch"
            prepend-inner-icon="mdi-swap-vertical"
            variant="outlined"
            density="comfortable"
            hide-details
            clearable
          />

          <!-- Date Range -->
          <v-menu
            v-model="dateMenu"
            :close-on-content-click="false"
            transition="scale-transition"
            offset-y
            min-width="auto"
          >
            <template #activator="{ props }">
              <v-text-field
                v-bind="props"
                v-model="dateRangeText"
                label="Khoảng thời gian"
                prepend-inner-icon="mdi-calendar"
                variant="outlined"
                density="comfortable"
                hide-details
                readonly
              />
            </template>
            <v-date-picker
              v-model="dateRange"
              range
              @update:model-value="dateMenu = false"
            />
          </v-menu>

          <!-- User Filter -->
          <v-select
            v-model="userFilter"
            :items="userOptions"
            label="Người thực hiện"
            prepend-inner-icon="mdi-account"
            variant="outlined"
            density="comfortable"
            hide-details
            clearable
          />
        </div>
        
        <!-- Quick Filter Chips -->
        <div class="flex flex-wrap gap-2 mt-4">
          <v-chip
            :variant="quickFilter === 'all' ? 'flat' : 'outlined'"
            :color="quickFilter === 'all' ? 'primary' : 'default'"
            size="small"
            clickable
            @click="setQuickFilter('all')"
          >
            Tất cả ({{ totalTransactions }})
          </v-chip>
          <v-chip
            :variant="quickFilter === 'import' ? 'flat' : 'outlined'"
            :color="quickFilter === 'import' ? 'success' : 'default'"
            size="small"
            clickable
            @click="setQuickFilter('import')"
          >
            Nhập kho ({{ importCount }})
          </v-chip>
          <v-chip
            :variant="quickFilter === 'export' ? 'flat' : 'outlined'"
            :color="quickFilter === 'export' ? 'info' : 'default'"
            size="small"
            clickable
            @click="setQuickFilter('export')"
          >
            Xuất kho ({{ exportCount }})
          </v-chip>
          <v-chip
            :variant="quickFilter === 'adjust' ? 'flat' : 'outlined'"
            :color="quickFilter === 'adjust' ? 'warning' : 'default'"
            size="small"
            clickable
            @click="setQuickFilter('adjust')"
          >
            Điều chỉnh ({{ adjustCount }})
          </v-chip>
        </div>
      </v-card-text>
    </v-card>

    <!-- Main Data Table -->
    <v-card class="main-table-card" elevation="0">
      <v-card-text class="pa-0">
        <!-- Loading State -->
        <div v-if="isLoading" class="flex justify-center items-center h-64">
          <v-progress-circular
            indeterminate
            color="primary"
            size="48"
          />
        </div>

        <!-- Data Table -->
        <v-data-table
          v-else
          v-model:page="currentPage"
          v-model:items-per-page="itemsPerPage"
          v-model:sort-by="sortBy"
          :headers="tableHeaders"
          :items="filteredItems"
          :loading="isLoading"
          :search="searchQuery"
          class="custom-table"
          density="comfortable"
          hover
          fixed-header
          height="500"
        >
          <!-- Custom slots -->
          <template #item.timestamp="{ item }">
            <div class="text-sm">{{ formatDateTime(item.timestamp) }}</div>
          </template>

          <template #item.drugCode="{ item }">
            <div class="font-mono text-sm font-medium">{{ item.drugCode }}</div>
          </template>

          <template #item.drugName="{ item }">
            <div class="font-medium">{{ item.drugName }}</div>
            <div class="text-xs text-gray-500">{{ item.batchNumber }}</div>
          </template>

          <template #item.transactionType="{ item }">
            <v-chip
              :color="getTransactionColor(item.transactionType)"
              size="small"
              variant="flat"
            >
              <v-icon start size="16">{{ getTransactionIcon(item.transactionType) }}</v-icon>
              {{ item.transactionType }}
            </v-chip>
          </template>

          <template #item.quantity="{ item }">
            <div class="text-right">
              <div 
                class="font-medium"
                :class="getQuantityClass(item.quantity)"
              >
                {{ item.quantity > 0 ? '+' : '' }}{{ formatNumber(item.quantity) }}
              </div>
              <div class="text-xs text-gray-500">{{ item.unit }}</div>
            </div>
          </template>

          <template #item.performedBy="{ item }">
            <div class="flex items-center gap-2">
              <v-avatar size="24" color="primary" variant="tonal">
                <v-icon size="12">mdi-account</v-icon>
              </v-avatar>
              <span class="text-sm">{{ item.performedBy }}</span>
            </div>
          </template>

          <template #item.actions="{ item }">
            <div class="flex gap-1">
              <v-tooltip text="Xem chi tiết">
                <template #activator="{ props }">
                  <v-btn
                    v-bind="props"
                    icon="mdi-eye"
                    size="small"
                    variant="text"
                    color="primary"
                    @click="viewTransactionDetails(item)"
                  />
                </template>
              </v-tooltip>
            </div>
          </template>

          <!-- No data state -->
          <template #no-data>
            <div class="text-center py-8">
              <v-icon size="64" color="grey-lighten-1">mdi-history</v-icon>
              <div class="text-h6 text-gray-500 mt-4">Không có lịch sử giao dịch</div>
              <div class="text-gray-400">Chưa có giao dịch nào được thực hiện</div>
            </div>
          </template>

          <!-- Loading slot -->
          <template #loading>
            <div class="flex justify-center items-center h-32">
              <v-progress-circular indeterminate color="primary" />
            </div>
          </template>
        </v-data-table>
      </v-card-text>
    </v-card>

    <!-- Detail Dialog -->
    <v-dialog v-model="detailDialog" max-width="700" persistent>
      <v-card rounded="xl">
        <v-card-title class="px-6 py-4 border-b border-gray-100">
          <div class="flex items-center gap-2">
            <v-icon color="primary">mdi-clipboard-text</v-icon>
            <span>Chi tiết giao dịch</span>
          </div>
        </v-card-title>
        
        <v-card-text v-if="selectedTransaction" class="px-6 py-4">
          <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
            <!-- Transaction Info -->
            <div class="space-y-4">
              <div>
                <label class="text-sm font-medium text-gray-600">ID Giao dịch</label>
                <div class="font-mono">{{ selectedTransaction.id }}</div>
              </div>
              <div>
                <label class="text-sm font-medium text-gray-600">Thời gian</label>
                <div>{{ formatDateTime(selectedTransaction.timestamp) }}</div>
              </div>
              <div>
                <label class="text-sm font-medium text-gray-600">Loại giao dịch</label>
                <v-chip
                  :color="getTransactionColor(selectedTransaction.transactionType)"
                  size="small"
                  class="mt-1"
                >
                  {{ selectedTransaction.transactionType }}
                </v-chip>
              </div>
              <div>
                <label class="text-sm font-medium text-gray-600">Người thực hiện</label>
                <div>{{ selectedTransaction.performedBy }}</div>
              </div>
            </div>

            <!-- Drug Info -->
            <div class="space-y-4">
              <div>
                <label class="text-sm font-medium text-gray-600">Mã thuốc</label>
                <div class="font-mono">{{ selectedTransaction.drugCode }}</div>
              </div>
              <div>
                <label class="text-sm font-medium text-gray-600">Tên thuốc</label>
                <div class="font-medium">{{ selectedTransaction.drugName }}</div>
              </div>
              <div>
                <label class="text-sm font-medium text-gray-600">Số lượng</label>
                <div class="text-lg font-bold">
                  {{ selectedTransaction.quantity > 0 ? '+' : '' }}{{ formatNumber(selectedTransaction.quantity) }} {{ selectedTransaction.unit }}
                </div>
              </div>
              <div>
                <label class="text-sm font-medium text-gray-600">Ghi chú</label>
                <div>{{ selectedTransaction.notes || 'Không có ghi chú' }}</div>
              </div>
            </div>
          </div>
        </v-card-text>
        
        <v-card-actions class="px-6 pb-6">
          <v-spacer />
          <v-btn variant="text" @click="detailDialog = false">
            Đóng
          </v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>

    <!-- Success Snackbar -->
    <v-snackbar
      v-model="showSnackbar"
      color="success"
      timeout="3000"
      location="bottom right"
    >
      <div class="flex items-center gap-2">
        <v-icon>mdi-check-circle</v-icon>
        <span>{{ snackbarMessage }}</span>
      </div>
      <template #actions>
        <v-btn icon="mdi-close" variant="text" @click="showSnackbar = false" />
      </template>
    </v-snackbar>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'

// Types
interface Transaction {
  id: string
  timestamp: string
  drugCode: string
  drugName: string
  batchNumber: string
  transactionType: string
  quantity: number
  unit: string
  performedBy: string
  notes?: string
}

interface SummaryStat {
  key: string
  label: string
  value: string | number
  color: string
  icon: string
}

// Reactive state
const searchQuery = ref<string>('')
const typeFilter = ref<string>('')
const userFilter = ref<string>('')
const quickFilter = ref<string>('all')
const dateMenu = ref<boolean>(false)
const dateRange = ref<Date[]>([])
const isLoading = ref<boolean>(false)
const currentPage = ref<number>(1)
const itemsPerPage = ref<number>(10)
const sortBy = ref([{ key: 'timestamp', order: 'desc' as const }])

// Dialog states
const detailDialog = ref<boolean>(false)
const selectedTransaction = ref<Transaction | null>(null)

// Snackbar
const showSnackbar = ref<boolean>(false)
const snackbarMessage = ref<string>('')

// Sample data - In real app, this would come from API/Pinia store
const transactions = ref<Transaction[]>([
  {
    id: 'TX001',
    timestamp: '2024-12-15T17:00:00Z',
    drugCode: 'TH001',
    drugName: 'Paracetamol 500mg',
    batchNumber: 'LOT2024001',
    transactionType: 'Nhập kho',
    quantity: 100,
    unit: 'viên',
    performedBy: 'Admin',
    notes: 'Nhập hàng từ nhà cung cấp ABC'
  },
  {
    id: 'TX002',
    timestamp: '2024-12-15T10:21:00Z',
    drugCode: 'TH002',
    drugName: 'Vitamin C 1000mg',
    batchNumber: 'LOT2024002',
    transactionType: 'Điều chỉnh',
    quantity: -5,
    unit: 'viên',
    performedBy: 'Thủ kho',
    notes: 'Điều chỉnh do kiểm kê'
  },
  {
    id: 'TX003',
    timestamp: '2024-12-14T15:43:00Z',
    drugCode: 'TH001',
    drugName: 'Paracetamol 500mg',
    batchNumber: 'LOT2024001',
    transactionType: 'Xuất kho',
    quantity: -20,
    unit: 'viên',
    performedBy: 'Nhân viên bán hàng',
    notes: 'Bán lẻ cho khách hàng'
  }
])

// Table configuration
const tableHeaders = [
  { title: 'Thời gian', key: 'timestamp', sortable: true },
  { title: 'Mã thuốc', key: 'drugCode', sortable: true },
  { title: 'Tên thuốc', key: 'drugName', sortable: true },
  { title: 'Loại giao dịch', key: 'transactionType', sortable: true },
  { title: 'Số lượng', key: 'quantity', sortable: true },
  { title: 'Người thực hiện', key: 'performedBy', sortable: true },
  { title: 'Thao tác', key: 'actions', sortable: false, width: 80 }
]

// Filter options
const transactionTypes = [
  { title: 'Tất cả', value: '' },
  { title: 'Nhập kho', value: 'Nhập kho' },
  { title: 'Xuất kho', value: 'Xuất kho' },
  { title: 'Điều chỉnh', value: 'Điều chỉnh' },
  { title: 'Hủy thuốc', value: 'Hủy thuốc' }
]

const userOptions = [
  { title: 'Tất cả người dùng', value: '' },
  { title: 'Admin', value: 'Admin' },
  { title: 'Thủ kho', value: 'Thủ kho' },
  { title: 'Nhân viên bán hàng', value: 'Nhân viên bán hàng' }
]

// Computed properties
const filteredItems = computed(() => {
  let filtered = transactions.value

  // Quick filter
  if (quickFilter.value !== 'all') {
    const typeMap = {
      'import': 'Nhập kho',
      'export': 'Xuất kho', 
      'adjust': 'Điều chỉnh'
    }
    const targetType = typeMap[quickFilter.value as keyof typeof typeMap]
    if (targetType) {
      filtered = filtered.filter(item => item.transactionType === targetType)
    }
  }

  // Type filter
  if (typeFilter.value) {
    filtered = filtered.filter(item => item.transactionType === typeFilter.value)
  }

  // User filter
  if (userFilter.value) {
    filtered = filtered.filter(item => item.performedBy === userFilter.value)
  }

  return filtered
})

const summaryStats = computed<SummaryStat[]>(() => {
  return [
    {
      key: 'total',
      label: 'Tổng giao dịch',
      value: totalTransactions.value,
      color: 'primary',
      icon: 'mdi-swap-vertical'
    },
    {
      key: 'import',
      label: 'Nhập kho',
      value: importCount.value,
      color: 'success',
      icon: 'mdi-arrow-down-circle'
    },
    {
      key: 'export',
      label: 'Xuất kho',
      value: exportCount.value,
      color: 'info',
      icon: 'mdi-arrow-up-circle'
    },
    {
      key: 'adjust',
      label: 'Điều chỉnh',
      value: adjustCount.value,
      color: 'warning',
      icon: 'mdi-tune'
    }
  ]
})

const totalTransactions = computed(() => transactions.value.length)
const importCount = computed(() => transactions.value.filter(item => item.transactionType === 'Nhập kho').length)
const exportCount = computed(() => transactions.value.filter(item => item.transactionType === 'Xuất kho').length)
const adjustCount = computed(() => transactions.value.filter(item => item.transactionType === 'Điều chỉnh').length)

const dateRangeText = computed(() => {
  if (!dateRange.value || dateRange.value.length === 0) return ''
  if (dateRange.value.length === 1) {
    return formatDate(dateRange.value[0].toISOString())
  }
  return `${formatDate(dateRange.value[0].toISOString())} - ${formatDate(dateRange.value[1].toISOString())}`
})

// Methods
const getTransactionColor = (type: string): string => {
  const colors = {
    'Nhập kho': 'success',
    'Xuất kho': 'info',
    'Điều chỉnh': 'warning',
    'Hủy thuốc': 'error'
  }
  return colors[type as keyof typeof colors] || 'grey'
}

const getTransactionIcon = (type: string): string => {
  const icons = {
    'Nhập kho': 'mdi-arrow-down-circle',
    'Xuất kho': 'mdi-arrow-up-circle',
    'Điều chỉnh': 'mdi-tune',
    'Hủy thuốc': 'mdi-delete-circle'
  }
  return icons[type as keyof typeof icons] || 'mdi-help-circle'
}

const getQuantityClass = (quantity: number): string => {
  if (quantity > 0) return 'text-success'
  if (quantity < 0) return 'text-error'
  return 'text-grey'
}

const setQuickFilter = (filter: string): void => {
  quickFilter.value = filter
  if (filter !== 'all') {
    const typeMap = {
      'import': 'Nhập kho',
      'export': 'Xuất kho',
      'adjust': 'Điều chỉnh'
    }
    typeFilter.value = typeMap[filter as keyof typeof typeMap] || ''
  } else {
    typeFilter.value = ''
  }
}

const refreshData = async (): Promise<void> => {
  isLoading.value = true
  try {
    // Simulate API call
    await new Promise(resolve => setTimeout(resolve, 1000))
    showSuccess('Dữ liệu đã được cập nhật')
  } finally {
    isLoading.value = false
  }
}

const exportHistory = (): void => {
  // Export logic here
  showSuccess('Đang xuất báo cáo lịch sử giao dịch...')
}

const viewTransactionDetails = (transaction: Transaction): void => {
  selectedTransaction.value = transaction
  detailDialog.value = true
}

const formatNumber = (value: number): string => {
  return new Intl.NumberFormat('vi-VN').format(value)
}

const formatDate = (dateString: string): string => {
  return new Date(dateString).toLocaleDateString('vi-VN')
}

const formatDateTime = (dateString: string): string => {
  return new Date(dateString).toLocaleString('vi-VN')
}

const showSuccess = (message: string): void => {
  snackbarMessage.value = message
  showSnackbar.value = true
}

// Lifecycle
onMounted(() => {
  refreshData()
})
</script>

<style scoped>
.stock-history-container {
  @apply space-y-6;
}

.filter-card {
  @apply border border-gray-100 shadow-sm;
  border-radius: 12px;
}

.stat-card {
  @apply border border-gray-100 shadow-sm transition-shadow duration-200;
  border-radius: 12px;
}

.stat-card:hover {
  @apply shadow-md;
}

.main-table-card {
  @apply border border-gray-100 shadow-sm;
  border-radius: 12px;
}

.custom-table :deep(.v-data-table__wrapper) {
  border-radius: 0 0 12px 12px;
}

.custom-table :deep(th) {
  @apply bg-gray-50/50 font-semibold;
}

/* Responsive adjustments */
@media (max-width: 768px) {
  .stock-history-container {
    @apply space-y-4;
  }
  
  .custom-table {
    font-size: 0.875rem;
  }
}

/* Custom scrollbar */
:deep(.v-data-table__wrapper::-webkit-scrollbar) {
  width: 6px;
  height: 6px;
}

:deep(.v-data-table__wrapper::-webkit-scrollbar-track) {
  @apply bg-gray-100 rounded-full;
}

:deep(.v-data-table__wrapper::-webkit-scrollbar-thumb) {
  @apply bg-gray-300 rounded-full hover:bg-gray-400;
}
</style>