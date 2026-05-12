<!--Отображает карточку резюме в результатах поиска. -->
<script setup lang="ts">
import { ref } from 'vue'
import AppCard from '@/components/AppCard.vue'
import InteractionButtons from '@/components/InteractionButtons.vue'
import { downloadResumePdf } from '@/utils/fileUtils'
import type { ResumeSearchDto } from '@/types/search'
import DownloadButton from '@/components/DownloadButton.vue'

const props = defineProps<{
  resume: ResumeSearchDto
}>()

const isDownloading = ref(false)
const handleDownload = async () => {
  if (isDownloading.value) return
  isDownloading.value = true

  await downloadResumePdf(props.resume.id, props.resume.fullName)

  isDownloading.value = false
}
</script>

<template>
  <AppCard class="result-card">
    <div class="card-header">
      <div class="header-left">
        <router-link :to="`/students/${resume.studentId}`" class="resume-title">
          {{ resume.title }}
        </router-link>
        <div class="candidate-name">{{ resume.fullName }}</div>
      </div>

      <div class="actions-wrapper">
        <DownloadButton
          :is-loading="isDownloading"
          title="Скачать PDF резюме"
          @click="handleDownload"
        />

        <InteractionButtons
          :target-id="resume.studentId"
          favorite-type="Student"
          :block-user-id="resume.studentId"
          :initial-is-favorite="resume.isFavorite"
          :initial-is-blocked="resume.isBlocked"
        />
      </div>
    </div>

    <div class="education-info">
      <span v-if="resume.cityName" class="text-muted">{{ resume.cityName }}</span>
      <span v-if="resume.cityName && resume.universityName" class="dot-separator">•</span>
      <span v-if="resume.universityName" class="text-muted">{{ resume.universityName }}</span>

      <template v-if="resume.facultyName">
        <span class="dot-separator">•</span>
        <span class="text-muted">{{ resume.facultyName }}</span>
      </template>

      <template v-if="resume.studyDirectionName">
        <span class="dot-separator">•</span>
        <span class="text-muted">{{ resume.studyDirectionName }}</span>
      </template>

      <template v-if="resume.courseNumber">
        <span class="dot-separator">•</span>
        <span class="badge-type">{{ resume.courseNumber }} курс</span>
      </template>
    </div>

    <p class="resume-desc">
      {{ resume.description?.slice(0, 150) || 'Описание отсутствует...' }}
    </p>

    <div class="contacts-row" v-if="resume.email || resume.phone">
      <span v-if="resume.email" class="contact-item">Email: {{ resume.email }}</span>
      <span v-if="resume.phone" class="contact-item">Тел: {{ resume.phone }}</span>
    </div>

    <div class="tags-row" v-if="resume.completedCourses && resume.completedCourses.length > 0">
      <span class="section-label">Дисциплины:</span>
      <span
        v-for="course in resume.completedCourses.slice(0, 2)"
        :key="course"
        class="tag-pill course-pill"
      >
        {{ course }}
      </span>
      <span v-if="resume.completedCourses.length > 2" class="tag-pill text-muted">
        еще {{ resume.completedCourses.length - 2 }}...
      </span>
    </div>

    <div class="tags-row mt-2" v-if="resume.skills && resume.skills.length > 0">
      <span class="section-label">Навыки:</span>
      <span v-for="skill in resume.skills.slice(0, 6)" :key="skill" class="tag-pill">
        {{ skill }}
      </span>
      <span v-if="resume.skills.length > 6" class="tag-pill text-muted">...</span>
    </div>
  </AppCard>
</template>

<style scoped>
.result-card {
  padding: 24px;
  transition: border-color 0.2s;
}
.result-card:hover {
  border-color: var(--gray-border-focus);
}

.card-header {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  gap: 20px;
  margin-bottom: 12px;
}

.header-left {
  flex: 1;
  display: flex;
  flex-direction: column;
  gap: 4px;
}

.actions-wrapper {
  display: flex;
  align-items: center;
  gap: 12px;
  flex-shrink: 0;
}

.resume-title {
  font-size: 18px;
  font-weight: 600;
  color: var(--susu-blue);
  text-decoration: none;
}
.resume-title:hover {
  text-decoration: underline;
}
.candidate-name {
  font-size: 16px;
  font-weight: 500;
  color: var(--dark-text);
}
.education-info {
  display: flex;
  flex-wrap: wrap;
  align-items: center;
  gap: 8px;
  font-size: 14px;
  margin-bottom: 12px;
}
.dot-separator {
  color: var(--gray-border);
}
.badge-type {
  background: #f1f5f9;
  padding: 2px 8px;
  border-radius: 4px;
  font-size: 12px;
  color: var(--gray-text-focus);
  font-weight: 500;
}
.resume-desc {
  font-size: 14px;
  line-height: 1.5;
  color: var(--dark-text);
  margin: 0 0 16px 0;
}
.contacts-row {
  display: flex;
  gap: 16px;
  margin-bottom: 16px;
  font-size: 13px;
  color: var(--dark-text);
  font-weight: 500;
}

.tags-row {
  display: flex;
  flex-wrap: wrap;
  align-items: center;
  gap: 8px;
}
.mt-2 {
  margin-top: 8px;
}
.section-label {
  font-size: 12px;
  font-weight: 600;
  color: var(--dark-text);
  margin-right: 4px;
}
.tag-pill {
  background: var(--background-page);
  border: 1px solid var(--gray-border);
  padding: 2px 10px;
  border-radius: 12px;
  font-size: 12px;
  color: var(--gray-text-focus);
}
.course-pill {
  background: #f8fafc;
  border-style: dashed;
}
</style>
