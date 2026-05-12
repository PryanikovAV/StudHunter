<script setup lang="ts">
import { ref, onMounted, computed } from 'vue'
import { useToast } from 'vue-toastification'
import apiClient from '@/api'
import AppCard from '@/components/AppCard.vue'
import IconBuilding from '@/components/icons/IconBuilding.vue'
import InteractionButtons from '@/components/InteractionButtons.vue'
import type { EmployerHeroDto } from '@/types/employer'

const props = withDefaults(
  defineProps<{
    employerId?: string | null
    readonlyMode?: boolean
    showInteractions?: boolean
  }>(),
  {
    employerId: null,
    readonlyMode: false,
    showInteractions: false,
  },
)

const toast = useToast()
// const isDev = import.meta.env.DEV // TODO: вернуть на релизе
const heroData = ref<EmployerHeroDto | null>(null)
const isLoading = ref(true)
const fileInput = ref<HTMLInputElement | null>(null)
const isUploading = ref(false)

const changeDebugStage = async (event: Event) => {
  const target = event.target as HTMLSelectElement
  const newStage = target.value

  const idToUpdate = props.employerId || heroData.value?.id
  if (!idToUpdate) return

  try {
    await apiClient.patch(`/debug/employers/${idToUpdate}/stage?stage=${newStage}`)
    await loadHeroData()
    toast.success('Статус успешно изменен (Debug)')
  } catch (err) {
    console.error('Ошибка смены статуса (Debug):', err)
  }
}

const loadHeroData = async () => {
  try {
    const endpoint = props.employerId
      ? `/employers/${props.employerId}/hero`
      : '/employers/me/profile'

    const response = await apiClient.get<EmployerHeroDto>(endpoint)
    heroData.value = response.data
  } catch (err) {
    console.error('Employer Hero load error:', err)
  } finally {
    isLoading.value = false
  }
}

onMounted(loadHeroData)
defineExpose({ loadHeroData })

const vacanciesText = computed(() => {
  const count = heroData.value?.activeVacanciesCount || 0
  if (count === 0) return 'Нет активных вакансий'

  const lastDigit = count % 10
  const lastTwoDigits = count % 100

  if (lastTwoDigits >= 11 && lastTwoDigits <= 14) return `${count} вакансий`
  if (lastDigit === 1) return `${count} вакансия`
  if (lastDigit >= 2 && lastDigit <= 4) return `${count} вакансии`
  return `${count} вакансий`
})

const showLegalAddress = computed(() => {
  if (!heroData.value?.legalAddress) return false
  if (!heroData.value?.actualAddress) return true
  return (
    heroData.value.legalAddress.trim().toLowerCase() !==
    heroData.value.actualAddress.trim().toLowerCase()
  )
})

const statusInfo = computed(() => {
  const stage = heroData.value?.registrationStage?.toString()
  switch (stage) {
    case 'FullyActivated':
    case '3':
      return { text: 'Аккредитован', colorClass: 'status-green' }
    case 'ProfileFilled':
    case '2':
      return { text: 'Модерация', colorClass: 'status-blue' }
    case 'Anonymous':
    case '1':
    default:
      return { text: 'Аноним', colorClass: 'status-red' }
  }
})

const handleAvatarClick = () => {
  if (props.readonlyMode || isUploading.value) return
  fileInput.value?.click()
}

const handleFileSelect = async (event: Event) => {
  const target = event.target as HTMLInputElement
  const file = target.files?.[0]

  if (!file) return

  if (file.size > 5 * 1024 * 1024) {
    toast.warning('Размер файла не должен превышать 5 Мб')
    return
  }

  isUploading.value = true
  try {
    const formData = new FormData()
    formData.append('file', file)

    const uploadRes = await apiClient.post('/files/images?type=avatars', formData, {
      headers: { 'Content-Type': 'multipart/form-data' },
    })

    await apiClient.put('/employers/me/avatar', {
      avatarUrl: uploadRes.data.url,
    })

    await loadHeroData()
    toast.success('Логотип успешно обновлен')
  } catch (error) {
    console.error('Ошибка при обновлении логотипа:', error)
  } finally {
    isUploading.value = false
    if (fileInput.value) fileInput.value.value = ''
  }
}
</script>

<template>
  <div class="container" style="margin-top: 24px">
    <AppCard>
      <input
        v-if="!readonlyMode"
        type="file"
        ref="fileInput"
        accept="image/jpeg, image/png, image/webp"
        style="display: none"
        @change="handleFileSelect"
      />

      <div class="hero-grid">
        <div v-if="isLoading" class="hero-message">Загрузка профиля компании...</div>
        <div v-else-if="!heroData" class="hero-message error">Данные не загружены</div>

        <template v-else>
          <div class="col-main">
            <div class="title-row">
              <h1 class="page-title">{{ heroData.name }}</h1>
              <div class="status-indicator" :class="statusInfo.colorClass">
                <span class="status-dot"></span>
                <span class="status-text">{{ statusInfo.text }}</span>
              </div>
            </div>

            <h2 class="specialization-name" v-if="heroData.specializationName">
              {{ heroData.specializationName }}
            </h2>

            <div class="contacts-block">
              <div class="contact-row" v-if="heroData.cityName || heroData.website">
                <span v-if="heroData.cityName" class="text-semibold"
                  >г. {{ heroData.cityName }}</span
                >
                <span v-if="heroData.cityName && heroData.website" class="dot-separator">•</span>
                <a
                  v-if="heroData.website"
                  :href="heroData.website"
                  target="_blank"
                  class="website-link"
                >
                  {{ heroData.website.replace(/^https?:\/\//, '') }}
                </a>
              </div>
              <div class="contact-row" v-if="heroData.contactPhone">
                <span class="contact-label">Тел:</span> {{ heroData.contactPhone }}
              </div>
              <div class="contact-row" v-if="heroData.contactEmail">
                <span class="contact-label">Email:</span> {{ heroData.contactEmail }}
              </div>
            </div>
          </div>

          <div class="col-legal">
            <div class="legal-info-block">
              <div v-if="heroData.actualAddress" class="legal-row">
                <span class="legal-label">Адрес:</span>
                <span class="legal-value">{{ heroData.actualAddress }}</span>
              </div>
              <div v-if="showLegalAddress" class="legal-row">
                <span class="legal-label">Юр. адрес:</span>
                <span class="legal-value">{{ heroData.legalAddress }}</span>
              </div>

              <div
                class="legal-separator"
                v-if="heroData.inn || heroData.ogrn || heroData.kpp"
              ></div>

              <div v-if="heroData.inn" class="legal-row">
                <span class="legal-label">ИНН:</span>
                <span class="legal-value mono">{{ heroData.inn }}</span>
              </div>
              <div v-if="heroData.ogrn" class="legal-row">
                <span class="legal-label">ОГРН:</span>
                <span class="legal-value mono">{{ heroData.ogrn }}</span>
              </div>
              <div v-if="heroData.kpp" class="legal-row">
                <span class="legal-label">КПП:</span>
                <span class="legal-value mono">{{ heroData.kpp }}</span>
              </div>
            </div>
          </div>

          <div class="col-visual">
            <!-- <div
              class="debug-wrapper"
              v-if="isDev && !readonlyMode"
              title="Сменить стадию аккредитации"
            > -->
            <!-- TODO: вернуть на релизе -->
            <div class="debug-wrapper" v-if="!readonlyMode" title="Сменить стадию аккредитации">
              <span class="debug-badge">DEBUG</span>
              <select
                class="debug-stage-select"
                @change="changeDebugStage"
                :value="heroData.registrationStage"
              >
                <option value="1">1 - Аноним</option>
                <option value="2">2 - Модерация</option>
                <option value="3">3 - Аккредитован</option>
              </select>
            </div>

            <div
              class="avatar"
              @click="handleAvatarClick"
              :class="{ 'is-loading': isUploading, editable: !readonlyMode }"
            >
              <img v-if="heroData.avatarUrl" :src="heroData.avatarUrl" alt="Company Logo" />
              <div v-else class="avatar-placeholder">
                <IconBuilding class="icon-large" />
              </div>

              <div class="avatar-overlay" v-if="!readonlyMode">
                <span class="overlay-text">{{ isUploading ? 'Загрузка...' : 'Изменить' }}</span>
              </div>
            </div>

            <p class="vacancies-count">{{ vacanciesText }}</p>

            <div class="employer-actions" v-if="showInteractions && heroData.id">
              <InteractionButtons
                :target-id="heroData.id"
                favorite-type="Employer"
                :initial-is-favorite="heroData.isFavorite"
                :initial-is-blocked="heroData.isBlocked"
              />
            </div>
          </div>
        </template>
      </div>
    </AppCard>
  </div>
</template>

<style scoped>
.hero-grid {
  display: grid;
  grid-template-columns: 1fr 1fr 150px;
  gap: 32px;
  align-items: center;
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

.col-main {
  display: flex;
  flex-direction: column;
  gap: 8px;
}
.title-row {
  display: flex;
  align-items: center;
  gap: 12px;
  flex-wrap: wrap;
}
.page-title {
  margin: 0;
  font-size: 24px;
  font-weight: 700;
  line-height: 1.2;
  color: var(--dark-text);
}
.specialization-name {
  font-size: 15px;
  font-weight: 500;
  color: var(--susu-blue);
  margin: 0 0 4px 0;
}
.contacts-block {
  display: flex;
  flex-direction: column;
  gap: 6px;
}
.contact-row {
  font-size: 14px;
  color: var(--dark-text);
  display: flex;
  align-items: center;
  gap: 6px;
}
.contact-label {
  color: var(--gray-text);
}
.text-semibold {
  font-weight: 500;
}
.dot-separator {
  color: var(--gray-border);
}
.website-link {
  color: var(--susu-blue);
  text-decoration: none;
}
.website-link:hover {
  text-decoration: underline;
}

.status-indicator {
  display: flex;
  align-items: center;
  gap: 6px;
  font-size: 12px;
  font-weight: 600;
  padding: 4px 10px;
  border-radius: 100px;
}
.status-dot {
  width: 8px;
  height: 8px;
  border-radius: 50%;
  flex-shrink: 0;
}
.status-red {
  background: #fef2f2;
  color: #dc2626;
}
.status-red .status-dot {
  background: #dc2626;
}
.status-blue {
  background: #eff6ff;
  color: #2563eb;
}
.status-blue .status-dot {
  background: #2563eb;
}
.status-green {
  background: #f0fdf4;
  color: #16a34a;
}
.status-green .status-dot {
  background: #16a34a;
}

.col-legal {
  display: flex;
  flex-direction: column;
  justify-content: center;
  padding-left: 24px;
  border-left: 1px solid var(--gray-border);
}
.legal-info-block {
  display: flex;
  flex-direction: column;
  gap: 4px;
}
.legal-row {
  display: flex;
  align-items: baseline;
  gap: 8px;
  font-size: 13px;
}
.legal-label {
  color: var(--gray-text);
  min-width: 65px;
}
.legal-value {
  color: var(--dark-text);
  line-height: 1.4;
}
.legal-separator {
  height: 8px;
}
.mono {
  font-family: monospace;
  font-size: 14px;
  color: var(--gray-text-focus);
}

.col-visual {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  gap: 10px;
}

.debug-wrapper {
  display: flex;
  align-items: center;
  width: 100%;
  background-color: #ffffff;
  border: 1px dashed var(--gray-border);
  border-radius: 4px;
  padding: 2px;
  transition: border-color 0.2s;
}
.debug-wrapper:hover {
  border-color: var(--gray-text);
}

.debug-badge {
  background-color: #f1f5f9;
  color: var(--gray-text-focus);
  font-size: 10px;
  font-weight: 700;
  padding: 2px 4px;
  border-radius: 2px;
  margin-right: 4px;
  font-family: monospace;
}

.debug-stage-select {
  flex: 1;
  border: none;
  background: transparent;
  outline: none;
  color: var(--gray-text-focus);
  font-family: monospace;
  font-size: 11px;
  cursor: pointer;
}
.debug-stage-select:hover {
  border-color: var(--gray-text);
}

.avatar {
  position: relative;
  width: 90px;
  height: 90px;
  border-radius: 16px;
  overflow: hidden;
  background-color: var(--background-page);
  border: 1px solid var(--gray-border);
}
.avatar.editable {
  cursor: pointer;
}
.avatar img {
  width: 100%;
  height: 100%;
  object-fit: contain;
  padding: 4px;
}
.avatar-placeholder {
  width: 100%;
  height: 100%;
  display: flex;
  align-items: center;
  justify-content: center;
  background: linear-gradient(135deg, #f8fafc 0%, #e2e8f0 100%);
  color: var(--gray-text-focus);
}
.icon-large {
  width: 36px;
  height: 36px;
  opacity: 0.8;
}
.avatar-overlay {
  position: absolute;
  inset: 0;
  background: rgba(0, 0, 0, 0.6);
  display: flex;
  align-items: center;
  justify-content: center;
  opacity: 0;
  transition: opacity 0.2s ease;
}
.avatar.editable:hover .avatar-overlay {
  opacity: 1;
}
.avatar.is-loading .avatar-overlay {
  opacity: 1;
  background: rgba(0, 0, 0, 0.8);
}
.overlay-text {
  color: #fff;
  font-size: 12px;
  font-weight: 500;
  text-align: center;
}

.vacancies-count {
  font-size: 13px;
  color: var(--susu-blue);
  font-weight: 600;
  margin: 0;
  text-align: center;
}
.employer-actions {
  display: flex;
  justify-content: center;
  width: 100%;
}

@media (max-width: 860px) {
  .hero-grid {
    grid-template-columns: 1fr;
    gap: 20px;
  }
  .col-legal {
    padding-left: 0;
    border-left: none;
    border-top: 1px solid var(--gray-border);
    padding-top: 16px;
  }
  .col-visual {
    border-top: 1px solid var(--gray-border);
    padding-top: 16px;
  }
  .debug-stage-select {
    width: auto;
    min-width: 120px;
  }
}
</style>
