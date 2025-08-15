<template>
  <div class="expired-warning-container">
    <!-- Header với action buttons -->
    <div class="flex items-center justify-between mb-6">
      <div class="flex items-center gap-3">
        <v-icon color="error" size="24">mdi-calendar-alert</v-icon>
        <div>
          <h2 class="text-xl font-bold text-gray-800">Cảnh báo hết hạn</h2>
          <p class="text-gray-500 text-sm">Theo dõi các thuốc sắp hết hạn hoặc đã hết hạn</p>
        </div>
      </div>

      <!-- Action Buttons -->
      <div class="flex gap-2">
        <v-btn
          variant="outlined"
          color="warning"
          size="small"
          prepend-icon="mdi-refresh"
          @click="refreshData"
        >
          Làm mới
        </v-btn>
        <v-btn
          variant="outlined"
          color="error"
          size="small"
          prepend-icon="mdi-delete-sweep"
          @click="handleExpiredItems"
        >
          Xử lý hết hạn
        </v-btn>
        <v-btn
          variant="outlined"
          color="primary"
          size="small"
          prepend-icon="mdi-download"
          @click="exportReport"
        >
          Xuất báo cáo
        </v-btn>
      </div>
    </div>

    <!-- Filter và Search Section -->
    <v-card class="mb-6" elevation="0">
      <v-card-text class="pb-4">
        <div class="flex flex-wrap gap-4 items-center">
          <!-- Search -->
          <v-text-field
            v-model="searchQuery"
            label="Tìm kiếm thuốc..."
            prepend-inner-icon="mdi-magnify"
            variant="outlined"
            density="compact"
            hide-details
            clearable
            class="max-w-xs"
          />

          <!-- Status Filter -->
          <v-select
            v-model="statusFilter"
            :items="statusOptions"
            label="Trạng thái"
            variant="outlined"
            density="compact"
            hide-details
            class="max-w-40"
          />

          <!-- Date Range Filter -->
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
                density="compact"
                hide-details
                readonly
                class="max-w-48"
              />
            </template>
            <v-date-picker
              v-model="dateRange"
              range
              @update:model-value="dateMenu = false"
            />
          </v-menu>

          <!-- Quick Filters -->
          <div class="flex gap-2">
            <v-chip
              :variant="quickFilter === 'expired' ? 'flat' : 'outlined'"
              :color="quickFilter === 'expired' ? 'error' : 'default'"
              size="small"
              clickable
              @click="setQuickFilter('expired')"
            >
              Đã hết hạn
            </v-chip>
            <v-chip
              :variant="quickFilter === 'expiring_soon' ? 'flat' : 'outlined'"
              :color="quickFilter === 'expiring_soon' ? 'warning' : 'default'"
              size="small"
              clickable
              @click="setQuickFilter('expiring_soon')"
            >
              Sắp hết hạn
            </v-chip>
            <v-chip
              :variant="quickFilter === 'all' ? 'flat' : 'outlined'"
              :color="quickFilter === 'all' ? 'primary' : 'default'"
              size="small"
              clickable
              @click="setQuickFilter('all')"
            >
              Tất cả
            </v-chip>
          </div>
        </div>
      </v-card-text>
    </v-card>

    <!-- Summary Cards -->

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
          <template #item.drugCode="{ item }">
            <div class="font-mono text-sm">{{ item.drugCode }}</div>
          </template>

          <template #item.drugName="{ item }">
            <div class="font-medium">{{ item.drugName }}</div>
            <div class="text-xs text-gray-500">{{ item.genericName }}</div>
          </template>

          <template #item.batchNumber="{ item }">
            <v-chip size="x-small" variant="outlined">
              {{ item.batchNumber }}
            </v-chip>
          </template>

          <template #item.expiryDate="{ item }">
            <div class="flex items-center gap-2">
              <v-chip
                :color="getExpiryStatus(item.expiryDate).color"
                :variant="getExpiryStatus(item.expiryDate).variant"
                size="small"
              >
                <v-icon start size="16">{{ getExpiryStatus(item.expiryDate).icon }}</v-icon>
                {{ formatDate(item.expiryDate) }}
              </v-chip>
            </div>
          </template>

          <template #item.currentStock="{ item }">
            <div class="text-right font-medium">
              {{ formatNumber(item.currentStock) }}
              <div class="text-xs text-gray-500">{{ item.unit }}</div>
            </div>
          </template>

          <template #item.supplierName="{ item }">
            <div class="text-sm">{{ item.supplierName }}</div>
          </template>

          <template #item.daysToExpiry="{ item }">
            <v-chip
              :color="getDaysToExpiryColor(item.daysToExpiry)"
              size="small"
              variant="flat"
            >
              {{ item.daysToExpiry > 0 ? `${item.daysToExpiry} ngày` : 'Đã hết hạn' }}
            </v-chip>
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
                    @click="viewDetails(item)"
                  />
                </template>
              </v-tooltip>
              
              <v-tooltip text="Xử lý hết hạn">
                <template #activator="{ props }">
                  <v-btn
                    v-bind="props"
                    icon="mdi-delete"
                    size="small"
                    variant="text"
                    color="error"
                    @click="handleSingleExpired(item)"
                  />
                </template>
              </v-tooltip>
            </div>
          </template>

          <!-- No data state -->
          <template #no-data>
            <div class="text-center py-8">
              <v-icon size="64" color="grey-lighten-1">mdi-calendar-check</v-icon>
              <div class="text-h6 text-gray-500 mt-4">Không có thuốc nào hết hạn</div>
              <div class="text-gray-400">Tất cả thuốc đều còn trong thời hạn sử dụng</div>
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
    <v-dialog v-model="detailDialog" max-width="800" persistent>
      <v-card rounded="xl">
        <v-card-title class="px-6 py-4 border-b border-gray-100">
          <div class="flex items-center gap-2">
            <v-icon color="primary">mdi-pill</v-icon>
            <span>Chi tiết thuốc</span>
          </div>
        </v-card-title>
        
        <v-card-text v-if="selectedItem" class="px-6 py-4">
          <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
            <!-- Basic Info -->
            <div class="space-y-4">
              <div>
                <label class="text-sm font-medium text-gray-600">Mã thuốc</label>
                <div class="font-mono">{{ selectedItem.drugCode }}</div>
              </div>
              <div>
                <label class="text-sm font-medium text-gray-600">Tên thuốc</label>
                <div class="font-medium">{{ selectedItem.drugName }}</div>
              </div>
              <div>
                <label class="text-sm font-medium text-gray-600">Tên gốc</label>
                <div>{{ selectedItem.genericName }}</div>
              </div>
              <div>
                <label class="text-sm font-medium text-gray-600">Số lô</label>
                <div>{{ selectedItem.batchNumber }}</div>
              </div>
            </div>

            <!-- Expiry Info -->
            <div class="space-y-4">
              <div>
                <label class="text-sm font-medium text-gray-600">Hạn sử dụng</label>
                <div class="flex items-center gap-2">
                  <v-chip
                    :color="getExpiryStatus(selectedItem.expiryDate).color"
                    size="small"
                  >
                    {{ formatDate(selectedItem.expiryDate) }}
                  </v-chip>
                </div>
              </div>
              <div>
                <label class="text-sm font-medium text-gray-600">Số ngày còn lại</label>
                <div class="font-medium">
                  {{ selectedItem.daysToExpiry > 0 ? `${selectedItem.daysToExpiry} ngày` : 'Đã hết hạn' }}
                </div>
              </div>
              <div>
                <label class="text-sm font-medium text-gray-600">Tồn kho hiện tại</label>
                <div class="font-medium">{{ formatNumber(selectedItem.currentStock) }} {{ selectedItem.unit }}</div>
              </div>
              <div>
                <label class="text-sm font-medium text-gray-600">Nhà cung cấp</label>
                <div>{{ selectedItem.supplierName }}</div>
              </div>
            </div>
          </div>
        </v-card-text>
        
        <v-card-actions class="px-6 pb-6">
          <v-spacer />
          <v-btn
            variant="text"
            @click="detailDialog = false"
          >
            Đóng
          </v-btn>
          <v-btn
            color="error"
            variant="outlined"
            prepend-icon="mdi-delete"
            @click="handleSingleExpired(selectedItem!)"
          >
            Xử lý hết hạn
          </v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>

    <!-- Confirmation Dialog -->
    <v-dialog v-model="confirmDialog" max-width="500" persistent>
      <v-card rounded="xl">
        <v-card-title class="px-6 py-4 border-b border-gray-100">
          <v-icon color="warning" class="mr-2">mdi-alert</v-icon>
          <span>Xác nhận xử lý</span>
        </v-card-title>
        
        <v-card-text class="px-6 py-4">
          <p>Bạn có chắc chắn muốn xử lý các thuốc hết hạn đã chọn?</p>
          <p class="text-sm text-gray-500 mt-2">Hành động này không thể hoàn tác.</p>
        </v-card-text>
        
        <v-card-actions class="px-6 pb-6">
          <v-spacer />
          <v-btn variant="text" @click="confirmDialog = false">
            Hủy
          </v-btn>
          <v-btn
            color="error"
            variant="flat"
            @click="confirmExpiredAction"
          >
            Xác nhận
          </v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>

    <!-- Success Snackbar -->
    <v-snackbar
      v-model="showSuccessMessage"
      color="success"
      timeout="3000"
      location="bottom right"
    >
      {{ successMessage }}
      <template #actions>
        <v-btn icon="mdi-close" variant="text" @click="showSuccessMessage = false" />
      </template>
    </v-snackbar>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, watch } from 'vue'

// Types
interface ExpiredDrug {
  id: string
  drugCode: string
  drugName: string
  genericName: string
  batchNumber: string
  expiryDate: string
  currentStock: number
  unit: string
  supplierName: string
  daysToExpiry: number
  status: 'expired' | 'expiring_soon' | 'valid'
}

interface SummaryStat {
  key: string
  label: string
  value: number | string
  color: string
  icon: string
}

// Reactive state
const isLoading = ref<boolean>(false)
const searchQuery = ref<string>('')
const statusFilter = ref<string>('all')
const quickFilter = ref<string>('all')
const dateMenu = ref<boolean>(false)
const dateRange = ref<Date[]>([])
const currentPage = ref<number>(1)
const itemsPerPage = ref<number>(10)
const sortBy = ref([{ key: 'daysToExpiry', order: 'asc' as const }])

// Dialog states
const detailDialog = ref<boolean>(false)
const confirmDialog = ref<boolean>(false)
const selectedItem = ref<ExpiredDrug | null>(null)

// Success message
const showSuccessMessage = ref<boolean>(false)
const successMessage = ref<string>('')

// Sample data - In real app, this would come from API/store
const expiredDrugs = ref<ExpiredDrug[]>([
  {
    id: '1',
    drugCode: 'TH001',
    drugName: 'Paracetamol 500mg',
    genericName: 'Acetaminophen',
    batchNumber: 'LOT2024001',
    expiryDate: '2024-12-15',
    currentStock: 150,
    unit: 'viên',
    supplierName: 'Công ty Dược phẩm ABC',
    daysToExpiry: -30,
    status: 'expired'
  },
  {
    id: '2',
    drugCode: 'TH002',
    drugName: 'Amoxicillin 250mg',
    genericName: 'Amoxicillin trihydrate',
    batchNumber: 'LOT2024002',
    expiryDate: '2025-01-10',
    currentStock: 75,
    unit: 'viên',
    supplierName: 'Công ty Dược phẩm XYZ',
    daysToExpiry: 15,
    status: 'expiring_soon'
  },
  {
    id: '3',
    drugCode: 'TH003',
    drugName: 'Ibuprofen 400mg',
    genericName: 'Ibuprofen',
    batchNumber: 'LOT2024003',
    expiryDate: '2025-01-20',
    currentStock: 200,
    unit: 'viên',
    supplierName: 'Công ty Dược phẩm DEF',
    daysToExpiry: 25,
    status: 'expiring_soon'
  }
])

// Table configuration
const tableHeaders = [
  { title: 'Mã thuốc', key: 'drugCode', sortable: true },
  { title: 'Tên thuốc', key: 'drugName', sortable: true },
  { title: 'Số lô', key: 'batchNumber', sortable: true },
  { title: 'Hạn sử dụng', key: 'expiryDate', sortable: true },
  { title: 'Tồn kho', key: 'currentStock', sortable: true },
  { title: 'Nhà cung cấp', key: 'supplierName', sortable: true },
  { title: 'Còn lại', key: 'daysToExpiry', sortable: true },
  { title: 'Thao tác', key: 'actions', sortable: false, width: 120 }
]

const statusOptions = [
  { title: 'Tất cả', value: 'all' },
  { title: 'Đã hết hạn', value: 'expired' },
  { title: 'Sắp hết hạn', value: 'expiring_soon' },
  { title: 'Còn hiệu lực', value: 'valid' }
]

// Computed properties
const filteredItems = computed(() => {
  let filtered = expiredDrugs.value

  // Quick filter
  if (quickFilter.value !== 'all') {
    filtered = filtered.filter(item => item.status === quickFilter.value)
  }

  // Status filter
  if (statusFilter.value !== 'all') {
    filtered = filtered.filter(item => item.status === statusFilter.value)
  }

  return filtered
})

const summaryStats = computed<SummaryStat[]>(() => {
  const expired = expiredDrugs.value.filter(item => item.status === 'expired')
  const expiringSoon = expiredDrugs.value.filter(item => item.status === 'expiring_soon')
  const totalValue = expiredDrugs.value.reduce((sum, item) => sum + item.currentStock, 0)

  return [
    {
      key: 'total',
      label: 'Tổng sản phẩm',
      value: expiredDrugs.value.length,
      color: 'primary',
      icon: 'mdi-pill'
    },
    {
      key: 'expired',
      label: 'Đã hết hạn',
      value: expired.length,
      color: 'error',
      icon: 'mdi-calendar-remove'
    },
    {
      key: 'expiring_soon',
      label: 'Sắp hết hạn',
      value: expiringSoon.length,
      color: 'warning',
      icon: 'mdi-calendar-alert'
    },
    {
      key: 'total_quantity',
      label: 'Tổng số lượng',
      value: formatNumber(totalValue),
      color: 'info',
      icon: 'mdi-package-variant'
    }
  ]
})

const dateRangeText = computed(() => {
  if (!dateRange.value || dateRange.value.length === 0) return ''
  if (dateRange.value.length === 1) {
    return formatDate(dateRange.value[0].toISOString())
  }
  return `${formatDate(dateRange.value[0].toISOString())} - ${formatDate(dateRange.value[1].toISOString())}`
})

// Methods
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

const handleExpiredItems = (): void => {
  const expiredItems = expiredDrugs.value.filter(item => item.status === 'expired')
  if (expiredItems.length === 0) {
    showSuccess('Không có thuốc hết hạn nào cần xử lý')
    return
  }
  confirmDialog.value = true
}

const handleSingleExpired = (item: ExpiredDrug): void => {
  selectedItem.value = item
  confirmDialog.value = true
}

const confirmExpiredAction = (): void => {
  // Handle expired drug removal logic here
  if (selectedItem.value) {
    // Remove single item
    const index = expiredDrugs.value.findIndex(drug => drug.id === selectedItem.value!.id)
    if (index !== -1) {
      expiredDrugs.value.splice(index, 1)
    }
    showSuccess(`Đã xử lý thuốc ${selectedItem.value.drugName}`)
  } else {
    // Remove all expired items
    const expiredCount = expiredDrugs.value.filter(item => item.status === 'expired').length
    expiredDrugs.value = expiredDrugs.value.filter(item => item.status !== 'expired')
    showSuccess(`Đã xử lý ${expiredCount} thuốc hết hạn`)
  }
  
  confirmDialog.value = false
  detailDialog.value = false
  selectedItem.value = null
}

const exportReport = (): void => {
  // Export logic here
  showSuccess('Đang xuất báo cáo...')
}

const viewDetails = (item: ExpiredDrug): void => {
  selectedItem.value = item
  detailDialog.value = true
}

const setQuickFilter = (filter: string): void => {
  quickFilter.value = filter
  statusFilter.value = filter === 'all' ? 'all' : filter
}

const getExpiryStatus = (expiryDate: string) => {
  const today = new Date()
  const expiry = new Date(expiryDate)
  const daysToExpiry = Math.ceil((expiry.getTime() - today.getTime()) / (1000 * 60 * 60 * 24))

  if (daysToExpiry < 0) {
    return { color: 'error', variant: 'flat' as const, icon: 'mdi-calendar-remove' }
  } else if (daysToExpiry <= 30) {
    return { color: 'warning', variant: 'flat' as const, icon: 'mdi-calendar-alert' }
  } else {
    return { color: 'success', variant: 'outlined' as const, icon: 'mdi-calendar-check' }
  }
}

const getDaysToExpiryColor = (days: number): string => {
  if (days < 0) return 'error'
  if (days <= 7) return 'error'
  if (days <= 30) return 'warning'
  return 'success'
}

const formatDate = (dateString: string): string => {
  return new Date(dateString).toLocaleDateString('vi-VN')
}

const formatNumber = (value: number): string => {
  return new Intl.NumberFormat('vi-VN').format(value)
}

const showSuccess = (message: string): void => {
  successMessage.value = message
  showSuccessMessage.value = true
}

// Lifecycle
onMounted(() => {
  refreshData()
})

// Watchers
watch([searchQuery, statusFilter, quickFilter], () => {
  currentPage.value = 1
}, { deep: true })
</script>

<style scoped>
.expired-warning-container {
  @apply space-y-6;
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

.custom-table :deep(.v-data-table-rows-no-data) {
  @apply text-center py-8;
}

/* Responsive adjustments */
@media (max-width: 768px) {
  .expired-warning-container {
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