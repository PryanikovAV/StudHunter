<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import apiClient from '@/api'
import AppCard from '@/components/AppCard.vue'
import SecuritySettingsCard from '@/components/SecuritySettingsCard.vue'

interface EmployerDto {
  id: string
  email: string
  name: string
  cityId: string | null
  cityName: string | null
  contactPhone: string | null
  contactEmail: string | null
  avatarUrl: string | null
  description: string | null
  website: string | null
  specializationId: string | null
  inn: string | null
  ogrn: string | null
  kpp: string | null
  legalAddress: string | null
  actualAddress: string | null
}

const profile = ref<EmployerDto | null>(null)
const isLoading = ref(true)
const isSavingProfile = ref(false)

const router = useRouter()

const securityCardRef = ref<InstanceType<typeof SecuritySettingsCard> | null>(null)
const isSavingPassword = ref(false)
const isDeleting = ref(false)

const handleUpdatePassword = async (payload: { currentPassword: string; newPassword: string }) => {
  isSavingPassword.value = true
  try {
    await apiClient.put('/employers/me/password', payload)
    alert('Пароль успешно изменен!')
    if (securityCardRef.value) securityCardRef.value.resetPasswordForm()
  } catch (error) {
    console.error('Ошибка изменения пароля', error)
    alert('Не удалось изменить пароль. Проверьте текущий пароль.')
  } finally {
    isSavingPassword.value = false
  }
}

const handleDeleteAccount = async (password: string) => {
  isDeleting.value = true
  try {
    await apiClient.delete('/employers/me', { data: { password } })
    if (securityCardRef.value) securityCardRef.value.closeDeleteModal()
    localStorage.removeItem('token')
    localStorage.removeItem('userRole')
    router.push('/login')
  } catch (error) {
    console.error('Ошибка удаления', error)
    alert('Не удалось удалить аккаунт. Возможно, неверный пароль.')
  } finally {
    isDeleting.value = false
  }
}

const dictionaries = ref({
  cities: [] as { id: string; name: string }[],
  specializations: [] as { id: string; name: string }[],
})

const fetchProfile = async () => {
  try {
    isLoading.value = true
    const response = await apiClient.get<EmployerDto>('/employers/me/profile')
    profile.value = response.data
  } catch (error) {
    console.error('Ошибка загрузки профиля компании', error)
  } finally {
    isLoading.value = false
  }
}

const saveProfileData = async () => {
  if (!profile.value) return
  isSavingProfile.value = true
  try {
    const payload = {
      name: profile.value.name,
      cityId: profile.value.cityId,
      description: profile.value.description,
      contactPhone: profile.value.contactPhone,
      contactEmail: profile.value.contactEmail,
      website: profile.value.website,
      specializationId: profile.value.specializationId,
      inn: profile.value.inn,
      ogrn: profile.value.ogrn,
      kpp: profile.value.kpp,
      legalAddress: profile.value.legalAddress,
      actualAddress: profile.value.actualAddress,
    }
    await apiClient.put('/employers/me/profile', payload)
    alert('Профиль компании успешно обновлен!')
  } catch (error) {
    console.error('Ошибка сохранения профиля', error)
    alert('Не удалось сохранить изменения.')
  } finally {
    isSavingProfile.value = false
  }
}

onMounted(fetchProfile)
</script>

<template>
  <div class="profile-page">
    <h1 class="page-title">Настройки профиля</h1>

    <div v-if="isLoading" class="loading">Загрузка данных...</div>

    <div v-else-if="profile" class="settings-layout">
      <AppCard class="profile-card">
        <form @submit.prevent="saveProfileData" class="profile-form">
          <section class="form-section">
            <h2 class="section-title">Основная информация</h2>
            <div class="form-grid">
              <div class="form-group full-width">
                <label>Название компании <span class="required">*</span></label>
                <input v-model="profile.name" type="text" required class="input-main input-fix" />
              </div>
              <div class="form-group">
                <label>Сфера деятельности</label>
                <select v-model="profile.specializationId" class="input-main input-fix">
                  <option :value="null">Не указана</option>
                  <option
                    v-for="spec in dictionaries.specializations"
                    :key="spec.id"
                    :value="spec.id"
                  >
                    {{ spec.name }}
                  </option>
                </select>
              </div>
              <div class="form-group">
                <label>Сайт компании</label>
                <input v-model="profile.website" type="url" class="input-main input-fix" />
              </div>
              <div class="form-group full-width">
                <label>Описание компании</label>
                <textarea
                  v-model="profile.description"
                  class="input-main textarea-field"
                  rows="4"
                ></textarea>
              </div>
            </div>
          </section>

          <section class="form-section">
            <h2 class="section-title">Контакты</h2>
            <div class="form-grid">
              <div class="form-group">
                <label>Город</label>
                <select v-model="profile.cityId" class="input-main input-fix">
                  <option :value="null">Не указан</option>
                  <option v-for="city in dictionaries.cities" :key="city.id" :value="city.id">
                    {{ city.name }}
                  </option>
                </select>
              </div>
              <div class="form-group"></div>
              <div class="form-group">
                <label>Контактный телефон</label>
                <input v-model="profile.contactPhone" type="tel" class="input-main input-fix" />
              </div>
              <div class="form-group">
                <label>Контактный Email</label>
                <input v-model="profile.contactEmail" type="email" class="input-main input-fix" />
              </div>
            </div>
          </section>

          <section class="form-section">
            <h2 class="section-title">Реквизиты организации</h2>
            <div class="form-grid">
              <div class="form-group">
                <label>ИНН</label
                ><input
                  v-model="profile.inn"
                  type="text"
                  maxlength="12"
                  class="input-main input-fix"
                />
              </div>
              <div class="form-group">
                <label>ОГРН</label
                ><input
                  v-model="profile.ogrn"
                  type="text"
                  maxlength="15"
                  class="input-main input-fix"
                />
              </div>
              <div class="form-group">
                <label>КПП</label
                ><input
                  v-model="profile.kpp"
                  type="text"
                  maxlength="9"
                  class="input-main input-fix"
                />
              </div>
              <div class="form-group full-width">
                <label>Юридический адрес</label
                ><input v-model="profile.legalAddress" type="text" class="input-main input-fix" />
              </div>
              <div class="form-group full-width">
                <label>Фактический адрес</label
                ><input v-model="profile.actualAddress" type="text" class="input-main input-fix" />
              </div>
            </div>
          </section>

          <div class="actions">
            <button type="submit" class="btn-main btn-dark" :disabled="isSavingProfile">
              {{ isSavingProfile ? 'Сохранение...' : 'Сохранить профиль' }}
            </button>
          </div>
        </form>
      </AppCard>

      <SecuritySettingsCard
        ref="securityCardRef"
        :is-saving-password="isSavingPassword"
        :is-deleting="isDeleting"
        @update-password="handleUpdatePassword"
        @delete-account="handleDeleteAccount"
      />
    </div>
  </div>
</template>

<style scoped>
.profile-page {
  max-width: 800px;
  margin: 0 auto;
}
.page-title {
  margin-bottom: 24px;
  color: var(--dark-text);
}
.settings-layout {
  display: flex;
  flex-direction: column;
  gap: 24px;
}
.profile-card {
  padding: 32px;
}
.profile-form {
  display: flex;
  flex-direction: column;
  gap: 32px;
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
.form-grid {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 16px;
}
.full-width {
  grid-column: 1 / -1;
}
.form-group {
  display: flex;
  flex-direction: column;
  gap: 6px;
}
.form-group label {
  font-size: 14px;
  font-weight: 500;
  color: var(--dark-text);
}
.input-fix {
  height: 44px;
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
.actions {
  display: flex;
  justify-content: flex-end;
  margin-top: 16px;
  padding-top: 24px;
  border-top: 1px solid var(--gray-border);
}
.inline-form {
  display: flex;
  gap: 12px;
  align-items: center;
}
.inline-form .input-main {
  flex-grow: 1;
}
.divider {
  border: none;
  border-top: 1px solid var(--gray-border);
  margin: 24px 0;
}
@media (max-width: 640px) {
  .form-grid {
    grid-template-columns: 1fr;
  }
  .profile-card {
    padding: 24px 16px;
  }
  .inline-form {
    flex-direction: column;
    align-items: stretch;
  }
}
</style>
