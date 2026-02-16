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

// Динамический заголовок
const titleText = computed(() =>
  role.value === 'student' ? 'Регистрация студента' : 'Регистрация компании',
)

interface RegisterPayload {
  email: string
  password: string
  name?: string
}

const handleRegister = async () => {
  errorText.value = ''

  // 1. Простые проверки на клиенте
  if (!email.value.trim()) {
    errorText.value = 'Укажите email'
    return
  }

  if (role.value === 'employer' && !companyName.value.trim()) {
    errorText.value = 'Укажите название компании'
    return
  }

  if (!password.value) {
    errorText.value = 'Придумайте пароль'
    return
  }

  if (password.value.length < 8) {
    errorText.value = 'Пароль слишком короткий (мин. 8)'
    return
  }

  if (password.value !== confirmPassword.value) {
    errorText.value = 'Пароли не совпадают'
    return
  }

  isLoading.value = true

  try {
    const isStudent = role.value === 'student'
    const endpoint = isStudent ? 'register/student' : 'register/employer'

    const payload: RegisterPayload = {
      email: email.value,
      password: password.value,
    }
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
      errorText.value = err.response?.data?.detail || 'Ошибка регистрации'
    } else {
      errorText.value = 'Ошибка сети'
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

      <h1 class="page-title text-center">{{ titleText }}</h1>

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

      <form @submit.prevent="handleRegister" class="auth-form" novalidate>
        <div class="inputs-stack">
          <div class="input-group">
            <label for="reg-email" class="input-label">Email</label>
            <input
              id="reg-email"
              v-model="email"
              type="email"
              placeholder="example@mail.ru"
              class="input-main"
              :class="{ 'input-error': errorText && !email }"
              required
              @input="errorText = ''"
            />
          </div>

          <Transition name="slide">
            <div v-if="role === 'employer'" class="input-group">
              <label for="reg-company" class="input-label">Название компании</label>
              <input
                id="reg-company"
                v-model="companyName"
                type="text"
                placeholder="ООО 'Технологии'"
                class="input-main"
                :class="{ 'input-error': errorText && !companyName }"
                @input="errorText = ''"
              />
            </div>
          </Transition>

          <div class="input-group">
            <label for="reg-password" class="input-label">Пароль</label>
            <input
              id="reg-password"
              v-model="password"
              type="password"
              placeholder="Минимум 8 символов"
              class="input-main"
              :class="{ 'input-error': errorText && !password }"
              required
              @input="errorText = ''"
            />
          </div>

          <div class="input-group">
            <label for="reg-confirm" class="input-label">Повторите пароль</label>
            <input
              id="reg-confirm"
              v-model="confirmPassword"
              type="password"
              placeholder="Повторите пароль"
              class="input-main"
              :class="{ 'input-error': errorText && password !== confirmPassword }"
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
            {{ isLoading ? 'Создание...' : 'Создать аккаунт' }}
          </button>

          <button type="button" class="btn-main btn-outline w-full" @click="router.push('/login')">
            У меня есть аккаунт
          </button>
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

.role-selector {
  display: flex;
  background: var(--gray-border);
  padding: 4px;
  border-radius: 100px;
  margin-bottom: 24px;
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
  color: var(--gray-text);
  transition: all 0.2s ease;
}

.role-btn.active {
  background: var(--background-field);
  color: var(--dark-text);
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.05);
}

.text-center {
  text-align: center;
  font-size: 20px;
  margin-bottom: 24px;
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

.input-error {
  border-color: var(--red-text-error) !important;
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

.actions-stack {
  display: flex;
  flex-direction: column;
  gap: 12px;
}

.w-full {
  width: 100%;
}

button:disabled {
  opacity: 0.7;
  cursor: wait;
}

.slide-enter-active,
.slide-leave-active {
  transition: all 0.3s ease;
  max-height: 100px;
  opacity: 1;
  overflow: hidden;
}

.slide-enter-from,
.slide-leave-to {
  max-height: 0;
  opacity: 0;
  margin-top: -16px;
}
</style>
