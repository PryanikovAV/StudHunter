<script setup lang="ts">
import { ref, onMounted, computed } from 'vue'
import { useRoute } from 'vue-router'
import apiClient from '@/api'
import AppCard from '@/components/AppCard.vue'
import BackButton from '@/components/BackButton.vue'
import EmployerHero from '@/components/EmployerHero.vue'
import InteractionButtons from '@/components/InteractionButtons.vue'
import type { VacancySearchDto } from '@/types/employer'

const route = useRoute()
const employerId = computed(() => route.params.id as string)

const vacancies = ref<VacancySearchDto[]>([])
const isLoading = ref(true)
const favoriteVacancyIds = ref<string[]>([])

const role = computed(() => (localStorage.getItem('userRole') || '').toLowerCase())
const isStudent = computed(() => role.value === 'student')

const fetchFavorites = async () => {
  if (!isStudent.value) return
  try {
    const favRes = await apiClient.get('/favorites')
    const favItems = favRes.data.items || favRes.data
    favoriteVacancyIds.value = favItems.map((f: { targetId: string }) => f.targetId)
  } catch (error) {
    console.error('Ошибка загрузки избранного:', error)
  }
}

const fetchVacancies = async () => {
  try {
    const vacResponse = await apiClient.get('/vacancies', {
      params: { EmployerId: employerId.value },
    })

    const rawVacancies = vacResponse.data.items || vacResponse.data
    vacancies.value = rawVacancies.map((vac: VacancySearchDto) => ({
      ...vac,
      isFavorite: favoriteVacancyIds.value.includes(vac.id),
    }))
  } catch (error) {
    console.error('Ошибка загрузки вакансий:', error)
  } finally {
    isLoading.value = false
  }
}

const formatType = (type: string) => (type === 'Internship' ? 'Стажировка' : 'Работа')
const formatDate = (dateString: string) =>
  new Date(dateString).toLocaleDateString('ru-RU', { day: 'numeric', month: 'long' })
const truncateText = (text: string | null, length: number = 180) =>
  text && text.length > length ? text.substring(0, length) + '...' : text

onMounted(async () => {
  await fetchFavorites()
  await fetchVacancies()
})
</script>

<template>
  <div class="public-view-page">
    <div class="container">
      <BackButton />
    </div>

    <EmployerHero :employer-id="employerId" :readonly-mode="true" :show-interactions="isStudent" />

    <div class="vacancies-wrapper">
      <div v-if="isLoading" class="loading mt-4">Загрузка вакансий...</div>

      <div class="vacancies-section" v-else>
        <h2 class="section-heading">Открытые вакансии</h2>

        <div v-if="vacancies.length === 0" class="empty-state">
          Компания пока не опубликовала активных вакансий.
        </div>

        <div class="vacancies-list" v-else>
          <AppCard v-for="vac in vacancies" :key="vac.id" class="vacancy-card">
            <div class="vacancy-content-top">
              <div class="vacancy-main">
                <h3 class="vacancy-title">{{ vac.title }}</h3>
                <div class="meta-row">
                  <span class="type-tag">{{ formatType(vac.type) }}</span>
                  <span class="date-text">Обновлено: {{ formatDate(vac.updatedAt) }}</span>
                </div>
                <div class="description-snippet" v-if="vac.description">
                  {{ truncateText(vac.description) }}
                </div>
              </div>

              <div class="vacancy-salary" v-if="vac.salary">
                {{ vac.salary.toLocaleString('ru-RU') }} ₽
              </div>
            </div>

            <div
              class="tags-wrapper"
              v-if="(vac.skills && vac.skills.length) || (vac.courses && vac.courses.length)"
            >
              <span v-for="skill in vac.skills?.slice(0, 5)" :key="skill" class="static-tag">
                {{ skill }}
              </span>
              <span
                v-for="course in vac.courses?.slice(0, 3)"
                :key="course"
                class="static-tag course-tag"
              >
                {{ course }}
              </span>
            </div>

            <div class="vacancy-footer">
              <InteractionButtons
                v-if="isStudent"
                :target-id="vac.id"
                favorite-type="Vacancy"
                :initial-is-favorite="vac.isFavorite"
              />

              <router-link :to="`/vacancies/${vac.id}`" class="btn-main btn-dark btn-details">
                Подробнее
              </router-link>
            </div>
          </AppCard>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
.public-view-page {
  padding-top: 24px;
  padding-bottom: 40px;
  width: 100%;
}

.vacancies-wrapper {
  max-width: 900px;
  margin: 0 auto;
  padding: 0 15px;
  margin-top: 32px;
}

.mt-4 {
  margin-top: 24px;
}
.loading {
  text-align: center;
  color: var(--gray-text);
  padding: 20px;
}

.vacancies-section {
  display: flex;
  flex-direction: column;
  gap: 16px;
}
.section-heading {
  font-size: 22px;
  font-weight: 700;
  color: var(--dark-text);
  margin: 0 0 12px 0;
  padding-left: 4px;
}
.vacancies-list {
  display: flex;
  flex-direction: column;
  gap: 16px;
}
.vacancy-card {
  padding: 24px;
  display: flex;
  flex-direction: column;
  gap: 16px;
}
.vacancy-content-top {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  gap: 24px;
}
.vacancy-main {
  display: flex;
  flex-direction: column;
  gap: 8px;
  flex: 1;
}
.vacancy-title {
  font-size: 20px;
  font-weight: 600;
  color: var(--dark-text);
  margin: 0;
}
.meta-row {
  display: flex;
  align-items: center;
  gap: 12px;
}
.type-tag {
  background: #f1f5f9;
  padding: 4px 10px;
  border-radius: 4px;
  font-size: 13px;
  color: #475569;
  font-weight: 500;
}
.date-text {
  font-size: 13px;
  color: var(--gray-text-focus);
}
.description-snippet {
  font-size: 14px;
  line-height: 1.5;
  color: var(--dark-text);
  margin-top: 4px;
  max-width: 700px;
}
.vacancy-salary {
  font-size: 20px;
  font-weight: 600;
  color: var(--dark-text);
  white-space: nowrap;
}
.tags-wrapper {
  display: flex;
  flex-wrap: wrap;
  gap: 8px;
}
.static-tag {
  background: #f8fafc;
  border: 1px solid var(--gray-border);
  padding: 4px 10px;
  border-radius: 6px;
  font-size: 12px;
  color: var(--dark-text);
}
.course-tag {
  background: #fdf8f6;
  border-color: #fce7f3;
}
.vacancy-footer {
  display: flex;
  align-items: center;
  gap: 12px;
  margin-top: 8px;
  padding-top: 16px;
  border-top: 1px solid var(--gray-border);
}
.btn-details {
  margin-left: auto;
  text-decoration: none;
  display: inline-flex;
  align-items: center;
  justify-content: center;
}
.empty-state {
  text-align: center;
  padding: 40px;
  color: var(--gray-text-focus);
  background: var(--background-field);
  border-radius: 12px;
  border: 1px dashed var(--gray-border);
}

@media (max-width: 768px) {
  .vacancy-content-top {
    flex-direction: column-reverse;
  }
  .vacancy-salary {
    font-size: 18px;
  }
  .vacancy-footer {
    justify-content: space-between;
  }
  .btn-details {
    margin-left: 0;
  }
}
</style>
