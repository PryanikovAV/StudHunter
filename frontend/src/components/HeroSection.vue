<script setup lang="ts">
import IconLogo from '@/components/icons/IconLogo.vue'
const SiteName = 'StudHunter'
const Slogan = 'Работа и стажировки для студентов ЮУрГУ'
const NumberOfResumes = 2847
const JobVacanciesPosted = 1153
const NumberOfEmployers = 2402
</script>

<template>
  <section class="hero">
    <div class="hero-bg"></div>
    <div class="hero-overlay"></div>

    <div class="container">
      <div class="hero-content">
        <div class="hero-left">
          <div class="hero-logo-row">
            <IconLogo />
            <h1>{{ SiteName }}</h1>
          </div>
          <p class="hero-tagline">{{ Slogan }}</p>
        </div>

        <div class="hero-right">
          <div class="hero-actions">
            <button class="btn-primary">Создать резюме</button>
            <router-link
              to="/login"
              class="btn-outline"
              style="text-decoration: none; display: inline-flex; align-items: center"
            >
              Войти
            </router-link>
          </div>

          <div class="hero-stats">
            <div class="stat-item">
              <span class="stat-value">{{ NumberOfResumes }}</span>
              <span class="stat-label">Создано<br />резюме</span>
            </div>
            <div class="stat-item">
              <span class="stat-value">{{ JobVacanciesPosted }}</span>
              <span class="stat-label">Размещено<br />вакансий</span>
            </div>
            <div class="stat-item">
              <span class="stat-value">{{ NumberOfEmployers }}</span>
              <span class="stat-label">Аккредитованных<br />работодателей</span>
            </div>
          </div>
        </div>
      </div>
    </div>
  </section>
</template>

<style scoped>
/* 1. Основа секции */
.hero {
  position: relative;
  height: 350px;
  width: 100%;
  display: flex;
  align-items: center; /* Центрирует всё содержимое по вертикали */
  user-select: none;
  -webkit-user-select: none;
}

/* 2. Главный ряд (Auto Layout: Horizontal, Space Between) */
.hero-content {
  position: relative;
  z-index: 3;
  display: flex;
  justify-content: space-between; /* Лево в левый угол, право в правый */
  align-items: flex-end; /* Все прижимаются к "полу" секции */
  width: 100%;
}

/* 3. Левая колонка */
.hero-left {
  display: flex;
  flex-direction: column;
}

.hero-logo-row {
  display: flex;
  align-items: center;
  gap: 16px;
  color: var(--dark-text);
}

h1 {
  font-size: 56px;
  font-weight: 800;
  margin: 0;
  line-height: 1;
}

.hero-tagline {
  font-size: 18px;
  font-weight: 500;
  margin-top: 12px;
  color: var(--dark-text);
}

/* 4. Правая колонка */
.hero-right {
  display: flex;
  flex-direction: column;
  align-items: flex-end; /* Все элементы колонки жмутся вправо */
}

/* Кнопки */
.hero-actions {
  display: flex;
  gap: 15px;
  margin-bottom: 55px; /* Отступ между кнопками и статистикой */
}

/* Статистика */
.hero-stats {
  display: flex;
  gap: 40px;
}

.stat-item {
  display: flex;
  flex-direction: column;
  align-items: center; /* Цифры и текст в колонке тоже вправо */
  text-align: center;
}

.stat-value {
  font-size: 24px;
  font-weight: 800;
  color: var(--susu-blue);
}

.stat-label {
  font-size: 13px;
  line-height: 1.2;
  font-weight: 500;
  color: var(--dark-text);
}

/* Фоны */
.hero-bg,
.hero-overlay {
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
}
.hero-bg {
  background: url('@/assets/susu-panorama.png') center/cover no-repeat;
  opacity: 0.2;
  z-index: 1;
}
.hero-overlay {
  background-color: var(--susu-blue);
  opacity: 0.2;
  z-index: 2;
}
</style>
