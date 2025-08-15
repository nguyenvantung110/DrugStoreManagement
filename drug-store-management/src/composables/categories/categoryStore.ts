import { defineStore } from 'pinia';
import { ref, computed } from 'vue';
import type { DrugItem, SalesTransaction } from '@/models/sales';
import { useApi } from '@/composables/common/api-instance';
import { END_POINTS } from '@/endpoints/api-endpoints';

export const useCategoryStore = defineStore('category-store', () => {
  // State
  const categoryList = ref<any[]>([]);

  // Getters
  const getCategoryList = computed(() => categoryList.value);
  
  // Actions

  const fetchCategories = async (): Promise<void> => {
    try {
      const { get } = useApi();
      await get(END_POINTS.PRESCRIPTION.GET_PRESCRIPTION_TEMPLATES())
        .then((res) => {
          categoryList.value = res.data as any
        });
    } catch (error) {
      console.error('Error loading initial prescription templates:', error);
      throw error;
    }
  };

  const fetchCategoryByType = async (type: string): Promise<void> => {
    try {
      const { get } = useApi();
      await get(END_POINTS.CATEGORIES.GET_CATEGORY_BY_TYPE({
        queryParams: {
          categoryId: type,
        }
      }))
        .then((res) => {
          categoryList.value = res.data as any
        });
    } catch (error) {
      console.error('Error loading initial prescription templates:', error);
      throw error;
    }
  };

  return {
    // State
    getCategoryList,
    fetchCategories,
    fetchCategoryByType
  };
});