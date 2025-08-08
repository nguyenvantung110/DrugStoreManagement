<template>
  <v-container class="px-2 md:px-8 py-4">
    <v-card elevation="0" class="p-5 rounded-lg bg-white">
      <!-- Header Section -->
      <div class="flex flex-col md:flex-row md:items-center md:justify-between mb-4 gap-4">
        <div>
          <h2 class="text-xl font-bold text-gray-800 mb-1">Bán hàng</h2>
          <p class="text-gray-500 text-sm">Bạn có thể chọn loại thuốc muốn bán hoặc quét mã vạch</p>
        </div>
        
        <div class="flex gap-2 flex-wrap">
          <v-btn
            class="sales-btn-primary"
            elevation="0"
            density="compact"
            size="small"
            @click="selectPrescription"
          >
            <v-icon size="16" class="mr-1">mdi-prescription</v-icon>
            <span class="text-xs">Chọn đơn thuốc</span>
          </v-btn>
          
          <v-btn
            class="sales-btn-primary"
            elevation="0"
            density="compact"
            size="small"
            :disabled="!isValidForPayment"
            @click="handlePayment"
          >
            <v-icon size="16" class="mr-1">mdi-cash-multiple</v-icon>
            <span class="text-xs">Thanh toán ({{ formatCurrency(grandTotal) }})</span>
          </v-btn>
          
          <v-btn
            class="sales-btn-secondary"
            elevation="0"
            density="compact"
            size="small"
            @click="addNewDrug"
          >
            <v-icon size="16" class="mr-1">mdi-plus</v-icon>
            <span class="text-xs">Thêm thuốc</span>
          </v-btn>
          
          <v-btn
            class="sales-btn-secondary"
            elevation="0"
            density="compact"
            size="small"
            @click="handleBarcodeScanning"
          >
            <v-icon size="16" class="mr-1">mdi-barcode-scan</v-icon>
            <span class="text-xs">Quét mã vạch</span>
          </v-btn>
        </div>
      </div>

      <!-- Drug Table -->
      <v-data-table
        :headers="tableHeaders"
        :items="drugList"
        :items-per-page="-1"
        hide-default-footer
        class="drug-table"
        density="compact"
        fixed-header
        height="480"
      >
        <template #item="{ item, index }">
          <tr class="table-row">
            <td class="table-cell text-center py-2">{{ item.seq }}</td>
            
            <!-- Drug Name -->
            <td class="table-cell py-2">
              <v-combobox
                v-model="drugList[index].name"
                :items="drugNameList"
                density="compact"
                variant="outlined"
                class="input-field-sm"
                hide-details
                placeholder="Chọn tên thuốc"
                @update:model-value="handleDrugNameChange(index, $event)"
              />
            </td>
            
            <!-- Quantity -->
            <td class="table-cell py-2">
              <v-text-field
                v-model.number="drugList[index].quantity"
                type="number"
                min="0"
                density="compact"
                variant="outlined"
                class="input-field-xs"
                hide-details
                @input="updateItemTotal(index)"
              />
            </td>
            
            <!-- Unit -->
            <td class="table-cell py-2">
              <v-combobox
                v-model="drugList[index].unit"
                :items="unitOptions"
                density="compact"
                variant="outlined"
                class="input-field-sm"
                hide-details
                placeholder="Đơn vị"
              />
            </td>
            
            <!-- Dosage -->
            <td class="table-cell py-2">
              <v-combobox
                v-model="drugList[index].dosage"
                :items="dosageOptions"
                density="compact"
                variant="outlined"
                class="input-field-md"
                hide-details
                placeholder="Chọn liều dùng"
              />
            </td>
            
            <!-- Unit Price -->
            <td class="table-cell text-center font-medium py-2">
              {{ formatCurrency(item.unitPrice) }}
            </td>
            
            <!-- Subtotal -->
            <td class="table-cell text-center font-bold text-[#1164c3] py-2">
              {{ formatCurrency(item.subTotal) }}
            </td>
            
            <!-- Actions -->
            <td class="table-cell text-center py-2">
              <v-btn
                icon
                size="small"
                variant="plain"
                class="action-delete-btn"
                @click="removeDrugItem(index)"
              >
                <v-icon size="16" color="error">mdi-delete-outline</v-icon>
              </v-btn>
            </td>
          </tr>
        </template>
        
        <!-- Empty state -->
        <template #no-data>
          <div class="text-center py-8 text-gray-500">
            <v-icon size="48" color="grey-lighten-2">mdi-package-variant</v-icon>
            <p class="mt-2">Chưa có thuốc nào được chọn</p>
            <v-btn 
              color="primary" 
              variant="text" 
              @click="addNewDrug"
              class="mt-2"
            >
              Thêm thuốc đầu tiên
            </v-btn>
          </div>
        </template>
      </v-data-table>

      <!-- Total Section -->
      <div class="total-section">
        <div class="flex justify-between items-center">
          <span class="text-lg font-medium text-gray-700">Tổng tiền thanh toán:</span>
          <span class="text-2xl font-bold text-[#11c393] tracking-wide">
            {{ formatCurrency(grandTotal) }}
          </span>
        </div>
      </div>

      <!-- Notes Section -->
      <div class="mt-6">
        <v-textarea
          v-model="notes"
          label="Ghi chú đơn hàng"
          rows="3"
          variant="outlined"
          auto-grow
          class="notes-textarea"
        ></v-textarea>
      </div>
    </v-card>
  </v-container>
</template>

<script setup lang="ts">
import { computed, onMounted, ref } from 'vue';
import { useRouter } from 'vue-router';
import { useSalesStore } from '@/composables/sales/salesStore';
import { useDrugOperations } from '@/composables/drugs/useDrugOperations';
import type { DrugItem, TableHeader } from '@/models/sales';

// Composables
const router = useRouter();
const salesStore = useSalesStore();
const { 
  drugNameList, 
  unitOptions, 
  dosageOptions, 
  createEmptyDrug, 
  generateDummyDrugs,
  validateDrugItem,
  formatCurrency 
} = useDrugOperations();

// Local state
const drugList = ref<DrugItem[]>([]);
const notes = ref('');

// Table configuration
const tableHeaders = ref<TableHeader[]>([
  { title: 'STT', key: 'seq', width: '60px', align: 'center' },
  { title: 'Tên thuốc', key: 'name', width: '250px' },
  { title: 'Số lượng', key: 'quantity', width: '100px' },
  { title: 'Đơn vị', key: 'unit', width: '120px' },
  { title: 'Liều dùng', key: 'dosage', width: '300px' },
  { title: 'Đơn giá', key: 'unitPrice', width: '120px', align: 'center' },
  { title: 'Thành tiền', key: 'subTotal', width: '140px', align: 'center' },
  { title: 'Thao tác', key: 'action', width: '80px', align: 'center' }
]);

// Computed properties
const grandTotal = computed(() => 
  drugList.value.reduce((sum, item) => sum + (item.subTotal || 0), 0)
);

const isValidForPayment = computed(() => 
  drugList.value.length > 0 && 
  drugList.value.some(item => validateDrugItem(item)) &&
  grandTotal.value > 0
);

// Lifecycle
onMounted(() => {
  if (salesStore.getGoBackStatus) {
    drugList.value = [...salesStore.getDrugListToSale];
  } else {
    initializeDummyData();
  }
});

// Methods
const initializeDummyData = (): void => {
  drugList.value = generateDummyDrugs(3);
};

const addNewDrug = (): void => {
  const newDrug = createEmptyDrug(drugList.value.length + 1);
  drugList.value.push(newDrug);
};

const removeDrugItem = (index: number): void => {
  drugList.value.splice(index, 1);
  // Cập nhật lại số thứ tự
  drugList.value.forEach((item, idx) => {
    item.seq = idx + 1;
  });
};

const updateItemTotal = (index: number): void => {
  const item = drugList.value[index];
  if (item) {
    item.subTotal = (item.unitPrice || 0) * (item.quantity || 0);
  }
};

const handleDrugNameChange = (index: number, drugName: string): void => {
  // Logic để load thông tin thuốc từ database dựa trên tên
  // Tạm thời chỉ cập nhật tên
  if (drugList.value[index]) {
    drugList.value[index].name = drugName;
  }
};

const selectPrescription = (): void => {
  // Logic chọn đơn thuốc
  console.log('Chọn đơn thuốc');
};

const handleBarcodeScanning = (): void => {
  // Logic quét mã vạch
  console.log('Quét mã vạch');
};

const handlePayment = (): void => {
  if (!isValidForPayment.value) {
    return;
  }
  
  // Validate dữ liệu trước khi chuyển sang payment
  const validDrugs = drugList.value.filter(validateDrugItem);
  
  if (validDrugs.length === 0) {
    // Show error message
    return;
  }
  
  salesStore.updateDrugListToSale(validDrugs);
  router.push('/payment');
};
</script>

<style scoped>
/* Primary buttons - Chọn đơn thuốc, Thanh toán */
.sales-btn-primary {
  @apply rounded px-4 h-8 font-medium text-white bg-[#11c393] hover:bg-[#10b18d] tracking-wide transition-all duration-200;
  min-width: auto !important;
}

/* Secondary buttons - Thêm thuốc, Quét mã vạch */
.sales-btn-secondary {
  @apply rounded px-4 h-8 font-medium text-[#11c393] bg-white border border-[#11c393] hover:bg-[#11c393] hover:text-white tracking-wide transition-all duration-200;
  min-width: auto !important;
}

/* Button disabled state */
.sales-btn-primary[disabled] {
  @apply opacity-50 cursor-not-allowed bg-gray-400;
}

.sales-btn-secondary[disabled] {
  @apply opacity-50 cursor-not-allowed border-gray-400 text-gray-400;
}

/* Table styling */
.drug-table {
  @apply rounded-lg shadow-sm border border-gray-200 overflow-hidden;
  background: white;
}

.drug-table :deep(thead th) {
  @apply font-semibold text-gray-700 bg-gray-50 text-sm;
  border-bottom: 2px solid #e5e7eb;
  padding: 12px 8px;
  height: 48px;
}

.table-row {
  @apply hover:bg-blue-50/40 transition-colors duration-150;
  border-bottom: 1px solid #f1f5f9;
}

.table-cell {
  @apply px-2 text-sm;
  border-bottom: 1px solid #f1f5f9;
}

/* Input field sizes - nhỏ hơn và compact hơn */
.input-field-xs {
  @apply min-w-[80px] max-w-[90px];
}

.input-field-xs :deep(.v-field) {
  @apply rounded h-8;
  background: #f8fafc;
  min-height: 32px !important;
}

.input-field-sm {
  @apply w-full;
  min-width: 100% !important;
  max-width: 100% !important;
}

.input-field-sm :deep(.v-field) {
  @apply rounded h-8;
  background: #f8fafc;
  min-height: 32px !important;
  width: 100% !important;
}

.input-field-md {
  @apply w-full;
  min-width: 100% !important;
  max-width: 100% !important;
}

.input-field-md :deep(.v-field) {
  @apply rounded h-8;
  background: #f8fafc;
  min-height: 32px !important;
  width: 100% !important;
}

/* Input field common styling - căn giữa theo chiều dọc và sửa text bị cắt */
.input-field-xs :deep(.v-field__input),
.input-field-sm :deep(.v-field__input),
.input-field-md :deep(.v-field__input) {
  @apply text-xs px-2;
  background: transparent;
  min-height: 32px !important;
  height: 32px !important;
  display: flex !important;
  align-items: center !important;
  padding-top: 0 !important;
  padding-bottom: 0 !important;
  line-height: 1.2 !important;
}

.input-field-xs :deep(.v-field__input input),
.input-field-sm :deep(.v-field__input input),
.input-field-md :deep(.v-field__input input) {
  height: 100% !important;
  line-height: 1.2 !important;
  padding: 0 !important;
  margin: 0 !important;
  border: none !important;
  outline: none !important;
  box-shadow: none !important;
}

/* Chỉ căn giữa cho input số lượng */
.input-field-xs :deep(.v-field__input input) {
  text-align: center;
}

/* Căn trái cho tên thuốc và liều dùng */
.input-field-sm :deep(.v-field__input input),
.input-field-md :deep(.v-field__input input) {
  text-align: left;
}

/* Styling cho combobox dropdown */
.input-field-sm :deep(.v-field__input),
.input-field-md :deep(.v-field__input) {
  padding-left: 8px !important;
  padding-right: 32px !important; /* Space for dropdown arrow */
}

.input-field-xs :deep(.v-field__outline),
.input-field-sm :deep(.v-field__outline),
.input-field-md :deep(.v-field__outline) {
  @apply border-gray-300;
}

/* Loại bỏ outline khi focus */
.input-field-xs :deep(.v-field--focused .v-field__outline),
.input-field-sm :deep(.v-field--focused .v-field__outline),
.input-field-md :deep(.v-field--focused .v-field__outline) {
  @apply border-[#11c393];
  box-shadow: none !important;
  outline: none !important;
}

.input-field-xs :deep(.v-field--focused),
.input-field-sm :deep(.v-field--focused),
.input-field-md :deep(.v-field--focused) {
  box-shadow: none !important;
  outline: none !important;
}

.input-field-xs :deep(.v-field__input:focus),
.input-field-sm :deep(.v-field__input:focus),
.input-field-md :deep(.v-field__input:focus) {
  box-shadow: none !important;
  outline: none !important;
}

/* Delete button styling */
.action-delete-btn {
  @apply hover:bg-red-50 rounded-full w-7 h-7;
  transition: all 0.2s ease;
}

.action-delete-btn:hover {
  transform: scale(1.1);
}

/* Total section */
.total-section {
  @apply mt-6 p-4 bg-gradient-to-r from-gray-50 to-blue-50 rounded-xl border border-gray-200;
}

.notes-textarea :deep(.v-field__input) {
  padding-top: 15px !important;
  margin-top: 0 !important;
}
</style>