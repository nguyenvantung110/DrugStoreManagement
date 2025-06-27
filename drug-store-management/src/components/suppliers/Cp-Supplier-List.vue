<template>
    <v-card class="pl-12 pr-12 h-100 d-flex flex-column ga-3">
      <v-card-title class="d-flex flex-column ga-3">
        Danh sách nhà cung cấp
        <v-spacer />
        <div class="d-flex flex-row justify-space-between align-center">
            <v-text-field
                v-model="search"
                label="Tìm kiếm"
                prepend-inner-icon="mdi-magnify"
                dense
                hide-details
                density="comfortable"
                class="search-field"
            />
            <v-btn color="primary" @click="openAdd">Thêm mới</v-btn>
        </div>
      </v-card-title>
      <v-card-text class="flex-grow-1 d-flex flex-column">
        <v-data-table
            :headers="headers"
            :items="filteredSuppliers"
            :items-per-page="10"
            :search="search"
            :footer-props="{
            'items-per-page-options': [5, 10, 25, 50],
            'items-per-page-text': 'Số dòng/trang'
            }"
            class="elevation-1 flex-grow-1"
        >
            <template #item.created_At="{ item }">
            {{ formatDate(item.created_At) }}
            </template>
            <template #item.updated_At="{ item }">
            {{ formatDate(item.updated_At) }}
            </template>
            <template #item.actions="{ item }">
            <v-btn icon size="small" @click="openEdit(item)">
                <v-icon>mdi-pencil</v-icon>
            </v-btn>
            <v-btn icon size="small" color="red" @click="handleDelete(item)">
                <v-icon>mdi-delete</v-icon>
            </v-btn>
            </template>
        </v-data-table>
      </v-card-text>
      <SupplierFormModal
        v-model="showForm"
        :supplier="selectedSupplier"
        @submit="handleSubmit"
      />
    </v-card>
  </template>
  
  <script setup lang="ts">
  import { ref, computed, onMounted } from 'vue'
  import SupplierFormModal from './Cp-Supplier-Form-Modal.vue'
  import { useSupplierStore } from '@/composables/suppliers/supplierStore'
  import { useDialog } from '@/composables/common/useDialog'

  const dialog = useDialog()

  interface SupplierDto {
    supplier_Id: string
    supplier_Name: string
    contact_Person: string
    phone_Number: string
    email: string
    address: string
    created_At?: string | null
    updated_At?: string | null
  }
  
  const suppliers = ref<SupplierDto[]>([])
  
  const supplierStore = useSupplierStore()
  const search = ref('')
  const page = ref(1)
  const showForm = ref(false)
  const showDelete = ref(false)
  const selectedSupplier = ref<SupplierDto | null>(null)
  
  const headers = [
  { title: 'Tên nhà cung cấp', value: 'supplier_Name' },
  { title: 'Người liên hệ', value: 'contact_Person' },
  { title: 'Số điện thoại', value: 'phone_Number' },
  { title: 'Email', value: 'email' },
  { title: 'Địa chỉ', value: 'address' },
  { title: 'Ngày tạo', value: 'created_At' },
  { title: 'Ngày cập nhật', value: 'updated_At' },
  { title: '', value: 'actions', sortable: false }
]
  onMounted(async () => {
    await getSupplierList();
  })
  
  const getSupplierList = async () => {
    // Load suppliers from store or API
    supplierStore.getSupplierList((res: any) => {
      suppliers.value = res?.data;
    })
  }

  const filteredSuppliers = computed(() => {
    if (!search.value) return suppliers.value
    const s = search.value.toLowerCase()
    return suppliers.value.filter(sup =>
      sup.supplier_Name.toLowerCase().includes(s) ||
      sup.contact_Person.toLowerCase().includes(s) ||
      sup.phone_Number.includes(s) ||
      sup.email.toLowerCase().includes(s)
    )
  })
  
  function formatDate(dateStr?: string | null) {
    if (!dateStr) return ''
    const d = new Date(dateStr)
    return d.toLocaleString()
  }

  // Action handlers
function openAdd() {
  selectedSupplier.value = null
  showForm.value = true
}
function openEdit(item: SupplierDto) {
  selectedSupplier.value = { ...item }
  showForm.value = true
}
function openDelete(item: SupplierDto) {
  selectedSupplier.value = item
  showDelete.value = true
}
async function handleSubmit(newSupplier: SupplierDto) {
  if (newSupplier.supplier_Id) {
    // Edit
    const idx = suppliers.value.findIndex(s => s.supplier_Id === newSupplier.supplier_Id)
    if (idx !== -1) {
      suppliers.value[idx] = {
        ...newSupplier,
        updated_At: new Date().toISOString()
      }
    }

    await supplierStore.updateSupplier(newSupplier);
  } else {
    // Add
    await supplierStore.createSupplier(newSupplier);
  }

  await getSupplierList();
}
async function handleDelete(supplier: any) {
    const confirmed = await dialog.confirm(`Bạn có chắc chắn muốn xoá nhà cung cấp "${supplier?.supplier_Name}"?`);
    
    if(confirmed) {
      await supplierStore.deleteSupplier(supplier?.supplier_Id);
      await getSupplierList();
    }
}
  </script>
<style scoped>
.search-field {
    max-width: 20rem;
}
</style>