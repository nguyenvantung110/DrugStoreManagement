<!-- src/components/Login.vue -->
<template>
  <v-container class="fill-height">
    <v-row class="justify-center align-center" no-gutters>
      <!-- Hình nền -->
      <v-col cols="12" class="pa-0">
        <div class="login-bg"></div>
      </v-col>

      <!-- Form đăng nhập -->
      <v-col cols="12" sm="8" md="6" lg="4" class="pa-4">
        <v-card elevation="10" class="rounded-lg pa-6">
          <v-card-title class="justify-center text-h5 font-weight-bold mb-4">
            Đăng nhập
          </v-card-title>

          <v-card-text>
            <v-form @submit.prevent="handleLogin">
              <v-text-field
                v-model="email"
                label="Email"
                type="email"
                variant="outlined"
                :error-messages="errors.email"
                @blur="validateEmail"
                class="mb-4"
                density="compact"
              />

              <v-text-field
                v-model="password"
                label="Mật khẩu"
                type="password"
                variant="outlined"
                :error-messages="errors.password"
                @blur="validatePassword"
                class="mb-4"
                density="compact"
              />

              <v-checkbox
                v-model="rememberMe"
                label="Nhớ tôi"
                class="mb-4"
                density="compact"
              />

              <v-btn
                type="submit"
                color="primary"
                block
                size="large"
                class="mb-4"
                :loading="loading"
              >
                Đăng nhập
              </v-btn>

              <div class="text-center">
                <a href="#" class="text-decoration-none text-primary">Quên mật khẩu?</a>
              </div>

              <div class="text-center mt-4">
                Chưa có tài khoản? 
                <a href="#" class="text-decoration-none text-primary">Tạo tài khoản</a>
              </div>
            </v-form>
          </v-card-text>
        </v-card>
      </v-col>
    </v-row>
  </v-container>
</template>

<script setup lang="ts">
import { ref } from 'vue'

// Dữ liệu form
const email = ref('')
const password = ref('')
const rememberMe = ref(false)
const loading = ref(false)

// Lỗi validation
const errors = ref({
  email: '',
  password: ''
})

// Validate email
const validateEmail = () => {
  const re = /^[^\s@]+@[^\s@]+\.[^\s@]+$/
  if (!email.value) {
    errors.value.email = 'Email là bắt buộc'
  } else if (!re.test(email.value)) {
    errors.value.email = 'Email không hợp lệ'
  } else {
    errors.value.email = ''
  }
}

// Validate password
const validatePassword = () => {
  if (!password.value) {
    errors.value.password = 'Mật khẩu là bắt buộc'
  } else if (password.value.length < 6) {
    errors.value.password = 'Mật khẩu ít nhất 6 ký tự'
  } else {
    errors.value.password = ''
  }
}

// Xử lý đăng nhập
const handleLogin = () => {
  // Validate trước
  validateEmail()
  validatePassword()

  if (errors.value.email || errors.value.password) return

  loading.value = true

  // Simulate API call
  setTimeout(() => {
    console.log('Đăng nhập thành công:', { email: email.value, rememberMe: rememberMe.value })
    loading.value = false
    alert('Đăng nhập thành công!')
  }, 1500)
}
</script>

<style scoped>
.login-bg {
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background: linear-gradient(135deg, #4facfe 0%, #00c9ff 100%);
  opacity: 0.9;
  z-index: -1;
  filter: brightness(1.1);
}

/* Responsive */
@media (max-width: 600px) {
  .login-bg {
    background: linear-gradient(135deg, #4facfe 0%, #00c9ff 100%);
  }
}
</style>