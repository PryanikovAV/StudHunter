<script setup lang="ts">
import { computed } from 'vue'

interface Props {
  employerId: string
  companyName: string
  vacancyId?: string | null
  vacancyTitle?: string | null
  salary?: number | null
}

const props = defineProps<Props>()

const formattedSalary = computed(() => {
  if (!props.salary) return null

  return new Intl.NumberFormat('ru-RU', {
    style: 'currency',
    currency: 'RUB',
    maximumFractionDigits: 0,
  }).format(props.salary)
})
</script>

<template>
  <div class="employer-info">
    <router-link :to="`/employer/${employerId}`" class="company-name">
      {{ companyName }}
    </router-link>

    <div v-if="vacancyTitle" class="vacancy-block">
      <span class="label">Вакансия:</span>

      <router-link v-if="vacancyId" :to="`/vacancy/${vacancyId}`" class="vacancy-link">
        {{ vacancyTitle }}
      </router-link>
      <span v-else class="vacancy-text">
        {{ vacancyTitle }}
      </span>
    </div>

    <div v-if="formattedSalary" class="salary-block">
      {{ formattedSalary }}
    </div>
  </div>
</template>

<style scoped>
.employer-info {
  display: flex;
  flex-direction: column;
  gap: 6px;
}

.company-name {
  font-size: 16px;
  font-weight: 700;
  color: var(--dark-text);
  text-decoration: none;
}

.company-name:hover {
  text-decoration: underline;
  color: var(--susu-blue);
}

.vacancy-block {
  display: flex;
  align-items: center;
  flex-wrap: wrap;
  gap: 6px;
  font-size: 14px;
}

.label {
  color: var(--gray-text);
  font-size: 13px;
}

.vacancy-link {
  font-weight: 600;
  color: var(--dark-text);
  text-decoration: none;
  transition: color 0.2s;
}

.vacancy-link:hover {
  color: var(--susu-blue);
}

.vacancy-text {
  font-weight: 600;
  color: var(--dark-text);
}

.salary-block {
  font-size: 13px;
  font-weight: 600;
  color: var(--green-text, #16a34a);
  background-color: rgba(22, 163, 74, 0.05);
  align-self: flex-start;
  padding: 2px 8px;
  border-radius: 6px;
}
</style>
