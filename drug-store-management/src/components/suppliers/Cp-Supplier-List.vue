<template>
  <v-container class="px-2 md:px-8 py-4">
    <v-card elevation="0" class="p-5 rounded-lg bg-white">
      <!-- Header Section -->
      <div class="flex flex-col md:flex-row md:items-center md:justify-between mb-6 gap-4">
        <div>
          <h2 class="text-xl font-bold text-gray-800 mb-1">Quản lý nhà cung cấp</h2>
          <p class="text-gray-500 text-sm">Quản lý thông tin và liên hệ với các nhà cung cấp</p>
        </div>
        
        <div class="flex gap-2 flex-wrap">
          <v-btn
            class="supplier-btn-primary"
            elevation="0"
            density="compact"
            size="small"
            @click="openAddDialog"
          >
            <v-icon size="16" class="mr-1">mdi-plus</v-icon>
            <span class="text-xs">Thêm nhà cung cấp</span>
          </v-btn>
          
          <v-btn
            class="supplier-btn-secondary"
            elevation="0"
            density="compact"
            size="small"
            @click="refreshSuppliers"
            :loading="isLoading"
          >
            <v-icon size="16" class="mr-1">mdi-refresh</v-icon>
            <span class="text-xs">Làm mới</span>
          </v-btn>

          <v-btn
            class="supplier-btn-secondary"
            elevation="0"
            density="compact"
            size="small"
            @click="exportSuppliers"
          >
            <v-icon size="16" class="mr-1">mdi-download</v-icon>
            <span class="text-xs">Xuất Excel</span>
          </v-btn>
        </div>
      </div>

      <!-- Statistics Cards -->
      <div class="grid grid-cols-2 md:grid-cols-4 gap-4 mb-6">
        <v-card class="stat-card" elevation="1">
          <v-card-text class="p-4 text-center">
            <div class="flex items-center justify-center gap-3">
              <div class="p-2 bg-blue-100 rounded-full">
                <v-icon size="20" color="primary">mdi-truck-delivery</v-icon>
              </div>
              <div class="text-left">
                <p class="text-2xl font-bold text-blue-600">{{ totalSuppliers }}</p>
                <p class="text-xs text-gray-600">Tổng NCC</p>
              </div>
            </div>
          </v-card-text>
        </v-card>

        <v-card class="stat-card" elevation="1">
          <v-card-text class="p-4 text-center">
            <div class="flex items-center justify-center gap-3">
              <div class="p-2 bg-green-100 rounded-full">
                <v-icon size="20" color="success">mdi-check-circle</v-icon>
              </div>
              <div class="text-left">
                <p class="text-2xl font-bold text-green-600">{{ activeSuppliers }}</p>
                <p class="text-xs text-gray-600">Đang hợp tác</p>
              </div>
            </div>
          </v-card-text>
        </v-card>

        <v-card class="stat-card" elevation="1">
          <v-card-text class="p-4 text-center">
            <div class="flex items-center justify-center gap-3">
              <div class="p-2 bg-orange-100 rounded-full">
                <v-icon size="20" color="warning">mdi-clock-outline</v-icon>
              </div>
              <div class="text-left">
                <p class="text-2xl font-bold text-orange-600">{{ pendingSuppliers }}</p>
                <p class="text-xs text-gray-600">Chờ duyệt</p>
              </div>
            </div>
          </v-card-text>
        </v-card>

        <v-card class="stat-card" elevation="1">
          <v-card-text class="p-4 text-center">
            <div class="flex items-center justify-center gap-3">
              <div class="p-2 bg-purple-100 rounded-full">
                <v-icon size="20" color="purple">mdi-calendar-month</v-icon>
              </div>
              <div class="text-left">
                <p class="text-2xl font-bold text-purple-600">{{ newThisMonth }}</p>
                <p class="text-xs text-gray-600">Mới tháng này</p>
              </div>
            </div>
          </v-card-text>
        </v-card>
      </div>

      <!-- Filter Section -->
      <div class="mb-4 grid grid-cols-1 md:grid-cols-3 gap-3">
        <v-text-field
          v-model="searchQuery"
          density="compact"
          variant="outlined"
          placeholder="Tìm kiếm nhà cung cấp..."
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

        <v-btn
          class="clear-filter-btn-secondary"
          elevation="0"
          block
          @click="clearFilters"
        >
          <v-icon size="16" class="mr-1">mdi-filter-off</v-icon>
          <span class="text-xs">Xóa bộ lọc</span>
        </v-btn>
      </div>

      <!-- Suppliers Table -->
      <v-data-table
        :headers="tableHeaders"
        :items="filteredSuppliers"
        :loading="isLoading"
        :items-per-page="10"
        class="supplier-table"
        density="compact"
      >
        <template #item.supplier_Name="{ item }">
          <div class="flex items-center gap-3">
            <v-avatar size="32" color="#11c393">
              <v-icon color="white" size="16">mdi-domain</v-icon>
            </v-avatar>
            <div>
              <p class="font-medium text-gray-800">{{ item.supplier_Name }}</p>
              <!-- <p class="text-xs text-gray-500">ID: {{ item.supplier_Id }}</p> -->
            </div>
          </div>
        </template>

        <template #item.contact_Person="{ item }">
          <div>
            <p class="font-medium text-gray-700">{{ item.contact_Person }}</p>
            <p class="text-xs text-gray-500">{{ item.phone_Number }}</p>
          </div>
        </template>

        <template #item.email="{ item }">
          <div class="flex items-center gap-2">
            <v-icon size="14" color="grey">mdi-email</v-icon>
            <span class="text-sm">{{ item.email }}</span>
          </div>
        </template>

        <template #item.address="{ item }">
          <div class="max-w-[200px] truncate" :title="item.address">
            <v-icon size="14" color="grey" class="mr-1">mdi-map-marker</v-icon>
            <span class="text-sm">{{ item.address }}</span>
          </div>
        </template>

        <template #item.status="{ item }">
          <v-chip
            :color="getStatusColor(item.status || 'active')"
            variant="tonal"
            size="small"
            class="font-medium"
          >
            {{ getStatusText(item.status || 'active') }}
          </v-chip>
        </template>

        <template #item.created_At="{ item }">
          <span class="text-sm text-gray-600">
            {{ formatDate(item.created_At) }}
          </span>
        </template>

        <template #item.updated_At="{ item }">
          <span class="text-sm text-gray-600">
            {{ formatDate(item.updated_At) }}
          </span>
        </template>

        <template #item.actions="{ item }">
          <div class="flex gap-1 justify-center">
            <v-btn
              icon
              size="small"
              variant="plain"
              class="action-btn"
              @click="viewSupplierDetail(item)"
            >
              <v-icon size="16" color="#11c393">mdi-eye</v-icon>
            </v-btn>
            
            <v-btn
              icon
              size="small"
              variant="plain"
              class="action-btn"
              @click="openEditDialog(item)"
            >
              <v-icon size="16" color="#11c393">mdi-pencil</v-icon>
            </v-btn>
            
            <v-btn
              icon
              size="small"
              variant="plain"
              class="action-btn action-delete"
              @click="confirmDeleteSupplier(item)"
            >
              <v-icon size="16" color="error">mdi-delete</v-icon>
            </v-btn>
          </div>
        </template>

        <template #no-data>
          <div class="text-center py-8 text-gray-500">
            <v-icon size="48" color="grey-lighten-2">mdi-truck-delivery-outline</v-icon>
            <p class="mt-2">Không tìm thấy nhà cung cấp nào</p>
            <v-btn 
              class="supplier-btn-primary mt-4"
              @click="openAddDialog"
            >
              <v-icon size="16" class="mr-1">mdi-plus</v-icon>
              Thêm nhà cung cấp đầu tiên
            </v-btn>
          </div>
        </template>
      </v-data-table>
    </v-card>

    <!-- Supplier Form Modal -->
    <Cp-Supplier-Form-Modal
      v-model="showFormDialog"
      :supplier="selectedSupplier"
      @save="handleSaveSupplier"
    />

    <Cp-Supplier-Detail-Modal
      v-model="showDetailModal"
      :supplier="selectedSupplierForDetail"
    />

    <!-- Delete Confirmation Dialog -->
    <Cp-Confirm-Dialog
      v-model="showDeleteDialog"
      title="Xác nhận xóa nhà cung cấp"
      :message="`Bạn có chắc chắn muốn xóa nhà cung cấp '${supplierToDelete?.supplier_Name}'? Hành động này không thể hoàn tác.`"
      color="error"
      @confirm="handleDeleteSupplier"
    />
  </v-container>
</template>

<script setup lang="ts">
import { computed, onMounted, ref } from 'vue';
import { useSupplierStore } from '@/composables/suppliers/supplierStore';
import { useDialog } from '@/composables/common/useDialog';
import type { TableHeader } from '@/models/sales';
import CpSupplierFormModal from '@/components/suppliers/Cp-Supplier-Form-Modal.vue';
import CpSupplierDetailModal from '@/components/suppliers/Cp-Supplier-Detail-Modal.vue'


interface SupplierDto {
  supplier_Id: string;
  supplier_Name: string;
  contact_Person: string;
  phone_Number: string;
  email: string;
  address: string;
  status?: 'active' | 'inactive' | 'pending';
  created_At?: string | null;
  updated_At?: string | null;
}

type SupplierStatus = 'active' | 'inactive' | 'pending';

// Store and services
const supplierStore = useSupplierStore();
const dialog = useDialog();

// Local state
const suppliers = ref<SupplierDto[]>([]);
const searchQuery = ref('');
const selectedStatus = ref<SupplierStatus | ''>('');
const showFormDialog = ref(false);
const showDeleteDialog = ref(false);
const selectedSupplier = ref<SupplierDto | null>(null);
const supplierToDelete = ref<SupplierDto | null>(null);
const isLoading = ref(false);
const showDetailModal = ref<boolean>(false)
const selectedSupplierForDetail = ref<SupplierDto | null>(null)

// Table configuration
const tableHeaders = ref<TableHeader[]>([
  { title: 'Nhà cung cấp', key: 'supplier_Name', width: '200px', sortable: true },
  { title: 'Người liên hệ', key: 'contact_Person', width: '150px', sortable: true },
  { title: 'Email', key: 'email', width: '200px', sortable: true },
  { title: 'Địa chỉ', key: 'address', width: '250px', sortable: true },
  { title: 'Trạng thái', key: 'status', width: '120px', align: 'center' },
  { title: 'Ngày tạo', key: 'created_At', width: '120px', align: 'center' },
  { title: 'Thao tác', key: 'actions', width: '120px', align: 'center', sortable: false }
]);

// Filter options
const statusOptions = ref([
  { text: 'Tất cả trạng thái', value: '' },
  { text: 'Đang hợp tác', value: 'active' },
  { text: 'Không hoạt động', value: 'inactive' },
  { text: 'Chờ duyệt', value: 'pending' }
]);

// Computed properties
const filteredSuppliers = computed(() => {
  let filtered = suppliers.value;

  // Filter by search
  if (searchQuery.value) {
    const search = searchQuery.value.toLowerCase();
    filtered = filtered.filter(supplier =>
      supplier.supplier_Name.toLowerCase().includes(search) ||
      supplier.contact_Person.toLowerCase().includes(search) ||
      supplier.phone_Number.includes(search) ||
      supplier.email.toLowerCase().includes(search) ||
      supplier.address.toLowerCase().includes(search)
    );
  }

  // Filter by status
  if (selectedStatus.value) {
    filtered = filtered.filter(supplier => 
      (supplier.status || 'active') === selectedStatus.value
    );
  }

  return filtered;
});

const totalSuppliers = computed(() => suppliers.value.length);
const activeSuppliers = computed(() => 
  suppliers.value.filter(s => (s.status || 'active') === 'active').length
);
const pendingSuppliers = computed(() => 
  suppliers.value.filter(s => s.status === 'pending').length
);
const newThisMonth = computed(() => {
  const thisMonth = new Date().getMonth();
  const thisYear = new Date().getFullYear();
  return suppliers.value.filter(s => {
    if (!s.created_At) return false;
    const createdDate = new Date(s.created_At);
    return createdDate.getMonth() === thisMonth && createdDate.getFullYear() === thisYear;
  }).length;
});

// Methods
const getStatusColor = (status: SupplierStatus): string => {
  switch (status) {
    case 'active': return 'success';
    case 'inactive': return 'grey';
    case 'pending': return 'warning';
    default: return 'grey';
  }
};

const getStatusText = (status: SupplierStatus): string => {
  switch (status) {
    case 'active': return 'Đang hợp tác';
    case 'inactive': return 'Không hoạt động';
    case 'pending': return 'Chờ duyệt';
    default: return 'Không xác định';
  }
};

const formatDate = (dateStr?: string | null): string => {
  if (!dateStr) return 'N/A';
  return new Intl.DateTimeFormat('vi-VN', {
    day: '2-digit',
    month: '2-digit',
    year: 'numeric'
  }).format(new Date(dateStr));
};

const debouncedSearch = (() => {
  let timeout: NodeJS.Timeout;
  return () => {
    clearTimeout(timeout);
    timeout = setTimeout(() => {
      // Search is handled by computed property
    }, 300);
  };
})();

const applyFilters = (): void => {
  // Filters are handled by computed property
};

const clearFilters = (): void => {
  searchQuery.value = '';
  selectedStatus.value = '';
};

const refreshSuppliers = async (): Promise<void> => {
  await getSupplierList();
};

const exportSuppliers = (): void => {
  // Implementation for export functionality
  console.log('Export suppliers to Excel');
};

const getSupplierList = async (): Promise<void> => {
  try {
    isLoading.value = true;
    await supplierStore.getSupplierList((res: any) => {
      suppliers.value = res?.data || [];
    });
  } catch (error) {
    console.error('Error fetching suppliers:', error);
    await dialog.error('Không thể tải danh sách nhà cung cấp');
  } finally {
    isLoading.value = false;
  }
};

const openAddDialog = (): void => {
  selectedSupplier.value = null;
  showFormDialog.value = true;
};

const openEditDialog = (supplier: SupplierDto): void => {
  selectedSupplier.value = { ...supplier };
  showFormDialog.value = true;
};

const viewSupplierDetail = (supplier: SupplierDto): void => {
  selectedSupplierForDetail.value = supplier
  showDetailModal.value = true
};

const confirmDeleteSupplier = (supplier: SupplierDto): void => {
  supplierToDelete.value = supplier;
  showDeleteDialog.value = true;
};

const handleSaveSupplier = async (success: boolean): Promise<void> => {
  if (success) {
    showFormDialog.value = false;
    await getSupplierList();
  }
};

const handleDeleteSupplier = async (): Promise<void> => {
  if (supplierToDelete.value) {
    try {
      await supplierStore.deleteSupplier(supplierToDelete.value.supplier_Id);
      await getSupplierList();
      showDeleteDialog.value = false;
      supplierToDelete.value = null;
      await dialog.success('Xóa nhà cung cấp thành công');
    } catch (error) {
      console.error('Error deleting supplier:', error);
      await dialog.error('Không thể xóa nhà cung cấp');
    }
  }
};

// Lifecycle
onMounted(async () => {
  await getSupplierList();
});
</script>

<style scoped>
.supplier-btn-primary {
  @apply rounded px-4 h-8 font-medium text-white bg-[#11c393] hover:bg-[#10b18d] tracking-wide transition-all duration-200;
  min-width: auto !important;
}

.supplier-btn-secondary {
  @apply rounded px-4 h-8 font-medium text-[#11c393] bg-white border border-[#11c393] hover:bg-[#11c393] hover:text-white tracking-wide transition-all duration-200;
  min-width: auto !important;
}

.clear-filter-btn-secondary {
  @apply rounded px-4 h-full font-medium text-[#11c393] bg-white border border-[#11c393] hover:bg-[#11c393] hover:text-white tracking-wide transition-all duration-200;
  min-width: auto !important;
}

.filter-input :deep(.v-field) {
  @apply rounded h-10;
  background: #f8fafc;
}

.filter-input :deep(.v-field--focused .v-field__outline) {
  @apply border-[#11c393];
  box-shadow: none !important;
}

.stat-card {
  @apply rounded-lg border border-gray-200 hover:shadow-lg transition-all duration-200;
  background: linear-gradient(135deg, #ffffff 0%, #f8fafc 100%);
}

.stat-card:hover {
  transform: translateY(-2px);
}

.supplier-table {
  @apply rounded-lg shadow-sm border border-gray-200 overflow-hidden;
  background: white;
}

.supplier-table :deep(thead th) {
  @apply font-semibold text-gray-700 bg-gray-50 text-sm !important;
  border-bottom: 2px solid #e5e7eb;
  padding: 12px 8px;
  height: 48px;
}

.supplier-table :deep(tbody tr) {
  @apply hover:bg-blue-50/40 transition-colors duration-150;
  border-bottom: 1px solid #f1f5f9;
}

.supplier-table :deep(tbody td) {
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