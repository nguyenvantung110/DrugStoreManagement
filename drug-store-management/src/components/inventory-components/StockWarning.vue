<template>
  <div class="stock-warning-container">
    <!-- Header Section -->
    <div class="flex items-center justify-between mb-6">
      <div class="flex items-center gap-3">
        <v-icon color="error" size="24">mdi-alert-decagram</v-icon>
        <div>
          <h2 class="text-xl font-bold text-gray-800">Cảnh báo tồn kho</h2>
          <p class="text-gray-500 text-sm">Theo dõi các sản phẩm có số lượng tồn kho thấp cần được nhập thêm</p>
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
          prepend-icon="mdi-check-all"
          @click="markAllResolved"
        >
          Đánh dấu đã xử lý
        </v-btn>
        <v-btn
          variant="outlined"
          color="warning"
          size="small"
          prepend-icon="mdi-download"
          @click="exportWarnings"
        >
          Xuất cảnh báo
        </v-btn>
      </div>
    </div>

    <!-- Alert Summary -->
    <v-alert
      type="warning"
      variant="tonal"
      class="mb-6"
      rounded="lg"
    >
      <template #prepend>
        <v-icon>mdi-alert-circle</v-icon>
      </template>
      <div class="font-medium">Cảnh báo tồn kho thấp</div>
      <div class="text-sm mt-1">
        Hiện có <strong>{{ criticalCount }}</strong> sản phẩm ở mức tồn kho nguy hiểm và 
        <strong>{{ warningCount }}</strong> sản phẩm ở mức cảnh báo cần được nhập thêm.
      </div>
    </v-alert>

    <!-- Filter Section -->
    <v-card class="mb-6 filter-card" elevation="0">
      <v-card-text class="pb-4">
        <div class="grid grid-cols-1 md:grid-cols-3 gap-4 items-center">
          <!-- Search Input -->
          <v-text-field
            v-model="searchQuery"
            label="Tìm kiếm sản phẩm..."
            prepend-inner-icon="mdi-magnify"
            variant="outlined"
            density="comfortable"
            hide-details
            clearable
            placeholder="Nhập tên hoặc mã sản phẩm..."
          />

          <!-- Warning Level Filter -->
          <v-select
            v-model="levelFilter"
            :items="warningLevels"
            label="Mức độ cảnh báo"
            prepend-inner-icon="mdi-alert-circle"
            variant="outlined"
            density="comfortable"
            hide-details
            clearable
          />

          <!-- Category Filter -->
          <v-select
            v-model="categoryFilter"
            :items="categoryOptions"
            label="Danh mục sản phẩm"
            prepend-inner-icon="mdi-tag"
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
            Tất cả ({{ totalWarnings }})
          </v-chip>
          <v-chip
            :variant="quickFilter === 'critical' ? 'flat' : 'outlined'"
            :color="quickFilter === 'critical' ? 'error' : 'default'"
            size="small"
            clickable
            @click="setQuickFilter('critical')"
          >
            Nguy hiểm ({{ criticalCount }})
          </v-chip>
          <v-chip
            :variant="quickFilter === 'warning' ? 'flat' : 'outlined'"
            :color="quickFilter === 'warning' ? 'warning' : 'default'"
            size="small"
            clickable
            @click="setQuickFilter('warning')"
          >
            Cảnh báo ({{ warningCount }})
          </v-chip>
          <v-chip
            :variant="quickFilter === 'low' ? 'flat' : 'outlined'"
            :color="quickFilter === 'low' ? 'info' : 'default'"
            size="small"
            clickable
            @click="setQuickFilter('low')"
          >
            Thấp ({{ lowCount }})
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
          <template #item.code="{ item }">
            <div class="font-mono text-sm font-medium">{{ item.code }}</div>
          </template>

          <template #item.name="{ item }">
            <div class="font-medium">{{ item.name }}</div>
            <div class="text-xs text-gray-500">{{ item.category }}</div>
          </template>

          <template #item.currentStock="{ item }">
            <div class="text-right">
              <div class="font-medium" :class="getStockClass(item)">
                {{ formatNumber(item.currentStock) }}
              </div>
              <div class="text-xs text-gray-500">{{ item.unit }}</div>
            </div>
          </template>

          <template #item.minStock="{ item }">
            <div class="text-right font-medium">
              {{ formatNumber(item.minStock) }}
            </div>
          </template>

          <template #item.warningLevel="{ item }">
            <v-chip
              :color="getWarningColor(item.warningLevel)"
              :variant="getWarningVariant(item.warningLevel)"
              size="small"
            >
              <v-icon start size="16">{{ getWarningIcon(item.warningLevel) }}</v-icon>
              {{ getWarningText(item.warningLevel) }}
            </v-chip>
          </template>

          <template #item.shortage="{ item }">
            <div class="text-right">
              <div class="font-medium text-error">
                {{ formatNumber(Math.max(0, item.minStock - item.currentStock)) }}
              </div>
              <div class="text-xs text-gray-500">{{ item.unit }}</div>
            </div>
          </template>

          <template #item.lastUpdated="{ item }">
            <div class="text-sm">{{ formatDateTime(item.lastUpdated) }}</div>
          </template>

          <template #item.actions="{ item }">
            <div class="flex gap-1">
              <v-tooltip text="Nhập hàng">
                <template #activator="{ props }">
                  <v-btn
                    v-bind="props"
                    icon="mdi-plus-circle"
                    size="small"
                    variant="text"
                    color="success"
                    @click="addStock(item)"
                  />
                </template>
              </v-tooltip>
              
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
              
              <v-tooltip text="Đánh dấu đã xử lý">
                <template #activator="{ props }">
                  <v-btn
                    v-bind="props"
                    icon="mdi-check"
                    size="small"
                    variant="text"
                    color="warning"
                    @click="markResolved(item)"
                  />
                </template>
              </v-tooltip>
            </div>
          </template>

          <!-- No data state -->
          <template #no-data>
            <div class="text-center py-8">
              <v-icon size="64" color="success">mdi-check-circle</v-icon>
              <div class="text-h6 text-gray-500 mt-4">Không có cảnh báo tồn kho</div>
              <div class="text-gray-400">Tất cả sản phẩm đều có tồn kho ở mức an toàn</div>
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
            <span>Chi tiết cảnh báo tồn kho</span>
          </div>
        </v-card-title>
        
        <v-card-text v-if="selectedItem" class="px-6 py-4">
          <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
            <!-- Basic Info -->
            <div class="space-y-4">
              <div>
                <label class="text-sm font-medium text-gray-600">Mã sản phẩm</label>
                <div class="font-mono font-medium">{{ selectedItem.code }}</div>
              </div>
              <div>
                <label class="text-sm font-medium text-gray-600">Tên sản phẩm</label>
                <div class="font-medium">{{ selectedItem.name }}</div>
              </div>
              <div>
                <label class="text-sm font-medium text-gray-600">Danh mục</label>
                <div>{{ selectedItem.category }}</div>
              </div>
              <div>
                <label class="text-sm font-medium text-gray-600">Mức độ cảnh báo</label>
                <v-chip
                  :color="getWarningColor(selectedItem.warningLevel)"
                  size="small"
                  class="mt-1"
                >
                  {{ getWarningText(selectedItem.warningLevel) }}
                </v-chip>
              </div>
            </div>

            <!-- Stock Info -->
            <div class="space-y-4">
              <div>
                <label class="text-sm font-medium text-gray-600">Tồn kho hiện tại</label>
                <div class="text-xl font-bold" :class="getStockClass(selectedItem)">
                  {{ formatNumber(selectedItem.currentStock) }} {{ selectedItem.unit }}
                </div>
              </div>
              <div>
                <label class="text-sm font-medium text-gray-600">Tồn kho tối thiểu</label>
                <div class="font-medium">{{ formatNumber(selectedItem.minStock) }} {{ selectedItem.unit }}</div>
              </div>
              <div>
                <label class="text-sm font-medium text-gray-600">Thiếu hụt</label>
                <div class="text-xl font-bold text-error">
                  {{ formatNumber(Math.max(0, selectedItem.minStock - selectedItem.currentStock)) }} {{ selectedItem.unit }}
                </div>
              </div>
              <div>
                <label class="text-sm font-medium text-gray-600">Cập nhật lần cuối</label>
                <div>{{ formatDateTime(selectedItem.lastUpdated) }}</div>
              </div>
            </div>
          </div>

          <!-- Action Recommendations -->
          <div class="mt-6 p-4 bg-gray-50 rounded-lg">
            <h4 class="font-medium text-gray-800 mb-2">Khuyến nghị hành động</h4>
            <ul class="text-sm text-gray-600 space-y-1">
              <li>• Liên hệ nhà cung cấp để đặt hàng bổ sung</li>
              <li>• Kiểm tra lại số lượng thực tế trong kho</li>
              <li>• Xem xét điều chỉnh mức tồn kho tối thiểu</li>
              <li>• Theo dõi tốc độ tiêu thụ để dự báo nhu cầu</li>
            </ul>
          </div>
        </v-card-text>
        
        <v-card-actions class="px-6 pb-6">
          <v-spacer />
          <v-btn variant="text" @click="detailDialog = false">
            Đóng
          </v-btn>
          <v-btn
            color="success"
            variant="outlined"
            prepend-icon="mdi-plus-circle"
            @click="addStock(selectedItem!)"
          >
            Nhập hàng
          </v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>

    <!-- Bulk Action Dialog -->
    <v-dialog v-model="bulkActionDialog" max-width="500" persistent>
      <v-card rounded="xl">
        <v-card-title class="px-6 py-4 border-b border-gray-100">
          <v-icon color="success" class="mr-2">mdi-check-all</v-icon>
          <span>Đánh dấu tất cả đã xử lý</span>
        </v-card-title>
        
        <v-card-text class="px-6 py-4">
          <div class="space-y-3">
            <p>Bạn có chắc chắn muốn đánh dấu tất cả <strong>{{ filteredItems.length }}</strong> cảnh báo là đã xử lý?</p>
            <p class="text-sm text-gray-500">Hành động này sẽ ẩn các cảnh báo khỏi danh sách hiện tại.</p>
          </div>
        </v-card-text>
        
        <v-card-actions class="px-6 pb-6">
          <v-spacer />
          <v-btn variant="text" @click="bulkActionDialog = false">
            Hủy bỏ
          </v-btn>
          <v-btn
            color="success"
            variant="flat"
            :loading="isProcessing"
            @click="confirmBulkAction"
          >
            Xác nhận
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
interface WarningItem {
  id: string
  code: string
  name: string
  category: string
  currentStock: number
  minStock: number
  unit: string
  warningLevel: 'critical' | 'warning' | 'low'
  lastUpdated: string
  supplierName?: string
  location?: string
}

interface SummaryStat {
  key: string
  label: string
  value: number
  color: string
  icon: string
}

// Reactive state
const searchQuery = ref<string>('')
const levelFilter = ref<string>('')
const categoryFilter = ref<string>('')
const quickFilter = ref<string>('all')
const isLoading = ref<boolean>(false)
const isProcessing = ref<boolean>(false)
const currentPage = ref<number>(1)
const itemsPerPage = ref<number>(10)
const sortBy = ref([{ key: 'warningLevel', order: 'asc' as const }])

// Dialog states
const detailDialog = ref<boolean>(false)
const bulkActionDialog = ref<boolean>(false)
const selectedItem = ref<WarningItem | null>(null)

// Snackbar
const showSnackbar = ref<boolean>(false)
const snackbarMessage = ref<string>('')

// Sample data - In real app, this would come from API/Pinia store
const warningItems = ref<WarningItem[]>([
  {
    id: '1',
    code: 'TH001',
    name: 'Paracetamol 500mg',
    category: 'Thuốc giảm đau',
    currentStock: 5,
    minStock: 50,
    unit: 'viên',
    warningLevel: 'critical',
    lastUpdated: '2024-12-15T10:30:00Z',
    supplierName: 'Công ty Dược A',
    location: 'Kệ A-1'
  },
  {
    id: '2',
    code: 'TH002',
    name: 'Vitamin C 1000mg',
    category: 'Vitamin',
    currentStock: 25,
    minStock: 30,
    unit: 'viên',
    warningLevel: 'warning',
    lastUpdated: '2024-12-15T09:15:00Z',
    supplierName: 'Công ty Dược B',
    location: 'Kệ B-2'
  },
  {
    id: '3',
    code: 'TH003',
    name: 'Amoxicillin 500mg',
    category: 'Kháng sinh',
    currentStock: 15,
    minStock: 20,
    unit: 'viên',
    warningLevel: 'low',
    lastUpdated: '2024-12-14T14:20:00Z',
    supplierName: 'Công ty Dược C',
    location: 'Kệ C-3'
  },
  {
    id: '4',
    code: 'TH004',
    name: 'Omeprazole 20mg',
    category: 'Thuốc dạ dày',
    currentStock: 2,
    minStock: 15,
    unit: 'viên',
    warningLevel: 'critical',
    lastUpdated: '2024-12-15T11:45:00Z',
    supplierName: 'Công ty Dược A',
    location: 'Kệ D-4'
  },
  {
    id: '5',
    code: 'TH005',
    name: 'Ibuprofen 400mg',
    category: 'Thuốc giảm đau',
    currentStock: 18,
    minStock: 25,
    unit: 'viên',
    warningLevel: 'low',
    lastUpdated: '2024-12-15T08:30:00Z',
    supplierName: 'Công ty Dược B',
    location: 'Kệ A-2'
  }
])

// Table configuration
const tableHeaders = [
  { title: 'Mã SP', key: 'code', sortable: true },
  { title: 'Tên sản phẩm', key: 'name', sortable: true },
  { title: 'Tồn kho', key: 'currentStock', sortable: true },
  { title: 'Tối thiểu', key: 'minStock', sortable: true },
  { title: 'Mức độ', key: 'warningLevel', sortable: true },
  { title: 'Thiếu hụt', key: 'shortage', sortable: true },
  { title: 'Cập nhật', key: 'lastUpdated', sortable: true },
  { title: 'Thao tác', key: 'actions', sortable: false, width: 150 }
]

// Filter options
const warningLevels = [
  { title: 'Tất cả mức độ', value: '' },
  { title: 'Nguy hiểm', value: 'critical' },
  { title: 'Cảnh báo', value: 'warning' },
  { title: 'Thấp', value: 'low' }
]

const categoryOptions = [
  { title: 'Tất cả danh mục', value: '' },
  { title: 'Thuốc giảm đau', value: 'Thuốc giảm đau' },
  { title: 'Kháng sinh', value: 'Kháng sinh' },
  { title: 'Vitamin', value: 'Vitamin' },
  { title: 'Thuốc dạ dày', value: 'Thuốc dạ dày' }
]

// Computed properties
const filteredItems = computed(() => {
  let filtered = warningItems.value

  // Quick filter
  if (quickFilter.value !== 'all') {
    filtered = filtered.filter(item => item.warningLevel === quickFilter.value)
  }

  // Level filter
  if (levelFilter.value) {
    filtered = filtered.filter(item => item.warningLevel === levelFilter.value)
  }

  // Category filter
  if (categoryFilter.value) {
    filtered = filtered.filter(item => item.category === categoryFilter.value)
  }

  // Search filter
  if (searchQuery.value) {
    const searchLower = searchQuery.value.toLowerCase()
    filtered = filtered.filter(item =>
      item.name.toLowerCase().includes(searchLower) ||
      item.code.toLowerCase().includes(searchLower)
    )
  }

  return filtered
})

const summaryStats = computed<SummaryStat[]>(() => {
  return [
    {
      key: 'total',
      label: 'Tổng cảnh báo',
      value: totalWarnings.value,
      color: 'primary',
      icon: 'mdi-alert-circle'
    },
    {
      key: 'critical',
      label: 'Nguy hiểm',
      value: criticalCount.value,
      color: 'error',
      icon: 'mdi-alert-octagon'
    },
    {
      key: 'warning',
      label: 'Cảnh báo',
      value: warningCount.value,
      color: 'warning',
      icon: 'mdi-alert'
    },
    {
      key: 'low',
      label: 'Thấp',
      value: lowCount.value,
      color: 'info',
      icon: 'mdi-information'
    }
  ]
})

const totalWarnings = computed(() => warningItems.value.length)
const criticalCount = computed(() => warningItems.value.filter(item => item.warningLevel === 'critical').length)
const warningCount = computed(() => warningItems.value.filter(item => item.warningLevel === 'warning').length)
const lowCount = computed(() => warningItems.value.filter(item => item.warningLevel === 'low').length)

// Methods
const getWarningColor = (level: string): string => {
  switch (level) {
    case 'critical': return 'error'
    case 'warning': return 'warning'
    case 'low': return 'info'
    default: return 'grey'
  }
}

const getWarningVariant = (level: string): 'flat' | 'outlined' => {
  return level === 'critical' ? 'flat' : 'outlined'
}

const getWarningIcon = (level: string): string => {
  switch (level) {
    case 'critical': return 'mdi-alert-octagon'
    case 'warning': return 'mdi-alert'
    case 'low': return 'mdi-information'
    default: return 'mdi-help'
  }
}

const getWarningText = (level: string): string => {
  switch (level) {
    case 'critical': return 'Nguy hiểm'
    case 'warning': return 'Cảnh báo'
    case 'low': return 'Thấp'
    default: return 'Không xác định'
  }
}

const getStockClass = (item: WarningItem): string => {
  if (item.warningLevel === 'critical') return 'text-error'
  if (item.warningLevel === 'warning') return 'text-warning'
  return 'text-info'
}

const setQuickFilter = (filter: string): void => {
  quickFilter.value = filter
  levelFilter.value = filter === 'all' ? '' : filter
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

const markAllResolved = (): void => {
  if (filteredItems.value.length === 0) {
    showSuccess('Không có cảnh báo nào để xử lý')
    return
  }
  bulkActionDialog.value = true
}

const confirmBulkAction = async (): Promise<void> => {
  isProcessing.value = true
  try {
    // Simulate API call
    await new Promise(resolve => setTimeout(resolve, 1500))
    
    // Remove filtered items from warningItems
    const filteredIds = filteredItems.value.map(item => item.id)
    warningItems.value = warningItems.value.filter(item => !filteredIds.includes(item.id))
    
    showSuccess(`Đã đánh dấu ${filteredIds.length} cảnh báo là đã xử lý`)
    bulkActionDialog.value = false
  } catch (error) {
    console.error('Error marking all resolved:', error)
  } finally {
    isProcessing.value = false
  }
}

const exportWarnings = (): void => {
  showSuccess('Đang xuất danh sách cảnh báo...')
}

const addStock = (item: WarningItem): void => {
  showSuccess(`Chuyển đến trang nhập hàng cho ${item.name}`)
}

const viewDetails = (item: WarningItem): void => {
  selectedItem.value = item
  detailDialog.value = true
}

const markResolved = async (item: WarningItem): Promise<void> => {
  try {
    // Simulate API call
    await new Promise(resolve => setTimeout(resolve, 500))
    
    // Remove item from list
    const index = warningItems.value.findIndex(w => w.id === item.id)
    if (index !== -1) {
      warningItems.value.splice(index, 1)
    }
    
    showSuccess(`Đã đánh dấu cảnh báo cho ${item.name} là đã xử lý`)
  } catch (error) {
    console.error('Error marking resolved:', error)
  }
}

const formatNumber = (value: number): string => {
  return new Intl.NumberFormat('vi-VN').format(value)
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
.stock-warning-container {
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

.custom-table :deep(.v-data-table-rows-no-data) {
  @apply text-center py-8;
}

/* Responsive adjustments */
@media (max-width: 768px) {
  .stock-warning-container {
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