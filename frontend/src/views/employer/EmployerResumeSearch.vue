<script setup lang="ts">
import { ref, onMounted, watch } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import apiClient from '@/api'
import AppTagAutocomplete from '@/components/AppTagAutocomplete.vue'
import SearchPageLayout from '@/components/search/SearchPageLayout.vue'
import SearchBar from '@/components/SearchBar.vue'
import ResumeCard from '@/components/ResumeCard.vue'
import type { PagedResult, ResumeSearchDto } from '@/types/search'
import type { DictionaryItem } from '@/types/student'

const route = useRoute()
const router = useRouter()
const searchQuery = ref((route.query.q as string) || '')
const isLoading = ref(false)
const results = ref<ResumeSearchDto[]>([])
const totalCount = ref(0)
const hasNextPage = ref(false)

const dictionaries = ref({
  cities: [] as DictionaryItem[],
  universities: [] as DictionaryItem[],
  faculties: [] as DictionaryItem[],
  directions: [] as DictionaryItem[],
})

const filters = ref({
  cityId: null as string | null,
  isForeign: false as boolean | null,
  hasAvatar: false as boolean | null,
  statuses: [] as string[],
  studyForms: [] as string[],
  universityId: null as string | null,
  facultyId: null as string | null,
  studyDirectionId: null as string | null,
  courseNumber: null as number | null,
  courses: [] as { id: string; name: string }[],
  skills: [] as { id: string; name: string }[],
})

const favoriteIds = ref<string[]>([])
const blockedIds = ref<string[]>([])

// --- ЗАГРУЗКА СПРАВОЧНИКОВ И СОСТОЯНИЙ ---
const fetchInitialData = async () => {
  try {
    const [favRes, blockRes, cities, unis, facs, dirs] = await Promise.all([
      apiClient.get('/favorites'),
      apiClient.get('/blacklist'),
      apiClient.get('/dictionaries/cities'),
      apiClient.get('/dictionaries/universities'),
      apiClient.get('/dictionaries/faculties'),
      apiClient.get('/dictionaries/specialities'),
    ])

    const favItems = favRes.data.items || favRes.data
    favoriteIds.value = favItems.map((f: { targetId: string }) => f.targetId)

    const blockItems = blockRes.data.items || blockRes.data
    blockedIds.value = blockItems.map((b: { blockedUserId: string }) => b.blockedUserId)

    dictionaries.value = {
      cities: cities.data,
      universities: unis.data,
      faculties: facs.data,
      directions: dirs.data,
    }

    const chelyabinsk = cities.data.find((c: DictionaryItem) => c.name === 'Челябинск')
    if (chelyabinsk && !filters.value.cityId) {
      filters.value.cityId = chelyabinsk.id
    }
  } catch (error) {
    console.error('Ошибка загрузки данных', error)
  }
}

const fetchSkills = async (q: string) =>
  (await apiClient.get(`/dictionaries/skills/search?q=${q}&limit=10`)).data
const fetchCourses = async (q: string) =>
  (await apiClient.get(`/dictionaries/courses/search?q=${q}&limit=10`)).data

const onUniversityChange = () => {
  filters.value.facultyId = null
  filters.value.studyDirectionId = null
  executeSearch(false)
}

const onFacultyChange = () => {
  filters.value.studyDirectionId = null
  executeSearch(false)
}

const toggleForeign = (e: Event) => {
  const isChecked = (e.target as HTMLInputElement).checked
  filters.value.isForeign = isChecked ? null : false
  executeSearch(false)
}

const executeSearch = async (isLoadMore = false) => {
  if (!isLoadMore) {
    isLoading.value = true
    results.value = []
  }

  const currentPage = isLoadMore ? Math.floor(results.value.length / 10) + 1 : 1

  const payload = {
    searchTerm: searchQuery.value || null,
    skillIds: filters.value.skills.map((s) => s.id),
    statuses: filters.value.statuses,
    studyForms: filters.value.studyForms,
    cityId: filters.value.cityId,
    universityId: filters.value.universityId,
    facultyId: filters.value.facultyId,
    studyDirectionId: filters.value.studyDirectionId,
    courseNumber: filters.value.courseNumber,
    courseIds: filters.value.courses.map((c) => c.id),
    isForeign: filters.value.isForeign,
    hasAvatar: filters.value.hasAvatar || null,
    paging: { pageNumber: currentPage, pageSize: 10 },
  }

  try {
    const response = await apiClient.post<PagedResult<ResumeSearchDto>>('/search/resumes', payload)

    const mappedItems = response.data.items.map((r) => ({
      ...r,
      isFavorite: favoriteIds.value.includes(r.studentId),
      isBlocked: blockedIds.value.includes(r.studentId),
    }))

    if (isLoadMore) results.value.push(...mappedItems)
    else results.value = mappedItems

    totalCount.value = response.data.totalCount
    hasNextPage.value = response.data.hasNextPage
  } catch (error) {
    console.error('Ошибка поиска резюме:', error)
  } finally {
    isLoading.value = false
  }
}

let debounceTimeout: ReturnType<typeof setTimeout>
const handleSearchSubmit = (query: string) => {
  searchQuery.value = query
  const newQuery = { ...route.query }
  if (query) newQuery.q = query
  else delete newQuery.q

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
    isForeign: false,
    hasAvatar: false,
    statuses: [],
    studyForms: [],
    universityId: null,
    facultyId: null,
    studyDirectionId: null,
    courseNumber: null,
    courses: [],
    skills: [],
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

onMounted(async () => {
  await fetchInitialData()
  executeSearch(false)
})
</script>

<template>
  <SearchPageLayout>
    <template #search-bar>
      <SearchBar
        :initial-value="searchQuery"
        placeholder="Профессия, навык или имя студента..."
        @search="handleSearchSubmit"
      />
    </template>

    <template #filters>
      <div class="filter-group">
        <label class="filter-label">Город</label>
        <select
          v-model="filters.cityId"
          class="input-main compact-select"
          @change="executeSearch(false)"
        >
          <option :value="null">Все города</option>
          <option v-for="city in dictionaries.cities" :key="city.id" :value="city.id">
            {{ city.name }}
          </option>
        </select>
      </div>

      <div class="filter-group checkbox-group">
        <label class="checkbox-label">
          <input type="checkbox" :checked="filters.isForeign === null" @change="toggleForeign" />
          <span class="custom-check"></span>
          Включая иностранных студентов
        </label>
        <label class="checkbox-label">
          <input type="checkbox" v-model="filters.hasAvatar" @change="executeSearch(false)" />
          <span class="custom-check"></span>
          Только с фото
        </label>
      </div>

      <hr class="filter-divider" />

      <div class="filter-group">
        <label class="filter-label">Статус кандидата</label>
        <div class="checkbox-list">
          <label class="checkbox-label"
            ><input
              type="checkbox"
              value="Studying"
              v-model="filters.statuses"
              @change="executeSearch(false)"
            /><span class="custom-check"></span> Учится</label
          >
          <label class="checkbox-label"
            ><input
              type="checkbox"
              value="SeekingInternship"
              v-model="filters.statuses"
              @change="executeSearch(false)"
            /><span class="custom-check"></span> Ищет стажировку</label
          >
          <label class="checkbox-label"
            ><input
              type="checkbox"
              value="SeekingJob"
              v-model="filters.statuses"
              @change="executeSearch(false)"
            /><span class="custom-check"></span> Ищет работу</label
          >
          <label class="checkbox-label"
            ><input
              type="checkbox"
              value="Interning"
              v-model="filters.statuses"
              @change="executeSearch(false)"
            /><span class="custom-check"></span> Проходит стажировку</label
          >
          <label class="checkbox-label"
            ><input
              type="checkbox"
              value="Working"
              v-model="filters.statuses"
              @change="executeSearch(false)"
            /><span class="custom-check"></span> Работает</label
          >
        </div>
      </div>

      <div class="filter-group mt-3">
        <label class="filter-label">Форма обучения</label>
        <div class="checkbox-list">
          <label class="checkbox-label"
            ><input
              type="checkbox"
              value="FullTime"
              v-model="filters.studyForms"
              @change="executeSearch(false)"
            /><span class="custom-check"></span> Очная</label
          >
          <label class="checkbox-label"
            ><input
              type="checkbox"
              value="PartTime"
              v-model="filters.studyForms"
              @change="executeSearch(false)"
            /><span class="custom-check"></span> Очно-заочная</label
          >
          <label class="checkbox-label"
            ><input
              type="checkbox"
              value="Correspondence"
              v-model="filters.studyForms"
              @change="executeSearch(false)"
            /><span class="custom-check"></span> Заочная</label
          >
        </div>
      </div>

      <hr class="filter-divider" />

      <div class="filter-group">
        <label class="filter-label">Университет</label>
        <select
          v-model="filters.universityId"
          class="input-main compact-select"
          @change="onUniversityChange"
        >
          <option :value="null">Все ВУЗы</option>
          <option v-for="uni in dictionaries.universities" :key="uni.id" :value="uni.id">
            {{ uni.name }}
          </option>
        </select>
      </div>

      <div class="filter-group">
        <label class="filter-label">Факультет / Институт</label>
        <select
          v-model="filters.facultyId"
          class="input-main compact-select"
          :disabled="!filters.universityId"
          @change="onFacultyChange"
        >
          <option :value="null">Все факультеты</option>
          <option v-for="fac in dictionaries.faculties" :key="fac.id" :value="fac.id">
            {{ fac.name }}
          </option>
        </select>
      </div>

      <div class="filter-group">
        <label class="filter-label">Направление подготовки</label>
        <select
          v-model="filters.studyDirectionId"
          class="input-main compact-select"
          :disabled="!filters.facultyId"
          @change="executeSearch(false)"
        >
          <option :value="null">Все направления</option>
          <option v-for="dir in dictionaries.directions" :key="dir.id" :value="dir.id">
            {{ dir.name }}
          </option>
        </select>
      </div>

      <div class="filter-group">
        <label class="filter-label">Курс обучения</label>
        <select
          v-model.number="filters.courseNumber"
          class="input-main compact-select"
          @change="executeSearch(false)"
        >
          <option :value="null">Любой курс</option>
          <option v-for="n in 6" :key="n" :value="n">{{ n }} курс</option>
        </select>
      </div>

      <hr class="filter-divider" />

      <div class="filter-group">
        <label class="filter-label">Ключевые навыки</label>
        <AppTagAutocomplete
          v-model="filters.skills"
          :fetch-fn="fetchSkills"
          placeholder="C#, Vue.js..."
          @update:modelValue="executeSearch(false)"
        />
      </div>

      <div class="filter-group">
        <label class="filter-label">Учебные дисциплины</label>
        <AppTagAutocomplete
          v-model="filters.courses"
          :fetch-fn="fetchCourses"
          placeholder="Базы данных..."
          @update:modelValue="executeSearch(false)"
        />
      </div>

      <button class="btn-main btn-secondary full-width mt-4" @click="resetFilters">
        Сбросить фильтры
      </button>
    </template>

    <template #results>
      <div class="results-meta" v-if="!isLoading">
        Найдено кандидатов: <span class="highlight">{{ totalCount }}</span>
      </div>

      <div v-if="isLoading && results.length === 0" class="loading-state">
        Анализ базы резюме...
      </div>
      <div v-else-if="results.length === 0" class="empty-state">
        По вашему запросу кандидатов не найдено. Попробуйте смягчить фильтры.
      </div>

      <div v-else class="results-list">
        <ResumeCard v-for="resume in results" :key="resume.id" :resume="resume" />

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
  margin-bottom: 20px;
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
  margin: 24px 0;
}
.compact-select {
  height: 38px !important;
  padding: 0 12px;
  font-size: 14px;
}
.compact-select:disabled {
  background-color: #f3f4f6;
  cursor: not-allowed;
  opacity: 0.7;
}

.checkbox-group {
  gap: 12px;
}
.checkbox-list {
  display: flex;
  flex-direction: column;
  gap: 10px;
}
.checkbox-label {
  display: flex;
  align-items: center;
  font-size: 14px;
  color: var(--dark-text);
  cursor: pointer;
  user-select: none;
}
.checkbox-label input {
  position: absolute;
  opacity: 0;
  cursor: pointer;
}
.custom-check {
  width: 18px;
  height: 18px;
  border: 1.5px solid var(--gray-border);
  border-radius: 4px;
  margin-right: 10px;
  display: inline-flex;
  align-items: center;
  justify-content: center;
  transition: all 0.2s;
  background-color: #fff;
}
.checkbox-label input:checked ~ .custom-check {
  background-color: var(--susu-blue);
  border-color: var(--susu-blue);
}
.checkbox-label input:checked ~ .custom-check::after {
  content: '';
  width: 5px;
  height: 10px;
  border: solid white;
  border-width: 0 2px 2px 0;
  transform: rotate(45deg);
  margin-bottom: 2px;
}

.mt-3 {
  margin-top: 12px;
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
