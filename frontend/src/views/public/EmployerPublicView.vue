<script setup lang="ts">
import { ref, onMounted, computed } from 'vue'
import { useRoute } from 'vue-router'
import apiClient from '@/api'
import AppCard from '@/components/AppCard.vue'
import IconBuilding from '@/components/icons/IconBuilding.vue'
import BackButton from '@/components/BackButton.vue'
import InteractionButtons from '@/components/InteractionButtons.vue'

interface EmployerHeroDto {
  id: string
  name: string
  avatarUrl: string | null
  cityName: string | null
  specializationName: string | null
  website: string | null
  activeVacanciesCount: number
  isFavorite?: boolean
  isBlocked?: boolean
}

interface VacancySearchDto {
  id: string
  employerId: string
  title: string
  description: string | null
  salary: number | null
  type: string
  updatedAt: string
  employerName: string
  specializationName: string | null
  cityName: string | null
  actualAddress: string | null
  contactPhone: string | null
  contactEmail: string | null
  isDeleted: boolean
  courses: string[]
  skills: string[]
  isFavorite?: boolean // Добавили для кнопок
}

const route = useRoute()
const employerId = computed(() => route.params.id as string)

const employer = ref<EmployerHeroDto | null>(null)
const vacancies = ref<VacancySearchDto[]>([])
const isLoading = ref(true)

const role = computed(() => (localStorage.getItem('userRole') || '').toLowerCase())

const fetchEmployerData = async () => {
  try {
    const heroResponse = await apiClient.get<EmployerHeroDto>(`/employers/${employerId.value}/hero`)
    employer.value = heroResponse.data

    const vacResponse = await apiClient.get('/vacancies', {
      params: {
        EmployerId: employerId.value,
      },
    })

    vacancies.value = vacResponse.data.items || vacResponse.data
  } catch (error) {
    console.error('Ошибка загрузки данных компании:', error)
  } finally {
    isLoading.value = false
  }
}

const vacanciesText = computed(() => {
  const count = employer.value?.activeVacanciesCount || 0
  if (count === 0) return 'Нет активных вакансий'

  const lastDigit = count % 10
  const lastTwoDigits = count % 100

  if (lastTwoDigits >= 11 && lastTwoDigits <= 14) return `${count} вакансий`
  if (lastDigit === 1) return `${count} вакансия`
  if (lastDigit >= 2 && lastDigit <= 4) return `${count} вакансии`
  return `${count} вакансий`
})

const formatType = (type: string) => (type === 'Internship' ? 'Стажировка' : 'Работа')
const formatDate = (dateString: string) =>
  new Date(dateString).toLocaleDateString('ru-RU', { day: 'numeric', month: 'long' })
const truncateText = (text: string | null, length: number = 180) =>
  text && text.length > length ? text.substring(0, length) + '...' : text

onMounted(fetchEmployerData)
</script>

<template>
  <div class="public-view-page">
    <BackButton />
    <div v-if="isLoading" class="loading">Загрузка информации...</div>

    <template v-else-if="employer">
      <AppCard class="hero-compact-card">
        <div class="hero-grid">
          <div class="col-personal">
            <h1 class="page-title">{{ employer.name }}</h1>
            <div class="text-muted" v-if="employer.cityName">{{ employer.cityName }}</div>

            <div class="employer-actions" v-if="role === 'student'">
              <InteractionButtons
                :target-id="employer.id"
                favorite-type="Employer"
                :initial-is-favorite="employer.isFavorite"
                :initial-is-blocked="employer.isBlocked"
              />
            </div>
          </div>

          <div class="col-study">
            <h2 class="uni-name" v-if="employer.specializationName">
              {{ employer.specializationName }}
            </h2>
            <div class="study-info" v-if="employer.website">
              <a :href="employer.website" target="_blank" class="website-link">
                {{ employer.website.replace(/^https?:\/\//, '') }}
              </a>
            </div>
          </div>

          <div class="col-visual">
            <div class="avatar">
              <img v-if="employer.avatarUrl" :src="employer.avatarUrl" alt="Company Logo" />
              <div v-else class="avatar-placeholder">
                <IconBuilding class="icon-large" />
              </div>
            </div>
            <p class="status-text">{{ vacanciesText }}</p>
          </div>
        </div>
      </AppCard>

      <div class="vacancies-section">
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
              <span v-for="skill in vac.skills?.slice(0, 5)" :key="skill" class="static-tag">{{
                skill
              }}</span>
              <span
                v-for="course in vac.courses?.slice(0, 3)"
                :key="course"
                class="static-tag course-tag"
                >{{ course }}</span
              >
            </div>

            <div class="vacancy-footer">
              <InteractionButtons
                v-if="role === 'student'"
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
    </template>

    <div v-else class="empty-state">Компания не найдена.</div>
  </div>
</template>

<style scoped>
.public-view-page {
  max-width: 900px;
  margin: 0 auto;
  padding-top: 24px;
  padding-bottom: 40px;
}

.hero-compact-card {
  padding: 24px 32px;
  margin-bottom: 32px;
}
.hero-grid {
  display: grid;
  grid-template-columns: 1fr 1.5fr 100px;
  gap: 16px;
  align-items: center;
}
.col-personal {
  display: flex;
  flex-direction: column;
  gap: 4px;
}
.page-title {
  margin: 0;
  font-size: 26px;
  color: var(--dark-text);
  font-weight: 700;
}
.text-muted {
  font-size: 15px;
  color: var(--gray-text);
}

.employer-actions {
  margin-top: 8px;
}

.col-study {
  display: flex;
  flex-direction: column;
}
.uni-name {
  font-size: 18px;
  font-weight: 600;
  color: var(--dark-text);
  margin: 0 0 6px 0;
}
.website-link {
  font-size: 15px;
  font-weight: 500;
  color: var(--susu-blue);
  text-decoration: none;
}
.website-link:hover {
  text-decoration: underline;
}
.col-visual {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 8px;
}
.avatar {
  width: 80px;
  height: 80px;
  border-radius: 12px;
  overflow: hidden;
  background-color: #f1f5f9;
  border: 1px solid var(--gray-border);
}
.avatar img {
  width: 100%;
  height: 100%;
  object-fit: cover;
}
.avatar-placeholder {
  width: 100%;
  height: 100%;
  display: flex;
  align-items: center;
  justify-content: center;
  color: var(--gray-text-focus);
}
.icon-large {
  width: 32px;
  height: 32px;
  opacity: 0.7;
}
.status-text {
  font-size: 13px;
  color: var(--susu-blue);
  font-weight: 600;
  margin: 0;
  text-align: center;
}

/* Список вакансий */
.vacancies-section {
  display: flex;
  flex-direction: column;
  gap: 16px;
}
.section-heading {
  font-size: 20px;
  font-weight: 600;
  color: var(--dark-text);
  margin: 0 0 8px 0;
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
  .hero-grid {
    grid-template-columns: 1fr;
    text-align: center;
  }
  .col-personal {
    align-items: center;
  }
  .col-study {
    padding-top: 12px;
    border-top: 1px solid var(--gray-border);
  }
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
