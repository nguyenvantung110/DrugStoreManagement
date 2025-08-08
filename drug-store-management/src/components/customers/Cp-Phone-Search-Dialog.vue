<template>
  <v-dialog
    :model-value="modelValue"
    @update:model-value="$emit('update:modelValue', $event)"
    max-width="500px"
    persistent
  >
    <v-card class="rounded-lg">
      <v-card-title class="bg-[#11c393] text-white">
        <div class="flex items-center gap-3">
          <v-icon size="24">mdi-phone-outline</v-icon>
          <span class="text-lg font-semibold">Tìm kiếm khách hàng</span>
        </div>
      </v-card-title>

      <v-card-text class="p-6">
        <!-- Phone Input -->
        <div class="mb-6">
          <v-text-field
            v-model="phoneNumber"
            label="Số điện thoại"
            placeholder="Nhập số điện thoại khách hàng..."
            :rules="phoneRules"
            density="compact"
            variant="outlined"
            class="phone-input"
            hide-details="auto"
            autofocus
            @keyup.enter="searchCustomer"
            @input="clearResults"
          >
            <template #prepend-inner>
              <v-icon size="20" color="grey">mdi-phone</v-icon>
            </template>
            <template #append-inner>
              <v-btn
                icon
                size="small"
                variant="plain"
                :disabled="!phoneNumber || isSearching"
                @click="searchCustomer"
              >
                <v-icon size="20" color="primary">mdi-magnify</v-icon>
              </v-btn>
            </template>
          </v-text-field>
        </div>

        <!-- Loading State -->
        <div v-if="isSearching" class="text-center py-8">
          <v-progress-circular
            indeterminate
            color="primary"
            size="40"
          />
          <p class="mt-3 text-gray-600">Đang tìm kiếm...</p>
        </div>

        <!-- Search Results -->
        <div v-else-if="searchResults.length > 0" class="search-results">
          <h4 class="text-md font-semibold text-gray-700 mb-4">
            Kết quả tìm kiếm ({{ searchResults.length }})
          </h4>
          
          <div class="space-y-3 max-h-[400px] overflow-y-auto">
            <div
              v-for="customer in searchResults"
              :key="customer.id"
              class="customer-item"
              @click="selectCustomer(customer)"
            >
              <div class="flex items-center gap-4 p-4 bg-white border border-gray-200 rounded-lg hover:border-[#11c393] hover:bg-gray-50 cursor-pointer transition-all duration-200">
                <v-avatar size="40" color="primary">
                  <v-icon color="white" size="20">
                    {{ getGenderIcon(customer.gender) }}
                  </v-icon>
                </v-avatar>
                
                <div class="flex-1">
                  <div class="flex justify-between items-start">
                    <div>
                      <h5 class="font-medium text-gray-800">{{ customer.fullName }}</h5>
                      <p class="text-sm text-gray-600">{{ customer.phone }}</p>
                    </div>
                    <div class="text-right">
                      <v-chip
                        :color="getStatusColor(customer.status)"
                        variant="tonal"
                        size="small"
                        class="mb-1"
                      >
                        {{ getStatusText(customer.status) }}
                      </v-chip>
                      <p class="text-xs text-gray-500">{{ customer.customerCode }}</p>
                    </div>
                  </div>
                  
                  <div class="mt-2 grid grid-cols-2 gap-2 text-xs">
                    <div class="flex items-center gap-1 text-gray-600">
                      <v-icon size="12">mdi-calendar</v-icon>
                      <span>{{ calculateAge(customer.dateOfBirth ?? new Date) }} tuổi</span>
                    </div>
                    <div class="flex items-center gap-1 text-gray-600">
                      <v-icon size="12">mdi-cash</v-icon>
                      <span>{{ formatCurrency(customer.totalSpent || 0) }}</span>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>

        <!-- No Results -->
        <div v-else-if="hasSearched && searchResults.length === 0" class="no-results">
          <div class="text-center py-8">
            <v-icon size="64" color="grey-lighten-2">mdi-account-search</v-icon>
            <h4 class="text-lg font-medium text-gray-700 mt-4 mb-2">
              Không tìm thấy khách hàng
            </h4>
            <p class="text-gray-600 mb-4">
              Không có khách hàng nào với số điện thoại "{{ phoneNumber }}"
            </p>
            
            <v-btn
              class="create-btn-primary"
              @click="createNewCustomer"
            >
              <v-icon size="16" class="mr-2">mdi-account-plus</v-icon>
              Tạo khách hàng mới
            </v-btn>
          </div>
        </div>

        <!-- Initial State -->
        <div v-else class="initial-state">
          <div class="text-center py-8 text-gray-500">
            <v-icon size="64" color="grey-lighten-2">mdi-phone-outline</v-icon>
            <h4 class="text-lg font-medium text-gray-700 mt-4 mb-2">
              Tìm kiếm khách hàng
            </h4>
            <p class="text-gray-600">
              Nhập số điện thoại để tìm kiếm thông tin khách hàng
            </p>
          </div>
        </div>

        <!-- Recent Searches -->
        <div v-if="recentSearches.length > 0 && !hasSearched" class="recent-searches mt-6">
          <h4 class="text-sm font-medium text-gray-700 mb-3">Tìm kiếm gần đây</h4>
          <div class="flex flex-wrap gap-2">
            <v-chip
              v-for="phone in recentSearches"
              :key="phone"
              size="small"
              variant="outlined"
              class="cursor-pointer hover:bg-gray-100"
              @click="phoneNumber = phone; searchCustomer()"
            >
              <v-icon size="14" class="mr-1">mdi-history</v-icon>
              {{ phone }}
            </v-chip>
          </div>
        </div>
      </v-card-text>

      <v-card-actions class="px-6 pb-6">
        <v-spacer />
        <v-btn
          class="dialog-btn-cancel"
          @click="handleCancel"
        >
          Hủy
        </v-btn>
        <v-btn
          v-if="selectedCustomer"
          class="dialog-btn-confirm"
          @click="handleConfirm"
        >
          Chọn khách hàng
        </v-btn>
      </v-card-actions>
    </v-card>
  </v-dialog>
</template>

<script setup lang="ts">
import { ref, watch } from 'vue';
import type { Customer, CustomerGender, CustomerStatus } from '@/models/customers';

// Props & Emits
interface Props {
  modelValue: boolean;
}

interface Emits {
  (e: 'update:modelValue', value: boolean): void;
  (e: 'select', customer: Customer): void;
  (e: 'create', phone: string): void;
}

const props = defineProps<Props>();
const emit = defineEmits<Emits>();

// Local state
const phoneNumber = ref('');
const isSearching = ref(false);
const hasSearched = ref(false);
const searchResults = ref<Customer[]>([]);
const selectedCustomer = ref<Customer | null>(null);
const recentSearches = ref<string[]>(['0901234567', '0987654321', '0912345678']);

// Validation rules
const phoneRules = [
  (v: string) => !!v || 'Số điện thoại là bắt buộc',
  (v: string) => /^[0-9]{10,11}$/.test(v) || 'Số điện thoại không hợp lệ'
];

// Methods
const searchCustomer = async (): Promise<void> => {
  if (!phoneNumber.value || !/^[0-9]{10,11}$/.test(phoneNumber.value)) {
    return;
  }

  isSearching.value = true;
  hasSearched.value = true;
  selectedCustomer.value = null;

  try {
    // Mock API call - replace with actual API
    await new Promise(resolve => setTimeout(resolve, 1500));

    // Mock search results
    const mockResults: Customer[] = [];
    
    // Simulate finding customer if phone matches certain patterns
    if (phoneNumber.value === '0901234567') {
      mockResults.push({
        id: '1',
        customerCode: 'KH001',
        fullName: 'Nguyễn Văn An',
        phone: '0901234567',
        email: 'nguyenvanan@email.com',
        dateOfBirth: new Date('1985-05-15'),
        gender: 'male',
        status: 'active',
        totalSpent: 1250000,
        loyaltyPoints: 125,
        createdAt: new Date('2024-01-15'),
        updatedAt: new Date('2024-08-01'),
        lastVisit: new Date('2024-08-01')
      });
    } else if (phoneNumber.value === '0987654321') {
      mockResults.push({
          id: '2',
          customerCode: 'KH002',
          fullName: 'Trần Thị Bình',
          phone: '0987654321',
          email: 'tranthibinh@email.com',
          dateOfBirth: new Date('1990-08-20'),
          gender: 'female',
          totalSpent: 5500000,
          loyaltyPoints: 550,
          createdAt: new Date('2023-12-01'),
          updatedAt: new Date('2024-07-28'),
          lastVisit: new Date('2024-07-28'),
          status: 'active'
      });
    }

    searchResults.value = mockResults;
    
    // Add to recent searches if not already present
    if (!recentSearches.value.includes(phoneNumber.value)) {
      recentSearches.value.unshift(phoneNumber.value);
      recentSearches.value = recentSearches.value.slice(0, 5); // Keep only 5 recent searches
    }

  } catch (error) {
    console.error('Error searching customer:', error);
    searchResults.value = [];
  } finally {
    isSearching.value = false;
  }
};

const selectCustomer = (customer: Customer): void => {
  selectedCustomer.value = customer;
};

const createNewCustomer = (): void => {
  emit('create', phoneNumber.value);
  handleCancel();
};

const clearResults = (): void => {
  searchResults.value = [];
  hasSearched.value = false;
  selectedCustomer.value = null;
};

const handleConfirm = (): void => {
  if (selectedCustomer.value) {
    emit('select', selectedCustomer.value);
    resetDialog();
    emit('update:modelValue', false);
  }
};

const handleCancel = (): void => {
  resetDialog();
  emit('update:modelValue', false);
};

const resetDialog = (): void => {
  phoneNumber.value = '';
  searchResults.value = [];
  hasSearched.value = false;
  selectedCustomer.value = null;
  isSearching.value = false;
};

// Utility methods
const getGenderIcon = (gender: CustomerGender): string => {
  switch (gender) {
    case 'male': return 'mdi-account';
    case 'female': return 'mdi-account-outline';
    default: return 'mdi-account-question';
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

const formatCurrency = (amount: number): string => {
  return new Intl.NumberFormat('vi-VN', {
    style: 'currency',
    currency: 'VND'
  }).format(amount);
};

// Watchers
watch(() => props.modelValue, (isOpen) => {
  if (isOpen) {
    resetDialog();
  }
});
</script>

<style scoped>
.phone-input :deep(.v-field) {
  @apply rounded;
  background: #f8fafc;
}

.phone-input :deep(.v-field--focused .v-field__outline) {
  @apply border-[#11c393];
  box-shadow: none !important;
}

.customer-item:hover {
  transform: translateY(-1px);
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);
}

.create-btn-primary {
  @apply rounded px-6 h-10 font-semibold text-white bg-[#11c393] hover:bg-[#10b18d] tracking-wide transition-all duration-200;
  min-width: auto !important;
}

.dialog-btn-cancel {
  @apply rounded px-6 h-10 font-semibold text-gray-600 bg-gray-100 hover:bg-gray-200 tracking-wide transition-all duration-200;
  min-width: auto !important;
}

.dialog-btn-confirm {
  @apply rounded px-6 h-10 font-semibold text-white bg-[#11c393] hover:bg-[#10b18d] tracking-wide transition-all duration-200;
  min-width: auto !important;
}

.search-results {
  animation: fadeIn 0.3s ease-in-out;
}

.no-results, .initial-state {
  animation: fadeIn 0.3s ease-in-out;
}

@keyframes fadeIn {
  from {
    opacity: 0;
    transform: translateY(10px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

/* Scrollbar styling */
.search-results div::-webkit-scrollbar {
  width: 6px;
}

.search-results div::-webkit-scrollbar-track {
  background: #f1f1f1;
  border-radius: 3px;
}

.search-results div::-webkit-scrollbar-thumb {
  background: #c1c1c1;
  border-radius: 3px;
}

.search-results div::-webkit-scrollbar-thumb:hover {
  background: #a1a1a1;
}
</style>