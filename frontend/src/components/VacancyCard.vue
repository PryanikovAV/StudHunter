<script setup lang="ts">
import AppCard from '@/components/AppCard.vue'
import InteractionButtons from '@/components/InteractionButtons.vue'
import type { VacancySearchDto } from '@/types/search'

defineProps<{
  vacancy: VacancySearchDto
}>()
</script>

<template>
  <AppCard class="result-card">
    <div class="card-header">
      <div class="header-left">
        <router-link :to="`/vacancies/${vacancy.id}`" class="vacancy-title">
          {{ vacancy.title }}
        </router-link>
        <div class="salary-text">
          {{ vacancy.salary ? `${vacancy.salary.toLocaleString('ru-RU')} ₽` : 'Доход не указан' }}
        </div>
      </div>

      <InteractionButtons
        :target-id="vacancy.id"
        favorite-type="Vacancy"
        :block-user-id="vacancy.employerId"
        :initial-is-favorite="vacancy.isFavorite"
        :initial-is-blocked="vacancy.isBlocked"
      />
    </div>

    <div class="company-info">
      <span class="company-name">{{ vacancy.employerName }}</span>
      <span class="dot-separator">•</span>
      <span class="text-muted">{{ vacancy.cityName || 'Локация не указана' }}</span>
      <span class="badge-type">{{ vacancy.type === 'Internship' ? 'Стажировка' : 'Работа' }}</span>
    </div>

    <p class="vacancy-desc">
      {{ vacancy.description?.slice(0, 150) || 'Описание отсутствует...' }}
    </p>

    <div class="tags-row" v-if="vacancy.skills && vacancy.skills.length > 0">
      <span v-for="skill in vacancy.skills.slice(0, 4)" :key="skill" class="tag-pill">
        {{ skill }}
      </span>
      <span v-if="vacancy.skills.length > 4" class="tag-pill text-muted">...</span>
    </div>
  </AppCard>
</template>

<style scoped>
.result-card {
  padding: 24px;
  transition: border-color 0.2s;
}
.result-card:hover {
  border-color: var(--gray-border-focus);
}
.card-header {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  margin-bottom: 12px;
}
.header-left {
  display: flex;
  flex-direction: column;
  gap: 4px;
}
.vacancy-title {
  font-size: 18px;
  font-weight: 600;
  color: var(--susu-blue);
  text-decoration: none;
}
.vacancy-title:hover {
  text-decoration: underline;
}
.salary-text {
  font-size: 16px;
  font-weight: 600;
  color: var(--dark-text);
}
.company-info {
  display: flex;
  align-items: center;
  gap: 8px;
  font-size: 14px;
  margin-bottom: 12px;
}
.company-name {
  font-weight: 500;
  color: var(--dark-text);
}
.dot-separator {
  color: var(--gray-border);
}
.badge-type {
  background: #f1f5f9;
  padding: 2px 8px;
  border-radius: 4px;
  font-size: 12px;
  color: var(--gray-text-focus);
}
.vacancy-desc {
  font-size: 14px;
  line-height: 1.5;
  color: var(--dark-text);
  margin: 0 0 16px 0;
}
.tags-row {
  display: flex;
  flex-wrap: wrap;
  gap: 8px;
}
.tag-pill {
  background: var(--background-page);
  border: 1px solid var(--gray-border);
  padding: 2px 10px;
  border-radius: 12px;
  font-size: 12px;
  color: var(--gray-text-focus);
}
</style>
