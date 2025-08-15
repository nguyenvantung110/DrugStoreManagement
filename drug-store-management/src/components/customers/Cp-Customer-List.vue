<template>
  <v-container class="px-2 md:px-8 py-4">
    <v-card elevation="0" class="p-5 rounded-lg bg-white">
      <!-- Header Section -->
      <div class="flex flex-col md:flex-row md:items-center md:justify-between mb-6 gap-4">
        <div>
          <h2 class="text-xl font-bold text-gray-800 mb-1">Quản lý khách hàng</h2>
          <p class="text-gray-500 text-sm">Quản lý thông tin và lịch sử mua hàng của khách hàng</p>
        </div>
        
        <div class="flex gap-2 flex-wrap">
          <v-btn
            class="customer-btn-primary"
            elevation="0"
            density="compact"
            size="small"
            @click="openCreateDialog"
          >
            <v-icon size="16" class="mr-1">mdi-account-plus</v-icon>
            <span class="text-xs">Thêm khách hàng</span>
          </v-btn>
          
          <v-btn
            class="customer-btn-secondary"
            elevation="0"
            density="compact"
            size="small"
            @click="openSearchDialog"
          >
            <v-icon size="16" class="mr-1">mdi-phone-outline</v-icon>
            <span class="text-xs">Tìm theo SĐT</span>
          </v-btn>
          
          <v-btn
            class="customer-btn-secondary"
            elevation="0"
            density="compact"
            size="small"
            @click="refreshCustomers"
            :loading="isLoading"
          >
            <v-icon size="16" class="mr-1">mdi-refresh</v-icon>
            <span class="text-xs">Làm mới</span>
          </v-btn>
        </div>
      </div>

      <!-- Statistics Cards -->
      <div class="grid grid-cols-1 md:grid-cols-4 gap-4 mb-6">
        <v-card class="stat-card" elevation="1">
          <v-card-text class="p-4">
            <div class="flex items-center justify-between">
              <div>
                <p class="text-sm text-gray-600">Tổng khách hàng</p>
                <p class="text-2xl font-bold text-blue-600">{{ totalCustomers }}</p>
              </div>
              <v-icon color="blue" size="32">mdi-account-group</v-icon>
            </div>
          </v-card-text>
        </v-card>

        <v-card class="stat-card" elevation="1">
          <v-card-text class="p-4">
            <div class="flex items-center justify-between">
              <div>
                <p class="text-sm text-gray-600">Đang hoạt động</p>
                <p class="text-2xl font-bold text-green-600">{{ activeCustomersCount }}</p>
              </div>
              <v-icon color="green" size="32">mdi-account-check</v-icon>
            </div>
          </v-card-text>
        </v-card>

        <v-card class="stat-card" elevation="1">
          <v-card-text class="p-4">
            <div class="flex items-center justify-between">
              <div>
                <p class="text-sm text-gray-600">Điểm tích lũy</p>
                <p class="text-2xl font-bold text-orange-600">{{ totalLoyaltyPoints }}</p>
              </div>
              <v-icon color="orange" size="32">mdi-star</v-icon>
            </div>
          </v-card-text>
        </v-card>

        <v-card class="stat-card" elevation="1">
          <v-card-text class="p-4">
            <div class="flex items-center justify-between">
              <div>
                <p class="text-sm text-gray-600">Khách VIP</p>
                <p class="text-2xl font-bold text-purple-600">{{ topCustomers.length }}</p>
              </div>
              <v-icon color="purple" size="32">mdi-crown</v-icon>
            </div>
          </v-card-text>
        </v-card>
      </div>

      <!-- Filter Section -->
      <div class="mb-4 grid grid-cols-1 md:grid-cols-5 gap-3">
        <v-text-field
          v-model="searchQuery"
          density="compact"
          variant="outlined"
          placeholder="Tìm kiếm khách hàng..."
          hide-details
          clearable
          class="filter-input"
          @input="debouncedSearch"
        >
          <template #prepend-inner>
            <v-icon size="20" color="grey">mdi-magnify</v-icon>
          </template>
        </v-text-field>

        <v-select
          v-model="selectedGender"
          :items="genderOptions"
          item-title="text"
          item-value="value"
          density="compact"
          variant="outlined"
          placeholder="Chọn giới tính"
          hide-details
          clearable
          class="filter-input"
          @update:model-value="applyFilters"
        />

        <v-select
          v-model="selectedStatus"
          :items="statusOptions"
          item-title="text"
          item-value="value"
          density="compact"
          variant="outlined"
          placeholder="Chọn trạng thái"
          hide-details
          clearable
          class="filter-input"
          @update:model-value="applyFilters"
        />

        <v-select
          v-model="selectedAgeRange"
          :items="ageRangeOptions"
          item-title="text"
          item-value="value"
          density="compact"
          variant="outlined"
          placeholder="Chọn độ tuổi"
          hide-details
          clearable
          class="filter-input"
          @update:model-value="applyFilters"
        />

        <v-btn
          class="customer-btn-secondary h-10"
          elevation="0"
          block
          @click="clearFilters"
        >
          <v-icon size="16" class="mr-1">mdi-filter-off</v-icon>
          <span class="text-xs">Xóa bộ lọc</span>
        </v-btn>
      </div>

      <!-- Customers Table -->
      <v-data-table
        :headers="tableHeaders"
        :items="customersList"
        :loading="isLoading"
        :items-per-page="10"
        class="customer-table"
        density="compact"
      >
        <template #item.customerCode="{ item }">
          <span class="font-mono text-sm font-medium text-blue-600">{{ item.customerCode }}</span>
        </template>

        <template #item.fullName="{ item }">
          <div class="flex items-center gap-2">
            <v-avatar size="28" :color="getGenderColor(item.gender)">
              <v-icon color="white" size="14">
                {{ getGenderIcon(item.gender) }}
              </v-icon>
            </v-avatar>
            <div>
              <p class="font-medium text-sm">{{ item.fullName }}</p>
              <p class="text-xs text-gray-500">{{ calculateAge(item.dateOfBirth) }} tuổi</p>
            </div>
          </div>
        </template>

        <template #item.contact="{ item }">
          <div class="text-sm">
            <p class="font-medium">{{ item.phone }}</p>
            <p class="text-gray-500">{{ item.email || 'Chưa có email' }}</p>
          </div>
        </template>

        <template #item.address="{ item }">
          <div class="text-sm text-gray-600">
            <p v-if="item.address">{{ item.address.district }}, {{ item.address.city }}</p>
            <p v-else class="text-gray-400 italic">Chưa cập nhật</p>
          </div>
        </template>

        <template #item.loyaltyInfo="{ item }">
          <div class="text-center">
            <v-chip
              :color="getLoyaltyColor(item.loyaltyPoints)"
              variant="tonal"
              size="small"
              class="mb-1"
            >
              {{ item.loyaltyPoints }} điểm
            </v-chip>
            <p class="text-xs text-gray-500">{{ formatCurrency(item.totalSpent) }}</p>
          </div>
        </template>

        <template #item.status="{ item }">
          <v-chip
            :color="getStatusColor(item.status)"
            variant="tonal"
            size="small"
            class="font-medium"
          >
            {{ getStatusText(item.status) }}
          </v-chip>
        </template>

        <template #item.lastVisit="{ item }">
          <span class="text-sm text-gray-600">
            {{ item.lastVisit ? formatDateTime(item.lastVisit) : 'Chưa có' }}
          </span>
        </template>

        <template #item.actions="{ item }">
          <div class="flex gap-1">
            <v-btn
              icon
              size="small"
              variant="plain"
              class="action-btn"
              @click="viewCustomerDetail(item)"
            >
              <v-icon size="16" color="info">mdi-eye</v-icon>
            </v-btn>
            
            <v-btn
              icon
              size="small"
              variant="plain"
              class="action-btn"
              @click="openEditDialog(item)"
            >
              <v-icon size="16" color="primary">mdi-pencil</v-icon>
            </v-btn>
            
            <v-btn
              icon
              size="small"
              variant="plain"
              class="action-btn"
              @click="toggleCustomerStatus(item)"
            >
              <v-icon size="16" :color="item.status === 'active' ? 'orange' : 'green'">
                {{ item.status === 'active' ? 'mdi-pause' : 'mdi-play' }}
              </v-icon>
            </v-btn>
            
            <v-btn
              icon
              size="small"
              variant="plain"
              class="action-btn action-delete"
              @click="confirmDeleteCustomer(item)"
            >
              <v-icon size="16" color="error">mdi-delete</v-icon>
            </v-btn>
          </div>
        </template>

        <template #no-data>
          <div class="text-center py-8 text-gray-500">
            <v-icon size="48" color="grey-lighten-2">mdi-account-off</v-icon>
            <p class="mt-2">Không tìm thấy khách hàng nào</p>
          </div>
        </template>
      </v-data-table>
    </v-card>

    <!-- Customer Form Dialog -->
    <Cp-Customer-Form
      v-model="showDialog"
      :customer="selectedCustomer"
      @save="handleSaveCustomer"
    />

    <!-- Customer Detail Dialog -->
    <Cp-Customer-Detail
      v-model="showDetailDialog"
      :customer="selectedCustomer"
      @edit="openEditDialog"
    />

    <!-- Phone Search Dialog -->
    <Cp-Phone-Search-Dialog
      v-model="showPhoneSearchDialog"
      @customer-found="handleCustomerFound"
    />

    <!-- Delete Confirmation Dialog -->
    <Cp-Confirm-Dialog
      v-model="showDeleteDialog"
      title="Xác nhận xóa khách hàng"
      :message="`Bạn có chắc chắn muốn xóa khách hàng '${customerToDelete?.fullName}'? Hành động này không thể hoàn tác.`"
      color="error"
      @confirm="handleDeleteCustomer"
    />
  </v-container>
</template>

<script setup lang="ts">
import { computed, onMounted, ref, watch } from 'vue';
import { useCustomersStore } from '@/composables/customers/customersStore';
import type { Customer, TableHeader, CustomerGender, CustomerStatus } from '@/models/customers';
import CpCustomerForm from './Cp-Customer-Form.vue';
import CpCustomerDetail from './Cp-Customer-Detail.vue';
import CpPhoneSearchDialog from './Cp-Phone-Search-Dialog.vue';

// Store
const customersStore = useCustomersStore();

// Local state
const searchQuery = ref('');
const selectedGender = ref<CustomerGender | ''>('');
const selectedStatus = ref<CustomerStatus | ''>('');
const selectedAgeRange = ref('');
const showDialog = ref(false);
const showDetailDialog = ref(false);
const showPhoneSearchDialog = ref(false);
const showDeleteDialog = ref(false);
const selectedCustomer = ref<Customer | null>(null);
const customerToDelete = ref<Customer | null>(null);

// Table configuration
const tableHeaders = ref<TableHeader[]>([
  { title: 'Mã KH', key: 'customerCode', width: '100px', sortable: true },
  { title: 'Thông tin KH', key: 'fullName', width: '200px', sortable: true },
  { title: 'Liên hệ', key: 'contact', width: '180px' },
  { title: 'Địa chỉ', key: 'address', width: '150px' },
  { title: 'Tích lũy', key: 'loyaltyInfo', width: '120px', align: 'center' },
  { title: 'Trạng thái', key: 'status', width: '100px', align: 'center' },
  { title: 'Lần cuối', key: 'lastVisit', width: '120px' },
  { title: 'Thao tác', key: 'actions', width: '140px', align: 'center', sortable: false }
]);

// Filter options
const genderOptions = ref([
  { text: 'Tất cả giới tính', value: '' },
  { text: 'Nam', value: 'male' },
  { text: 'Nữ', value: 'female' },
  { text: 'Khác', value: 'other' }
]);

const statusOptions = ref([
  { text: 'Tất cả trạng thái', value: '' },
  { text: 'Hoạt động', value: 'active' },
  { text: 'Không hoạt động', value: 'inactive' },
  { text: 'Đã chặn', value: 'blocked' }
]);

const ageRangeOptions = ref([
  { text: 'Tất cả độ tuổi', value: '' },
  { text: 'Dưới 18', value: '0-17' },
  { text: '18-30', value: '18-30' },
  { text: '31-50', value: '31-50' },
  { text: '51-65', value: '51-65' },
  { text: 'Trên 65', value: '66-999' }
]);

// Computed properties
const customersList = computed(() => customersStore.getCustomersList);
const isLoading = computed(() => customersStore.getIsLoading);
const totalCustomers = computed(() => customersStore.getTotalCustomers);
const activeCustomersCount = computed(() => customersStore.getActiveCustomersCount);
const totalLoyaltyPoints = computed(() => customersStore.getTotalLoyaltyPoints);
const topCustomers = computed(() => customersStore.getTopCustomers);

// Methods
const calculateAge = (dateOfBirth?: Date): number => {
  if (!dateOfBirth) return 0;
  const today = new Date();
  const birth = new Date(dateOfBirth);
  let age = today.getFullYear() - birth.getFullYear();
  const monthDiff = today.getMonth() - birth.getMonth();
  if (monthDiff < 0 || (monthDiff === 0 && today.getDate() < birth.getDate())) {
    age--;
  }
  return age;
};

const getGenderIcon = (gender: CustomerGender): string => {
  switch (gender) {
    case 'male': return 'mdi-gender-male';
    case 'female': return 'mdi-gender-female';
    default: return 'mdi-gender-male-female';
  }
};

const getGenderColor = (gender: CustomerGender): string => {
  switch (gender) {
    case 'male': return 'blue';
    case 'female': return 'pink';
    default: return 'grey';
  }
};

const getLoyaltyColor = (points: number): string => {
  if (points >= 200) return 'purple';
  if (points >= 100) return 'orange';
  if (points >= 50) return 'blue';
  return 'grey';
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
    case 'blocked': return 'Đã chặn';
    default: return 'Không xác định';
  }
};

const formatDateTime = (date: Date): string => {
  return new Intl.DateTimeFormat('vi-VN', {
    day: '2-digit',
    month: '2-digit',
    year: 'numeric'
  }).format(new Date(date));
};

const formatCurrency = (amount: number): string => {
  return new Intl.NumberFormat('vi-VN', {
    style: 'currency',
    currency: 'VND'
  }).format(amount);
};

const debouncedSearch = (() => {
  let timeout: NodeJS.Timeout;
  return () => {
    clearTimeout(timeout);
    timeout = setTimeout(() => {
      applyFilters();
    }, 500);
  };
})();

const applyFilters = (): void => {
  const ageRange = selectedAgeRange.value ? parseAgeRange(selectedAgeRange.value) : undefined;
  
  customersStore.updateFilter({
    search: searchQuery.value,
    gender: selectedGender.value || undefined,
    status: selectedStatus.value || undefined,
    ageRange: ageRange,
    page: 1
  });
  customersStore.fetchCustomers();
};

const parseAgeRange = (rangeString: string) => {
  const [min, max] = rangeString.split('-').map(Number);
  return { min, max };
};

const clearFilters = (): void => {
  searchQuery.value = '';
  selectedGender.value = '';
  selectedStatus.value = '';
  selectedAgeRange.value = '';
  applyFilters();
};

const refreshCustomers = (): void => {
  customersStore.fetchCustomers();
};

const openCreateDialog = (): void => {
  selectedCustomer.value = null;
  showDialog.value = true;
};

const openEditDialog = (customer: Customer): void => {
  selectedCustomer.value = customer;
  showDialog.value = true;
  showDetailDialog.value = false;
};

const viewCustomerDetail = (customer: Customer): void => {
  selectedCustomer.value = customer;
  showDetailDialog.value = true;
};

const openSearchDialog = (): void => {
  showPhoneSearchDialog.value = true;
};

const handleSaveCustomer = async (success: boolean): Promise<void> => {
  if (success) {
    showDialog.value = false;
    await customersStore.fetchCustomers();
  }
};

const handleCustomerFound = (customer: Customer): void => {
  viewCustomerDetail(customer);
  showPhoneSearchDialog.value = false;
};

const toggleCustomerStatus = async (customer: Customer): Promise<void> => {
  const newStatus: CustomerStatus = customer.status === 'active' ? 'inactive' : 'active';
  await customersStore.updateCustomer(customer.id, { status: newStatus });
};

const confirmDeleteCustomer = (customer: Customer): void => {
  customerToDelete.value = customer;
  showDeleteDialog.value = true;
};

const handleDeleteCustomer = async (): Promise<void> => {
  if (customerToDelete.value) {
    const success = await customersStore.deleteCustomer(customerToDelete.value.id);
    if (success) {
      showDeleteDialog.value = false;
      customerToDelete.value = null;
    }
  }
};

// Lifecycle
onMounted(() => {
  customersStore.fetchCustomers();
});
</script>

<style scoped>
.customer-btn-primary {
  @apply rounded px-4 h-8 font-medium text-white bg-[#11c393] hover:bg-[#10b18d] tracking-wide transition-all duration-200;
  min-width: auto !important;
}

.customer-btn-secondary {
  @apply rounded px-4 h-8 font-medium text-[#11c393] bg-white border border-[#11c393] hover:bg-[#11c393] hover:text-white tracking-wide transition-all duration-200;
  min-width: auto !important;
}

.stat-card {
  @apply rounded-lg border border-gray-200 hover:shadow-md transition-all duration-200;
}

.filter-input :deep(.v-field) {
  @apply rounded h-10;
  background: #f8fafc;
}

.filter-input :deep(.v-field--focused .v-field__outline) {
  @apply border-[#11c393];
  box-shadow: none !important;
}

.customer-table {
  @apply rounded-lg shadow-sm border border-gray-200 overflow-hidden;
  background: white;
}

.customer-table :deep(thead th) {
  @apply font-semibold text-gray-700 bg-gray-50 text-sm;
  border-bottom: 2px solid #e5e7eb;
  padding: 12px 8px;
  height: 48px;
}

.customer-table :deep(tbody tr) {
  @apply hover:bg-blue-50/40 transition-colors duration-150;
  border-bottom: 1px solid #f1f5f9;
}

.customer-table :deep(tbody td) {
  @apply px-2 py-2 text-sm !important;
  border-bottom: 1px solid #f1f5f9;
}

.action-btn {
  @apply hover:bg-gray-50 rounded-full w-7 h-7;
  transition: all 0.2s ease;
}

.action-btn:hover {
  transform: scale(1.1);
}

.action-delete:hover {
  @apply bg-red-50;
}
</style>