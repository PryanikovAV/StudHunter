<script setup lang="ts">
import { ref, computed } from 'vue'
import { useRouter } from 'vue-router'
import axios, { isAxiosError } from 'axios'
import IconBackButton from '@/components/icons/IconBackButton.vue'
import IconLogo from '@/components/icons/IconLogo.vue'

const router = useRouter()

const role = ref<'student' | 'employer'>('student')
const email = ref('')
const companyName = ref('')
const password = ref('')
const confirmPassword = ref('')
const isLoading = ref(false)
const errorText = ref('')

const titleText = computed(() =>
  role.value === 'student' ? 'Регистрация для поиска работы' : 'Регистрация для поиска сотрудников',
)

interface RegisterPayload {
  email: string
  password: string
  name?: string // Знак вопроса делает поле необязательным
}

const handleRegister = async () => {
  errorText.value = ''

  if (password.value !== confirmPassword.value) {
    errorText.value = 'Пароли не совпадают'
    return
  }

  // Дополнительная проверка для работодателя на фронтенде
  if (role.value === 'employer' && !companyName.value.trim()) {
    errorText.value = 'Введите название компании'
    return
  }

  isLoading.value = true

  try {
    const isStudent = role.value === 'student'
    const endpoint = isStudent ? 'register/student' : 'register/employer'

    // Формируем тело запроса динамически
    const payload: RegisterPayload = {
      email: email.value,
      password: password.value,
    }
    // Если это работодатель, добавляем поле name согласно вашему DTO
    if (!isStudent) {
      payload.name = companyName.value
    }

    const response = await axios.post(`http://localhost:5010/api/v1/auth/${endpoint}`, payload)

    const { token, role: userRole } = response.data
    localStorage.setItem('token', token)
    localStorage.setItem('userRole', userRole)

    router.push('/')
  } catch (err: unknown) {
    if (isAxiosError(err)) {
      errorText.value = err.response?.data?.detail || 'Ошибка при регистрации'
    } else {
      errorText.value = 'Произошла ошибка сети'
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

    <h1 class="auth-title">{{ titleText }}</h1>

    <div class="role-selector">
      <button
        type="button"
        class="role-btn"
        :class="{ active: role === 'student' }"
        @click="role = 'student'"
      >
        Студент
      </button>
      <button
        type="button"
        class="role-btn"
        :class="{ active: role === 'employer' }"
        @click="role = 'employer'"
      >
        Работодатель
      </button>
    </div>

    <form @submit.prevent="handleRegister" class="auth-form">
      <div class="input-group">
        <label>Email *</label>
        <input
          id="reg-email"
          v-model="email"
          type="email"
          placeholder="example@mail.ru"
          class="auth-input"
          required
        />
      </div>

      <Transition name="slide">
        <div v-if="role === 'employer'" class="input-group">
          <label for="reg-company">Название компании *</label>
          <input
            id="reg-company"
            v-model="companyName"
            type="text"
            placeholder="ООО 'Моя Компания'"
            class="auth-input"
            :required="role === 'employer'"
          />
        </div>
      </Transition>

      <div class="input-group">
        <label>Пароль *</label>
        <input
          id="reg-password"
          v-model="password"
          type="password"
          placeholder="Минимум 8 символов"
          class="auth-input"
          required
        />
      </div>

      <div class="input-group">
        <label>Повтор пароля *</label>
        <input
          id="reg-confirm-password"
          v-model="confirmPassword"
          type="password"
          placeholder="Повторите пароль"
          class="auth-input"
          required
        />
      </div>

      <p v-if="errorText" class="error-message">{{ errorText }}</p>

      <div class="auth-actions">
        <button type="submit" class="btn-primary w-full" :disabled="isLoading">
          {{ isLoading ? 'Создание...' : 'Создать аккаунт' }}
        </button>
        <button type="button" class="btn-outline w-full" @click="router.push('/login')">
          У меня уже есть аккаунт
        </button>
      </div>
    </form>
  </div>
</template>

<style scoped>
.error-message {
  color: #e11d48;
  font-size: 13px;
  margin-bottom: 15px;
  padding-left: 4px;
}
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
  display: flex;
  /* Принудительно задаем цвет, если в SVG стоит stroke="currentColor" */
  color: var(--dark-text);
  margin-left: -8px;
}

.icon-back {
  width: 24px;
  height: 24px;
  display: block;
}

.auth-title {
  text-align: center;
  font-size: 18px; /* Чуть меньше, так как текст длиннее */
  font-weight: 700;
  margin-bottom: 32px;
  color: var(--dark-text);
  min-height: 54px; /* Чтобы текст не "прыгал" при смене длины строки */
}

/* Стили переключателя */
.role-selector {
  display: flex;
  background: #f1f5f9;
  padding: 4px;
  border-radius: 100px;
  margin-bottom: 32px;
}

.role-btn {
  flex: 1;
  border: none;
  padding: 10px;
  border-radius: 100px;
  font-size: 14px;
  font-weight: 600;
  cursor: pointer;
  background: transparent;
  color: #64748b;
  transition: all 0.2s ease;
}

.role-btn.active {
  background: #ffffff;
  color: var(--dark-text);
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.05);
}

/* Стили формы */
.auth-form {
  display: flex;
  flex-direction: column;
}
.input-group {
  display: flex;
  flex-direction: column;
  gap: 8px;
  margin-bottom: 16px;
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

.auth-actions {
  display: flex;
  flex-direction: column;
  gap: 12px;
  margin-top: 32px;
}
.w-full {
  width: 100%;
  height: 48px;
  box-sizing: border-box;
}

.slide-enter-active,
.slide-leave-active {
  transition: all 0.3s ease-out;
  max-height: 100px; /* Достаточно для input-group */
  opacity: 1;
}

.slide-enter-from,
.slide-leave-to {
  max-height: 0;
  opacity: 0;
  margin-bottom: 0;
  transform: translateY(-10px);
}
</style>
