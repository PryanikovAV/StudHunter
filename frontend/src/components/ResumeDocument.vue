<script setup lang="ts">
import { computed } from 'vue'
import { calculateAge } from '@/utils/dateUtils'
import AppTagAutocomplete from '@/components/AppTagAutocomplete.vue'
import AppCard from '@/components/AppCard.vue'
import type { ResumeDocumentDto, TagItem } from '@/types/resume'

const props = withDefaults(
  defineProps<{
    mode: 'student' | 'employer'
    data: ResumeDocumentDto
    fetchSkillsFn?: (query: string) => Promise<TagItem[]>
  }>(),
  {
    mode: 'employer',
    fetchSkillsFn: async () => [],
  },
)

const emit = defineEmits<{
  (e: 'update:title', value: string): void
  (e: 'update:description', value: string): void
  (e: 'update:skillsList', value: TagItem[]): void
}>()

const localTitle = computed({
  get: () => props.data.title,
  set: (val) => emit('update:title', val),
})
const localDescription = computed({
  get: () => props.data.description || '',
  set: (val) => emit('update:description', val),
})
const localSkills = computed({
  get: () => props.data.skillsList || [],
  set: (val) => emit('update:skillsList', val),
})

const ageText = computed(() => calculateAge(props.data.birthDate))
const genderText = computed(() => {
  if (props.data.gender === 'Male') return 'Мужской'
  if (props.data.gender === 'Female') return 'Женский'
  return props.data.gender
})
const studyFormText = computed(() => {
  if (props.data.studyForm === 'FullTime') return 'Очная'
  if (props.data.studyForm === 'PartTime') return 'Очно-заочная'
  if (props.data.studyForm === 'Correspondence') return 'Заочная'
  return props.data.studyForm
})
const formattedUpdateDate = computed(() => {
  if (!props.data.updatedAt) return ''
  return new Date(props.data.updatedAt).toLocaleDateString('ru-RU', {
    day: 'numeric',
    month: 'long',
    year: 'numeric',
  })
})

const statusMap: Record<string, string> = {
  Studying: 'Учусь',
  SeekingInternship: 'Ищу стажировку',
  SeekingJob: 'Ищу работу',
  Interning: 'На стажировке',
  Working: 'Работаю',
  '1': 'Учусь',
  '2': 'Ищу стажировку',
  '3': 'Ищу работу',
  '4': 'На стажировке',
  '5': 'Работаю',
}
const statusText = computed(() => {
  if (!props.data.status) return ''
  return statusMap[props.data.status.toString()] || props.data.status
})

const initials = computed(() => {
  if (!props.data.fullName) return '?'
  return props.data.fullName
    .split(' ')
    .slice(0, 2)
    .map((w) => w[0])
    .join('')
    .toUpperCase()
})

const handleTextareaResize = (event: Event) => {
  const target = event.target as HTMLTextAreaElement
  target.style.height = 'auto'
  target.style.height = `${target.scrollHeight}px`
}
</script>

<template>
  <AppCard :class="['resume-sheet', { 'is-hidden': data.isDeleted }]">
    <div class="resume-grid">
      <div class="header-left">
        <template v-if="mode === 'student'">
          <textarea
            v-model="localTitle"
            class="title-input"
            placeholder="Желаемая должность"
            rows="1"
            @input="handleTextareaResize"
          ></textarea>
        </template>
        <template v-else>
          <h1 class="resume-title">{{ data.title }}</h1>
        </template>

        <div class="candidate-name">{{ data.fullName }}</div>

        <div class="meta-row">
          <span v-if="ageText">{{ ageText }}</span>
          <span v-if="ageText && genderText" class="separator">•</span>
          <span v-if="genderText">{{ genderText }}</span>
          <span v-if="(ageText || genderText) && data.cityName" class="separator">•</span>
          <span v-if="data.cityName">г. {{ data.cityName }}</span>
          <span v-if="data.citizenship" class="separator">•</span>
          <span v-if="data.citizenship">{{ data.citizenship }}</span>
        </div>

        <div class="update-date" v-if="data.updatedAt">Обновлено: {{ formattedUpdateDate }}</div>
      </div>

      <div class="header-right">
        <div class="avatar-wrapper">
          <div class="avatar">
            <img v-if="data.avatarUrl" :src="data.avatarUrl" alt="Avatar" />
            <div v-else class="avatar-placeholder">{{ initials }}</div>
          </div>
          <div class="status-badge" v-if="statusText">{{ statusText }}</div>
        </div>
      </div>
    </div>

    <section class="resume-section" v-if="data.email || data.phone">
      <h2 class="section-title">Контакты</h2>
      <div class="section-content">
        <div v-if="data.email" class="info-row">
          <span class="label">Email:</span> <span class="val">{{ data.email }}</span>
        </div>
        <div v-if="data.phone" class="info-row">
          <span class="label">Телефон:</span> <span class="val">{{ data.phone }}</span>
        </div>
      </div>
    </section>

    <section
      class="resume-section"
      v-if="data.universityName || data.facultyName || data.departmentName"
    >
      <h2 class="section-title">Образование</h2>
      <div class="section-content">
        <div v-if="data.universityName" class="info-row">
          <span class="label">ВУЗ:</span> <span class="val">{{ data.universityName }}</span>
        </div>
        <div v-if="data.facultyName" class="info-row">
          <span class="label">Факультет/Институт:</span>
          <span class="val">{{ data.facultyName }}</span>
        </div>
        <div v-if="data.departmentName" class="info-row">
          <span class="label">Кафедра:</span> <span class="val">{{ data.departmentName }}</span>
        </div>
        <div v-if="data.studyDirectionName" class="info-row">
          <span class="label">Направление:</span>
          <span class="val">{{ data.studyDirectionName }}</span>
        </div>
        <div v-if="data.courseNumber || studyFormText" class="info-row">
          <span class="label">Курс и форма обучения:</span>
          <span class="val">
            <template v-if="data.courseNumber">{{ data.courseNumber }} курс</template>
            <template v-if="data.courseNumber && studyFormText">, </template>
            <template v-if="studyFormText">{{ studyFormText }}</template>
          </span>
        </div>
      </div>
    </section>

    <section
      class="resume-section"
      v-if="data.completedCourses && data.completedCourses.length > 0"
    >
      <h2 class="section-title">Пройденные дисциплины</h2>
      <div class="tags-wrapper">
        <span v-for="course in data.completedCourses" :key="course" class="static-tag">{{
          course
        }}</span>
      </div>
    </section>

    <section class="resume-section">
      <h2 class="section-title">Ключевые навыки</h2>
      <div v-if="mode === 'student'">
        <AppTagAutocomplete
          v-model="localSkills"
          :fetch-fn="fetchSkillsFn"
          placeholder="Введите навык (например, JavaScript, PostgreSQL)..."
        />
      </div>
      <div v-else-if="data.skillNames && data.skillNames.length > 0" class="tags-wrapper">
        <span v-for="skillName in data.skillNames" :key="skillName" class="static-tag">
          {{ skillName }}
        </span>
      </div>
      <div v-else class="text-missing">Навыки не указаны</div>
    </section>

    <section class="resume-section">
      <h2 class="section-title">О себе</h2>
      <div v-if="mode === 'student'">
        <textarea
          v-model="localDescription"
          class="about-textarea"
          rows="2"
          maxlength="2500"
          placeholder="Опишите ваши учебные проекты, курсовые работы, достижения..."
          @input="handleTextareaResize"
        ></textarea>
      </div>
      <div v-else-if="data.description" class="about-text">
        {{ data.description }}
      </div>
      <div v-else class="text-missing">Информация не заполнена</div>
    </section>

    <hr class="actions-divider" />
    <div class="actions-bar">
      <div class="actions-left">
        <slot name="actions-left"></slot>
      </div>
      <div class="actions-right">
        <slot name="actions-right"></slot>
      </div>
    </div>
  </AppCard>
</template>

<style scoped>
.resume-sheet {
  padding: 40px;
  transition: all 0.3s ease;
}
.resume-sheet.is-hidden {
  opacity: 0.5;
  filter: grayscale(40%);
}
.resume-grid {
  display: grid;
  grid-template-columns: 3fr 1fr;
  gap: 24px;
  margin-bottom: 32px;
}
.header-left {
  display: flex;
  flex-direction: column;
  gap: 8px;
}

.title-input {
  font-size: 20px;
  font-weight: 600;
  color: var(--dark-text);
  width: 100%;
  box-sizing: border-box;
  border: 1px solid var(--gray-border);
  border-radius: 8px;
  outline: none;
  background-color: var(--background-field, #f9fafb);
  resize: none;
  padding: 12px 16px;
  margin: 0 0 16px 0;
  line-height: 1.4;
  transition:
    border-color 0.2s ease,
    background-color 0.2s ease;
}
.title-input:hover {
  border-color: #d1d5db;
}
.title-input:focus {
  border-color: var(--susu-blue);
  background-color: #ffffff;
}

.title-input::placeholder {
  color: rgba(17, 24, 39, 0.4);
  font-weight: 500;
}
.resume-title {
  font-size: 28px;
  font-weight: 700;
  color: var(--dark-text);
  margin: 0;
  line-height: 1.2;
}
.resume-title {
  font-size: 28px;
  font-weight: 700;
  color: var(--dark-text);
  margin: 0;
  line-height: 1.2;
}

.candidate-name {
  font-size: 18px;
  font-weight: 600;
  color: var(--dark-text);
}
.meta-row {
  font-size: 15px;
  color: var(--dark-text);
  display: flex;
  align-items: center;
  flex-wrap: wrap;
  gap: 8px;
}
.separator {
  color: var(--gray-border);
}
.update-date {
  font-size: 13px;
  color: var(--gray-text-focus);
  margin-top: 4px;
}

.header-right {
  display: flex;
  flex-direction: column;
  align-items: flex-end;
}
.avatar-wrapper {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 8px;
}
.avatar {
  width: 100px;
  height: 100px;
  border-radius: 12px;
  overflow: hidden;
  background-color: var(--background-page);
  border: 1px solid var(--gray-border);
}
.avatar img {
  width: 100%;
  height: 100%;
  object-fit: cover;
}
.avatar-placeholder {
  width: 100%;
  height: 100%;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 32px;
  color: var(--gray-text-focus);
  background: linear-gradient(135deg, #f8fafc 0%, #e2e8f0 100%);
}
.status-badge {
  font-size: 14px;
  font-weight: 600;
  color: var(--susu-blue);
  text-align: center;
}

.resume-section {
  margin-bottom: 24px;
}
.section-title {
  font-size: 18px;
  font-weight: 600;
  color: var(--dark-text);
  margin: 0 0 16px 0;
  border-bottom: 1px solid var(--gray-border);
  padding-bottom: 8px;
}
.info-row {
  font-size: 15px;
  color: var(--dark-text);
  margin-bottom: 8px;
}
.info-row .label {
  font-weight: 600;
  color: var(--gray-text-focus);
  margin-right: 6px;
}

.tags-wrapper {
  display: flex;
  flex-wrap: wrap;
  gap: 8px;
}
.static-tag {
  background: #f8fafc;
  border: 1px solid var(--gray-border);
  padding: 6px 12px;
  border-radius: 6px;
  font-size: 13px;
  color: var(--dark-text);
  display: inline-flex;
  align-items: center;
}

.about-textarea {
  width: 100%;
  box-sizing: border-box;
  padding: 16px;
  border: 1px solid var(--gray-border);
  border-radius: 8px;
  font-size: 15px;
  font-family: inherit;
  background-color: var(--background-field);
  resize: none;
  line-height: 1.5;
  transition: border-color 0.2s;
}
.about-textarea:focus {
  outline: none;
  border-color: var(--susu-blue);
}
.about-text {
  white-space: pre-wrap;
  font-size: 15px;
  line-height: 1.6;
  color: var(--dark-text);
}
.text-missing {
  color: #9ca3af;
  font-style: italic;
  font-size: 14px;
}

.actions-divider {
  border: none;
  border-top: 1px solid var(--gray-border);
  margin: 32px 0 24px 0;
}
.actions-bar {
  display: flex;
  justify-content: space-between;
  align-items: center;
  flex-wrap: wrap;
  gap: 20px;
}
.actions-left {
  display: flex;
  align-items: center;
  gap: 12px;
}
.actions-right {
  display: flex;
  align-items: center;
  gap: 12px;
}

@media (max-width: 640px) {
  .resume-grid {
    grid-template-columns: 1fr;
  }
  .header-right {
    align-items: flex-start;
    order: -1;
  }
  .actions-bar {
    flex-direction: column-reverse;
    align-items: stretch;
  }
  .actions-left,
  .actions-right {
    justify-content: center;
    width: 100%;
  }
}
</style>
