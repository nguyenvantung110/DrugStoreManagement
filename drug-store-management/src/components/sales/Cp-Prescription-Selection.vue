<template>
  <v-dialog 
    v-model="isVisible" 
    max-width="1000" 
    persistent
    @click:outside="closeModal"
  >
    <v-card rounded="lg" class="prescription-modal">
      <!-- Header -->
      <v-card-title class="px-6 py-4 border-b border-gray-100">
        <div class="flex items-center gap-3">
          <v-icon color="primary" size="24">mdi-prescription</v-icon>
          <div>
            <h3 class="text-xl font-bold text-gray-800">Chọn đơn thuốc mẫu</h3>
            <p class="text-sm text-gray-500 mt-1">Chọn đơn thuốc mẫu để thêm nhanh vào hóa đơn bán hàng</p>
          </div>
        </div>
      </v-card-title>
      
      <!-- Search & Filter Section -->
      <v-card-text class="px-6 py-4 border-b border-gray-50">
        <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
          <!-- Search Input -->
          <v-text-field
            v-model="searchQuery"
            label="Tìm kiếm đơn thuốc mẫu..."
            prepend-inner-icon="mdi-magnify"
            variant="outlined"
            density="comfortable"
            hide-details
            clearable
            placeholder="Nhập tên đơn thuốc hoặc mô tả bệnh..."
            @input="handleSearch"
          />

          <!-- Category Filter -->
          <v-select
            v-model="categoryFilter"
            :items="categoryOptions"
            label="Danh mục bệnh"
            prepend-inner-icon="mdi-tag-multiple"
            variant="outlined"
            density="comfortable"
            hide-details
            clearable
            @update:model-value="handleFilterChange"
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
            Tất cả ({{ totalPrescriptions }})
          </v-chip>
          <v-chip
            :variant="quickFilter === 'common' ? 'flat' : 'outlined'"
            :color="quickFilter === 'common' ? 'success' : 'default'"
            size="small"
            clickable
            @click="setQuickFilter('common')"
          >
            Phổ biến ({{ commonCount }})
          </v-chip>
          <v-chip
            :variant="quickFilter === 'recent' ? 'flat' : 'outlined'"
            :color="quickFilter === 'recent' ? 'info' : 'default'"
            size="small"
            clickable
            @click="setQuickFilter('recent')"
          >
            Mới tạo ({{ recentCount }})
          </v-chip>
        </div>
      </v-card-text>

      <!-- Prescriptions List -->
      <v-card-text class="px-0 py-0">
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
          :items="filteredPrescriptions"
          :loading="isLoading"
          class="prescription-table"
          density="comfortable"
          hover
          fixed-header
          height="400"
          @click:row="selectPrescription"
        >
          <!-- Custom slots -->
          <template #item.templateId="{ item }">
            <div class="font-mono text-sm font-medium text-primary">
              {{ item.templateId }}
            </div>
          </template>

          <template #item.templateName="{ item }">
            <div class="font-medium">{{ item.templateName }}</div>
            <div class="text-xs text-gray-500">{{ item.category.categoryName }}</div>
          </template>

          <template #item.shortDescription="{ item }">
            <div class="text-sm max-w-xs">
              {{ item.shortDescription }}
            </div>
          </template>

          <template #item.totalItems="{ item }">
            <div class="text-center">
              <v-chip size="small" color="info" variant="outlined">
                {{ item.prescriptionItems?.length || 0 }} thuốc
              </v-chip>
            </div>
          </template>

          <template #item.usageFrequency="{ item }">
            <div class="text-center">
              <v-chip 
                size="small" 
                :color="getFrequencyColor(item.usageFrequency)"
                variant="outlined"
              >
                {{ item.usageFrequency }} lần
              </v-chip>
            </div>
          </template>

          <template #item.createdBy="{ item }">
            <div class="text-sm">{{ item.createdBy }}</div>
          </template>

          <template #item.createdAt="{ item }">
            <div class="text-sm">{{ formatDate(item.createdAt) }}</div>
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
                    @click.stop="viewDetails(item)"
                  />
                </template>
              </v-tooltip>
              
              <v-tooltip text="Chọn đơn mẫu">
                <template #activator="{ props }">
                  <v-btn
                    v-bind="props"
                    icon="mdi-check"
                    size="small"
                    variant="text"
                    color="success"
                    @click.stop="selectPrescription(null, item)"
                  />
                </template>
              </v-tooltip>
            </div>
          </template>

          <!-- No data state -->
          <template #no-data>
            <div class="text-center py-8">
              <v-icon size="64" color="grey-lighten-1">mdi-prescription</v-icon>
              <div class="text-h6 text-gray-500 mt-4">Không tìm thấy đơn thuốc mẫu</div>
              <div class="text-gray-400">Thử thay đổi bộ lọc hoặc từ khóa tìm kiếm</div>
            </div>
          </template>
        </v-data-table>
      </v-card-text>

      <!-- Footer Actions -->
      <v-card-actions class="px-6 py-4 border-t border-gray-100">
        <v-spacer />
        <v-btn
          variant="text"
          color="grey-darken-1"
          @click="closeModal"
        >
          Hủy bỏ
        </v-btn>
        <v-btn
          color="primary"
          variant="flat"
          :disabled="!selectedPrescription"
          @click="confirmSelection"
        >
          Chọn đơn mẫu
        </v-btn>
      </v-card-actions>
    </v-card>

    <!-- Detail Dialog -->
    <v-dialog v-model="detailDialog" max-width="900" persistent>
      <v-card rounded="lg" v-if="selectedPrescriptionDetail" class="py-2">
        <v-card-title class="px-6 py-4 border-b border-gray-100">
          <div class="flex items-center gap-2">
            <v-icon color="primary">mdi-prescription</v-icon>
            <span>Chi tiết đơn thuốc mẫu: {{ selectedPrescriptionDetail.templateName }}</span>
          </div>
        </v-card-title>
        
        <v-card-text class="px-6 py-4">
          <!-- Template Basic Info -->
          <div class="grid grid-cols-1 md:grid-cols-2 gap-6 mb-6">
            <div class="space-y-3">
              <h4 class="font-semibold text-gray-800">Thông tin cơ bản</h4>
              <div>
                <label class="text-sm font-medium text-gray-600">Mã đơn mẫu</label>
                <div class="font-mono font-medium">{{ selectedPrescriptionDetail.templateId }}</div>
              </div>
              <div>
                <label class="text-sm font-medium text-gray-600">Tên đơn mẫu</label>
                <div class="font-medium">{{ selectedPrescriptionDetail.templateName }}</div>
              </div>
              <div>
                <label class="text-sm font-medium text-gray-600 mr-2">Danh mục</label>
                <v-chip size="small" color="info" variant="outlined">
                  {{ selectedPrescriptionDetail.category.categoryName }}
                </v-chip>
              </div>
            </div>

            <div class="space-y-3">
              <h4 class="font-semibold text-gray-800">Thống kê sử dụng</h4>
              <div>
                <label class="text-sm font-medium text-gray-600">Số lần sử dụng</label>
                <div class="font-medium text-success">{{ selectedPrescriptionDetail.usageFrequency }} lần</div>
              </div>
              <div>
                <label class="text-sm font-medium text-gray-600">Ngày tạo</label>
                <div>{{ formatDateTime(selectedPrescriptionDetail.createdAt) }}</div>
              </div>
              <div>
                <label class="text-sm font-medium text-gray-600">Cập nhật lần cuối</label>
                <div>{{ formatDateTime(selectedPrescriptionDetail.updatedAt) }}</div>
              </div>
            </div>
          </div>

          <!-- Full Description -->
          <div class="mb-6">
            <h4 class="font-semibold text-gray-800 mb-3">Mô tả chi tiết</h4>
            <div class="p-4 bg-gray-50 rounded-lg">
              <p class="text-gray-700 leading-relaxed whitespace-pre-line">{{ selectedPrescriptionDetail.fullDescription }}</p>
            </div>
          </div>

          <!-- Prescription Items -->
          <div>
            <h4 class="font-semibold text-gray-800 mb-3">
              Danh sách thuốc ({{ selectedPrescriptionDetail.prescriptionItems?.length || 0 }} thuốc)
            </h4>
            <div class="space-y-3">
              <div 
                v-for="(item, index) in selectedPrescriptionDetail.prescriptionItems"
                :key="item.product.productId"
                class="flex items-center justify-between p-4 bg-white border border-gray-200 rounded-lg hover:shadow-sm transition-shadow"
              >
                <div class="flex-1">
                  <div class="flex items-center gap-2 mb-1">
                    <span class="text-sm font-medium text-gray-500">{{ index + 1 }}.</span>
                    <div class="font-medium text-gray-800">{{ item.product.productName }}</div>
                  </div>
                  <div class="text-sm text-gray-600 ml-6">
                    <div><strong>Hướng dẫn:</strong> {{ item.dosage }}</div>
                    <div v-if="item.dosage" class="mt-1"><strong>Ghi chú:</strong> {{ item.product.description }}</div>
                  </div>
                </div>
                <div class="text-right">
                  <div class="font-medium text-primary">{{ item.quantity }} {{ item.product.unitOfMeasure }}</div>
                  <div class="text-sm text-gray-600">{{ formatCurrency(item.product.pricePerUnit) }}</div>
                </div>
              </div>
            </div>
          </div>

          <!-- Total Cost -->
          <div class="mt-6 p-4 bg-blue-50 rounded-lg">
            <div class="flex justify-between items-center">
              <span class="font-medium text-gray-800">Tổng giá trị đơn mẫu:</span>
              <span class="text-xl font-bold text-primary">
                {{ formatCurrency(calculateTotal(selectedPrescriptionDetail.prescriptionItems)) }}
              </span>
            </div>
          </div>
        </v-card-text>
        
        <v-card-actions class="px-6 pb-6">
          <v-spacer />
          <v-btn variant="text" @click="detailDialog = false">
            Đóng
          </v-btn>
          <v-btn
            color="success"
            variant="flat"
            @click="selectFromDetail"
          >
            Chọn đơn này
          </v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>
  </v-dialog>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, watch } from 'vue'
import { usePrescriptionStore } from '@/composables/prescriptions/prescriptionStore'
import { useCategoryStore } from '@/composables/categories/categoryStore'
// Store
const prescriptionStore = usePrescriptionStore()
const categoryStore = useCategoryStore()

// Types
interface ProductForPrescription{
  productId: string
  productName: string
  pricePerUnit: number
  description?: string
  genericName: string
  unitOfMeasure: string
  wholesalePrice: number
  manufacturer: string
  requiresPrescription: boolean
  stockThreshold: number
}

interface PrescriptionItem {
  product: ProductForPrescription
  templateId: string
  productId: string
  quantity: number
  dosage: string
  usageInstructions: string
}

interface PrescriptionTemplate {
  id: string
  templateId: string
  templateName: string
  category: string
  shortDescription: string
  fullDescription: string
  usageFrequency: number
  createdBy: string
  createdAt: string
  updatedAt: string
  prescriptionItems?: PrescriptionItem[]
}

// Props & Emits
interface Props {
  modelValue: boolean
}

interface Emits {
  (e: 'update:modelValue', value: boolean): void
  (e: 'prescription-selected', prescription: PrescriptionTemplate): void
}

// const props = defineProps<{
//   modelValue: boolean
// }>()

const props = defineProps<Props>()
const emit = defineEmits<Emits>()

// Reactive state
const isVisible = ref(props.modelValue)
const searchQuery = ref<string>('')
const categoryFilter = ref<string>('')
const quickFilter = ref<string>('all')
const isLoading = ref<boolean>(false)
const currentPage = ref<number>(1)
const itemsPerPage = ref<number>(10)
const sortBy = ref([{ key: 'usageFrequency', order: 'desc' as const }])

// Dialog states
const detailDialog = ref<boolean>(false)
const selectedPrescription = ref<PrescriptionTemplate | null>(null)
const selectedPrescriptionDetail = ref<PrescriptionTemplate | null>(null)

// Sample data - In real app, this would come from API/Pinia store
const prescriptionTemplates = computed(() => prescriptionStore.getPrescriptionTemplates)
const categoryList = computed(() => categoryStore.getCategoryList)

// Table configuration
const tableHeaders = [
  { title: 'Mã mẫu', key: 'templateId', sortable: true },
  { title: 'Tên đơn mẫu', key: 'templateName', sortable: true },
  { title: 'Mô tả', key: 'shortDescription', sortable: false },
  { title: 'Số thuốc', key: 'totalItems', sortable: false },
  { title: 'Đã dùng', key: 'usageFrequency', sortable: true },
  { title: 'Người tạo', key: 'createdBy', sortable: true },
  { title: 'Ngày tạo', key: 'createdAt', sortable: true },
  { title: 'Thao tác', key: 'actions', sortable: false, width: 100 }
]

const categoryOptions = computed(() => {
    return [
        { title: 'Tất cả danh mục', value: '' },
        ...categoryList.value.map(category => ({
            title: category.categoryName,
            value: category.categoryId,
        })),
    ];
})

// Computed properties
const filteredPrescriptions = computed(() => {
  let filtered = prescriptionTemplates.value

  // Quick filter
  if (quickFilter.value !== 'all') {
    if (quickFilter.value === 'common') {
      filtered = filtered.filter(item => item.usageFrequency >= 20)
    } else if (quickFilter.value === 'recent') {
      const oneMonthAgo = new Date()
      oneMonthAgo.setMonth(oneMonthAgo.getMonth() - 1)
      filtered = filtered.filter(item => new Date(item.createdAt) >= oneMonthAgo)
    }
  }

  // Search filter
  if (searchQuery.value) {
    const searchLower = searchQuery.value.toLowerCase()
    filtered = filtered.filter(item =>
      item.templateId.includes(searchLower) ||
      item.templateName.toLowerCase().includes(searchLower) ||
      item.shortDescription.toLowerCase().includes(searchLower) ||
      item.category.toLowerCase().includes(searchLower)
    )
  }

  // Category filter
  if (categoryFilter.value) {
    filtered = filtered.filter(item => item.category.categoryId === categoryFilter.value)
  }

  return filtered
})

const totalPrescriptions = computed(() => prescriptionTemplates.value.length)
const commonCount = computed(() => prescriptionTemplates.value.filter(p => p.usageFrequency >= 20).length)
const recentCount = computed(() => {
  const oneMonthAgo = new Date()
  oneMonthAgo.setMonth(oneMonthAgo.getMonth() - 1)
  return prescriptionTemplates.value.filter(p => new Date(p.createdAt) >= oneMonthAgo).length
})

// Debounced search
const debouncedSearch = () => {
  isLoading.value = true
  setTimeout(() => {
    isLoading.value = false
  }, 300) // Simulate API delay
}

// Watch for prop changes
watch(() => props.modelValue, (newVal) => {
  isVisible.value = newVal
})

watch(isVisible, (newVal) => {
  emit('update:modelValue', newVal)
})

// Methods
const handleSearch = (): void => {
  debouncedSearch()
}

const handleFilterChange = async (): Promise<void> => {
  await prescriptionStore.fetchPrescriptionTemplates(categoryFilter.value);
  currentPage.value = 1
}

const setQuickFilter = (filter: string): void => {
  quickFilter.value = filter
}

const selectPrescription = (event: any, prescription?: PrescriptionTemplate): void => {
  if (prescription) {
    selectedPrescription.value = prescription
  }
}

const viewDetails = (prescription: PrescriptionTemplate): void => {
  selectedPrescriptionDetail.value = prescription
  detailDialog.value = true
}

const selectFromDetail = (): void => {
  if (selectedPrescriptionDetail.value) {
    selectedPrescription.value = selectedPrescriptionDetail.value
    detailDialog.value = false
    confirmSelection()
  }
}

const confirmSelection = (): void => {
  if (selectedPrescription.value) {
    emit('prescription-selected', selectedPrescription.value)
    closeModal()
  }
}

const closeModal = (): void => {
  isVisible.value = false
  selectedPrescription.value = null
  selectedPrescriptionDetail.value = null
  detailDialog.value = false
  // Reset filters
  searchQuery.value = ''
  categoryFilter.value = ''
  quickFilter.value = 'all'
}

const getFrequencyColor = (frequency: number): string => {
  if (frequency >= 30) return 'success'
  if (frequency >= 15) return 'info'
  return 'warning'
}

const calculateTotal = (items?: PrescriptionItem[]): number => {
  if (!items) return 0
  return items.reduce((total, item) => total + (item.quantity * item.product.pricePerUnit), 0)
}

const formatDate = (dateString: string): string => {
  return new Date(dateString).toLocaleDateString('vi-VN')
}

const formatDateTime = (dateString: string): string => {
  return new Date(dateString).toLocaleString('vi-VN')
}

const formatCurrency = (value: number): string => {
  return new Intl.NumberFormat('vi-VN', {
    style: 'currency',
    currency: 'VND'
  }).format(value)
}

// Lifecycle
onMounted(async () => {
  // await initData()
})

const initData = async () => {
  isLoading.value = true
  await prescriptionStore.fetchPrescriptionTemplates()
  await categoryStore.fetchCategoryByType('prescription')
  isLoading.value = false
}

watch(
  () => props.modelValue,
  async (newVal, oldVal) => {
    if (newVal && !oldVal) {
      await initData()
    }
  }
)
</script>

<style scoped>
.prescription-modal {
  @apply shadow-2xl;
}

.prescription-table :deep(.v-data-table__wrapper) {
  border-radius: 0;
}

.prescription-table :deep(th) {
  @apply bg-gray-50/50 font-semibold;
}

.prescription-table :deep(.v-data-table-rows-no-data) {
  @apply text-center py-8;
}

.prescription-table :deep(tbody tr) {
  cursor: pointer;
  transition: all 0.2s ease;
}

.prescription-table :deep(tbody tr:hover) {
  @apply bg-blue-50/50;
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