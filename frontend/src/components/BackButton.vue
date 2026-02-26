<script setup lang="ts">
import { useRouter } from 'vue-router'
import IconBackButton from '@/components/icons/IconBackButton.vue'

const props = withDefaults(
  defineProps<{
    text?: string
    to?: string | object
  }>(),
  {
    text: 'Назад',
    to: undefined,
  },
)

const router = useRouter()

const handleClick = () => {
  if (props.to) {
    router.push(props.to)
  } else {
    router.back()
  }
}
</script>

<template>
  <div class="back-navigation">
    <button class="btn-back" @click="handleClick">
      <IconBackButton class="back-icon" />
      <span>{{ text }}</span>
    </button>
  </div>
</template>

<style scoped>
.back-navigation {
  margin-bottom: 16px;
}
.btn-back {
  display: inline-flex;
  align-items: center;
  gap: 8px;
  background: none;
  border: none;
  color: var(--gray-text-focus);
  font-size: 15px;
  font-weight: 500;
  cursor: pointer;
  padding: 0;
  transition: color 0.2s;
}
.btn-back:hover {
  color: var(--dark-text);
}
.back-icon {
  width: 20px;
  height: 20px;
}
</style>
