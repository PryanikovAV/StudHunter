<script setup lang="ts">
import { ref, onMounted, computed } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import apiClient from '@/api'
import AppCard from '@/components/AppCard.vue'
import BackButton from '@/components/BackButton.vue'
import InteractionButtons from '@/components/InteractionButtons.vue'

interface ResumeSearchDto {
  id: string
  studentId: string
  title: string
  description: string | null
  updatedAt: string
  email: string | null
  phone: string | null
  fullName: string
  facultyName: string | null
  studyDirectionName: string | null
  courseNumber: number | null
  skills: string[]
  isFavorite?: boolean
  isBlocked?: boolean
}

interface VacancyShortDto {
  id: string
  title: string
}

const route = useRoute()
const router = useRouter()
const studentId = computed(() => route.params.id as string)

const resume = ref<ResumeSearchDto | null>(null)
const isLoading = ref(true)

const showInviteModal = ref(false)
const inviteMessage = ref('')
const selectedVacancyId = ref<string | null>(null)
const myVacancies = ref<VacancyShortDto[]>([])
const isInviting = ref(false)

const role = computed(() => (localStorage.getItem('userRole') || '').toLowerCase())

const fetchResume = async () => {
  try {
    const response = await apiClient.get<ResumeSearchDto>(`/resumes/${studentId.value}`)
    resume.value = response.data
  } catch (error) {
    console.error('Ошибка загрузки резюме:', error)
    alert('Не удалось загрузить профиль студента. Возможно, он скрыт.')
    router.push('/')
  } finally {
    isLoading.value = false
  }
}

const loadMyVacancies = async () => {
  try {
    const response = await apiClient.get('/employer/vacancies?includeDeleted=false')
    myVacancies.value = response.data.items || response.data

    if (myVacancies.value.length > 0) {
      selectedVacancyId.value = myVacancies.value[0]?.id ?? null
    }
  } catch (error) {
    console.error('Ошибка загрузки вакансий:', error)
  }
}

const openInviteModal = async () => {
  await loadMyVacancies()
  if (myVacancies.value.length === 0) {
    alert('У вас нет активных вакансий! Сначала создайте вакансию, чтобы пригласить кандидата.')
    router.push('/employer/vacancies')
    return
  }
  showInviteModal.value = true
}

const submitInvite = async () => {
  isInviting.value = true
  try {
    const payload = {
      vacancyId: selectedVacancyId.value,
      message: inviteMessage.value || null,
    }
    await apiClient.post(`/resumes/${studentId.value}/invite`, payload)

    alert('Приглашение успешно отправлено!')
    showInviteModal.value = false
    inviteMessage.value = ''
  } catch (error) {
    console.error('Ошибка отправки приглашения:', error)
    alert('Не удалось отправить приглашение. Возможно, вы уже приглашали этого кандидата.')
  } finally {
    isInviting.value = false
  }
}

const formattedUpdateDate = computed(() => {
  if (!resume.value?.updatedAt) return ''
  return new Date(resume.value.updatedAt).toLocaleDateString('ru-RU', {
    day: 'numeric',
    month: 'long',
    year: 'numeric',
  })
})

onMounted(fetchResume)
</script>

<template>
  <div class="public-view-page">
    <BackButton />
    <div v-if="isLoading" class="loading">Загрузка профиля...</div>

    <div v-else-if="resume" class="resume-content">
      <AppCard class="profile-card">
        <div class="resume-header-section">
          <div class="resume-title-row">
            <h1 class="resume-title">{{ resume.title }}</h1>
          </div>

          <div class="candidate-info">
            <span class="candidate-name">{{ resume.fullName }}</span>
          </div>

          <div class="meta-tags">
            <span class="meta-tag">Обновлено: {{ formattedUpdateDate }}</span>
          </div>
        </div>

        <section class="form-section" v-if="resume.facultyName || resume.studyDirectionName">
          <h2 class="section-title">Образование</h2>
          <div class="education-info">
            <div class="info-item" v-if="resume.facultyName">
              <strong>Факультет/Институт:</strong> {{ resume.facultyName }}
            </div>
            <div class="info-item" v-if="resume.studyDirectionName">
              <strong>Направление:</strong> {{ resume.studyDirectionName }}
            </div>
            <div class="info-item" v-if="resume.courseNumber">
              <strong>Курс:</strong> {{ resume.courseNumber }}
            </div>
          </div>
        </section>

        <section class="form-section" v-if="resume.skills && resume.skills.length">
          <h2 class="section-title">Ключевые навыки</h2>
          <div class="tags-wrapper">
            <span v-for="skill in resume.skills" :key="skill" class="static-tag">{{ skill }}</span>
          </div>
        </section>

        <section class="form-section" v-if="resume.description">
          <h2 class="section-title">О себе</h2>
          <div class="description-text">{{ resume.description }}</div>
        </section>

        <section class="form-section" v-if="resume.email || resume.phone">
          <h2 class="section-title">Контакты</h2>
          <div class="contacts-list">
            <div class="contact-item" v-if="resume.email">
              <strong>Email:</strong> {{ resume.email }}
            </div>
            <div class="contact-item" v-if="resume.phone">
              <strong>Телефон:</strong> {{ resume.phone }}
            </div>
          </div>
        </section>

        <div class="actions" v-if="role === 'employer'">
          <div class="left-actions">
            <InteractionButtons
              :target-id="resume.studentId"
              favorite-type="Student"
              :initial-is-favorite="resume.isFavorite"
              :initial-is-blocked="resume.isBlocked"
            />
          </div>

          <button class="btn-main btn-dark btn-large" @click="openInviteModal">Пригласить</button>
        </div>
      </AppCard>
    </div>

    <Teleport to="body">
      <div v-if="showInviteModal" class="modal-overlay" @click.self="showInviteModal = false">
        <AppCard class="modal-card">
          <h3 class="modal-title">Приглашение кандидата</h3>
          <p class="modal-text">
            Выберите вакансию, на которую хотите пригласить <strong>{{ resume?.fullName }}</strong
            >.
          </p>

          <div class="form-group" style="margin-top: 16px">
            <label>Выберите вакансию</label>
            <select v-model="selectedVacancyId" class="input-main input-fix">
              <option v-for="vac in myVacancies" :key="vac.id" :value="vac.id">
                {{ vac.title }}
              </option>
            </select>
          </div>

          <div class="form-group" style="margin-top: 16px">
            <label>Сообщение кандидату (необязательно)</label>
            <textarea
              v-model="inviteMessage"
              class="input-main textarea-field"
              rows="4"
              placeholder="Здравствуйте! Ваше резюме показалось нам интересным..."
            ></textarea>
          </div>

          <div class="modal-actions">
            <button class="btn-main btn-secondary" @click="showInviteModal = false">Отмена</button>
            <button class="btn-main btn-dark" @click="submitInvite" :disabled="isInviting">
              {{ isInviting ? 'Отправка...' : 'Отправить приглашение' }}
            </button>
          </div>
        </AppCard>
      </div>
    </Teleport>
  </div>
</template>

<style scoped>
.public-view-page {
  max-width: 900px;
  margin: 0 auto;
  padding-top: 24px;
  padding-bottom: 40px;
}
.resume-content {
  display: flex;
  flex-direction: column;
}

.profile-card {
  padding: 32px;
  display: flex;
  flex-direction: column;
  gap: 32px;
}

.resume-header-section {
  display: flex;
  flex-direction: column;
  gap: 8px;
}
.resume-title-row {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  flex-wrap: wrap;
  gap: 16px;
}
.resume-title {
  margin: 0;
  font-size: 28px;
  color: var(--dark-text);
  font-weight: 700;
}
.candidate-info {
  font-size: 18px;
}
.candidate-name {
  font-weight: 600;
  color: var(--dark-text);
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

.education-info,
.contacts-list {
  display: flex;
  flex-direction: column;
  gap: 8px;
  font-size: 15px;
  color: var(--dark-text);
}
.info-item strong {
  font-weight: 500;
  color: var(--gray-text-focus);
  margin-right: 4px;
}

.description-text {
  white-space: pre-wrap;
  font-size: 15px;
  line-height: 1.6;
  color: var(--dark-text);
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
.input-fix {
  height: 44px;
  width: 100%;
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

@media (max-width: 640px) {
  .profile-card {
    padding: 24px 16px;
  }
  .resume-title-row {
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
