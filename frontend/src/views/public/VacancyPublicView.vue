<script setup lang="ts">
import { ref, onMounted, computed } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import apiClient from '@/api'
import AppCard from '@/components/AppCard.vue'
import BackButton from '@/components/BackButton.vue'
import InteractionButtons from '@/components/InteractionButtons.vue'

interface VacancyDetailsDto {
  id: string
  employerId: string
  title: string
  description: string | null
  salary: number | null
  type: string
  updatedAt: string
  employerName: string
  specializationName: string | null
  cityName: string | null
  actualAddress: string | null
  contactPhone: string | null
  contactEmail: string | null
  isDeleted: boolean
  courses: string[]
  skills: string[]
  isFavorite?: boolean
  isBlocked?: boolean
}

const route = useRoute()
const router = useRouter()
const vacancyId = computed(() => route.params.id as string)

const vacancy = ref<VacancyDetailsDto | null>(null)
const isLoading = ref(true)

const showApplyModal = ref(false)
const applyMessage = ref('')
const isApplying = ref(false)
const myResumeId = ref<string | null>(null)
const role = computed(() => (localStorage.getItem('userRole') || '').toLowerCase())

const fetchVacancy = async () => {
  try {
    const response = await apiClient.get<VacancyDetailsDto>(`/vacancies/${vacancyId.value}`)
    vacancy.value = response.data
  } catch (error) {
    console.error('Ошибка загрузки вакансии:', error)
    alert('Не удалось загрузить вакансию. Возможно, она была удалена.')
    router.push('/')
  } finally {
    isLoading.value = false
  }
}

const checkMyResume = async () => {
  try {
    const response = await apiClient.get('/my-resume')
    if (response.data && response.data.id) {
      myResumeId.value = response.data.id
    }
  } catch (error) {
    console.warn('У студента еще нет резюме или ошибка проверки.', error)
  }
}

const openApplyModal = async () => {
  await checkMyResume()
  if (!myResumeId.value) {
    alert('У вас еще нет резюме! Пожалуйста, создайте его в профиле перед откликом.')
    router.push('/student/profile')
    return
  }
  showApplyModal.value = true
}

const submitApplication = async () => {
  if (!myResumeId.value) return

  isApplying.value = true
  try {
    const payload = {
      resumeId: myResumeId.value,
      message: applyMessage.value || null,
    }

    await apiClient.post(`/vacancies/${vacancyId.value}/apply`, payload)

    alert('Отклик успешно отправлен!')
    showApplyModal.value = false
    applyMessage.value = ''
  } catch (error) {
    console.error('Ошибка отправки отклика:', error)
    alert('Не удалось отправить отклик. Возможно, вы уже откликались на эту вакансию.')
  } finally {
    isApplying.value = false
  }
}

const vacancyTypeDisplay = computed(() => {
  if (vacancy.value?.type === 'Internship') return 'Стажировка'
  if (vacancy.value?.type === 'Job') return 'Работа'
  return vacancy.value?.type
})

onMounted(fetchVacancy)
</script>

<template>
  <div class="public-view-page">
    <BackButton />
    <div v-if="isLoading" class="loading">Загрузка вакансии...</div>

    <div v-else-if="vacancy" class="vacancy-content">
      <AppCard class="profile-card">
        <div class="vacancy-header-section">
          <div class="vacancy-title-row">
            <h1 class="vacancy-title">{{ vacancy.title }}</h1>
            <div class="salary-text" v-if="vacancy.salary">
              {{ vacancy.salary.toLocaleString('ru-RU') }} ₽
            </div>
          </div>

          <div class="company-info">
            <router-link :to="`/employers/${vacancy.employerId}`" class="company-name hover-link">
              {{ vacancy.employerName }}
            </router-link>
            <span v-if="vacancy.cityName" class="text-muted">, {{ vacancy.cityName }}</span>
          </div>

          <div class="meta-tags">
            <span class="meta-tag">{{ vacancyTypeDisplay }}</span>
            <span v-if="vacancy.specializationName" class="meta-tag">{{
              vacancy.specializationName
            }}</span>
          </div>
        </div>

        <section
          class="form-section"
          v-if="vacancy.actualAddress || vacancy.contactEmail || vacancy.contactPhone"
        >
          <h2 class="section-title">Контакты</h2>
          <div class="contacts-list">
            <div class="contact-item" v-if="vacancy.actualAddress">
              <strong>Адрес:</strong> {{ vacancy.actualAddress }}
            </div>
            <div class="contact-item" v-if="vacancy.contactEmail">
              <strong>Email:</strong> {{ vacancy.contactEmail }}
            </div>
            <div class="contact-item" v-if="vacancy.contactPhone">
              <strong>Телефон:</strong> {{ vacancy.contactPhone }}
            </div>
          </div>
        </section>

        <section class="form-section" v-if="vacancy.description">
          <h2 class="section-title">Описание вакансии</h2>
          <div class="description-text">{{ vacancy.description }}</div>
        </section>

        <section class="form-section" v-if="vacancy.skills.length || vacancy.courses.length">
          <h2 class="section-title">Требования</h2>

          <div class="tags-group" v-if="vacancy.skills.length">
            <h3 class="group-title">Навыки</h3>
            <div class="tags-wrapper">
              <span v-for="skill in vacancy.skills" :key="skill" class="static-tag">{{
                skill
              }}</span>
            </div>
          </div>

          <div class="tags-group" v-if="vacancy.courses.length">
            <h3 class="group-title">Пройденные дисциплины</h3>
            <div class="tags-wrapper">
              <span v-for="course in vacancy.courses" :key="course" class="static-tag">{{
                course
              }}</span>
            </div>
          </div>
        </section>

        <div class="actions" v-if="role === 'student'">
          <div class="left-actions">
            <InteractionButtons
              :target-id="vacancy.id"
              favorite-type="Vacancy"
              :block-user-id="vacancy.employerId"
              :initial-is-favorite="vacancy.isFavorite"
              :initial-is-blocked="vacancy.isBlocked"
            />
          </div>

          <button class="btn-main btn-dark btn-large" @click="openApplyModal">Откликнуться</button>
        </div>
      </AppCard>
    </div>

    <Teleport to="body">
      <div v-if="showApplyModal" class="modal-overlay" @click.self="showApplyModal = false">
        <AppCard class="modal-card">
          <h3 class="modal-title">Отклик на вакансию</h3>
          <p class="modal-text">
            Вы собираетесь отправить отклик на позицию <strong>{{ vacancy?.title }}</strong
            >.
          </p>

          <div class="form-group" style="margin-top: 16px">
            <label>Сопроводительное письмо (необязательно)</label>
            <textarea
              v-model="applyMessage"
              class="input-main textarea-field"
              rows="4"
              placeholder="Здравствуйте! Меня заинтересовала ваша вакансия..."
            ></textarea>
          </div>

          <div class="modal-actions">
            <button class="btn-main btn-outline" @click="showApplyModal = false">Отмена</button>
            <button class="btn-main btn-dark" @click="submitApplication" :disabled="isApplying">
              {{ isApplying ? 'Отправка...' : 'Отправить отклик' }}
            </button>
          </div>
        </AppCard>
      </div>
    </Teleport>
  </div>
</template>

<style scoped>
.public-view-page {
  max-width: 800px;
  margin: 0 auto;
  padding-bottom: 40px;
}
.vacancy-content {
  display: flex;
  flex-direction: column;
}

.profile-card {
  padding: 32px;
  display: flex;
  flex-direction: column;
  gap: 32px;
}

.vacancy-header-section {
  display: flex;
  flex-direction: column;
  gap: 8px;
}
.vacancy-title-row {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  flex-wrap: wrap;
  gap: 16px;
}
.vacancy-title {
  margin: 0;
  font-size: 28px;
  color: var(--dark-text);
  font-weight: 700;
}
.salary-text {
  font-size: 22px;
  font-weight: 600;
  color: var(--dark-text);
  white-space: nowrap;
}
.company-info {
  font-size: 16px;
}
.company-name {
  font-weight: 600;
}
.hover-link {
  text-decoration: none;
  color: inherit;
  transition: color 0.2s;
}
.hover-link:hover {
  color: var(--susu-blue);
  text-decoration: underline;
}
.meta-tags {
  display: flex;
  gap: 12px;
  margin-top: 8px;
}
.meta-tag {
  background: #f1f5f9;
  color: #475569;
  padding: 6px 12px;
  border-radius: 6px;
  font-size: 13px;
  font-weight: 500;
}

.form-section {
  display: flex;
  flex-direction: column;
  gap: 16px;
}
.section-title {
  font-size: 18px;
  font-weight: 600;
  color: var(--dark-text);
  margin: 0;
  border-bottom: 1px solid var(--gray-border);
  padding-bottom: 8px;
}

.contacts-list {
  display: flex;
  flex-direction: column;
  gap: 8px;
  font-size: 15px;
  color: var(--dark-text);
}

.description-text {
  white-space: pre-wrap;
  font-size: 15px;
  line-height: 1.6;
  color: var(--dark-text);
}

.tags-group {
  display: flex;
  flex-direction: column;
  gap: 8px;
}
.group-title {
  font-size: 14px;
  color: var(--gray-text-focus);
  margin: 0;
  font-weight: 500;
}
.tags-wrapper {
  display: flex;
  flex-wrap: wrap;
  gap: 8px;
}
.static-tag {
  background: #f8fafc;
  border: 1px solid var(--gray-border);
  padding: 6px 12px;
  border-radius: 6px;
  font-size: 13px;
  color: var(--dark-text);
  display: inline-flex;
  align-items: center;
}
.actions {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding-top: 24px;
  border-top: 1px solid var(--gray-border);
}

.left-actions {
  display: flex;
  align-items: center;
}

.btn-large {
  font-size: 16px;
  padding: 12px 32px;
  height: auto;
}
.modal-overlay {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background-color: rgba(0, 0, 0, 0.5);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 1000;
  padding: 20px;
}
.modal-card {
  max-width: 500px;
  width: 100%;
  padding: 32px;
  box-shadow: 0 10px 25px rgba(0, 0, 0, 0.1);
}
.modal-title {
  margin: 0 0 12px 0;
  font-size: 20px;
}
.modal-text {
  margin: 0;
  font-size: 15px;
  color: var(--gray-text-focus);
}
.form-group label {
  font-size: 14px;
  font-weight: 500;
  color: var(--dark-text);
  display: block;
  margin-bottom: 6px;
}
.textarea-field {
  width: 100%;
  box-sizing: border-box;
  padding: 12px;
  border: 1px solid var(--gray-border);
  border-radius: 8px;
  background-color: var(--background-field);
  resize: vertical;
}
.modal-actions {
  display: flex;
  justify-content: flex-end;
  gap: 12px;
  margin-top: 24px;
}
.public-view-page {
  max-width: 800px;
  margin: 0 auto;
  padding-top: 24px;
  padding-bottom: 40px;
}

@media (max-width: 640px) {
  .profile-card {
    padding: 24px 16px;
  }
  .vacancy-title-row {
    flex-direction: column;
    gap: 8px;
  }
  .actions {
    flex-direction: column-reverse;
    gap: 16px;
  }
  .left-actions {
    width: 100%;
    justify-content: center;
  }
  .btn-large {
    width: 100%;
  }
}
</style>
