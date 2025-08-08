import { ref, computed } from 'vue';
import type { DrugItem } from '@/models/sales';

export const useDrugOperations = () => {
  const drugNameList = ref([
    'Paracetamol 500mg',
    'Alphachoay',
    'Zinnat 500mg',
    'Fexophar 120mg',
    'Medrol 4mg'
  ]);

  const unitOptions = ref([
    'viên', 'vỉ', 'hộp', 'chai', 'tuýp', 'miếng', 'gói', 'lọ'
  ]);

  const dosageOptions = ref([
    'Ngày 2 lần sáng/tối, sau ăn',
    'Ngày 3 lần sau ăn',
    'Ngày 1 lần trước ăn',
    'Ngày 1 lần sau ăn',
    'Khi cần thiết',
    'Theo chỉ định của bác sĩ'
  ]);

  const createEmptyDrug = (seq: number): DrugItem => ({
    seq,
    name: '',
    quantity: 0,
    unit: 'viên',
    dosage: '',
    unitPrice: 2000,
    subTotal: 0
  });

  const generateDummyDrugs = (count: number): DrugItem[] => {
    return Array.from({ length: count }, (_, i) => {
      const quantity = Math.floor(Math.random() * 10) + 1;
      const unitPrice = Math.floor(Math.random() * 50000) + 5000;
      
      return {
        seq: i + 1,
        name: drugNameList.value[i % drugNameList.value.length],
        quantity,
        unit: 'viên',
        dosage: dosageOptions.value[0],
        unitPrice,
        subTotal: quantity * unitPrice
      };
    });
  };

  const validateDrugItem = (drug: DrugItem): boolean => {
    return !!(drug.name && drug.quantity > 0 && drug.unit && drug.unitPrice > 0);
  };

  const formatCurrency = (amount: number): string => {
    return new Intl.NumberFormat('vi-VN', {
      style: 'currency',
      currency: 'VND'
    }).format(amount);
  };

  return {
    drugNameList,
    unitOptions,
    dosageOptions,
    createEmptyDrug,
    generateDummyDrugs,
    validateDrugItem,
    formatCurrency
  };
};