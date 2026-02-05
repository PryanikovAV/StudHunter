<script setup lang="ts">
import IconLightning from '@/components/icons/IconLightning.vue'
</script>
<template>
  <section class="popular-section">
    <div class="container">
      <div class="section-header">
        <IconLightning class="icon-orange" />
        <h2>Популярное</h2>
      </div>

      <div class="filter-grid">
        <div class="filter-card">
          <div class="card-info">
            <span class="card-title">Вакансии месяца</span>
            <span class="card-count">142 вакансии</span>
          </div>
        </div>

        <div class="filter-card">
          <div class="card-info">
            <span class="card-title">Компании месяца</span>
            <span class="card-count">1893 компании</span>
          </div>
        </div>

        <div class="filter-card">
          <div class="card-info">
            <span class="card-title">Можно удалённо</span>
            <span class="card-count">48 302 вакансии</span>
          </div>
        </div>

        <div class="filter-card">
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
  color: #f59e0b; /* Оранжевая молния */
  width: 24px;
  height: 24px;
}

/* Сетка 4 в ряд */
.filter-grid {
  display: grid;
  grid-template-columns: repeat(4, 1fr);
  gap: 20px;
}

.filter-card {
  background: #ffffff;
  border: 1px solid #e2e8f0;
  border-radius: 16px;
  padding: 20px;
  cursor: pointer;
  transition: all 0.2s ease;
}

.filter-card:hover {
  border-color: var(--susu-blue);
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.05);
  transform: translateY(-2px);
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
  color: #94a3b8;
}
</style>
