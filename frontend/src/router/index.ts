import { createRouter, createWebHistory } from 'vue-router'
import HomeView from '@/views/HomeView.vue'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    // --- ПУБЛИЧНЫЕ СТРАНИЦЫ ---
    {
      path: '/',
      name: 'home',
      component: HomeView,
      meta: { hideLayout: false },
    },
    {
      path: '/login',
      name: 'login',
      component: () => import('@/views/LoginView.vue'),
      meta: { hideLayout: true },
    },
    {
      path: '/register',
      name: 'register',
      // Исправил путь с '../' на '@/' для единообразия
      component: () => import('@/views/RegistrationView.vue'),
      meta: { hideLayout: true },
    },

    // --- ЛИЧНЫЙ КАБИНЕТ СТУДЕНТА ---
    {
      path: '/student',
      component: () => import('@/layouts/StudentLayout.vue'),
      redirect: '/student/invitations',
      meta: { requiresAuth: true, role: 'student' },
      children: [
        {
          path: 'invitations',
          name: 'student-invitations',
          // ИСПОЛЬЗУЕМ ОБЩИЙ КОМПОНЕНТ
          component: () => import('@/views/InvitationsView.vue'),
        },
        {
          path: 'messages',
          name: 'student-messages',
          // Если сообщения тоже унифицированы, ссылаемся на общий файл
          // Если пока нет - оставь заглушку или старый путь
          component: () => import('@/views/student/MessagesView.vue'),
        },
        {
          path: 'favorites',
          name: 'student-favorites',
          component: () => import('@/views/student/FavoritesView.vue'),
        },
        {
          path: 'profile',
          name: 'student-profile',
          component: () => import('@/views/student/ProfileSettingsView.vue'),
        },
        {
          path: 'resume',
          name: 'student-resume', // Уникальная страница студента
          component: () => import('@/views/student/ResumeView.vue'),
        },
      ],
    },

    // --- ЛИЧНЫЙ КАБИНЕТ РАБОТОДАТЕЛЯ (НОВОЕ) ---
    {
      path: '/employer',
      component: () => import('@/layouts/EmployerLayout.vue'), // Убедись, что файл создан
      redirect: '/employer/invitations',
      meta: { requiresAuth: true, role: 'employer' },
      children: [
        {
          path: 'invitations',
          name: 'employer-invitations',
          // ИСПОЛЬЗУЕМ ТОТ ЖЕ ОБЩИЙ КОМПОНЕНТ!
          component: () => import('@/views/InvitationsView.vue'),
        },
        {
          path: 'messages',
          name: 'employer-messages',
          // Тоже можно использовать общий компонент
          component: () => import('@/views/student/MessagesView.vue'),
        },
        // {
        //   path: 'favorites',
        //   name: 'employer-favorites',
        //   // Скорее всего будет EmployerFavoritesView.vue
        //   component: () => import('@/views/employer/FavoritesView.vue'),
        // },
        // {
        //   path: 'profile',
        //   name: 'employer-profile',
        //   // Специфичный профиль компании
        //   component: () => import('@/views/employer/CompanyProfileView.vue'),
        // },
        // {
        //   path: 'vacancies',
        //   name: 'employer-vacancies', // Аналог "Резюме" у студента
        //   component: () => import('@/views/employer/MyVacanciesView.vue'),
        // },
      ],
    },
  ],
})

export default router
