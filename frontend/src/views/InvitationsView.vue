<script setup lang="ts">
import { ref, onMounted } from 'vue'
import axios from 'axios'
import { useRouter } from 'vue-router'
import InvitationCard from '@/components/invitation/InvitationCard.vue'

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

interface InvitationCardDto {
  id: string
  status: 'Sent' | 'Accepted' | 'Rejected' | 'Expired' | 'Cancelled'
  type: string
  direction: 'Incoming' | 'Outgoing'
  sentAt: string | Date
  message?: string | null
  candidate: InvitationCandidateDto
  job: InvitationJobDto
}

const router = useRouter()
const invitations = ref<InvitationCardDto[]>([])
const isLoading = ref(true)
const error = ref(false)
const userRole = ref<'student' | 'employer'>('student')

const getApiEndpoint = () => {
  const storedRole = localStorage.getItem('role')?.toLowerCase()

  if (storedRole === 'employer') {
    userRole.value = 'employer'
    return 'http://localhost:5010/api/v1/employer/invitations'
  } else {
    userRole.value = 'student'
    return 'http://localhost:5010/api/v1/student/invitations'
  }
}

const fetchInvitations = async () => {
  isLoading.value = true
  error.value = false

  try {
    const url = getApiEndpoint()
    const response = await axios.get<InvitationCardDto[]>(url, {
      headers: { Authorization: `Bearer ${localStorage.getItem('token')}` },
    })
    invitations.value = response.data
  } catch {
    error.value = true
  } finally {
    isLoading.value = false
  }
}

const handleStatusChange = async (id: string, action: 'accept' | 'reject' | 'cancel') => {
  try {
    await axios.post(
      `http://localhost:5010/api/v1/invitations/${id}/${action}`,
      {},
      { headers: { Authorization: `Bearer ${localStorage.getItem('token')}` } },
    )
    await fetchInvitations()
  } catch {
    alert('Ошибка при выполнении действия')
  }
}

const handleChat = (id: string) => {
  router.push(`/messages/${id}`)
}

onMounted(fetchInvitations)
</script>

<template>
  <div class="invitations-page">
    <div class="container">
      <h1 class="page-title">
        {{ userRole === 'student' ? 'Мои отклики и приглашения' : 'Отклики на вакансии' }}
      </h1>

      <div v-if="isLoading" class="state-block">Загрузка данных...</div>

      <div v-else-if="error" class="state-block error">
        Не удалось загрузить список. Попробуйте обновить страницу.
      </div>

      <div v-else-if="invitations.length === 0" class="state-block">
        Список пуст. Активных откликов нет.
      </div>

      <div v-else class="invitations-list">
        <InvitationCard
          v-for="item in invitations"
          :key="item.id"
          :invitation="item"
          :role="userRole"
          @accept="handleStatusChange(item.id, 'accept')"
          @reject="handleStatusChange(item.id, 'reject')"
          @cancel="handleStatusChange(item.id, 'cancel')"
          @chat="handleChat(item.id)"
        />
      </div>
    </div>
  </div>
</template>

<style scoped>
.invitations-page {
  padding-top: 24px;
  padding-bottom: 64px;
  min-height: 80vh;
}

.invitations-list {
  display: flex;
  flex-direction: column;
  gap: 16px;
}

.state-block {
  padding: 40px;
  text-align: center;
  background-color: var(--background-field);
  border: 1px solid var(--gray-border);
  border-radius: 16px;
  color: var(--gray-text);
  font-size: 14px;
}

.state-block.error {
  color: var(--red-text-error);
  border-color: var(--red-text-error);
}
</style>
