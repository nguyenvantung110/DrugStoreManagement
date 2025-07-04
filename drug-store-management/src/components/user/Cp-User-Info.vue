<template>
    <v-container>
      <v-row justify="center">
        <v-col cols="12" md="8" lg="6">
          <v-card>
            <v-card-title>
              <v-avatar color="primary" size="48" class="mr-3">
                <v-icon size="32">mdi-account</v-icon>
              </v-avatar>
              <span class="text-h6">{{ user.username }}</span>
              <v-spacer />
              <v-btn color="primary" @click="editMode = !editMode">
                <v-icon left>
                  {{ editMode ? 'mdi-close' : 'mdi-pencil' }}
                </v-icon>
                {{ editMode ? 'Hủy' : 'Chỉnh sửa' }}
              </v-btn>
            </v-card-title>
            <v-divider></v-divider>
  
            <v-card-text>
              <v-form ref="formRef" v-model="valid" lazy-validation>
                <v-text-field
                  v-model="editUser.fullName"
                  :readonly="!editMode"
                  label="Họ tên"
                  prepend-icon="mdi-account"
                  :rules="[rules.required]"
                  class="mb-2"
                />
                <v-text-field
                  v-model="editUser.email"
                  :readonly="!editMode"
                  label="Email"
                  prepend-icon="mdi-email"
                  type="email"
                  class="mb-2"
                  :rules="[rules.email]"
                />
                <v-text-field
                  v-model="editUser.phoneNumber"
                  :readonly="!editMode"
                  label="Số điện thoại"
                  prepend-icon="mdi-phone"
                  class="mb-2"
                />
                <v-text-field
                  v-model="editUser.role"
                  :readonly="true"
                  label="Vai trò"
                  prepend-icon="mdi-account-key"
                  class="mb-2"
                />
                <v-text-field
                  v-model="editUser.username"
                  :readonly="true"
                  label="Username"
                  prepend-icon="mdi-account-circle"
                  class="mb-2"
                />
                <v-text-field
                  v-model="editUser.createdAt"
                  :readonly="true"
                  label="Ngày tạo"
                  prepend-icon="mdi-calendar-plus"
                  class="mb-2"
                  :value="formatDate(editUser.createdAt)"
                />
                <v-text-field
                  v-model="editUser.updatedAt"
                  :readonly="true"
                  label="Ngày cập nhật"
                  prepend-icon="mdi-calendar-refresh"
                  class="mb-2"
                  :value="formatDate(editUser.updatedAt)"
                />
  
                <v-row v-if="editMode" class="mt-3">
                  <v-col class="d-flex justify-end">
                    <v-btn color="primary" @click="saveUser">Lưu thay đổi</v-btn>
                  </v-col>
                </v-row>
              </v-form>
            </v-card-text>
          </v-card>
        </v-col>
      </v-row>
    </v-container>
  </template>
  
  <script setup lang="ts">
  import { ref, reactive, onMounted } from 'vue'
  import { VForm } from 'vuetify/components'
  import { useAuthenticationStore } from '@/composables/auth/authenticationStore'
  import { useDialog } from '@/composables/common/useDialog'
  import { useRouter } from 'vue-router'
  import { useUserStore } from '@/composables/user/userStore'
  
  interface UserDto {
    userId: string
    username: string
    passwordHash: string
    fullName?: string
    role: string
    email?: string
    phoneNumber?: string
    createdAt: string
    updatedAt: string
  }
  
  const authenticationStore = useAuthenticationStore()
  const userStore = useUserStore()
  const router = useRouter()
  const dialog = useDialog()

  const user = reactive<UserDto>({
    userId: '',
    username: '',
    passwordHash: '',
    fullName: '',
    role: '',
    email: '',
    phoneNumber: '',
    createdAt: '',
    updatedAt: ''
  })
  
  const editUser = reactive<UserDto>({ ...user })
  const editMode = ref(false)
  const valid = ref(false)
  const formRef = ref<typeof VForm | null>(null)
  
  const rules = {
    required: (v: any) => !!v || 'Không được để trống',
    email: (v: string) => !v || /^[\w-.]+@([\w-]+\.)+[\w-]{2,4}$/.test(v) || 'Email không hợp lệ'
  }
  
  const api = {
    async updateUser(data: Partial<UserDto>) {
      return true
    }
  }
  
  function formatDate(dateStr: string | undefined) {
    if (!dateStr) return ''
    return new Date(dateStr).toLocaleString()
  }
  
  async function fetchUser() {
    const userId = authenticationStore.getterUserId;
    await userStore.getUserInfo(userId, (res: any) => {
        Object.assign(user, res?.data || {})
        Object.assign(editUser, res?.data || {})
    })
  }
  
  async function saveUser() {
    if (!(await (formRef.value as any)?.validate())) return
  
    const updateData = {
      userId: user.userId,
      fullName: editUser.fullName,
      email: editUser.email,
      phoneNumber: editUser.phoneNumber
    }
    await userStore.updateUserInfo(updateData)
    await dialog.success('Lưu thông tin user thành công!')
    await fetchUser();
    editMode.value = false
  }
  
  onMounted(fetchUser)
  </script>