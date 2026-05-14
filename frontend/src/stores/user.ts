import { defineStore } from 'pinia'
import apiClient from '@/api'

type UserProfileData = Record<string, unknown>

export const useUserStore = defineStore('user', {
  state: () => ({
    profile: null as UserProfileData | null,
    isLoading: false,
  }),

  getters: {
    isProfileLoaded: (state) => state.profile !== null,
  },

  actions: {
    async fetchProfile() {
      const role = (localStorage.getItem('userRole') || '').toLowerCase()
      if (!role) return

      this.isLoading = true
      try {
        const endpoint = role === 'employer' ? '/employers/me/profile' : '/students/profile'
        const response = await apiClient.get(endpoint)
        this.profile = response.data
      } catch (error) {
        console.error('Ошибка загрузки профиля в глобальный Store:', error)
      } finally {
        this.isLoading = false
      }
    },

    updateProfileLocally(newData: Partial<UserProfileData>) {
      if (this.profile) {
        this.profile = { ...this.profile, ...newData }
      } else {
        this.profile = newData as UserProfileData
      }
    },

    clearProfile() {
      this.profile = null
    },
  },
})
