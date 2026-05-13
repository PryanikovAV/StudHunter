<script setup lang="ts">
import { computed } from 'vue'
import { useRouter } from 'vue-router'
import type { VacancySearchDto } from '@/types/search'
import IconBuilding from '@/components/icons/IconBuilding.vue'

defineProps<{
  vacancies: VacancySearchDto[]
}>()

const router = useRouter()

const userRole = computed(() => (localStorage.getItem('userRole') || '').toLowerCase())

const openVacancy = (id: string) => {
  if (userRole.value === 'employer') return
  if (!userRole.value) {
    router.push('/login')
    return
  }
  router.push(`/vacancies/${id}`)
}
</script>

<template>
  <section class="vacancies-section">
    <div class="container">
      <div class="section-header">
        <h2>Свежие вакансии</h2>
      </div>

      <div class="vacancy-grid">
        <div
          v-for="vacancy in vacancies"
          :key="vacancy.id"
          class="card interactive-card"
          :class="{ 'is-employer': userRole === 'employer' }"
          @click="openVacancy(vacancy.id)"
        >
          <div class="vacancy-main-info">
            <h3 class="vacancy-title">{{ vacancy.title }}</h3>
            <p class="vacancy-salary">
              {{
                vacancy.salary
                  ? `${vacancy.salary.toLocaleString('ru-RU')} ₽`
                  : 'Зарплата не указана'
              }}
            </p>
          </div>

          <div class="company-block">
            <img
              v-if="vacancy.avatarUrl"
              :src="vacancy.avatarUrl"
              class="company-logo"
              alt="Logo"
            />
            <div v-else class="company-logo-stub">
              <IconBuilding class="icon-tiny" />
            </div>

            <span class="company-name">{{ vacancy.employerName }}</span>
          </div>
        </div>
      </div>
    </div>
  </section>
</template>

<style scoped>
.vacancies-section {
  padding: 0px 0 60px;
}
.section-header h2 {
  font-size: 20px;
  font-weight: 700;
  margin-bottom: 20px;
}

.vacancy-grid {
  display: grid;
  grid-template-columns: repeat(4, 1fr);
  gap: 20px;
}

.interactive-card {
  transition:
    border-color 0.2s,
    box-shadow 0.2s;
  background: #fff;
  padding: 20px;
  border-radius: 12px;
  border: 1px solid var(--gray-border);
  display: flex;
  flex-direction: column;
  justify-content: space-between;
  min-width: 0;
}

.interactive-card:not(.is-employer) {
  cursor: pointer;
}

.interactive-card:hover {
  border-color: var(--susu-blue);
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.05);
}

@media (max-width: 1024px) {
  .vacancy-grid {
    grid-template-columns: repeat(2, 1fr);
    gap: 12px;
  }
}

@media (max-width: 600px) {
  .vacancy-grid {
    grid-template-columns: repeat(2, 1fr);
    gap: 8px;
  }
  .vacancy-title {
    font-size: 14px;
  }
}

.vacancy-main-info {
  display: flex;
  flex-direction: column;
  gap: 12px;
}
.vacancy-title {
  margin: 0;
  font-weight: 700;
  font-size: 16px;
  color: var(--dark-text);
  line-height: 1.2;
}
.vacancy-salary {
  margin: 0;
  font-size: 14px;
  font-weight: 600;
  color: var(--susu-blue);
}

.company-block {
  margin-top: 16px;
  display: flex;
  align-items: center;
  gap: 10px;
  padding-top: 14px;
  border-top: 1px solid var(--gray-border);
  min-width: 0;
}

.company-logo {
  width: 28px;
  height: 28px;
  object-fit: contain;
  border-radius: 6px;
  background: #fff;
  border: 1px solid var(--gray-border);
  flex-shrink: 0;
}

.company-logo-stub {
  width: 28px;
  height: 28px;
  background: #f1f5f9;
  border-radius: 6px;
  flex-shrink: 0;
  display: flex;
  align-items: center;
  justify-content: center;
  color: var(--gray-text-focus);
}

.icon-tiny {
  width: 16px;
  height: 16px;
  opacity: 0.7;
}

.company-name {
  font-size: 12px;
  color: var(--gray-text);
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
  flex: 1;
}
</style>
