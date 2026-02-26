<script setup lang="ts">
import { computed } from 'vue'
import AppCard from '@/components/AppCard.vue'

interface InvitationCardDto {
  id: string
  status: string
  type: string
  direction: string
  sentAt: string
  message: string | null
  candidate: {
    studentId: string
    fullName: string
    courseNumber: number | null
    universityAbbreviation: string | null
    skills: string[]
    resumeId: string | null
  }
  job: {
    employerId: string
    companyName: string
    vacancyTitle: string | null
    salary: number | null
    vacancyId: string | null
  }
}

const props = defineProps<{
  invitation: InvitationCardDto
}>()

defineEmits(['accept', 'reject', 'cancel', 'chat'])

const currentUserRole = computed(() => (localStorage.getItem('userRole') || '').toLowerCase())
const isStudent = computed(() => currentUserRole.value === 'student')
const isEmployer = computed(() => currentUserRole.value === 'employer')

const formattedDate = computed(() => {
  return new Date(props.invitation.sentAt).toLocaleDateString('ru-RU', {
    day: 'numeric',
    month: 'long',
    year: 'numeric',
  })
})

const statusMap: Record<string, { text: string; class: string }> = {
  Sent: { text: 'Ожидает ответа', class: 'badge-sent' },
  Accepted: { text: 'Принято', class: 'badge-accepted' },
  Rejected: { text: 'Отклонено', class: 'badge-rejected' },
  Expired: { text: 'Просрочено', class: 'badge-expired' },
  Cancelled: { text: 'Отозвано', class: 'badge-expired' },
}

const statusDisplay = computed(
  () =>
    statusMap[props.invitation.status] || { text: props.invitation.status, class: 'badge-sent' },
)

const titleText = computed(() => {
  if (props.invitation.type === 'Response') return 'Отклик на вакансию'
  if (props.invitation.type === 'Offer') return 'Приглашение от компании'
  return 'Приглашение'
})
</script>

<template>
  <AppCard class="invitation-card">
    <div class="card-header">
      <div class="header-left">
        <span class="type-title">{{ titleText }}</span>
        <span class="date-text">{{ formattedDate }}</span>
      </div>
      <span :class="['status-badge', statusDisplay.class]">{{ statusDisplay.text }}</span>
    </div>

    <div class="card-body">
      <div class="info-block">
        <div class="info-label">Компания и вакансия</div>

        <router-link
          v-if="isStudent"
          :to="`/employers/${invitation.job.employerId}`"
          class="info-main hover-link"
        >
          {{ invitation.job.companyName }}
        </router-link>
        <div v-else class="info-main">
          {{ invitation.job.companyName }}
        </div>

        <template v-if="invitation.job.vacancyId">
          <router-link
            v-if="isStudent"
            :to="`/vacancies/${invitation.job.vacancyId}`"
            class="info-sub hover-link"
          >
            {{ invitation.job.vacancyTitle }}
          </router-link>
          <div v-else class="info-sub">
            {{ invitation.job.vacancyTitle }}
          </div>
        </template>
        <div v-else class="info-sub">
          {{ invitation.job.vacancyTitle || 'Вакансия удалена' }}
        </div>

        <div class="info-sub" v-if="invitation.job.salary">
          {{ invitation.job.salary.toLocaleString('ru-RU') }} ₽
        </div>
      </div>

      <div class="info-block">
        <div class="info-label">Кандидат</div>

        <router-link
          v-if="isEmployer"
          :to="`/students/${invitation.candidate.studentId}`"
          class="info-main hover-link"
        >
          {{ invitation.candidate.fullName }}
        </router-link>
        <div v-else class="info-main">
          {{ invitation.candidate.fullName }}
        </div>

        <router-link
          v-if="isEmployer && invitation.candidate.resumeId"
          :to="`/students/${invitation.candidate.studentId}`"
          class="info-sub hover-link"
        >
          Открыть профиль
        </router-link>

        <div class="info-sub" v-if="invitation.candidate.universityAbbreviation">
          {{ invitation.candidate.universityAbbreviation }}
          <span v-if="invitation.candidate.courseNumber"
            >({{ invitation.candidate.courseNumber }} курс)</span
          >
        </div>

        <div class="skills-list" v-if="invitation.candidate.skills.length">
          <span
            v-for="skill in invitation.candidate.skills.slice(0, 3)"
            :key="skill"
            class="skill-tag"
          >
            {{ skill }}
          </span>
          <span v-if="invitation.candidate.skills.length > 3" class="skill-tag">...</span>
        </div>
      </div>
    </div>

    <div v-if="invitation.message" class="message-block">
      {{ invitation.message }}
    </div>

    <div class="card-actions">
      <template v-if="invitation.direction === 'Incoming' && invitation.status === 'Sent'">
        <button class="btn-main btn-dark" @click="$emit('accept', invitation.id)">Принять</button>
        <button class="btn-main btn-outline" @click="$emit('reject', invitation.id)">
          Отклонить
        </button>
      </template>

      <template v-if="invitation.direction === 'Outgoing' && invitation.status === 'Sent'">
        <button class="btn-main btn-outline" @click="$emit('cancel', invitation.id)">
          Отозвать отклик
        </button>
      </template>

      <template v-if="invitation.status === 'Accepted'">
        <button class="btn-main btn-outline" @click="$emit('chat', invitation.id)">
          Написать в чат
        </button>
      </template>
    </div>
  </AppCard>
</template>

<style scoped>
.invitation-card {
  padding: 20px;
  display: flex;
  flex-direction: column;
  gap: 16px;
}

.card-header {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  border-bottom: 1px solid var(--gray-border);
  padding-bottom: 12px;
}
.header-left {
  display: flex;
  flex-direction: column;
  gap: 4px;
}
.type-title {
  font-weight: 600;
  color: var(--dark-text);
  font-size: 16px;
}
.date-text {
  font-size: 13px;
  color: var(--gray-text);
}

.status-badge {
  padding: 4px 10px;
  border-radius: 6px;
  font-size: 12px;
  font-weight: 500;
}
.badge-sent {
  background: #f1f5f9;
  color: #475569;
}
.badge-accepted {
  background: #dcfce7;
  color: #166534;
}
.badge-rejected {
  background: #fee2e2;
  color: #991b1b;
}
.badge-expired {
  background: #f3f4f6;
  color: #6b7280;
}

.card-body {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 16px;
}
.info-block {
  display: flex;
  flex-direction: column;
  gap: 4px;
}
.info-label {
  font-size: 12px;
  color: var(--gray-text);
  text-transform: uppercase;
  letter-spacing: 0.5px;
  margin-bottom: 4px;
}
.info-main {
  font-weight: 600;
  color: var(--dark-text);
  font-size: 15px;
}
.info-sub {
  font-size: 14px;
  color: var(--gray-text-focus);
}
.skills-list {
  display: flex;
  flex-wrap: wrap;
  gap: 6px;
  margin-top: 6px;
}
.skill-tag {
  background: var(--background-page);
  border: 1px solid var(--gray-border);
  padding: 2px 8px;
  border-radius: 4px;
  font-size: 12px;
  color: var(--gray-text-focus);
}

.message-block {
  background: #f8fafc;
  padding: 12px;
  border-radius: 8px;
  font-size: 14px;
  color: var(--dark-text);
  font-style: italic;
  border-left: 3px solid var(--gray-border);
}

.card-actions {
  display: flex;
  gap: 12px;
  margin-top: 8px;
}

.hover-link {
  text-decoration: none;
  color: inherit;
  transition: color 0.2s ease;
  display: inline-block;
}
.hover-link:hover {
  color: var(--susu-blue);
  text-decoration: underline;
}

@media (max-width: 640px) {
  .card-body {
    grid-template-columns: 1fr;
    gap: 24px;
  }
  .card-actions {
    flex-direction: column;
  }
  .card-actions button {
    width: 100%;
  }
}
</style>
