<template>
  <section class="vacancies-section">
    <div class="container">
      <div class="section-header">
        <h2>Вакансии дня</h2>
      </div>

      <div class="vacancy-grid">
        <div v-for="i in 8" :key="i" class="card">
          <div class="vacancy-main-info">
            <h3 class="vacancy-title">Менеджер по работе с клиентами</h3>
            <p class="vacancy-salary">от 80 000 до 120 000 ₽</p>
          </div>

          <div class="company-block">
            <div class="company-logo-stub"></div>
            <span class="company-name">Россельхозбанк</span>
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
}

/* СЕТКА С АДАПТИВОМ */
.vacancy-grid {
  display: grid;
  grid-template-columns: repeat(4, 1fr);
  gap: 20px;
}

@media (max-width: 1024px) {
  .vacancy-grid {
    grid-template-columns: repeat(2, 1fr);
  }
}

@media (max-width: 600px) {
  .vacancy-grid {
    grid-template-columns: 1fr;
  }
}

.vacancy-main-info {
  display: flex;
  flex-direction: column;
  gap: 15px;
}

.vacancy-title {
  margin: 0;
  padding: 0;
  font-weight: 700;
  font-size: 16px;
  color: var(--dark-text);
  line-height: 1.2;
}

.vacancy-salary {
  margin: 0;
  padding: 0;
  font-size: 14px;
  font-weight: 600;
  color: var(--dark-text);
}

.company-block {
  margin-top: 16px;
  display: flex;
  align-items: center;
  gap: 10px;
  padding-top: 14px;
  border-top: 1px solid var(--gray-border);
}

.company-logo-stub {
  width: 28px;
  height: 28px;
  background: var(--gray-border);
  border-radius: 6px;
}

.company-name {
  font-size: 13px;
  color: var(--gray-text);
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}
</style>
