<script setup lang="ts">
import { ref, watch, defineProps, defineEmits } from 'vue'

const props = defineProps<{
  modelValue: boolean
  message: string
  type?: 'success' | 'error' | 'info' | 'warning' | 'confirm'
}>()
const emit = defineEmits(['confirm'])

const dialog = ref(props.modelValue)
watch(() => props.modelValue, val => (dialog.value = val))

const iconMap = {
  success: 'mdi-check-circle',
  error: 'mdi-alert-circle',
  info: 'mdi-information',
  warning: 'mdi-alert',
  confirm: 'mdi-help-circle'
}
const colorMap = {
  success: 'success',
  error: 'error',
  info: 'info',
  warning: 'warning',
  confirm: 'primary'
}

function emitConfirm(result: boolean) {
  dialog.value = false
  emit('confirm', result)
}

function emitClose() {
  dialog.value = false
  emit('confirm', true)
}
</script>

<template>
  <v-dialog v-model="dialog" max-width="350">
    <v-card :color="colorMap[props.type || 'info']" dark>
      <v-card-title class="d-flex align-center">
        <v-icon class="mr-2" size="32">{{ iconMap[props.type || 'info'] }}</v-icon>
        <span>{{ props.type === 'success' ? 'Thành công' : props.type === 'error' ? 'Thất bại' : 'Thông báo' }}</span>
      </v-card-title>
      <v-card-text>{{ props.message }}</v-card-text>
      <v-card-actions>
        <v-spacer />
        <v-btn v-if="props.type !== 'confirm'" color="white" text @click="emitClose">Đóng</v-btn>
        <v-btn v-if="props.type === 'confirm'" text @click="emitConfirm(true)">Xác nhận</v-btn>
        <v-btn v-if="props.type === 'confirm'" text @click="emitConfirm(false)">Huỷ</v-btn>
      </v-card-actions>
    </v-card>
  </v-dialog>
</template>