<script setup lang="ts">
import { ref, onMounted, watch } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import apiClient from '@/api'
import AppTagAutocomplete from '@/components/AppTagAutocomplete.vue'
import SearchPageLayout from '@/components/search/SearchPageLayout.vue'
import SearchBar from '@/components/SearchBar.vue'
import VacancyCard from '@/components/VacancyCard.vue'
import type { VacancySearchFilter, VacancySearchDto, PagedResult } from '@/types/search'
import type { DictionaryItem } from '@/types/student'

const route = useRoute()
const router = useRouter()

const searchQuery = ref((route.query.q as string) || '')
const isLoading = ref(false)
const results = ref<VacancySearchDto[]>([])
const totalCount = ref(0)
const hasNextPage = ref(false)

const dictionaries = ref({
  cities: [] as DictionaryItem[],
  specializations: [] as DictionaryItem[],
})

const filters = ref({
  cityId: null as string | null,
  specializationId: null as string | null,
  type: null as 'Internship' | 'Job' | null,
  minSalary: null as number | null,
  skills: [] as { id: string; name: string }[],
  courses: [] as { id: string; name: string }[],
})

const favoriteIds = ref<string[]>([])
const blockedIds = ref<string[]>([])

const fetchInitialData = async () => {
  try {
    const [favRes, blockRes, citiesRes, specRes] = await Promise.all([
      apiClient.get('/favorites'),
      apiClient.get('/blacklist'),
      apiClient.get('/dictionaries/cities'),
      apiClient.get('/dictionaries/specializations'),
    ])

    const favItems = favRes.data.items || favRes.data
    favoriteIds.value = favItems.map((f: { targetId: string }) => f.targetId)

    const blockItems = blockRes.data.items || blockRes.data
    blockedIds.value = blockItems.map((b: { blockedUserId: string }) => b.blockedUserId)

    dictionaries.value.cities = citiesRes.data
    dictionaries.value.specializations = specRes.data

    const chelyabinsk = citiesRes.data.find((c: DictionaryItem) => c.name === 'Челябинск')
    if (chelyabinsk && !filters.value.cityId) {
      filters.value.cityId = chelyabinsk.id
    }
  } catch (error) {
    console.error('Ошибка загрузки базовых данных', error)
  }
}

const fetchSkills = async (q: string) =>
  (await apiClient.get(`/dictionaries/skills/search?q=${q}&limit=10`)).data
const fetchCourses = async (q: string) =>
  (await apiClient.get(`/dictionaries/courses/search?q=${q}&limit=10`)).data

const executeSearch = async (isLoadMore = false) => {
  if (!isLoadMore) {
    isLoading.value = true
    results.value = []
  }

  const currentPage = isLoadMore ? Math.floor(results.value.length / 10) + 1 : 1

  const payload: VacancySearchFilter = {
    searchTerm: searchQuery.value || null,
    cityId: filters.value.cityId,
    specializationIds: filters.value.specializationId ? [filters.value.specializationId] : [],
    vacancyTypes: filters.value.type ? [filters.value.type] : [],
    minSalary: filters.value.minSalary || null,
    onlyWithSalary: !!filters.value.minSalary,
    skillIds: filters.value.skills.map((s) => s.id),
    courseIds: filters.value.courses.map((c) => c.id),
    paging: { pageNumber: currentPage, pageSize: 10 },
  }

  try {
    const response = await apiClient.post<PagedResult<VacancySearchDto>>(
      '/search/vacancies',
      payload,
    )

    const mappedItems = response.data.items.map((v) => ({
      ...v,
      isFavorite: favoriteIds.value.includes(v.id),
      isBlocked: blockedIds.value.includes(v.employerId),
    }))

    if (isLoadMore) results.value.push(...mappedItems)
    else results.value = mappedItems

    totalCount.value = response.data.totalCount
    hasNextPage.value = response.data.hasNextPage
  } catch (error) {
    console.error('Ошибка поиска:', error)
  } finally {
    isLoading.value = false
  }
}

let debounceTimeout: ReturnType<typeof setTimeout>
const handleSearchSubmit = (query: string) => {
  searchQuery.value = query

  const newQuery = { ...route.query }
  if (query) {
    newQuery.q = query
  } else {
    delete newQuery.q
  }
  router.push({ query: newQuery })

  clearTimeout(debounceTimeout)
  debounceTimeout = setTimeout(() => {
    executeSearch(false)
  }, 500)
}

const resetFilters = () => {
  const chelyabinsk = dictionaries.value.cities.find((c) => c.name === 'Челябинск')
  filters.value = {
    cityId: chelyabinsk ? chelyabinsk.id : null,
    specializationId: null,
    type: null,
    minSalary: null,
    skills: [],
    courses: [],
  }
  executeSearch(false)
}

watch(
  () => route.query.q,
  (newQ) => {
    if (searchQuery.value !== newQ) {
      searchQuery.value = (newQ as string) || ''
      executeSearch(false)
    }
  },
)

const syncFiltersFromUrl = () => {
  const typeQuery = route.query.VacancyTypes || route.query.vacancyTypes || route.query.vacancyType
  if (typeQuery === 'Internship' || typeQuery === 'Job') {
    filters.value.type = typeQuery
  }
  const specQuery =
    route.query.SpecializationIds || route.query.SpecializationId || route.query.specializationId
  if (specQuery && typeof specQuery === 'string') {
    filters.value.specializationId = specQuery
  }
}

onMounted(async () => {
  await fetchInitialData()
  syncFiltersFromUrl()
  executeSearch(false)
})
</script>

<template>
  <SearchPageLayout>
    <template #search-bar>
      <SearchBar
        :initial-value="searchQuery"
        placeholder="Название вакансии, описание или имя компании..."
        @search="handleSearchSubmit"
      />
    </template>

    <template #filters>
      <div class="filter-group">
        <label class="filter-label">Город</label>
        <select
          v-model="filters.cityId"
          class="input-main uniform-input"
          @change="executeSearch(false)"
        >
          <option :value="null">Все города</option>
          <option v-for="city in dictionaries.cities" :key="city.id" :value="city.id">
            {{ city.name }}
          </option>
        </select>
      </div>

      <div class="filter-group">
        <label class="filter-label">Сфера деятельности</label>
        <select
          v-model="filters.specializationId"
          class="input-main uniform-input"
          @change="executeSearch(false)"
        >
          <option :value="null">Все сферы</option>
          <option v-for="spec in dictionaries.specializations" :key="spec.id" :value="spec.id">
            {{ spec.name }}
          </option>
        </select>
      </div>

      <hr class="filter-divider" />

      <div class="filter-group">
        <label class="filter-label">Тип занятости</label>
        <div class="radio-group">
          <label class="custom-radio">
            <input
              type="radio"
              v-model="filters.type"
              :value="null"
              @change="executeSearch(false)"
            />
            <span class="radio-circle"></span> Любой
          </label>
          <label class="custom-radio">
            <input type="radio" v-model="filters.type" value="Job" @change="executeSearch(false)" />
            <span class="radio-circle"></span> Работа
          </label>
          <label class="custom-radio">
            <input
              type="radio"
              v-model="filters.type"
              value="Internship"
              @change="executeSearch(false)"
            />
            <span class="radio-circle"></span> Стажировка
          </label>
        </div>
      </div>

      <div class="filter-group">
        <label class="filter-label">Заработная плата от (₽)</label>
        <input
          v-model.number="filters.minSalary"
          type="number"
          class="input-main uniform-input no-spinners"
          placeholder="Например, 50000"
          @change="executeSearch(false)"
        />
      </div>

      <hr class="filter-divider" />

      <div class="filter-group">
        <label class="filter-label">Необходимые навыки</label>
        <AppTagAutocomplete
          v-model="filters.skills"
          :fetch-fn="fetchSkills"
          placeholder="C#, Vue.js..."
          @update:modelValue="executeSearch(false)"
        />
      </div>

      <div class="filter-group">
        <label class="filter-label">Дисциплины</label>
        <AppTagAutocomplete
          v-model="filters.courses"
          :fetch-fn="fetchCourses"
          placeholder="Базы данных..."
          @update:modelValue="executeSearch(false)"
        />
      </div>

      <hr class="filter-divider" />

      <button class="btn-main btn-secondary full-width" @click="resetFilters">
        Сбросить фильтры
      </button>
    </template>

    <template #results>
      <div class="results-meta" v-if="!isLoading">
        Найдено вакансий: <span class="highlight">{{ totalCount }}</span>
      </div>
      <div v-if="isLoading && results.length === 0" class="loading-state">Поиск вакансий...</div>
      <div v-else-if="results.length === 0" class="empty-state">
        По вашему запросу ничего не найдено. Попробуйте изменить параметры фильтрации.
      </div>

      <div v-else class="results-list">
        <VacancyCard v-for="vacancy in results" :key="vacancy.id" :vacancy="vacancy" />

        <button
          v-if="hasNextPage"
          class="btn-main btn-outline full-width mt-4"
          @click="executeSearch(true)"
          :disabled="isLoading"
        >
          {{ isLoading ? 'Загрузка...' : 'Загрузить еще' }}
        </button>
      </div>
    </template>
  </SearchPageLayout>
</template>

<style scoped>
.filter-group {
  margin-bottom: 16px;
  display: flex;
  flex-direction: column;
  gap: 8px;
}
.filter-label {
  font-size: 14px;
  font-weight: 600;
  color: var(--dark-text);
}
.filter-divider {
  border: none;
  border-top: 1px solid var(--gray-border);
  margin: 16px 0;
}

.uniform-input {
  height: 42px !important;
  box-sizing: border-box;
  padding: 0 12px;
  font-size: 14px;
}

.no-spinners::-webkit-outer-spin-button,
.no-spinners::-webkit-inner-spin-button {
  -webkit-appearance: none;
  margin: 0;
}

.radio-group {
  display: flex;
  flex-direction: column;
  gap: 8px;
}
.custom-radio {
  font-size: 14px;
  color: var(--dark-text);
  display: flex;
  align-items: center;
  cursor: pointer;
  user-select: none;
}
.custom-radio input {
  position: absolute;
  opacity: 0;
  cursor: pointer;
}
.radio-circle {
  width: 18px;
  height: 18px;
  border: 1.5px solid var(--gray-border);
  border-radius: 50%;
  margin-right: 10px;
  display: inline-flex;
  align-items: center;
  justify-content: center;
  transition: all 0.2s;
  background-color: #fff;
  flex-shrink: 0;
}
.custom-radio input:checked ~ .radio-circle {
  border-color: var(--susu-blue);
}
.custom-radio input:checked ~ .radio-circle::after {
  content: '';
  width: 10px;
  height: 10px;
  background-color: var(--susu-blue);
  border-radius: 50%;
}

.mt-4 {
  margin-top: 16px;
}
.full-width {
  width: 100%;
  justify-content: center;
}

.results-list {
  display: flex;
  flex-direction: column;
  gap: 16px;
}
</style>
