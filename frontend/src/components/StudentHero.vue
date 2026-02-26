<script setup lang="ts">
import { ref, onMounted, computed } from 'vue'
import { calculateAge } from '@/utils/dateUtils'
import apiClient from '@/api'
import AppCard from '@/components/AppCard.vue'

interface StudentHeroDto {
  fullName: string
  birthDate: string | null
  avatarUrl: string | null
  status: string
  universityName: string | null
  facultyName: string | null
  departmentName: string | null
  studyDirectionName: string | null
  courseNumber: number | null
}

const heroData = ref<StudentHeroDto | null>(null)
const isLoading = ref(true)

onMounted(async () => {
  try {
    const response = await apiClient.get<StudentHeroDto>('/students/me/hero')
    heroData.value = response.data
  } catch (err) {
    console.error('Hero load error:', err)
  } finally {
    isLoading.value = false
  }
})

const ageDisplay = computed(() => calculateAge(heroData.value?.birthDate))

const initials = computed(() => {
  if (!heroData.value?.fullName) return 'ST'
  return heroData.value.fullName
    .split(' ')
    .slice(0, 2)
    .map((word) => word[0])
    .join('')
    .toUpperCase()
})

const statusMap: Record<string, string> = {
  Studying: 'Учусь',
  SeekingInternship: 'Ищу стажировку',
  SeekingJob: 'Ищу работу',
  Interning: 'На стажировке',
  Working: 'Работаю',
}

const statusText = computed(() => {
  if (!heroData.value?.status) return 'Статус не указан'
  return statusMap[heroData.value.status] || heroData.value.status
})

const formattedName = computed(() => {
  if (!heroData.value?.fullName) return ''
  const parts = heroData.value.fullName.split(' ')
  if (parts.length === 3) {
    return `${parts[0]} ${parts[1]} <br> ${parts[2]}`
  }
  return heroData.value.fullName
})
</script>

<template>
  <div class="container" style="margin-top: 24px">
    <AppCard>
      <div class="hero-grid">
        <div v-if="isLoading" class="hero-message">Загрузка профиля...</div>
        <div v-else-if="!heroData" class="hero-message error">Данные не загружены</div>

        <template v-else>
          <div class="col-personal">
            <h1 class="page-title" v-html="formattedName"></h1>
            <div class="text-muted" v-if="ageDisplay">
              {{ ageDisplay }}
            </div>
          </div>

          <div class="col-study">
            <h2 class="uni-name">{{ heroData.universityName }}</h2>
            <div class="study-info">
              <p v-if="heroData.facultyName">{{ heroData.facultyName }}</p>
              <p v-if="heroData.departmentName">{{ heroData.departmentName }}</p>
              <p v-if="heroData.studyDirectionName">
                {{ heroData.studyDirectionName }}
                <span v-if="heroData.courseNumber" class="course-num">
                  • {{ heroData.courseNumber }} курс
                </span>
              </p>
            </div>
          </div>

          <div class="col-visual">
            <div class="avatar">
              <img v-if="heroData.avatarUrl" :src="heroData.avatarUrl" alt="Avatar" />
              <div v-else class="avatar-placeholder">{{ initials }}</div>
            </div>
            <p class="status-text">{{ statusText }}</p>
          </div>
        </template>
      </div>
    </AppCard>
  </div>
</template>

<style scoped>
.hero-grid {
  display: grid;
  grid-template-columns: 1fr 1.5fr 100px;
  gap: 24px;
  align-items: center;
  width: 100%;
}

.hero-message {
  grid-column: 1 / -1;
  text-align: center;
  color: var(--gray-text);
  padding: 20px;
}
.hero-message.error {
  color: var(--red-text-error);
}

.col-personal {
  display: flex;
  flex-direction: column;
  align-items: flex-start;
  gap: 8px;
}

.page-title {
  margin: 0;
  font-size: 24px;
  line-height: 1.3;
  color: var(--dark-text);
}

.text-muted {
  font-size: 15px;
  color: var(--gray-text);
  margin-top: auto;
}

.col-study {
  display: flex;
  flex-direction: column;
  align-items: center;
  text-align: center;
}

.uni-name {
  font-size: 18px;
  font-weight: 700;
  color: var(--dark-text);
  margin: 0 0 8px 0;
  line-height: 1.3;
}

.study-info p {
  margin: 0 0 4px 0;
  font-size: 15px;
  color: var(--gray-text-focus);
  line-height: 1.4;
}

.course-num {
  color: var(--dark-text);
  font-weight: 500;
}

.col-visual {
  display: flex;
  flex-direction: column;
  align-items: flex-end;
  gap: 8px;
}

.avatar {
  width: 80px;
  height: 80px;
  border-radius: 50%;
  overflow: hidden;
  background-color: var(--background-page);
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
  background-color: var(--gray-border);
  color: var(--gray-text-focus);
  font-size: 24px;
  font-weight: 600;
}

.status-text {
  font-size: 13px;
  color: var(--gray-text);
  margin: 0;
  text-align: center;
  width: 80px;
}

@media (max-width: 768px) {
  .hero-grid {
    grid-template-columns: 1fr;
    gap: 20px;
    text-align: center;
  }

  .col-personal {
    align-items: center;
  }

  .col-visual {
    align-items: center;
  }

  .col-study {
    padding-top: 16px;
    border-top: 1px solid var(--gray-border);
  }
}
</style>
