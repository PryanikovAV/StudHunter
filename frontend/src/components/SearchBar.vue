<script setup lang="ts">
import { ref } from 'vue'
import IconSearch from '@/components/icons/IconSearch.vue'
import IconFilters from '@/components/icons/IconFilters.vue'
import AppInput from '@/components/AppInput.vue'

withDefaults(
  defineProps<{
    placeholder?: string
  }>(),
  {
    placeholder: 'Поиск...',
  },
)

const emit = defineEmits<{
  (e: 'search', query: string): void
}>()

const searchQuery = ref('')

const handleSearch = () => {
  const query = searchQuery.value.trim()
  if (query) {
    emit('search', query)
  }
}
</script>

<template>
  <form class="search-form" @submit.prevent="handleSearch">
    <div class="input-wrapper">
      <IconSearch class="icon-main search-icon-left" />

      <AppInput type="text" v-model="searchQuery" :placeholder="placeholder" class="search-input" />

      <button type="button" class="filter-btn" title="Расширенный поиск">
        <IconFilters class="icon-main" />
      </button>
    </div>

    <button type="submit" class="btn-main btn-dark search-submit-btn">Найти</button>
  </form>
</template>

<style scoped>
.search-form {
  display: flex;
  gap: 12px;
  width: 100%;
  align-items: center;
}

.input-wrapper {
  position: relative;
  width: 100%;
  flex-grow: 1;
  display: flex;
  align-items: center;
}

.search-input {
  border-radius: 100px;
  padding-left: 48px;
  padding-right: 48px;
}

:deep(.search-input::placeholder) {
  color: var(--gray-text);
  opacity: 1;
}

.search-icon-left {
  position: absolute;
  left: 16px;
  color: var(--gray-text);
  pointer-events: none;
}

.filter-btn {
  position: absolute;
  right: 8px;
  background: none;
  border: none;
  padding: 8px;
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: center;
  color: var(--gray-text);
  border-radius: 50%;
  transition: all 0.2s;
}

.filter-btn:hover {
  color: var(--gray-text-focus);
  background-color: rgba(0, 0, 0, 0.05);
}

.search-submit-btn {
  min-width: 120px;
  font-weight: 600;
}

@media (max-width: 640px) {
  .search-form {
    flex-direction: column;
    align-items: stretch;
    gap: 16px;
  }

  .search-submit-btn {
    width: 100%;
  }
}
</style>
