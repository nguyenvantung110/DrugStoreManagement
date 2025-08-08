import { defineStore } from 'pinia';
import { ref, computed } from 'vue';
import type { User, CreateUserRequest, UpdateUserRequest, UserListFilter, UserRole } from '@/models/users';
import { useApi } from "@/composables/common/api-instance";
import { END_POINTS } from "@/endpoints/api-endpoints";

export const useUsersStore = defineStore('users-store', () => {
  // State
  const users = ref<User[]>([]);
  const roles = ref<UserRole[]>([]);
  const selectedUser = ref<User | null>(null);
  const isLoading = ref(false);
  const error = ref<string | null>(null);
  const totalUsers = ref(0);
  const currentFilter = ref<UserListFilter>({
    search: '',
    role: '',
    status: undefined,
    page: 1,
    limit: 10
  });

  // Getters
  const getUsersList = computed(() => users.value);
  const getRolesList = computed(() => roles.value);
  const getSelectedUser = computed(() => selectedUser.value);
  const getIsLoading = computed(() => isLoading.value);
  const getError = computed(() => error.value);
  const getTotalUsers = computed(() => totalUsers.value);
  const getCurrentFilter = computed(() => currentFilter.value);

  // Actions
  const fetchUsers = async (filter?: Partial<UserListFilter>): Promise<void> => {
    try {
      isLoading.value = true;
      error.value = null;

      if (filter) {
        currentFilter.value = { ...currentFilter.value, ...filter };
      }

      // Mock API call - thay thế bằng API thực tế
      await new Promise(resolve => setTimeout(resolve, 1000));

      // Mock data
      const mockUsers: User[] = [
        {
          id: '1',
          username: 'admin',
          email: 'admin@pharmacy.com',
          fullName: 'Nguyễn Văn Admin',
          phone: '0901234567',
          role: { id: '1', name: 'admin', displayName: 'Quản trị viên', permissions: [] },
          status: 'active',
          createdAt: new Date('2024-01-01'),
          updatedAt: new Date('2024-01-15'),
          lastLogin: new Date('2024-08-08')
        },
        {
          id: '2',
          username: 'pharmacist1',
          email: 'pharmacist1@pharmacy.com',
          fullName: 'Trần Thị Dược Sĩ',
          phone: '0901234568',
          role: { id: '2', name: 'pharmacist', displayName: 'Dược sĩ', permissions: [] },
          status: 'active',
          createdAt: new Date('2024-01-02'),
          updatedAt: new Date('2024-01-16'),
          lastLogin: new Date('2024-08-07')
        },
        {
          id: '3',
          username: 'cashier1',
          email: 'cashier1@pharmacy.com',
          fullName: 'Lê Văn Thu Ngân',
          phone: '0901234569',
          role: { id: '3', name: 'cashier', displayName: 'Thu ngân', permissions: [] },
          status: 'inactive',
          createdAt: new Date('2024-01-03'),
          updatedAt: new Date('2024-01-17')
        }
      ];

      users.value = mockUsers;
      totalUsers.value = mockUsers.length;
    } catch (err) {
      error.value = 'Không thể tải danh sách người dùng';
      console.error('Error fetching users:', err);
    } finally {
      isLoading.value = false;
    }
  };

  const fetchRoles = async (): Promise<void> => {
    try {
      // Mock API call
      await new Promise(resolve => setTimeout(resolve, 500));

      const mockRoles: UserRole[] = [
        { id: '1', name: 'admin', displayName: 'Quản trị viên', permissions: [] },
        { id: '2', name: 'pharmacist', displayName: 'Dược sĩ', permissions: [] },
        { id: '3', name: 'cashier', displayName: 'Thu ngân', permissions: [] }
      ];

      roles.value = mockRoles;
    } catch (err) {
      error.value = 'Không thể tải danh sách vai trò';
      console.error('Error fetching roles:', err);
    }
  };

  const createUser = async (userData: CreateUserRequest): Promise<boolean> => {
    try {
      isLoading.value = true;
      error.value = null;

      // Mock API call
      await new Promise(resolve => setTimeout(resolve, 1000));

      // Mock creation
      const newUser: User = {
        id: Date.now().toString(),
        username: userData.username,
        email: userData.email,
        fullName: userData.fullName,
        phone: userData.phone,
        role: roles.value.find(r => r.id === userData.roleId)!,
        status: 'active',
        createdAt: new Date(),
        updatedAt: new Date()
      };

      users.value.unshift(newUser);
      totalUsers.value++;

      return true;
    } catch (err) {
      error.value = 'Không thể tạo người dùng mới';
      console.error('Error creating user:', err);
      return false;
    } finally {
      isLoading.value = false;
    }
  };

  const updateUser = async (userId: string, userData: UpdateUserRequest): Promise<boolean> => {
    try {
      isLoading.value = true;
      error.value = null;

      // Mock API call
      await new Promise(resolve => setTimeout(resolve, 1000));

      const userIndex = users.value.findIndex(u => u.id === userId);
      if (userIndex !== -1) {
        users.value[userIndex] = {
          ...users.value[userIndex],
          ...userData,
          role: userData.roleId ? roles.value.find(r => r.id === userData.roleId)! : users.value[userIndex].role,
          updatedAt: new Date()
        };
      }

      return true;
    } catch (err) {
      error.value = 'Không thể cập nhật người dùng';
      console.error('Error updating user:', err);
      return false;
    } finally {
      isLoading.value = false;
    }
  };

  const deleteUser = async (userId: string): Promise<boolean> => {
    try {
      isLoading.value = true;
      error.value = null;

      // Mock API call
      await new Promise(resolve => setTimeout(resolve, 1000));

      users.value = users.value.filter(u => u.id !== userId);
      totalUsers.value--;

      return true;
    } catch (err) {
      error.value = 'Không thể xóa người dùng';
      console.error('Error deleting user:', err);
      return false;
    } finally {
      isLoading.value = false;
    }
  };

  const setSelectedUser = (user: User | null): void => {
    selectedUser.value = user;
  };

  const clearError = (): void => {
    error.value = null;
  };

  const updateFilter = (filter: Partial<UserListFilter>): void => {
    currentFilter.value = { ...currentFilter.value, ...filter };
  };

  return {
    // State
    users,
    roles,
    selectedUser,
    isLoading,
    error,
    totalUsers,
    currentFilter,
    // Getters
    getUsersList,
    getRolesList,
    getSelectedUser,
    getIsLoading,
    getError,
    getTotalUsers,
    getCurrentFilter,
    // Actions
    fetchUsers,
    fetchRoles,
    createUser,
    updateUser,
    deleteUser,
    setSelectedUser,
    clearError,
    updateFilter
  };
});