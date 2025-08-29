<template>
  <v-container class="px-10 md:px-8 py-4">
    <!-- Welcome Header -->
    <div class="mb-8 text-center">
      <div class="flex items-center justify-center gap-4 mb-4">
        <div class="p-3 bg-[#11c393] rounded-2xl shadow-lg">
          <v-icon size="32" color="white">mdi-pill-multiple</v-icon>
        </div>
        <div class="text-left">
          <h1 class="text-2xl md:text-3xl font-bold text-gray-800">Chào mừng đến với Nhà thuốc Xanh</h1>
          <p class="text-gray-500 text-sm md:text-base">Hệ thống quản lý hiện đại, an toàn và thông minh</p>
        </div>
      </div>
    </div>

    <!-- Statistics Overview -->
    <div class="grid grid-cols-2 md:grid-cols-4 gap-4 mb-6">
      <v-card 
        v-for="stat in statistics" 
        :key="stat.title"
        class="stat-card"
        elevation="2"
      >
        <v-card-text class="p-4">
          <div class="flex items-center justify-between">
            <div class="text-left">
              <p class="text-2xl font-bold" :class="stat.valueColor">{{ stat.value }}</p>
              <p class="text-xs text-gray-600 mt-1">{{ stat.title }}</p>
              <div class="flex items-center gap-1 mt-1">
                <v-icon 
                  :color="stat.trend === 'up' ? 'success' : 'error'" 
                  size="12"
                >
                  {{ stat.trend === 'up' ? 'mdi-trending-up' : 'mdi-trending-down' }}
                </v-icon>
                <span 
                  class="text-xs font-medium"
                  :class="stat.trend === 'up' ? 'text-green-600' : 'text-red-600'"
                >
                  {{ stat.change }}
                </span>
              </div>
            </div>
            <div class="p-3 rounded-full" :style="{ backgroundColor: stat.bgColor }">
              <v-icon :color="stat.iconColor" size="20">{{ stat.icon }}</v-icon>
            </div>
          </div>
        </v-card-text>
      </v-card>
    </div>

    <!-- Main Dashboard Content -->
    <div class="grid grid-cols-1 lg:grid-cols-3 gap-6 mb-6">
      <!-- Sales Chart -->
      <v-card class="lg:col-span-2 dashboard-card" elevation="2">
        <v-card-title class="card-title">
          <div class="flex items-center gap-2">
            <v-icon color="primary" size="20">mdi-chart-line</v-icon>
            <span>Doanh thu 30 ngày gần nhất</span>
          </div>
          <v-chip color="success" variant="tonal" size="small">
            +12.5%
          </v-chip>
        </v-card-title>
        
        <v-card-text class="py-4">
          <div class="h-80">
            <apexchart
              type="area"
              height="100%"
              :options="salesChartOptions"
              :series="salesChartSeries"
            />
          </div>
        </v-card-text>
      </v-card>

      <!-- Top Selling Products Chart -->
      <v-card class="dashboard-card" elevation="2">
        <v-card-title class="card-title">
          <div class="flex items-center gap-2">
            <v-icon color="primary" size="20">mdi-chart-donut</v-icon>
            <span>Sản phẩm bán chạy</span>
          </div>
          <v-btn
            variant="text"
            size="small"
            class="text-[#11c393]"
            @click="$router.push('/inventorymanagement')"
          >
            Chi tiết
            <v-icon size="14" class="ml-1">mdi-arrow-right</v-icon>
          </v-btn>
        </v-card-title>
        
        <v-card-text class="py-4">
          <div class="h-48 mb-4">
            <apexchart
              type="donut"
              height="100%"
              :options="topProductsChartOptions"
              :series="topProductsChartSeries"
            />
          </div>
          
          <!-- Product Legend -->
          <div class="space-y-1.5 max-h-32 overflow-y-auto">
            <div
              v-for="(product, index) in topSellingProducts"
              :key="product.name"
              class="flex items-center justify-between p-1.5 rounded-lg hover:bg-gray-50 transition-colors"
            >
              <div class="flex items-center gap-2">
                <div 
                  class="w-2.5 h-2.5 rounded-full flex-shrink-0"
                  :style="{ backgroundColor: productChartColors[index] }"
                />
                <span class="text-xs font-medium text-gray-700 truncate">{{ product.name }}</span>
              </div>
              <div class="text-right flex-shrink-0 ml-2">
                <p class="text-xs font-bold text-gray-800">{{ product.quantity }}</p>
                <p class="text-xs text-gray-500">{{ product.percentage }}%</p>
              </div>
            </div>
          </div>
        </v-card-text>
      </v-card>
    </div>

    <!-- Recent Activities & Notifications -->
    <div class="grid grid-cols-1 lg:grid-cols-2 gap-6 mb-6">
      <!-- Recent Orders -->
      <v-card class="dashboard-card" elevation="2">
        <v-card-title class="card-title">
          <div class="flex items-center gap-2">
            <v-icon color="#11c393" size="25">mdi-receipt-text-clock-outline</v-icon>
            <span>Đơn hàng gần đây</span>
          </div>
          <v-btn
            variant="text"
            size="small"
            class="text-[#11c393]"
            @click="$router.push('/sales')"
          >
            Xem tất cả
            <v-icon size="14" class="ml-1">mdi-arrow-right</v-icon>
          </v-btn>
        </v-card-title>
        
        <v-card-text class="pb-6">
          <div class="space-y-3">
            <div
              v-for="order in recentOrders"
              :key="order.id"
              class="flex items-center gap-3 p-3 rounded-lg hover:bg-gray-50 transition-colors cursor-pointer"
            >
              <v-avatar size="32" :color="getOrderStatusColor(order.status)">
                <v-icon color="white" size="16">mdi-receipt-text-clock-outline</v-icon>
              </v-avatar>
              <div class="flex-1">
                <div class="flex justify-between items-start">
                  <div>
                    <p class="font-medium text-sm">{{ order.customerName }}</p>
                    <p class="text-xs text-gray-500">{{ order.orderCode }}</p>
                  </div>
                  <div class="text-right">
                    <p class="font-medium text-sm text-green-600">{{ formatCurrency(order.total) }}</p>
                    <p class="text-xs text-gray-500">{{ formatTime(order.createdAt) }}</p>
                  </div>
                </div>
              </div>
              <v-chip
                :color="getOrderStatusColor(order.status)"
                variant="tonal"
                size="x-small"
              >
                {{ getOrderStatusText(order.status) }}
              </v-chip>
            </div>
          </div>
        </v-card-text>
      </v-card>

      <!-- Notifications & Alerts -->
      <v-card class="dashboard-card" elevation="2">
        <v-card-title class="card-title">
          <div class="flex items-center gap-2">
            <v-icon color="primary" size="20">mdi-bell-outline</v-icon>
            <span>Thông báo & Cảnh báo</span>
          </div>
          <v-chip color="error" size="small" variant="tonal">
            {{ notifications.filter(n => n.priority === 'high').length }} quan trọng
          </v-chip>
        </v-card-title>
        
        <v-card-text class="pb-6">
          <div class="space-y-3 max-h-64 overflow-y-auto">
            <div
              v-for="notification in notifications"
              :key="notification.id"
              class="flex items-start gap-3 p-3 rounded-lg hover:bg-gray-50 transition-colors"
              :class="notification.priority === 'high' ? 'bg-red-50 border border-red-200' : ''"
            >
              <div 
                class="p-2 rounded-full mt-1"
                :style="{ backgroundColor: notification.bgColor }"
              >
                <v-icon :color="notification.iconColor" size="14">{{ notification.icon }}</v-icon>
              </div>
              <div class="flex-1">
                <p class="text-sm font-medium text-gray-800">{{ notification.title }}</p>
                <p class="text-xs text-gray-600 mt-1">{{ notification.message }}</p>
                <p class="text-xs text-gray-500 mt-2">{{ formatTime(notification.createdAt) }}</p>
              </div>
              <v-btn
                v-if="notification.priority === 'high'"
                icon
                size="x-small"
                variant="plain"
                color="error"
              >
                <v-icon size="16">mdi-close</v-icon>
              </v-btn>
            </div>
          </div>
        </v-card-text>
      </v-card>
    </div>

    <!-- Inventory Alerts -->
    <v-card class="dashboard-card" elevation="2">
      <v-card-title class="card-title">
        <div class="flex items-center gap-2">
          <v-icon color="primary" size="20">mdi-package-variant</v-icon>
          <span>Cảnh báo tồn kho</span>
        </div>
        <v-btn
          variant="text"
          size="small"
          class="text-[#11c393]"
          @click="$router.push('/inventorymanagement')"
        >
          Quản lý kho
          <v-icon size="14" class="ml-1">mdi-arrow-right</v-icon>
        </v-btn>
      </v-card-title>
      
      <v-card-text class="pb-6">
        <div class="grid grid-cols-1 md:grid-cols-3 gap-4">
          <!-- Low Stock -->
          <div class="p-4 bg-orange-50 rounded-lg border border-orange-200">
            <div class="flex items-center justify-between mb-3">
              <div class="flex items-center gap-2">
                <v-icon color="warning" size="20">mdi-alert-circle</v-icon>
                <span class="font-medium text-orange-700">Sắp hết hàng</span>
              </div>
              <v-chip color="warning" size="small" variant="tonal">{{ lowStockItems.length }}</v-chip>
            </div>
            <div class="space-y-2">
              <div v-for="item in lowStockItems.slice(0, 3)" :key="item.id" class="text-sm">
                <div class="flex justify-between">
                  <span class="text-gray-700">{{ item.name }}</span>
                  <span class="font-medium text-orange-600">{{ item.quantity }} còn lại</span>
                </div>
              </div>
              <v-btn
                v-if="lowStockItems.length > 3"
                variant="text"
                size="small"
                class="text-orange-600 p-0"
              >
                Xem thêm {{ lowStockItems.length - 3 }} sản phẩm
              </v-btn>
            </div>
          </div>

          <!-- Expiring Soon -->
          <div class="p-4 bg-red-50 rounded-lg border border-red-200">
            <div class="flex items-center justify-between mb-3">
              <div class="flex items-center gap-2">
                <v-icon color="error" size="20">mdi-clock-alert</v-icon>
                <span class="font-medium text-red-700">Sắp hết hạn</span>
              </div>
              <v-chip color="error" size="small" variant="tonal">{{ expiringItems.length }}</v-chip>
            </div>
            <div class="space-y-2">
              <div v-for="item in expiringItems.slice(0, 3)" :key="item.id" class="text-sm">
                <div class="flex justify-between">
                  <span class="text-gray-700">{{ item.name }}</span>
                  <span class="font-medium text-red-600">{{ item.daysToExpiry }} ngày</span>
                </div>
              </div>
              <v-btn
                v-if="expiringItems.length > 3"
                variant="text"
                size="small"
                class="text-red-600 p-0"
              >
                Xem thêm {{ expiringItems.length - 3 }} sản phẩm
              </v-btn>
            </div>
          </div>

          <!-- Out of Stock -->
          <div class="p-4 bg-gray-50 rounded-lg border border-gray-200">
            <div class="flex items-center justify-between mb-3">
              <div class="flex items-center gap-2">
                <v-icon color="grey" size="20">mdi-package-variant-closed-remove</v-icon>
                <span class="font-medium text-gray-700">Hết hàng</span>
              </div>
              <v-chip color="grey" size="small" variant="tonal">{{ outOfStockItems.length }}</v-chip>
            </div>
            <div class="space-y-2">
              <div v-for="item in outOfStockItems.slice(0, 3)" :key="item.id" class="text-sm">
                <div class="flex justify-between">
                  <span class="text-gray-700">{{ item.name }}</span>
                  <span class="font-medium text-gray-500">Hết hàng</span>
                </div>
              </div>
              <v-btn
                v-if="outOfStockItems.length > 3"
                variant="text"
                size="small"
                class="text-gray-600 p-0"
              >
                Xem thêm {{ outOfStockItems.length - 3 }} sản phẩm
              </v-btn>
            </div>
          </div>
        </div>
      </v-card-text>
    </v-card>
  </v-container>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue';
import { useRouter } from 'vue-router';

const router = useRouter();

// Mock data - replace with real API calls
const statistics = ref([
  {
    title: 'Doanh thu hôm nay',
    value: '12.5M',
    valueColor: 'text-green-600',
    icon: 'mdi-currency-usd',
    iconColor: 'white',
    bgColor: '#dcfce7',
    trend: 'up',
    change: '+8.2%'
  },
  {
    title: 'Đơn hàng',
    value: '156',
    valueColor: 'text-blue-600',
    icon: 'mdi-cart',
    iconColor: 'white',
    bgColor: '#dbeafe',
    trend: 'up',
    change: '+12%'
  },
  {
    title: 'Khách hàng',
    value: '2,341',
    valueColor: 'text-purple-600',
    icon: 'mdi-account-group',
    iconColor: 'white',
    bgColor: '#ede9fe',
    trend: 'up',
    change: '+5.1%'
  },
  {
    title: 'Tồn kho',
    value: '1,247',
    valueColor: 'text-orange-600',
    icon: 'mdi-package-variant',
    iconColor: 'white',
    bgColor: '#fed7aa',
    trend: 'down',
    change: '-2.3%'
  }
]);

// Top selling products data
const topSellingProducts = ref([
  { name: 'Paracetamol 500mg', quantity: 245, percentage: 35.2 },
  { name: 'Amoxicillin 250mg', quantity: 156, percentage: 22.4 },
  { name: 'Ibuprofen 400mg', quantity: 98, percentage: 14.1 },
  { name: 'Vitamin C', quantity: 87, percentage: 12.5 },
  { name: 'Aspirin 100mg', quantity: 65, percentage: 9.3 },
  { name: 'Khác', quantity: 45, percentage: 6.5 }
]);

const productChartColors = ref([
  '#11c393',  // Primary green
  '#3b82f6',  // Blue
  '#8b5cf6',  // Purple
  '#f59e0b',  // Orange
  '#ef4444',  // Red
  '#6b7280'   // Gray
]);

const topProductsChartSeries = computed(() => 
  topSellingProducts.value.map(product => product.quantity)
);

const topProductsChartOptions = ref({
  chart: {
    type: 'donut',
    toolbar: { show: false },
    offsetY: -10
  },
  colors: productChartColors.value,
  labels: topSellingProducts.value.map(product => product.name),
  dataLabels: {
    enabled: true,
    formatter: (val: number) => `${val.toFixed(1)}%`,
    style: {
      fontSize: '10px',
      fontWeight: 600,
      colors: ['#fff']
    },
    dropShadow: {
      enabled: true,
      top: 1,
      left: 1,
      blur: 1,
      color: '#000',
      opacity: 0.45
    }
  },
  stroke: {
    show: true,
    width: 2,
    colors: ['#ffffff']
  },
  plotOptions: {
    pie: {
      donut: {
        size: '65%',
        labels: {
          show: true,
          total: {
            show: true,
            showAlways: true,
            label: 'Tổng cộng',
            fontSize: '12px',
            fontWeight: 600,
            color: '#374151',
            formatter: () => {
              const total = topSellingProducts.value.reduce((sum, product) => sum + product.quantity, 0);
              return `${total}`;
            }
          },
          value: {
            show: true,
            fontSize: '16px',
            fontWeight: 'bold',
            color: '#11c393',
            formatter: () => 'sản phẩm'
          }
        }
      }
    }
  },
  legend: {
    show: false
  },
  tooltip: {
    theme: 'light',
    y: {
      formatter: (value: number) => `${value} sản phẩm`
    }
  },
  responsive: [{
    breakpoint: 768,
    options: {
      dataLabels: {
        style: {
          fontSize: '9px'
        }
      },
      stroke: {
        width: 2
      },
      plotOptions: {
        pie: {
          donut: {
            size: '60%',
            labels: {
              total: {
                fontSize: '11px'
              },
              value: {
                fontSize: '14px'
              }
            }
          }
        }
      }
    }
  }]
});

const recentOrders = ref([
  {
    id: '1',
    orderCode: 'HD001',
    customerName: 'Nguyễn Văn An',
    total: 250000,
    status: 'completed',
    createdAt: new Date(Date.now() - 1000 * 60 * 30) // 30 minutes ago
  },
  {
    id: '2',
    orderCode: 'HD002',
    customerName: 'Trần Thị Bình',
    total: 180000,
    status: 'pending',
    createdAt: new Date(Date.now() - 1000 * 60 * 60 * 2) // 2 hours ago
  },
  {
    id: '3',
    orderCode: 'HD003',
    customerName: 'Lê Minh Cường',
    total: 95000,
    status: 'completed',
    createdAt: new Date(Date.now() - 1000 * 60 * 60 * 4) // 4 hours ago
  },
  {
    id: '4',
    orderCode: 'HD004',
    customerName: 'Phạm Thu Dung',
    total: 320000,
    status: 'completed',
    createdAt: new Date(Date.now() - 1000 * 60 * 60 * 6) // 6 hours ago
  }
]);

const notifications = ref([
  {
    id: '1',
    title: 'Thuốc sắp hết hạn',
    message: '5 loại thuốc sẽ hết hạn trong 7 ngày tới',
    icon: 'mdi-clock-alert',
    iconColor: 'white',
    bgColor: '#fecaca',
    priority: 'high',
    createdAt: new Date(Date.now() - 1000 * 60 * 15) // 15 minutes ago
  },
  {
    id: '2',
    title: 'Tồn kho thấp',
    message: 'Paracetamol 500mg còn lại 10 viên',
    icon: 'mdi-alert-circle',
    iconColor: 'white',
    bgColor: '#fed7aa',
    priority: 'medium',
    createdAt: new Date(Date.now() - 1000 * 60 * 60 * 1) // 1 hour ago
  },
  {
    id: '3',
    title: 'Đơn hàng mới',
    message: 'Có 3 đơn hàng mới cần xử lý',
    icon: 'mdi-cart-plus',
    iconColor: 'white',
    bgColor: '#bfdbfe',
    priority: 'low',
    createdAt: new Date(Date.now() - 1000 * 60 * 60 * 2) // 2 hours ago
  },
  {
    id: '4',
    title: 'Nhà cung cấp',
    message: 'Nhà cung cấp ABC đã gửi báo giá mới',
    icon: 'mdi-truck-delivery',
    iconColor: 'white',
    bgColor: '#d1fae5',
    priority: 'low',
    createdAt: new Date(Date.now() - 1000 * 60 * 60 * 3) // 3 hours ago
  }
]);

const lowStockItems = ref([
  { id: '1', name: 'Paracetamol 500mg', quantity: 15 },
  { id: '2', name: 'Amoxicillin 250mg', quantity: 8 },
  { id: '3', name: 'Ibuprofen 400mg', quantity: 12 },
  { id: '4', name: 'Vitamin C', quantity: 5 },
  { id: '5', name: 'Aspirin 100mg', quantity: 7 }
]);

const expiringItems = ref([
  { id: '1', name: 'Cephalexin 500mg', daysToExpiry: 3 },
  { id: '2', name: 'Metformin 850mg', daysToExpiry: 7 },
  { id: '3', name: 'Losartan 50mg', daysToExpiry: 10 },
  { id: '4', name: 'Atorvastatin 20mg', daysToExpiry: 14 }
]);

const outOfStockItems = ref([
  { id: '1', name: 'Omeprazole 20mg' },
  { id: '2', name: 'Ciprofloxacin 500mg' },
  { id: '3', name: 'Prednisolone 5mg' }
]);

const salesChartOptions = ref({
  chart: {
    type: 'area',
    toolbar: { show: false },
    zoom: { enabled: false },
    offsetX: 0,
    offsetY: 0
  },
  colors: ['#11c393'],
  dataLabels: { enabled: false },
  stroke: { 
    curve: 'smooth', 
    width: 3 
  },
  fill: {
    type: 'gradient',
    gradient: {
      shade: 'light',
      type: 'vertical',
      shadeIntensity: 0.3,
      gradientToColors: ['#11c393'],
      opacityFrom: 0.6,
      opacityTo: 0.1
    }
  },
  grid: {
    show: true,
    borderColor: '#f1f5f9',
    strokeDashArray: 2,
    padding: {
      top: 0,
      right: 0,
      bottom: 0,
      left: 0
    }
  },
  xaxis: {
    categories: Array.from({ length: 30 }, (_, i) => `${i + 1}/8`),
    labels: {
      style: { 
        colors: '#64748b', 
        fontSize: '11px' 
      },
      rotate: -45,
      maxHeight: 60
    },
    axisBorder: { show: false },
    axisTicks: { show: false },
    tooltip: { enabled: false }
  },
  yaxis: {
    labels: {
      style: { 
        colors: '#64748b', 
        fontSize: '11px' 
      },
      formatter: (value: number) => `${(value / 1000000).toFixed(1)}M`
    }
  },
  tooltip: {
    theme: 'light',
    x: {
      format: 'dd/MM'
    },
    y: {
      formatter: (value: number) => formatCurrency(value)
    }
  },
  markers: {
    size: 0,
    hover: {
      size: 6
    }
  }
});

const salesChartSeries = ref([
  {
    name: 'Doanh thu',
    data: [
      8500000, 9200000, 7800000, 11200000, 12500000, 10800000, 13200000,
      11800000, 9500000, 14500000, 12200000, 10500000, 13800000, 12800000,
      9200000, 10800000, 11500000, 12200000, 13500000, 12800000, 11200000,
      14200000, 15800000, 16500000, 13800000, 14500000, 12200000, 13800000,
      15200000, 16800000
    ]
  }
]);

// Utility methods
const formatCurrency = (amount: number): string => {
  return new Intl.NumberFormat('vi-VN', {
    style: 'currency',
    currency: 'VND'
  }).format(amount);
};

const formatTime = (date: Date): string => {
  const now = new Date();
  const diff = now.getTime() - date.getTime();
  const minutes = Math.floor(diff / (1000 * 60));
  const hours = Math.floor(diff / (1000 * 60 * 60));
  const days = Math.floor(diff / (1000 * 60 * 60 * 24));

  if (minutes < 60) return `${minutes} phút trước`;
  if (hours < 24) return `${hours} giờ trước`;
  return `${days} ngày trước`;
};

const getOrderStatusColor = (status: string): string => {
  switch (status) {
    case 'completed': return 'success';
    case 'pending': return 'warning';
    case 'cancelled': return 'error';
    default: return 'grey';
  }
};

const getOrderStatusText = (status: string): string => {
  switch (status) {
    case 'completed': return 'Hoàn thành';
    case 'pending': return 'Chờ xử lý';
    case 'cancelled': return 'Đã hủy';
    default: return 'Không rõ';
  }
};
</script>

<style scoped>
.stat-card {
  @apply rounded-lg border border-gray-200 hover:shadow-lg transition-all duration-200;
  background: linear-gradient(135deg, #ffffff 0%, #f8fafc 100%);
}

.stat-card:hover {
  transform: translateY(-2px);
}

.dashboard-card {
  @apply rounded-lg border border-gray-200;
  background: white;
}

.card-title {
  @apply flex items-center justify-between px-6 py-4 border-b border-gray-100 font-semibold text-gray-800;
}

/* Custom scrollbar for notifications */
.overflow-y-auto::-webkit-scrollbar {
  width: 4px;
}

.overflow-y-auto::-webkit-scrollbar-track {
  @apply bg-gray-100 rounded;
}

.overflow-y-auto::-webkit-scrollbar-thumb {
  @apply bg-gray-300 rounded;
}

.overflow-y-auto::-webkit-scrollbar-thumb:hover {
  @apply bg-gray-400;
}

/* Responsive adjustments */
@media (max-width: 768px) {
  .grid {
    @apply gap-3;
  }
  
  .stat-card .p-4 {
    @apply p-3;
  }
  
  .card-title {
    @apply px-4 py-3 text-sm;
  }
}

/* Animation for dashboard elements */
.dashboard-card {
  animation: fadeInUp 0.6s ease-out;
}

@keyframes fadeInUp {
  from {
    opacity: 0;
    transform: translateY(20px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

.stat-card:nth-child(1) { animation-delay: 0.1s; }
.stat-card:nth-child(2) { animation-delay: 0.2s; }
.stat-card:nth-child(3) { animation-delay: 0.3s; }
.stat-card:nth-child(4) { animation-delay: 0.4s; }
</style>