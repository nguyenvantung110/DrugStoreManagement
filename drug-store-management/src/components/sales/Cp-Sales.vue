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
            @click="openBarcodeScanner"
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
                v-model="drugList[index].productId"
                :items="filteredDrugNames"
                :loading="isSearchingDrugs"
                density="compact"
                variant="outlined"
                class="input-field-sm drug-search-combobox"
                hide-details
                placeholder="Chọn hoặc tìm tên thuốc"
                clearable
                item-title="title"
                item-value="value"
                :search="drugSearchQueries[index]"
                @update:search="handleDrugSearch(index, $event)"
                @update:model-value="handleDrugNameChange(index, $event)"
                @focus="handleDrugInputFocus(index)"
                @blur="handleDrugInputBlur(index)"
              >
                <!-- Custom prepend icon -->
                <template #prepend-inner>
                  <v-icon size="16" color="primary">mdi-pill</v-icon>
                </template>
                <template #selection="{ item }">
                  <span class="drug-selection-text">
                    {{ getDrugDisplayName(drugList[index].productId) }}
                  </span>
                </template>
                <!-- Fixed: Custom dropdown items -->
                <template #item="{ props: itemProps, item }">
                  <v-list-item
                    v-bind="itemProps"
                    class="drug-dropdown-item"
                  >
                    <template #title>
                      <div class="drug-item-content">
                        <div class="drug-name">{{ item.title }}</div>
                      </div>
                    </template>
                    
                    <template #subtitle>
                      <div class="drug-manufacturer">
                        <v-chip size="x-small" color="success" variant="tonal">
                          {{ item.raw.subtitle }}
                        </v-chip>
                      </div>
                    </template>
                  </v-list-item>
                </template>

                <!-- No data state -->
                <template #no-data>
                  <div class="pa-4 text-center">
                    <v-icon size="32" color="grey-lighten-2">mdi-pill-off</v-icon>
                    <div class="text-caption text-grey mt-2">
                      {{ drugSearchQueries[index] ? 'Không tìm thấy thuốc phù hợp' : 'Nhập tên thuốc để tìm kiếm' }}
                    </div>
                    
                    <!-- Quick add new drug option -->
                    <v-btn
                      v-if="drugSearchQueries[index] && drugSearchQueries[index].length >= 3"
                      variant="text"
                      size="small"
                      color="primary"
                      class="mt-2"
                      @click="quickAddDrug(index, drugSearchQueries[index])"
                    >
                      <v-icon size="16" class="mr-1">mdi-plus</v-icon>
                      Thêm "{{ drugSearchQueries[index] }}" vào danh mục
                    </v-btn>
                  </div>
                </template>
              </v-combobox>
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
                @update:model-value="calculateSubTotal(index)"
              />
            </td>
            
            <!-- Unit -->
            <td class="table-cell py-2">
              <v-combobox
                v-model="drugList[index].unitOfMeasure"
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
              {{ formatCurrency(item.pricePerUnit) }}
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

    <PrescriptionSelection
      v-model="showPrescriptionModal"
      @prescription-selected="handlePrescriptionSelected"
    />

    <Cp-Barcode-Scanner
      v-model="showBarcodeScanner"
      :continuous-mode="true"
      @product-scanned="handleProductScanned"
      @products-batch-add="handleBatchProductsAdd"
    />
  </v-container>
</template>

<script setup lang="ts">
import { computed, onMounted, ref } from 'vue';
import { useRouter } from 'vue-router';
import { useSalesStore } from '@/composables/sales/salesStore';
import { DrugSearchResult, useDrugOperations } from '@/composables/drugs/useDrugOperations';
import type { DrugItem, TableHeader } from '@/models/sales';
import PrescriptionSelection from '@/components/sales/Cp-Prescription-Selection.vue';
import { debounce } from 'lodash-es';
import CpBarcodeScanner from '@/components/sales/Cp-Barcode-Scanner.vue';

// Composables
const router = useRouter();
const salesStore = useSalesStore();
const { 
  // drugNameList, 
  unitOptions, 
  dosageOptions, 
  createEmptyDrug, 
  generateDummyDrugs,
  validateDrugItem,
  formatCurrency ,
  loadInitialDrugList,
  searchDrugs,
} = useDrugOperations();

// Local state
const drugList = ref<DrugItem[]>([]);
const notes = ref('');
const showBarcodeScanner = ref(false);
const drugNameList = ref<any[]>([]);

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

const showPrescriptionModal = ref<boolean>(false)
const searchResults = ref<any[]>([]);
const drugSearchQueries = ref<Record<number, string>>({});
const isSearchingDrugs = ref<boolean>(false);

const filteredDrugNames = computed(() => {
  // Combine static drug names with search results
  const staticDrugs = drugNameList.value.slice(0,5);

  const combinedList = [...searchResults.value, ...staticDrugs];
  
  // Remove duplicates based on name
  const uniqueDrugs = combinedList.filter((drug, index, self) => 
    index === self.findIndex(d => d.productName === drug.productName)
  );
  
  return uniqueDrugs.map(drug => {
    // Ensure we have proper display text
    const displayText = drug.productName || 'Tên thuốc không xác định';
    const manufacturer = drug.manufacturer || '';
    
    return {
      title: displayText, // Hiển thị trong dropdown
      value: drug.productId, // Giá trị được chọn
      subtitle: manufacturer,
      raw: drug // Dữ liệu gốc
    };
  });
});


// Lifecycle
onMounted(async() => {
  await initData()
});

const initData = async() => {
  await salesStore.loadInitialDrugList()

  drugNameList.value = [...salesStore.getDrugListToSale];
}

const getDrugDisplayName = (drugIdOrData: any): string => {
  // If no selection, return empty
  if (!drugIdOrData) return '';
  
  // If it's a productId (string), find the drug name
  if (typeof drugIdOrData === 'string') {
    const drug = drugNameList.value.find(d => d.productId === drugIdOrData);
    return drug?.productName || drugIdOrData;
  }
  
  // If it's an object with productName
  if (drugIdOrData?.productName) {
    return drugIdOrData.productName;
  }
  
  // If it's an object with raw data
  if (drugIdOrData?.raw?.productName) {
    return drugIdOrData.raw.productName;
  }
  
  // Fallback
  return String(drugIdOrData);
};

// Computed properties
const grandTotal = computed(() => 
  drugList.value.reduce((sum, item) => sum + (item.subTotal || 0), 0)
);

const isValidForPayment = computed(() => 
  drugList.value.length > 0 && 
  drugList.value.some(item => validateDrugItem(item)) &&
  grandTotal.value > 0
);

const calculateSubTotal = (index: number): void => {
  const item = drugList.value[index];
  item.subTotal = (item.quantity || 0) * (item.pricePerUnit || 0);
};

// Enhanced methods
const handleDrugSearch = (index: number, searchQuery: string): void => {
  drugSearchQueries.value[index] = searchQuery || '';
  
  if (searchQuery) {
    debouncedDrugSearch(searchQuery);
  }
};

const handleDrugInputFocus = (index: number): void => {
  // Clear previous selection to allow fresh search
  drugSearchQueries.value[index] = '';
};

const handleDrugInputBlur = (index: number): void => {
  // Optional: Handle blur event
  setTimeout(() => {
    drugSearchQueries.value[index] = '';
  }, 200);
};

const quickAddDrug = (index: number, drugName: string): void => {
  // Quick add functionality for new drugs
  const newDrug = {
    name: drugName,
    brand: 'Chưa xác định',
    activeIngredient: '',
    price: 0
  };

  // Add to drug list
  drugList.value[index].productName = drugName;
  
  // Optional: Show confirmation or open form to complete drug info
  console.log('Quick adding drug:', drugName);
  // You could emit an event or show a modal here
};


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
    item.subTotal = (item.pricePerUnit || 0) * (item.quantity || 0);
  }
};

const handleDrugNameChange = (index: number, selectedValue: any): void => {console.log('Selected value:', selectedValue, typeof selectedValue);
  
  if (!selectedValue) {
    // Clear the selection
    drugList.value[index].productId = '';
    drugList.value[index].productName = '';
    drugList.value[index].manufacturer = '';
    drugList.value[index].pricePerUnit = 0;
    return;
  }

  // Handle case when user types free text (not selecting from dropdown)
  if (typeof selectedValue === 'string' && !drugNameList.value.find(d => d.productId === selectedValue)) {
    // User typed custom text
    drugList.value[index].productName = selectedValue;
    drugList.value[index].productId = selectedValue; // Or generate unique ID
    drugList.value[index].manufacturer = 'Chưa xác định';
    drugList.value[index].pricePerUnit = 0;
    return;
  }

  // Find drug by productId
  let selectedDrug = null;
  
  if (typeof selectedValue === 'string') {
    selectedDrug = drugNameList.value.find(drug => drug.productId === selectedValue);
  } else if (selectedValue.raw) {
    selectedDrug = selectedValue.raw;
  }
  
  if (selectedDrug) {
    // Update drug list item với thông tin đầy đủ
    drugList.value[index] = {
      ...drugList.value[index],
      productId: selectedDrug.productId,
      productName: selectedDrug.productName,
      manufacturer: selectedDrug.manufacturer,
      unitOfMeasure: selectedDrug.unitOfMeasure || 'viên',
      pricePerUnit: selectedDrug.pricePerUnit || 0,
      // Giữ nguyên các thuộc tính khác
      seq: drugList.value[index].seq,
      quantity: drugList.value[index].quantity || 0
    };

    // Auto-calculate if quantity exists
    if (drugList.value[index].quantity > 0) {
      calculateSubTotal(index);
    }

    // Clear search query for this index
    drugSearchQueries.value[index] = '';

    // Auto-focus next field (quantity)
    setTimeout(() => {
      const quantityInput = document.querySelector(`tr:nth-child(${index + 1}) input[type="number"]`) as HTMLInputElement;
      if (quantityInput) {
        quantityInput.focus();
        quantityInput.select();
      }
    }, 100);
  }
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

const selectPrescription = (): void => {
  showPrescriptionModal.value = true
}

const handlePrescriptionSelected = (prescription: any): void => {
  // Convert prescription items to drug list format
  if (prescription.prescriptionItems) {
    const prescriptionDrugs = prescription.prescriptionItems.map((item: any, index: number) => ({
      seq: drugList.value.length + index + 1,
      name: item.drugName,
      quantity: item.quantity,
      unit: item.unit,
      dosage: item.dosage,
      unitPrice: item.unitPrice,
      subTotal: item.quantity * item.unitPrice,
      prescriptionCode: prescription.prescriptionCode,
      patientName: prescription.patientName
    }))
    
    // Add prescription drugs to current list
    drugList.value.push(...prescriptionDrugs)
    
    // Show success message
  }
}

// Enhanced search function
const debouncedDrugSearch = debounce(async (query: string) => {
  if (!query || query.length < 2) {
    searchResults.value = [];
    isSearchingDrugs.value = false;
    return;
  }

  try {
    isSearchingDrugs.value = true;
    
    // Search trong drugNameList hiện tại
    const filteredResults = drugNameList.value.filter(drug => 
      drug.productName?.toLowerCase().includes(query.toLowerCase()) ||
      drug.manufacturer?.toLowerCase().includes(query.toLowerCase())
    );
    
    searchResults.value = filteredResults;
    
  } catch (error) {
    console.error('Error searching drugs:', error);
    searchResults.value = [];
  } finally {
    isSearchingDrugs.value = false;
  }
}, 300);

const openBarcodeScanner = (): void => {
  showBarcodeScanner.value = true;
};


const handleProductScanned = (product: DrugSearchResult): void => {
  // Find empty row or add new row
  let targetIndex = drugList.value.findIndex(item => !item.productId);
  
  if (targetIndex === -1) {
    addNewDrug();
    targetIndex = drugList.value.length - 1;
  }

  // Fill product data
  drugList.value[targetIndex] = {
    ...drugList.value[targetIndex],
    productId: product.productId!,
    productName: product.productName,
    manufacturer: product.manufacturer,
    unitOfMeasure: product.unitOfMeasure || 'viên',
    pricePerUnit: product.pricePerUnit || 0,
    quantity: 1 // Default quantity
  };

  // Calculate subtotal
  calculateSubTotal(targetIndex);
};

const handleBatchProductsAdd = (products: DrugSearchResult[]): void => {
  products.forEach((product, index) => {
    setTimeout(() => {
      handleProductScanned(product);
    }, index * 100); // Stagger adding products
  });

  showBarcodeScanner.value = false;
  console.log(`Added ${products.length} products from batch scan`);
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

/* Enhanced combobox styling */
.drug-search-combobox :deep(.v-field) {
  @apply rounded border-gray-200;
  background: #fafbfc;
  transition: all 0.2s ease;
}

.drug-search-combobox :deep(.v-field--focused) {
  /* @apply border-primary bg-white; */
  box-shadow: 0 0 0 2px rgba(17, 195, 147, 0.1);
}

.drug-search-combobox :deep(.v-field--focused .v-field__outline) {
  /* @apply border-primary; */
}

/* Enhanced dropdown styling */
.drug-dropdown-item {
  @apply hover:bg-blue-50 transition-colors duration-150;
}

.drug-item-content {
  @apply flex flex-col;
}

.drug-name {
  @apply font-medium text-gray-800 text-sm;
}

.drug-brand {
  /* @apply text-xs text-primary font-medium; */
}

.drug-ingredient {
  @apply text-xs text-gray-500 italic;
}

.drug-selection-text {
  @apply text-sm font-medium;
}

/* Enhanced input field styling */
.input-field-sm :deep(.v-field) {
  @apply rounded border-gray-200;
  background: #fafbfc;
  min-height: 36px;
  font-size: 13px;
}

.input-field-sm :deep(.v-field--focused .v-field__outline) {
  /* @apply border-primary; */
  border-width: 1px !important;
}

.input-field-sm :deep(.v-field__input) {
  @apply text-sm py-1 px-2;
  min-height: 24px;
}

/* Table enhancements */
.drug-table :deep(tbody tr) {
  @apply hover:bg-blue-50/30 transition-colors duration-150;
  border-bottom: 1px solid #f1f5f9;
  height: 64px; /* Increased height for better combobox display */
}

.drug-table :deep(tbody td) {
  @apply px-2;
  border-bottom: 1px solid #f1f5f9;
  padding-top: 12px !important;
  padding-bottom: 12px !important;
  vertical-align: middle;
}

.table-cell {
  @apply align-middle;
}

/* Loading states */
.drug-search-combobox :deep(.v-progress-linear) {
  @apply rounded-b;
}

/* Custom scrollbar for dropdown */
:deep(.v-overlay__content .v-list) {
  max-height: 300px;
  overflow-y: auto;
}

:deep(.v-overlay__content .v-list::-webkit-scrollbar) {
  width: 6px;
}

:deep(.v-overlay__content .v-list::-webkit-scrollbar-track) {
  @apply bg-gray-100 rounded-full;
}

:deep(.v-overlay__content .v-list::-webkit-scrollbar-thumb) {
  @apply bg-gray-300 rounded-full hover:bg-gray-400;
}

/* Responsive adjustments */
@media (max-width: 768px) {
  .drug-table {
    font-size: 12px;
  }
  
  .input-field-sm {
    font-size: 11px;
  }
}
</style>