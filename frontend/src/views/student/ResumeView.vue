<script setup lang="ts">
import { ref, onMounted } from 'vue'
import apiClient from '@/api'
import AppCard from '@/components/AppCard.vue'
import AppInput from '@/components/AppInput.vue'
import AppTagAutocomplete from '@/components/AppTagAutocomplete.vue'
import { calculateAge } from '@/utils/dateUtils'

interface StudentProfileDto {
  firstName: string
  lastName: string
  patronymic: string | null
  contactPhone: string | null
  contactEmail: string | null
  cityId: string | null
  gender: 'Male' | 'Female' | null
  birthDate: string | null
  universityId: string | null
  facultyId: string | null
  departmentId: string | null
  studyDirectionId: string | null
  courseNumber: number
  studyForm: 'FullTime' | 'PartTime' | 'Correspondence'
  courses: { id: string; name: string }[]
}

interface ResumeFillDto {
  id?: string | null
  title: string
  description: string | null
  isDeleted: boolean // Добавлено поле из обновленного DTO
  skillIds: string[]
  skills: { id: string; name: string }[]
}

interface DictionaryItem {
  id: string
  name: string
}

const profile = ref<StudentProfileDto | null>(null)
const resume = ref<ResumeFillDto>({
  title: '',
  description: '',
  isDeleted: false,
  skillIds: [],
  skills: [],
})

const dictionaries = ref({
  cities: [] as DictionaryItem[],
  universities: [] as DictionaryItem[],
  faculties: [] as DictionaryItem[],
  departments: [] as DictionaryItem[],
  directions: [] as DictionaryItem[],
})

const isLoading = ref(true)
const isSaving = ref(false)
const isActionLoading = ref(false) // Отдельный флаг для скрытия/восстановления

const getDictName = (dict: DictionaryItem[], id: string | null | undefined): string | null => {
  if (!id) return null
  const item = dict.find((d) => d.id === id)
  return item ? item.name : null
}

const formatGender = (gender: string | null) => {
  if (gender === 'Male') return 'Мужской'
  if (gender === 'Female') return 'Женский'
  return null
}

const formatStudyForm = (form: string | undefined) => {
  if (form === 'FullTime') return 'Очная'
  if (form === 'PartTime') return 'Очно-заочная'
  if (form === 'Correspondence') return 'Заочная'
  return null
}

const fetchSkills = async (query: string) => {
  const response = await apiClient.get(`/dictionaries/skills/search?q=${query}&limit=10`)
  return response.data
}

const fetchData = async () => {
  isLoading.value = true
  try {
    const [profRes, resRes, cities, unis, facs, deps, dirs] = await Promise.all([
      apiClient.get<StudentProfileDto>('/students/me/profile'),
      apiClient.get<ResumeFillDto>('/my-resume'),
      apiClient.get('/dictionaries/cities'),
      apiClient.get('/dictionaries/universities'),
      apiClient.get('/dictionaries/faculties'),
      apiClient.get('/dictionaries/departments'),
      apiClient.get('/dictionaries/specialities'),
    ])

    profile.value = profRes.data
    dictionaries.value = {
      cities: cities.data,
      universities: unis.data,
      faculties: facs.data,
      departments: deps.data,
      directions: dirs.data,
    }

    if (resRes.data && resRes.data.id) {
      resume.value = resRes.data
      if (!resume.value.skills) resume.value.skills = []
    }
  } catch (error) {
    console.error('Ошибка загрузки данных', error)
  } finally {
    isLoading.value = false
  }
}

const saveResume = async () => {
  isSaving.value = true
  try {
    const payload = {
      ...resume.value,
      skillIds: resume.value.skills.map((s) => s.id),
    }

    await apiClient.put('/my-resume', payload)
    alert('Резюме успешно сохранено!')
  } catch (error) {
    console.error('Ошибка сохранения', error)
    alert('Не удалось сохранить резюме.')
  } finally {
    isSaving.value = false
  }
}

// Новая логика: Скрытие резюме (Delete)
const hideResume = async () => {
  if (
    !confirm(
      'Вы уверены, что хотите скрыть резюме? Оно больше не будет доступно для поиска работодателями.',
    )
  )
    return

  isActionLoading.value = true
  try {
    await apiClient.delete('/my-resume')
    resume.value.isDeleted = true
  } catch (error) {
    console.error('Ошибка скрытия', error)
    alert('Не удалось скрыть резюме.')
  } finally {
    isActionLoading.value = false
  }
}

// Новая логика: Восстановление резюме (Restore)
const restoreResume = async () => {
  isActionLoading.value = true
  try {
    await apiClient.post('/my-resume/restore')
    resume.value.isDeleted = false
  } catch (error) {
    console.error('Ошибка восстановления', error)
    alert('Не удалось восстановить резюме.')
  } finally {
    isActionLoading.value = false
  }
}

onMounted(fetchData)
</script>

<template>
  <div class="resume-page">
    <h1 class="page-title">Моё резюме</h1>

    <div v-if="isLoading" class="loading">Загрузка данных...</div>

    <template v-else-if="profile">
      <div v-if="resume.isDeleted" class="deleted-banner">
        <p>
          Вы скрыли своё резюме. Работодатели не смогут найти вас, однако вы можете продолжать
          откликаться на вакансии.
        </p>
      </div>

      <AppCard :class="['resume-sheet', { 'is-hidden': resume.isDeleted }]">
        <fieldset :disabled="resume.isDeleted" class="clean-fieldset">
          <form @submit.prevent="saveResume" class="resume-form">
            <header class="resume-header">
              <div class="header-main">
                <h2 class="full-name">
                  {{ profile.lastName }} {{ profile.firstName }} {{ profile.patronymic || '' }}
                </h2>
                <div class="personal-details">
                  <span v-if="profile.birthDate">{{ calculateAge(profile.birthDate) }}</span>
                  <span v-else class="text-missing">возраст не указан</span>
                  <span class="separator">•</span>
                  <span v-if="profile.gender">{{ formatGender(profile.gender) }}</span>
                  <span v-else class="text-missing">пол не указан</span>
                  <span class="separator">•</span>
                  <span v-if="profile.cityId">{{
                    getDictName(dictionaries.cities, profile.cityId)
                  }}</span>
                  <span v-else class="text-missing">город не указан (укажите в настройках)</span>
                </div>
              </div>

              <div class="header-contacts">
                <div class="contact-item">
                  <span class="contact-label">Телефон:</span>
                  <span v-if="profile.contactPhone">{{ profile.contactPhone }}</span>
                  <span v-else class="text-missing">не указан</span>
                </div>
                <div class="contact-item">
                  <span class="contact-label">Email:</span>
                  <span v-if="profile.contactEmail">{{ profile.contactEmail }}</span>
                  <span v-else class="text-missing">не указан</span>
                </div>
                <router-link to="/student/profile" class="edit-link"
                  >✏️ Изменить личные данные</router-link
                >
              </div>
            </header>

            <hr class="divider" />

            <div class="form-group editable-section">
              <label>Желаемая должность <span class="required">*</span></label>
              <AppInput
                v-model="resume.title"
                placeholder="Например: Frontend-разработчик (Vue) / Младший аналитик"
                required
                class="title-input"
              />
            </div>

            <section class="readonly-section">
              <h3 class="section-title">Образование</h3>
              <ul class="education-list">
                <li>
                  <span class="edu-label">ВУЗ:</span>
                  <span v-if="profile.universityId" class="edu-val">{{
                    getDictName(dictionaries.universities, profile.universityId)
                  }}</span>
                  <span v-else class="text-missing">укажите ВУЗ в настройках профиля</span>
                </li>
                <li>
                  <span class="edu-label">Факультет/Институт:</span>
                  <span v-if="profile.facultyId" class="edu-val">{{
                    getDictName(dictionaries.faculties, profile.facultyId)
                  }}</span>
                  <span v-else class="text-missing">не указан</span>
                </li>
                <li>
                  <span class="edu-label">Кафедра:</span>
                  <span v-if="profile.departmentId" class="edu-val">{{
                    getDictName(dictionaries.departments, profile.departmentId)
                  }}</span>
                  <span v-else class="text-missing">не указана</span>
                </li>
                <li>
                  <span class="edu-label">Направление:</span>
                  <span v-if="profile.studyDirectionId" class="edu-val">{{
                    getDictName(dictionaries.directions, profile.studyDirectionId)
                  }}</span>
                  <span v-else class="text-missing">не указано</span>
                </li>
                <li>
                  <span class="edu-label">Курс и форма:</span>
                  <span v-if="profile.courseNumber" class="edu-val"
                    >{{ profile.courseNumber }} курс, {{ formatStudyForm(profile.studyForm) }}</span
                  >
                  <span v-else class="text-missing">не указаны</span>
                </li>
              </ul>

              <div class="passed-courses">
                <span class="edu-label">Пройденные дисциплины:</span>
                <div
                  class="static-tags-container"
                  v-if="profile.courses && profile.courses.length > 0"
                >
                  <span v-for="course in profile.courses" :key="course.id" class="static-tag">{{
                    course.name
                  }}</span>
                </div>
                <span v-else class="text-missing block-missing"
                  >дисциплины не добавлены (укажите в настройках профиля)</span
                >
              </div>
            </section>

            <div class="form-group editable-section">
              <label>Ключевые навыки (для поиска)</label>
              <AppTagAutocomplete
                v-model="resume.skills"
                :fetch-fn="fetchSkills"
                placeholder="Введите навык (например, JavaScript, PostgreSQL, Figma)..."
              />
            </div>

            <div class="form-group editable-section">
              <label>О себе</label>
              <textarea
                v-model="resume.description"
                class="input-main textarea-field"
                placeholder="Опишите ваши учебные проекты, курсовые работы, достижения и мотивацию. Что вы умеете делать руками?"
                rows="6"
              ></textarea>
            </div>
          </form>
        </fieldset>
      </AppCard>

      <div class="resume-actions-panel">
        <template v-if="!resume.isDeleted">
          <button
            v-if="resume.id"
            class="btn-main btn-outline btn-danger"
            @click="hideResume"
            :disabled="isSaving || isActionLoading"
          >
            Скрыть резюме
          </button>

          <button
            class="btn-main btn-dark"
            @click="saveResume"
            :disabled="isSaving || isActionLoading"
          >
            {{ isSaving ? 'Сохранение...' : 'Сохранить изменения' }}
          </button>
        </template>

        <template v-else>
          <button class="btn-main btn-dark" @click="restoreResume" :disabled="isActionLoading">
            Восстановить резюме
          </button>
        </template>
      </div>
    </template>
  </div>
</template>

<style scoped>
.resume-page {
  max-width: 850px;
  margin: 0 auto;
  padding-bottom: 40px;
}
.page-title {
  margin-bottom: 24px;
  color: var(--dark-text);
}

/* Баннер удаления */
.deleted-banner {
  background-color: #f8fafc;
  border: 1px solid var(--gray-border);
  border-radius: 8px;
  padding: 12px 16px;
  margin-bottom: 20px;
  text-align: center;
}
.deleted-banner p {
  margin: 0;
  font-size: 14px;
  color: var(--gray-text-focus);
}

/* Карточка А4 */
.resume-sheet {
  padding: 40px;
  transition: all 0.3s ease;
}
.resume-sheet.is-hidden {
  opacity: 0.5;
  filter: grayscale(40%);
  pointer-events: none;
}
.clean-fieldset {
  border: none;
  padding: 0;
  margin: 0;
}

.resume-actions-panel {
  display: flex;
  justify-content: flex-end;
  gap: 16px;
  margin-top: 24px;
}

.resume-form {
  display: flex;
  flex-direction: column;
  gap: 32px;
}
.resume-header {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  flex-wrap: wrap;
  gap: 24px;
}

.header-main {
  flex: 1;
  min-width: 250px;
}

.full-name {
  font-size: 28px;
  font-weight: 700;
  margin-bottom: 8px;
  color: var(--dark-text);
  line-height: 1.2;
}

.personal-details {
  font-size: 15px;
  color: var(--gray-text-focus);
}

.separator {
  margin: 0 8px;
  color: var(--gray-border);
}

.header-contacts {
  display: flex;
  flex-direction: column;
  gap: 6px;
  font-size: 14px;
  background-color: #f9fafb;
  padding: 16px;
  border-radius: 8px;
  min-width: 200px;
}

.contact-item {
  display: flex;
  gap: 8px;
}

.contact-label {
  color: var(--gray-text-focus);
  font-weight: 500;
}

.edit-link {
  margin-top: 8px;
  font-size: 13px;
  color: var(--susu-blue, #005aaa);
  text-decoration: none;
  align-self: flex-start;
}
.edit-link:hover {
  text-decoration: underline;
}
.divider {
  border: none;
  border-top: 1px solid var(--gray-border);
  margin: 0;
}
.text-missing {
  color: #9ca3af;
  font-style: italic;
  font-size: 13px;
}
.block-missing {
  display: block;
  margin-top: 4px;
}
.section-title {
  font-size: 18px;
  font-weight: 600;
  margin-bottom: 16px;
  color: var(--dark-text);
  text-transform: uppercase;
  letter-spacing: 0.5px;
}
.education-list {
  list-style: none;
  padding: 0;
  margin: 0 0 16px 0;
  display: flex;
  flex-direction: column;
  gap: 8px;
  font-size: 15px;
}
.edu-label {
  font-weight: 600;
  color: var(--gray-text-focus);
  margin-right: 8px;
}
.edu-val {
  color: var(--dark-text);
}
.passed-courses {
  margin-top: 16px;
}
.static-tags-container {
  display: flex;
  flex-wrap: wrap;
  gap: 8px;
  margin-top: 8px;
}
.static-tag {
  display: inline-flex;
  align-items: center;
  background-color: var(--gray-border);
  color: var(--gray-text-focus);
  padding: 4px 12px;
  border-radius: 16px;
  font-size: 13px;
  border: 1px solid var(--gray-border);
}
.form-group {
  display: flex;
  flex-direction: column;
  gap: 8px;
}
.form-group label {
  font-size: 15px;
  font-weight: 600;
  color: var(--dark-text);
}
.required {
  color: #ef4444;
}
.title-input {
  font-size: 16px;
  font-weight: 500;
}
.textarea-field {
  width: 100%;
  box-sizing: border-box;
  padding: 16px;
  border: 1px solid var(--gray-border);
  border-radius: 8px;
  font-size: 15px;
  font-family: inherit;
  background-color: var(--background-field);
  resize: vertical;
  line-height: 1.5;
  transition: border-color 0.1s ease;
}
.textarea-field:focus {
  outline: none;
  border-color: var(--gray-border-focus);
}

@media (max-width: 640px) {
  .resume-sheet {
    padding: 24px 16px;
  }
  .header-contacts {
    width: 100%;
  }
}
</style>
