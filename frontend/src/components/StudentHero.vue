<script setup lang="ts">
import { ref, onMounted, computed } from 'vue'
import { useRouter } from 'vue-router'
import { calculateAge } from '@/utils/dateUtils'
import axios from 'axios'
import IconEditProfile from '@/components/icons/IconEditProfile.vue'

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

const router = useRouter()
const heroData = ref<StudentHeroDto | null>(null)
const isLoading = ref(true)

// Загрузка
onMounted(async () => {
  try {
    // TODO: API URL в env переменные
    const response = await axios.get<StudentHeroDto>('http://localhost:5010/api/v1/student/hero', {
      headers: { Authorization: `Bearer ${localStorage.getItem('token')}` },
    })
    heroData.value = response.data
  } catch (err) {
    console.error('Hero load error:', err)
  } finally {
    isLoading.value = false
  }
})

// Возраст
const ageDisplay = computed(() => calculateAge(heroData.value?.birthDate))

// Инициалы
const initials = computed(() => {
  if (!heroData.value?.fullName) return 'ST'
  return heroData.value.fullName
    .split(' ')
    .slice(0, 2)
    .map((word) => word[0])
    .join('')
    .toUpperCase()
})

// Статусы
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
</script>

<template>
  <div class="container" style="margin-top: 24px">
    <div class="card hero-grid">
      <div v-if="isLoading" class="hero-message">Загрузка профиля...</div>
      <div v-else-if="!heroData" class="hero-message error">Данные не загружены</div>

      <template v-else>
        <div class="col-personal">
          <h1 class="page-title" style="margin-bottom: 4px">{{ heroData.fullName }}</h1>
          <div class="text-muted" style="margin-bottom: 16px" v-if="ageDisplay">
            {{ ageDisplay }}
          </div>

          <button class="btn-main btn-secondary" @click="router.push('/student/profile')">
            <IconEditProfile class="icon-main" />
            <span>Редактировать</span>
          </button>
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
  </div>
</template>

<style scoped>
/* Сетка компонента */
.hero-grid {
  display: grid;
  grid-template-columns: 1fr 1.2fr 120px; /* Пропорции колонок */
  gap: 32px;
  align-items: start;
  min-height: 160px;
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

/* 1. Личное */
.col-personal {
  display: flex;
  flex-direction: column;
  align-items: flex-start;
}

/* 2. Учеба */
.col-study {
  display: flex;
  flex-direction: column;
  padding-left: 32px;
}

.uni-name {
  font-size: 16px;
  font-weight: 700;
  color: var(--dark-text);
  margin: 0 0 12px 0;
  line-height: 1.3;
}

.study-info p {
  margin: 0 0 4px 0;
  font-size: 14px;
  color: var(--gray-text);
  line-height: 1.4;
}

.course-num {
  color: var(--dark-text);
}

.col-visual {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 12px;
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
  font-size: 12px;
  color: var(--gray-text);
  margin: 0;
  text-align: center;
}

@media (max-width: 768px) {
  .hero-grid {
    grid-template-columns: 1fr;
    gap: 24px;
    text-align: center;
  }

  .col-personal {
    align-items: center;
  }

  .col-study {
    border-left: none;
    padding-left: 0;
    border-top: 1px solid var(--gray-border);
    padding-top: 24px;
  }
}
</style>
