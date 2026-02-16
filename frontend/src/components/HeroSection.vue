<script setup lang="ts">
import { computed } from 'vue'
import IconLogo from '@/components/icons/IconLogo.vue'

const siteName = 'StudHunter'
const universityName = 'ЮУрГУ'
const slogan = computed(() => `Работа и стажировки для студентов ${universityName}`)

const stats = {
  resumes: 2847,
  vacancies: 1153,
  employers: 2402,
}
const formatNumber = (num: number) => num.toLocaleString('ru-RU')

const isAuthenticated = !!localStorage.getItem('token')
const userRole = localStorage.getItem('userRole')?.toLowerCase() || ''

const authLink = computed(() => {
  if (!isAuthenticated) return '/login'
  return userRole === 'employer' ? '/employer/profile' : '/student/profile'
})

const authLinkText = computed(() => (isAuthenticated ? 'Личный кабинет' : 'Войти'))

const createResumeLink = isAuthenticated ? '/student/resume' : '/login'
</script>

<template>
  <section class="hero">
    <div class="hero-bg"></div>
    <div class="hero-overlay"></div>

    <div class="container">
      <div class="hero-content">
        <div class="hero-left">
          <div class="hero-logo-row">
            <IconLogo class="hero-logo-icon" />
            <h1 class="hero-title">{{ siteName }}</h1>
          </div>
          <p class="hero-tagline">{{ slogan }}</p>
        </div>

        <div class="hero-right">
          <div class="hero-actions">
            <router-link :to="createResumeLink" class="btn-main btn-dark">
              Создать резюме
            </router-link>

            <router-link :to="authLink" class="btn-main btn-outline">
              {{ authLinkText }}
            </router-link>
          </div>

          <div class="hero-stats">
            <div class="stat-item">
              <span class="stat-value">{{ formatNumber(stats.resumes) }}</span>
              <span class="stat-label">Создано<br />резюме</span>
            </div>
            <div class="stat-item">
              <span class="stat-value">{{ formatNumber(stats.vacancies) }}</span>
              <span class="stat-label">Размещено<br />вакансий</span>
            </div>
            <div class="stat-item">
              <span class="stat-value">{{ formatNumber(stats.employers) }}</span>
              <span class="stat-label">Аккредитованных<br />работодателей</span>
            </div>
          </div>
        </div>
      </div>
    </div>
  </section>
</template>

<style scoped>
.hero {
  position: relative;
  height: 350px;
  width: 100%;
  display: flex;
  align-items: center;
  user-select: none;
  overflow: hidden;
}
.hero-content {
  position: relative;
  z-index: 3;
  display: flex;
  justify-content: space-between;
  align-items: flex-end;
  width: 100%;
  padding-bottom: 20px;
}
/* 3. Левая колонка */
.hero-left {
  display: flex;
  flex-direction: column;
  margin-bottom: 20px;
}
.hero-logo-row {
  display: flex;
  align-items: center;
  gap: 16px;
  color: var(--dark-text);
}
.hero-logo-icon {
  width: 48px;
  height: 48px;
}
.hero-title {
  font-size: 56px;
  font-weight: 800;
  margin: 0;
  line-height: 1;
  letter-spacing: -1px;
}
.hero-tagline {
  font-size: 18px;
  font-weight: 500;
  margin: 12px 0 0 0;
  color: var(--dark-text);
}

/* 4. Правая колонка */
.hero-right {
  display: flex;
  flex-direction: column;
  align-items: flex-end;
}

/* Кнопки */
.hero-actions {
  display: flex;
  gap: 15px;
  margin-bottom: 48px;
}

/* Статистика */
.hero-stats {
  display: flex;
  gap: 40px;
}

.stat-item {
  display: flex;
  flex-direction: column;
  align-items: center;
  text-align: center;
}

.stat-value {
  font-size: 24px;
  font-weight: 800;
  color: var(--susu-blue);
  line-height: 1;
  margin-bottom: 4px;
}

.stat-label {
  font-size: 13px;
  line-height: 1.3;
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
/* .hero-overlay {
  background-color: var(--susu-blue);
  opacity: 0.2;
  z-index: 2;
} */
.hero-overlay {
  background: linear-gradient(90deg, rgba(255, 255, 255, 1) 0%, rgba(39, 88, 134, 0.7) 100%);
  opacity: 0.55;
  z-index: 2;
}
</style>
