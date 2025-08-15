import { defineStore } from 'pinia';
import { ref, computed } from 'vue';
import type { DrugItem, SalesTransaction } from '@/models/sales';
import { useApi } from '@/composables/common/api-instance';
import { END_POINTS } from '@/endpoints/api-endpoints';

export const usePrescriptionStore = defineStore('prescription-store', () => {
  // State
  const prescriptionTempalte = ref<any[]>([]);

  // Getters
  const getPrescriptionTemplates = computed(() => prescriptionTempalte.value);
  
  // Actions

  const fetchPrescriptionTemplates = async (categoryId: string |  null = null): Promise<void> => {
    try {
      const { get } = useApi();
      const params: Record<string, any> = {};
      if (categoryId) params.categoryId = categoryId;
      console.log("Fetching prescription templates with params:", params);
      await get(END_POINTS.PRESCRIPTION.GET_PRESCRIPTION_TEMPLATES({
            queryParams: params
        }))
        .then((res) => {
          prescriptionTempalte.value = res.data as any
        });
    } catch (error) {
      console.error('Error loading initial prescription templates:', error);
      throw error;
    }
  };

  return {
    // State
    getPrescriptionTemplates,
    fetchPrescriptionTemplates,
  };
});