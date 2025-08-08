<template>
  <v-dialog
    :model-value="modelValue"
    @update:model-value="$emit('update:modelValue', $event)"
    max-width="600px"
    persistent
  >
    <v-card class="rounded-lg">
      <v-card-title class="bg-[#11c393] text-white">
        <span class="text-lg font-semibold">
          {{ isEditing ? 'Chỉnh sửa người dùng' : 'Thêm người dùng mới' }}
        </span>
      </v-card-title>

      <v-form ref="formRef" v-model="isFormValid" @submit.prevent="handleSubmit">
        <v-card-text class="py-6">
          <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
            <!-- Username -->
            <v-text-field
              v-model="formData.username"
              label="Tên đăng nhập *"
              :rules="usernameRules"
              :disabled="isEditing"
              density="compact"
              variant="outlined"
              class="form-input"
              :hint="isEditing ? 'Không thể thay đổi tên đăng nhập' : ''"
              persistent-hint
            />

            <!-- Email -->
            <v-text-field
              v-model="formData.email"
              label="Email *"
              :rules="emailRules"
              density="compact"
              variant="outlined"
              class="form-input"
            />

            <!-- Full Name -->
            <v-text-field
              v-model="formData.fullName"
              label="Họ và tên *"
              :rules="fullNameRules"
              density="compact"
              variant="outlined"
              class="form-input"
            />

            <!-- Phone -->
            <v-text-field
              v-model="formData.phone"
              label="Số điện thoại"
              :rules="phoneRules"
              density="compact"
              variant="outlined"
              class="form-input"
            />

            <!-- Role -->
            <v-select
              v-model="formData.roleId"
              :items="roleOptions"
              item-title="text"
              item-value="value"
              label="Vai trò *"
              :rules="roleRules"
              density="compact"
              variant="outlined"
              class="form-input"
            />

            <!-- Status (only for editing) -->
            <v-select
              v-if="isEditing"
              v-model="formData.status"
              :items="statusOptions"
              item-title="text"
              item-value="value"
              label="Trạng thái *"
              :rules="statusRules"
              density="compact"
              variant="outlined"
              class="form-input"
            />

            <!-- Password (only for creating) -->
            <template v-if="!isEditing">
              <v-text-field
                v-model="formData.password"
                label="Mật khẩu *"
                :rules="passwordRules"
                type="password"
                density="compact"
                variant="outlined"
                class="form-input"
              />

              <v-text-field
                v-model="confirmPassword"
                label="Xác nhận mật khẩu *"
                :rules="confirmPasswordRules"
                type="password"
                density="compact"
                variant="outlined"
                class="form-input"
              />
            </template>
          </div>
        </v-card-text>

        <v-card-actions class="px-6 pb-6">
          <v-spacer />
          <v-btn
            class="form-btn-secondary"
            @click="handleCancel"
          >
            Hủy
          </v-btn>
          <v-btn
            class="form-btn-primary"
            type="submit"
            :loading="isLoading"
            :disabled="!isFormValid"
          >
            {{ isEditing ? 'Cập nhật' : 'Tạo mới' }}
          </v-btn>
        </v-card-actions>
      </v-form>
    </v-card>
  </v-dialog>
</template>

<script setup lang="ts">
import { computed, ref, watch } from 'vue';
import { useUsersStore } from '@/composables/users/usersStore';
import type { User, UserRole, UserStatus, CreateUserRequest, UpdateUserRequest } from '@/models/users';

// Props & Emits
interface Props {
  modelValue: boolean;
  user?: User | null;
  roles: UserRole[];
}

interface Emits {
  (e: 'update:modelValue', value: boolean): void;
  (e: 'save', success: boolean): void;
}

const props = defineProps<Props>();
const emit = defineEmits<Emits>();

// Store
const usersStore = useUsersStore();

// Local state
const formRef = ref();
const isFormValid = ref(false);
const confirmPassword = ref('');

const formData = ref({
  username: '',
  email: '',
  fullName: '',
  phone: '',
  roleId: '',
  password: '',
  status: 'active' as UserStatus
});

// Computed
const isEditing = computed(() => !!props.user);
const isLoading = computed(() => usersStore.getIsLoading);

const roleOptions = computed(() =>
  props.roles.map(role => ({ text: role.displayName, value: role.id }))
);

const statusOptions = ref([
  { text: 'Hoạt động', value: 'active' },
  { text: 'Không hoạt động', value: 'inactive' },
  { text: 'Tạm khóa', value: 'suspended' }
]);

// Validation rules
const usernameRules = [
  (v: string) => !!v || 'Tên đăng nhập là bắt buộc',
  (v: string) => v.length >= 3 || 'Tên đăng nhập phải có ít nhất 3 ký tự',
  (v: string) => /^[a-zA-Z0-9_]+$/.test(v) || 'Tên đăng nhập chỉ chứa chữ cái, số và dấu gạch dưới'
];

const emailRules = [
  (v: string) => !!v || 'Email là bắt buộc',
  (v: string) => /.+@.+\..+/.test(v) || 'Email không hợp lệ'
];

const fullNameRules = [
  (v: string) => !!v || 'Họ tên là bắt buộc',
  (v: string) => v.length >= 2 || 'Họ tên phải có ít nhất 2 ký tự'
];

const phoneRules = [
  (v: string) => !v || /^[0-9]{10,11}$/.test(v) || 'Số điện thoại không hợp lệ'
];

const roleRules = [
  (v: string) => !!v || 'Vai trò là bắt buộc'
];

const statusRules = [
  (v: string) => !!v || 'Trạng thái là bắt buộc'
];

const passwordRules = [
  (v: string) => !!v || 'Mật khẩu là bắt buộc',
  (v: string) => v.length >= 6 || 'Mật khẩu phải có ít nhất 6 ký tự'
];

const confirmPasswordRules = [
  (v: string) => !!v || 'Xác nhận mật khẩu là bắt buộc',
  (v: string) => v === formData.value.password || 'Mật khẩu xác nhận không khớp'
];

// Methods
const resetForm = (): void => {
  formData.value = {
    username: '',
    email: '',
    fullName: '',
    phone: '',
    roleId: '',
    password: '',
    status: 'active'
  };
  confirmPassword.value = '';
  formRef.value?.resetValidation();
};

const loadUserData = (user: User): void => {
  formData.value = {
    username: user.username,
    email: user.email,
    fullName: user.fullName,
    phone: user.phone || '',
    roleId: user.role.id,
    password: '',
    status: user.status
  };
};

const handleSubmit = async (): Promise<void> => {
  if (!isFormValid.value) return;

  let success = false;

  if (isEditing.value && props.user) {
    // Update existing user
    const updateData: UpdateUserRequest = {
      email: formData.value.email,
      fullName: formData.value.fullName,
      phone: formData.value.phone || undefined,
      roleId: formData.value.roleId,
      status: formData.value.status
    };
    success = await usersStore.updateUser(props.user.id, updateData);
  } else {
    // Create new user
    const createData: CreateUserRequest = {
      username: formData.value.username,
      email: formData.value.email,
      fullName: formData.value.fullName,
      phone: formData.value.phone || undefined,
      password: formData.value.password,
      roleId: formData.value.roleId
    };
    success = await usersStore.createUser(createData);
  }

  emit('save', success);
  if (success) {
    resetForm();
  }
};

const handleCancel = (): void => {
  resetForm();
  emit('update:modelValue', false);
};

// Watchers
watch(() => props.user, (newUser) => {
  if (newUser) {
    loadUserData(newUser);
  } else {
    resetForm();
  }
}, { immediate: true });

watch(() => props.modelValue, (isOpen) => {
  if (isOpen && !props.user) {
    resetForm();
  }
});
</script>

<style scoped>
.form-input :deep(.v-field) {
  @apply rounded;
  background: #f8fafc;
}

.form-input :deep(.v-field--focused .v-field__outline) {
  @apply border-[#11c393];
  box-shadow: none !important;
}

.form-btn-primary {
  @apply rounded px-6 h-10 font-semibold text-white bg-[#11c393] hover:bg-[#10b18d] tracking-wide transition-all duration-200;
  min-width: auto !important;
}

.form-btn-secondary {
  @apply rounded px-6 h-10 font-semibold text-gray-600 bg-gray-100 hover:bg-gray-200 tracking-wide transition-all duration-200;
  min-width: auto !important;
}
</style>