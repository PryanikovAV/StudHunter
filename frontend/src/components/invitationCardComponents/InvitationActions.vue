<script setup lang="ts">
type InvitationStatus = 'Sent' | 'Accepted' | 'Rejected' | 'Expired' | 'Cancelled'

defineProps<{
  direction: string
  status: InvitationStatus
}>()

const emit = defineEmits<{
  (e: 'accept'): void
  (e: 'reject'): void
  (e: 'cancel'): void
  (e: 'chat'): void
}>()
</script>

<template>
  <div class="actions-container">
    <template v-if="direction === 'Incoming'">
      <template v-if="status === 'Sent'">
        <button class="btn-main btn-dark btn-sm" @click="emit('accept')">Принять</button>
        <button class="btn-main btn-outline btn-sm" @click="emit('reject')">Отклонить</button>
        <button class="btn-main btn-outline btn-sm" @click="emit('chat')">Написать</button>
      </template>

      <template v-else>
        <button class="btn-main btn-outline btn-sm" @click="emit('chat')">Написать</button>
      </template>
    </template>

    <template v-else>
      <template v-if="status === 'Sent'">
        <button class="btn-main btn-dark btn-sm" @click="emit('chat')">Написать</button>
        <button class="btn-main btn-outline btn-sm" @click="emit('cancel')">Отменить</button>
      </template>

      <template v-else>
        <button class="btn-main btn-outline btn-sm" @click="emit('chat')">Написать</button>
      </template>
    </template>
  </div>
</template>

<style scoped>
.actions-container {
  display: flex;
  flex-wrap: wrap;
  gap: 12px;
  margin-top: 16px;
}

.btn-sm {
  height: 38px;
  padding: 0 20px;
  font-size: 13px;
  width: auto;
}

@media (max-width: 480px) {
  .actions-container {
    flex-direction: column;
    gap: 8px;
  }

  .btn-sm {
    width: 100%;
  }
}
</style>
