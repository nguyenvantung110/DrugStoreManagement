<template>
  <div class="stock-adjust-container">
    <!-- Header Section -->
    <div class="flex items-center justify-between mb-6">
      <div class="flex items-center gap-3">
        <v-icon color="secondary" size="24">mdi-tune-variant</v-icon>
        <div>
          <h2 class="text-xl font-bold text-gray-800">Điều chỉnh tồn kho</h2>
          <p class="text-gray-500 text-sm">Điều chỉnh số liệu tồn kho khi có sai lệch hoặc cần chỉnh sửa thông tin</p>
        </div>
      </div>

      <!-- Action Buttons -->
      <div class="flex gap-2">
        <v-btn
          variant="outlined"
          color="primary"
          size="small"
          prepend-icon="mdi-refresh"
          @click="resetForm"
        >
          Làm mới
        </v-btn>
        <v-btn
          variant="outlined"
          color="info"
          size="small"
          prepend-icon="mdi-history"
          @click="showHistory = true"
        >
          Lịch sử điều chỉnh
        </v-btn>
      </div>
    </div>

    <!-- Alert Warning -->
    <v-alert
      type="warning"
      variant="tonal"
      class="mb-6"
      rounded="lg"
    >
      <template #prepend>
        <v-icon>mdi-alert-circle</v-icon>
      </template>
      <div class="font-medium">Lưu ý quan trọng</div>
      <div class="text-sm mt-1">
        Chức năng này dùng để điều chỉnh số liệu tồn kho trong các trường hợp: 
        kiểm kê thực tế, sai lệch, thất thoát, hủy thuốc, nhập nhầm...
      </div>
    </v-alert>

    <!-- Search Drug Section -->
    <v-card class="mb-6 search-card" elevation="0">
      <v-card-title class="px-6 py-4 border-b border-gray-100">
        <div class="flex items-center gap-2">
          <v-icon color="primary" size="20">mdi-magnify</v-icon>
          <span class="font-semibold">Tìm kiếm thuốc</span>
        </div>
      </v-card-title>
      
      <v-card-text class="px-6 py-4">
        <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
          <v-text-field
            v-model="searchQuery"
            label="Tìm theo mã hoặc tên thuốc"
            prepend-inner-icon="mdi-magnify"
            variant="outlined"
            density="comfortable"
            hide-details
            clearable
            @input="searchDrugs"
          />
          
          <v-autocomplete
            v-model="selectedDrug"
            :items="searchResults"
            :loading="searchLoading"
            item-title="displayName"
            item-value="id"
            label="Chọn thuốc từ danh sách"
            prepend-inner-icon="mdi-pill"
            variant="outlined"
            density="comfortable"
            hide-details
            clearable
            no-data-text="Không tìm thấy thuốc nào"
            @update:model-value="selectDrug"
          >
            <template #item="{ props, item }">
              <v-list-item v-bind="props">
                <template #prepend>
                  <v-avatar size="32" color="primary" variant="tonal">
                    <v-icon size="16">mdi-pill</v-icon>
                  </v-avatar>
                </template>
                
                <v-list-item-title>{{ item.raw.name }}</v-list-item-title>
                <v-list-item-subtitle>
                  Mã: {{ item.raw.code }} | Tồn kho: {{ formatNumber(item.raw.currentStock) }} {{ item.raw.unit }}
                </v-list-item-subtitle>
              </v-list-item>
            </template>
          </v-autocomplete>
        </div>
      </v-card-text>
    </v-card>

    <!-- Adjustment Form -->
    <v-card class="main-form-card" elevation="0">
      <v-card-title class="px-6 py-4 border-b border-gray-100">
        <div class="flex items-center gap-2">
          <v-icon color="secondary" size="20">mdi-clipboard-edit</v-icon>
          <span class="font-semibold">Thông tin điều chỉnh</span>
        </div>
      </v-card-title>
      
      <v-card-text class="px-6 py-6">
        <v-form ref="formRef" v-model="formValid" @submit.prevent="onAdjust">
          <!-- Drug Information -->
          <div class="mb-6">
            <h3 class="text-lg font-semibold text-gray-800 mb-4">Thông tin thuốc</h3>
            <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
              <v-text-field
                v-model="adjustData.code"
                label="Mã thuốc *"
                prepend-inner-icon="mdi-barcode"
                variant="outlined"
                density="comfortable"
                :rules="codeRules"
                readonly
                class="drug-info-field"
              />
              
              <v-text-field
                v-model="adjustData.name"
                label="Tên thuốc *"
                prepend-inner-icon="mdi-pill"
                variant="outlined"
                density="comfortable"
                :rules="nameRules"
                readonly
                class="drug-info-field"
              />
              
              <v-text-field
                v-model="adjustData.genericName"
                label="Tên hoạt chất"
                prepend-inner-icon="mdi-flask"
                variant="outlined"
                density="comfortable"
                readonly
                class="drug-info-field"
              />
              
              <v-text-field
                v-model="adjustData.unit"
                label="Đơn vị tính"
                prepend-inner-icon="mdi-scale-balance"
                variant="outlined"
                density="comfortable"
                readonly
                class="drug-info-field"
              />
            </div>
          </div>

          <!-- Stock Adjustment -->
          <div class="mb-6">
            <h3 class="text-lg font-semibold text-gray-800 mb-4">Điều chỉnh số lượng</h3>
            <div class="grid grid-cols-1 md:grid-cols-3 gap-4">
              <v-text-field
                v-model="adjustData.oldQuantity"
                label="Tồn kho hiện tại"
                prepend-inner-icon="mdi-package-variant"
                variant="outlined"
                density="comfortable"
                type="number"
                readonly
                class="stock-field"
              />
              
              <v-text-field
                v-model="adjustData.newQuantity"
                label="Tồn kho sau điều chỉnh *"
                prepend-inner-icon="mdi-package-variant-closed"
                variant="outlined"
                density="comfortable"
                type="number"
                :rules="quantityRules"
                min="0"
                class="stock-field"
                @input="calculateDifference"
              />
              
              <v-text-field
                v-model="difference"
                :label="differenceLabel"
                :prepend-inner-icon="differenceIcon"
                variant="outlined"
                density="comfortable"
                type="number"
                readonly
                :class="[
                  'difference-field',
                  differenceClass
                ]"
              />
            </div>
          </div>

          <!-- Adjustment Details -->
          <div class="mb-6">
            <h3 class="text-lg font-semibold text-gray-800 mb-4">Thông tin điều chỉnh</h3>
            <div class="grid grid-cols-1 md:grid-cols-2 gap-4 mb-4">
              <v-select
                v-model="adjustData.adjustmentType"
                :items="adjustmentTypes"
                label="Loại điều chỉnh *"
                prepend-inner-icon="mdi-format-list-bulleted"
                variant="outlined"
                density="comfortable"
                :rules="typeRules"
              />
              
              <v-text-field
                v-model="adjustData.batchNumber"
                label="Số lô (nếu có)"
                prepend-inner-icon="mdi-pound"
                variant="outlined"
                density="comfortable"
              />
            </div>
            
            <v-textarea
              v-model="adjustData.reason"
              label="Lý do điều chỉnh *"
              prepend-inner-icon="mdi-text"
              variant="outlined"
              density="comfortable"
              :rules="reasonRules"
              rows="3"
              counter="500"
              max="500"
            />
          </div>

          <!-- Action Buttons -->
          <div class="flex justify-end gap-3">
            <v-btn
              variant="outlined"
              color="grey-darken-1"
              @click="resetForm"
            >
              Hủy bỏ
            </v-btn>
            
            <v-btn
              type="submit"
              color="secondary"
              variant="flat"
              :loading="isSubmitting"
              :disabled="!formValid || !adjustData.code"
              prepend-icon="mdi-content-save"
            >
              Xác nhận điều chỉnh
            </v-btn>
          </div>
        </v-form>
      </v-card-text>
    </v-card>

    <!-- History Dialog -->
    <v-dialog v-model="showHistory" max-width="900" persistent>
      <v-card rounded="xl">
        <v-card-title class="px-6 py-4 border-b border-gray-100">
          <div class="flex items-center gap-2">
            <v-icon color="info">mdi-history</v-icon>
            <span>Lịch sử điều chỉnh tồn kho</span>
          </div>
        </v-card-title>
        
        <v-card-text class="pa-0">
          <v-data-table
            :headers="historyHeaders"
            :items="adjustmentHistory"
            :loading="historyLoading"
            class="custom-table"
            density="comfortable"
            hover
            fixed-header
            height="400"
          >
            <template #item.adjustmentDate="{ item }">
              {{ formatDateTime(item.adjustmentDate) }}
            </template>
            
            <template #item.difference="{ item }">
              <v-chip
                :color="item.difference > 0 ? 'success' : item.difference < 0 ? 'error' : 'grey'"
                size="small"
                variant="flat"
              >
                {{ item.difference > 0 ? '+' : '' }}{{ formatNumber(item.difference) }}
              </v-chip>
            </template>
            
            <template #item.adjustmentType="{ item }">
              <v-chip size="small" variant="outlined">
                {{ item.adjustmentType }}
              </v-chip>
            </template>
          </v-data-table>
        </v-card-text>
        
        <v-card-actions class="px-6 pb-6">
          <v-spacer />
          <v-btn variant="text" @click="showHistory = false">
            Đóng
          </v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>

    <!-- Confirmation Dialog -->
    <v-dialog v-model="confirmDialog" max-width="500" persistent>
      <v-card rounded="xl">
        <v-card-title class="px-6 py-4 border-b border-gray-100">
          <v-icon color="warning" class="mr-2">mdi-alert</v-icon>
          <span>Xác nhận điều chỉnh tồn kho</span>
        </v-card-title>
        
        <v-card-text class="px-6 py-4">
          <div class="space-y-3">
            <p class="font-medium">Bạn có chắc chắn muốn thực hiện điều chỉnh này?</p>
            
            <div class="bg-gray-50 p-4 rounded-lg space-y-2">
              <div class="flex justify-between">
                <span class="text-gray-600">Thuốc:</span>
                <span class="font-medium">{{ adjustData.name }}</span>
              </div>
              <div class="flex justify-between">
                <span class="text-gray-600">Tồn kho hiện tại:</span>
                <span>{{ formatNumber(adjustData.oldQuantity) }} {{ adjustData.unit }}</span>
              </div>
              <div class="flex justify-between">
                <span class="text-gray-600">Tồn kho sau điều chỉnh:</span>
                <span>{{ formatNumber(adjustData.newQuantity) }} {{ adjustData.unit }}</span>
              </div>
              <div class="flex justify-between">
                <span class="text-gray-600">Chênh lệch:</span>
                <span :class="differenceClass">
                  {{ difference > 0 ? '+' : '' }}{{ formatNumber(difference) }} {{ adjustData.unit }}
                </span>
              </div>
            </div>
            
            <p class="text-sm text-gray-500">Hành động này không thể hoàn tác.</p>
          </div>
        </v-card-text>
        
        <v-card-actions class="px-6 pb-6">
          <v-spacer />
          <v-btn variant="text" @click="confirmDialog = false">
            Hủy bỏ
          </v-btn>
          <v-btn
            color="secondary"
            variant="flat"
            :loading="isSubmitting"
            @click="confirmAdjustment"
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
      timeout="4000"
      location="bottom right"
    >
      <div class="flex items-center gap-2">
        <v-icon>mdi-check-circle</v-icon>
        <span>{{ successMessage }}</span>
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
interface Drug {
  id: string
  code: string
  name: string
  genericName: string
  currentStock: number
  unit: string
  displayName: string
}

interface AdjustmentData {
  code: string
  name: string
  genericName: string
  oldQuantity: number
  newQuantity: number
  unit: string
  reason: string
  adjustmentType: string
  batchNumber: string
}

interface AdjustmentHistory {
  id: string
  drugCode: string
  drugName: string
  oldQuantity: number
  newQuantity: number
  difference: number
  adjustmentType: string
  reason: string
  adjustmentDate: string
  adjustedBy: string
}

// Reactive state
const formRef = ref()
const formValid = ref<boolean>(false)
const searchQuery = ref<string>('')
const selectedDrug = ref<string>('')
const searchLoading = ref<boolean>(false)
const isSubmitting = ref<boolean>(false)
const showSnackbar = ref<boolean>(false)
const showHistory = ref<boolean>(false)
const confirmDialog = ref<boolean>(false)
const historyLoading = ref<boolean>(false)
const successMessage = ref<string>('')

// Form data
const adjustData = ref<AdjustmentData>({
  code: '',
  name: '',
  genericName: '',
  oldQuantity: 0,
  newQuantity: 0,
  unit: '',
  reason: '',
  adjustmentType: '',
  batchNumber: ''
})

// Sample data - In real app, these would come from API/store
const searchResults = ref<Drug[]>([])
const adjustmentHistory = ref<AdjustmentHistory[]>([])

// Mock drugs data
const mockDrugs: Drug[] = [
  {
    id: '1',
    code: 'TH001',
    name: 'Paracetamol 500mg',
    genericName: 'Acetaminophen',
    currentStock: 150,
    unit: 'viên',
    displayName: 'Paracetamol 500mg (TH001)'
  },
  {
    id: '2',
    code: 'TH002',
    name: 'Amoxicillin 250mg',
    genericName: 'Amoxicillin trihydrate',
    currentStock: 75,
    unit: 'viên',
    displayName: 'Amoxicillin 250mg (TH002)'
  },
  {
    id: '3',
    code: 'TH003',
    name: 'Ibuprofen 400mg',
    genericName: 'Ibuprofen',
    currentStock: 200,
    unit: 'viên',
    displayName: 'Ibuprofen 400mg (TH003)'
  }
]

// Constants
const adjustmentTypes = [
  { title: 'Kiểm kê thực tế', value: 'inventory_count' },
  { title: 'Sai lệch hệ thống', value: 'system_error' },
  { title: 'Thất thoát', value: 'loss' },
  { title: 'Hủy thuốc', value: 'disposal' },
  { title: 'Nhập nhầm', value: 'input_error' },
  { title: 'Trả hàng nhà cung cấp', value: 'supplier_return' },
  { title: 'Khác', value: 'other' }
]

const historyHeaders = [
  { title: 'Mã thuốc', key: 'drugCode', sortable: true },
  { title: 'Tên thuốc', key: 'drugName', sortable: true },
  { title: 'Tồn kho cũ', key: 'oldQuantity', sortable: true },
  { title: 'Tồn kho mới', key: 'newQuantity', sortable: true },
  { title: 'Chênh lệch', key: 'difference', sortable: true },
  { title: 'Loại điều chỉnh', key: 'adjustmentType', sortable: true },
  { title: 'Ngày điều chỉnh', key: 'adjustmentDate', sortable: true },
  { title: 'Người thực hiện', key: 'adjustedBy', sortable: true }
]

// Validation rules
const codeRules = [
  (v: string) => !!v || 'Mã thuốc là bắt buộc'
]

const nameRules = [
  (v: string) => !!v || 'Tên thuốc là bắt buộc'
]

const quantityRules = [
  (v: string) => !!v || 'Số lượng là bắt buộc',
  (v: string) => !isNaN(Number(v)) || 'Số lượng phải là số',
  (v: string) => Number(v) >= 0 || 'Số lượng không được âm'
]

const reasonRules = [
  (v: string) => !!v || 'Lý do điều chỉnh là bắt buộc',
  (v: string) => v.length <= 500 || 'Lý do không được quá 500 ký tự'
]

const typeRules = [
  (v: string) => !!v || 'Loại điều chỉnh là bắt buộc'
]

// Computed properties
const difference = computed(() => {
  return adjustData.value.newQuantity - adjustData.value.oldQuantity
})

const differenceLabel = computed(() => {
  const diff = difference.value
  if (diff > 0) return 'Tăng thêm'
  if (diff < 0) return 'Giảm đi'
  return 'Không thay đổi'
})

const differenceIcon = computed(() => {
  const diff = difference.value
  if (diff > 0) return 'mdi-trending-up'
  if (diff < 0) return 'mdi-trending-down'
  return 'mdi-minus'
})

const differenceClass = computed(() => {
  const diff = difference.value
  if (diff > 0) return 'text-success font-weight-bold'
  if (diff < 0) return 'text-error font-weight-bold'
  return 'text-grey font-weight-medium'
})

// Methods
const searchDrugs = (): void => {
  if (!searchQuery.value || searchQuery.value.length < 2) {
    searchResults.value = []
    return
  }

  searchLoading.value = true
  
  // Simulate API call
  setTimeout(() => {
    const query = searchQuery.value.toLowerCase()
    searchResults.value = mockDrugs.filter(drug => 
      drug.code.toLowerCase().includes(query) || 
      drug.name.toLowerCase().includes(query)
    )
    searchLoading.value = false
  }, 500)
}

const selectDrug = (): void => {
  const drug = mockDrugs.find(d => d.id === selectedDrug.value)
  if (drug) {
    adjustData.value = {
      ...adjustData.value,
      code: drug.code,
      name: drug.name,
      genericName: drug.genericName,
      oldQuantity: drug.currentStock,
      newQuantity: drug.currentStock,
      unit: drug.unit
    }
  }
}

const calculateDifference = (): void => {
  // This is handled by computed property
}

const onAdjust = (): void => {
  if (!formRef.value?.validate()) return
  confirmDialog.value = true
}

const confirmAdjustment = async (): Promise<void> => {
  isSubmitting.value = true
  
  try {
    // Simulate API call
    await new Promise(resolve => setTimeout(resolve, 1500))
    
    // Add to history
    const historyItem: AdjustmentHistory = {
      id: Date.now().toString(),
      drugCode: adjustData.value.code,
      drugName: adjustData.value.name,
      oldQuantity: adjustData.value.oldQuantity,
      newQuantity: adjustData.value.newQuantity,
      difference: difference.value,
      adjustmentType: adjustmentTypes.find(t => t.value === adjustData.value.adjustmentType)?.title || '',
      reason: adjustData.value.reason,
      adjustmentDate: new Date().toISOString(),
      adjustedBy: 'Người dùng hiện tại'
    }
    
    adjustmentHistory.value.unshift(historyItem)
    
    successMessage.value = `Đã điều chỉnh tồn kho thuốc ${adjustData.value.name} thành công!`
    showSnackbar.value = true
    confirmDialog.value = false
    
    resetForm()
  } catch (error) {
    console.error('Error adjusting stock:', error)
  } finally {
    isSubmitting.value = false
  }
}

const resetForm = (): void => {
  adjustData.value = {
    code: '',
    name: '',
    genericName: '',
    oldQuantity: 0,
    newQuantity: 0,
    unit: '',
    reason: '',
    adjustmentType: '',
    batchNumber: ''
  }
  searchQuery.value = ''
  selectedDrug.value = ''
  searchResults.value = []
  formRef.value?.resetValidation()
}

const loadAdjustmentHistory = async (): Promise<void> => {
  historyLoading.value = true
  try {
    // Simulate API call
    await new Promise(resolve => setTimeout(resolve, 1000))
    // adjustmentHistory would be loaded from API
  } finally {
    historyLoading.value = false
  }
}

const formatNumber = (value: number): string => {
  return new Intl.NumberFormat('vi-VN').format(value)
}

const formatDateTime = (dateString: string): string => {
  return new Date(dateString).toLocaleString('vi-VN')
}

// Lifecycle
onMounted(() => {
  loadAdjustmentHistory()
})
</script>

<style scoped>
.stock-adjust-container {
  @apply space-y-6;
}

.search-card {
  @apply border border-gray-100 shadow-sm;
  border-radius: 12px;
}

.main-form-card {
  @apply border border-gray-100 shadow-sm;
  border-radius: 12px;
}

.drug-info-field :deep(.v-field) {
  background-color: #f8fafc;
}

.stock-field :deep(.v-field--focused) {
  box-shadow: 0 0 0 2px rgba(139, 92, 246, 0.2);
}

.difference-field :deep(.v-field) {
  background-color: #f1f5f9;
}

.custom-table :deep(.v-data-table__wrapper) {
  border-radius: 0;
}

.custom-table :deep(th) {
  @apply bg-gray-50/50 font-semibold;
}

/* Responsive adjustments */
@media (max-width: 768px) {
  .stock-adjust-container {
    @apply space-y-4;
  }
}

/* Animation for form sections */
.main-form-card {
  animation: slideInUp 0.4s ease-out;
}

@keyframes slideInUp {
  from {
    opacity: 0;
    transform: translateY(20px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}
</style>