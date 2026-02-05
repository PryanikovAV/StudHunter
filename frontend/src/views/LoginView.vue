<script setup lang="ts">
import IconBackButton from '@/components/icons/IconBackButton.vue'
import IconLogo from '@/components/icons/IconLogo.vue'

const recoveryLink = 'https://edu.susu.ru/edususudocs/ru/intro/login-and-password'
</script>

<template>
  <div class="login-container">
    <header class="auth-header">
      <div class="header-side">
        <button @click="$router.push('/')" class="back-button">
          <IconBackButton class="icon-back" />
        </button>
      </div>
      <div class="auth-logo">
        <IconLogo class="main-logo" />
      </div>
      <div class="header-side"></div>
    </header>

    <h1 class="auth-title">Вход</h1>

    <form @submit.prevent class="auth-form">
      <div class="input-group">
        <label for="email">Email</label>
        <input id="email" type="email" placeholder="example@mail.ru" class="auth-input" />
      </div>

      <div class="input-group">
        <label for="password">Пароль</label>
        <input id="password" type="password" placeholder="Введите пароль" class="auth-input" />
      </div>

      <a :href="recoveryLink" target="_blank" class="btn-text btn-text-blue forgot-link">
        Восстановить пароль
      </a>

      <div class="auth-actions">
        <button type="submit" class="btn-primary w-full">Войти</button>
        <button type="button" class="btn-outline w-full" @click="$router.push('/register')">
          Зарегистрироваться
        </button>
      </div>
    </form>
  </div>
</template>

<style scoped>
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

.auth-logo {
  display: flex;
  justify-content: center;
}

.main-logo {
  width: 50px;
  height: auto;
}

.auth-title {
  text-align: center;
  font-size: 20px;
  font-weight: 700;
  margin-bottom: 40px;
  color: var(--dark-text);
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
  transition: border-color 0.2s;
}

.auth-input:focus {
  border-color: #275886;
}

.forgot-link {
  font-size: 13px;
  margin-bottom: 32px;
  /* Перебиваем дефолтный стиль ссылки, если он есть */
  text-decoration: none;
}

.auth-actions {
  display: flex;
  flex-direction: column;
  gap: 12px;
}

/* Оставляем только те свойства, которых нет в main.css (ширина и высота) */
.w-full {
  width: 100%;
  height: 48px;
  box-sizing: border-box;
}
</style>
