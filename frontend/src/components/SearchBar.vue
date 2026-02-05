<script setup lang="ts">
import IconSearch from './icons/IconSearch.vue'
import IconFilters from './icons/IconFilters.vue'
</script>

<template>
  <div class="search-section">
    <div class="container">
      <h2 class="search-title">Поиск стажировок и работы в Челябинске</h2>

      <div class="search-row">
        <div class="input-wrapper">
          <IconSearch class="icon-left" />
          <input
            type="text"
            placeholder="Профессия, должность, или компания"
            class="search-input"
          />
          <button class="filter-btn">
            <IconFilters />
          </button>
        </div>

        <button class="btn-primary">Найти</button>
      </div>

      <button class="btn-text btn-text-gray employer-link">Я ищу сотрудника</button>
    </div>
  </div>
</template>

<style scoped>
.search-section {
  padding: 10px 0; /* Отступ вместо наползания */
  background-color: #f8fafc; /* Легкий фон всей секции, чтобы инпут выделялся */
}

/* 1 строка: Стиль как у hero-tagline */
.search-title {
  font-size: 18px;
  font-weight: 500;
  color: var(--dark-text);
  margin-bottom: 16px;
  text-align: left;
}

/* 2 строка: Флекс-ряд */
.search-row {
  display: flex;
  gap: 15px;
}

.input-wrapper {
  position: relative;
  flex-grow: 1; /* Занимает всё доступное место */
  display: flex;
  align-items: center;
}

.search-input {
  width: 100%;
  height: 40px;
  padding: 0 50px 0 50px; /* Отступы под иконки слева и справа */
  border: 1px solid #e2e8f0;
  border-radius: 100px;
  font-size: 16px;
  outline: none;
  transition: border-color 0.2s;
}

.search-input:focus {
  border-color: var(--susu-blue);
}

/* Иконка лупы слева */
.icon-left {
  position: absolute;
  left: 18px;
  color: #94a3b8; /* Светло-серый */
  width: 23px;
  height: 23px;
}

/* Кнопка-фильтр справа */
.filter-btn {
  position: absolute;
  right: 18px;
  background: none;
  border: none;
  color: #94a3b8;
  cursor: pointer;
  display: flex;
  align-items: center;
  width: 40px;
  height: 40px;
}

.employer-link {
  font-size: 16px;
  margin-top: 5px;
}

.employer-link:hover {
  color: var(--dark-text);
}

.btn-text-gray:hover {
  color: var(--dark-text); /* При наведении становится темнее */
}
</style>
