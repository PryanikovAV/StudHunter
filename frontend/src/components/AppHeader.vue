<script setup lang="ts">
import { computed, ref, onMounted, watch } from 'vue'
import { useRouter, useRoute } from 'vue-router'

import IconMapPin from '@/components/icons/IconMapPin.vue'
import IconBuilding from '@/components/icons/IconBuilding.vue'
import IconUser from '@/components/icons/IconUser.vue'
import IconResume from '@/components/icons/IconResume.vue'
import IconInvitations from '@/components/icons/IconInvitations.vue'
import IconSignOut from '@/components/icons/IconSignOut.vue'
import IconChat from '@/components/icons/IconChat.vue'
import IconFavorites from '@/components/icons/IconFavorites.vue'
import IconVacancies from '@/components/icons/IconVacancies.vue'
import IconLogo from '@/components/icons/IconLogo.vue'

const CURRENT_CITY = 'Челябинск'
const CURRENT_UNIVERSITY = 'ЮУрГУ'

const router = useRouter()
const route = useRoute()

const isAuth = ref(false)
const userRole = ref<string>('')

const isHomePage = computed(() => route.path === '/')

const checkAuth = () => {
  const token = localStorage.getItem('token')
  const role = localStorage.getItem('userRole')
  isAuth.value = !!token
  userRole.value = role ? role.toLowerCase() : ''
}

onMounted(checkAuth)
watch(() => route.path, checkAuth)

const handleLogout = () => {
  localStorage.removeItem('token')
  localStorage.removeItem('userRole')
  isAuth.value = false
  userRole.value = ''
  router.push('/login')
}

const isActive = (path: string) => route.path === path || route.path.startsWith(path + '/')
</script>

<template>
  <header :class="['header-outer', isHomePage ? 'theme-light' : 'theme-dark']">
    <div class="container header-inner">
      <div class="left-group">
        <router-link to="/" class="nav-btn logo-btn">
          <IconLogo class="icon-main" />
        </router-link>

        <div class="nav-btn">
          <IconMapPin class="icon-main" />
          <span class="btn-text">{{ CURRENT_CITY }}</span>
        </div>

        <div class="nav-btn">
          <IconBuilding class="icon-main" />
          <span class="btn-text">{{ CURRENT_UNIVERSITY }}</span>
        </div>
      </div>

      <div class="right-group">
        <template v-if="isHomePage">
          <button class="nav-btn" @click="router.push(isAuth ? `/${userRole}/profile` : '/login')">
            <IconUser class="icon-main" />
            <span>{{ isAuth ? 'Профиль' : 'Войти' }}</span>
          </button>
        </template>

        <template v-else>
          <nav class="internal-nav">
            <template v-if="userRole === 'student'">
              <button
                class="nav-btn"
                :class="{ active: isActive('/student/favorites') }"
                @click="router.push('/student/favorites')"
              >
                <IconFavorites class="icon-main" />
                <span class="btn-text">Избранное</span>
              </button>

              <button
                class="nav-btn"
                :class="{ active: isActive('/student/messages') }"
                @click="router.push('/student/messages')"
              >
                <IconChat class="icon-main" />
                <span class="btn-text">Сообщения</span>
              </button>

              <button
                class="nav-btn"
                :class="{ active: isActive('/student/invitations') }"
                @click="router.push('/student/invitations')"
              >
                <IconInvitations class="icon-main" />
                <span class="btn-text">Отклики</span>
              </button>

              <button
                class="nav-btn"
                :class="{ active: isActive('/student/resume') }"
                @click="router.push('/student/resume')"
              >
                <IconResume class="icon-main" />
                <span class="btn-text">Резюме</span>
              </button>

              <button
                class="nav-btn"
                :class="{ active: isActive('/student/profile') }"
                @click="router.push('/student/profile')"
              >
                <IconUser class="icon-main" />
                <span class="btn-text">Профиль</span>
              </button>
            </template>

            <template v-else-if="userRole === 'employer'">
              <button
                class="nav-btn"
                :class="{ active: isActive('/employer/favorites') }"
                @click="router.push('/employer/favorites')"
              >
                <IconFavorites class="icon-main" />
                <span class="btn-text">Избранное</span>
              </button>

              <button
                class="nav-btn"
                :class="{ active: isActive('/employer/messages') }"
                @click="router.push('/employer/messages')"
              >
                <IconChat class="icon-main" />
                <span class="btn-text">Сообщения</span>
              </button>

              <button
                class="nav-btn"
                :class="{ active: isActive('/employer/invitations') }"
                @click="router.push('/employer/invitations')"
              >
                <IconInvitations class="icon-main" />
                <span class="btn-text">Отклики</span>
              </button>

              <button
                class="nav-btn"
                :class="{ active: isActive('/employer/vacancies') }"
                @click="router.push('/employer/vacancies')"
              >
                <IconVacancies class="icon-main" />
                <span class="btn-text">Вакансии</span>
              </button>

              <button
                class="nav-btn"
                :class="{ active: isActive('/employer/profile') }"
                @click="router.push('/employer/profile')"
              >
                <IconUser class="icon-main" />
                <span class="btn-text">Профиль</span>
              </button>
            </template>

            <div class="divider"></div>

            <button class="nav-btn" @click="handleLogout">
              <IconSignOut class="icon-main" />
              <span class="btn-text">Выйти</span>
            </button>
          </nav>
        </template>
      </div>
    </div>
  </header>
</template>

<style scoped>
.header-outer {
  width: 100%;
  height: 64px;
  display: flex;
  align-items: center;
  transition:
    background-color 0.2s ease,
    color 0.2s ease;
  user-select: none;
  z-index: 100;
}

.header-inner {
  display: flex;
  justify-content: space-between;
  align-items: center;
}
.theme-light {
  background-color: var(--background-page);
  color: var(--dark-text);
  border-bottom: 1px solid var(--gray-border);
}
.theme-dark {
  background-color: var(--header-dark-theme);
  color: var(--white-text);
  border-bottom: none;
}
.left-group,
.right-group,
.internal-nav {
  display: flex;
  align-items: center;
  gap: 4px;
}

@media (min-width: 1200px) {
  .internal-nav {
    gap: 12px;
  }
}

/* --- ЕДИНЫЙ СТИЛЬ КНОПОК --- */
.nav-btn {
  background: transparent;
  border: none;
  cursor: pointer;
  display: flex;
  align-items: center;
  gap: 8px;

  /* Размеры и шрифт */
  padding: 8px 12px;
  border-radius: 8px;
  font-size: 14px;
  font-weight: 500;
  white-space: nowrap;
  color: inherit;
  text-decoration: none;
  transition:
    background-color 0.2s ease,
    opacity 0.2s ease;
}
.theme-light .nav-btn:hover {
  background-color: rgba(0, 0, 0, 0.05);
}
.theme-dark .nav-btn:hover,
.theme-dark .nav-btn.active {
  background-color: rgba(255, 255, 255, 0.15);
}
.divider {
  width: 1px;
  height: 24px;
  background-color: currentColor;
  opacity: 0.2;
  margin: 0 8px;
}

@media (max-width: 992px) {
  .btn-text {
    display: none;
  }
}
</style>
