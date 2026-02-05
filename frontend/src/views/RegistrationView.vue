<script setup lang="ts">
import { ref, computed } from 'vue'
import { useRouter } from 'vue-router'
import IconBackButton from '@/components/icons/IconBackButton.vue'
import IconLogo from '@/components/icons/IconLogo.vue'

const router = useRouter()

const role = ref<'student' | 'employer'>('student')

const titleText = computed(() =>
  role.value === 'student' ? 'Регистрация для поиска работы' : 'Регистрация для поиска сотрудников',
)
</script>

<template>
  <div class="login-container">
    <header class="auth-header">
      <div class="header-side">
        <button @click="router.push('/')" class="back-button">
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

    <form @submit.prevent class="auth-form">
      <div class="input-group">
        <label>Email *</label>
        <input type="email" placeholder="example@mail.ru" class="auth-input" required />
      </div>

      <div class="input-group">
        <label>Телефон</label>
        <input type="tel" placeholder="+7 (___) ___-__-__" class="auth-input" />
      </div>

      <div class="input-group">
        <label>Пароль *</label>
        <input type="password" placeholder="Минимум 8 символов" class="auth-input" required />
      </div>

      <div class="input-group">
        <label>Повтор пароля *</label>
        <input type="password" placeholder="Повторите пароль" class="auth-input" required />
      </div>

      <div class="auth-actions">
        <button type="submit" class="btn-primary w-full">Создать аккаунт</button>
        <button type="button" class="btn-outline w-full" @click="router.push('/login')">
          У меня уже есть аккаунт
        </button>
      </div>
    </form>
  </div>
</template>

<style scoped>
/* Копируем базовые стили из LoginView */
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
</style>
