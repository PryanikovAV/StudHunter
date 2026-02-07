<script setup lang="ts">
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import axios, { isAxiosError } from 'axios'
import IconBackButton from '@/components/icons/IconBackButton.vue'
import IconLogo from '@/components/icons/IconLogo.vue'

const router = useRouter()
const recoveryLink = 'https://edu.susu.ru/edususudocs/ru/intro/login-and-password'

// 1. Данные формы
const email = ref('')
const password = ref('')
const isLoading = ref(false)
const errorText = ref('')

// 2. Функция входа
const handleLogin = async () => {
  errorText.value = ''
  isLoading.value = true

  try {
    const response = await axios.post('http://localhost:5010/api/v1/auth/login', {
      email: email.value,
      password: password.value,
    })

    // Согласно твоему AuthDto: response.data содержит { id, email, role, token }
    const { token, role } = response.data

    // Сохраняем данные для дальнейшего использования
    localStorage.setItem('token', token)
    localStorage.setItem('userRole', role)

    // Уходим на главную
    router.push('/')
  } catch (err: unknown) {
    // Используем unknown вместо any
    if (isAxiosError(err)) {
      // Безопасно проверяем, что это ошибка Axios
      if (err.response?.status === 401) {
        errorText.value = 'Неверный логин или пароль'
      } else {
        errorText.value = 'Ошибка сервера: ' + (err.response?.data?.message || 'попробуйте позже')
      }
    } else {
      errorText.value = 'Произошла непредвиденная ошибка'
    }
  } finally {
    isLoading.value = false
  }
}
</script>

<template>
  <div class="login-container">
    <header class="auth-header">
      <div class="header-side">
        <button @click="router.push('/')" class="back-button" type="button">
          <IconBackButton class="icon-back" />
        </button>
      </div>
      <div class="auth-logo">
        <IconLogo class="main-logo" />
      </div>
      <div class="header-side"></div>
    </header>

    <h1 class="auth-title">Вход</h1>

    <form @submit.prevent="handleLogin" class="auth-form">
      <div class="input-group">
        <label for="email">Email</label>
        <input
          id="email"
          v-model="email"
          type="email"
          placeholder="example@mail.ru"
          class="auth-input"
          required
        />
      </div>

      <div class="input-group">
        <label for="password">Пароль</label>
        <input
          id="password"
          v-model="password"
          type="password"
          placeholder="Введите пароль"
          class="auth-input"
          required
        />
      </div>

      <p v-if="errorText" class="error-message">{{ errorText }}</p>

      <a :href="recoveryLink" target="_blank" class="btn-text btn-text-blue forgot-link">
        Восстановить пароль
      </a>

      <div class="auth-actions">
        <button type="submit" class="btn-primary w-full" :disabled="isLoading">
          {{ isLoading ? 'Вход...' : 'Войти' }}
        </button>
        <button type="button" class="btn-outline w-full" @click="router.push('/register')">
          Зарегистрироваться
        </button>
      </div>
    </form>
  </div>
</template>

<style scoped>
/* Добавим стиль для ошибки */
.error-message {
  color: #e11d48;
  font-size: 13px;
  margin-top: -10px;
  margin-bottom: 10px;
  padding-left: 4px;
}

/* Все остальные твои стили без изменений... */
.login-container {
  width: 100%;
  max-width: 360px;
  padding: 24px;
  background: #ffffff;
  box-sizing: border-box;
}
.auth-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  margin-bottom: 32px;
}
.header-side {
  width: 40px;
  display: flex;
  align-items: center;
}
.auth-logo {
  display: flex;
  justify-content: center;
  flex: 1;
}
.main-logo {
  width: 50px;
  height: auto;
}
.back-button {
  background: none;
  border: none;
  cursor: pointer;
  padding: 8px;
  color: var(--dark-text);
  margin-left: -8px;
  display: flex;
}
.icon-back {
  width: 24px;
  height: 24px;
}
.auth-title {
  text-align: center;
  font-size: 20px;
  font-weight: 700;
  margin-bottom: 40px;
}
.auth-form {
  display: flex;
  flex-direction: column;
}
.input-group {
  display: flex;
  flex-direction: column;
  gap: 8px;
  margin-bottom: 20px;
}
.input-group label {
  font-size: 14px;
  font-weight: 600;
  color: var(--dark-text);
  padding-left: 4px;
}
.auth-input {
  width: 100%;
  height: 48px;
  padding: 0 20px;
  border-radius: 100px;
  border: 1px solid #cbd5e1;
  font-family: inherit;
  font-size: 14px;
  outline: none;
  box-sizing: border-box;
}
.auth-input:focus {
  border-color: #275886;
}
.forgot-link {
  font-size: 13px;
  margin-bottom: 32px;
  text-decoration: none;
}
.auth-actions {
  display: flex;
  flex-direction: column;
  gap: 12px;
}
.w-full {
  width: 100%;
  height: 48px;
  box-sizing: border-box;
}

/* Состояние disabled для кнопки */
.btn-primary:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}
</style>
