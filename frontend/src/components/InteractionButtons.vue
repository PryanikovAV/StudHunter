<script setup lang="ts">
import { ref, watch } from 'vue'
import apiClient from '@/api'
import IconFavorites from '@/components/icons/IconFavorites.vue'
import IconBlockCommunication from '@/components/icons/IconBlockCommunication.vue'

const props = withDefaults(
  defineProps<{
    targetId: string
    favoriteType: 'Vacancy' | 'Student' | 'Employer'
    blockUserId?: string | null
    initialIsFavorite?: boolean
    initialIsBlocked?: boolean
  }>(),
  {
    initialIsFavorite: false,
    initialIsBlocked: false,
    blockUserId: null,
  },
)

const isFavorite = ref(props.initialIsFavorite)
const isBlocked = ref(props.initialIsBlocked)

watch(
  () => props.initialIsFavorite,
  (newVal) => (isFavorite.value = newVal),
)
watch(
  () => props.initialIsBlocked,
  (newVal) => (isBlocked.value = newVal),
)

const toggleFavorite = async () => {
  const previousState = isFavorite.value
  isFavorite.value = !previousState

  const typeMap = {
    Vacancy: 0,
    Student: 1,
    Employer: 2,
  }

  try {
    await apiClient.post('/favorites/toggle', {
      targetId: props.targetId,
      type: typeMap[props.favoriteType],
      isFavorite: isFavorite.value,
    })
  } catch (error) {
    console.error('Ошибка при изменении избранного:', error)
    isFavorite.value = previousState
    alert('Не удалось обновить избранное.')
  }
}

const toggleBlock = async () => {
  const idToBlock = props.blockUserId || props.targetId
  if (!idToBlock) return

  const previousState = isBlocked.value
  isBlocked.value = !previousState

  try {
    await apiClient.post(`/blacklist/toggle/${idToBlock}?shouldBlock=${isBlocked.value}`)
  } catch (error) {
    console.error('Ошибка при изменении черного списка:', error)
    isBlocked.value = previousState
    alert('Не удалось обновить черный список.')
  }
}
</script>

<template>
  <div class="interaction-buttons">
    <button
      v-if="blockUserId || favoriteType !== 'Vacancy'"
      class="btn-circle btn-block"
      :class="{ active: isBlocked }"
      :title="isBlocked ? 'Разблокировать пользователя' : 'Заблокировать пользователя'"
      @click.prevent="toggleBlock"
    >
      <IconBlockCommunication class="icon-inner" />
    </button>

    <button
      class="btn-circle btn-fav"
      :class="{ active: isFavorite }"
      :title="isFavorite ? 'Убрать из избранного' : 'Добавить в избранное'"
      @click.prevent="toggleFavorite"
    >
      <IconFavorites class="icon-inner" />
    </button>
  </div>
</template>

<style scoped>
.interaction-buttons {
  display: flex;
  align-items: center;
  gap: 12px;
}

.btn-circle {
  width: 40px;
  height: 40px;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  cursor: pointer;
  padding: 0;
  transition: all 0.2s ease;
  flex-shrink: 0;
}

.icon-inner {
  width: 20px;
  height: 20px;
}

.btn-fav {
  background-color: var(--dark-text, #111827);
  border: 1px solid var(--dark-text, #111827);
  color: #ffffff;
}
.btn-fav:hover {
  opacity: 0.8;
  transform: scale(1.05);
}
.btn-fav.active {
  background-color: var(--susu-blue, #005aaa);
  border-color: var(--susu-blue, #005aaa);
}

.btn-block {
  background-color: transparent;
  border: 1px solid var(--dark-text, #111827);
  color: var(--dark-text, #111827);
}
.btn-block:hover {
  background-color: #f3f4f6;
  transform: scale(1.05);
}
.btn-block.active {
  border-color: var(--red-text-error, #dc2626);
  color: var(--red-text-error, #dc2626);
  background-color: #fef2f2;
}
</style>
