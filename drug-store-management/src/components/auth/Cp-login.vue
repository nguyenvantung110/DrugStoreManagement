<template>
  <!-- Template content remains unchanged -->
  <div class="min-h-screen login-background flex items-center justify-center p-4">
    <!-- Background Decorative Elements -->
    <div class="absolute inset-0 overflow-hidden">
      <!-- Gradient Background -->
      <div class="absolute inset-0 bg-gradient-to-br from-primary/5 via-blue-50/30 to-green-50/20"></div>
      
      <!-- Floating Shapes -->
      <div class="floating-shapes">
        <div class="shape shape-1"></div>
        <div class="shape shape-2"></div>
        <div class="shape shape-3"></div>
        <div class="shape shape-4"></div>
        <div class="shape shape-5"></div>
        <div class="shape shape-6"></div>
      </div>
      
      <!-- Grid Pattern -->
      <div class="absolute inset-0 opacity-[0.02]">
        <div class="grid-pattern"></div>
      </div>
    </div>

    <!-- Login Card -->
    <v-slide-x-transition mode="out-in">
      <v-card
        :key="isLogin ? 'login' : 'register'"
        elevation="0"
        class="login-card rounded-xl border border-gray-200 relative z-10"
        max-width="480"
        width="100%"
      >
        <!-- Header -->
        <v-card-title class="login-header px-8 py-6 text-center border-b border-gray-100">
          <div class="w-full">
            <!-- Logo/Icon -->
            <div class="flex justify-center mb-4">
              <v-avatar size="64" class="logo-avatar mb-3">
                <v-icon color="white" size="32">mdi-pill-multiple</v-icon>
              </v-avatar>
            </div>
            
            <!-- Title -->
            <h1 class="text-2xl font-bold text-gray-800 mb-2">
              {{ isLogin ? 'Đăng nhập hệ thống' : 'Đăng ký tài khoản' }}
            </h1>
            <p class="text-sm text-gray-500">
              {{ isLogin ? 'Chào mừng bạn quay trở lại' : 'Tạo tài khoản mới để sử dụng hệ thống' }}
            </p>
          </div>
        </v-card-title>

        <!-- Form Content -->
        <v-card-text class="login-content px-8 py-6">
          <v-form
            @submit.prevent="isLogin ? onLogin() : onRegister()"
            v-model="valid"
            ref="formRef"
            class="space-y-4"
          >
            <!-- Username Field -->
            <div class="form-group">
              <label class="form-label">
                <v-icon size="16" class="mr-2 label-icon">mdi-account</v-icon>
                Tên đăng nhập
              </label>
              <v-text-field
                v-model="username"
                placeholder="Nhập tên đăng nhập"
                :rules="usernameRules"
                density="compact"
                variant="outlined"
                class="input-field user-name-input"
                hide-details="auto"
                autofocus
              >
                <template #prepend-inner>
                  <v-icon size="18" color="gray-400">mdi-account</v-icon>
                </template>
              </v-text-field>
            </div>

            <!-- Password Field -->
            <div class="form-group">
              <label class="form-label">
                <v-icon size="16" class="mr-2 label-icon">mdi-lock</v-icon>
                Mật khẩu
              </label>
              <v-text-field
                v-model="password"
                placeholder="Nhập mật khẩu"
                :type="showPassword ? 'text' : 'password'"
                :rules="passwordRules"
                density="compact"
                variant="outlined"
                class="input-field"
                hide-details="auto"
              >
                <template #prepend-inner>
                  <v-icon size="18" color="gray-400">mdi-lock</v-icon>
                </template>
                <template #append-inner>
                  <v-btn
                    icon
                    size="small"
                    variant="plain"
                    @click="showPassword = !showPassword"
                    class="hover:bg-gray-100"
                  >
                    <v-icon size="18" color="gray-400">
                      {{ showPassword ? 'mdi-eye-off' : 'mdi-eye' }}
                    </v-icon>
                  </v-btn>
                </template>
              </v-text-field>
            </div>

            <!-- Confirm Password Field (Register only) -->
            <div v-if="!isLogin" class="form-group">
              <label class="form-label">
                <v-icon size="16" class="mr-2 label-icon">mdi-lock-check</v-icon>
                Xác nhận mật khẩu
              </label>
              <v-text-field
                v-model="confirmPassword"
                placeholder="Nhập lại mật khẩu"
                :type="showConfirmPassword ? 'text' : 'password'"
                :rules="confirmPasswordRules"
                density="compact"
                variant="outlined"
                class="input-field"
                hide-details="auto"
              >
                <template #prepend-inner>
                  <v-icon size="18" color="gray-400">mdi-lock-check</v-icon>
                </template>
                <template #append-inner>
                  <v-btn
                    icon
                    size="small"
                    variant="plain"
                    @click="showConfirmPassword = !showConfirmPassword"
                    class="hover:bg-gray-100"
                  >
                    <v-icon size="18" color="gray-400">
                      {{ showConfirmPassword ? 'mdi-eye-off' : 'mdi-eye' }}
                    </v-icon>
                  </v-btn>
                </template>
              </v-text-field>
            </div>

            <!-- Remember Me (Login only) -->
            <div v-if="isLogin" class="form-group">
              <div class="flex items-center justify-between">
                <v-checkbox
                  v-model="rememberMe"
                  color="#11c393"
                  density="compact"
                  hide-details
                  class="remember-checkbox"
                >
                  <template #label>
                    <span class="text-sm text-gray-600 ml-1">Ghi nhớ đăng nhập</span>
                  </template>
                </v-checkbox>
                
                <v-btn
                  variant="text"
                  size="small"
                  class="text-sm forgot-password-btn"
                  @click="forgotPassword"
                >
                  Quên mật khẩu?
                </v-btn>
              </div>
            </div>

            <!-- Submit Button -->
            <div class="form-group pt-2">
              <v-btn
                :loading="loading"
                :disabled="!valid || loading"
                type="submit"
                color="#11c393"
                variant="flat"
                size="large"
                block
                class="submit-btn text-white"
              >
                <v-icon start size="18">
                  {{ isLogin ? 'mdi-login' : 'mdi-account-plus' }}
                </v-icon>
                {{ isLogin ? 'Đăng nhập' : 'Đăng ký tài khoản' }}
              </v-btn>
            </div>
          </v-form>
        </v-card-text>

        <!-- Footer -->
        <v-card-actions class="login-footer px-8 py-6 border-t border-gray-100 bg-gray-50">
          <div class="w-full text-center">
            <p class="text-sm text-gray-600 mb-3">
              {{ isLogin ? 'Chưa có tài khoản?' : 'Đã có tài khoản?' }}
            </p>
            <v-btn
              variant="outlined"
              color="#11c393"
              size="large"
              block
              @click="toggleForm"
              class="toggle-btn"
            >
              <v-icon start size="18">
                {{ isLogin ? 'mdi-account-plus' : 'mdi-login' }}
              </v-icon>
              {{ isLogin ? 'Tạo tài khoản mới' : 'Đăng nhập ngay' }}
            </v-btn>
            
            <!-- Additional Info -->
            <div class="mt-4 pt-3 border-t border-gray-200">
              <p class="text-xs text-gray-500">
                Hệ thống quản lý nhà thuốc phiên bản 2.0
              </p>
              <div class="flex items-center justify-center gap-4 mt-2">
                <span class="text-xs text-gray-400 flex items-center gap-1">
                  <v-icon size="12" color="#11c393">mdi-shield-check</v-icon>
                  Bảo mật cao
                </span>
                <span class="text-xs text-gray-400 flex items-center gap-1">
                  <v-icon size="12" color="#11c393">mdi-clock</v-icon>
                  24/7 hỗ trợ
                </span>
              </div>
            </div>
          </div>
        </v-card-actions>
      </v-card>
    </v-slide-x-transition>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthenticationStore } from '@/composables/auth/authenticationStore'
import { useDialog } from '@/composables/common/useDialog'
import { MESSAGE } from '@/constants/message'

// Store
const authStore = useAuthenticationStore()
const dialog = useDialog()

// State
const isLogin = ref(true)
const username = ref('')
const password = ref('')
const confirmPassword = ref('')
const showPassword = ref(false)
const showConfirmPassword = ref(false)
const rememberMe = ref(false)
const loading = ref(false)
const valid = ref(false)
const formRef = ref()

// Validation rules
const usernameRules = [
  (v: string) => !!v || MESSAGE.LGSC.LGSC_VLD_001,
  (v: string) => v.length >= 3 || MESSAGE.LGSC.LGSC_VLD_002,
]

const passwordRules = [
  (v: string) => !!v || MESSAGE.LGSC.LGSC_VLD_003,
  (v: string) => v.length >= 3 || MESSAGE.LGSC.LGSC_VLD_004,
]

const confirmPasswordRules = [
  (v: string) => !!v || MESSAGE.LGSC.LGSC_VLD_005,
  (v: string) => v === password.value || MESSAGE.LGSC.LGSC_VLD_006
]

const router = useRouter()

// Methods
const onLogin = async () => {
  if (!valid.value) {
    (formRef.value as any)?.validate?.()
    return
  }

  const loginRequest = {
    username: username.value,
    password: password.value
  }

  loading.value = true
  
  await authStore.login(loginRequest, (res: any) => {
    loading.value = false
    if (res) {
      router.push('/')
    } else {
      dialog.error(MESSAGE.LGSC.LGSC_ER_001)
    }
  })
}

const onRegister = () => {
  if (!valid.value) {
    (formRef.value as any)?.validate?.()
    return
  }
  
  loading.value = true
  
  setTimeout(() => {
    loading.value = false
    isLogin.value = true
    resetForm()
  }, 1000)
}

const toggleForm = () => {
  resetForm()
  isLogin.value = !isLogin.value
}

const resetForm = () => {
  username.value = ''
  password.value = ''
  confirmPassword.value = ''
  rememberMe.value = false
  showPassword.value = false
  showConfirmPassword.value = false
  valid.value = false
  
  // Reset form validation
  setTimeout(() => {
    (formRef.value as any)?.resetValidation?.()
  }, 100)
}

const forgotPassword = () => {
  // dialog.info('Tính năng quên mật khẩu đang được phát triển')
}
</script>

<style scoped>
.user-name-input :deep(.v-field__input) {
  @apply rounded-e-xl !important;
}

/* Main Background */
.login-background {
  background: linear-gradient(135deg, #f8fafc 0%, #f1f5f9 25%, #e2e8f0 75%, #cbd5e1 100%);
  position: relative;
  overflow: hidden;
}

/* Floating Shapes Animation */
.floating-shapes {
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  overflow: hidden;
  z-index: 1;
}

.shape {
  position: absolute;
  border-radius: 50%;
  background: linear-gradient(135deg, rgba(17, 195, 147, 0.1), rgba(59, 130, 246, 0.05));
  animation: float 20s infinite ease-in-out;
}

.shape-1 {
  width: 120px;
  height: 120px;
  top: 10%;
  left: 10%;
  animation-delay: 0s;
  background: linear-gradient(135deg, rgba(17, 195, 147, 0.08), transparent);
}

.shape-2 {
  width: 80px;
  height: 80px;
  top: 20%;
  right: 15%;
  animation-delay: -5s;
  background: linear-gradient(135deg, rgba(17, 195, 147, 0.06), rgba(34, 197, 94, 0.04));
}

.shape-3 {
  width: 160px;
  height: 160px;
  bottom: 20%;
  left: 15%;
  animation-delay: -10s;
  background: linear-gradient(135deg, rgba(17, 195, 147, 0.05), transparent);
}

.shape-4 {
  width: 100px;
  height: 100px;
  bottom: 30%;
  right: 20%;
  animation-delay: -15s;
  background: linear-gradient(135deg, rgba(17, 195, 147, 0.07), rgba(16, 185, 129, 0.03));
}

.shape-5 {
  width: 60px;
  height: 60px;
  top: 50%;
  left: 5%;
  animation-delay: -8s;
  background: linear-gradient(135deg, rgba(17, 195, 147, 0.04), transparent);
}

.shape-6 {
  width: 140px;
  height: 140px;
  top: 60%;
  right: 10%;
  animation-delay: -12s;
  background: linear-gradient(135deg, rgba(17, 195, 147, 0.06), rgba(6, 182, 212, 0.02));
}

@keyframes float {
  0%, 100% {
    transform: translateY(0px) translateX(0px) rotate(0deg);
    opacity: 0.5;
  }
  25% {
    transform: translateY(-20px) translateX(10px) rotate(5deg);
    opacity: 0.8;
  }
  50% {
    transform: translateY(-10px) translateX(-15px) rotate(-3deg);
    opacity: 0.6;
  }
  75% {
    transform: translateY(-15px) translateX(5px) rotate(2deg);
    opacity: 0.9;
  }
}

/* Grid Pattern */
.grid-pattern {
  width: 100%;
  height: 100%;
  background-image: 
    linear-gradient(rgba(17, 195, 147, 0.1) 1px, transparent 1px),
    linear-gradient(90deg, rgba(17, 195, 147, 0.1) 1px, transparent 1px);
  background-size: 50px 50px;
  animation: gridMove 30s linear infinite;
}

@keyframes gridMove {
  0% {
    transform: translate(0, 0);
  }
  100% {
    transform: translate(50px, 50px);
  }
}

/* Login Card - NO HOVER EFFECTS */
.login-card {
  background: rgba(255, 255, 255, 0.98);
  backdrop-filter: blur(20px);
  box-shadow: 
    0 25px 50px -12px rgba(0, 0, 0, 0.08),
    0 0 0 1px rgba(17, 195, 147, 0.05),
    inset 0 1px 0 rgba(255, 255, 255, 0.1);
  border: 1px solid rgba(17, 195, 147, 0.1);
  /* NO transition for hover effects */
}

.login-header {
  background: linear-gradient(135deg, rgba(255, 255, 255, 0.9), rgba(248, 250, 252, 0.8));
}

.login-content {
  background: rgba(255, 255, 255, 0.95);
}

.login-footer {
  background: linear-gradient(135deg, rgba(248, 250, 252, 0.9), rgba(241, 245, 249, 0.8));
  border-radius: 0 0 12px 12px;
}

/* Logo Avatar */
.logo-avatar {
  background: linear-gradient(135deg, #11c393 0%, #10b981 50%, #059669 100%);
  box-shadow: 
    0 8px 16px rgba(17, 195, 147, 0.3),
    0 0 0 4px rgba(17, 195, 147, 0.1);
  position: relative;
}

.logo-avatar::before {
  content: '';
  position: absolute;
  inset: -2px;
  background: linear-gradient(135deg, #11c393, #10b981);
  border-radius: 50%;
  z-index: -1;
  opacity: 0.2;
  animation: pulse 2s infinite;
}

@keyframes pulse {
  0%, 100% {
    opacity: 0.2;
    transform: scale(1);
  }
  50% {
    opacity: 0.4;
    transform: scale(1.05);
  }
}

/* Form Styling */
.form-group {
  margin-bottom: 1.5rem;
}

.form-label {
  display: flex;
  align-items: center;
  font-size: 0.875rem;
  font-weight: 500;
  color: #374151;
  margin-bottom: 0.5rem;
}

.label-icon {
  color: #11c393 !important;
}

/* Completely redesigned Input Field States */
.input-field :deep(.v-field) {
  border-radius: 12px;
  /* Start with border instead of shadow for consistent base state */
  border: 1.5px solid #e5e7eb !important;
  background: rgba(255, 255, 255, 0.9);
  /* Smooth transition for all states */
  transition: border-color 0.2s ease, box-shadow 0.2s ease, background-color 0.2s ease, transform 0.2s ease;
  outline: none !important;
}

/* Completely hide Vuetify's default outline system */
.input-field :deep(.v-field__outline),
.input-field :deep(.v-field__outline--start),
.input-field :deep(.v-field__outline--notched),
.input-field :deep(.v-field__outline--end) {
  display: none !important;
  opacity: 0 !important;
  border: none !important;
}

/* Hover State - subtle enhancement */
.input-field :deep(.v-field:hover:not(.v-field--focused)) {
  border-color: #d1d5db !important;
  background: rgba(255, 255, 255, 1);
}

/* Focus State - smooth transition to shadow-based styling */
.input-field :deep(.v-field--focused) {
  /* Transition from border to shadow seamlessly */
  border-color: transparent !important;
  background: rgba(255, 255, 255, 1);
  box-shadow: 
    0 0 0 2px rgba(17, 195, 147, 0.2),
    0 0 0 4px rgba(17, 195, 147, 0.08),
    0 4px 16px rgba(17, 195, 147, 0.1) !important;
  transform: translateY(-1px);
}

/* Remove any browser default styling */
.input-field :deep(.v-field__input input),
.input-field :deep(.v-field__input) {
  outline: none !important;
  border: none !important;
  color: #1f2937;
  font-size: 0.875rem;
  padding: 14px 16px;
  min-height: 48px !important;
}

.input-field :deep(.v-field__prepend-inner) {
  padding-right: 12px;
}

.input-field :deep(.v-field__append-inner) {
  padding-left: 12px;
}

/* Icon color transitions */
.input-field :deep(.v-field__prepend-inner .v-icon) {
  transition: color 0.2s ease, transform 0.2s ease;
}

.input-field:focus-within :deep(.v-field__prepend-inner .v-icon) {
  color: #11c393 !important;
  transform: scale(1.05);
}

/* Error State - consistent with focus pattern */
.input-field :deep(.v-field--error) {
  border-color: transparent !important;
  box-shadow: 
    0 0 0 2px rgba(239, 68, 68, 0.3),
    0 0 0 4px rgba(239, 68, 68, 0.08),
    0 4px 16px rgba(239, 68, 68, 0.1) !important;
}

/* Success State - consistent with focus pattern */
.input-field :deep(.v-field--valid:not(.v-field--focused)) {
  border-color: rgba(16, 185, 129, 0.3) !important;
}

/* Checkbox Styling */
.remember-checkbox :deep(.v-selection-control__wrapper) {
  height: 20px;
  width: 20px;
}

.remember-checkbox :deep(.v-label) {
  font-size: 0.875rem;
  color: #6b7280;
  opacity: 1;
}

/* Button Styling */
.submit-btn {
  font-weight: 600;
  border-radius: 12px;
  min-height: 52px !important;
  background: linear-gradient(135deg, #11c393 0%, #10b981 50%, #059669 100%);
  transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
  position: relative;
  overflow: hidden;
}

.submit-btn::before {
  content: '';
  position: absolute;
  inset: 0;
  background: linear-gradient(135deg, rgba(255, 255, 255, 0.2), transparent);
  opacity: 0;
  transition: opacity 0.3s ease;
}

.submit-btn:hover::before {
  opacity: 1;
}

.submit-btn:hover {
  box-shadow: 
    0 8px 25px rgba(17, 195, 147, 0.4),
    0 4px 12px rgba(17, 195, 147, 0.3);
  transform: translateY(-2px);
}

.submit-btn:active {
  transform: translateY(0px);
}

.toggle-btn {
  font-weight: 500;
  border-radius: 12px;
  min-height: 48px !important;
  border: 1.5px solid #11c393;
  color: #11c393;
  transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
}

.toggle-btn:hover {
  background: rgba(17, 195, 147, 0.05);
  border-color: #10b981;
  transform: translateY(-1px);
  box-shadow: 0 4px 12px rgba(17, 195, 147, 0.2);
}

.forgot-password-btn {
  color: #11c393 !important;
  font-weight: 500;
}

.forgot-password-btn:hover {
  background: rgba(17, 195, 147, 0.05) !important;
}

/* Loading and Disabled States */
.v-btn--loading {
  pointer-events: none;
}

.v-btn:disabled {
  opacity: 0.6;
  transform: none !important;
  box-shadow: none !important;
}

/* Transitions */
.v-slide-x-transition-enter-active,
.v-slide-x-transition-leave-active {
  transition: all 0.4s cubic-bezier(0.25, 0.8, 0.25, 1);
}

.v-slide-x-transition-enter-from {
  opacity: 0;
  transform: translateX(-30px) scale(0.95) rotateY(-5deg);
}

.v-slide-x-transition-leave-to {
  opacity: 0;
  transform: translateX(30px) scale(0.95) rotateY(5deg);
}

/* Remove all control elements that might interfere */
.input-field :deep(.v-input__control),
.input-field :deep(.v-input__slot),
.input-field :deep(.v-field__overlay) {
  outline: none !important;
  border: none !important;
}

/* Custom Scrollbar */
.login-content::-webkit-scrollbar {
  width: 4px;
}

.login-content::-webkit-scrollbar-track {
  background: #f1f5f9;
  border-radius: 2px;
}

.login-content::-webkit-scrollbar-thumb {
  background: #11c393;
  border-radius: 2px;
}

.login-content::-webkit-scrollbar-thumb:hover {
  background: #10b981;
}

/* Responsive Design */
@media (max-width: 640px) {
  .login-card {
    margin: 1rem;
    max-width: calc(100% - 2rem);
  }
  
  .login-header,
  .login-content,
  .login-footer {
    padding-left: 1.5rem;
    padding-right: 1.5rem;
    padding-top: 1rem;
    padding-bottom: 1rem;
  }
  
  .form-group {
    margin-bottom: 1rem;
  }
  
  .input-field :deep(.v-field__input) {
    padding: 12px 14px;
    min-height: 44px !important;
  }
  
  .submit-btn {
    min-height: 48px !important;
  }
  
  .toggle-btn {
    min-height: 44px !important;
  }
  
  /* Reduce floating shapes on mobile */
  .shape {
    opacity: 0.3;
  }
  
  .shape-1, .shape-3 {
    display: none;
  }
}

/* Accessibility */
@media (prefers-reduced-motion: reduce) {
  .floating-shapes .shape {
    animation: none;
  }
  
  .grid-pattern {
    animation: none;
  }
  
  .logo-avatar::before {
    animation: none;
  }
  
  /* Keep input transitions even with reduced motion for better UX */
  .input-field :deep(.v-field) {
    transition: border-color 0.1s ease, box-shadow 0.1s ease !important;
  }
}

/* High contrast mode */
@media (prefers-contrast: high) {
  .login-card {
    border: 2px solid #11c393;
    background: rgba(255, 255, 255, 1);
  }
  
  .input-field :deep(.v-field) {
    border-width: 2px !important;
  }
  
  .input-field :deep(.v-field--focused) {
    box-shadow: 
      0 0 0 3px rgba(17, 195, 147, 0.5) !important;
  }
}

/* Focus-visible for keyboard navigation */
.input-field :deep(.v-field:focus-visible) {
  box-shadow: 
    0 0 0 2px rgba(17, 195, 147, 0.4),
    0 0 0 4px rgba(17, 195, 147, 0.12),
    0 4px 20px rgba(17, 195, 147, 0.15) !important;
}
</style>