<script setup lang="ts">
import { ref, onMounted, computed } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import apiClient from '@/api'
import AppCard from '@/components/AppCard.vue'
import AppInput from '@/components/AppInput.vue'
import AppTagAutocomplete from '@/components/AppTagAutocomplete.vue'

interface VacancyFillDto {
  id?: string | null
  title: string
  description: string | null
  salary: number | null
  type: string
  skillIds: string[]
  courseIds: string[]
  skills: { id: string; name: string }[]
  courses: { id: string; name: string }[]
}

const route = useRoute()
const router = useRouter()

const vacancyId = computed(() => route.params.id as string | undefined)
const isEditMode = computed(() => !!vacancyId.value)

const isLoading = ref(isEditMode.value)
const isSaving = ref(false)

const emit = defineEmits(['update-hero'])

const vacancy = ref<VacancyFillDto>({
  title: '',
  description: '',
  salary: null,
  type: 'FullTime',
  skillIds: [],
  courseIds: [],
  skills: [],
  courses: [],
})

const fetchSkills = async (q: string) =>
  (await apiClient.get(`/dictionaries/skills/search?q=${q}&limit=10`)).data
const fetchCourses = async (q: string) =>
  (await apiClient.get(`/dictionaries/courses/search?q=${q}&limit=10`)).data

const loadVacancy = async () => {
  if (!isEditMode.value) return

  try {
    const response = await apiClient.get<VacancyFillDto>(`/employer/vacancies/${vacancyId.value}`)
    vacancy.value = response.data
    if (!vacancy.value.skills) vacancy.value.skills = []
    if (!vacancy.value.courses) vacancy.value.courses = []
  } catch (error) {
    console.error('Ошибка загрузки вакансии:', error)
    alert('Не удалось загрузить вакансию')
    router.push('/employer/vacancies')
  } finally {
    isLoading.value = false
  }
}

const saveVacancy = async () => {
  isSaving.value = true
  try {
    const payload = {
      ...vacancy.value,
      skillIds: vacancy.value.skills.map((s) => s.id),
      courseIds: vacancy.value.courses.map((c) => c.id),
    }

    if (isEditMode.value) {
      await apiClient.put(`/employer/vacancies/${vacancyId.value}`, payload)
    } else {
      await apiClient.post('/employer/vacancies', payload)
    }

    emit('update-hero')

    router.push('/employer/vacancies')
  } catch (error) {
    console.error('Ошибка сохранения:', error)
    alert('Не удалось сохранить вакансию. Проверьте правильность полей.')
  } finally {
    isSaving.value = false
  }
}

onMounted(loadVacancy)
</script>

<template>
  <div class="vacancy-edit-page">
    <div class="page-header">
      <button class="btn-main btn-secondary" @click="router.push('/employer/vacancies')">
        ← К списку вакансий
      </button>
      <h1 class="page-title" style="margin: 0">
        {{ isEditMode ? 'Редактирование вакансии' : 'Новая вакансия' }}
      </h1>
    </div>

    <div v-if="isLoading" class="loading">Загрузка данных...</div>

    <AppCard v-else class="form-card">
      <form @submit.prevent="saveVacancy" class="edit-form">
        <div class="form-grid">
          <div class="form-group full-width">
            <label>Должность <span class="required">*</span></label>
            <AppInput
              v-model="vacancy.title"
              placeholder="Например: Junior Backend Developer"
              required
            />
          </div>

          <div class="form-group">
            <label>Тип занятости <span class="required">*</span></label>
            <select v-model="vacancy.type" class="input-main" required>
              <option value="FullTime">Полная занятость</option>
              <option value="PartTime">Частичная занятость</option>
              <option value="Internship">Стажировка</option>
              <option value="Project">Проектная работа</option>
            </select>
          </div>

          <div class="form-group">
            <label>Зарплата (₽, на руки)</label>
            <AppInput v-model.number="vacancy.salary" type="number" placeholder="Например: 60000" />
          </div>
        </div>

        <div class="form-group">
          <label>Описание вакансии (Обязанности, Требования, Условия)</label>
          <textarea
            v-model="vacancy.description"
            class="input-main textarea-field"
            rows="8"
            placeholder="Опишите, чем предстоит заниматься, что вы ждете от кандидата и что предлагаете взамен..."
          ></textarea>
        </div>

        <div class="form-grid">
          <div class="form-group full-width">
            <label>Ожидаемые навыки (Skills)</label>
            <AppTagAutocomplete
              v-model="vacancy.skills"
              :fetch-fn="fetchSkills"
              placeholder="Введите навык (C#, Figma)..."
            />
          </div>

          <div class="form-group full-width">
            <label>Требуемые дисциплины</label>
            <AppTagAutocomplete
              v-model="vacancy.courses"
              :fetch-fn="fetchCourses"
              placeholder="Например: Базы данных..."
            />
          </div>
        </div>

        <div class="actions">
          <button type="submit" class="btn-main btn-dark" :disabled="isSaving">
            {{
              isSaving
                ? 'Сохранение...'
                : isEditMode
                  ? 'Сохранить изменения'
                  : 'Опубликовать вакансию'
            }}
          </button>
        </div>
      </form>
    </AppCard>
  </div>
</template>

<style scoped>
.vacancy-edit-page {
  max-width: 800px;
  margin: 0 auto;
}

.page-header {
  display: flex;
  align-items: center;
  gap: 24px;
  margin-bottom: 24px;
}

.form-card {
  padding: 32px;
}

.edit-form {
  display: flex;
  flex-direction: column;
  gap: 24px;
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
  gap: 8px;
}

.form-group label {
  font-size: 14px;
  font-weight: 500;
  color: var(--dark-text);
}

.required {
  color: var(--red-text-error);
}

.textarea-field {
  height: auto;
  padding: 16px;
  resize: vertical;
}

.actions {
  display: flex;
  justify-content: flex-end;
  border-top: 1px solid var(--gray-border);
  padding-top: 24px;
  margin-top: 8px;
}

@media (max-width: 640px) {
  .form-grid {
    grid-template-columns: 1fr;
  }
}
</style>
