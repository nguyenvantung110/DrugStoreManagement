<template>
    <div class="import-page-root">
      <v-card class="import-card" elevation="8">
        <v-form ref="importForm" @submit.prevent="handleSave">
          <!-- Thông tin phiếu nhập -->
          <v-row class="mb-6">
            <v-col cols="12" md="4">
              <v-select
                v-model="form.supplier"
                :items="supplierOptions"
                label="Nhà cung cấp"
                prepend-inner-icon="mdi-truck"
                required
                :rules="[v => !!v || 'Bắt buộc chọn']"
                dense
                outlined
              />
            </v-col>
            <v-col cols="12" md="4">
              <v-text-field
                v-model="form.invoiceNumber"
                label="Số hóa đơn"
                prepend-inner-icon="mdi-file-document-outline"
                dense
                outlined
              />
            </v-col>
            <v-col cols="12" md="4">
              <v-text-field
                v-model="form.date"
                label="Ngày nhập"
                prepend-inner-icon="mdi-calendar"
                type="date"
                required
                :rules="[v => !!v || 'Bắt buộc nhập']"
                dense
                outlined
              />
            </v-col>
          </v-row>
  
          <!-- Danh sách thuốc nhập (scrollable table if needed) -->
          <div
            class="medicine-table-container mb-6"
            :class="{ 'scroll-enabled': isTableScroll }"
          >
            <div class="d-flex align-center mb-3">
              <span class="font-weight-medium text-subtitle-1 mr-auto">Danh sách thuốc nhập</span>
              <v-btn color="success" variant="tonal" size="small" @click="addMedicine">
                <v-icon left size="18">mdi-plus</v-icon>Thêm thuốc
              </v-btn>
            </div>
            <div
              class="medicine-table-scroll"
              :style="isTableScroll ? 'flex-grow: 1;overflow-y:auto;' : ''"
              ref="tableScrollRef"
            >
              <v-table density="comfortable" class="medicine-table">
                <thead>
                  <tr>
                    <th>Tên thuốc</th>
                    <th>Đơn vị</th>
                    <th>Số lượng</th>
                    <th>Giá nhập (VNĐ)</th>
                    <th>Hạn dùng</th>
                    <th></th>
                  </tr>
                </thead>
                <tbody>
                  <tr
                    v-for="(item, idx) in form.medicines"
                    :key="idx"
                  >
                    <td class="table-cell-fixed">
                      <v-autocomplete
                        v-model="item.name"
                        :items="medicineOptions"
                        label="Tên thuốc"
                        dense
                        hide-details
                        required
                        :rules="[v => !!v || 'Bắt buộc']"
                        placeholder="Nhập tên..."
                        outlined
                        class="no-flex-grow"
                        :menu-props="{ contentClass: 'autocomplete-menu-fixed' }"
                      />
                    </td>
                    <td class="table-cell-fixed">
                      <v-select
                        v-model="item.unit"
                        :items="unitOptions"
                        label="Đơn vị"
                        dense
                        hide-details
                        outlined
                        placeholder="Chọn đơn vị"
                        class="no-flex-grow"
                        :menu-props="{ maxHeight: 200 }"
                      />
                    </td>
                    <td class="table-cell-fixed">
                      <v-text-field
                        v-model.number="item.quantity"
                        type="number"
                        min="1"
                        dense
                        hide-details
                        label="SL"
                        placeholder="0"
                        outlined
                        class="no-flex-grow"
                      />
                    </td>
                    <td class="table-cell-fixed">
                      <v-text-field
                        v-model.number="item.price"
                        type="number"
                        min="0"
                        dense
                        hide-details
                        label="Giá"
                        placeholder="0"
                        outlined
                        class="no-flex-grow"
                      />
                    </td>
                    <td class="table-cell-fixed">
                      <v-text-field
                        v-model="item.expiry"
                        type="date"
                        dense
                        hide-details
                        label="Hạn dùng"
                        outlined
                        class="no-flex-grow"
                      />
                    </td>
                    <td class="table-cell-fixed">
                      <v-icon small color="red" @click="removeMedicine(idx)">
                        mdi-delete
                      </v-icon>
                    </td>
                  </tr>
                  <tr v-if="!form.medicines.length">
                    <td colspan="3" class="text-center text-grey">Chưa có thuốc nào</td>
                  </tr>
                </tbody>
              </v-table>
            </div>
          </div>
  
          <!-- Tổng cộng và ghi chú -->
          <v-row class="mb-2">
            <v-col cols="12" md="6">
              <v-textarea
                v-model="form.note"
                label="Ghi chú"
                auto-grow
                rows="2"
                prepend-inner-icon="mdi-note-outline"
                dense
                outlined
              />
            </v-col>
            <v-col cols="12" md="6" class="d-flex flex-column align-end pr-md-8">
              <div class="d-flex align-center mb-1 mt-3">
                <span class="font-weight-medium text-subtitle-1 mr-2">Tổng tiền:</span>
                <span class="text-h6 text-primary">{{ totalPrice.toLocaleString() }} VNĐ</span>
              </div>
            </v-col>
          </v-row>
  
          <v-divider class="my-3" />
  
          <div class="d-flex justify-end mt-2">
            <v-btn color="primary" @click="handleSave" :loading="saving" size="large" elevation="2">
              <v-icon left>mdi-content-save</v-icon>
              Lưu phiếu nhập
            </v-btn>
          </div>
        </v-form>
      </v-card>
    </div>
  </template>
  
  <script setup>
  import { ref, computed, onMounted, onBeforeUnmount, watch } from 'vue'
  import { useSupplierStore } from '@/composables/suppliers/supplierStore'

  const supplierStore = useSupplierStore()
  const saving = ref(false)
  const importForm = ref()
  const tableScrollRef = ref(null)
  const isTableScroll = ref(false)
  
  const supplierOptions = ref()
  const medicineOptions = [
    'Paracetamol 500mg',
    'Amoxicillin 500mg',
    'Vitamin C 500mg',
    'Cephalexin 500mg',
    'Loperamid 2mg',
    'Ibuprofen 400mg',
    'Azithromycin 250mg',
  ]
  const unitOptions = [
    'Viên',
    'Hộp',
    'Lọ',
    'Tuýp',
    'Chai',
    'Ống',
    'Gói',
  ]
  
  const form = ref({
    supplier: '',
    invoiceNumber: '',
    date: new Date().toISOString().slice(0, 10),
    medicines: [],
    note: '',
  })
  
  onMounted(async () => {
    await supplierStore.getSupplierList((res) => {
      supplierOptions.value = res?.data.map(x => x.supplier_Name) || [];
    })
  })

  function addMedicine() {
    form.value.medicines.push({
      name: '',
      unit: '',
      quantity: 1,
      price: 0,
      expiry: '',
    })
  }
  
  function removeMedicine(idx) {
    form.value.medicines.splice(idx, 1)
  }
  
  const totalPrice = computed(() =>
    form.value.medicines.reduce((sum, m) => sum + (Number(m.price) || 0) * (Number(m.quantity) || 0), 0)
  )
  
  function checkTableScroll() {
    const screenHeight = window.innerHeight
    const tableContainer = tableScrollRef.value
    if (!tableContainer) {
      isTableScroll.value = false
      return
    }
    const tableElement = tableContainer.querySelector('table')
    if (!tableElement) {
      isTableScroll.value = false
      return
    }
    const tableHeight = tableElement.getBoundingClientRect().height
    const otherHeight = 450 // adjust if needed
    const spaceForTable = screenHeight - otherHeight
    isTableScroll.value = tableHeight > spaceForTable
  }
  
  onMounted(() => {
    window.addEventListener('resize', checkTableScroll)
    setTimeout(checkTableScroll, 150)
  })
  onBeforeUnmount(() => {
    window.removeEventListener('resize', checkTableScroll)
  })
  watch(
    () => form.value.medicines.length,
    () => setTimeout(checkTableScroll, 120)
  )
  
  async function handleSave() {
    if (!importForm.value?.validate()) return
    saving.value = true
    setTimeout(() => {
      saving.value = false
      alert('Lưu phiếu nhập thành công!')
      // Reset nếu cần
      // form.value = {...}
    }, 1200)
  }
  </script>
  
  <style scoped>
  .import-page-root {
    min-height: 100vh;
    background: linear-gradient(135deg, #e3f0ff 0%, #f8fdff 100%);
    padding: 0;
    margin: 0;
  }
  .import-card {
    width: 100vw;
    min-height: 100vh;
    border-radius: 0;
    margin: 0;
    padding: 36px 32px 28px 32px;
    background: #fff;
    box-shadow: 0 8px 32px 0 rgba(31,38,135,0.12);
  }
  .medicine-table-container {
    width: 100%;
    max-width: 100vw;
  }
  .medicine-table-scroll {
    border-radius: 10px;
    border: 1px solid #e3e3ec;
    background: #f9fafd;
    box-shadow: 0 1px 6px 0 rgba(31,38,135,0.05);
    overflow-y: visible;
    max-height: unset;
    transition: max-height 0.2s;
  }
  .medicine-table-container.scroll-enabled .medicine-table-scroll {
    max-height: 340px;
    overflow-y: auto;
  }
  .medicine-table {
    background: transparent;
  }
  .medicine-table tr {
    height: 64px;
  }
  .table-cell-fixed {
    min-width: 120px;
    max-width: 220px;
    width: 180px;
    vertical-align: middle;
  }
  .no-flex-grow {
    flex: none !important;
    min-width: 0 !important;
    width: 100% !important;
  }
  .autocomplete-menu-fixed {
    min-width: 220px !important;
    max-width: 320px !important;
  }
  @media (max-width: 900px) {
    .import-card { padding: 16px 2vw 10px 2vw; }
    .medicine-table-scroll { max-height: 230px; }
    .medicine-table-container.scroll-enabled .medicine-table-scroll { max-height: 230px; }
  }
  </style>