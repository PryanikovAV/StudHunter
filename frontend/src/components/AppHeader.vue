<script setup lang="ts">
import { computed, ref, onMounted, watch } from 'vue'
import { useRouter, useRoute } from 'vue-router'

// Импорт иконок
import IconMapPin from '@/components/icons/IconMapPin.vue'
import IconBuilding from '@/components/icons/IconBuilding.vue'
import IconUser from '@/components/icons/IconUser.vue'
import IconResume from '@/components/icons/IconResume.vue'
import IconVacancies from '@/components/icons/IconVacancies.vue'
import IconInvitations from '@/components/icons/IconInvitations.vue'
import IconSignOut from '@/components/icons/IconSignOut.vue'

const router = useRouter()
const route = useRoute()

// Состояние авторизации
const isAuth = ref(false)
const userRole = ref<string>('')

// Проверяем, главная ли это страница
const isHomePage = computed(() => route.path === '/')

// Функция обновления данных из localStorage
const checkAuth = () => {
  const token = localStorage.getItem('token')
  const role = localStorage.getItem('userRole')
  isAuth.value = !!token
  userRole.value = role ? role.toLowerCase() : ''
}

// Проверяем авторизацию при монтировании и при смене маршрута
onMounted(checkAuth)
watch(() => route.path, checkAuth)

// Логика выхода
const handleLogout = () => {
  localStorage.removeItem('token')
  localStorage.removeItem('userRole')
  isAuth.value = false
  userRole.value = ''
  router.push('/login')
}

// Проверка активности ссылки для "переключателя" на черном хедере
const isActive = (path: string) => route.path.startsWith(path)
</script>

<template>
  <header :class="['header-outer', isHomePage ? 'theme-light' : 'theme-dark']">
    <div class="container header-inner">
      <div class="left-group">
        <div class="info-item">
          <IconMapPin class="header-icon" />
          <span>Челябинск</span>
        </div>
        <div class="info-item">
          <IconBuilding class="header-icon" />
          <span>ЮУрГУ</span>
        </div>
      </div>

      <div class="right-group">
        <template v-if="isHomePage">
          <button class="nav-btn" @click="router.push(isAuth ? '/profile' : '/login')">
            <IconUser class="header-icon" />
            <span>Профиль</span>
          </button>
        </template>

        <template v-else>
          <nav class="internal-nav">
            <button
              class="nav-btn"
              :class="{ active: isActive('/profile') }"
              @click="router.push('/profile')"
            >
              <IconUser class="header-icon" />
              <span>Профиль</span>
            </button>

            <button
              v-if="userRole === 'student'"
              class="nav-btn"
              :class="{ active: isActive('/resume') }"
              @click="router.push('/resume')"
            >
              <IconResume class="header-icon" />
              <span>Резюме</span>
            </button>

            <button
              v-else-if="userRole === 'employer'"
              class="nav-btn"
              :class="{ active: isActive('/vacancies') }"
              @click="router.push('/vacancies')"
            >
              <IconVacancies class="header-icon" />
              <span>Вакансии</span>
            </button>

            <button
              class="nav-btn"
              :class="{ active: isActive('/invitations') }"
              @click="router.push('/invitations')"
            >
              <IconInvitations class="header-icon" />
              <span>Отклики</span>
            </button>

            <div class="divider"></div>
            <button class="nav-btn btn-logout" @click="handleLogout">
              <IconSignOut class="header-icon" />
              <span>Выйти</span>
            </button>
          </nav>
        </template>
      </div>
    </div>
  </header>
</template>

<style scoped>
/* --- Базовые стили контейнера --- */
.header-outer {
  width: 100%;
  height: 64px; /* Чуть увеличили высоту для солидности */
  display: flex;
  align-items: center;
  transition:
    background-color 0.3s ease,
    color 0.3s ease;
  user-select: none;
}

.header-inner {
  display: flex;
  justify-content: space-between;
  align-items: center;
  width: 100%;
  padding: 0 24px;
}

/* --- Темы --- */
/* Светлая тема (Главная) */
.theme-light {
  background-color: #ffffff;
  color: #1e293b; /* Темно-серый текст */
  border-bottom: 1px solid #e2e8f0;
}

/* Темная тема (Внутренние страницы) */
.theme-dark {
  background-color: #0f172a; /* Глубокий черный/синий */
  color: #94a3b8; /* Светло-серый текст для неактивных */
  border-bottom: none;
}

/* --- Элементы --- */
.left-group,
.right-group,
.internal-nav {
  display: flex;
  align-items: center;
  gap: 24px;
}

.info-item {
  display: flex;
  align-items: center;
  gap: 8px;
  font-weight: 500;
  font-size: 14px;
}

/* Кнопки навигации */
.nav-btn {
  background: none;
  border: none;
  cursor: pointer;
  display: flex;
  align-items: center;
  gap: 8px;
  font-size: 14px;
  font-weight: 500;
  padding: 8px 12px;
  border-radius: 8px;
  transition: all 0.2s ease;
  color: inherit; /* Наследует цвет от темы */
}

.header-icon {
  width: 20px;
  height: 20px;
  display: block;
  /* Важно: иконки будут краситься в цвет текста */
  fill: currentColor;
}

/* Стили для светлой темы */
.theme-light .nav-btn:hover {
  background-color: #f1f5f9;
  color: #0f172a;
}

/* Стили для темной темы (Активные и Ховер) */
.theme-dark .nav-btn:hover {
  color: #ffffff;
}

.theme-dark .nav-btn.active {
  color: #ffffff; /* Активный пункт становится ярко-белым */
  background-color: rgba(255, 255, 255, 0.1); /* Легкая подсветка */
}

/* Кнопка выхода */
.btn-logout {
  opacity: 0.7;
}
.btn-logout:hover {
  opacity: 1;
  color: #f87171 !important; /* Красный оттенок при наведении */
}

.divider {
  width: 1px;
  height: 24px;
  background-color: rgba(255, 255, 255, 0.2);
}
</style>
