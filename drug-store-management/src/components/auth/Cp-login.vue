<template>
    <v-container fluid class="login-bg d-flex align-center justify-center fill-height pa-0">
      <div class="overlay"></div>
      <v-slide-x-transition mode="out-in">
        <v-card
          :key="isLogin ? 'login' : 'register'"
          class="pa-8"
          max-width="500"
          min-width="400"
          elevation="12"
          v-motion
          v-motion-fade
          v-motion-slide-bottom
        >
          <v-card-title class="text-h5 font-weight-bold text-center mb-4">
            {{ isLogin ? 'Đăng nhập' : 'Đăng ký tài khoản' }}
          </v-card-title>
          <v-form
            @submit.prevent="isLogin ? onLogin() : onRegister()"
            v-model="valid"
            ref="formRef"
          >
            <v-text-field
              v-model="username"
              label="Tên đăng nhập"
              prepend-inner-icon="mdi-account"
              :rules="usernameRules"
              color="primary"
              variant="outlined"
              class="mb-4"
              autofocus
            />
            <v-text-field
              v-model="password"
              label="Mật khẩu"
              prepend-inner-icon="mdi-lock"
              :type="showPassword ? 'text' : 'password'"
              :append-inner-icon="showPassword ? 'mdi-eye' : 'mdi-eye-off'"
              @click:append-inner="showPassword = !showPassword"
              :rules="passwordRules"
              color="primary"
              variant="outlined"
              class="mb-4"
            />
            <v-text-field
              v-if="!isLogin"
              v-model="confirmPassword"
              label="Xác nhận mật khẩu"
              prepend-inner-icon="mdi-lock-check"
              :type="showConfirmPassword ? 'text' : 'password'"
              :append-inner-icon="showConfirmPassword ? 'mdi-eye' : 'mdi-eye-off'"
              @click:append-inner="showConfirmPassword = !showConfirmPassword"
              :rules="confirmPasswordRules"
              color="primary"
              variant="outlined"
              class="mb-6"
            />
            <v-btn
              :loading="loading"
              :disabled="!valid || loading"
              block
              color="primary"
              size="large"
              type="submit"
              elevation="2"
              class="transition-fast-in-fast-out mb-2"
            >
              {{ isLogin ? 'Đăng nhập' : 'Đăng ký' }}
            </v-btn>
            <v-btn
              variant="text"
              color="primary"
              block
              @click="toggleForm"
              class="text-capitalize"
            >
              {{ isLogin ? 'Chưa có tài khoản? Đăng ký' : 'Đã có tài khoản? Đăng nhập' }}
            </v-btn>
          </v-form>
        </v-card>
      </v-slide-x-transition>
    </v-container>
  </template>
  
  <script setup lang="ts">
  import { ref, computed } from 'vue'
  import { useRouter } from 'vue-router'
  
  // State
  const isLogin = ref(true)
  const username = ref('')
  const password = ref('')
  const confirmPassword = ref('')
  const showPassword = ref(false)
  const showConfirmPassword = ref(false)
  const loading = ref(false)
  const valid = ref(false)
  const formRef = ref()
  
  const usernameRules = [
    (v: string) => !!v || 'Vui lòng nhập tên đăng nhập',
    (v: string) => v.length >= 3 || 'Tối thiểu 3 ký tự',
  ]
  const passwordRules = [
    (v: string) => !!v || 'Vui lòng nhập mật khẩu',
    (v: string) => v.length >= 3 || 'Tối thiểu 3 ký tự',
  ]
  const confirmPasswordRules = [
    (v: string) => !!v || 'Vui lòng xác nhận mật khẩu',
    (v: string) => v === password.value || 'Mật khẩu xác nhận không khớp'
  ]
  
  const router = useRouter()
  
  function onLogin() {
    router.push('/')
    if (!valid.value) {
      (formRef.value as any)?.validate?.()
      return
    }
    loading.value = true
    setTimeout(() => {
      loading.value = false
      router.push('/')
    }, 1000)
  }
  
  function onRegister() {
    if (!valid.value) {
      (formRef.value as any)?.validate?.()
      return
    }
    loading.value = true
    setTimeout(() => {
      loading.value = false
      isLogin.value = true
      //router.push('/login')
    }, 1000)
  }
  
  function toggleForm() {
    username.value = ''
    password.value = ''
    confirmPassword.value = ''
    valid.value = false
    isLogin.value = !isLogin.value
  }
  </script>
  
  <style scoped>
  .login-bg {
    min-height: 100vh;
    background: url('https://images.unsplash.com/photo-1506744038136-46273834b3fb?auto=format&fit=crop&w=1350&q=80') center center/cover no-repeat;
    position: relative;
  }
  .overlay {
    position: absolute;
    z-index: 1;
    inset: 0;
    background: linear-gradient(120deg, rgba(77, 182, 172, 0.7), rgba(0,0,0,0.3));
  }
  .v-card {
    position: relative;
    z-index: 2;
    border-radius: 18px;
    box-shadow: 0 8px 32px rgba(40, 40, 130, 0.18);
    background: rgba(255,255,255,0.97);
    transition: box-shadow 0.2s, transform 0.2s;
  }
  .v-card:hover {
    box-shadow: 0 12px 40px rgba(40, 40, 130, 0.24);
    transform: translateY(-4px) scale(1.02);
  }
  .v-btn {
    font-weight: 600;
    letter-spacing: .5px;
  }
  </style>