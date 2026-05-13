<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import apiClient from '@/api'
import HeroSection from '@/components/HeroSection.vue'
import SearchBar from '@/components/SearchBar.vue'
import PopularCategories from '@/components/PopularCategories.vue'
import VacancyFeed from '@/components/VacancyFeed.vue'
import type { HomePageDto } from '@/types/home'

const router = useRouter()
const currentCity = ref('Челябинске')
const userRole = computed(() => (localStorage.getItem('userRole') || '').toLowerCase())
const isLoading = ref(true)
const homeData = ref<HomePageDto | null>(null)

const fetchHomeData = async () => {
  try {
    const response = await apiClient.get<HomePageDto>('/vacancies/featured')
    homeData.value = response.data
  } catch (error) {
    console.error('Ошибка загрузки данных главной страницы:', error)
  } finally {
    isLoading.value = false
  }
}

onMounted(fetchHomeData)

const searchPlaceholder = computed(() => {
  if (userRole.value === 'employer') {
    return 'Навык, профессия, ВУЗ или имя студента'
  }
  return 'Профессия, должность или компания'
})

const searchTitle = computed(() => {
  if (userRole.value === 'employer') {
    return `Поиск талантов и молодых специалистов в ${currentCity.value}`
  }
  return `Поиск стажировок и работы в ${currentCity.value}`
})

const handleSearch = (query: string) => {
  if (!userRole.value) {
    router.push('/login')
    return
  }

  if (userRole.value === 'employer') {
    router.push({
      name: 'employer-resume-search',
      query: query.trim() ? { q: query } : {},
    })
    return
  }

  router.push({
    name: 'student-vacancy-search',
    query: query.trim() ? { q: query } : {},
  })
}

const handleAdvancedSearch = () => {
  if (!userRole.value) {
    router.push('/login')
    return
  }

  if (userRole.value === 'employer') {
    router.push({ name: 'employer-resume-search' })
    return
  }

  router.push({ name: 'student-vacancy-search' })
}
</script>

<template>
  <div class="home-page">
    <HeroSection />

    <section class="main-search-wrapper">
      <div class="container">
        <h2 class="page-title">{{ searchTitle }}</h2>

        <SearchBar
          :placeholder="searchPlaceholder"
          @search="handleSearch"
          @advanced-search="handleAdvancedSearch"
        />

        <div class="search-footer">
          <a
            href="https://www.susu.ru/ru/partners"
            target="_blank"
            rel="noopener noreferrer"
            class="btn-main btn-secondary"
          >
            Я ищу сотрудника
          </a>
        </div>
      </div>
    </section>

    <PopularCategories
      v-if="homeData?.popularCategories"
      :categories="homeData.popularCategories"
    />

    <VacancyFeed v-if="homeData?.latestVacancies" :vacancies="homeData.latestVacancies" />
  </div>
</template>

<style scoped>
.main-search-wrapper {
  padding: 30px 0;
}

.page-title {
  margin-bottom: 24px;
  color: var(--dark-text);
}

.search-footer {
  margin-top: 16px;
}
</style>
