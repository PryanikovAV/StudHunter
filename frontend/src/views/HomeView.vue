<script setup lang="ts">
import { ref } from 'vue'
import HeroSection from '@/components/HeroSection.vue'
import SearchBar from '@/components/SearchBar.vue'
import PopularCategories from '@/components/PopularCategories.vue'
import VacancyFeed from '@/components/VacancyFeed.vue'

const currentCity = ref('Челябинске')

const handleSearch = (query: string) => {
  console.log('Поиск с главной:', query)
  // TODO:
  // router.push({ path: '/vacancies', query: { q: query } })
}
</script>

<template>
  <div class="home-page">
    <HeroSection />

    <section class="main-search-wrapper">
      <div class="container">
        <h2 class="page-title">Поиск стажировок и работы в {{ currentCity }}</h2>

        <SearchBar placeholder="Профессия, должность или компания" @search="handleSearch" />

        <div class="search-footer">
          <router-link to="/employer-promo" class="btn-main btn-secondary">
            Я ищу сотрудника
          </router-link>
        </div>
      </div>
    </section>

    <PopularCategories />
    <VacancyFeed />
  </div>
</template>

<style scoped>
.main-search-wrapper {
  padding: 30px 0;
}

.search-footer {
  margin-top: 16px;
}
</style>
