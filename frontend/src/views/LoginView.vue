<script setup lang="ts">
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { isAxiosError } from 'axios'
import apiClient from '@/api'

import IconBackButton from '@/components/icons/IconBackButton.vue'
import IconLogo from '@/components/icons/IconLogo.vue'

const router = useRouter()
const recoveryLink = 'https://edu.susu.ru/edususudocs/ru/intro/login-and-password'

const email = ref('')
const password = ref('')
const isLoading = ref(false)
const errorText = ref('')

const handleLogin = async () => {
  errorText.value = ''

  if (!email.value.trim()) {
    errorText.value = 'Укажите почту'
    return
  }

  const emailPattern = /^[^\s@]+@[^\s@]+\.[^\s@]+$/
  if (!emailPattern.test(email.value)) {
    errorText.value = 'Некорректный формат email'
    return
  }

  if (!password.value) {
    errorText.value = 'Введите пароль'
    return
  }

  isLoading.value = true

  try {
    const response = await apiClient.post('/auth/login', {
      email: email.value,
      password: password.value,
    })

    const { token, role } = response.data

    localStorage.setItem('token', token)
    localStorage.setItem('userRole', role)

    router.push('/')
  } catch (err: unknown) {
    if (isAxiosError(err)) {
      if (err.response?.status === 401) {
        errorText.value = 'Неверный email или пароль'
      } else {
        errorText.value = 'Ошибка сервера: ' + (err.response?.data?.message || 'попробуйте позже')
      }
    } else {
      errorText.value = 'Произошла ошибка сети'
    }
  } finally {
    isLoading.value = false
  }
}
</script>

<template>
  <div class="auth-wrapper">
    <div class="auth-card">
      <header class="auth-header">
        <button @click="router.push('/')" class="btn-icon-back" type="button">
          <IconBackButton class="icon-main" />
        </button>

        <div class="logo-center">
          <IconLogo class="auth-logo" />
        </div>

        <div class="spacer"></div>
      </header>

      <h1 class="page-title text-center">Вход</h1>

      <form @submit.prevent="handleLogin" class="auth-form" novalidate>
        <div class="inputs-stack">
          <div class="input-group">
            <label for="login-email" class="input-label">Email</label>
            <input
              id="login-email"
              v-model="email"
              type="email"
              placeholder="example@mail.ru"
              class="input-main"
              :class="{ 'input-error': errorText && !email }"
              required
              @input="errorText = ''"
            />
          </div>

          <div class="input-group">
            <label for="login-password" class="input-label">Пароль</label>
            <input
              id="login-password"
              v-model="password"
              type="password"
              placeholder="Введите пароль"
              class="input-main"
              :class="{ 'input-error': errorText && !password }"
              required
              @input="errorText = ''"
            />
          </div>
        </div>

        <div class="error-container" :class="{ visible: errorText }">
          {{ errorText }}
        </div>

        <div class="actions-stack">
          <button type="submit" class="btn-main btn-dark w-full" :disabled="isLoading">
            {{ isLoading ? 'Вход...' : 'Войти' }}
          </button>

          <button
            type="button"
            class="btn-main btn-outline w-full"
            @click="router.push('/register')"
          >
            Зарегистрироваться
          </button>

          <button
            type="button"
            class="btn-main btn-outline w-full"
            @click="router.push('/recover')"
          >
            Восстановить аккаунт
          </button>

          <a :href="recoveryLink" target="_blank" class="btn-main btn-secondary btn-fix">
            Восстановить пароль
          </a>
        </div>
      </form>
    </div>
  </div>
</template>

<style scoped>
.auth-wrapper {
  min-height: 100vh;
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 20px;
  background-color: var(--background-page);
}
.auth-card {
  width: 100%;
  max-width: 320px;
  display: flex;
  flex-direction: column;
}
.auth-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  margin-bottom: 24px;
}
.btn-icon-back {
  background: none;
  border: none;
  padding: 8px;
  cursor: pointer;
  color: var(--dark-text);
  display: flex;
  margin-left: -8px;
}
.logo-center {
  flex: 1;
  display: flex;
  justify-content: center;
}
.auth-logo {
  width: 48px;
  height: 48px;
}
.spacer {
  width: 40px;
}
.text-center {
  text-align: center;
  margin-bottom: 32px;
}
.auth-form {
  display: flex;
  flex-direction: column;
}
.inputs-stack {
  display: flex;
  flex-direction: column;
  gap: 16px;
}
.input-group {
  display: flex;
  flex-direction: column;
  gap: 6px;
}
.input-label {
  font-size: 13px;
  font-weight: 600;
  color: var(--dark-text);
  padding-left: 12px;
}
.error-container {
  min-height: 32px;
  margin-top: 12px;
  margin-bottom: 8px;
  font-size: 13px;
  color: var(--red-text-error);
  padding-left: 12px;
  opacity: 0;
  transition: opacity 0.2s ease;
  word-wrap: break-word;
}
.error-container.visible {
  opacity: 1;
}
.input-error {
  border-color: var(--red-text-error) !important;
}
.actions-stack {
  display: flex;
  flex-direction: column;
  gap: 12px;
}
.w-full {
  width: 100%;
}
.btn-fix {
  justify-content: flex-start;
}
button:disabled {
  opacity: 0.7;
  cursor: wait;
}
</style>
