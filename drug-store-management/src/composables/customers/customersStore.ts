import { defineStore } from 'pinia';
import { ref, computed } from 'vue';
import type { 
  Customer, 
  CreateCustomerRequest, 
  UpdateCustomerRequest, 
  CustomerListFilter,
  CustomerGender,
  CustomerStatus 
} from '@/models/customers';

export const useCustomersStore = defineStore('customers-store', () => {
  // State
  const customers = ref<Customer[]>([]);
  const selectedCustomer = ref<Customer | null>(null);
  const isLoading = ref(false);
  const error = ref<string | null>(null);
  const totalCustomers = ref(0);
  const currentFilter = ref<CustomerListFilter>({
    search: '',
    gender: undefined,
    status: undefined,
    ageRange: undefined,
    city: '',
    page: 1,
    limit: 10
  });

  // Getters
  const getCustomersList = computed(() => customers.value);
  const getSelectedCustomer = computed(() => selectedCustomer.value);
  const getIsLoading = computed(() => isLoading.value);
  const getError = computed(() => error.value);
  const getTotalCustomers = computed(() => totalCustomers.value);
  const getCurrentFilter = computed(() => currentFilter.value);

  // Statistics getters
  const getActiveCustomersCount = computed(() => 
    customers.value.filter(c => c.status === 'active').length
  );

  const getTotalLoyaltyPoints = computed(() =>
    customers.value.reduce((sum, customer) => sum + customer.loyaltyPoints, 0)
  );

  const getTopCustomers = computed(() =>
    customers.value
      .filter(c => c.status === 'active')
      .sort((a, b) => b.totalSpent - a.totalSpent)
      .slice(0, 10)
  );

  // Actions
  const fetchCustomers = async (filter?: Partial<CustomerListFilter>): Promise<void> => {
    try {
      isLoading.value = true;
      error.value = null;

      if (filter) {
        currentFilter.value = { ...currentFilter.value, ...filter };
      }

      // Mock API call - thay thế bằng API thực tế
      await new Promise(resolve => setTimeout(resolve, 1000));

      // Mock data
      const mockCustomers: Customer[] = [
        {
          id: '1',
          customerCode: 'KH001',
          fullName: 'Nguyễn Thị Mai',
          dateOfBirth: new Date('1985-03-15'),
          gender: 'female',
          phone: '0901234567',
          email: 'mai.nguyen@email.com',
          address: {
            street: '123 Lê Lợi',
            ward: 'Phường 1',
            district: 'Quận 1',
            city: 'Hồ Chí Minh'
          },
          identityCard: '001234567890',
          insuranceCard: 'BH123456789',
          allergyHistory: ['Penicillin', 'Aspirin'],
          loyaltyPoints: 250,
          totalSpent: 2500000,
          status: 'active',
          createdAt: new Date('2024-01-15'),
          updatedAt: new Date('2024-08-01'),
          lastVisit: new Date('2024-08-07')
        },
        {
          id: '2',
          customerCode: 'KH002',
          fullName: 'Trần Văn Nam',
          dateOfBirth: new Date('1978-07-22'),
          gender: 'male',
          phone: '0901234568',
          email: 'nam.tran@email.com',
          address: {
            street: '456 Nguyễn Huệ',
            ward: 'Phường 2',
            district: 'Quận 3',
            city: 'Hồ Chí Minh'
          },
          identityCard: '001234567891',
          loyaltyPoints: 150,
          totalSpent: 1800000,
          status: 'active',
          createdAt: new Date('2024-02-10'),
          updatedAt: new Date('2024-07-20'),
          lastVisit: new Date('2024-08-05')
        },
        {
          id: '3',
          customerCode: 'KH003',
          fullName: 'Lê Thị Hoa',
          dateOfBirth: new Date('1990-11-08'),
          gender: 'female',
          phone: '0901234569',
          address: {
            street: '789 Điện Biên Phủ',
            ward: 'Phường 5',
            district: 'Quận Bình Thạnh',
            city: 'Hồ Chí Minh'
          },
          allergyHistory: ['Iodine'],
          loyaltyPoints: 80,
          totalSpent: 950000,
          status: 'inactive',
          createdAt: new Date('2024-03-20'),
          updatedAt: new Date('2024-06-15')
        }
      ];

      customers.value = mockCustomers;
      totalCustomers.value = mockCustomers.length;
    } catch (err) {
      error.value = 'Không thể tải danh sách khách hàng';
      console.error('Error fetching customers:', err);
    } finally {
      isLoading.value = false;
    }
  };

  const createCustomer = async (customerData: CreateCustomerRequest): Promise<boolean> => {
    try {
      isLoading.value = true;
      error.value = null;

      // Mock API call
      await new Promise(resolve => setTimeout(resolve, 1000));

      const customerCode = `KH${(customers.value.length + 1).toString().padStart(3, '0')}`;
      
      const newCustomer: Customer = {
        id: Date.now().toString(),
        customerCode,
        fullName: customerData.fullName,
        dateOfBirth: customerData.dateOfBirth,
        gender: customerData.gender,
        phone: customerData.phone,
        email: customerData.email,
        address: customerData.address,
        identityCard: customerData.identityCard,
        insuranceCard: customerData.insuranceCard,
        allergyHistory: customerData.allergyHistory || [],
        loyaltyPoints: 0,
        totalSpent: 0,
        status: 'active',
        createdAt: new Date(),
        updatedAt: new Date()
      };

      customers.value.unshift(newCustomer);
      totalCustomers.value++;

      return true;
    } catch (err) {
      error.value = 'Không thể tạo khách hàng mới';
      console.error('Error creating customer:', err);
      return false;
    } finally {
      isLoading.value = false;
    }
  };

  const updateCustomer = async (customerId: string, customerData: UpdateCustomerRequest): Promise<boolean> => {
    try {
      isLoading.value = true;
      error.value = null;

      // Mock API call
      await new Promise(resolve => setTimeout(resolve, 1000));

      const customerIndex = customers.value.findIndex(c => c.id === customerId);
      if (customerIndex !== -1) {
        customers.value[customerIndex] = {
          ...customers.value[customerIndex],
          ...customerData,
          updatedAt: new Date()
        };
      }

      return true;
    } catch (err) {
      error.value = 'Không thể cập nhật thông tin khách hàng';
      console.error('Error updating customer:', err);
      return false;
    } finally {
      isLoading.value = false;
    }
  };

  const deleteCustomer = async (customerId: string): Promise<boolean> => {
    try {
      isLoading.value = true;
      error.value = null;

      // Mock API call
      await new Promise(resolve => setTimeout(resolve, 1000));

      customers.value = customers.value.filter(c => c.id !== customerId);
      totalCustomers.value--;

      return true;
    } catch (err) {
      error.value = 'Không thể xóa khách hàng';
      console.error('Error deleting customer:', err);
      return false;
    } finally {
      isLoading.value = false;
    }
  };

  const searchCustomerByPhone = async (phone: string): Promise<Customer | null> => {
    try {
      // Mock API search
      const customer = customers.value.find(c => c.phone === phone);
      return customer || null;
    } catch (err) {
      console.error('Error searching customer:', err);
      return null;
    }
  };

  const addLoyaltyPoints = async (customerId: string, points: number): Promise<boolean> => {
    try {
      const customerIndex = customers.value.findIndex(c => c.id === customerId);
      if (customerIndex !== -1) {
        customers.value[customerIndex].loyaltyPoints += points;
        customers.value[customerIndex].updatedAt = new Date();
        return true;
      }
      return false;
    } catch (err) {
      console.error('Error adding loyalty points:', err);
      return false;
    }
  };

  const setSelectedCustomer = (customer: Customer | null): void => {
    selectedCustomer.value = customer;
  };

  const clearError = (): void => {
    error.value = null;
  };

  const updateFilter = (filter: Partial<CustomerListFilter>): void => {
    currentFilter.value = { ...currentFilter.value, ...filter };
  };

  const generateCustomerCode = (): string => {
    return `KH${(customers.value.length + 1).toString().padStart(3, '0')}`;
  };

  return {
    // State
    customers,
    selectedCustomer,
    isLoading,
    error,
    totalCustomers,
    currentFilter,
    // Getters
    getCustomersList,
    getSelectedCustomer,
    getIsLoading,
    getError,
    getTotalCustomers,
    getCurrentFilter,
    getActiveCustomersCount,
    getTotalLoyaltyPoints,
    getTopCustomers,
    // Actions
    fetchCustomers,
    createCustomer,
    updateCustomer,
    deleteCustomer,
    searchCustomerByPhone,
    addLoyaltyPoints,
    setSelectedCustomer,
    clearError,
    updateFilter,
    generateCustomerCode
  };
});