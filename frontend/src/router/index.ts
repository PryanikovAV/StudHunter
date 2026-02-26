import { createRouter, createWebHistory } from 'vue-router'
import HomeView from '@/views/HomeView.vue'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
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
      path: '/recover',
      name: 'recover',
      component: () => import('@/views/RecoverView.vue'),
      meta: { hideLayout: true },
    },
    {
      path: '/register',
      name: 'register',
      component: () => import('@/views/RegistrationView.vue'),
      meta: { hideLayout: true },
    },
    {
      path: '/vacancies/:id',
      name: 'vacancy-public',
      component: () => import('@/views/public/VacancyPublicView.vue'),
    },
    {
      path: '/students/:id',
      name: 'student-public',
      component: () => import('@/views/public/StudentPublicView.vue'),
    },
    {
      path: '/employers/:id',
      name: 'employer-public',
      component: () => import('@/views/public/EmployerPublicView.vue'),
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
          component: () => import('@/views/InvitationsView.vue'),
        },
        {
          path: 'messages',
          name: 'student-messages',
          component: () => import('@/views/student/MessagesView.vue'),
        },
        {
          path: 'favorites',
          name: 'student-favorites',
          component: () => import('@/views/FavoritesView.vue'),
        },
        {
          path: 'profile',
          name: 'student-profile',
          component: () => import('@/views/student/ProfileSettingsView.vue'),
        },
        {
          path: 'resume',
          name: 'student-resume',
          component: () => import('@/views/student/ResumeView.vue'),
        },
      ],
    },

    // --- ЛИЧНЫЙ КАБИНЕТ РАБОТОДАТЕЛЯ ---
    {
      path: '/employer',
      component: () => import('@/layouts/EmployerLayout.vue'),
      redirect: '/employer/invitations',
      meta: { requiresAuth: true, role: 'employer' },
      children: [
        {
          path: 'invitations',
          name: 'employer-invitations',
          component: () => import('@/views/InvitationsView.vue'),
        },
        {
          path: 'messages',
          name: 'employer-messages',
          component: () => import('@/views/student/MessagesView.vue'),
        },
        {
          path: 'favorites',
          name: 'employer-favorites',
          component: () => import('@/views/FavoritesView.vue'),
        },
        {
          path: 'profile',
          name: 'employer-profile',
          component: () => import('@/views/employer/ProfileSettingsView.vue'),
        },
        {
          path: 'vacancies',
          name: 'employer-vacancies',
          component: () => import('@/views/employer/MyVacanciesView.vue'),
        },
        {
          path: 'vacancies/create',
          name: 'employer-vacancy-create',
          component: () => import('@/views/employer/VacancyEditView.vue'),
        },
        {
          path: 'vacancies/:id/edit',
          name: 'employer-vacancy-edit',
          component: () => import('@/views/employer/VacancyEditView.vue'),
        },
        // {
        //   // Маршрут для результатов поиска (который вызываем из SearchBar)
        //   path: 'search',
        //   name: 'employer-resume-search',
        //   component: () => import('@/views/employer/ResumeSearchView.vue'),
        // },
      ],
    },
  ],
})

export default router
