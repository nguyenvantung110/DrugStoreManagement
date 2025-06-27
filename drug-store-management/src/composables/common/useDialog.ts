import { ref } from 'vue'

type DialogType = 'success' | 'error' | 'info' | 'warning' | 'confirm'

const isOpen = ref(false)
const dialogType = ref<DialogType>('info')
const dialogMessage = ref('')
let resolver: ((value: boolean) => void) | null = null

function openDialog(type: DialogType, message: string): Promise<boolean> {
  dialogType.value = type
  dialogMessage.value = message
  isOpen.value = true
  return new Promise(resolve => {
    resolver = resolve
  })
}

function confirm(message: string) {
  return openDialog('confirm', message)
}
function alert(message: string) {
  return openDialog('info', message)
}
function success(message: string) {
  return openDialog('success', message)
}
function error(message: string) {
  return openDialog('error', message)
}
function closeDialog(result: boolean = true) {
  isOpen.value = false
  if (resolver) {
    resolver(result)
    resolver = null
  }
}

export function useDialog() {
  return {
    isOpen,
    dialogType,
    dialogMessage,
    confirm,
    alert,
    success,
    error,
    closeDialog,
  }
}