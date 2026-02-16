<script setup lang="ts">
import InvitationPartyInfo from '@/components/invitationCardComponents/InvitationPartyInfo.vue'
import InvitationStatus from '@/components/invitationCardComponents/InvitationStatus.vue'
import InvitationActions from '@/components/invitationCardComponents/InvitationActions.vue'

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

defineProps<{
  invitation: InvitationCardDto
  role: 'student' | 'employer'
}>()

const emit = defineEmits<{
  (e: 'accept'): void
  (e: 'reject'): void
  (e: 'cancel'): void
  (e: 'chat'): void
}>()
</script>

<template>
  <div class="invitation-card card">
    <div class="card-header-row">
      <InvitationPartyInfo
        :type="invitation.type"
        :direction="invitation.direction"
        :role="role"
        :candidate="invitation.candidate"
        :job="invitation.job"
      />

      <InvitationStatus :status="invitation.status" :date="invitation.sentAt" />
    </div>

    <div v-if="invitation.message" class="card-message">
      <p>«{{ invitation.message }}»</p>
    </div>

    <InvitationActions
      :direction="invitation.direction"
      :status="invitation.status"
      @accept="emit('accept')"
      @reject="emit('reject')"
      @cancel="emit('cancel')"
      @chat="emit('chat')"
    />
  </div>
</template>

<style scoped>
.invitation-card {
  display: flex;
  flex-direction: column;
  padding: 24px;
  gap: 16px;
  width: 100%;
  background-color: var(--background-field);
  border: 1px solid var(--gray-border);
  border-radius: 16px;
}

.card-header-row {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  gap: 20px;
}

.card-message {
  padding: 12px 16px;
  background-color: var(--background-page);
  border-left: 4px solid var(--susu-blue);
  border-radius: 8px;
}

.card-message p {
  margin: 0;
  font-size: 14px;
  font-style: italic;
  color: var(--gray-text-focus);
  line-height: 1.5;
}

@media (max-width: 640px) {
  .card-header-row {
    flex-direction: column-reverse;
    gap: 16px;
  }
}
</style>
