<template>
  <v-dialog
    :model-value="modelValue"
    @update:model-value="$emit('update:modelValue', $event)"
    max-width="900px"
    persistent
  >
    <v-card class="rounded-lg" v-if="customer">
      <!-- Header -->
      <v-card-title class="bg-[#11c393] text-white flex items-center justify-between">
        <div class="flex items-center gap-3">
          <v-avatar size="40" color="white">
            <v-icon color="primary" size="24">
              {{ getGenderIcon(customer.gender) }}
            </v-icon>
          </v-avatar>
          <div>
            <h3 class="text-lg font-semibold">{{ customer.fullName }}</h3>
            <p class="text-sm opacity-90">{{ customer.customerCode }}</p>
          </div>
        </div>
        
        <div class="flex gap-2">
          <v-btn
            icon
            variant="text"
            class="text-white action-btn"
            @click="$emit('edit', customer)"
          >
            <v-icon size="20">mdi-pencil</v-icon>
          </v-btn>
          
          <v-btn
            icon
            variant="text"
            class="text-white action-btn"
            @click="$emit('update:modelValue', false)"
          >
            <v-icon size="20">mdi-close</v-icon>
          </v-btn>
        </div>
      </v-card-title>

      <v-card-text class="p-0">
        <!-- Statistics Row -->
        <div class="p-6 bg-gradient-to-r from-gray-50 to-blue-50 border-b">
          <div class="grid grid-cols-2 md:grid-cols-4 gap-4">
            <div class="text-center p-4 bg-white rounded-lg shadow-sm">
              <p class="text-2xl font-bold text-blue-600">{{ customer.loyaltyPoints || 0 }}</p>
              <p class="text-sm text-gray-600">Điểm tích lũy</p>
            </div>
            <div class="text-center p-4 bg-white rounded-lg shadow-sm">
              <p class="text-2xl font-bold text-green-600">{{ formatCurrency(customer.totalSpent || 0) }}</p>
              <p class="text-sm text-gray-600">Tổng chi tiêu</p>
            </div>
            <div class="text-center p-4 bg-white rounded-lg shadow-sm">
              <p class="text-2xl font-bold text-purple-600">{{ calculateAge(customer.dateOfBirth) }}</p>
              <p class="text-sm text-gray-600">Tuổi</p>
            </div>
            <div class="text-center p-4 bg-white rounded-lg shadow-sm">
              <p class="text-2xl font-bold text-orange-600">{{ daysSinceLastVisit }}</p>
              <p class="text-sm text-gray-600">Ngày chưa đến</p>
            </div>
          </div>
        </div>

        <!-- Tabs -->
        <v-tabs v-model="activeTab" class="border-b bg-gray-50" density="compact">
          <v-tab value="info" class="tab-item">
            <v-icon size="16" class="mr-2">mdi-account</v-icon>
            <span class="text-xs">Thông tin cá nhân</span>
          </v-tab>
          <v-tab value="medical" class="tab-item">
            <v-icon size="16" class="mr-2">mdi-medical-bag</v-icon>
            <span class="text-xs">Thông tin y tế</span>
          </v-tab>
          <v-tab value="history" class="tab-item">
            <v-icon size="16" class="mr-2">mdi-history</v-icon>
            <span class="text-xs">Lịch sử mua hàng</span>
          </v-tab>
        </v-tabs>

        <v-tabs-window v-model="activeTab">
          <!-- Personal Information Tab -->
          <v-tab-item value="info">
            <div class="p-6">
              <div class="grid grid-cols-1 md:grid-cols-2 gap-8">
                <!-- Basic Info -->
                <div class="info-section">
                  <h4 class="section-title">Thông tin cơ bản</h4>
                  <div class="info-grid">
                    <div class="info-item">
                      <span class="info-label">Họ tên:</span>
                      <span class="info-value">{{ customer.fullName }}</span>
                    </div>
                    <div class="info-item">
                      <span class="info-label">Ngày sinh:</span>
                      <span class="info-value">{{ formatDate(customer.dateOfBirth) }}</span>
                    </div>
                    <div class="info-item">
                      <span class="info-label">Giới tính:</span>
                      <span class="info-value">{{ getGenderText(customer.gender) }}</span>
                    </div>
                    <div class="info-item">
                      <span class="info-label">Trạng thái:</span>
                      <v-chip
                        :color="getStatusColor(customer.status)"
                        variant="tonal"
                        size="small"
                        class="font-medium"
                      >
                        {{ getStatusText(customer.status) }}
                      </v-chip>
                    </div>
                    <div class="info-item">
                      <span class="info-label">Ngày tạo:</span>
                      <span class="info-value">{{ formatDate(customer.createdAt) }}</span>
                    </div>
                  </div>
                </div>

                <!-- Contact Info -->
                <div class="info-section">
                  <h4 class="section-title">Thông tin liên hệ</h4>
                  <div class="info-grid">
                    <div class="info-item">
                      <span class="info-label">Điện thoại:</span>
                      <span class="info-value font-medium text-blue-600">{{ customer.phone }}</span>
                    </div>
                    <div class="info-item">
                      <span class="info-label">Email:</span>
                      <span class="info-value">{{ customer.email || 'Chưa cập nhật' }}</span>
                    </div>
                    <div v-if="customer.address" class="info-item">
                      <span class="info-label">Địa chỉ:</span>
                      <div class="info-value">
                        <p>{{ customer.address.street }}</p>
                        <p class="text-gray-600">
                          {{ customer.address.ward }}, {{ customer.address.district }}
                        </p>
                        <p class="text-gray-600">{{ customer.address.city }}</p>
                      </div>
                    </div>
                  </div>
                </div>

                <!-- Documents -->
                <div class="info-section">
                  <h4 class="section-title">Giấy tờ tùy thân</h4>
                  <div class="info-grid">
                    <div v-if="customer.identityCard" class="info-item">
                      <span class="info-label">CMND/CCCD:</span>
                      <span class="info-value">{{ customer.identityCard.number }}</span>
                    </div>
                    <div v-if="customer.identityCard?.issuedDate" class="info-item">
                      <span class="info-label">Ngày cấp:</span>
                      <span class="info-value">{{ formatDate(customer.identityCard.issuedDate) }}</span>
                    </div>
                    <div v-if="customer.identityCard?.issuedPlace" class="info-item">
                      <span class="info-label">Nơi cấp:</span>
                      <span class="info-value">{{ customer.identityCard.issuedPlace }}</span>
                    </div>
                  </div>
                </div>

                <!-- Insurance -->
                <div class="info-section">
                  <h4 class="section-title">Bảo hiểm y tế</h4>
                  <div class="info-grid">
                    <div v-if="customer.insurance?.cardNumber" class="info-item">
                      <span class="info-label">Số thẻ BHYT:</span>
                      <span class="info-value">{{ customer.insurance.cardNumber }}</span>
                    </div>
                    <div v-if="customer.insurance?.validUntil" class="info-item">
                      <span class="info-label">Có hiệu lực đến:</span>
                      <span class="info-value">{{ formatDate(customer.insurance.validUntil) }}</span>
                    </div>
                    <div v-if="customer.insurance?.provider" class="info-item">
                      <span class="info-label">Nhà cung cấp:</span>
                      <span class="info-value">{{ customer.insurance.provider }}</span>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </v-tab-item>

          <!-- Medical Information Tab -->
          <v-tab-item value="medical">
            <div class="p-6">
              <div class="grid grid-cols-1 md:grid-cols-2 gap-8">
                <!-- Allergies -->
                <div class="info-section">
                  <h4 class="section-title">Dị ứng thuốc</h4>
                  <div v-if="customer.medicalInfo?.allergies && customer.medicalInfo.allergies.length > 0">
                    <div v-for="allergy in customer.medicalInfo.allergies" :key="allergy.id" 
                         class="p-3 bg-red-50 border border-red-200 rounded-lg mb-3">
                      <div class="flex items-center justify-between">
                        <div>
                          <p class="font-medium text-red-700">{{ allergy.substance }}</p>
                          <p class="text-sm text-red-600">{{ allergy.reaction }}</p>
                        </div>
                        <v-chip color="error" size="small" variant="tonal">
                          {{ getSeverityText(allergy.severity) }}
                        </v-chip>
                      </div>
                      <p v-if="allergy.notes" class="text-sm text-gray-600 mt-2">{{ allergy.notes }}</p>
                    </div>
                  </div>
                  <div v-else class="text-gray-500 text-center py-4">
                    Không có thông tin dị ứng
                  </div>
                </div>

                <!-- Medical Conditions -->
                <div class="info-section">
                  <h4 class="section-title">Tiền sử bệnh</h4>
                  <div v-if="customer.medicalInfo?.conditions && customer.medicalInfo.conditions.length > 0">
                    <div v-for="condition in customer.medicalInfo.conditions" :key="condition.id"
                         class="p-3 bg-orange-50 border border-orange-200 rounded-lg mb-3">
                      <div class="flex items-center justify-between">
                        <p class="font-medium text-orange-700">{{ condition.name }}</p>
                        <v-chip 
                          :color="condition.isActive ? 'warning' : 'success'" 
                          size="small" 
                          variant="tonal"
                        >
                          {{ condition.isActive ? 'Đang điều trị' : 'Đã khỏi' }}
                        </v-chip>
                      </div>
                      <p v-if="condition.diagnosedDate" class="text-sm text-gray-600">
                        Chẩn đoán: {{ formatDate(condition.diagnosedDate) }}
                      </p>
                      <p v-if="condition.notes" class="text-sm text-gray-600 mt-2">{{ condition.notes }}</p>
                    </div>
                  </div>
                  <div v-else class="text-gray-500 text-center py-4">
                    Không có tiền sử bệnh
                  </div>
                </div>

                <!-- Current Medications -->
                <div class="info-section md:col-span-2">
                  <h4 class="section-title">Thuốc đang sử dụng</h4>
                  <div v-if="customer.medicalInfo?.currentMedications && customer.medicalInfo.currentMedications.length > 0">
                    <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
                      <div v-for="medication in customer.medicalInfo.currentMedications" :key="medication.id"
                           class="p-4 bg-blue-50 border border-blue-200 rounded-lg">
                        <div class="flex justify-between items-start mb-2">
                          <h5 class="font-medium text-blue-700">{{ medication.drugName }}</h5>
                          <v-chip color="blue" size="small" variant="tonal">
                            {{ medication.dosage }}
                          </v-chip>
                        </div>
                        <p class="text-sm text-gray-600">{{ medication.frequency }}</p>
                        <p v-if="medication.prescribedBy" class="text-sm text-gray-600">
                          Kê bởi: {{ medication.prescribedBy }}
                        </p>
                        <p v-if="medication.startDate" class="text-sm text-gray-600">
                          Từ: {{ formatDate(medication.startDate) }}
                        </p>
                      </div>
                    </div>
                  </div>
                  <div v-else class="text-gray-500 text-center py-4">
                    Không có thuốc đang sử dụng
                  </div>
                </div>
              </div>
            </div>
          </v-tab-item>

          <!-- Purchase History Tab -->
          <v-tab-item value="history">
            <div class="p-6">
              <div class="flex justify-between items-center mb-4">
                <h4 class="section-title">Lịch sử mua hàng</h4>
                <div class="flex gap-2">
                  <v-btn
                    class="history-btn-secondary"
                    size="small"
                    @click="exportHistory"
                  >
                    <v-icon size="16" class="mr-1">mdi-download</v-icon>
                    <span class="text-xs">Xuất Excel</span>
                  </v-btn>
                </div>
              </div>

              <!-- Purchase History Table -->
              <v-data-table
                :headers="historyHeaders"
                :items="purchaseHistory"
                :loading="isLoadingHistory"
                :items-per-page="5"
                class="history-table"
                density="compact"
              >
                <template #item.date="{ item }">
                  <span class="text-sm">{{ formatDateTime(item.date) }}</span>
                </template>

                <template #item.total="{ item }">
                  <span class="font-medium text-green-600">{{ formatCurrency(item.total) }}</span>
                </template>

                <template #item.status="{ item }">
                  <v-chip
                    :color="getPurchaseStatusColor(item.status)"
                    variant="tonal"
                    size="small"
                  >
                    {{ getPurchaseStatusText(item.status) }}
                  </v-chip>
                </template>

                <template #item.actions="{ item }">
                  <div class="flex gap-1">
                    <v-btn
                      icon
                      size="small"
                      variant="plain"
                      class="action-btn"
                      @click="viewPurchaseDetail(item)"
                    >
                      <v-icon size="16" color="primary">mdi-eye</v-icon>
                    </v-btn>
                    <v-btn
                      icon
                      size="small"
                      variant="plain"
                      class="action-btn"
                      @click="printReceipt(item)"
                    >
                      <v-icon size="16" color="grey">mdi-printer</v-icon>
                    </v-btn>
                  </div>
                </template>

                <template #no-data>
                  <div class="text-center py-8 text-gray-500">
                    <v-icon size="48" color="grey-lighten-2">mdi-cart-off</v-icon>
                    <p class="mt-2">Chưa có lịch sử mua hàng</p>
                  </div>
                </template>
              </v-data-table>
            </div>
          </v-tab-item>
        </v-tabs-window>
      </v-card-text>
    </v-card>
  </v-dialog>
</template>

<script setup lang="ts">
import { computed, ref, watch } from 'vue';
import type { Customer, TableHeader, CustomerStatus, PurchaseHistory, CustomerGender } from '@/models/customers';

// Props & Emits
interface Props {
  modelValue: boolean;
  customer?: Customer | null;
}

interface Emits {
  (e: 'update:modelValue', value: boolean): void;
  (e: 'edit', customer: Customer): void;
}

const props = defineProps<Props>();
const emit = defineEmits<Emits>();

// Local state
const activeTab = ref('info');
const isLoadingHistory = ref(false);
const purchaseHistory = ref<PurchaseHistory[]>([]);

// Table headers for purchase history
const historyHeaders = ref<TableHeader[]>([
  { title: 'Mã đơn', key: 'orderCode', width: '120px' },
  { title: 'Ngày mua', key: 'date', width: '150px' },
  { title: 'Tổng tiền', key: 'total', width: '120px', align: 'end' },
  { title: 'Trạng thái', key: 'status', width: '120px', align: 'center' },
  { title: 'Thao tác', key: 'actions', width: '100px', align: 'center', sortable: false }
]);

// Computed properties
const daysSinceLastVisit = computed(() => {
  if (!props.customer?.lastVisit) return 'N/A';
  const today = new Date();
  const lastVisit = new Date(props.customer.lastVisit);
  const diffTime = Math.abs(today.getTime() - lastVisit.getTime());
  const diffDays = Math.ceil(diffTime / (1000 * 60 * 60 * 24));
  return diffDays;
});

// Methods
const getGenderIcon = (gender: CustomerGender): string => {
  switch (gender) {
    case 'male': return 'mdi-account';
    case 'female': return 'mdi-account-outline';
    default: return 'mdi-account-question';
  }
};

const getGenderText = (gender: CustomerGender): string => {
  switch (gender) {
    case 'male': return 'Nam';
    case 'female': return 'Nữ';
    default: return 'Khác';
  }
};

const getStatusColor = (status: CustomerStatus): string => {
  switch (status) {
    case 'active': return 'success';
    case 'inactive': return 'grey';
    case 'blocked': return 'error';
    default: return 'grey';
  }
};

const getStatusText = (status: CustomerStatus): string => {
  switch (status) {
    case 'active': return 'Hoạt động';
    case 'inactive': return 'Không hoạt động';
    case 'blocked': return 'Bị khóa';
    default: return 'Không xác định';
  }
};

const getSeverityText = (severity: AllergySeverity): string => {
  switch (severity) {
    case 'mild': return 'Nhẹ';
    case 'moderate': return 'Vừa';
    case 'severe': return 'Nặng';
    default: return 'Không rõ';
  }
};

const getPurchaseStatusColor = (status: PurchaseStatus): string => {
  switch (status) {
    case 'completed': return 'success';
    case 'pending': return 'warning';
    case 'cancelled': return 'error';
    case 'returned': return 'info';
    default: return 'grey';
  }
};

const getPurchaseStatusText = (status: PurchaseStatus): string => {
  switch (status) {
    case 'completed': return 'Hoàn thành';
    case 'pending': return 'Chờ xử lý';
    case 'cancelled': return 'Đã hủy';
    case 'returned': return 'Đã trả';
    default: return 'Không rõ';
  }
};

const calculateAge = (dateOfBirth: Date): number => {
  const today = new Date();
  const birthDate = new Date(dateOfBirth);
  let age = today.getFullYear() - birthDate.getFullYear();
  const monthDiff = today.getMonth() - birthDate.getMonth();
  
  if (monthDiff < 0 || (monthDiff === 0 && today.getDate() < birthDate.getDate())) {
    age--;
  }
  
  return age;
};

const formatDate = (date: Date): string => {
  return new Intl.DateTimeFormat('vi-VN', {
    day: '2-digit',
    month: '2-digit',
    year: 'numeric'
  }).format(new Date(date));
};

const formatDateTime = (date: Date): string => {
  return new Intl.DateTimeFormat('vi-VN', {
    day: '2-digit',
    month: '2-digit',
    year: 'numeric',
    hour: '2-digit',
    minute: '2-digit'
  }).format(new Date(date));
};

const formatCurrency = (amount: number): string => {
  return new Intl.NumberFormat('vi-VN', {
    style: 'currency',
    currency: 'VND'
  }).format(amount);
};

const loadPurchaseHistory = async (): Promise<void> => {
  if (!props.customer) return;
  
  isLoadingHistory.value = true;
  try {
    // Mock API call - replace with actual API
    await new Promise(resolve => setTimeout(resolve, 1000));
    
    // Mock data
    purchaseHistory.value = [
      {
        id: '1',
        orderCode: 'HD001',
        date: new Date('2024-08-01'),
        total: 250000,
        status: 'completed'
      },
      {
        id: '2',
        orderCode: 'HD002',
        date: new Date('2024-07-15'),
        total: 180000,
        status: 'completed'
      },
      {
        id: '3',
        orderCode: 'HD003',
        date: new Date('2024-07-01'),
        total: 95000,
        status: 'returned'
      }
    ];
  } catch (error) {
    console.error('Error loading purchase history:', error);
  } finally {
    isLoadingHistory.value = false;
  }
};

const viewPurchaseDetail = (purchase: PurchaseHistory): void => {
  // Logic to view purchase detail
  console.log('View purchase detail:', purchase);
};

const printReceipt = (purchase: PurchaseHistory): void => {
  // Logic to print receipt
  console.log('Print receipt:', purchase);
};

const exportHistory = (): void => {
  // Logic to export purchase history
  console.log('Export purchase history');
};

// Watchers
watch(() => props.modelValue, (isOpen) => {
  if (isOpen && props.customer) {
    activeTab.value = 'info';
    loadPurchaseHistory();
  }
});
</script>

<style scoped>
.action-btn {
  @apply hover:bg-white/20 rounded-full w-8 h-8;
  transition: all 0.2s ease;
}

.action-btn:hover {
  transform: scale(1.1);
}

.tab-item {
  @apply text-gray-600 hover:text-[#11c393] transition-colors duration-200;
  min-width: auto !important;
}

.info-section {
  @apply bg-gray-50 rounded-lg p-4;
}

.section-title {
  @apply font-semibold text-gray-700 mb-4 pb-2 border-b border-gray-200;
}

.info-grid {
  @apply space-y-3;
}

.info-item {
  @apply flex justify-between items-start py-2;
}

.info-label {
  @apply text-gray-600 text-sm font-medium min-w-[120px];
}

.info-value {
  @apply text-gray-800 text-sm text-right flex-1;
}

.history-btn-secondary {
  @apply rounded px-4 h-8 font-medium text-[#11c393] bg-white border border-[#11c393] hover:bg-[#11c393] hover:text-white tracking-wide transition-all duration-200;
  min-width: auto !important;
}

.history-table {
  @apply rounded-lg shadow-sm border border-gray-200 overflow-hidden;
  background: white;
}

.history-table :deep(thead th) {
  @apply font-semibold text-gray-700 bg-gray-50 text-sm;
  border-bottom: 2px solid #e5e7eb;
  padding: 12px 8px;
  height: 48px;
}

.history-table :deep(tbody tr) {
  @apply hover:bg-blue-50/40 transition-colors duration-150;
  border-bottom: 1px solid #f1f5f9;
}

.history-table :deep(tbody td) {
  @apply px-2 py-3 text-sm;
  border-bottom: 1px solid #f1f5f9;
}

.action-btn {
  @apply hover:bg-gray-50 rounded-full w-7 h-7;
  transition: all 0.2s ease;
}

.action-btn:hover {
  transform: scale(1.1);
}
</style>