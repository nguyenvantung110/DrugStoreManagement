import { defineStore } from 'pinia';
import { ref, computed } from 'vue';
import type { DrugItem, OrderCreateDto, SalesTransaction } from '@/models/sales';
import { useApi } from '@/composables/common/api-instance';
import { END_POINTS } from '@/endpoints/api-endpoints';
import { useLoadingStore } from '@/composables/common/loadingStore';

export const useSalesStore = defineStore('sales-store', () => {
  const loadingStore = useLoadingStore();

  // State
  const drugListToSale = ref<DrugItem[]>([]);
  const currentTransaction = ref<SalesTransaction | null>(null);
  const goBackStatus = ref(false);
  const isLoading = ref(false);
  const error = ref<string | null>(null);
  const notes = ref<string>('');
  const grandTotal = ref<number>(0);

  // Getters
  const getDrugListToSale = computed(() => drugListToSale.value);
  const getGoBackStatus = computed(() => goBackStatus.value);
  const getCurrentTransaction = computed(() => currentTransaction.value);
  const getGrandTotal = computed(() => grandTotal.value);
  const getNotes = computed(() => notes.value);

  // Actions
  const updateDrugListToPayment = (drugs: DrugItem[], presGrandTotal: number, presNotes: string): void => {
    drugListToSale.value = [...drugs];
    grandTotal.value = presGrandTotal;
    notes.value = presNotes;
    goBackStatus.value = true;
  };

  const addDrugToSale = (drug: DrugItem): void => {
    const newDrug = { ...drug, seq: drugListToSale.value.length + 1 };
    drugListToSale.value.push(newDrug);
  };

  const removeDrugFromSale = (index: number): void => {
    drugListToSale.value.splice(index, 1);
    // Cập nhật lại số thứ tự
    drugListToSale.value.forEach((item, idx) => {
      item.seq = idx + 1;
    });
  };

  const updateDrugQuantity = (index: number, quantity: number): void => {
    if (drugListToSale.value[index]) {
      drugListToSale.value[index].quantity = quantity;
      drugListToSale.value[index].subTotal = 
        drugListToSale.value[index].pricePerUnit * quantity;
    }
  };

  const clearSaleData = (): void => {
    drugListToSale.value = [];
    currentTransaction.value = null;
    goBackStatus.value = false;
    error.value = null;
  };

  const setError = (errorMessage: string): void => {
    error.value = errorMessage;
  };

  const clearError = (): void => {
    error.value = null;
  };

  const loadInitialDrugList = async (): Promise<void> => {
    try {
      const { get } = useApi();
      await get(END_POINTS.PRODUCT.GET_BASIC_INFO_LIST())
        .then((res) => {
          // success(res);
          drugListToSale.value = res.data as any
        });
    } catch (error) {
      console.error('Error loading initial drug list:', error);
      throw error;
    }
  };

  const createOrder = async (orderCreateDto: OrderCreateDto): Promise<void> => {
    loadingStore.show();
    const { post } = useApi();
    await post(END_POINTS.SALE_ORDERS.CREATE_ORDER(), { ...orderCreateDto })
    loadingStore.hide();
  };

  return {
    // State
    drugListToSale,
    currentTransaction,
    goBackStatus,
    isLoading,
    error,
    // Getters
    getDrugListToSale,
    getGoBackStatus,
    getCurrentTransaction,
    getGrandTotal,
    getNotes,
    // Actions
    updateDrugListToPayment,
    loadInitialDrugList,
    addDrugToSale,
    removeDrugFromSale,
    updateDrugQuantity,
    clearSaleData,
    setError,
    clearError,
    createOrder
  };
});