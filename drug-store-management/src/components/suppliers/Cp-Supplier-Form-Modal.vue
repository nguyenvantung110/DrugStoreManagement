<template>
    <v-dialog v-model="props.modelValue" max-width="600">
      <v-card>
        <v-card-title>
          {{ isEdit ? 'Chỉnh sửa thông tin nhà cung cấp' : 'Thêm nhà cung cấp' }}
        </v-card-title>
        <v-card-text>
          <v-form ref="formRef" v-model="valid" @submit.prevent="submit">
            <v-text-field
              v-model="localSupplier.supplier_Name"
              label="Tên nhà cung cấp"
              :rules="[v => !!v || 'Không được để trống']"
              required
            />
            <v-text-field
              v-model="localSupplier.contact_Person"
              label="Người liên hệ"
            />
            <v-text-field
              v-model="localSupplier.phone_Number"
              label="Số điện thoại"
            />
            <v-text-field
              v-model="localSupplier.email"
              label="Email"
              type="email"
            />
            <v-text-field
              v-model="localSupplier.address"
              label="Địa chỉ"
            />
          </v-form>
        </v-card-text>
        <v-card-actions>
          <v-spacer/>
          <v-btn color="grey" variant="text" @click="close">Hủy</v-btn>
          <v-btn color="primary" :disabled="!valid" @click="submit">
            {{ isEdit ? 'Lưu' : 'Thêm' }}
          </v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>
  </template>
  
  <script lang="ts" setup>
  import { ref, watch, computed } from 'vue'
  
  export interface SupplierDto {
    supplier_Id?: string
    supplier_Name: string
    contact_Person: string
    phone_Number: string
    email: string
    address: string
    created_At?: string | null
    updated_At?: string | null
  }
  
  const props = defineProps<{
    modelValue: boolean
    supplier?: SupplierDto | null
  }>()
  
  const emit = defineEmits<{
    (e: 'update:modelValue', v: boolean): void
    (e: 'submit', v: SupplierDto): void
  }>()
  
  const formRef = ref()
  const valid = ref(false)
  
  const isEdit = computed(() => !!props.supplier?.supplier_Id)
  
  const emptySupplier: SupplierDto = {
    supplier_Name: '',
    contact_Person: '',
    phone_Number: '',
    email: '',
    address: ''
  }
  const localSupplier = ref<SupplierDto>({ ...emptySupplier })
  
  watch(() => props.modelValue, (show) => {
    if (show) {
      localSupplier.value = props.supplier
        ? { ...props.supplier }
        : { ...emptySupplier }
    }
  })
  
  function close() {
    emit('update:modelValue', false)
  }
  function submit() {
    if (!formRef.value?.validate?.()) return
    emit('submit', { ...localSupplier.value })
    close()
  }
  </script>