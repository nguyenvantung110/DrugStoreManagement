<template>
    
    <v-container class="border-sm rounded-lg">
      <v-row justify="space-around">
        <v-date-picker elevation="24" v-model="selectedDate" :hide-header=true @update:model-value=update></v-date-picker>
      </v-row>
    </v-container>
  </template>
<script  setup lang="ts">
import { ref } from 'vue';
const selectedDate = ref();
const emit = defineEmits<{
    (e: 'update', value: Date): void;
}>();
const update = () => {
  emit('update', selectedDate.value);
};
</script>
<style scoped></style>