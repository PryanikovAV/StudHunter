<script setup lang="ts">
import IconLightning from '@/components/icons/IconLightning.vue'
</script>
<template>
  <section class="popular-section">
    <div class="container">
      <div class="section-header">
        <IconLightning class="icon-main icon-orange" />
        <h2>Популярное</h2>
      </div>

      <div class="filter-grid">
        <div class="card">
          <div class="card-info">
            <span class="card-title">Вакансии месяца</span>
            <span class="card-count">142 вакансии</span>
          </div>
        </div>

        <div class="card">
          <div class="card-info">
            <span class="card-title">Компании месяца</span>
            <span class="card-count">1893 компании</span>
          </div>
        </div>

        <div class="card">
          <div class="card-info">
            <span class="card-title">Можно удалённо</span>
            <span class="card-count">48 302 вакансии</span>
          </div>
        </div>

        <div class="card">
          <div class="card-info">
            <span class="card-title">Пройти стажировку</span>
            <span class="card-count">589 вакансий</span>
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

@media (max-width: 1024px) {
  .filter-grid {
    grid-template-columns: repeat(2, 1fr);
  }
}

@media (max-width: 600px) {
  .filter-grid {
    grid-template-columns: 1fr;
  }
}

.card-info {
  display: flex;
  flex-direction: column;
  gap: 8px;
}

.card-title {
  font-weight: 700;
  font-size: 16px;
  color: var(--dark-text);
}

.card-count {
  font-size: 13px;
  color: var(--gray-text);
}
</style>
