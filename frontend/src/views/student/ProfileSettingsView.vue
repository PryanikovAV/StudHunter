<script setup lang="ts">
import { ref, onMounted, watch } from 'vue'
import apiClient from '@/api'
import AppTagAutocomplete from '@/components/AppTagAutocomplete.vue'

interface StudentProfileDto {
  id: string
  email: string
  firstName: string
  lastName: string
  patronymic: string | null
  contactPhone: string | null
  contactEmail: string | null
  cityId: string | null
  gender: 'Male' | 'Female' | null
  birthDate: string | null
  avatarUrl: string | null
  isForeign: boolean
  status: 'Studying' | 'SeekingJob' | 'SeekingInternship' | 'Interning' | 'Working'

  universityId: string | null
  facultyId: string | null
  departmentId: string | null
  studyDirectionId: string | null
  courseNumber: number
  studyForm: 'FullTime' | 'PartTime' | 'Correspondence'

  courseIds: string[]
  courses: { id: string; name: string }[]
}

const profile = ref<StudentProfileDto | null>(null)
const isLoading = ref(true)
const isSaving = ref(false)

const dictionaries = ref({
  universities: [] as { id: string; name: string }[],
  faculties: [] as { id: string; name: string }[],
  departments: [] as { id: string; name: string }[],
  directions: [] as { id: string; name: string }[],
  cities: [] as { id: string; name: string }[],
})

const fetchDictionaries = async () => {
  try {
    const res = await apiClient.get('/dictionaries/cities')
    dictionaries.value.cities = res.data
  } catch {
    console.warn('Ошибка загрузки городов')
  }

  try {
    const res = await apiClient.get('/dictionaries/universities')
    dictionaries.value.universities = res.data
  } catch {
    console.warn('Ошибка загрузки университетов')
  }

  try {
    const res = await apiClient.get('/dictionaries/faculties')
    dictionaries.value.faculties = res.data
  } catch {
    console.warn('Ошибка загрузки факультетов')
  }

  try {
    const res = await apiClient.get('/dictionaries/departments')
    dictionaries.value.departments = res.data
  } catch {
    console.warn('Ошибка загрузки кафедр')
  }

  try {
    const res = await apiClient.get('/dictionaries/specialities')
    dictionaries.value.directions = res.data
  } catch {
    console.warn('Ошибка загрузки направлений')
  }
}

const fetchCourses = async (query: string) => {
  const response = await apiClient.get(`/dictionaries/courses/search?q=${query}&limit=10`)
  return response.data
}

// 1. При смене ВУЗа сбрасываем всё, что ниже
watch(
  () => profile.value?.universityId,
  (newUniId, oldUniId) => {
    if (oldUniId !== undefined && profile.value) {
      profile.value.facultyId = null
      profile.value.departmentId = null
      profile.value.studyDirectionId = null
    }
  },
)

// 2. При смене Факультета сбрасываем Кафедру и Направление
watch(
  () => profile.value?.facultyId,
  (newFacId, oldFacId) => {
    if (oldFacId !== undefined && profile.value) {
      profile.value.departmentId = null
      profile.value.studyDirectionId = null
    }
  },
)

// 3. При смене Кафедры сбрасываем Направление (если они связаны логически)
watch(
  () => profile.value?.departmentId,
  (newDepId, oldDepId) => {
    if (oldDepId !== undefined && profile.value) {
      profile.value.studyDirectionId = null
    }
  },
)

const fetchProfile = async () => {
  try {
    const response = await apiClient.get<StudentProfileDto>('/student/profile')
    profile.value = response.data

    if (!profile.value.courses) {
      profile.value.courses = []
    }

    await fetchDictionaries()
  } catch (error) {
    console.error('Ошибка загрузки профиля', error)
  } finally {
    isLoading.value = false
  }
}

const saveProfile = async () => {
  if (!profile.value) return
  isSaving.value = true

  try {
    const payload = {
      ...profile.value,
      courseIds: profile.value.courses.map((c) => c.id),
    }

    await apiClient.put('/student/profile', payload)
    alert('Профиль успешно обновлен')
  } catch (error) {
    console.error('Ошибка сохранения', error)
    alert('Не удалось сохранить изменения')
  } finally {
    isSaving.value = false
  }
}

onMounted(fetchProfile)
</script>

<template>
  <div class="profile-page">
    <h1 class="page-title">Настройки профиля</h1>

    <div v-if="isLoading" class="loading">Загрузка...</div>

    <form v-else-if="profile" @submit.prevent="saveProfile" class="profile-form">
      <section class="form-section">
        <h2 class="section-title">Личные данные</h2>
        <div class="form-grid">
          <div class="form-group">
            <label>Фамилия</label>
            <input v-model="profile.lastName" type="text" required class="input-main input-fix" />
          </div>
          <div class="form-group">
            <label>Имя</label>
            <input v-model="profile.firstName" type="text" required class="input-main input-fix" />
          </div>
          <div class="form-group">
            <label>Отчество</label>
            <input v-model="profile.patronymic" type="text" class="input-main input-fix" />
          </div>
          <div class="form-group">
            <label>Дата рождения</label>
            <input v-model="profile.birthDate" type="date" class="input-main input-fix" />
          </div>
          <div class="form-group">
            <label>Пол</label>
            <select v-model="profile.gender" class="input-main input-fix">
              <option :value="null">Не указан</option>
              <option value="Male">Мужской</option>
              <option value="Female">Женский</option>
            </select>
          </div>
          <div class="form-group">
            <label>Текущий статус</label>
            <select v-model="profile.status" class="input-main input-fix">
              <option value="Studying">Учусь</option>
              <option value="SeekingJob">Ищу работу</option>
              <option value="SeekingInternship">Ищу стажировку</option>
            </select>
          </div>
        </div>
        <div class="form-group checkbox-group">
          <input type="checkbox" id="foreign" v-model="profile.isForeign" />
          <label for="foreign">Я иностранный студент</label>
        </div>
      </section>

      <section class="form-section">
        <h2 class="section-title">Образование</h2>

        <div class="form-group">
          <label>Город</label>
          <select v-model="profile.cityId" class="input-main input-fix">
            <option :value="null">Не указан</option>
            <option v-for="city in dictionaries.cities" :key="city.id" :value="city.id">
              {{ city.name }}
            </option>
          </select>
        </div>

        <div class="form-grid">
          <div class="form-group full-width">
            <label>Университет</label>
            <select v-model="profile.universityId" class="input-main input-fix">
              <option :value="null">Выберите вуз</option>
              <option v-for="uni in dictionaries.universities" :key="uni.id" :value="uni.id">
                {{ uni.name }}
              </option>
            </select>
          </div>

          <div class="form-group full-width">
            <label>Факультет / Институт</label>
            <select
              v-model="profile.facultyId"
              class="input-main input-fix"
              :disabled="!profile.universityId"
            >
              <option :value="null">
                {{ profile.universityId ? 'Выберите факультет' : 'Сначала выберите вуз' }}
              </option>
              <option v-for="fac in dictionaries.faculties" :key="fac.id" :value="fac.id">
                {{ fac.name }}
              </option>
            </select>
          </div>

          <div class="form-group">
            <label>Кафедра</label>
            <select
              v-model="profile.departmentId"
              class="input-main input-fix"
              :disabled="!profile.facultyId"
            >
              <option :value="null">
                {{ profile.facultyId ? 'Выберите кафедру' : 'Сначала выберите факультет' }}
              </option>
              <option v-for="dep in dictionaries.departments" :key="dep.id" :value="dep.id">
                {{ dep.name }}
              </option>
            </select>
          </div>

          <div class="form-group">
            <label>Направление подготовки</label>
            <select
              v-model="profile.studyDirectionId"
              class="input-main input-fix"
              :disabled="!profile.departmentId"
            >
              <option :value="null">
                {{ profile.departmentId ? 'Выберите направление' : 'Сначала выберите кафедру' }}
              </option>
              <option v-for="dir in dictionaries.directions" :key="dir.id" :value="dir.id">
                {{ dir.name }}
              </option>
            </select>
          </div>

          <div class="form-group">
            <label>Курс</label>
            <select v-model="profile.courseNumber" class="input-main input-fix">
              <option v-for="n in 6" :key="n" :value="n">{{ n }} курс</option>
            </select>
          </div>

          <div class="form-group">
            <label>Форма обучения</label>
            <select v-model="profile.studyForm" class="input-main input-fix">
              <option value="FullTime">Очная</option>
              <option value="PartTime">Очно-заочная</option>
              <option value="Correspondence">Заочная</option>
            </select>
          </div>

          <div class="form-group full-width" style="margin-top: 8px">
            <label>Пройденные дисциплины</label>
            <AppTagAutocomplete
              v-model="profile.courses"
              :fetch-fn="fetchCourses"
              placeholder="Введите название дисциплины (например, Математика)..."
            />
          </div>
        </div>
      </section>

      <section class="form-section">
        <h2 class="section-title">Контакты</h2>
        <div class="form-grid">
          <div class="form-group">
            <label>Телефон</label>
            <input
              v-model="profile.contactPhone"
              type="tel"
              class="input-main input-fix"
              placeholder="+7..."
            />
          </div>
          <div class="form-group">
            <label>Email для связи</label>
            <input v-model="profile.contactEmail" type="email" class="input-main input-fix" />
          </div>
        </div>
      </section>

      <div class="actions">
        <button type="submit" class="btn-main btn-dark" :disabled="isSaving">
          {{ isSaving ? 'Сохранение...' : 'Сохранить изменения' }}
        </button>
      </div>
    </form>
  </div>
</template>

<style scoped>
.profile-page {
  max-width: 800px;
  margin: 0 auto;
}

.form-section {
  background: var(--background-field);
  padding: 24px;
  border-radius: 12px;
  border: 1px solid var(--gray-border);
  margin-bottom: 24px;
}

.section-title {
  font-size: 18px;
  font-weight: 600;
  margin-bottom: 16px;
  color: var(--dark-text);
}

.form-grid {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 16px;
}

.full-width {
  grid-column: 1 / -1;
}

.form-group {
  display: flex;
  flex-direction: column;
  gap: 6px;
}

.form-group label {
  font-size: 13px;
  color: var(--gray-text-focus);
}

.input-fix {
  height: 43px;
}

.checkbox-group {
  flex-direction: row;
  align-items: center;
  gap: 8px;
  margin-top: 8px;
}

.actions {
  display: flex;
  justify-content: flex-end;
}

@media (max-width: 600px) {
  .form-grid {
    grid-template-columns: 1fr;
  }
}
</style>
