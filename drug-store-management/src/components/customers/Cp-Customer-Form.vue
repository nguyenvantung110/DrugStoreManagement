<template>
  <v-dialog
    :model-value="modelValue"
    @update:model-value="$emit('update:modelValue', $event)"
    max-width="800px"
    persistent
  >
    <v-card class="rounded-lg">
      <v-card-title class="bg-[#11c393] text-white">
        <span class="text-lg font-semibold">
          {{ isEditing ? 'Chỉnh sửa khách hàng' : 'Thêm khách hàng mới' }}
        </span>
      </v-card-title>

      <v-form ref="formRef" v-model="isFormValid" @submit.prevent="handleSubmit">
        <v-card-text class="py-6">
          <div class="grid grid-cols-1 md:grid-cols-2 gap-4 mb-6">
            <!-- Customer Code (Read-only for editing) -->
            <v-text-field
              v-if="isEditing"
              :model-value="customer?.customerCode"
              label="Mã khách hàng"
              readonly
              density="compact"
              variant="outlined"
              class="form-input"
              bg-color="grey-lighten-4"
            />

            <!-- Full Name -->
            <v-text-field
              v-model="formData.fullName"
              label="Họ và tên *"
              :rules="fullNameRules"
              density="compact"
              variant="outlined"
              class="form-input"
              :class="{ 'md:col-span-1': isEditing, 'md:col-span-2': !isEditing }"
            />

            <!-- Date of Birth -->
            <v-text-field
              v-model="formattedDateOfBirth"
              label="Ngày sinh"
              type="date"
              density="compact"
              variant="outlined"
              class="form-input"
              @update:model-value="updateDateOfBirth"
            />

            <!-- Gender -->
            <v-select
              v-model="formData.gender"
              :items="genderOptions"
              item-title="text"
              item-value="value"
              label="Giới tính *"
              :rules="genderRules"
              density="compact"
              variant="outlined"
              class="form-input"
            />

            <!-- Phone -->
            <v-text-field
              v-model="formData.phone"
              label="Số điện thoại *"
              :rules="phoneRules"
              density="compact"
              variant="outlined"
              class="form-input"
            />

            <!-- Email -->
            <v-text-field
              v-model="formData.email"
              label="Email"
              :rules="emailRules"
              density="compact"
              variant="outlined"
              class="form-input"
            />

            <!-- Identity Card -->
            <v-text-field
              v-model="formData.identityCard"
              label="CCCD/CMND"
              :rules="identityCardRules"
              density="compact"
              variant="outlined"
              class="form-input"
            />

            <!-- Insurance Card -->
            <v-text-field
              v-model="formData.insuranceCard"
              label="Thẻ bảo hiểm"
              density="compact"
              variant="outlined"
              class="form-input"
            />
          </div>

          <!-- Address Section -->
          <v-divider class="my-4" />
          <h3 class="text-md font-semibold text-gray-700 mb-4">Địa chỉ</h3>
          <div class="grid grid-cols-1 md:grid-cols-2 gap-4 mb-6">
            <v-text-field
              v-model="formData.address.street"
              label="Địa chỉ chi tiết"
              density="compact"
              variant="outlined"
              class="form-input md:col-span-2"
            />

            <v-text-field
              v-model="formData.address.ward"
              label="Phường/Xã"
              density="compact"
              variant="outlined"
              class="form-input"
            />

            <v-text-field
              v-model="formData.address.district"
              label="Quận/Huyện"
              density="compact"
              variant="outlined"
              class="form-input"
            />

            <v-select
              v-model="formData.address.city"
              :items="cityOptions"
              label="Tỉnh/Thành phố"
              density="compact"
              variant="outlined"
              class="form-input"
            />

            <v-text-field
              v-model="formData.address.zipCode"
              label="Mã bưu điện"
              density="compact"
              variant="outlined"
              class="form-input"
            />
          </div>

          <!-- Medical Information -->
          <v-divider class="my-4" />
          <h3 class="text-md font-semibold text-gray-700 mb-4">Thông tin y tế</h3>
          <div class="mb-4">
            <v-combobox
              v-model="formData.allergyHistory"
              label="Lịch sử dị ứng"
              chips
              multiple
              closable-chips
              density="compact"
              variant="outlined"
              class="form-input"
              hint="Nhập tên thuốc/chất gây dị ứng và nhấn Enter"
              persistent-hint
            />
          </div>

          <!-- Status (only for editing) -->
          <div v-if="isEditing" class="mb-4">
            <v-divider class="my-4" />
            <v-select
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
import { useCustomersStore } from '@/composables/customers/customersStore';
import type { Customer, CustomerGender, CustomerStatus, CreateCustomerRequest, UpdateCustomerRequest } from '@/models/customers';

// Props & Emits
interface Props {
  modelValue: boolean;
  customer?: Customer | null;
}

interface Emits {
  (e: 'update:modelValue', value: boolean): void;
  (e: 'save', success: boolean): void;
}

const props = defineProps<Props>();
const emit = defineEmits<Emits>();

// Store
const customersStore = useCustomersStore();

// Local state
const formRef = ref();
const isFormValid = ref(false);
const formattedDateOfBirth = ref('');

const formData = ref({
  fullName: '',
  dateOfBirth: undefined as Date | undefined,
  gender: 'male' as CustomerGender,
  phone: '',
  email: '',
  identityCard: '',
  insuranceCard: '',
  address: {
    street: '',
    ward: '',
    district: '',
    city: '',
    zipCode: ''
  },
  allergyHistory: [] as string[],
  status: 'active' as CustomerStatus
});

// Computed
const isEditing = computed(() => !!props.customer);
const isLoading = computed(() => customersStore.getIsLoading);

// Options
const genderOptions = ref([
  { text: 'Nam', value: 'male' },
  { text: 'Nữ', value: 'female' },
  { text: 'Khác', value: 'other' }
]);

const statusOptions = ref([
  { text: 'Hoạt động', value: 'active' },
  { text: 'Không hoạt động', value: 'inactive' },
  { text: 'Đã chặn', value: 'blocked' }
]);

const cityOptions = ref([
  'Hà Nội',
  'Hồ Chí Minh',
  'Đà Nẵng',
  'Hải Phòng',
  'Cần Thơ',
  'An Giang',
  'Bà Rịa - Vũng Tàu',
  'Bắc Giang',
  'Bắc Kạn',
  'Bạc Liêu',
  'Bắc Ninh',
  'Bến Tre',
  'Bình Định',
  'Bình Dương',
  'Bình Phước',
  'Bình Thuận',
  'Cà Mau',
  'Cao Bằng',
  'Đắk Lắk',
  'Đắk Nông',
  'Điện Biên',
  'Đồng Nai',
  'Đồng Tháp',
  'Gia Lai',
  'Hà Giang',
  'Hà Nam',
  'Hà Tĩnh',
  'Hải Dương',
  'Hậu Giang',
  'Hòa Bình',
  'Hưng Yên',
  'Khánh Hòa',
  'Kiên Giang',
  'Kon Tum',
  'Lai Châu',
  'Lâm Đồng',
  'Lạng Sơn',
  'Lào Cai',
  'Long An',
  'Nam Định',
  'Nghệ An',
  'Ninh Bình',
  'Ninh Thuận',
  'Phú Thọ',
  'Phú Yên',
  'Quảng Bình',
  'Quảng Nam',
  'Quảng Ngãi',
  'Quảng Ninh',
  'Quảng Trị',
  'Sóc Trăng',
  'Sơn La',
  'Tây Ninh',
  'Thái Bình',
  'Thái Nguyên',
  'Thanh Hóa',
  'Thừa Thiên Huế',
  'Tiền Giang',
  'Trà Vinh',
  'Tuyên Quang',
  'Vĩnh Long',
  'Vĩnh Phúc',
  'Yên Bái'
]);

// Validation rules
const fullNameRules = [
  (v: string) => !!v || 'Họ tên là bắt buộc',
  (v: string) => v.length >= 2 || 'Họ tên phải có ít nhất 2 ký tự',
  (v: string) => /^[a-zA-ZÀÁÂÃÈÉÊÌÍÒÓÔÕÙÚĂĐĨŨƠàáâãèéêìíòóôõùúăđĩũơƯĂẠẢẤẦẨẪẬẮẰẲẴẶẸẺẼỀỀỂếểễệỉịọỏốồổỗộớờởỡợụủứừử ]+$/.test(v) || 'Họ tên không hợp lệ'
];

const genderRules = [
  (v: string) => !!v || 'Giới tính là bắt buộc'
];

const phoneRules = [
  (v: string) => !!v || 'Số điện thoại là bắt buộc',
  (v: string) => /^[0-9]{10,11}$/.test(v) || 'Số điện thoại không hợp lệ'
];

const emailRules = [
  (v: string) => !v || /.+@.+\..+/.test(v) || 'Email không hợp lệ'
];

const identityCardRules = [
  (v: string) => !v || /^[0-9]{9}$|^[0-9]{12}$/.test(v) || 'CCCD/CMND không hợp lệ'
];

const statusRules = [
  (v: string) => !!v || 'Trạng thái là bắt buộc'
];

// Methods
const updateDateOfBirth = (value: string): void => {
  formData.value.dateOfBirth = value ? new Date(value) : undefined;
};

const resetForm = (): void => {
  formData.value = {
    fullName: '',
    dateOfBirth: undefined,
    gender: 'male',
    phone: '',
    email: '',
    identityCard: '',
    insuranceCard: '',
    address: {
      street: '',
      ward: '',
      district: '',
      city: '',
      zipCode: ''
    },
    allergyHistory: [],
    status: 'active'
  };
  formattedDateOfBirth.value = '';
  formRef.value?.resetValidation();
};

const loadCustomerData = (customer: Customer): void => {
  formData.value = {
    fullName: customer.fullName,
    dateOfBirth: customer.dateOfBirth,
    gender: customer.gender,
    phone: customer.phone,
    email: customer.email || '',
    identityCard: customer.identityCard || '',
    insuranceCard: customer.insuranceCard || '',
    address: customer.address ? { ...customer.address } : {
      street: '',
      ward: '',
      district: '',
      city: '',
      zipCode: ''
    },
    allergyHistory: customer.allergyHistory ? [...customer.allergyHistory] : [],
    status: customer.status
  };
  
  formattedDateOfBirth.value = customer.dateOfBirth 
    ? new Date(customer.dateOfBirth).toISOString().split('T')[0] 
    : '';
};

const handleSubmit = async (): Promise<void> => {
  if (!isFormValid.value) return;

  let success = false;

  if (isEditing.value && props.customer) {
    // Update existing customer
    const updateData: UpdateCustomerRequest = {
      fullName: formData.value.fullName,
      dateOfBirth: formData.value.dateOfBirth,
      gender: formData.value.gender,
      phone: formData.value.phone,
      email: formData.value.email || undefined,
      identityCard: formData.value.identityCard || undefined,
      insuranceCard: formData.value.insuranceCard || undefined,
      address: formData.value.address.street ? formData.value.address : undefined,
      allergyHistory: formData.value.allergyHistory.length > 0 ? formData.value.allergyHistory : undefined,
      status: formData.value.status
    };
    success = await customersStore.updateCustomer(props.customer.id, updateData);
  } else {
    // Create new customer
    const createData: CreateCustomerRequest = {
      fullName: formData.value.fullName,
      dateOfBirth: formData.value.dateOfBirth,
      gender: formData.value.gender,
      phone: formData.value.phone,
      email: formData.value.email || undefined,
      identityCard: formData.value.identityCard || undefined,
      insuranceCard: formData.value.insuranceCard || undefined,
      address: formData.value.address.street ? formData.value.address : undefined,
      allergyHistory: formData.value.allergyHistory.length > 0 ? formData.value.allergyHistory : undefined
    };
    success = await customersStore.createCustomer(createData);
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
watch(() => props.customer, (newCustomer) => {
  if (newCustomer) {
    loadCustomerData(newCustomer);
  } else {
    resetForm();
  }
}, { immediate: true });

watch(() => props.modelValue, (isOpen) => {
  if (isOpen && !props.customer) {
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