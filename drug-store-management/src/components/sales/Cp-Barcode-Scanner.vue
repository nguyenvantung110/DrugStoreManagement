<!-- filepath: e:\DrugStoreManagement\drug-store-management\src\components\sales\Cp-Barcode-Scanner.vue -->
<template>
  <v-dialog
    v-model="isVisible"
    max-width="1200px"
    persistent
    class="barcode-scanner-dialog"
  >
    <v-card elevation="0" class="scanner-card rounded-lg">
      <!-- Header -->
      <v-card-title class="scanner-header">
        <div class="flex items-center justify-between w-full">
          <div class="flex items-center gap-3">
            <v-avatar size="36" color="#11c393">
              <v-icon color="white" size="20">mdi-barcode-scan</v-icon>
            </v-avatar>
            <div>
              <h3 class="text-lg font-bold text-gray-800">Quét mã vạch sản phẩm</h3>
              <p class="text-sm text-gray-500 mt-1">
                {{ isContinuousMode ? 'Chế độ quét liên tục' : 'Chế độ quét đơn' }}
              </p>
            </div>
          </div>
          
          <!-- Status and Controls -->
          <div class="flex items-center gap-3">
            <v-chip
              :color="scannerStatus.color"
              :variant="scannerStatus.variant"
              size="small"
              class="status-chip"
            >
              <v-icon start size="14">{{ scannerStatus.icon }}</v-icon>
              {{ scannerStatus.text }}
            </v-chip>
            
            <v-btn
              icon
              variant="plain"
              size="small"
              class="hover:bg-gray-100 rounded-full"
              @click="closeScanner"
            >
              <v-icon>mdi-close</v-icon>
            </v-btn>
          </div>
        </div>
      </v-card-title>

      <!-- Scanner Content -->
      <v-card-text class="scanner-content">
        <div class="grid grid-cols-1 lg:grid-cols-2 gap-6">
          <!-- Camera Section -->
          <div class="camera-section">
            <!-- Camera Container -->
            <v-card elevation="0" class="camera-card rounded-lg border border-gray-200">
              <v-card-text class="p-0">
                <div class="camera-container">
                  <!-- Camera Stream -->
                  <div v-show="isScanning" class="relative">
                    <video
                      ref="videoElement"
                      class="scanner-video"
                      autoplay
                      muted
                      playsinline
                    />
                    
                    <!-- Scanning Overlay -->
                    <div class="scanning-overlay">
                      <div class="scan-frame">
                        <div class="scan-corners">
                          <div class="corner corner-tl"></div>
                          <div class="corner corner-tr"></div>
                          <div class="corner corner-bl"></div>
                          <div class="corner corner-br"></div>
                        </div>
                        
                        <!-- Scanning Line Animation -->
                        <div v-if="isActiveScanning" class="scan-line"></div>
                      </div>
                      
                      <!-- Instructions -->
                      <div class="scan-instructions">
                        <v-chip class="instruction-chip" color="primary" variant="elevated">
                          <v-icon start size="14">mdi-crosshairs-gps</v-icon>
                          Đưa mã vạch vào khung để quét
                        </v-chip>
                      </div>
                    </div>

                    <!-- Success Flash -->
                    <div
                      v-show="showSuccessFlash"
                      class="success-flash"
                      @animationend="showSuccessFlash = false"
                    />
                  </div>

                  <!-- Camera Status Messages -->
                  <div v-show="!isScanning" class="camera-placeholder">
                    <v-icon size="64" color="gray-300">mdi-camera-off</v-icon>
                    <p class="mt-4 text-gray-500 text-center font-medium">
                      {{ cameraStatusMessage }}
                    </p>
                    <v-btn
                      v-if="!hasPermission"
                      class="mt-4 text-white"
                      color="#11c393"
                      variant="flat"
                      @click="requestCameraPermission"
                    >
                      <v-icon start>mdi-camera</v-icon>
                      Cho phép camera
                    </v-btn>
                  </div>
                </div>
              </v-card-text>
            </v-card>

            <!-- Camera Controls -->
            <div class="camera-controls mt-4">
              <div class="flex justify-center gap-3 mb-4">
                <v-btn
                  class="text-white"
                  :color="isScanning ? 'error' : '#11c393'"
                  :variant="isScanning ? 'tonal' : 'flat'"
                  size="large"
                  @click="toggleScanning"
                >
                  <v-icon start>{{ isScanning ? 'mdi-pause' : 'mdi-play' }}</v-icon>
                  {{ isScanning ? 'Dừng quét' : 'Bắt đầu quét' }}
                </v-btn>
                
                <v-btn
                  variant="outlined"
                  size="large"
                  @click="switchCamera"
                  :disabled="!isScanning || availableCameras.length <= 1"
                >
                  <v-icon start>mdi-camera-switch</v-icon>
                  Đổi camera
                </v-btn>
              </div>
              
              <!-- Mode Toggle -->
              <v-card elevation="0" class="mode-toggle-card rounded-lg border border-gray-200">
                <v-card-text class="p-4">
                  <div class="flex items-center justify-between">
                    <div class="flex items-center gap-3">
                      <v-icon color="#11c393">mdi-infinity</v-icon>
                      <div>
                        <p class="text-sm font-medium text-gray-800">Quét liên tục</p>
                        <p class="text-xs text-gray-500">Quét nhiều sản phẩm một lượt</p>
                      </div>
                    </div>
                    <v-switch
                      v-model="isContinuousMode"
                      color="#11c393"
                      hide-details
                      density="compact"
                      @update:model-value="onModeChange"
                    />
                  </div>
                </v-card-text>
              </v-card>
            </div>
          </div>

          <!-- Results Section -->
          <div class="results-section">
            <!-- Statistics Cards -->
            <div class="grid grid-cols-3 gap-3 mb-6">
              <v-card elevation="0" class="stat-card rounded-lg border border-gray-200">
                <v-card-text class="p-4 text-center">
                  <v-avatar size="32" color="success" class="mb-2">
                    <v-icon color="white" size="16">mdi-check-circle</v-icon>
                  </v-avatar>
                  <p class="text-xs text-gray-500 mb-1">Đã quét</p>
                  <p class="text-lg font-bold text-success">{{ scannedCount }}</p>
                </v-card-text>
              </v-card>
              
              <v-card elevation="0" class="stat-card rounded-lg border border-gray-200">
                <v-card-text class="p-4 text-center">
                  <v-avatar size="32" color="error" class="mb-2">
                    <v-icon color="white" size="16">mdi-alert-circle</v-icon>
                  </v-avatar>
                  <p class="text-xs text-gray-500 mb-1">Lỗi</p>
                  <p class="text-lg font-bold text-error">{{ errorCount }}</p>
                </v-card-text>
              </v-card>
              
              <v-card elevation="0" class="stat-card rounded-lg border border-gray-200">
                <v-card-text class="p-4 text-center">
                  <v-avatar size="32" color="info" class="mb-2">
                    <v-icon color="white" size="16">mdi-clock-outline</v-icon>
                  </v-avatar>
                  <p class="text-xs text-gray-500 mb-1">Thời gian</p>
                  <p class="text-lg font-bold text-info">{{ scanDuration }}</p>
                </v-card-text>
              </v-card>
            </div>

            <!-- Recent Scans -->
            <v-card elevation="0" class="recent-scans-card rounded-lg border border-gray-200">
              <v-card-title class="px-4 py-3 border-b border-gray-200">
                <div class="flex items-center justify-between w-full">
                  <div class="flex items-center gap-2">
                    <v-icon color="primary" size="20">mdi-history</v-icon>
                    <span class="text-base font-semibold text-gray-800">Sản phẩm vừa quét</span>
                  </div>
                  <v-btn
                    icon
                    size="small"
                    variant="plain"
                    class="hover:bg-gray-100 rounded-full"
                    @click="clearScannedItems"
                    :disabled="scannedItems.length === 0"
                  >
                    <v-icon size="18">mdi-delete-sweep</v-icon>
                  </v-btn>
                </div>
              </v-card-title>
              
              <v-card-text class="p-0">
                <div class="scanned-items-list">
                  <div
                    v-if="scannedItems.length === 0"
                    class="empty-state p-8 text-center"
                  >
                    <v-icon size="48" color="gray-300">mdi-barcode</v-icon>
                    <p class="text-gray-500 mt-3 text-sm">Chưa có sản phẩm nào được quét</p>
                  </div>
                  
                  <div
                    v-for="(item, index) in recentScannedItems"
                    :key="`${item.barcode}-${index}`"
                    class="scan-result-item"
                    :class="{ 'scan-error': !item.found }"
                  >
                    <div class="p-4 flex items-center justify-between hover:bg-gray-50 transition-colors duration-150">
                      <div class="flex-1">
                        <div class="flex items-center gap-2 mb-2">
                          <v-avatar
                            size="20"
                            :color="item.found ? 'success' : 'error'"
                          >
                            <v-icon
                              size="12"
                              color="white"
                            >
                              {{ item.found ? 'mdi-check' : 'mdi-close' }}
                            </v-icon>
                          </v-avatar>
                          <span class="font-mono text-xs text-gray-600 font-medium">
                            {{ item.barcode }}
                          </span>
                          <v-chip size="x-small" variant="outlined" color="gray">
                            {{ formatScanTime(item.timestamp) }}
                          </v-chip>
                        </div>
                        
                        <div v-if="item.found && item.product">
                          <p class="font-medium text-sm text-gray-800 mb-1">{{ item.product.productName }}</p>
                          <p class="text-xs text-gray-500 mb-2">{{ item.product.manufacturer }}</p>
                          <div class="flex items-center gap-2">
                            <v-chip size="x-small" color="success" variant="tonal">
                              {{ formatCurrency(item.product.pricePerUnit) }}
                            </v-chip>
                            <v-chip size="x-small" color="info" variant="tonal">
                              {{ item.product.unitOfMeasure }}
                            </v-chip>
                          </div>
                        </div>
                        
                        <div v-else>
                          <p class="text-sm text-error font-medium">Không tìm thấy sản phẩm</p>
                          <p class="text-xs text-gray-500">Mã vạch không có trong hệ thống</p>
                        </div>
                      </div>
                      
                      <!-- Actions -->
                      <div class="flex items-center gap-1 ml-3">
                        <v-btn
                          v-if="item.found && item.product"
                          icon
                          size="small"
                          variant="plain"
                          color="primary"
                          class="hover:bg-primary/10 rounded-full"
                          @click="addToSale(item.product)"
                        >
                          <v-icon size="16">mdi-cart-plus</v-icon>
                        </v-btn>
                        
                        <v-btn
                          icon
                          size="small"
                          variant="plain"
                          color="error"
                          class="hover:bg-error/10 rounded-full"
                          @click="removeScannedItem(index)"
                        >
                          <v-icon size="16">mdi-delete</v-icon>
                        </v-btn>
                      </div>
                    </div>
                  </div>
                </div>
              </v-card-text>
            </v-card>
          </div>
        </div>
      </v-card-text>

      <!-- Footer Actions -->
      <v-card-actions class="scanner-footer border-t border-gray-200 bg-gray-50">
        <div class="flex items-center justify-between w-full p-2">
          <div class="scan-summary">
            <span class="text-sm text-gray-600 font-medium">
              Đã quét {{ scannedCount }} sản phẩm, {{ foundItemsCount }} tìm thấy
            </span>
          </div>
          
          <div class="flex gap-3">
            <v-btn
              class="text-white"
              color="#11c393"
              variant="flat"
              @click="addAllFoundItems"
              :disabled="foundItemsCount === 0"
            >
              <v-icon start size="16">mdi-cart-plus</v-icon>
              Thêm tất cả ({{ foundItemsCount }})
            </v-btn>
            
            <v-btn
              variant="outlined"
              @click="closeScanner"
            >
              Đóng
            </v-btn>
          </div>
        </div>
      </v-card-actions>
    </v-card>

    <!-- Success Snackbar -->
    <v-snackbar
      v-model="showSuccessSnackbar"
      color="success"
      timeout="2000"
      location="bottom right"
      elevation="3"
    >
      <div class="flex items-center gap-2">
        <v-icon>mdi-check-circle</v-icon>
        <span>{{ successMessage }}</span>
      </div>
      <template #actions>
        <v-btn icon="mdi-close" variant="text" @click="showSuccessSnackbar = false" />
      </template>
    </v-snackbar>

    <!-- Error Snackbar -->
    <v-snackbar
      v-model="showErrorSnackbar"
      color="error"
      timeout="3000"
      location="bottom right"
      elevation="3"
    >
      <div class="flex items-center gap-2">
        <v-icon>mdi-alert-circle</v-icon>
        <span>{{ errorMessage }}</span>
      </div>
      <template #actions>
        <v-btn icon="mdi-close" variant="text" @click="showErrorSnackbar = false" />
      </template>
    </v-snackbar>
  </v-dialog>
</template>

<script setup lang="ts">
import { computed, onMounted, onUnmounted, ref, watch } from 'vue';
// import { Html5QrcodeScanner, Html5Qrcode } from 'html5-qrcode';
import { useDrugOperations } from '@/composables/drugs/useDrugOperations';
import type { DrugSearchResult } from '@/composables/drugs/useDrugOperations';

// Props & Emits
interface Props {
  modelValue: boolean;
  continuousMode?: boolean;
}

interface ScannedItem {
  barcode: string;
  timestamp: Date;
  found: boolean;
  product?: DrugSearchResult;
}

interface Emits {
  (e: 'update:modelValue', value: boolean): void;
  (e: 'product-scanned', product: DrugSearchResult): void;
  (e: 'products-batch-add', products: DrugSearchResult[]): void;
}

const props = withDefaults(defineProps<Props>(), {
  continuousMode: false
});

const emit = defineEmits<Emits>();

// Composables
const drugOperations = useDrugOperations();

// Local state
const videoElement = ref<HTMLVideoElement | null>(null);
const isScanning = ref(false);
const isActiveScanning = ref(false);
const isContinuousMode = ref(props.continuousMode);
const hasPermission = ref(false);
const availableCameras = ref<MediaDeviceInfo[]>([]);
const currentCameraIndex = ref(0);
// const scanner = ref<Html5Qrcode | null>(null);

// Scan data
const scannedItems = ref<ScannedItem[]>([]);
const scannedCount = ref(0);
const errorCount = ref(0);
const scanStartTime = ref<Date | null>(null);
const scanDuration = ref('00:00');

// UI state
const showSuccessFlash = ref(false);
const showSuccessSnackbar = ref(false);
const showErrorSnackbar = ref(false);
const successMessage = ref('');
const errorMessage = ref('');

// Computed properties
const isVisible = computed({
  get: () => props.modelValue,
  set: (value) => emit('update:modelValue', value)
});

const scannerStatus = computed(() => {
  if (!hasPermission.value) {
    return {
      icon: 'mdi-camera-off',
      text: 'Cần quyền camera',
      color: 'warning',
      variant: 'tonal' as const
    };
  }
  if (!isScanning.value) {
    return {
      icon: 'mdi-pause',
      text: 'Tạm dừng',
      color: 'grey',
      variant: 'tonal' as const
    };
  }
  return {
    icon: 'mdi-radar',
    text: 'Đang quét',
    color: 'success',
    variant: 'flat' as const
  };
});

const cameraStatusMessage = computed(() => {
  if (!hasPermission.value) {
    return 'Cần cấp quyền truy cập camera để quét mã vạch';
  }
  if (availableCameras.value.length === 0) {
    return 'Không tìm thấy camera nào';
  }
  return 'Camera đã sẵn sàng';
});

const recentScannedItems = computed(() => {
  return scannedItems.value.slice(-10).reverse();
});

const foundItemsCount = computed(() => {
  return scannedItems.value.filter(item => item.found).length;
});

// Methods - Simplified for demonstration
const requestCameraPermission = async (): Promise<void> => {
  try {
    await navigator.mediaDevices.getUserMedia({ video: true });
    hasPermission.value = true;
    await getCameraDevices();
  } catch (error) {
    console.error('Camera permission denied:', error);
    showError('Không thể truy cập camera. Vui lòng cấp quyền truy cập.');
  }
};

const getCameraDevices = async (): Promise<void> => {
  try {
    const devices = await navigator.mediaDevices.enumerateDevices();
    availableCameras.value = devices.filter(device => device.kind === 'videoinput');
  } catch (error) {
    console.error('Error getting camera devices:', error);
  }
};

const initializeScanner = async (): Promise<void> => {
  // Simplified initialization
  isScanning.value = true;
  isActiveScanning.value = true;
  startTimer();
};

const toggleScanning = async (): Promise<void> => {
  if (isScanning.value) {
    await stopScanning();
  } else {
    await initializeScanner();
  }
};

const stopScanning = async (): Promise<void> => {
  isScanning.value = false;
  isActiveScanning.value = false;
  stopTimer();
};

const switchCamera = async (): Promise<void> => {
  if (availableCameras.value.length <= 1) return;
  
  await stopScanning();
  currentCameraIndex.value = (currentCameraIndex.value + 1) % availableCameras.value.length;
  await initializeScanner();
};

const onModeChange = (continuous: boolean): void => {
  if (continuous) {
    showSuccess('Chuyển sang chế độ quét liên tục');
  } else {
    showSuccess('Chuyển sang chế độ quét đơn');
  }
};

const addToSale = (product: DrugSearchResult): void => {
  emit('product-scanned', product);
  showSuccess(`Đã thêm ${product.productName} vào đơn hàng`);
};

const addAllFoundItems = (): void => {
  const foundProducts = scannedItems.value
    .filter(item => item.found && item.product)
    .map(item => item.product!);
  
  if (foundProducts.length > 0) {
    emit('products-batch-add', foundProducts);
    showSuccess(`Đã thêm ${foundProducts.length} sản phẩm vào đơn hàng`);
  }
};

const removeScannedItem = (index: number): void => {
  const actualIndex = scannedItems.value.length - 1 - index;
  scannedItems.value.splice(actualIndex, 1);
  scannedCount.value--;
};

const clearScannedItems = (): void => {
  scannedItems.value = [];
  scannedCount.value = 0;
  errorCount.value = 0;
  scanDuration.value = '00:00';
};

const startTimer = (): void => {
  scanStartTime.value = new Date();
  const timer = setInterval(() => {
    if (!isScanning.value || !scanStartTime.value) {
      clearInterval(timer);
      return;
    }
    
    const elapsed = Math.floor((new Date().getTime() - scanStartTime.value.getTime()) / 1000);
    const minutes = Math.floor(elapsed / 60);
    const seconds = elapsed % 60;
    scanDuration.value = `${minutes.toString().padStart(2, '0')}:${seconds.toString().padStart(2, '0')}`;
  }, 1000);
};

const stopTimer = (): void => {
  scanStartTime.value = null;
};

const formatScanTime = (timestamp: Date): string => {
  return timestamp.toLocaleTimeString('vi-VN', {
    hour: '2-digit',
    minute: '2-digit',
    second: '2-digit'
  });
};

const formatCurrency = (amount: number): string => {
  return new Intl.NumberFormat('vi-VN', {
    style: 'currency',
    currency: 'VND'
  }).format(amount);
};

const showSuccess = (message: string): void => {
  successMessage.value = message;
  showSuccessSnackbar.value = true;
};

const showError = (message: string): void => {
  errorMessage.value = message;
  showErrorSnackbar.value = true;
};

const closeScanner = async (): Promise<void> => {
  await stopScanning();
  isVisible.value = false;
};

// Watchers
watch(isVisible, async (newValue) => {
  if (newValue) {
    await requestCameraPermission();
  } else {
    await stopScanning();
  }
});

// Lifecycle
onMounted(async () => {
  if (isVisible.value) {
    await requestCameraPermission();
  }
});

onUnmounted(async () => {
  await stopScanning();
});
</script>

<style scoped>
/* Dialog and Card Styling */
.barcode-scanner-dialog :deep(.v-dialog) {
  align-items: flex-start !important;
  padding-top: 20px;
}

.scanner-card {
  @apply bg-white;
  max-height: calc(100vh - 40px);
  overflow: hidden;
}

.scanner-header {
  @apply px-6 py-4 bg-white;
  border-bottom: 1px solid #f1f5f9;
}

.scanner-content {
  @apply px-6 py-6 bg-white;
  max-height: calc(100vh - 220px);
  overflow-y: auto;
}

/* Camera Section */
.camera-card {
  @apply bg-white;
}

.camera-container {
  @apply relative bg-gray-900 rounded-lg overflow-hidden;
  aspect-ratio: 4/3;
  min-height: 320px;
}

.scanner-video {
  @apply w-full h-full object-cover;
}

.scanning-overlay {
  @apply absolute inset-0 flex flex-col items-center justify-center;
}

.scan-frame {
  @apply relative;
  width: 250px;
  height: 250px;
}

.scan-corners {
  @apply absolute inset-0;
}

.corner {
  @apply absolute w-8 h-8 border-4;
  border-color: #11c393;
}

.corner-tl {
  @apply top-0 left-0;
  border-right: none;
  border-bottom: none;
}

.corner-tr {
  @apply top-0 right-0;
  border-left: none;
  border-bottom: none;
}

.corner-bl {
  @apply bottom-0 left-0;
  border-right: none;
  border-top: none;
}

.corner-br {
  @apply bottom-0 right-0;
  border-left: none;
  border-top: none;
}

.scan-line {
  @apply absolute w-full h-0.5;
  background-color: #11c393;
  top: 50%;
  animation: scanLine 2s infinite;
  box-shadow: 0 0 10px #11c393;
}

@keyframes scanLine {
  0%, 100% { 
    top: 20%; 
    opacity: 1; 
  }
  50% { 
    top: 80%; 
    opacity: 0.7; 
  }
}

.scan-instructions {
  @apply absolute bottom-6 left-1/2 transform -translate-x-1/2;
}

.instruction-chip {
  @apply shadow-lg;
}

.success-flash {
  @apply absolute inset-0 bg-green-400 opacity-20;
  animation: flashSuccess 0.3s ease-out;
}

@keyframes flashSuccess {
  0% { opacity: 0.5; }
  100% { opacity: 0; }
}

.camera-placeholder {
  @apply absolute inset-0 flex flex-col items-center justify-center text-center p-6;
}

/* Controls and Cards */
.mode-toggle-card {
  @apply bg-white;
}

.stat-card {
  @apply bg-white hover:shadow-md transition-shadow duration-200;
}

.recent-scans-card {
  @apply bg-white;
}

.scanned-items-list {
  @apply max-h-80 overflow-y-auto;
}

.scan-result-item {
  @apply border-l-4 transition-all duration-200;
  border-left-color: #10b981;
}

.scan-result-item.scan-error {
  border-left-color: #ef4444;
}

.scan-result-item:not(:last-child) {
  @apply border-b border-gray-100;
}

.empty-state {
  @apply bg-gray-50;
}

/* Footer */
.scanner-footer {
  @apply px-6 py-4 bg-gray-50;
}

/* Status Chip */
.status-chip {
  @apply font-medium;
}

/* Button Styling */
.scanner-footer .v-btn {
  @apply font-medium;
}

/* Scrollbar */
.scanned-items-list::-webkit-scrollbar {
  width: 6px;
}

.scanned-items-list::-webkit-scrollbar-track {
  @apply bg-gray-100 rounded-full;
}

.scanned-items-list::-webkit-scrollbar-thumb {
  @apply bg-gray-300 rounded-full hover:bg-gray-400;
}

.scanner-content::-webkit-scrollbar {
  width: 6px;
}

.scanner-content::-webkit-scrollbar-track {
  @apply bg-gray-100 rounded-full;
}

.scanner-content::-webkit-scrollbar-thumb {
  @apply bg-gray-300 rounded-full hover:bg-gray-400;
}

/* Responsive Design */
@media (max-width: 1024px) {
  .grid.lg\\:grid-cols-2 {
    @apply grid-cols-1;
  }
  
  .camera-container {
    min-height: 280px;
  }
  
  .scanner-content {
    @apply px-4 py-4;
  }
  
  .scanner-header {
    @apply px-4 py-3;
  }
  
  .scanner-footer {
    @apply px-4 py-3;
  }
}

@media (max-width: 640px) {
  .grid.grid-cols-3 {
    @apply grid-cols-1;
  }
  
  .camera-container {
    min-height: 240px;
  }
  
  .scan-frame {
    width: 200px;
    height: 200px;
  }
}
</style>