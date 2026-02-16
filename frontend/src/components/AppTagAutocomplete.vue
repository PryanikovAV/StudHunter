<script setup lang="ts">
import { ref, watch } from 'vue'
import AppInput from '@/components/AppInput.vue'

export interface TagItem {
  id: string
  name: string
}

const props = defineProps<{
  modelValue: TagItem[]
  placeholder?: string
  fetchFn: (query: string) => Promise<TagItem[]>
  inputClass?: string
}>()

const emit = defineEmits<{
  (e: 'update:modelValue', value: TagItem[]): void
}>()

const searchQuery = ref('')
const suggestions = ref<TagItem[]>([])
const isLoading = ref(false)
const showDropdown = ref(false)
let debounceTimeout: ReturnType<typeof setTimeout> | null = null

watch(searchQuery, (newQuery) => {
  if (newQuery.length < 2) {
    suggestions.value = []
    showDropdown.value = false
    return
  }

  if (debounceTimeout) clearTimeout(debounceTimeout)

  debounceTimeout = setTimeout(async () => {
    isLoading.value = true
    try {
      const results = await props.fetchFn(newQuery)
      suggestions.value = results.filter(
        (res) => !props.modelValue.some((selected) => selected.id === res.id),
      )
      showDropdown.value = suggestions.value.length > 0
    } catch (error) {
      console.error('Ошибка поиска', error)
    } finally {
      isLoading.value = false
    }
  }, 300)
})

const selectItem = (item: TagItem) => {
  emit('update:modelValue', [...props.modelValue, item])
  searchQuery.value = ''
  showDropdown.value = false
}

const removeItem = (idToRemove: string) => {
  emit(
    'update:modelValue',
    props.modelValue.filter((item) => item.id !== idToRemove),
  )
}
</script>

<template>
  <div class="tag-autocomplete">
    <div class="tags-container" v-if="modelValue.length > 0">
      <span v-for="tag in modelValue" :key="tag.id" class="tag">
        {{ tag.name }}
        <button type="button" class="remove-btn" @click.prevent="removeItem(tag.id)">
          &times;
        </button>
      </span>
    </div>

    <div class="input-wrapper">
      <AppInput
        v-model="searchQuery"
        :placeholder="placeholder || 'Начните вводить...'"
        :class="inputClass"
        @focus="searchQuery.length >= 2 && suggestions.length > 0 ? (showDropdown = true) : null"
      />

      <span v-if="isLoading" class="loader">...</span>

      <ul v-if="showDropdown" class="dropdown">
        <li
          v-for="item in suggestions"
          :key="item.id"
          @click="selectItem(item)"
          class="dropdown-item"
        >
          {{ item.name }}
        </li>
      </ul>
    </div>
  </div>
</template>

<style scoped>
.tag-autocomplete {
  position: relative;
  display: flex;
  flex-direction: column;
  gap: 8px;
  width: 100%;
  box-sizing: border-box;
}

.tags-container {
  display: flex;
  flex-wrap: wrap;
  gap: 8px;
}

.tag {
  display: inline-flex;
  align-items: center;
  gap: 6px;
  background-color: var(--susu-blue, #005aaa);
  color: var(--background-field);
  padding: 4px 10px;
  border-radius: 16px;
  font-size: 13px;
}

.remove-btn {
  background: none;
  border: none;
  color: var(--background-field);
  font-size: 16px;
  cursor: pointer;
  padding: 0;
  line-height: 1;
}

.input-wrapper {
  position: relative;
  width: 100%;
  box-sizing: border-box;
}

.dropdown {
  position: absolute;
  top: 100%;
  left: 0;
  right: 0;
  background: var(--background-field);
  border: 1px solid var(--gray-border, #ccc);
  border-radius: 8px;
  margin-top: 4px;
  max-height: 200px;
  overflow-y: auto;
  z-index: 10;
  list-style: none;
  padding: 0;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);
}

.dropdown-item {
  padding: 10px 12px;
  cursor: pointer;
  font-size: 14px;
}

.dropdown-item:hover {
  background-color: var(--gray-border, #f0f0f0);
}

.loader {
  position: absolute;
  right: 12px;
  top: 50%;
  transform: translateY(-50%);
  color: #888;
}
</style>
