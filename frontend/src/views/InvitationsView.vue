<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import apiClient from '@/api'
import InvitationCard from '@/components/InvitationCard.vue'
import type { InvitationCardDto } from '@/types/invitation'

const router = useRouter()
const invitations = ref<InvitationCardDto[]>([])
const isLoading = ref(true)
const activeTab = ref<'incoming' | 'outgoing' | 'archive'>('incoming')

const fetchInvitations = async () => {
  isLoading.value = true
  try {
    const role = (localStorage.getItem('userRole') || 'student').toLowerCase()
    const endpoint = role === 'student' ? '/invitations/student' : '/invitations/employer'

    const [incomingRes, outgoingRes] = await Promise.all([
      apiClient.get(`${endpoint}?incoming=true`),
      apiClient.get(`${endpoint}?incoming=false`),
    ])

    const incomingData = incomingRes.data.items || incomingRes.data || []
    const outgoingData = outgoingRes.data.items || outgoingRes.data || []

    const combined = [...incomingData, ...outgoingData]
    invitations.value = combined.sort(
      (a, b) => new Date(b.sentAt).getTime() - new Date(a.sentAt).getTime(),
    )
  } catch (error) {
    console.error('Ошибка загрузки откликов:', error)
  } finally {
    isLoading.value = false
  }
}

const filteredInvitations = computed(() => {
  if (activeTab.value === 'incoming') {
    return invitations.value.filter((i) => i.direction === 'Incoming' && i.status === 'Sent')
  }
  if (activeTab.value === 'outgoing') {
    return invitations.value.filter((i) => i.direction === 'Outgoing' && i.status === 'Sent')
  }
  return invitations.value.filter((i) => i.status !== 'Sent')
})

const changeStatus = async (id: string, action: 'accept' | 'reject' | 'cancel') => {
  try {
    await apiClient.patch(`/invitations/${id}/${action}`)
    const item = invitations.value.find((i) => i.id === id)
    if (item) {
      if (action === 'accept') item.status = 'Accepted'
      if (action === 'reject') item.status = 'Rejected'
      if (action === 'cancel') item.status = 'Cancelled'
    }
  } catch (error) {
    console.error(`Ошибка при ${action}:`, error)
    window.alert('Не удалось изменить статус') // Добавили window.
  }
}

const handleChat = (payload: { invitationId: string; receiverId: string }) => {
  const role = (localStorage.getItem('userRole') || 'student').toLowerCase()

  router.push({
    name: `${role}-messages`,
    query: {
      receiverId: payload.receiverId,
      invitationId: payload.invitationId,
    },
  })
}

onMounted(fetchInvitations)
</script>

<template>
  <div class="page-narrow">
    <h1 class="page-title">Отклики и приглашения</h1>

    <div class="tabs">
      <button
        :class="['tab-btn', { active: activeTab === 'incoming' }]"
        @click="activeTab = 'incoming'"
      >
        Входящие
      </button>
      <button
        :class="['tab-btn', { active: activeTab === 'outgoing' }]"
        @click="activeTab = 'outgoing'"
      >
        Отправленные
      </button>
      <button
        :class="['tab-btn', { active: activeTab === 'archive' }]"
        @click="activeTab = 'archive'"
      >
        Архив
      </button>
    </div>

    <div v-if="isLoading" class="loading">Загрузка данных...</div>

    <div v-else-if="filteredInvitations.length === 0" class="empty-state">
      <p class="text-muted">В этой вкладке пока пусто.</p>
    </div>

    <div v-else class="invitations-list">
      <InvitationCard
        v-for="inv in filteredInvitations"
        :key="inv.id"
        :invitation="inv"
        @accept="changeStatus($event, 'accept')"
        @reject="changeStatus($event, 'reject')"
        @cancel="changeStatus($event, 'cancel')"
        @chat="handleChat($event)"
      />
    </div>
  </div>
</template>

<style scoped>
.tabs {
  display: flex;
  gap: 16px;
  margin-bottom: 24px;
  border-bottom: 1px solid var(--gray-border);
}
.tab-btn {
  background: none;
  border: none;
  font-size: 16px;
  font-weight: 500;
  color: var(--gray-text);
  padding: 8px 16px;
  cursor: pointer;
  border-bottom: 2px solid transparent;
  margin-bottom: -1px;
  transition: all 0.2s;
}
.tab-btn:hover {
  color: var(--dark-text);
}
.tab-btn.active {
  color: var(--susu-blue, #005aaa);
  border-bottom-color: var(--susu-blue, #005aaa);
}

.invitations-list {
  display: flex;
  flex-direction: column;
  gap: 16px;
}
.empty-state {
  text-align: center;
  padding: 40px 0;
}
</style>
