import { ref, computed } from 'vue';
import type { DrugItem } from '@/models/sales';
import { useApi } from '@/composables/common/api-instance';
import { END_POINTS } from '@/endpoints/api-endpoints';

// Enhanced drug interface for search results
interface DrugSearchResult {
  productId?: string;
  productName: string;
  manufacturer?: string;
  unitOfMeasure?: string;
  pricePerUnit?: number;
}

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

  // Enhanced drug database for search functionality
  const drugDatabase = ref<DrugSearchResult[]>([]);

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
    return !!(drug.productName && drug.quantity > 0 && drug.unitOfMeasure && drug.pricePerUnit > 0);
  };

  const formatCurrency = (amount: number): string => {
    if (amount === undefined || amount === null) {
      return 0.00.toLocaleString('vi-VN', { style: 'currency', currency: 'VND' });
    }
    
    return new Intl.NumberFormat('vi-VN', {
      style: 'currency',
      currency: 'VND'
    }).format(amount);
  };

  // New methods for enhanced drug operations
  
  /**
   * Search drugs by query string
   * @param query - Search query string
   * @returns Promise resolving to array of matching drugs
   */
  const searchDrugs = async (query: string): Promise<DrugSearchResult[]> => {
    // Simulate API delay
    await new Promise(resolve => setTimeout(resolve, 300));

    if (!query || query.length < 2) {
      return [];
    }

    const searchTerm = query.toLowerCase().trim();
    
    return drugDatabase.value.filter(drug => 
      drug.productName.toLowerCase().includes(searchTerm) ||
      drug.manufacturer?.toLowerCase().includes(searchTerm)
    ).slice(0, 10); // Limit to 10 results
  };

  /**
   * Load initial drug list for the application
   * @returns Promise that resolves when initial data is loaded
   */
  const loadInitialDrugList = async (): Promise<void> => {
    try {
      const { get } = useApi();
      await get(END_POINTS.SUPPLIERS.GET_LIST())
        .then((res) => {
          // success(res);
          drugDatabase.value = res.data as any
        });
    } catch (error) {
      console.error('Error loading initial drug list:', error);
      throw error;
    }
  };

  /**
   * Add new drug to database
   * @param drugData - Drug information to add
   * @returns Promise resolving to the added drug
   */
  const addNewDrug = async (drugData: Partial<DrugSearchResult>): Promise<DrugSearchResult> => {
    try {
      // Simulate API call
      await new Promise(resolve => setTimeout(resolve, 1000));
      
      const newDrug: DrugSearchResult = {
        productId: Date.now().toString(),
        productName: drugData.productName || '',
        manufacturer: drugData.manufacturer || 'Chưa xác định',
        pricePerUnit: drugData.pricePerUnit || 0,
        unitOfMeasure: drugData.unitOfMeasure || 'viên',
        
      };

      // Add to local database
      drugDatabase.value.push(newDrug);
      
      return newDrug;
    } catch (error) {
      console.error('Error adding new drug:', error);
      throw error;
    }
  };

  /**
   * Get drug details by ID
   * @param drugId - Drug ID to lookup
   * @returns Promise resolving to drug details or null if not found
   */
  const getDrugById = async (drugId: string): Promise<DrugSearchResult | null> => {
    try {
      // Simulate API call
      await new Promise(resolve => setTimeout(resolve, 200));
      
      const drug = drugDatabase.value.find(d => d.productId === drugId);
      return drug || null;
    } catch (error) {
      console.error('Error fetching drug by ID:', error);
      return null;
    }
  };

  /**
   * Get popular/frequently used drugs
   * @param limit - Number of drugs to return
   * @returns Array of popular drugs
   */
  const getPopularDrugs = (): DrugSearchResult[] => {
    // In real app, this would be based on sales data
    return drugDatabase.value
      .slice(0, 5)
      .map(drug => ({ ...drug }));
  };

  /**
   * Check drug interactions
   * @param drugNames - Array of drug names to check
   * @returns Promise resolving to interaction warnings
   */
  const checkDrugInteractions = async (drugNames: string[]): Promise<string[]> => {
    // Simulate API call to drug interaction service
    await new Promise(resolve => setTimeout(resolve, 800));
    
    const interactions: string[] = [];
    
    // Simple interaction checking logic (replace with real API)
    const hasParacetamol = drugNames.some(name => 
      name.toLowerCase().includes('paracetamol')
    );
    const hasIbuprofen = drugNames.some(name => 
      name.toLowerCase().includes('ibuprofen')
    );
    
    if (hasParacetamol && hasIbuprofen) {
      interactions.push('Cảnh báo: Không nên dùng đồng thời Paracetamol và Ibuprofen mà không có chỉ định của bác sĩ.');
    }
    
    return interactions;
  };

  /**
   * Format drug dosage instruction
   * @param dosage - Raw dosage string
   * @returns Formatted dosage instruction
   */
  const formatDosageInstruction = (dosage: string): string => {
    if (!dosage) return '';
    
    // Add common Vietnamese medical terms formatting
    return dosage
      .replace(/x(\d+)/g, '× $1')
      .replace(/(\d+)\s*lần/g, '$1 lần')
      .replace(/ngày/gi, 'ngày')
      .trim();
  };

  /**
   * Calculate drug expiry status
   * @param expiryDate - Expiry date string
   * @returns Status object with color and text
   */
  const getDrugExpiryStatus = (expiryDate: string): { 
    status: 'valid' | 'warning' | 'expired',
    color: string,
    text: string,
    daysRemaining: number
  } => {
    if (!expiryDate) {
      return { status: 'valid', color: 'grey', text: 'Chưa xác định', daysRemaining: 0 };
    }

    const today = new Date();
    const expiry = new Date(expiryDate);
    const diffTime = expiry.getTime() - today.getTime();
    const diffDays = Math.ceil(diffTime / (1000 * 60 * 60 * 24));

    if (diffDays < 0) {
      return { status: 'expired', color: 'error', text: 'Đã hết hạn', daysRemaining: diffDays };
    } else if (diffDays <= 30) {
      return { status: 'warning', color: 'warning', text: `Còn ${diffDays} ngày`, daysRemaining: diffDays };
    } else {
      return { status: 'valid', color: 'success', text: `Còn ${diffDays} ngày`, daysRemaining: diffDays };
    }
  };

  return {
    // Original exports
    drugNameList,
    unitOptions,
    dosageOptions,
    createEmptyDrug,
    generateDummyDrugs,
    validateDrugItem,
    formatCurrency,
    
    // New enhanced exports
    drugDatabase,
    searchDrugs,
    loadInitialDrugList,
    addNewDrug,
    getDrugById,
    getPopularDrugs,
    checkDrugInteractions,
    formatDosageInstruction,
    getDrugExpiryStatus
  };
};

// Export types for use in other components
export type { DrugSearchResult };