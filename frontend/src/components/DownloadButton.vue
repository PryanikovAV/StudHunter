<script setup lang="ts">
import IconDownload from '@/components/icons/IconDownload.vue'

withDefaults(
  defineProps<{
    isLoading?: boolean
    title?: string
  }>(),
  {
    isLoading: false,
    title: 'Скачать файл',
  },
)

const emit = defineEmits<{
  (e: 'click'): void
}>()

const handleClick = () => {
  emit('click')
}
</script>

<template>
  <button
    class="btn-circle btn-download"
    :class="{ 'is-loading': isLoading }"
    :title="isLoading ? 'Загрузка...' : title"
    :disabled="isLoading"
    @click.prevent="handleClick"
  >
    <IconDownload class="icon-inner" :class="{ 'pulse-animation': isLoading }" />
  </button>
</template>

<style scoped>
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
  transition: opacity 0.2s;
}
.btn-download {
  background-color: transparent;
  border: 1px solid var(--dark-text, #111827);
  color: var(--dark-text, #111827);
}
.btn-download:hover:not(:disabled) {
  background-color: #f3f4f6;
  transform: scale(1.05);
}
.btn-download:disabled,
.btn-download.is-loading {
  opacity: 0.5;
  cursor: not-allowed;
  border-color: var(--gray-border, #d1d5db);
  color: var(--gray-text-focus, #6b7280);
}

.pulse-animation {
  animation: pulse 1.5s infinite;
}

@keyframes pulse {
  0% {
    opacity: 1;
  }
  50% {
    opacity: 0.4;
  }
  100% {
    opacity: 1;
  }
}
</style>
