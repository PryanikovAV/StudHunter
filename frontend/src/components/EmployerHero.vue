<script setup lang="ts">
import { ref, onMounted, computed } from 'vue'
import apiClient from '@/api'
import AppCard from '@/components/AppCard.vue'
import IconBuilding from '@/components/icons/IconBuilding.vue'

interface HeroViewData {
  name: string
  avatarUrl: string | null
  cityName: string | null
  specializationName: string | null
  website: string | null
  vacanciesCount: number
}

const heroData = ref<HeroViewData | null>(null)
const isLoading = ref(true)

const loadHeroData = async () => {
  try {
    const response = await apiClient.get('/employers/me/profile')

    heroData.value = {
      name: response.data.name,
      avatarUrl: response.data.avatarUrl,
      cityName: response.data.cityName,
      specializationName: response.data.specializationName,
      website: response.data.website,
      vacanciesCount: response.data.vacanciesCount,
    }
  } catch (err) {
    console.error('Employer Hero load error:', err)
  } finally {
    isLoading.value = false
  }
}

onMounted(loadHeroData)

defineExpose({ loadHeroData })

const vacanciesText = computed(() => {
  const count = heroData.value?.vacanciesCount || 0
  if (count === 0) return 'Нет активных вакансий'

  const lastDigit = count % 10
  const lastTwoDigits = count % 100

  if (lastTwoDigits >= 11 && lastTwoDigits <= 14) return `${count} вакансий`
  if (lastDigit === 1) return `${count} вакансия`
  if (lastDigit >= 2 && lastDigit <= 4) return `${count} вакансии`
  return `${count} вакансий`
})
</script>

<template>
  <div class="container">
    <AppCard class="hero-compact-card">
      <div class="hero-grid">
        <div v-if="isLoading" class="hero-message">Загрузка профиля компании...</div>
        <div v-else-if="!heroData" class="hero-message error">Данные не загружены</div>

        <template v-else>
          <div class="col-personal">
            <h1 class="page-title">{{ heroData.name }}</h1>
            <div class="text-muted" v-if="heroData.cityName">{{ heroData.cityName }}</div>
          </div>

          <div class="col-study">
            <h2 class="uni-name" v-if="heroData.specializationName">
              {{ heroData.specializationName }}
            </h2>
            <div class="study-info" v-if="heroData.website">
              <a :href="heroData.website" target="_blank" class="website-link">
                {{ heroData.website.replace(/^https?:\/\//, '') }}
              </a>
            </div>
          </div>

          <div class="col-visual">
            <div class="avatar">
              <img v-if="heroData.avatarUrl" :src="heroData.avatarUrl" alt="Company Logo" />
              <div v-else class="avatar-placeholder">
                <IconBuilding class="icon-large" />
              </div>
            </div>
            <p class="status-text">{{ vacanciesText }}</p>
          </div>
        </template>
      </div>
    </AppCard>
  </div>
</template>

<style scoped>
.hero-compact-card {
  padding: 16px 24px;
  margin-top: 16px;
}
.hero-grid {
  display: grid;
  grid-template-columns: 1fr 1.5fr 100px;
  gap: 16px;
  align-items: center;
}
.hero-message {
  grid-column: 1 / -1;
  text-align: center;
  color: var(--gray-text);
  padding: 10px;
}
.hero-message.error {
  color: var(--red-text-error);
}
.col-personal {
  display: flex;
  flex-direction: column;
  align-items: flex-start;
  gap: 2px;
}
.page-title {
  margin: 0;
  font-size: 24px;
  line-height: 1.2;
  color: var(--dark-text);
}
.text-muted {
  font-size: 14px;
  color: var(--gray-text);
}
.col-study {
  display: flex;
  flex-direction: column;
}
.uni-name {
  font-size: 18px;
  font-weight: 600;
  color: var(--dark-text);
  margin: 0 0 4px 0;
  line-height: 1.3;
}
.study-info {
  margin: 0;
}
.website-link {
  font-size: 16px;
  font-weight: 500;
  color: var(--susu-blue);
  text-decoration: none;
  transition: opacity 0.2s;
}
.website-link:hover {
  text-decoration: underline;
  opacity: 0.8;
}
.col-visual {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 6px;
}
.avatar {
  width: 72px;
  height: 72px;
  border-radius: 12px;
  overflow: hidden;
  background-color: var(--background-page);
  border: 1px solid var(--gray-border);
}
.avatar img {
  width: 100%;
  height: 100%;
  object-fit: cover;
}
.avatar-placeholder {
  width: 100%;
  height: 100%;
  display: flex;
  align-items: center;
  justify-content: center;
  background-color: #f1f5f9;
  color: var(--gray-text-focus);
}
.icon-large {
  width: 32px;
  height: 32px;
  opacity: 0.7;
}
.status-text {
  font-size: 13px;
  color: var(--susu-blue);
  font-weight: 600;
  margin: 0;
  text-align: center;
}
@media (max-width: 768px) {
  .hero-grid {
    grid-template-columns: 1fr;
    gap: 16px;
    text-align: center;
  }
  .col-personal {
    align-items: center;
  }
  .col-study {
    padding-top: 12px;
    border-top: 1px solid var(--gray-border);
  }
}
</style>
