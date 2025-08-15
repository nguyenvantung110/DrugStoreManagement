<template>
  <div class="stock-check-container">
    <!-- Header Section -->
    <div class="flex items-center justify-between mb-6">
      <div class="flex items-center gap-3">
        <v-icon color="primary" size="24">mdi-warehouse</v-icon>
        <div>
          <h2 class="text-xl font-bold text-gray-800">Kiểm tra tồn kho</h2>
          <p class="text-gray-500 text-sm">Tra cứu và theo dõi thông tin tồn kho hiện tại</p>
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
          prepend-icon="mdi-file-export"
          @click="exportData"
        >
          Xuất Excel
        </v-btn>
      </div>
    </div>

    <!-- Filter & Search Section -->
    <v-card class="mb-6 filter-card" elevation="0">
      <v-card-text class="pb-4">
        <div class="grid grid-cols-1 md:grid-cols-3 gap-4 items-center">
          <!-- Search Input -->
          <v-text-field
            v-model="search"
            label="Tìm kiếm thuốc..."
            prepend-inner-icon="mdi-magnify"
            variant="outlined"
            density="comfortable"
            hide-details
            clearable
            placeholder="Nhập tên thuốc hoặc mã vạch..."
          />

          <!-- Stock Status Filter -->
          <v-select
            v-model="statusFilter"
            :items="statusOptions"
            label="Trạng thái tồn kho"
            prepend-inner-icon="mdi-filter"
            variant="outlined"
            density="comfortable"
            hide-details
            clearable
          />

          <!-- Category Filter -->
          <v-select
            v-model="categoryFilter"
            :items="categoryOptions"
            label="Danh mục thuốc"
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
            Tất cả ({{ totalItems }})
          </v-chip>
          <v-chip
            :variant="quickFilter === 'low_stock' ? 'flat' : 'outlined'"
            :color="quickFilter === 'low_stock' ? 'warning' : 'default'"
            size="small"
            clickable
            @click="setQuickFilter('low_stock')"
          >
            Sắp hết ({{ lowStockCount }})
          </v-chip>
          <v-chip
            :variant="quickFilter === 'out_of_stock' ? 'flat' : 'outlined'"
            :color="quickFilter === 'out_of_stock' ? 'error' : 'default'"
            size="small"
            clickable
            @click="setQuickFilter('out_of_stock')"
          >
            Hết hàng ({{ outOfStockCount }})
          </v-chip>
          <v-chip
            :variant="quickFilter === 'in_stock' ? 'flat' : 'outlined'"
            :color="quickFilter === 'in_stock' ? 'success' : 'default'"
            size="small"
            clickable
            @click="setQuickFilter('in_stock')"
          >
            Còn hàng ({{ inStockCount }})
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
          :search="search"
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
            <div class="text-xs text-gray-500">{{ item.genericName }}</div>
          </template>

          <template #item.category="{ item }">
            <v-chip size="x-small" variant="outlined" color="info">
              {{ item.category }}
            </v-chip>
          </template>

          <template #item.quantity="{ item }">
            <div class="text-right">
              <div class="font-medium">{{ formatNumber(item.quantity) }}</div>
              <div class="text-xs text-gray-500">{{ item.unit }}</div>
            </div>
          </template>

          <template #item.minStock="{ item }">
            <div class="text-right font-medium">
              {{ formatNumber(item.minStock) }}
            </div>
          </template>

          <template #item.status="{ item }">
            <v-chip
              :color="getStatusColor(item)"
              :variant="getStatusVariant(item)"
              size="small"
            >
              <v-icon start size="16">{{ getStatusIcon(item) }}</v-icon>
              {{ getStatusText(item) }}
            </v-chip>
          </template>

          <template #item.lastUpdated="{ item }">
            <div class="text-sm">{{ formatDateTime(item.lastUpdated) }}</div>
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
              
              <v-tooltip text="Điều chỉnh tồn kho">
                <template #activator="{ props }">
                  <v-btn
                    v-bind="props"
                    icon="mdi-pencil"
                    size="small"
                    variant="text"
                    color="secondary"
                    @click="adjustStock(item)"
                  />
                </template>
              </v-tooltip>
              
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
            </div>
          </template>

          <!-- No data state -->
          <template #no-data>
            <div class="text-center py-8">
              <v-icon size="64" color="grey-lighten-1">mdi-package-variant</v-icon>
              <div class="text-h6 text-gray-500 mt-4">Không tìm thấy dữ liệu</div>
              <div class="text-gray-400">Thử thay đổi bộ lọc hoặc từ khóa tìm kiếm</div>
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
                <div class="font-mono font-medium">{{ selectedItem.code }}</div>
              </div>
              <div>
                <label class="text-sm font-medium text-gray-600">Tên thuốc</label>
                <div class="font-medium">{{ selectedItem.name }}</div>
              </div>
              <div>
                <label class="text-sm font-medium text-gray-600">Hoạt chất</label>
                <div>{{ selectedItem.genericName }}</div>
              </div>
              <div>
                <label class="text-sm font-medium text-gray-600">Danh mục</label>
                <div>{{ selectedItem.category }}</div>
              </div>
            </div>

            <!-- Stock Info -->
            <div class="space-y-4">
              <div>
                <label class="text-sm font-medium text-gray-600">Tồn kho hiện tại</label>
                <div class="text-xl font-bold text-primary">
                  {{ formatNumber(selectedItem.quantity) }} {{ selectedItem.unit }}
                </div>
              </div>
              <div>
                <label class="text-sm font-medium text-gray-600">Tồn kho tối thiểu</label>
                <div class="font-medium">{{ formatNumber(selectedItem.minStock) }} {{ selectedItem.unit }}</div>
              </div>
              <div>
                <label class="text-sm font-medium text-gray-600">Trạng thái</label>
                <v-chip
                  :color="getStatusColor(selectedItem)"
                  size="small"
                  class="mt-1"
                >
                  {{ getStatusText(selectedItem) }}
                </v-chip>
              </div>
              <div>
                <label class="text-sm font-medium text-gray-600">Cập nhật lần cuối</label>
                <div>{{ formatDateTime(selectedItem.lastUpdated) }}</div>
              </div>
            </div>
          </div>
        </v-card-text>
        
        <v-card-actions class="px-6 pb-6">
          <v-spacer />
          <v-btn variant="text" @click="detailDialog = false">
            Đóng
          </v-btn>
          <v-btn
            color="secondary"
            variant="outlined"
            prepend-icon="mdi-pencil"
            @click="adjustStock(selectedItem!)"
          >
            Điều chỉnh
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
interface StockItem {
  id: string
  code: string
  name: string
  genericName: string
  category: string
  unit: string
  quantity: number
  minStock: number
  lastUpdated: string
}

interface SummaryStat {
  key: string
  label: string
  value: string | number
  color: string
  icon: string
}

// Reactive state
const search = ref<string>('')
const statusFilter = ref<string>('')
const categoryFilter = ref<string>('')
const quickFilter = ref<string>('all')
const isLoading = ref<boolean>(false)
const currentPage = ref<number>(1)
const itemsPerPage = ref<number>(10)
const sortBy = ref([{ key: 'name', order: 'asc' as const }])

// Dialog states
const detailDialog = ref<boolean>(false)
const selectedItem = ref<StockItem | null>(null)

// Snackbar
const showSnackbar = ref<boolean>(false)
const snackbarMessage = ref<string>('')

// Sample data - In real app, this would come from API/Pinia store
const items = ref<StockItem[]>([
  {
    id: '1',
    code: 'TH001',
    name: 'Paracetamol 500mg',
    genericName: 'Acetaminophen',
    category: 'Thuốc giảm đau',
    unit: 'viên',
    quantity: 150,
    minStock: 50,
    lastUpdated: '2024-12-15T10:30:00Z'
  },
  {
    id: '2',
    code: 'TH002',
    name: 'Vitamin C 1000mg',
    genericName: 'Ascorbic acid',
    category: 'Vitamin',
    unit: 'viên',
    quantity: 25,
    minStock: 30,
    lastUpdated: '2024-12-15T09:15:00Z'
  },
  {
    id: '3',
    code: 'TH003',
    name: 'Amoxicillin 500mg',
    genericName: 'Amoxicillin trihydrate',
    category: 'Kháng sinh',
    unit: 'viên',
    quantity: 0,
    minStock: 20,
    lastUpdated: '2024-12-14T14:20:00Z'
  },
  {
    id: '4',
    code: 'TH004',
    name: 'Ibuprofen 400mg',
    genericName: 'Ibuprofen',
    category: 'Thuốc giảm đau',
    unit: 'viên',
    quantity: 80,
    minStock: 25,
    lastUpdated: '2024-12-15T11:45:00Z'
  },
  {
    id: '5',
    code: 'TH005',
    name: 'Omeprazole 20mg',
    genericName: 'Omeprazole',
    category: 'Thuốc dạ dày',
    unit: 'viên',
    quantity: 12,
    minStock: 15,
    lastUpdated: '2024-12-15T08:30:00Z'
  }
])

// Table configuration
const tableHeaders = [
  { title: 'Mã thuốc', key: 'code', sortable: true },
  { title: 'Tên thuốc', key: 'name', sortable: true },
  { title: 'Danh mục', key: 'category', sortable: true },
  { title: 'Tồn kho', key: 'quantity', sortable: true },
  { title: 'Tối thiểu', key: 'minStock', sortable: true },
  { title: 'Trạng thái', key: 'status', sortable: false },
  { title: 'Cập nhật', key: 'lastUpdated', sortable: true },
  { title: 'Thao tác', key: 'actions', sortable: false, width: 150 }
]

// Filter options
const statusOptions = [
  { title: 'Tất cả trạng thái', value: '' },
  { title: 'Còn hàng', value: 'in_stock' },
  { title: 'Sắp hết hàng', value: 'low_stock' },
  { title: 'Hết hàng', value: 'out_of_stock' }
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
  let filtered = items.value

  // Quick filter
  if (quickFilter.value !== 'all') {
    filtered = filtered.filter(item => {
      switch (quickFilter.value) {
        case 'low_stock':
          return item.quantity <= item.minStock && item.quantity > 0
        case 'out_of_stock':
          return item.quantity === 0
        case 'in_stock':
          return item.quantity > item.minStock
        default:
          return true
      }
    })
  }

  // Status filter
  if (statusFilter.value) {
    filtered = filtered.filter(item => {
      switch (statusFilter.value) {
        case 'low_stock':
          return item.quantity <= item.minStock && item.quantity > 0
        case 'out_of_stock':
          return item.quantity === 0
        case 'in_stock':
          return item.quantity > item.minStock
        default:
          return true
      }
    })
  }

  // Category filter
  if (categoryFilter.value) {
    filtered = filtered.filter(item => item.category === categoryFilter.value)
  }

  // Search filter
  if (search.value) {
    const searchLower = search.value.toLowerCase()
    filtered = filtered.filter(item =>
      item.name.toLowerCase().includes(searchLower) ||
      item.code.toLowerCase().includes(searchLower) ||
      item.genericName.toLowerCase().includes(searchLower)
    )
  }

  return filtered
})

const summaryStats = computed<SummaryStat[]>(() => {
  return [
    {
      key: 'total',
      label: 'Tổng sản phẩm',
      value: totalItems.value,
      color: 'primary',
      icon: 'mdi-package-variant'
    },
    {
      key: 'in_stock',
      label: 'Còn hàng',
      value: inStockCount.value,
      color: 'success',
      icon: 'mdi-check-circle'
    },
    {
      key: 'low_stock',
      label: 'Sắp hết',
      value: lowStockCount.value,
      color: 'warning',
      icon: 'mdi-alert-circle'
    },
    {
      key: 'out_of_stock',
      label: 'Hết hàng',
      value: outOfStockCount.value,
      color: 'error',
      icon: 'mdi-close-circle'
    }
  ]
})

const totalItems = computed(() => items.value.length)
const inStockCount = computed(() => items.value.filter(item => item.quantity > item.minStock).length)
const lowStockCount = computed(() => items.value.filter(item => item.quantity <= item.minStock && item.quantity > 0).length)
const outOfStockCount = computed(() => items.value.filter(item => item.quantity === 0).length)

// Methods
const getStatusColor = (item: StockItem): string => {
  if (item.quantity === 0) return 'error'
  if (item.quantity <= item.minStock) return 'warning'
  return 'success'
}

const getStatusVariant = (item: StockItem): 'flat' | 'outlined' => {
  return item.quantity === 0 ? 'flat' : 'outlined'
}

const getStatusIcon = (item: StockItem): string => {
  if (item.quantity === 0) return 'mdi-close-circle'
  if (item.quantity <= item.minStock) return 'mdi-alert-circle'
  return 'mdi-check-circle'
}

const getStatusText = (item: StockItem): string => {
  if (item.quantity === 0) return 'Hết hàng'
  if (item.quantity <= item.minStock) return 'Sắp hết'
  return 'Còn hàng'
}

const setQuickFilter = (filter: string): void => {
  quickFilter.value = filter
  statusFilter.value = filter === 'all' ? '' : filter
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

const exportData = (): void => {
  // Export logic here
  showSuccess('Đang xuất dữ liệu ra Excel...')
}

const viewDetails = (item: StockItem): void => {
  selectedItem.value = item
  detailDialog.value = true
}

const adjustStock = (item: StockItem): void => {
  // Emit event to parent or navigate to adjust page
  showSuccess(`Chuyển đến trang điều chỉnh cho ${item.name}`)
}

const addStock = (item: StockItem): void => {
  // Emit event to parent or navigate to add stock page
  showSuccess(`Chuyển đến trang nhập hàng cho ${item.name}`)
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
.stock-check-container {
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
  .stock-check-container {
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