<script setup lang="ts">
import { computed } from 'vue'

interface Props {
  fullName: string
  studentId: string
  university: string
  specialty: string
  course: number | null
  age: number | null
  resumeTitle?: string | null
  resumeId?: string | null
  skills?: string[]
}

const props = defineProps<Props>()

// Хелпер для формирования строки возраста и курса
const metaInfo = computed(() => {
  const parts = []
  if (props.age) parts.push(`${props.age} лет`)
  if (props.course) parts.push(`${props.course} курс`)
  return parts.join(', ')
})

// Отображаем только первые 5 навыков, чтобы не раздувать карточку
const displaySkills = computed(() => props.skills?.slice(0, 5) || [])
</script>

<template>
  <div class="student-info">
    <router-link :to="`/student/${studentId}`" class="student-name">
      {{ fullName }}
    </router-link>

    <div class="education-block">
      <span class="university-tag">{{ university }}</span>
      <span class="specialty-text">{{ specialty }}</span>
    </div>

    <p class="meta-text">{{ metaInfo }}</p>

    <div v-if="resumeTitle && resumeId" class="resume-block">
      <router-link :to="`/resume/${resumeId}`" class="resume-link">
        {{ resumeTitle }}
      </router-link>
    </div>

    <div v-if="displaySkills.length" class="skills-list">
      <span v-for="skill in displaySkills" :key="skill" class="skill-pill">
        {{ skill }}
      </span>
      <span v-if="skills && skills.length > 5" class="skills-more"> +{{ skills.length - 5 }} </span>
    </div>
  </div>
</template>

<style scoped>
.student-info {
  display: flex;
  flex-direction: column;
  gap: 6px;
}

.student-name {
  font-size: 16px;
  font-weight: 700;
  color: var(--dark-text);
  text-decoration: none;
}
.student-name:hover {
  text-decoration: underline;
}

.education-block {
  display: flex;
  align-items: center;
  gap: 8px;
  font-size: 13px;
}

.university-tag {
  font-weight: 700;
  color: var(--susu-blue); /* Или основной акцентный цвет */
}

.specialty-text {
  color: var(--gray-text);
}

.meta-text {
  font-size: 13px;
  color: var(--gray-text-focus);
  margin: 0;
}

.resume-block {
  margin-top: 4px;
}

.resume-link {
  font-size: 14px;
  font-weight: 700;
  color: var(--dark-text);
  text-decoration: none;
}
.resume-link:hover {
  color: var(--susu-blue);
}

/* Стили навыков (как статусы: синий полупрозрачный фон) */
.skills-list {
  display: flex;
  flex-wrap: wrap;
  gap: 6px;
  margin-top: 4px;
}

.skill-pill {
  background-color: rgba(37, 99, 235, 0.1); /* Прозрачный синий */
  color: #1e40af; /* Темно-синий текст */
  padding: 3px 10px;
  border-radius: 100px;
  font-size: 11px;
  font-weight: 600;
  white-space: nowrap;
}

.skills-more {
  font-size: 11px;
  color: var(--gray-text);
  align-self: center;
}
</style>
