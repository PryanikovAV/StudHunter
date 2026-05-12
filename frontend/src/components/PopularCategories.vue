<script setup lang="ts">
import { computed } from 'vue'
import { useRouter } from 'vue-router'
import IconLightning from '@/components/icons/IconLightning.vue'
import type { CategoryCardDto } from '@/types/home'

defineProps<{
  categories: CategoryCardDto[]
}>()

const router = useRouter()
const userRole = computed(() => (localStorage.getItem('userRole') || '').toLowerCase())

const navigateToFilter = (card: CategoryCardDto) => {
  if (userRole.value === 'employer') {
    return
  }

  if (!userRole.value) {
    router.push('/login')
    return
  }

  router.push({
    name: 'student-vacancy-search',
    query: { [card.filterKey]: card.filterValue },
  })
}

const getVacancyLabel = (n: number) => {
  const last = n % 10
  const lastTwo = n % 100
  if (lastTwo > 10 && lastTwo < 20) return 'вакансий'
  if (last === 1) return 'вакансия'
  if (last >= 2 && last <= 4) return 'вакансии'
  return 'вакансий'
}
</script>

<template>
  <section class="popular-section">
    <div class="container">
      <div class="section-header">
        <IconLightning class="icon-main icon-orange" />
        <h2>Популярное</h2>
      </div>

      <div class="filter-grid">
        <div
          v-for="card in categories"
          :key="card.filterValue"
          class="card interactive-card"
          :class="{ 'is-employer': userRole === 'employer' }"
          @click="navigateToFilter(card)"
        >
          <div class="card-info">
            <span class="card-title">{{ card.title }}</span>
            <span class="card-count">{{ card.count }} {{ getVacancyLabel(card.count) }}</span>
          </div>
        </div>
      </div>
    </div>
  </section>
</template>

<style scoped>
.popular-section {
  padding: 32px 0;
}
.section-header {
  display: flex;
  align-items: center;
  gap: 2px;
  margin-bottom: 20px;
}
.section-header h2 {
  font-size: 20px;
  font-weight: 700;
}
.icon-orange {
  color: #f59e0b;
}

.filter-grid {
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
}

.interactive-card:not(.is-employer) {
  cursor: pointer;
}

.interactive-card:hover {
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.05);
  border-color: var(--susu-blue);
}

@media (max-width: 1024px) {
  .filter-grid {
    grid-template-columns: repeat(2, 1fr);
    gap: 12px;
  }
}

@media (max-width: 600px) {
  .filter-grid {
    grid-template-columns: repeat(2, 1fr);
    gap: 8px;
  }
}

.card-info {
  display: flex;
  flex-direction: column;
  gap: 8px;
}
.card-title {
  font-weight: 700;
  font-size: 15px;
  color: var(--dark-text);
}
.card-count {
  font-size: 12px;
  color: var(--gray-text);
}
</style>
