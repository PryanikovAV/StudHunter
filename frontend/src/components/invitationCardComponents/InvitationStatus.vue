<script setup lang="ts">
import { computed } from 'vue'
import IconHourGlass from '@/components/icons/IconHourGlass.vue'
import IconLike from '@/components/icons/IconLike.vue'
import IconDislike from '@/components/icons/IconDislike.vue'

type InvitationStatus = 'Sent' | 'Accepted' | 'Rejected' | 'Expired' | 'Cancelled'

const props = defineProps<{
  status: InvitationStatus
  date: string | Date
}>()

const statusConfig = computed(() => {
  switch (props.status) {
    case 'Accepted':
      return {
        text: 'Одобрено',
        class: 'status-accepted',
        icon: IconLike,
      }
    case 'Sent':
      return {
        text: 'Ожидает',
        class: 'status-pending',
        icon: IconHourGlass,
      }
    case 'Rejected':
    case 'Expired':
    case 'Cancelled':
    default:
      return {
        text: 'Отклонено',
        class: 'status-rejected',
        icon: IconDislike,
      }
  }
})

const formattedDate = computed(() => {
  const d = new Date(props.date)
  return d.toLocaleDateString('ru-RU', { day: 'numeric', month: 'short' })
})
</script>

<template>
  <div class="invitation-status-block">
    <div class="status-badge" :class="statusConfig.class">
      <component :is="statusConfig.icon" class="status-icon" />
      <span class="status-text">{{ statusConfig.text }}</span>
    </div>
    <span class="date-label">{{ formattedDate }}</span>
  </div>
</template>

<style scoped>
.invitation-status-block {
  display: flex;
  flex-direction: column;
  align-items: flex-end;
  gap: 4px;
}

.status-badge {
  display: inline-flex;
  align-items: center;
  gap: 6px;
  padding: 4px 10px;
  border-radius: 100px;
  font-size: 12px;
  font-weight: 600;
  white-space: nowrap;
}

.status-icon {
  width: 14px;
  height: 14px;
}

.status-pending {
  background-color: rgba(37, 99, 235, 0.1);
  color: #2563eb;
}

.status-accepted {
  background-color: rgba(22, 163, 74, 0.1);
  color: #16a34a;
}

.status-rejected {
  background-color: rgba(220, 38, 38, 0.1);
  color: #dc2626;
}

.date-label {
  font-size: 11px;
  color: var(--gray-text-focus);
  font-weight: 500;
}
</style>
