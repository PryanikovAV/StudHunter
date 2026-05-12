<script setup lang="ts">
import AppCard from '@/components/AppCard.vue'
</script>

<template>
  <div class="page-narrow search-page-wrapper">
    <div class="search-bar-fullwidth">
      <slot name="search-bar"></slot>
    </div>

    <div class="search-layout">
      <aside class="filters-sidebar">
        <AppCard class="filters-card">
          <h2 class="filters-title">Фильтры</h2>
          <slot name="filters"></slot>
        </AppCard>
      </aside>

      <main class="results-main">
        <slot name="results"></slot>
      </main>
    </div>
  </div>
</template>

<style scoped>
.search-page-wrapper {
  display: flex;
  flex-direction: column;
  gap: 24px;
}

.search-bar-fullwidth {
  width: 100%;
}

.search-layout {
  display: flex;
  gap: 24px;
  align-items: flex-start;
}

.filters-sidebar {
  width: 280px;
  flex-shrink: 0;
  position: sticky;
  top: 24px;
}
.filters-card {
  padding: 20px;
}
.filters-title {
  margin: 0 0 20px 0;
  font-size: 18px;
  font-weight: 600;
  color: var(--dark-text);
}

.results-main {
  flex: 1;
  min-width: 0;
  display: flex;
  flex-direction: column;
  gap: 16px;
}

@media (max-width: 768px) {
  .search-layout {
    flex-direction: column;
  }
  .filters-sidebar {
    width: 100%;
    position: static;
  }
}
</style>
