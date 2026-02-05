<template>
  <section class="vacancies-section">
    <div class="container">
      <div class="section-header">
        <h2>Вакансии дня</h2>
      </div>

      <div class="vacancy-grid">
        <div v-for="i in 8" :key="i" class="vacancy-card">
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

/* Если экран меньше 1024px (планшеты и узкие окна) */
@media (max-width: 1024px) {
  .vacancy-grid {
    grid-template-columns: repeat(2, 1fr); /* 2 колонки */
  }
}

/* Если экран меньше 600px (телефоны) */
@media (max-width: 600px) {
  .vacancy-grid {
    grid-template-columns: 1fr; /* 1 колонка во всю ширину */
  }
}

/* КАРТОЧКА */
.vacancy-card {
  background: #ffffff;
  border: 1px solid #e2e8f0;
  border-radius: 16px;
  /* Пункты 2 и 4: Единый отступ со всех сторон для симметрии */
  padding: 20px;
  display: flex;
  flex-direction: column;
  justify-content: flex-start;
  min-height: auto;
  cursor: pointer;
  transition: all 0.2s ease;
}

.vacancy-card:hover {
  border-color: #cbd5e1;
  box-shadow: 0 8px 20px rgba(0, 0, 0, 0.04);
}

/* Пункт 3: Группа для управления расстоянием между текстом */
.vacancy-main-info {
  display: flex;
  flex-direction: column;
  gap: 15px; /* Минимальный отступ между заголовком и зарплатой */
}

.vacancy-title {
  margin: 0; /* Убираем невидимые внешние отступы */
  padding: 0;
  font-weight: 700;
  font-size: 16px;
  color: var(--dark-text); /* Пункт 1: как в популярном */
  line-height: 1.2;
}

.vacancy-salary {
  margin: 0; /* Убираем невидимые внешние отступы */
  padding: 0;
  font-size: 14px;
  font-weight: 600; /* Пункт 3 */
  color: var(--dark-text);
}

.company-block {
  margin-top: 16px; /* Выталкивает блок вниз */
  display: flex;
  align-items: center;
  gap: 10px;
  padding-top: 14px;
  border-top: 1px solid #f1f5f9;
}

.company-logo-stub {
  width: 28px;
  height: 28px;
  background: #f1f5f9;
  border-radius: 6px;
}

.company-name {
  font-size: 13px;
  color: #64748b;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}
</style>
