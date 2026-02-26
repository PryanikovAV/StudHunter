<script setup lang="ts">
import { ref } from 'vue'
import AppCard from '@/components/AppCard.vue'

defineProps<{
  isSavingPassword?: boolean
  isDeleting?: boolean
}>()

const emit = defineEmits<{
  (e: 'update-password', payload: { currentPassword: string; newPassword: string }): void
  (e: 'delete-account', password: string): void
}>()

const passwords = ref({ currentPassword: '', newPassword: '', confirmPassword: '' })
const showDeleteModal = ref(false)
const deletePassword = ref('')

const handlePasswordSubmit = () => {
  if (passwords.value.newPassword !== passwords.value.confirmPassword) {
    alert('Новые пароли не совпадают!')
    return
  }
  emit('update-password', {
    currentPassword: passwords.value.currentPassword,
    newPassword: passwords.value.newPassword,
  })
}

const handleDeleteSubmit = () => {
  if (!deletePassword.value) {
    alert('Пожалуйста, введите пароль для подтверждения.')
    return
  }
  emit('delete-account', deletePassword.value)
}

const resetPasswordForm = () => {
  passwords.value = { currentPassword: '', newPassword: '', confirmPassword: '' }
}

const closeDeleteModal = () => {
  showDeleteModal.value = false
  deletePassword.value = ''
}

defineExpose({
  resetPasswordForm,
  closeDeleteModal,
})
</script>

<template>
  <AppCard class="profile-card safety-card">
    <h2 class="section-title">Безопасность и настройки</h2>

    <slot name="top"></slot>

    <div class="safety-block">
      <form @submit.prevent="handlePasswordSubmit" class="password-form">
        <div class="form-group">
          <label>Текущий пароль</label>
          <input
            v-model="passwords.currentPassword"
            type="password"
            required
            class="input-main input-fix"
          />
        </div>
        <div class="form-group">
          <label>Новый пароль</label>
          <input
            v-model="passwords.newPassword"
            type="password"
            required
            minlength="8"
            class="input-main input-fix"
          />
        </div>
        <div class="form-group">
          <label>Подтвердите новый пароль</label>
          <input
            v-model="passwords.confirmPassword"
            type="password"
            required
            minlength="8"
            class="input-main input-fix"
          />
        </div>

        <div class="password-actions">
          <button type="submit" class="btn-main btn-dark" :disabled="isSavingPassword">
            Сохранить
          </button>
          <button type="button" class="btn-main btn-secondary" @click="showDeleteModal = true">
            Удалить аккаунт
          </button>
        </div>
      </form>
    </div>

    <Teleport to="body">
      <div v-if="showDeleteModal" class="modal-overlay" @click.self="showDeleteModal = false">
        <AppCard class="modal-card">
          <h3 class="modal-title">Внимание</h3>
          <p class="modal-text">
            Ваш аккаунт будет удалён, все Вакансии и приглашения будут скрыты или удалены.
            Восстановить аккаунт возможно в течение 6 месяцев.
          </p>

          <div class="form-group" style="margin-top: 16px">
            <label>Введите текущий пароль для подтверждения:</label>
            <input
              v-model="deletePassword"
              type="password"
              class="input-main input-fix"
              placeholder="Ваш пароль"
            />
          </div>

          <div class="modal-actions">
            <button class="btn-main btn-dark" @click="showDeleteModal = false">Отмена</button>
            <button class="btn-main btn-outline" @click="handleDeleteSubmit" :disabled="isDeleting">
              Подтвердить
            </button>
          </div>
        </AppCard>
      </div>
    </Teleport>
  </AppCard>
</template>

<style scoped>
.profile-card {
  padding: 32px;
}
.safety-card {
}
.section-title {
  font-size: 18px;
  font-weight: 600;
  color: var(--dark-text);
  margin: 0;
  border-bottom: 1px solid var(--gray-border);
  padding-bottom: 8px;
}
.safety-block {
  margin: 16px 0;
}
.password-form {
  display: flex;
  flex-direction: column;
  gap: 16px;
  width: 100%;
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
.password-actions {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-top: 8px;
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
  max-width: 450px;
  width: 100%;
  padding: 32px;
  box-shadow: 0 10px 25px rgba(0, 0, 0, 0.1);
}
.modal-title {
  margin: 0 0 12px 0;
  color: var(--dark-text);
  font-size: 20px;
}
.modal-text {
  margin: 0;
  color: var(--gray-text-focus);
  font-size: 14px;
  line-height: 1.5;
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
  .password-actions {
    flex-direction: column;
    gap: 16px;
    align-items: stretch;
  }
}
</style>
