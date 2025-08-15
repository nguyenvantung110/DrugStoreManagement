<template>
  <div class="payment-page p-6 bg-gray-50 min-h-screen">
    <v-card class="mx-auto max-w-4xl p-6 rounded-lg shadow-md bg-white">
      <!-- Button Quay lại -->
      <v-btn
        color="gray"
        variant="text"
        class="mb-4"
        @click="goBack"
      >
        <v-icon left>mdi-arrow-left</v-icon>
        Quay Lại Chỉnh Sửa
      </v-btn>

      <v-card-title class="text-2xl font-bold mb-4">Thanh Toán Đơn Hàng</v-card-title>

      <!-- Thông tin khách hàng -->
      <div class="customer-info mb-6">
        <v-card-subtitle class="text-lg font-semibold mb-2">Thông Tin Khách Hàng</v-card-subtitle>
        <v-row>
          <v-col cols="12" md="6">
            <v-text-field
              v-model="customerName"
              label="Tên Khách Hàng"
              density="compact"
              variant="outlined"
              :rules="[v => !!v || 'Vui lòng nhập tên khách hàng']"
              hide-details="auto"
            />
          </v-col>
          <v-col cols="12" md="6">
            <v-text-field
              v-model="customerPhone"
              label="Số Điện Thoại"
              density="compact"
              variant="outlined"
              :rules="[v => !!v || 'Vui lòng nhập số điện thoại', v => /^\d{10,11}$/.test(v) || 'Số điện thoại không hợp lệ']"
              hide-details="auto"
            />
          </v-col>
        </v-row>
      </div>

      <!-- Danh sách items (table) -->
      <v-data-table
        :headers="headers"
        :items="drugList"
        :items-per-page="-1"
        class="mb-6 elevation-1"
        hide-default-footer
      >
        <template v-slot:item="{ item }">
          <tr class="hover:bg-gray-100 transition-colors">
            <td class="px-4 py-2">{{ item.name }}</td>
            <td class="px-4 py-2">{{ item.quantity }}</td>
            <td class="px-4 py-2">{{ item.unit }}</td>
            <td class="px-4 py-2">{{ item.unitPrice }} VND</td>
            <td class="px-4 py-2 font-bold">{{ item.subTotal }} VND</td>
          </tr>
        </template>
      </v-data-table>

      <!-- Tổng tiền -->
      <div class="text-right mb-6 text-lg font-bold">
        Tổng Tiền: {{ grandTotal }} VND
      </div>

      <!-- Form thanh toán -->
      <v-form ref="paymentForm" @submit.prevent="handlePayment">
        <v-select
          v-model="paymentMethod"
          :items="paymentOptions"
          label="Phương Thức Thanh Toán"
          density="compact"
          variant="outlined"
          :rules="[v => !!v || 'Vui lòng chọn phương thức thanh toán']"
          class="mb-4"
        />

        <v-btn
          type="submit"
          color="#11c393"
          class="w-full py-3 text-white font-bold rounded-md hover:bg-blue-700 transition-colors"
        >
          Thanh Toán Ngay
        </v-btn>
      </v-form>
    </v-card>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, computed } from 'vue';
import { useRouter } from 'vue-router';

// Định nghĩa type cho Item (TypeScript)
interface Item {
  name: string;
  quantity: number;
  unit: string;
  unitPrice: number;
  subTotal: number;  // Tính bằng quantity * unitPrice
}

// Khởi tạo ref cho drugList (dummy data)
const drugList = ref<Item[]>([]);

// Tạo dummy data (tính subTotal tự động)
const generateDummyData = () => {
  drugList.value = Array.from({ length: 5 }, (_, i) => {
    const quantity = Math.floor(Math.random() * 100) + 1;
    const unitPrice = 2000;
    const subTotal = quantity * unitPrice;  // Tính tự động

    return {
      name: `Tên thuốc ${i + 1}`,
      quantity,
      unit: 'viên',
      unitPrice,
      subTotal,
    };
  });
};

// Gọi khi mounted
onMounted(() => {
  generateDummyData();
});

// Headers cho table (Vuetify)
const headers = [
  { title: 'Tên Thuốc', key: 'name' },
  { title: 'Số Lượng', key: 'quantity' },
  { title: 'Đơn Vị', key: 'unit' },
  { title: 'Đơn Giá', key: 'price' },
  { title: 'Thành Tiền', key: 'subTotal' },
];

// Tính Grand Total (computed)
const grandTotal = computed(() => {
  return drugList.value.reduce((sum, item) => sum + item.subTotal, 0);
});

// Form data cho thanh toán
const paymentMethod = ref<string>('');
const paymentOptions = ref<string[]>(['Tiền Mặt', 'Thẻ Tín Dụng', 'Ví Điện Tử (Momo)', 'Chuyển Khoản Ngân Hàng']);

// Thông tin khách hàng (mới thêm)
const customerName = ref<string>('');
const customerPhone = ref<string>('');

// Form ref cho validation (bây giờ bao gồm customer fields)
const paymentForm = ref();

// Hàm xử lý thanh toán (dummy, bạn có thể thay bằng API)
const handlePayment = async () => {
  const { valid } = await paymentForm.value.validate();
  if (valid) {
    alert(`Thanh toán thành công với phương thức: ${paymentMethod.value}. Khách hàng: ${customerName.value} (${customerPhone.value}). Tổng tiền: ${grandTotal.value} VND`);
    // Ở đây gọi API thực tế, ví dụ: await api.post('/payment', { method: paymentMethod.value, total: grandTotal.value, customer: { name: customerName.value, phone: customerPhone.value } });
  }
};

// Hàm quay lại (mới thêm, dùng Vue Router)
const router = useRouter();
const goBack = () => {
  router.back();  // Quay lại trang trước (hoặc thay bằng router.push('/edit-drugs') nếu có route cụ thể)
};
</script>

<style scoped>
/* Tailwind CSS tùy chỉnh (kết hợp với Vuetify) */
.payment-page {
  @apply flex items-center justify-center;  /* Center nội dung */
}
</style>