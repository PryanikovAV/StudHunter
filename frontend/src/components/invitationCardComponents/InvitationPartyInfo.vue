<script setup lang="ts">
import { computed } from 'vue'
import StudentInfo from './StudentInfo.vue'
import EmployerInfo from './EmployerInfo.vue'
import IconUser from '@/components/icons/IconUser.vue'
import IconBuilding from '@/components/icons/IconBuilding.vue'

interface InvitationCandidateDto {
  studentId: string
  fullName: string
  avatarUrl: string | null
  universityAbbreviation?: string | null
  specialtyName?: string | null
  courseNumber?: number | null
  age?: number | null
  resumeTitle?: string | null
  resumeId?: string | null
  skills?: string[]
}

interface InvitationJobDto {
  employerId: string
  companyName: string
  avatarUrl: string | null
  vacancyId?: string | null
  vacancyTitle?: string | null
  salary?: number | null
}

const props = defineProps<{
  type: string
  direction: string
  role: 'student' | 'employer'
  candidate: InvitationCandidateDto
  job: InvitationJobDto
}>()

const contextText = computed(() => {
  const isStudent = props.role === 'student'
  const isIncoming = props.direction === 'Incoming'

  if (isStudent) {
    if (isIncoming) return 'Вас пригласили на вакансию'
    if (!isIncoming) return 'Вы откликнулись на вакансию'
  }

  if (!isStudent) {
    if (isIncoming) return 'Новый отклик на вакансию'
    if (!isIncoming) return 'Вы отправили приглашение'
  }

  return 'Уведомление'
})

const avatarSrc = computed(() => {
  if (props.role === 'student') return props.job.avatarUrl
  return props.candidate.avatarUrl
})

const isCompanyAvatar = computed(() => props.role === 'student')
</script>

<template>
  <div class="party-info-container">
    <div class="avatar-wrapper">
      <img v-if="avatarSrc" :src="avatarSrc" alt="Avatar" class="avatar-img" />
      <div v-else class="avatar-placeholder">
        <component :is="isCompanyAvatar ? IconBuilding : IconUser" class="placeholder-icon" />
      </div>
    </div>

    <div class="content-wrapper">
      <div class="context-label">
        {{ contextText }}
      </div>

      <EmployerInfo
        v-if="role === 'student'"
        :employer-id="job.employerId"
        :company-name="job.companyName"
        :vacancy-id="job.vacancyId"
        :vacancy-title="job.vacancyTitle"
        :salary="job.salary"
      />

      <StudentInfo
        v-else
        :student-id="candidate.studentId"
        :full-name="candidate.fullName"
        :university="candidate.universityAbbreviation || ''"
        :specialty="candidate.specialtyName || ''"
        :course="candidate.courseNumber ?? null"
        :age="candidate.age ?? null"
        :resume-title="candidate.resumeTitle"
        :resume-id="candidate.resumeId"
        :skills="candidate.skills ?? []"
      />
    </div>
  </div>
</template>

<style scoped>
.party-info-container {
  display: flex;
  gap: 16px; /* Отступ между аватаром и текстом */
  align-items: flex-start;
  width: 100%;
}

.avatar-wrapper {
  flex-shrink: 0; /* Не сжимать аватар */
}

.avatar-img,
.avatar-placeholder {
  width: 56px;
  height: 56px;
  border-radius: 50%;
  object-fit: cover;
}

.avatar-placeholder {
  background-color: var(--gray-light, #f1f5f9);
  display: flex;
  align-items: center;
  justify-content: center;
  border: 1px solid var(--gray-border);
}

.placeholder-icon {
  width: 28px;
  height: 28px;
  color: var(--gray-text-focus);
}

.content-wrapper {
  display: flex;
  flex-direction: column;
  gap: 4px; /* Отступ между "Вы откликнулись" и Именем */
  flex-grow: 1;
  min-width: 0;
}

.context-label {
  font-size: 13px;
  color: var(--gray-text);
  margin-bottom: 2px;
}
</style>
