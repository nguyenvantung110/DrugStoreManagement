<template>
  <v-container class="px-2 md:px-8 py-4">
    <v-card elevation="0" class="p-5 rounded-lg bg-white">
      <!-- Header Section -->
      <div class="flex flex-col md:flex-row md:items-center md:justify-between mb-6 gap-4">
        <div>
          <h2 class="text-xl font-bold text-gray-800 mb-1">Quản lý người dùng</h2>
          <p class="text-gray-500 text-sm">Quản lý tài khoản và phân quyền người dùng hệ thống</p>
        </div>
        
        <div class="flex gap-2 flex-wrap">
          <v-btn
            class="user-btn-primary"
            elevation="0"
            density="compact"
            size="small"
            @click="openCreateDialog"
          >
            <v-icon size="16" class="mr-1">mdi-account-plus</v-icon>
            <span class="text-xs">Thêm người dùng</span>
          </v-btn>
          
          <v-btn
            class="user-btn-secondary"
            elevation="0"
            density="compact"
            size="small"
            @click="refreshUsers"
            :loading="isLoading"
          >
            <v-icon size="16" class="mr-1">mdi-refresh</v-icon>
            <span class="text-xs">Làm mới</span>
          </v-btn>
        </div>
      </div>

      <!-- Filter Section -->
      <div class="mb-4 grid grid-cols-1 md:grid-cols-4 gap-3">
        <v-text-field
          v-model="searchQuery"
          density="compact"
          variant="outlined"
          placeholder="Tìm kiếm người dùng..."
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
          v-model="selectedRole"
          :items="roleOptions"
          item-title="text"
          item-value="value"
          density="compact"
          variant="outlined"
          placeholder="Chọn vai trò"
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

        <v-btn
          class="user-btn-secondary h-10"
          elevation="0"
          block
          @click="clearFilters"
        >
          <v-icon size="16" class="mr-1">mdi-filter-off</v-icon>
          <span class="text-xs">Xóa bộ lọc</span>
        </v-btn>
      </div>

      <!-- Users Table -->
      <v-data-table
        :headers="tableHeaders"
        :items="usersList"
        :loading="isLoading"
        :items-per-page="10"
        class="user-table"
        density="compact"
      >
        <template #item.avatar="{ item }">
          <v-avatar size="32" color="primary">
            <v-img v-if="item.avatar" :src="item.avatar" />
            <span v-else class="text-white text-sm">{{ getInitials(item.fullName) }}</span>
          </v-avatar>
        </template>

        <template #item.role="{ item }">
          <v-chip
            :color="getRoleColor(item.role.name)"
            variant="tonal"
            size="small"
            class="font-medium"
          >
            {{ item.role.displayName }}
          </v-chip>
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

        <template #item.lastLogin="{ item }">
          <span class="text-sm text-gray-600">
            {{ item.lastLogin ? formatDateTime(item.lastLogin) : 'Chưa đăng nhập' }}
          </span>
        </template>

        <template #item.actions="{ item }">
          <div class="flex gap-1">
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
              @click="toggleUserStatus(item)"
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
              @click="confirmDeleteUser(item)"
            >
              <v-icon size="16" color="error">mdi-delete</v-icon>
            </v-btn>
          </div>
        </template>

        <template #no-data>
          <div class="text-center py-8 text-gray-500">
            <v-icon size="48" color="grey-lighten-2">mdi-account-off</v-icon>
            <p class="mt-2">Không tìm thấy người dùng nào</p>
          </div>
        </template>
      </v-data-table>
    </v-card>

    <!-- User Form Dialog -->
    <Cp-User-Form
      v-model="showDialog"
      :user="selectedUser"
      :roles="rolesList"
      @save="handleSaveUser"
    />

    <!-- Delete Confirmation Dialog -->
    <Cp-Confirm-Dialog
      v-model="showDeleteDialog"
      title="Xác nhận xóa người dùng"
      :message="`Bạn có chắc chắn muốn xóa người dùng '${userToDelete?.fullName}'? Hành động này không thể hoàn tác.`"
      color="error"
      @confirm="handleDeleteUser"
    />
  </v-container>
</template>

<script setup lang="ts">
import { computed, onMounted, ref, watch } from 'vue';
import { useUsersStore } from '@/composables/users/usersStore';
import type { User, TableHeader, UserStatus } from '@/models/users';
import CpUserForm from '@/components/users/Cp-User-Form.vue';

// Store
const usersStore = useUsersStore();

// Local state
const searchQuery = ref('');
const selectedRole = ref('');
const selectedStatus = ref<UserStatus | ''>('');
const showDialog = ref(false);
const showDeleteDialog = ref(false);
const selectedUser = ref<User | null>(null);
const userToDelete = ref<User | null>(null);

// Table configuration
const tableHeaders = ref<TableHeader[]>([
  { title: '', key: 'avatar', width: '60px', align: 'center', sortable: false },
  { title: 'Tên đăng nhập', key: 'username', width: '150px', sortable: true },
  { title: 'Họ tên', key: 'fullName', width: '200px', sortable: true },
  { title: 'Email', key: 'email', width: '200px', sortable: true },
  { title: 'Vai trò', key: 'role', width: '120px', align: 'center' },
  { title: 'Trạng thái', key: 'status', width: '100px', align: 'center' },
  { title: 'Lần đăng nhập cuối', key: 'lastLogin', width: '150px' },
  { title: 'Thao tác', key: 'actions', width: '120px', align: 'center', sortable: false }
]);

// Filter options
const roleOptions = computed(() => [
  { text: 'Tất cả vai trò', value: '' },
  ...usersStore.getRolesList.map((role: any) => ({ text: role.displayName, value: role.id }))
]);

const statusOptions = ref([
  { text: 'Tất cả trạng thái', value: '' },
  { text: 'Hoạt động', value: 'active' },
  { text: 'Không hoạt động', value: 'inactive' },
  { text: 'Tạm khóa', value: 'suspended' }
]);

// Computed properties
const usersList = computed(() => usersStore.getUsersList);
const rolesList = computed(() => usersStore.getRolesList);
const isLoading = computed(() => usersStore.getIsLoading);

// Methods
const getInitials = (fullName: string): string => {
  return fullName
    .split(' ')
    .map(name => name.charAt(0))
    .join('')
    .toUpperCase()
    .slice(0, 2);
};

const getRoleColor = (roleName: string): string => {
  switch (roleName) {
    case 'admin': return 'red';
    case 'pharmacist': return 'blue';
    case 'cashier': return 'green';
    default: return 'grey';
  }
};

const getStatusColor = (status: UserStatus): string => {
  switch (status) {
    case 'active': return 'success';
    case 'inactive': return 'grey';
    case 'suspended': return 'error';
    default: return 'grey';
  }
};

const getStatusText = (status: UserStatus): string => {
  switch (status) {
    case 'active': return 'Hoạt động';
    case 'inactive': return 'Không hoạt động';
    case 'suspended': return 'Tạm khóa';
    default: return 'Không xác định';
  }
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
  usersStore.updateFilter({
    search: searchQuery.value,
    role: selectedRole.value,
    status: selectedStatus.value || undefined,
    page: 1
  });
  usersStore.fetchUsers();
};

const clearFilters = (): void => {
  searchQuery.value = '';
  selectedRole.value = '';
  selectedStatus.value = '';
  applyFilters();
};

const refreshUsers = (): void => {
  usersStore.fetchUsers();
};

const openCreateDialog = (): void => {
  selectedUser.value = null;
  showDialog.value = true;
};

const openEditDialog = (user: User): void => {
  selectedUser.value = user;
  showDialog.value = true;
};

const handleSaveUser = async (success: boolean): Promise<void> => {
  if (success) {
    showDialog.value = false;
    await usersStore.fetchUsers();
  }
};

const toggleUserStatus = async (user: User): Promise<void> => {
  const newStatus: UserStatus = user.status === 'active' ? 'inactive' : 'active';
  await usersStore.updateUser(user.id, { status: newStatus });
};

const confirmDeleteUser = (user: User): void => {
  userToDelete.value = user;
  showDeleteDialog.value = true;
};

const handleDeleteUser = async (): Promise<void> => {
  if (userToDelete.value) {
    const success = await usersStore.deleteUser(userToDelete.value.id);
    if (success) {
      showDeleteDialog.value = false;
      userToDelete.value = null;
    }
  }
};

// Lifecycle
onMounted(async () => {
  await Promise.all([
    usersStore.fetchUsers(),
    usersStore.fetchRoles()
  ]);
});
</script>

<style scoped>
.user-btn-primary {
  @apply rounded px-4 h-8 font-medium text-white bg-[#11c393] hover:bg-[#10b18d] tracking-wide transition-all duration-200;
  min-width: auto !important;
}

.user-btn-secondary {
  @apply rounded px-4 h-8 font-medium text-[#11c393] bg-white border border-[#11c393] hover:bg-[#11c393] hover:text-white tracking-wide transition-all duration-200;
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

.user-table {
  @apply rounded-lg shadow-sm border border-gray-200 overflow-hidden;
  background: white;
}

.user-table :deep(thead th) {
  @apply font-semibold text-gray-700 bg-gray-50 text-sm;
  border-bottom: 2px solid #e5e7eb;
  padding: 12px 8px;
  height: 48px;
}

.user-table :deep(tbody tr) {
  @apply hover:bg-blue-50/40 transition-colors duration-150;
  border-bottom: 1px solid #f1f5f9;
}

.user-table :deep(tbody td) {
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

.action-delete:hover {
  @apply bg-red-50;
}
</style>