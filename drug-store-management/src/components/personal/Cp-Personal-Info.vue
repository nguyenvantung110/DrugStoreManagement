<template>
  <v-container class="px-2 md:px-8 py-4">
    <v-card elevation="0" class="p-5 rounded-lg bg-white">
      <!-- Header Section -->
      <div class="flex flex-col md:flex-row md:items-center md:justify-between mb-6 gap-4">
        <div class="flex items-center gap-4">
          <v-avatar size="56" color="#11c393" class="shadow-lg">
            <v-icon size="32" color="white">mdi-account</v-icon>
          </v-avatar>
          <div>
            <h2 class="text-xl font-bold text-gray-800 mb-1">{{ user.username }}</h2>
            <p class="text-gray-500 text-sm">Thông tin tài khoản cá nhân</p>
          </div>
        </div>
        
        <div class="flex gap-2 flex-wrap">
          <v-btn
            :class="editMode ? 'personal-btn-secondary' : 'personal-btn-primary'"
            elevation="0"
            density="compact"
            size="small"
            @click="editMode = !editMode"
          >
            <v-icon size="16" class="mr-1">
              {{ editMode ? 'mdi-close' : 'mdi-pencil' }}
            </v-icon>
            <span class="text-xs">{{ editMode ? 'Hủy' : 'Chỉnh sửa' }}</span>
          </v-btn>
        </div>
      </div>

      <!-- User Info Form -->
      <div class="bg-gray-50 rounded-lg p-6">
        <v-form ref="formRef" v-model="valid" lazy-validation>
          <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
            <!-- Basic Information Section -->
            <div class="md:col-span-2">
              <h3 class="section-title">Thông tin cơ bản</h3>
            </div>

            <!-- Full Name -->
            <div class="info-field">
              <v-text-field
                v-model="editUser.fullName"
                :readonly="!editMode"
                label="Họ và tên *"
                :rules="[rules.required]"
                density="compact"
                variant="outlined"
                class="form-input"
                hide-details="auto"
              >
                <template #prepend-inner>
                  <v-icon size="20" color="primary">mdi-account</v-icon>
                </template>
              </v-text-field>
            </div>

            <!-- Email -->
            <div class="info-field">
              <v-text-field
                v-model="editUser.email"
                :readonly="!editMode"
                label="Email"
                type="email"
                :rules="[rules.email]"
                density="compact"
                variant="outlined"
                class="form-input"
                hide-details="auto"
              >
                <template #prepend-inner>
                  <v-icon size="20" color="primary">mdi-email</v-icon>
                </template>
              </v-text-field>
            </div>

            <!-- Phone Number -->
            <div class="info-field">
              <v-text-field
                v-model="editUser.phoneNumber"
                :readonly="!editMode"
                label="Số điện thoại"
                density="compact"
                variant="outlined"
                class="form-input"
                hide-details="auto"
              >
                <template #prepend-inner>
                  <v-icon size="20" color="primary">mdi-phone</v-icon>
                </template>
              </v-text-field>
            </div>

            <!-- Username (readonly) -->
            <div class="info-field">
              <v-text-field
                v-model="editUser.username"
                readonly
                label="Tên đăng nhập"
                density="compact"
                variant="outlined"
                class="form-input readonly-field"
                hide-details="auto"
              >
                <template #prepend-inner>
                  <v-icon size="20" color="grey">mdi-account-circle</v-icon>
                </template>
              </v-text-field>
            </div>

            <!-- Account Information Section -->
            <div class="md:col-span-2 mt-6">
              <h3 class="section-title">Thông tin tài khoản</h3>
            </div>

            <!-- Role -->
            <div class="info-field">
              <v-text-field
                v-model="editUser.role"
                readonly
                label="Vai trò"
                density="compact"
                variant="outlined"
                class="form-input readonly-field"
                hide-details="auto"
              >
                <template #prepend-inner>
                  <v-icon size="20" color="grey">mdi-account-key</v-icon>
                </template>
                <template #append-inner>
                  <v-chip 
                    :color="getRoleColor(editUser.role)"
                    variant="tonal"
                    size="small"
                    class="font-medium"
                  >
                    {{ getRoleDisplayName(editUser.role) }}
                  </v-chip>
                </template>
              </v-text-field>
            </div>

            <!-- User ID -->
            <div class="info-field">
              <v-text-field
                v-model="editUser.userId"
                readonly
                label="Mã người dùng"
                density="compact"
                variant="outlined"
                class="form-input readonly-field"
                hide-details="auto"
              >
                <template #prepend-inner>
                  <v-icon size="20" color="grey">mdi-identifier</v-icon>
                </template>
              </v-text-field>
            </div>

            <!-- Created Date -->
            <div class="info-field">
              <v-text-field
                :model-value="formatDate(editUser.createdAt)"
                readonly
                label="Ngày tạo tài khoản"
                density="compact"
                variant="outlined"
                class="form-input readonly-field"
                hide-details="auto"
              >
                <template #prepend-inner>
                  <v-icon size="20" color="grey">mdi-calendar-plus</v-icon>
                </template>
              </v-text-field>
            </div>

            <!-- Updated Date -->
            <div class="info-field">
              <v-text-field
                :model-value="formatDate(editUser.updatedAt)"
                readonly
                label="Lần cập nhật cuối"
                density="compact"
                variant="outlined"
                class="form-input readonly-field"
                hide-details="auto"
              >
                <template #prepend-inner>
                  <v-icon size="20" color="grey">mdi-calendar-refresh</v-icon>
                </template>
              </v-text-field>
            </div>
          </div>

          <!-- Action Buttons -->
          <div v-if="editMode" class="flex justify-end gap-3 mt-8 pt-6 border-t border-gray-200">
            <v-btn
              class="personal-btn-secondary"
              @click="cancelEdit"
            >
              <v-icon size="16" class="mr-1">mdi-close</v-icon>
              <span class="text-xs">Hủy bỏ</span>
            </v-btn>
            
            <v-btn
              class="personal-btn-primary"
              :disabled="!valid"
              :loading="isSaving"
              @click="saveUser"
            >
              <v-icon size="16" class="mr-1">mdi-content-save</v-icon>
              <span class="text-xs">Lưu thay đổi</span>
            </v-btn>
          </div>
        </v-form>
      </div>

      <!-- Account Status Card -->
      <div class="mt-6 p-4 bg-gradient-to-r from-blue-50 to-green-50 rounded-lg border border-gray-200">
        <div class="flex items-center justify-between">
          <div class="flex items-center gap-3">
            <div class="p-2 bg-green-100 rounded-full">
              <v-icon size="20" color="success">mdi-shield-check</v-icon>
            </div>
            <div>
              <h4 class="font-medium text-gray-800">Trạng thái tài khoản</h4>
              <p class="text-sm text-gray-600">Tài khoản đang hoạt động bình thường</p>
            </div>
          </div>
          
          <v-chip color="success" variant="tonal" size="small">
            <v-icon size="12" class="mr-1">mdi-circle</v-icon>
            Hoạt động
          </v-chip>
        </div>
      </div>
    </v-card>
  </v-container>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted } from 'vue';
import { VForm } from 'vuetify/components';
import { useAuthenticationStore } from '@/composables/auth/authenticationStore';
import { useDialog } from '@/composables/common/useDialog';
import { useRouter } from 'vue-router';
import { usePersonalStore } from '@/composables/users/usePersonalStore';

interface UserDto {
  userId: string;
  username: string;
  passwordHash: string;
  fullName?: string;
  role: string;
  email?: string;
  phoneNumber?: string;
  createdAt: string;
  updatedAt: string;
}

const authenticationStore = useAuthenticationStore();
const personalStore = usePersonalStore();
const router = useRouter();
const dialog = useDialog();

const user = reactive<UserDto>({
  userId: '',
  username: '',
  passwordHash: '',
  fullName: '',
  role: '',
  email: '',
  phoneNumber: '',
  createdAt: '',
  updatedAt: ''
});

const editUser = reactive<UserDto>({ ...user });
const originalUserData = reactive<UserDto>({ ...user });
const editMode = ref(false);
const valid = ref(false);
const isSaving = ref(false);
const formRef = ref<typeof VForm | null>(null);

const rules = {
  required: (v: any) => !!v || 'Không được để trống',
  email: (v: string) => !v || /^[\w-.]+@([\w-]+\.)+[\w-]{2,4}$/.test(v) || 'Email không hợp lệ'
};

// Utility methods
const formatDate = (dateStr: string | undefined): string => {
  if (!dateStr) return 'Chưa cập nhật';
  return new Intl.DateTimeFormat('vi-VN', {
    day: '2-digit',
    month: '2-digit',
    year: 'numeric',
    hour: '2-digit',
    minute: '2-digit'
  }).format(new Date(dateStr));
};

const getRoleColor = (role: string): string => {
  switch (role?.toLowerCase()) {
    case 'admin': return 'red';
    case 'pharmacist': return 'blue';
    case 'cashier': return 'green';
    case 'manager': return 'purple';
    default: return 'grey';
  }
};

const getRoleDisplayName = (role: string): string => {
  switch (role?.toLowerCase()) {
    case 'admin': return 'Quản trị viên';
    case 'pharmacist': return 'Dược sĩ';
    case 'cashier': return 'Thu ngân';
    case 'manager': return 'Quản lý';
    default: return role || 'Không xác định';
  }
};

// Data methods
const fetchUser = async (): Promise<void> => {
  try {
    const userId = authenticationStore.getterUserId;
    await personalStore.getUserInfo(userId, (res: any) => {
      const userData = res?.data || {};
      Object.assign(user, userData);
      Object.assign(editUser, userData);
      Object.assign(originalUserData, userData);
    });
  } catch (error) {
    console.error('Error fetching user info:', error);
    await dialog.error('Không thể tải thông tin người dùng');
  }
};

const saveUser = async (): Promise<void> => {
  if (!(await (formRef.value as any)?.validate())) return;

  try {
    isSaving.value = true;
    
    const updateData = {
      userId: user.userId,
      fullName: editUser.fullName,
      email: editUser.email,
      phoneNumber: editUser.phoneNumber
    };

    await personalStore.updateUserInfo(updateData);
    await dialog.success('Cập nhật thông tin thành công!');
    await fetchUser();
    editMode.value = false;
  } catch (error) {
    console.error('Error updating user info:', error);
    await dialog.error('Không thể cập nhật thông tin');
  } finally {
    isSaving.value = false;
  }
};

const cancelEdit = (): void => {
  // Restore original data
  Object.assign(editUser, originalUserData);
  editMode.value = false;
  formRef.value?.resetValidation();
};

onMounted(fetchUser);
</script>

<style scoped>
.personal-btn-primary {
  @apply rounded px-4 h-8 font-medium text-white bg-[#11c393] hover:bg-[#10b18d] tracking-wide transition-all duration-200;
  min-width: auto !important;
}

.personal-btn-secondary {
  @apply rounded px-4 h-8 font-medium text-[#11c393] bg-white border border-[#11c393] hover:bg-[#11c393] hover:text-white tracking-wide transition-all duration-200;
  min-width: auto !important;
}

.section-title {
  @apply text-lg font-semibold text-gray-700 mb-4 pb-2 border-b border-gray-200;
}

.info-field {
  @apply relative;
}

.form-input :deep(.v-field) {
  @apply rounded;
  background: white;
  border: 1px solid #e5e7eb;
}

.form-input :deep(.v-field--focused .v-field__outline) {
  @apply border-[#11c393];
  box-shadow: 0 0 0 3px rgba(17, 195, 147, 0.1) !important;
}

.form-input :deep(.v-field__input) {
  @apply text-gray-700;
}

.form-input :deep(.v-label) {
  @apply text-gray-600;
}

.readonly-field :deep(.v-field) {
  background: #f8fafc !important;
  border-color: #e2e8f0 !important;
}

.readonly-field :deep(.v-field__input) {
  @apply text-gray-500;
}

.readonly-field :deep(.v-label) {
  @apply text-gray-400;
}

/* Animation for edit mode */
.info-field {
  transition: all 0.3s ease;
}

.form-input:hover:not(.readonly-field) :deep(.v-field) {
  @apply border-gray-300;
  transform: translateY(-1px);
}

/* Loading animation for save button */
.personal-btn-primary:disabled {
  @apply bg-gray-300 text-gray-500;
  cursor: not-allowed;
}

/* Responsive adjustments */
@media (max-width: 768px) {
  .grid {
    @apply grid-cols-1;
  }
  
  .md\:col-span-2 {
    @apply col-span-1;
  }
  
  .section-title {
    @apply text-base;
  }
}

/* Focus states for accessibility */
.form-input :deep(.v-field--focused) {
  outline: 2px solid transparent;
  outline-offset: 2px;
}

/* Status card animation */
.bg-gradient-to-r {
  background: linear-gradient(135deg, #eff6ff 0%, #f0fdf4 100%);
  transition: all 0.3s ease;
}

.bg-gradient-to-r:hover {
  transform: translateY(-2px);
  box-shadow: 0 8px 25px rgba(0, 0, 0, 0.1);
}
</style>