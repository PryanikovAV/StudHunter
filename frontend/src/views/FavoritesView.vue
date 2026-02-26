<script setup lang="ts">
import { ref, onMounted, watch } from 'vue'
import apiClient from '@/api'
import AppCard from '@/components/AppCard.vue'
import IconBuilding from '@/components/icons/IconBuilding.vue'

// --- ИНТЕРФЕЙСЫ ---
interface FavoriteCardDto {
  id: string
  targetId: string
  type: 'Vacancy' | 'Student' | 'Employer'
  title: string
  subtitle: string | null
  avatarUrl: string | null
  addedAt: string
}

interface BlackListDto {
  id: string
  blockedUserId: string
  displayName: string
  avatarUrl: string | null
  role: string
  blockedAt: string
}

// --- СОСТОЯНИЕ ---
const activeTab = ref<'favorites' | 'blacklist'>('favorites')

const favorites = ref<FavoriteCardDto[]>([])
const blacklist = ref<BlackListDto[]>([])

const isLoading = ref(true)

const enumTypeMap: Record<string, number> = { Vacancy: 0, Student: 1, Employer: 2 }

// --- ЗАГРУЗКА ДАННЫХ ---
const fetchData = async () => {
  isLoading.value = true
  try {
    if (activeTab.value === 'favorites') {
      const response = await apiClient.get('/favorites')
      favorites.value = response.data.items || response.data
    } else {
      const response = await apiClient.get('/blacklist')
      blacklist.value = response.data.items || response.data
    }
  } catch (error) {
    console.error(`Ошибка загрузки ${activeTab.value}:`, error)
  } finally {
    isLoading.value = false
  }
}

// Перезагружаем данные при переключении вкладки
watch(activeTab, fetchData)

// --- ДЕЙСТВИЯ: ИЗБРАННОЕ ---
const removeFavorite = async (fav: FavoriteCardDto) => {
  const index = favorites.value.findIndex((f) => f.id === fav.id)
  if (index === -1) return

  const removedItem = favorites.value[index]!
  favorites.value.splice(index, 1)

  try {
    await apiClient.post('/favorites/toggle', {
      targetId: fav.targetId,
      type: enumTypeMap[fav.type],
      isFavorite: false,
    })
  } catch (error) {
    console.error('Ошибка удаления из закладок:', error)
    favorites.value.splice(index, 0, removedItem)
    alert('Не удалось удалить из закладок.')
  }
}

// --- ДЕЙСТВИЯ: ЧЕРНЫЙ СПИСОК ---
const unblockUser = async (user: BlackListDto) => {
  const index = blacklist.value.findIndex((b) => b.id === user.id)
  if (index === -1) return

  const removedItem = blacklist.value[index]!
  blacklist.value.splice(index, 1)

  try {
    // В контроллере: POST api/v1/blacklist/toggle/{blockedUserId}?shouldBlock=false
    await apiClient.post(`/blacklist/toggle/${user.blockedUserId}?shouldBlock=false`)
  } catch (error) {
    console.error('Ошибка разблокировки:', error)
    blacklist.value.splice(index, 0, removedItem)
    alert('Не удалось разблокировать пользователя.')
  }
}

// --- ХЕЛПЕРЫ ДЛЯ ОТОБРАЖЕНИЯ ---
const getRoute = (fav: FavoriteCardDto) => {
  if (fav.type === 'Vacancy') return `/vacancies/${fav.targetId}`
  if (fav.type === 'Student') return `/students/${fav.targetId}`
  if (fav.type === 'Employer') return `/employers/${fav.targetId}`
  return '#'
}

const getTypeLabel = (type: string) => {
  if (type === 'Vacancy') return 'Вакансия'
  if (type === 'Student') return 'Кандидат'
  if (type === 'Employer') return 'Компания'
  return type
}

const formatDate = (dateString: string) => {
  return new Date(dateString).toLocaleDateString('ru-RU', { day: 'numeric', month: 'long' })
}

const getInitials = (title: string) => (title ? title.charAt(0).toUpperCase() : '?')

onMounted(fetchData)
</script>

<template>
  <div class="favorites-page">
    <div class="page-header">
      <h1 class="page-title">Связи и предпочтения</h1>

      <div class="tabs-container">
        <button
          class="tab-btn"
          :class="{ active: activeTab === 'favorites' }"
          @click="activeTab = 'favorites'"
        >
          Избранное
        </button>
        <button
          class="tab-btn"
          :class="{ active: activeTab === 'blacklist' }"
          @click="activeTab = 'blacklist'"
        >
          Черный список
        </button>
      </div>
    </div>

    <div v-if="isLoading" class="loading">Загрузка...</div>

    <template v-else-if="activeTab === 'favorites'">
      <div v-if="favorites.length === 0" class="empty-state">
        У вас пока нет закладок. <br />
        Добавляйте интересные вакансии или профили в избранное, чтобы не потерять их!
      </div>
      <div v-else class="favorites-list">
        <AppCard v-for="fav in favorites" :key="fav.id" class="fav-card">
          <div class="card-content">
            <div class="avatar-block">
              <img v-if="fav.avatarUrl" :src="fav.avatarUrl" alt="Avatar" class="avatar-img" />
              <div
                v-else-if="fav.type === 'Student'"
                class="avatar-placeholder student-placeholder"
              >
                {{ getInitials(fav.title) }}
              </div>
              <div v-else class="avatar-placeholder company-placeholder">
                <IconBuilding class="icon-small" />
              </div>
            </div>
            <div class="info-block">
              <div class="header-row">
                <span class="type-badge">{{ getTypeLabel(fav.type) }}</span>
                <span class="date-text">{{ formatDate(fav.addedAt) }}</span>
              </div>
              <router-link :to="getRoute(fav)" class="fav-title hover-link">
                {{ fav.title }}
              </router-link>
              <div class="fav-subtitle" v-if="fav.subtitle">{{ fav.subtitle }}</div>
            </div>
          </div>
          <div class="actions-block">
            <router-link :to="getRoute(fav)" class="btn-main btn-dark">Открыть</router-link>
            <button class="btn-main btn-outline" @click="removeFavorite(fav)">Убрать</button>
          </div>
        </AppCard>
      </div>
    </template>

    <template v-else-if="activeTab === 'blacklist'">
      <div v-if="blacklist.length === 0" class="empty-state">Черный список пуст.</div>
      <div v-else class="favorites-list">
        <AppCard v-for="user in blacklist" :key="user.id" class="fav-card">
          <div class="card-content">
            <div class="avatar-block">
              <img v-if="user.avatarUrl" :src="user.avatarUrl" alt="Avatar" class="avatar-img" />
              <div
                v-else-if="user.role === 'Student'"
                class="avatar-placeholder student-placeholder"
              >
                {{ getInitials(user.displayName) }}
              </div>
              <div v-else class="avatar-placeholder company-placeholder">
                <IconBuilding class="icon-small" />
              </div>
            </div>
            <div class="info-block">
              <div class="header-row">
                <span class="type-badge blocked-badge">Заблокирован</span>
                <span class="date-text">{{ formatDate(user.blockedAt) }}</span>
              </div>
              <div class="fav-title">{{ user.displayName }}</div>
              <div class="fav-subtitle">
                {{ user.role === 'Student' ? 'Кандидат' : 'Компания' }}
              </div>
            </div>
          </div>
          <div class="actions-block single-action">
            <button class="btn-main btn-outline" @click="unblockUser(user)">Разблокировать</button>
          </div>
        </AppCard>
      </div>
    </template>
  </div>
</template>

<style scoped>
.favorites-page {
  max-width: 850px;
  margin: 0 auto;
  padding-bottom: 40px;
}
.page-header {
  margin-bottom: 24px;
}
.page-title {
  font-size: 28px;
  color: var(--dark-text);
  margin-bottom: 16px;
}

/* Стили для вкладок */
.tabs-container {
  display: flex;
  gap: 8px;
  border-bottom: 2px solid var(--gray-border);
  padding-bottom: 0;
}
.tab-btn {
  background: none;
  border: none;
  font-size: 16px;
  font-weight: 500;
  color: var(--gray-text);
  padding: 8px 16px;
  cursor: pointer;
  border-bottom: 2px solid transparent;
  margin-bottom: -1px;
  position: relative;
  transition: all 0.2s;
}
.tab-btn:hover {
  color: var(--dark-text);
}
.tab-btn.active {
  color: var(--susu-blue);
}
/* Анимированная полоска под активной вкладкой */
.tab-btn.active::after {
  content: '';
  position: absolute;
  bottom: -2px;
  left: 0;
  right: 0;
  height: 2px;
  background-color: var(--susu-blue);
  border-radius: 2px 2px 0 0;
}

.empty-state {
  text-align: center;
  padding: 40px;
  color: var(--gray-text-focus);
  background: var(--background-field);
  border-radius: 12px;
  border: 1px dashed var(--gray-border);
  line-height: 1.6;
}

.favorites-list {
  display: flex;
  flex-direction: column;
  gap: 16px;
}

/* Карточка */
.fav-card {
  padding: 20px 24px;
  display: flex !important;
  flex-direction: row !important;
  align-items: center !important;
  justify-content: space-between !important;
  gap: 24px;
  width: 100%;
  box-sizing: border-box;
}

.card-content {
  display: flex;
  flex-direction: row;
  align-items: flex-start;
  gap: 20px;
  flex: 1 1 auto;
  min-width: 0;
}

/* Аватар */
.avatar-block {
  width: 64px;
  height: 64px;
  flex-shrink: 0;
  border-radius: 12px;
  overflow: hidden;
  border: 1px solid var(--gray-border);
}
.avatar-img {
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
  font-weight: 600;
  font-size: 24px;
}
.company-placeholder {
  background-color: #f1f5f9;
  color: #94a3b8;
}
.student-placeholder {
  background-color: #f0fdf4;
  color: #166534;
}
.icon-small {
  width: 28px;
  height: 28px;
  opacity: 0.7;
}

/* Информация */
.info-block {
  display: flex;
  flex-direction: column;
  gap: 6px;
  padding-top: 2px;
  text-align: left;
}
.header-row {
  display: flex;
  align-items: center;
  gap: 12px;
}
.type-badge {
  background-color: var(--background-page);
  border: 1px solid var(--gray-border);
  padding: 2px 8px;
  border-radius: 4px;
  font-size: 11px;
  text-transform: uppercase;
  letter-spacing: 0.5px;
  color: var(--gray-text-focus);
}
.blocked-badge {
  background-color: #fef2f2;
  color: #b91c1c;
  border-color: #fecaca;
}
.date-text {
  font-size: 13px;
  color: var(--gray-text);
}
.fav-title {
  font-size: 18px;
  font-weight: 600;
  color: var(--dark-text);
  text-decoration: none;
  line-height: 1.3;
}
.hover-link:hover {
  color: var(--susu-blue);
  text-decoration: underline;
}
.fav-subtitle {
  font-size: 14px;
  color: var(--gray-text-focus);
}

.actions-block {
  display: flex;
  flex-direction: column;
  gap: 10px;
  width: 140px;
  flex-shrink: 0;
}
/* Если кнопка только одна (в Черном списке), можно отцентрировать её по вертикали */
.single-action {
  justify-content: center;
}

.actions-block .btn-main {
  width: 100%;
  justify-content: center;
  padding: 8px 16px;
  font-size: 14px;
}

/* Адаптив */
@media (max-width: 640px) {
  .fav-card {
    flex-direction: column !important;
    align-items: stretch !important;
  }
  .actions-block {
    width: 100%;
    flex-direction: row;
    margin-top: 8px;
  }
}
</style>
