<template>
  <v-container fluid class="homepage-bg d-flex align-center justify-center fill-height pa-0">
    <div class="homepage-overlay"></div>
    <v-btn @click="showSuccess" style="z-index: 100;">showSuccess</v-btn>
  <v-btn @click="showError" style="z-index: 100;">showError</v-btn>
  <v-btn @click="showConfirm" style="z-index: 100;">showConfirm</v-btn>
  <v-btn @click="showAlert" style="z-index: 100;">showAlert</v-btn>
    <v-row align="center" justify="center" class="ma-0 pa-0" style="z-index:2; width:100%;">
      <v-col cols="12" md="10" lg="9" xl="8">
        <v-card class="homepage-card pa-8" elevation="15" v-motion v-motion-fade v-motion-slide-bottom>
          <v-row class="mb-3" dense>
            <!-- Branding Card -->
            <v-col cols="12" md="6">
              <v-card class="mini-card pa-6 d-flex flex-column align-center justify-center" elevation="4">
                <v-avatar size="70" class="mb-4" color="primary" style="box-shadow: 0 4px 18px rgba(77,182,172,0.18)">
                  <v-icon size="48" color="white">mdi-pharmacy</v-icon>
                </v-avatar>
                <div class="text-h5 font-weight-bold mb-2 text-primary-gradient">Nhà thuốc Xanh+</div>
                <div class="text-subtitle-1 mb-4 text-grey-darken-2 text-center">
                  Hệ thống quản lý nhà thuốc hiện đại, <span class="text-primary">an toàn</span> và <span class="text-teal">thông minh</span>
                </div>
                <v-chip class="mb-2" color="teal" size="small" label>Phiên bản 2.5.0</v-chip>
                <div class="text-caption text-grey-darken-1 mt-1">Cập nhật lần cuối: 20/06/2025</div>
              </v-card>
            </v-col>
            <!-- Chart Card -->
            <v-col cols="12" md="6">
              <v-card class="mini-card pa-6" elevation="4">
                <div class="d-flex align-center mb-2">
                  <v-icon color="success" class="mr-2">mdi-chart-line</v-icon>
                  <span class="text-h6 font-weight-bold">Tăng trưởng đơn hàng</span>
                </div>
                <apexchart
                  type="area"
                  height="130"
                  :options="chartOptions"
                  :series="chartSeries"
                ></apexchart>
                <div class="text-caption text-right mt-1" style="color:#888">
                  * Đơn hàng 30 ngày gần nhất
                </div>
              </v-card>
            </v-col>
          </v-row>
          <v-row dense>
            <!-- Notification Card -->
            <v-col cols="12" md="6">
              <v-card class="mini-card pa-6" elevation="4">
                <div class="d-flex align-center mb-2">
                  <v-icon color="deep-orange" class="mr-2">mdi-bell-alert</v-icon>
                  <span class="text-h6 font-weight-bold">Thông báo</span>
                </div>
                <v-list density="compact" class="pl-0">
                  <v-list-item v-for="(note, i) in notifications" :key="i" class="pa-0">
                    <v-list-item-content>
                      <v-list-item-title>
                        <v-icon :color="note.color" size="16" class="mr-1">{{ note.icon }}</v-icon>
                        {{ note.text }}
                      </v-list-item-title>
                    </v-list-item-content>
                  </v-list-item>
                </v-list>
              </v-card>
            </v-col>
            <!-- Quick Stats Card -->
            <v-col cols="12" md="6">
              <v-card class="mini-card pa-6" elevation="4">
                <div class="d-flex align-center mb-3">
                  <v-icon color="info" class="mr-2">mdi-information</v-icon>
                  <span class="text-h6 font-weight-bold">Thống kê nhanh</span>
                </div>
                <v-row dense>
                  <v-col cols="6" v-for="info in quickStats" :key="info.title">
                    <v-card
                      :color="info.color"
                      theme="dark"
                      elevation="8"
                      class="quick-info-card d-flex flex-column align-center justify-center py-4 px-2"
                    >
                      <v-icon size="28" class="mb-1">{{ info.icon }}</v-icon>
                      <span class="text-h6 font-weight-bold">{{ info.value }}</span>
                      <span class="text-caption">{{ info.title }}</span>
                    </v-card>
                  </v-col>
                </v-row>
              </v-card>
            </v-col>
          </v-row>
        </v-card>
      </v-col>
    </v-row>
  </v-container>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { useDialog } from '@/composables/common/useDialog'

const dialog = useDialog()


function showSuccess() {
  dialog.success('Lưu thành công!')
}
function showError() {
  dialog.error('Có lỗi!')
}

async function showAlert() {
  await dialog.alert('Alert!')
  console.log('Alert confirmed!')
}

async function showConfirm() {
  const ok = await dialog.confirm('Bạn có chắc chắn muốn xóa không?')
  if (ok) {
    console.log('Xóa thành công!', ok)
  }
  else {
    console.log('Hủy thao tác!', ok)
  }
}

const quickStats = [
  { icon: 'mdi-package-variant-closed', value: 120, title: 'Loại thuốc', color: 'primary' },
  { icon: 'mdi-cart', value: 48, title: 'Đơn hàng hôm nay', color: 'teal' },
  { icon: 'mdi-account-group', value: 230, title: 'Khách hàng', color: 'amber darken-2' },
  { icon: 'mdi-truck-delivery', value: 7, title: 'Nhà cung cấp', color: 'deep-orange' }
]

const notifications = [
  { icon: 'mdi-truck-fast', color: 'teal', text: 'Hôm nay có 2 đơn nhập chờ xác nhận.' },
  { icon: 'mdi-alert-circle', color: 'red', text: 'Có 3 loại thuốc sắp hết hạn.' },
  { icon: 'mdi-cart-arrow-down', color: 'blue', text: '5 đơn hàng mới đã tạo.' },
  { icon: 'mdi-message-text', color: 'amber', text: 'Đừng quên cập nhật thông tin nhà thuốc.' }
]

const chartOptions = ref({
  chart: {
    toolbar: { show: false },
    sparkline: { enabled: true }
  },
  colors: ['#4db6ac'],
  xaxis: {
    categories: [
      'T1', 'T2', 'T3', 'T4', 'T5', 'T6', 'T7', 'T8', 'T9', 'T10',
      'T11', 'T12', 'T13', 'T14', 'T15', 'T16', 'T17', 'T18', 'T19', 'T20',
      'T21', 'T22', 'T23', 'T24', 'T25', 'T26', 'T27', 'T28', 'T29', 'T30'
    ],
    labels: { style: { fontSize: '12px', colors: '#666' } }
  },
  grid: { show: false },
  stroke: { curve: 'smooth', width: 3 },
  fill: { type: 'gradient', gradient: { shade: 'light', type: "vertical", opacityFrom: 0.45, opacityTo: 0.03 } },
  dataLabels: { enabled: false },
  tooltip: { enabled: true }
})
const chartSeries = ref([
  { name: 'Đơn hàng', data: [12, 18, 15, 22, 25, 21, 24, 23, 18, 29, 24, 20, 25, 23, 17, 19, 21, 22, 24, 23, 20, 25, 28, 29, 24, 25, 21, 23, 27, 30] }
])
</script>

<style scoped>
.homepage-bg {
  min-height: 100vh;
  background: url('https://images.unsplash.com/photo-1505751172876-fa1923c5c528?auto=format&fit=crop&w=1400&q=80') center center/cover no-repeat;
  position: relative;
}
.homepage-overlay {
  position: absolute;
  z-index: 1;
  inset: 0;
  background: linear-gradient(120deg, rgba(77, 182, 172, 0.55), rgba(0,0,0,0.17));
}
.homepage-card {
  position: relative;
  z-index: 2;
  border-radius: 22px;
  background: rgba(255,255,255,0.97);
  box-shadow: 0 8px 32px rgba(40, 40, 130, 0.16);
  transition: box-shadow 0.2s, transform 0.2s;
  animation: homepage-popin 0.85s cubic-bezier(.23,1.11,.61,.94);
  overflow: hidden;
}
@keyframes homepage-popin {
  from {
    transform: scale(.94) translateY(36px);
    opacity: 0;
  }
  to {
    transform: scale(1) translateY(0);
    opacity: 1;
  }
}
.mini-card {
  border-radius: 16px;
  background: rgba(255,255,255,0.99);
  box-shadow: 0 2px 10px rgba(77, 182, 172, 0.09);
  height: 100%;
  display: flex;
  flex-direction: column;
  justify-content: center;
}
.homepage-welcome {
  background: linear-gradient(120deg,rgba(77,182,172,0.07),rgba(255,255,255,0));
  min-height: 230px;
}
.text-primary-gradient {
  background: linear-gradient(90deg, #009688 30%, #4db6ac 90%);
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
}
.quick-info-card {
  border-radius: 12px;
  min-width: 100px;
  min-height: 60px;
  box-shadow: 0 2px 10px rgba(77, 182, 172, 0.10);
  transition: transform .15s, box-shadow .15s;
}
.quick-info-card:hover {
  transform: translateY(-3px) scale(1.045);
  box-shadow: 0 6px 18px rgba(77, 182, 172, 0.17);
}
</style>