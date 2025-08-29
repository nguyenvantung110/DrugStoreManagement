<template>
  <div class="payment-page p-6 bg-gray-50 h-full">
    <v-card class="mx-auto max-w-4xl p-6 rounded-lg shadow-md bg-white">
      <!-- Button Quay lại -->
      <v-btn
        color="gray"
        variant="text"
        class="mb-4 flex flex-row gap-2 items-center"
        @click="goBack"
      >
        <v-icon left>mdi-arrow-left</v-icon>
        Quay Lại Chỉnh Sửa
      </v-btn>

      <v-card-title class="text-2xl font-bold mb-4 pl-0">Thanh Toán Đơn Hàng</v-card-title>

      <!-- Thông tin khách hàng -->
      <div class="customer-info mb-6">
        <v-card-subtitle class="text-lg font-semibold mb-2 pl-0">Thông Tin Khách Hàng</v-card-subtitle>
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
            <td class="px-4 py-2">{{ item.productName }}</td>
            <td class="px-4 py-2">{{ item.quantity }}</td>
            <td class="px-4 py-2">{{ item.unitOfMeasure }}</td>
            <td class="px-4 py-2">{{ item.pricePerUnit }} VND</td>
            <td class="px-4 py-2 font-bold">{{ item.subTotal }} VND</td>
          </tr>
        </template>
      </v-data-table>

      <v-card-text class="py-4 pl-0">
        <p class="text-sm text-gray-700 leading-relaxed">
          <span class="text-base font-medium text-gray-800 leading-relaxed">Ghi chú: </span>
          {{ notes }}
        </p>
      </v-card-text>

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
import { useSalesStore } from '@/composables/sales/salesStore';
import type { DrugItem, TableHeader, OrderCreateDto } from '@/models/sales';
import { useDialog } from '@/composables/common/useDialog';
import { useAuthenticationStore } from '@/composables/auth/authenticationStore';

const dialog = useDialog()
const salesStore = useSalesStore();
const authStore = useAuthenticationStore();

const grandTotal = ref<number>(0);
const notes = ref<string>('');

// Khởi tạo ref cho drugList (dummy data)
const drugList = ref<DrugItem[]>([]);

// Gọi khi mounted
onMounted(() => {
  drugList.value = salesStore.getDrugListToSale
  grandTotal.value = salesStore.getGrandTotal;
  notes.value = salesStore.getNotes;
  //generateDummyData();
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
// const grandTotal = computed(() => {
//   return drugList.value.reduce((sum, item) => sum + item.subTotal, 0);
// });

// Form data cho thanh toán
const paymentMethod = ref<string>('');
const paymentOptions = ref<string[]>(['Tiền Mặt', 'Thẻ Tín Dụng', 'Ví Điện Tử (Momo)', 'Chuyển Khoản Ngân Hàng']);

// Thông tin khách hàng (mới thêm)
const customerName = ref<string>('');
const customerPhone = ref<string>('');

// Form ref cho validation (bây giờ bao gồm customer fields)
const paymentForm = ref();

const handlePayment = async (): Promise<void> => {
  try {
    // Validate form trước khi xử lý
    const { valid } = await paymentForm.value.validate()
    
    if (!valid) {
      await dialog.alert('Vui lòng kiểm tra lại thông tin nhập vào')
      return
    }

    // Validate dữ liệu cần thiết
    if (drugList.value.length === 0) {
      dialog.error('Không có sản phẩm để thanh toán')
      return
    }

    if (grandTotal.value <= 0) {
      dialog.error('Tổng tiền không hợp lệ')
      return
    }

    const userId = authStore.getterUserId
    
    // Chuẩn bị dữ liệu thanh toán theo interface
    const orderCreateDto: OrderCreateDto = {
      customer: {
        customerName: customerName.value || 'Khách lẻ',
        phoneNumber: customerPhone.value || ''
      },
      paymentMethod: paymentMethod.value,
      products: drugList.value.map(x => ({
        productId: x.productId,
        dosage: x.dosage,
        quantity: x.quantity,
        unitPrice: x.pricePerUnit,
        subTotal: x.subTotal
      })),
      grandTotal: grandTotal.value,
      notes: notes.value,
      userId: userId
    }
    await salesStore.createOrder(orderCreateDto).then(async () => {
      await dialog.success('Thanh toán thành công!!!').then(() => {
        //router.push('/sales')
        // Thêm CSS print styles động vào document
        addPrintStyles()
        
        let {products, ...dataForPrint} = orderCreateDto
        dataForPrint = {
          ...dataForPrint,
          items: drugList.value,
          invoiceNumber: 'HD' + new Date().getTime(),
        } as any
        // Thêm dữ liệu in vào DOM tạm thời
        const printContent = createPrintContent(dataForPrint)
        document.body.appendChild(printContent)

        // Trigger print dialog
        window.print()
        console.log('Print triggered')
        // Cleanup: Remove print content after printing
        setTimeout(() => {
          document.body.removeChild(printContent)
          removePrintStyles()
        }, 1000)
      })
    })
  } catch (error) {
    console.error('Payment processing error:', error)
    // dialog.error('Có lỗi xảy ra trong quá trình thanh toán. Vui lòng thử lại.')
  }
}

// Helper method để tạo print content
const createPrintContent = (data: any): HTMLElement => {
  const printDiv = document.createElement('div')
  printDiv.className = 'print-invoice'
  printDiv.innerHTML = `
    <div class="invoice-header">
      <h1>NHÀ THUỐC XANH</h1>
      <p>123 Đường ABC, Quận XYZ, TP.HCM</p>
      <p>Điện thoại: (028) 1234 5678</p>
      <hr>
      <h2>HÓA ĐƠN BÁN HÀNG</h2>
      <p>Số HĐ: ${data.invoiceNumber}</p>
      <p>Ngày: ${new Date(data.timestamp).toLocaleDateString('vi-VN')}</p>
    </div>
    
    <div class="customer-info">
      <h3>Thông tin khách hàng:</h3>
      <p><strong>Họ tên:</strong> ${data.customer.customerName}</p>
      <p><strong>Điện thoại:</strong> ${data.customer.phoneNumber}</p>
    </div>
    
    <table class="items-table">
      <thead>
        <tr>
          <th>STT</th>
          <th>Tên thuốc</th>
          <th>SL</th>
          <th>ĐVT</th>
          <th>Đơn giá</th>
          <th>Thành tiền</th>
        </tr>
      </thead>
      <tbody>
        ${data.items.map((item: DrugItem, index: number) => `
          <tr>
            <td>${index + 1}</td>
            <td>${item.productName}</td>
            <td>${item.quantity}</td>
            <td>${item.unitOfMeasure}</td>
            <td class="text-right">${formatCurrency(item.pricePerUnit)}</td>
            <td class="text-right">${formatCurrency(item.subTotal)}</td>
          </tr>
        `).join('')}
      </tbody>
    </table>
    
    ${data.notes ? `<div class="notes"><strong>Ghi chú:</strong> ${data.notes}</div>` : ''}
    
    <div class="total-section">
      <div class="total-row">
        <span>Tạm tính:</span>
        <span>${formatCurrency(grandTotal.value)}</span>
      </div>
      <div class="total-row">
        <span>VAT (0%):</span>
        <span>${formatCurrency(0)}</span>
      </div>
      <div class="total-row total-final">
        <span><strong>TỔNG CỘNG:</strong></span>
        <span><strong>${formatCurrency(data.grandTotal)}</strong></span>
      </div>
      <div class="total-row">
        <span>Phương thức TT:</span>
        <span>${data.paymentMethod}</span>
      </div>
    </div>
    
    <div class="signature-section">
      <div class="signature-left">
        <p>Khách hàng</p>
        <p class="signature-line">${data.customer.customerName}</p>
      </div>
      <div class="signature-right">
        <p>Nhân viên bán hàng</p>
        <p class="signature-line">${data.userId}</p>
      </div>
    </div>
    
    <div class="footer">
      <p>Cảm ơn quý khách đã mua hàng!</p>
      <p class="print-time">In lúc: ${new Date().toLocaleString('vi-VN')}</p>
    </div>
  `
  return printDiv
}

// Helper method để thêm print styles
const addPrintStyles = (): void => {
  const printStyles = document.createElement('style')
  printStyles.id = 'print-styles'
  printStyles.textContent = `
    @media print {
      body * { visibility: hidden; }
      .print-invoice, .print-invoice * { visibility: visible; }
      .print-invoice {
        position: absolute;
        left: 0;
        top: 0;
        width: 100%;
        padding: 20px;
        font-family: Arial, sans-serif;
        font-size: 14px;
        line-height: 1.4;
        color: black;
        background: white;
      }
      
      .invoice-header {
        text-align: center;
        margin-bottom: 20px;
        border-bottom: 2px solid #000;
        padding-bottom: 15px;
      }
      
      .invoice-header h1 {
        font-size: 24px;
        font-weight: bold;
        margin: 0 0 10px 0;
      }
      
      .invoice-header h2 {
        font-size: 18px;
        font-weight: bold;
        margin: 15px 0 10px 0;
      }
      
      .customer-info {
        margin: 20px 0;
        padding: 10px;
        border: 1px solid #ddd;
        background: #f9f9f9;
      }
      
      .items-table {
        width: 100%;
        border-collapse: collapse;
        margin: 20px 0;
      }
      
      .items-table th,
      .items-table td {
        border: 1px solid #333;
        padding: 8px;
        text-align: left;
      }
      
      .items-table th {
        background: #f0f0f0;
        font-weight: bold;
        text-align: center;
      }
      
      .items-table .text-right {
        text-align: right;
      }
      
      .notes {
        margin: 15px 0;
        padding: 10px;
        background: #f9f9f9;
        border-left: 4px solid #007bff;
      }
      
      .total-section {
        margin: 20px 0;
        border: 1px solid #333;
        padding: 15px;
      }
      
      .total-row {
        display: flex;
        justify-content: space-between;
        margin: 5px 0;
        padding: 3px 0;
      }
      
      .total-final {
        border-top: 2px solid #333;
        margin-top: 10px;
        padding-top: 8px;
        font-size: 16px;
      }
      
      .signature-section {
        margin: 30px 0 20px 0;
        display: flex;
        justify-content: space-between;
      }
      
      .signature-left, .signature-right {
        text-align: center;
        width: 40%;
      }
      
      .signature-line {
        margin-top: 50px;
        border-top: 1px solid #333;
        padding-top: 5px;
      }
      
      .footer {
        text-align: center;
        margin-top: 30px;
        border-top: 1px solid #ddd;
        padding-top: 15px;
      }
      
      .print-time {
        font-size: 12px;
        color: #666;
        margin-top: 10px;
      }
    }
  `
  document.head.appendChild(printStyles)
}

// Helper method để remove print styles
const removePrintStyles = (): void => {
  const printStyles = document.getElementById('print-styles')
  if (printStyles) {
    document.head.removeChild(printStyles)
  }
}

// Helper method để format currency
const formatCurrency = (amount: number): string => {
  return new Intl.NumberFormat('vi-VN', {
    style: 'currency',
    currency: 'VND'
  }).format(amount)
}

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