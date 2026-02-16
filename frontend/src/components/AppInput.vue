<script setup lang="ts">
defineProps<{
  modelValue: string | number | null
  type?: string
  placeholder?: string
}>()

defineEmits<{
  (e: 'update:modelValue', value: string): void
  (e: 'focus', event: FocusEvent): void
  (e: 'blur', event: FocusEvent): void
}>()
</script>

<template>
  <input
    :type="type || 'text'"
    :value="modelValue"
    :placeholder="placeholder"
    @input="$emit('update:modelValue', ($event.target as HTMLInputElement).value)"
    @focus="$emit('focus', $event)"
    @blur="$emit('blur', $event)"
    class="input-main"
  />
</template>

<style scoped></style>
