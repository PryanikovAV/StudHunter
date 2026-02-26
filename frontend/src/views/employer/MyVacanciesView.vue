<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import apiClient from '@/api'
import AppCard from '@/components/AppCard.vue'

interface VacancyListDto {
  id: string
  title: string
  salary: number | null
  type: string
  updatedAt: string
  isDeleted: boolean
}

const router = useRouter()
const vacancies = ref<VacancyListDto[]>([])
const isLoading = ref(true)
const activeTab = ref<'active' | 'archived'>('active')
const emit = defineEmits(['update-hero'])

const fetchVacancies = async () => {
  isLoading.value = true
  try {
    const response = await apiClient.get('/employer/vacancies?includeDeleted=true')
    vacancies.value = response.data.items || response.data
  } catch (error) {
    console.error('Ошибка загрузки вакансий:', error)
  } finally {
    isLoading.value = false
  }
}

const filteredVacancies = computed(() => {
  if (activeTab.value === 'active') {
    return vacancies.value.filter((v) => !v.isDeleted)
  } else {
    return vacancies.value.filter((v) => v.isDeleted)
  }
})

const hideVacancy = async (id: string) => {
  try {
    await apiClient.delete(`/employer/vacancies/${id}`)
    const target = vacancies.value.find((v) => v.id === id)
    if (target) target.isDeleted = true

    emit('update-hero')
  } catch (error) {
    console.error('Ошибка скрытия:', error)
  }
}

const restoreVacancy = async (id: string) => {
  try {
    await apiClient.post(`/employer/vacancies/${id}/restore`)
    const target = vacancies.value.find((v) => v.id === id)
    if (target) target.isDeleted = false

    emit('update-hero')
  } catch (error) {
    console.error('Ошибка восстановления:', error)
  }
}

const formatDate = (dateString: string) => {
  return new Date(dateString).toLocaleDateString('ru-RU', {
    day: 'numeric',
    month: 'long',
    year: 'numeric',
  })
}

onMounted(fetchVacancies)
</script>

<template>
  <div class="vacancies-page">
    <div class="page-header">
      <h1 class="page-title" style="margin: 0">Мои вакансии</h1>
      <button class="btn-main btn-dark" @click="router.push('/employer/vacancies/create')">
        + Создать вакансию
      </button>
    </div>

    <div class="tabs">
      <button
        :class="['tab-btn', { active: activeTab === 'active' }]"
        @click="activeTab = 'active'"
      >
        Активные
      </button>
      <button
        :class="['tab-btn', { active: activeTab === 'archived' }]"
        @click="activeTab = 'archived'"
      >
        В архиве (Скрытые)
      </button>
    </div>

    <div v-if="isLoading" class="loading">Загрузка вакансий...</div>

    <div v-else-if="filteredVacancies.length === 0" class="empty-state">
      <p class="text-muted">В этой вкладке пока нет вакансий.</p>
    </div>

    <div v-else class="vacancies-list">
      <AppCard v-for="vacancy in filteredVacancies" :key="vacancy.id" class="vacancy-card">
        <div class="vc-header">
          <h2 class="vc-title">{{ vacancy.title }}</h2>
          <span class="vc-salary" v-if="vacancy.salary">
            {{ vacancy.salary.toLocaleString('ru-RU') }} ₽
          </span>
          <span class="vc-salary text-muted" v-else>Зарплата не указана</span>
        </div>

        <div class="vc-meta">
          <span class="text-muted">Обновлено: {{ formatDate(vacancy.updatedAt) }}</span>
          <span class="text-muted">• Откликов: 0 (скоро)</span>
        </div>

        <div class="vc-actions">
          <button
            class="btn-main btn-secondary"
            @click="router.push(`/employer/vacancies/${vacancy.id}/edit`)"
          >
            Редактировать
          </button>

          <button
            v-if="!vacancy.isDeleted"
            class="btn-main btn-secondary"
            @click="hideVacancy(vacancy.id)"
          >
            Скрыть в архив
          </button>

          <button
            v-else
            class="btn-main btn-secondary restore-btn"
            @click="restoreVacancy(vacancy.id)"
          >
            Опубликовать
          </button>
        </div>
      </AppCard>
    </div>
  </div>
</template>

<style scoped>
.vacancies-page {
  max-width: 900px;
  margin: 0 auto;
}

.page-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 24px;
}

.tabs {
  display: flex;
  gap: 16px;
  margin-bottom: 24px;
  border-bottom: 1px solid var(--gray-border);
}

.tab-btn {
  background: none;
  border: none;
  font-size: 16px;
  font-weight: 500;
  color: var(--gray-text);
  padding: 8px 16px;
  cursor: pointer;
  border-bottom: 2px solid transparent;
  margin-bottom: -1px;
  transition: all 0.2s;
}

.tab-btn:hover {
  color: var(--dark-text);
}

.tab-btn.active {
  color: var(--susu-blue, #005aaa);
  border-bottom-color: var(--susu-blue, #005aaa);
}

.vacancies-list {
  display: flex;
  flex-direction: column;
  gap: 16px;
}

.vacancy-card {
  padding: 24px;
}

.vc-header {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  margin-bottom: 8px;
}

.vc-title {
  margin: 0;
  font-size: 20px;
  color: var(--dark-text);
}

.vc-salary {
  font-size: 18px;
  font-weight: 600;
  color: var(--dark-text);
}

.vc-meta {
  margin-bottom: 20px;
}

.vc-actions {
  display: flex;
  gap: 16px;
  border-top: 1px solid var(--gray-border);
  padding-top: 16px;
}

.restore-btn {
  color: var(--green-text-success);
}
</style>
