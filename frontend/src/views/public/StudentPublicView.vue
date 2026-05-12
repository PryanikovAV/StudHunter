<script setup lang="ts">
import { ref, onMounted, computed } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import apiClient from '@/api'
import AppCard from '@/components/AppCard.vue'
import BackButton from '@/components/BackButton.vue'
import InteractionButtons from '@/components/InteractionButtons.vue'
import DownloadButton from '@/components/DownloadButton.vue'
import ResumeDocument from '@/components/ResumeDocument.vue'
import { downloadResumePdf } from '@/utils/fileUtils'
import type { VacancyShortDto } from '@/types/student'
import type { ResumeDocumentDto } from '@/types/resume'

const route = useRoute()
const router = useRouter()
const studentId = computed(() => route.params.id as string)

const resume = ref<ResumeDocumentDto | null>(null)
const isLoading = ref(true)

const showInviteModal = ref(false)
const inviteMessage = ref('')
const selectedVacancyId = ref<string | null>(null)
const myVacancies = ref<VacancyShortDto[]>([])
const isInviting = ref(false)
const isDownloading = ref(false)

const role = computed(() => (localStorage.getItem('userRole') || '').toLowerCase())

const fetchResume = async () => {
  try {
    const response = await apiClient.get(`/resumes/${studentId.value}`)
    const data = response.data

    let citizenshipStatus = data.citizenship || null
    if (!citizenshipStatus && data.isForeign !== undefined) {
      if (data.isForeign === true) {
        citizenshipStatus = 'Иностранный студент'
      } else if (data.isForeign === false) {
        citizenshipStatus = 'Резидент'
      }
    }

    resume.value = {
      ...data,
      fullName: data.fullName || data.name || 'Неизвестный кандидат',
      skillNames: data.skills || [],
      citizenship: citizenshipStatus,
      updatedAt: data.updatedAt || null,
      isFavorite: data.isFavorite || false,
      isBlocked: data.isBlocked || false,
    } as ResumeDocumentDto
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

const handleDownload = async () => {
  if (!resume.value || isDownloading.value) return
  isDownloading.value = true
  await downloadResumePdf(resume.value.id || studentId.value, resume.value.fullName)
  isDownloading.value = false
}

onMounted(fetchResume)
</script>

<template>
  <div class="public-view-page">
    <BackButton />
    <div v-if="isLoading" class="loading">Загрузка профиля...</div>

    <div v-else-if="resume" class="resume-content">
      <ResumeDocument mode="employer" :data="resume">
        <template #actions-left>
          <DownloadButton
            :is-loading="isDownloading"
            title="Скачать PDF резюме"
            @click="handleDownload"
          />
          <InteractionButtons
            v-if="role === 'employer'"
            :target-id="studentId"
            favorite-type="Student"
            :block-user-id="studentId"
            :initial-is-favorite="resume.isFavorite"
            :initial-is-blocked="resume.isBlocked"
          />
        </template>
        <template #actions-right>
          <button
            v-if="role === 'employer'"
            class="btn-main btn-dark btn-large"
            @click="openInviteModal"
          >
            Пригласить
          </button>
        </template>
      </ResumeDocument>
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
            <textarea v-model="inviteMessage" class="input-main textarea-field" rows="4"></textarea>
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
.loading {
  text-align: center;
  color: var(--gray-text);
  padding: 40px;
}
.resume-content {
  display: flex;
  flex-direction: column;
}
.btn-large {
  font-size: 16px;
  padding: 12px 32px;
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
  .btn-large {
    width: 100%;
  }
}
</style>
