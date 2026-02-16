<script setup lang="ts">
import { ref, onMounted } from 'vue'
import apiClient from '@/api'
import AppCard from '@/components/AppCard.vue'
import AppInput from '@/components/AppInput.vue'
import AppTagAutocomplete from '@/components/AppTagAutocomplete.vue'

// Интерфейс из ProfileSettings (нам нужно только проверить наличие университета)
interface ProfileCheckDto {
  universityId: string | null
  facultyId: string | null
}

// Тот самый ResumeFillDto с бэкенда
interface ResumeFillDto {
  id?: string | null
  title: string
  description: string | null
  skillIds: string[]
  skills: { id: string; name: string }[]
}

const profile = ref<ProfileCheckDto | null>(null)
const resume = ref<ResumeFillDto>({
  title: '',
  description: '',
  skillIds: [],
  skills: [],
})

const isLoading = ref(true)
const isSaving = ref(false)

// Функция поиска навыков для нашего AppTagAutocomplete
const fetchSkills = async (query: string) => {
  // Убедись, что на бэкенде есть контроллер Dictionaries/skills/search
  const response = await apiClient.get(`/dictionaries/skills/search?q=${query}&limit=10`)
  return response.data
}

const fetchData = async () => {
  try {
    isLoading.value = true

    // Делаем два независимых запроса параллельно
    const [profileRes, resumeRes] = await Promise.all([
      apiClient.get<ProfileCheckDto>('/student/profile'),
      apiClient.get<ResumeFillDto>('/my-resume'),
    ])

    profile.value = profileRes.data

    // Если резюме пришло с бэкенда (не null), заполняем форму
    if (resumeRes.data && resumeRes.data.id) {
      resume.value = resumeRes.data

      // Страховка: если бэкенд не вернул массив skills, создаем пустой, чтобы компонент не упал
      if (!resume.value.skills) {
        resume.value.skills = []
      }
    }
  } catch (error) {
    console.error('Ошибка загрузки данных резюме', error)
  } finally {
    isLoading.value = false
  }
}

const saveResume = async () => {
  isSaving.value = true
  try {
    // Собираем массив ID из "капсул" навыков перед отправкой
    const payload = {
      ...resume.value,
      skillIds: resume.value.skills.map((s) => s.id),
    }

    await apiClient.put('/my-resume', payload)
    alert('Резюме успешно сохранено!')
  } catch (error) {
    console.error('Ошибка сохранения резюме', error)
    alert('Не удалось сохранить резюме. Проверьте правильность заполнения.')
  } finally {
    isSaving.value = false
  }
}

onMounted(fetchData)
</script>

<template>
  <div class="resume-page">
    <h1 class="page-title">Моё резюме</h1>

    <div v-if="isLoading" class="loading">Загрузка данных...</div>

    <template v-else>
      <AppCard v-if="!profile?.universityId" class="warning-card">
        <h3>Профиль не заполнен</h3>
        <p>Для создания резюме необходимо указать место обучения в настройках профиля.</p>
        <router-link to="/student/profile" class="btn-main">Перейти в настройки</router-link>
      </AppCard>

      <AppCard v-else class="resume-card">
        <form @submit.prevent="saveResume" class="resume-form">
          <div class="form-group">
            <label>Желаемая должность (Title) <span class="required">*</span></label>
            <AppInput
              v-model="resume.title"
              placeholder="Например: Frontend-разработчик (Vue)"
              required
            />
          </div>

          <div class="form-group">
            <label>О себе (Description)</label>
            <textarea
              v-model="resume.description"
              class="input-main textarea-field"
              placeholder="Расскажите о своих проектах, достижениях и мотивации..."
              rows="5"
            ></textarea>
          </div>

          <div class="form-group">
            <label>Ключевые навыки</label>
            <AppTagAutocomplete
              v-model="resume.skills"
              :fetch-fn="fetchSkills"
              placeholder="Введите навык (например, JavaScript, PostgreSQL)..."
            />
          </div>

          <div class="actions">
            <button type="submit" class="btn-main" :disabled="isSaving">
              {{ isSaving ? 'Сохранение...' : 'Сохранить резюме' }}
            </button>
          </div>
        </form>
      </AppCard>
    </template>
  </div>
</template>

<style scoped>
.resume-page {
  max-width: 800px;
  margin: 0 auto;
}

.page-title {
  margin-bottom: 24px;
  color: var(--dark-text);
}

.warning-card {
  text-align: center;
  padding: 40px 20px;
  background-color: #fff9e6; /* Легкий желтоватый фон для предупреждения */
  border-color: #ffe082;
}

.warning-card h3 {
  margin-bottom: 8px;
  color: #bfa000;
}

.warning-card p {
  margin-bottom: 20px;
  color: var(--gray-text);
}

.resume-form {
  display: flex;
  flex-direction: column;
  gap: 20px;
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
  color: red;
}

/* Стилизируем textarea под наш AppInput */
.textarea-field {
  width: 100%;
  box-sizing: border-box;
  padding: 12px;
  border: 1px solid var(--gray-border);
  border-radius: 8px;
  font-size: 14px;
  font-family: inherit;
  background-color: var(--background-field);
  resize: vertical; /* Позволяет тянуть только по высоте */
  transition: border-color 0.1s ease;
}

.textarea-field:focus {
  outline: none;
  border-color: var(--susu-blue);
}

.actions {
  display: flex;
  justify-content: flex-end;
  margin-top: 10px;
}
</style>
