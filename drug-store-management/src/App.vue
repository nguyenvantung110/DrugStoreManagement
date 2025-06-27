<template>
  <v-app>
    <LoadingComponent />
    <router-view />
    <CommonDialog
      v-model="dialog.isOpen.value"
      :message="dialog.dialogMessage.value"
      :type="dialogType"
      @confirm="dialog.closeDialog"
    />
  </v-app>
</template>
<script setup lang="ts">
  import LoadingComponent from "@/components/common/loading-component.vue";
  import { useDialog } from '@/composables/common/useDialog'
  import CommonDialog from '@/components/common/common-dialog.vue'
  import { computed } from "vue";

  const dialog = useDialog()
  const dialogType = computed(() => dialog.dialogType.value)
</script>
<style></style>
