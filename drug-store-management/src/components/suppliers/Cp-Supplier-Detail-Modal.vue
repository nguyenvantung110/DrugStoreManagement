<template>
  <v-dialog
    :model-value="modelValue"
    @update:model-value="$emit('update:modelValue', $event)"
    max-width="800px"
    persistent
  >
    <v-card rounded="lg" class="supplier-detail-modal">
      <!-- Header -->
      <v-card-title class="px-6 py-4 border-b border-gray-100">
        <div class="flex items-center justify-between w-full">
          <div class="flex items-center gap-3">
            <v-icon color="#11c393" size="24">mdi-eye</v-icon>
            <div>
              <h3 class="text-xl font-bold text-gray-800">Thông tin nhà cung cấp</h3>
              <p class="text-sm text-gray-500 mt-1">Xem chi tiết thông tin nhà cung cấp</p>
            </div>
          </div>
          
          <!-- Status Badge -->
          <v-chip 
            v-if="supplier"
            :color="getStatusColor(supplier.status)"
            :variant="getStatusVariant(supplier.status)"
            size="small"
          >
            <v-icon start size="16">{{ getStatusIcon(supplier.status) }}</v-icon>
            {{ getStatusText(supplier.status) }}
          </v-chip>
        </div>
      </v-card-title>

      <!-- Content -->
      <v-card-text v-if="supplier" class="px-6 py-6">
        <div class="space-y-8">
          <!-- Basic Information Section -->
          <div class="info-section">
            <h4 class="section-title">
              <v-icon size="20" class="mr-2">mdi-information</v-icon>
              Thông tin cơ bản
            </h4>
            <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
              <div class="info-group">
                <label class="info-label">Tên nhà cung cấp</label>
                <div class="info-value">{{ supplier.supplier_Name }}</div>
              </div>
              
              <div class="info-group">
                <label class="info-label">Người liên hệ</label>
                <div class="info-value">{{ supplier.contact_Person }}</div>
              </div>
              
              <div class="info-group">
                <label class="info-label">Số điện thoại</label>
                <div class="info-value">
                  <a :href="`tel:${supplier.phone_Number}`" class="contact-link">
                    <v-icon size="16" class="mr-1">mdi-phone</v-icon>
                    {{ supplier.phone_Number }}
                  </a>
                </div>
              </div>
              
              <div class="info-group">
                <label class="info-label">Email</label>
                <div class="info-value">
                  <a :href="`mailto:${supplier.email}`" class="contact-link">
                    <v-icon size="16" class="mr-1">mdi-email</v-icon>
                    {{ supplier.email }}
                  </a>
                </div>
              </div>
            </div>
            
            <!-- Address - Full width -->
            <div class="info-group mt-4">
              <label class="info-label">Địa chỉ</label>
              <div class="info-value address-value">
                <v-icon size="16" class="mr-2 text-gray-400">mdi-map-marker</v-icon>
                {{ supplier.address }}
              </div>
            </div>
          </div>

          <!-- Business Information Section -->
          <div v-if="hasBusinessInfo" class="info-section">
            <h4 class="section-title">
              <v-icon size="20" class="mr-2">mdi-briefcase</v-icon>
              Thông tin doanh nghiệp
            </h4>
            <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
              <div v-if="supplier.tax_Code" class="info-group">
                <label class="info-label">Mã số thuế</label>
                <div class="info-value tax-code">{{ supplier.tax_Code }}</div>
              </div>
              
              <div v-if="supplier.website" class="info-group">
                <label class="info-label">Website</label>
                <div class="info-value">
                  <a :href="formatWebsiteUrl(supplier.website)" target="_blank" rel="noopener" class="contact-link">
                    <v-icon size="16" class="mr-1">mdi-web</v-icon>
                    {{ supplier.website }}
                    <v-icon size="12" class="ml-1">mdi-open-in-new</v-icon>
                  </a>
                </div>
              </div>
              
              <div v-if="supplier.payment_Terms" class="info-group">
                <label class="info-label">Điều khoản thanh toán</label>
                <div class="info-value">
                  <v-chip size="small" color="info" variant="tonal">
                    {{ getPaymentTermsText(supplier.payment_Terms) }}
                  </v-chip>
                </div>
              </div>
              
              <div v-if="supplier.rating" class="info-group">
                <label class="info-label">Đánh giá</label>
                <div class="info-value">
                  <div class="flex items-center gap-2">
                    <v-rating
                      :model-value="supplier.rating"
                      color="amber"
                      size="small"
                      readonly
                      half-increments
                    />
                    <span class="text-sm font-medium">{{ supplier.rating }}/5</span>
                  </div>
                </div>
              </div>
            </div>
          </div>

          <!-- Additional Information Section -->
          <div v-if="supplier.notes || hasTimestamps" class="info-section">
            <h4 class="section-title">
              <v-icon size="20" class="mr-2">mdi-note-text</v-icon>
              Thông tin bổ sung
            </h4>
            
            <!-- Notes -->
            <div v-if="supplier.notes" class="info-group mb-4">
              <label class="info-label">Ghi chú</label>
              <div class="notes-content">
                {{ supplier.notes }}
              </div>
            </div>
            
            <!-- Timestamps -->
            <div v-if="hasTimestamps" class="grid grid-cols-1 md:grid-cols-2 gap-6">
              <div v-if="supplier.created_At" class="info-group">
                <label class="info-label">Ngày tạo</label>
                <div class="info-value timestamp">
                  <v-icon size="16" class="mr-1 text-gray-400">mdi-calendar-plus</v-icon>
                  {{ formatDateTime(supplier.created_At) }}
                </div>
              </div>
              
              <div v-if="supplier.updated_At" class="info-group">
                <label class="info-label">Cập nhật lần cuối</label>
                <div class="info-value timestamp">
                  <v-icon size="16" class="mr-1 text-gray-400">mdi-calendar-edit</v-icon>
                  {{ formatDateTime(supplier.updated_At) }}
                </div>
              </div>
            </div>
          </div>

          <!-- Quick Actions Section -->
          <div class="info-section">
            <h4 class="section-title">
              <v-icon size="20" class="mr-2">mdi-lightning-bolt</v-icon>
              Thao tác nhanh
            </h4>
            <div class="flex flex-wrap gap-3">
              <v-btn
                variant="outlined"
                color="primary"
                size="small"
                @click="copyContact"
              >
                <v-icon size="16" class="mr-1">mdi-content-copy</v-icon>
                Sao chép liên hệ
              </v-btn>  
              <v-btn
                v-if="supplier.status === 'active'"
                variant="outlined"
                color="warning"
                size="small"
                @click="toggleStatus"
              >
                <v-icon size="16" class="mr-1">mdi-pause</v-icon>
                Tạm dừng hợp tác
              </v-btn>
              
              <v-btn
                v-else-if="supplier.status === 'inactive'"
                variant="outlined"
                color="success"
                size="small"
                @click="toggleStatus"
              >
                <v-icon size="16" class="mr-1">mdi-play</v-icon>
                Kích hoạt lại
              </v-btn>
            </div>
          </div>
        </div>
      </v-card-text>

      <!-- Loading State -->
      <v-card-text v-else class="px-6 py-12">
        <div class="flex flex-col items-center justify-center">
          <v-progress-circular
            indeterminate
            color="primary"
            size="48"
          />
          <p class="text-gray-500 mt-4">Đang tải thông tin...</p>
        </div>
      </v-card-text>

      <!-- Footer Actions -->
      <v-card-actions class="px-6 pb-6 border-t border-gray-100">
        <v-spacer />
        <v-btn
          variant="text"
          color="grey-darken-1"
          @click="closeDialog"
        >
          <v-icon size="16" class="mr-1">mdi-close</v-icon>
          Đóng
        </v-btn>
      </v-card-actions>
    </v-card>

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
  </v-dialog>
</template>

<script setup lang="ts">
import { computed, onMounted, ref } from 'vue'

// Types
interface SupplierDto {
  supplier_Id?: string
  supplier_Name: string
  contact_Person: string
  phone_Number: string
  email: string
  address: string
  status?: 'active' | 'inactive' | 'pending'
  tax_Code?: string
  website?: string
  payment_Terms?: string
  rating?: number
  notes?: string
  created_At?: string | null
  updated_At?: string | null
}

// Props & Emits
interface Props {
  modelValue: boolean
  supplier?: SupplierDto | null
}

interface Emits {
  (e: 'update:modelValue', value: boolean): void
  (e: 'edit-supplier', supplier: SupplierDto): void
  (e: 'toggle-status', supplier: SupplierDto): void
  (e: 'view-history', supplier: SupplierDto): void
}

const props = defineProps<Props>()
const emit = defineEmits<Emits>()

// Local state
const showSnackbar = ref<boolean>(false)
const snackbarMessage = ref<string>('')

// Computed properties
const hasBusinessInfo = computed(() => {
  return props.supplier?.tax_Code || 
         props.supplier?.website || 
         props.supplier?.payment_Terms || 
         props.supplier?.rating
})

const hasTimestamps = computed(() => {
  return props.supplier?.created_At || props.supplier?.updated_At
})

// Methods
const getStatusColor = (status?: string): string => {
  switch (status) {
    case 'active': return 'success'
    case 'inactive': return 'error'
    case 'pending': return 'warning'
    default: return 'grey'
  }
}

const getStatusVariant = (status?: string): 'flat' | 'outlined' => {
  return status === 'active' ? 'flat' : 'outlined'
}

const getStatusIcon = (status?: string): string => {
  switch (status) {
    case 'active': return 'mdi-check-circle'
    case 'inactive': return 'mdi-cancel'
    case 'pending': return 'mdi-clock'
    default: return 'mdi-help'
  }
}

const getStatusText = (status?: string): string => {
  switch (status) {
    case 'active': return 'Đang hợp tác'
    case 'inactive': return 'Không hoạt động'
    case 'pending': return 'Chờ duyệt'
    default: return 'Không xác định'
  }
}

const getPaymentTermsText = (terms?: string): string => {
  switch (terms) {
    case 'immediate': return 'Thanh toán ngay'
    case '7_days': return '7 ngày'
    case '15_days': return '15 ngày'
    case '30_days': return '30 ngày'
    case '45_days': return '45 ngày'
    case '60_days': return '60 ngày'
    default: return terms || 'Không xác định'
  }
}

const formatWebsiteUrl = (website: string): string => {
  if (!website) return '#'
  if (website.startsWith('http://') || website.startsWith('https://')) {
    return website
  }
  return `https://${website}`
}

const formatDateTime = (dateString: string): string => {
  try {
    return new Date(dateString).toLocaleString('vi-VN', {
      year: 'numeric',
      month: '2-digit',
      day: '2-digit',
      hour: '2-digit',
      minute: '2-digit'
    })
  } catch {
    return dateString
  }
}

const copyContact = async (): Promise<void> => {
  if (!props.supplier) return
  
  const contactInfo = `
    Tên: ${props.supplier.supplier_Name}
    Người liên hệ: ${props.supplier.contact_Person}
    Điện thoại: ${props.supplier.phone_Number}
    Email: ${props.supplier.email}
    Địa chỉ: ${props.supplier.address}
  `.trim()

  try {
    await navigator.clipboard.writeText(contactInfo)
    showSuccess('Đã sao chép thông tin liên hệ')
  } catch {
    showSuccess('Không thể sao chép thông tin')
  }
}

const openEdit = (): void => {
  if (props.supplier) {
    emit('edit-supplier', props.supplier)
    closeDialog()
  }
}

const toggleStatus = (): void => {
  if (props.supplier) {
    emit('toggle-status', props.supplier)
  }
}

const closeDialog = (): void => {
  emit('update:modelValue', false)
}

const showSuccess = (message: string): void => {
  snackbarMessage.value = message
  showSnackbar.value = true
}
</script>

<style scoped>
.supplier-detail-modal {
  @apply shadow-2xl;
}

.info-section {
  @apply bg-white border border-gray-100 rounded-lg p-6;
}

.section-title {
  @apply text-lg font-semibold text-gray-800 mb-4 pb-2 border-b border-gray-100 flex items-center;
}

.info-group {
  @apply space-y-1;
}

.info-label {
  @apply text-sm font-medium text-gray-600 block;
}

.info-value {
  @apply text-base font-medium text-gray-900;
}

.contact-link {
  /* @apply text-primary hover:text-primary-dark transition-colors duration-200 inline-flex items-center; */
  text-decoration: none;
}

.contact-link:hover {
  text-decoration: underline;
}

.address-value {
  @apply flex items-start bg-gray-50 p-3 rounded-lg;
}

.tax-code {
  @apply font-mono bg-gray-50 px-3 py-1 rounded;
}

.notes-content {
  /* @apply text-gray-700 bg-gray-50 p-4 rounded-lg border-l-4 border-info leading-relaxed; */
  white-space: pre-wrap;
}

.timestamp {
  @apply text-sm flex items-center text-gray-600;
}

/* Responsive adjustments */
@media (max-width: 768px) {
  .grid {
    @apply grid-cols-1;
  }
  
  .info-section {
    @apply p-4;
  }
  
  .section-title {
    @apply text-base;
  }
  
  .flex-wrap {
    @apply flex-col;
  }
}

/* Custom scrollbar */
:deep(.v-card-text::-webkit-scrollbar) {
  width: 6px;
}

:deep(.v-card-text::-webkit-scrollbar-track) {
  @apply bg-gray-100 rounded-full;
}

:deep(.v-card-text::-webkit-scrollbar-thumb) {
  @apply bg-gray-300 rounded-full hover:bg-gray-400;
}
</style>