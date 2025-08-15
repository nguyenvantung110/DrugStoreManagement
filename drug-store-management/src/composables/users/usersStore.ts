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
      error.value = null;

      if (filter) {
        currentFilter.value = { ...currentFilter.value, ...filter };
      }

      const roleValue = roles.value.find(r => r.id === currentFilter.value?.role)!
      const { get } = useApi();

      await get(END_POINTS.USER.GET_USER_LIST())
      .then((res) => {
        users.value = res.data as any;
        // Call api with fiter
        users.value = users.value.filter(user => {
          return (!currentFilter.value.search || user.username.includes(currentFilter.value.search)) &&
            (!roleValue || user.role === roleValue.name) &&
            (!currentFilter.value.status || user.status === currentFilter.value.status);
        });

        totalUsers.value = users.value.length;
      });

    } catch (err) {
      error.value = 'Không thể tải danh sách người dùng';
      console.error('Error fetching users:', err);
    } finally {
      isLoading.value = false;
    }
  };

  const fetchRoles = async (): Promise<void> => {
    try {
      const mockRoles: UserRole[] = [
        { id: '1', name: 'admin', displayName: 'Quản trị viên', permissions: [] },
        { id: '2', name: 'user', displayName: 'Dược sĩ', permissions: [] },
        { id: '3', name: 'cashier', displayName: 'Thu ngân', permissions: [] },
        { id: '4', name: 'guest', displayName: 'Khách mời', permissions: [] }
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

      const roleValue = roles.value.find(r => r.id === userData.roleId)!

      // Mock creation
      const userDto: User = {
        userId: '00000000-0000-0000-0000-000000000000',
        username: userData.username,
        passwordHash: userData.passwordHash,
        fullName: userData.fullName,
        role: roleValue.name,
        email: userData.email,
        phoneNumber: userData.phoneNumber,
        status: 'active',
        createdAt: new Date(),
        updatedAt: new Date(),
        lastLogin: new Date(),
      };

      const { post } = useApi();
      await post(END_POINTS.USER.CREATE_USER(), userDto)
      .then((res) => {
        users.value.unshift(userDto);
        totalUsers.value++;
      });

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

      const userIndex = users.value.findIndex(u => u.userId === userId);
      const roleValue = roles.value.find(r => r.id === userData.roleId)!

      if (userIndex !== -1) {
        users.value[userIndex] = {
          ...users.value[userIndex],
          ...userData,
          role: roleValue.name,
          updatedAt: new Date()
        };

        const userDto = users.value[userIndex];

        const { post } = useApi();
        await post(END_POINTS.USER.UPDATE_USER_INFO(), userDto)
        .then((res) => {
          users.value.unshift(userDto);
          totalUsers.value++;
        });
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

      const { post } = useApi();
      await post(END_POINTS.USER.DELETE_USER({
        pathParams: { id: userId }
      }))
      .then((res) => {
        users.value = users.value.filter(u => u.userId !== userId);
        totalUsers.value--;
      });

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