<template>
  <v-dialog
    :model-value="modelValue"
    @update:model-value="$emit('update:modelValue', $event)"
    max-width="700px"
    persistent
  >
    <v-card class="rounded-lg">
      <v-card-title class="bg-[#11c393] text-white">
        <div class="flex items-center gap-3">
          <v-icon size="24">mdi-truck-delivery</v-icon>
          <span class="text-lg font-semibold">
            {{ isEditing ? 'Chỉnh sửa nhà cung cấp' : 'Thêm nhà cung cấp mới' }}
          </span>
        </div>
      </v-card-title>

      <v-form ref="formRef" v-model="isFormValid" @submit.prevent="handleSubmit">
        <v-card-text class="py-6">
          <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
            <!-- Supplier Name -->
            <div class="md:col-span-2">
              <v-text-field
                v-model="formData.supplier_Name"
                label="Tên nhà cung cấp *"
                :rules="supplierNameRules"
                density="compact"
                variant="outlined"
                class="form-input"
                hide-details="auto"
              >
                <template #prepend-inner>
                  <v-icon size="20" color="primary">mdi-domain</v-icon>
                </template>
              </v-text-field>
            </div>

            <!-- Contact Person -->
            <v-text-field
              v-model="formData.contact_Person"
              label="Người liên hệ *"
              :rules="contactPersonRules"
              density="compact"
              variant="outlined"
              class="form-input"
              hide-details="auto"
            >
              <template #prepend-inner>
                <v-icon size="20" color="primary">mdi-account</v-icon>
              </template>
            </v-text-field>

            <!-- Phone Number -->
            <v-text-field
              v-model="formData.phone_Number"
              label="Số điện thoại *"
              :rules="phoneRules"
              density="compact"
              variant="outlined"
              class="form-input"
              hide-details="auto"
            >
              <template #prepend-inner>
                <v-icon size="20" color="primary">mdi-phone</v-icon>
              </template>
            </v-text-field>

            <!-- Email -->
            <v-text-field
              v-model="formData.email"
              label="Email *"
              :rules="emailRules"
              type="email"
              density="compact"
              variant="outlined"
              class="form-input"
              hide-details="auto"
            >
              <template #prepend-inner>
                <v-icon size="20" color="primary">mdi-email</v-icon>
              </template>
            </v-text-field>

            <!-- Status (for editing) -->
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
              hide-details="auto"
            >
              <template #prepend-inner>
                <v-icon size="20" color="primary">mdi-check-circle</v-icon>
              </template>
            </v-select>

            <!-- Address -->
            <div class="md:col-span-2">
              <v-textarea
                v-model="formData.address"
                label="Địa chỉ *"
                :rules="addressRules"
                rows="3"
                density="compact"
                variant="outlined"
                class="form-input text-area-input"
                hide-details="auto"
                auto-grow
              >
                <template #prepend-inner>
                  <v-icon size="20" color="primary">mdi-map-marker</v-icon>
                </template>
              </v-textarea>
            </div>

            <!-- Additional Information Section -->
            <div class="md:col-span-2 mt-4">
              <h3 class="section-title">Thông tin bổ sung</h3>
            </div>

            <!-- Tax Code -->
            <v-text-field
              v-model="formData.tax_Code"
              label="Mã số thuế"
              density="compact"
              variant="outlined"
              class="form-input"
              hide-details="auto"
            >
              <template #prepend-inner>
                <v-icon size="20" color="primary">mdi-receipt</v-icon>
              </template>
            </v-text-field>

            <!-- Website -->
            <v-text-field
              v-model="formData.website"
              label="Website"
              type="url"
              density="compact"
              variant="outlined"
              class="form-input"
              hide-details="auto"
            >
              <template #prepend-inner>
                <v-icon size="20" color="primary">mdi-web</v-icon>
              </template>
            </v-text-field>

            <!-- Payment Terms -->
            <v-select
              v-model="formData.payment_Terms"
              :items="paymentTermsOptions"
              item-title="text"
              item-value="value"
              label="Điều khoản thanh toán"
              density="compact"
              variant="outlined"
              class="form-input"
              hide-details="auto"
            >
              <template #prepend-inner>
                <v-icon size="20" color="primary">mdi-credit-card</v-icon>
              </template>
            </v-select>

            <!-- Rating -->
            <v-select
              v-model="formData.rating"
              :items="ratingOptions"
              item-title="text"
              item-value="value"
              label="Đánh giá"
              density="compact"
              variant="outlined"
              class="form-input"
              hide-details="auto"
            >
              <template #prepend-inner>
                <v-icon size="20" color="primary">mdi-star</v-icon>
              </template>
            </v-select>

            <!-- Notes -->
            <div class="md:col-span-2">
              <v-textarea
                v-model="formData.notes"
                label="Ghi chú"
                rows="2"
                density="compact"
                variant="outlined"
                class="form-input text-area-input"
                hide-details="auto"
                auto-grow
              >
                <template #prepend-inner>
                  <v-icon size="20" color="primary">mdi-note-text</v-icon>
                </template>
              </v-textarea>
            </div>
          </div>
        </v-card-text>

        <v-card-actions class="px-6 pb-6">
          <v-spacer />
          <v-btn
            class="form-btn-secondary"
            @click="handleCancel"
          >
            <v-icon size="16" class="mr-1">mdi-close</v-icon>
            Hủy
          </v-btn>
          <v-btn
            class="form-btn-primary"
            type="submit"
            :loading="isLoading"
            :disabled="!isFormValid"
          >
            <v-icon size="16" class="mr-1">{{ isEditing ? 'mdi-content-save' : 'mdi-plus' }}</v-icon>
            {{ isEditing ? 'Cập nhật' : 'Tạo mới' }}
          </v-btn>
        </v-card-actions>
      </v-form>
    </v-card>
  </v-dialog>
</template>

<script setup lang="ts">
import { computed, ref, watch } from 'vue';
import { useSupplierStore } from '@/composables/suppliers/supplierStore';
import { useDialog } from '@/composables/common/useDialog';
import { VForm } from 'vuetify/components';

// Props & Emits
interface SupplierDto {
  supplier_Id?: string;
  supplier_Name: string;
  contact_Person: string;
  phone_Number: string;
  email: string;
  address: string;
  status?: 'active' | 'inactive' | 'pending';
  tax_Code?: string;
  website?: string;
  payment_Terms?: string;
  rating?: number;
  notes?: string;
  created_At?: string | null;
  updated_At?: string | null;
}

interface Props {
  modelValue: boolean;
  supplier?: SupplierDto | null;
}

interface Emits {
  (e: 'update:modelValue', value: boolean): void;
  (e: 'save', success: boolean): void;
}

const props = defineProps<Props>();
const emit = defineEmits<Emits>();

// Store and services
const supplierStore = useSupplierStore();
const dialog = useDialog();

// Local state
const formRef = ref<typeof VForm | null>(null);
const isFormValid = ref(false);
const isLoading = ref(false);

const formData = ref<SupplierDto>({
  supplier_Name: '',
  contact_Person: '',
  phone_Number: '',
  email: '',
  address: '',
  status: 'active',
  tax_Code: '',
  website: '',
  payment_Terms: '',
  rating: undefined,
  notes: ''
});

// Computed
const isEditing = computed(() => !!props.supplier?.supplier_Id);

// Options
const statusOptions = ref([
  { text: 'Đang hợp tác', value: 'active' },
  { text: 'Không hoạt động', value: 'inactive' },
  { text: 'Chờ duyệt', value: 'pending' }
]);

const paymentTermsOptions = ref([
  { text: 'Thanh toán ngay', value: 'immediate' },
  { text: '7 ngày', value: '7_days' },
  { text: '15 ngày', value: '15_days' },
  { text: '30 ngày', value: '30_days' },
  { text: '45 ngày', value: '45_days' },
  { text: '60 ngày', value: '60_days' }
]);

const ratingOptions = ref([
  { text: '5 sao - Xuất sắc', value: 5 },
  { text: '4 sao - Tốt', value: 4 },
  { text: '3 sao - Trung bình', value: 3 },
  { text: '2 sao - Kém', value: 2 },
  { text: '1 sao - Rất kém', value: 1 }
]);

// Validation rules
const supplierNameRules = [
  (v: string) => !!v || 'Tên nhà cung cấp là bắt buộc',
  (v: string) => v.length >= 2 || 'Tên nhà cung cấp phải có ít nhất 2 ký tự',
  (v: string) => v.length <= 100 || 'Tên nhà cung cấp không được quá 100 ký tự'
];

const contactPersonRules = [
  (v: string) => !!v || 'Người liên hệ là bắt buộc',
  (v: string) => v.length >= 2 || 'Tên người liên hệ phải có ít nhất 2 ký tự'
];

const phoneRules = [
  (v: string) => !!v || 'Số điện thoại là bắt buộc',
  (v: string) => /^[0-9+\-\s()]{10,15}$/.test(v) || 'Số điện thoại không hợp lệ'
];

const emailRules = [
  (v: string) => !!v || 'Email là bắt buộc',
  (v: string) => /.+@.+\..+/.test(v) || 'Email không hợp lệ'
];

const addressRules = [
  (v: string) => !!v || 'Địa chỉ là bắt buộc',
  (v: string) => v.length >= 10 || 'Địa chỉ phải có ít nhất 10 ký tự'
];

const statusRules = [
  (v: string) => !!v || 'Trạng thái là bắt buộc'
];

// Methods
const resetForm = (): void => {
  formData.value = {
    supplier_Name: '',
    contact_Person: '',
    phone_Number: '',
    email: '',
    address: '',
    status: 'active',
    tax_Code: '',
    website: '',
    payment_Terms: '',
    rating: undefined,
    notes: ''
  };
  formRef.value?.resetValidation();
};

const loadSupplierData = (supplier: SupplierDto): void => {
  formData.value = {
    supplier_Id: supplier.supplier_Id,
    supplier_Name: supplier.supplier_Name,
    contact_Person: supplier.contact_Person,
    phone_Number: supplier.phone_Number,
    email: supplier.email,
    address: supplier.address,
    status: supplier.status || 'active',
    tax_Code: supplier.tax_Code || '',
    website: supplier.website || '',
    payment_Terms: supplier.payment_Terms || '',
    rating: supplier.rating,
    notes: supplier.notes || ''
  };
};

const handleSubmit = async (): Promise<void> => {
  if (!isFormValid.value) return;

  try {
    isLoading.value = true;
    let success = false;

    if (isEditing.value && formData.value.supplier_Id) {
      // Update existing supplier
      await supplierStore.updateSupplier(formData.value);
      success = true;
      await dialog.success('Cập nhật nhà cung cấp thành công!');
    } else {
      // Create new supplier
      await supplierStore.createSupplier(formData.value);
      success = true;
      await dialog.success('Thêm nhà cung cấp thành công!');
    }

    emit('save', success);
    if (success) {
      resetForm();
    }
  } catch (error) {
    console.error('Error saving supplier:', error);
    await dialog.error(
      isEditing.value 
        ? 'Không thể cập nhật nhà cung cấp' 
        : 'Không thể thêm nhà cung cấp mới'
    );
    emit('save', false);
  } finally {
    isLoading.value = false;
  }
};

const handleCancel = (): void => {
  resetForm();
  emit('update:modelValue', false);
};

// Watchers
watch(() => props.supplier, (newSupplier) => {
  if (newSupplier) {
    loadSupplierData(newSupplier);
  } else {
    resetForm();
  }
}, { immediate: true });

watch(() => props.modelValue, (isOpen) => {
  if (isOpen && !props.supplier) {
    resetForm();
  }
});
</script>

<style scoped>
/* .section-title {
  @apply text-md font-semibold text-gray-700 mb-3 pb-2 border-b border-gray-200;
} */

.form-input :deep(.v-field) {
  @apply rounded;
  background: #f8fafc;
}

.form-input :deep(.v-field--focused .v-field__outline) {
  @apply border-[#11c393];
  box-shadow: none !important;
}

.form-input :deep(.v-field__input) {
  @apply text-gray-700;
}

.form-input :deep(.v-label) {
  @apply text-gray-600;
}

.form-btn-primary {
  @apply rounded px-6 h-10 font-semibold text-white bg-[#11c393] hover:bg-[#10b18d] tracking-wide transition-all duration-200;
  min-width: auto !important;
}

.form-btn-secondary {
  @apply rounded px-6 h-10 font-semibold text-gray-600 bg-gray-100 hover:bg-gray-200 tracking-wide transition-all duration-200;
  min-width: auto !important;
}

/* Form field hover effects */
.form-input:hover :deep(.v-field) {
  @apply border-gray-300;
  transform: translateY(-1px);
}

/* Responsive adjustments */
@media (max-width: 768px) {
  .grid {
    @apply grid-cols-1;
  }
  
  .md\:col-span-2 {
    @apply col-span-1;
  }
}

.text-area-input :deep(.v-field__input) {
  padding-top: 10px !important;
}
</style>