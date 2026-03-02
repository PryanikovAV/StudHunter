<script setup lang="ts">
import type { ChatDto } from '@/types/chat'
import { formatChatTime } from '@/utils/dateUtils'
import IconLock from '@/components/icons/IconLock.vue'

defineProps<{
  chats: ChatDto[]
  selectedChatId: string | null
  isLoading: boolean
}>()

defineEmits<{
  (e: 'select', chatId: string): void
}>()

const getInitials = (name: string) => (name ? name.charAt(0).toUpperCase() : '?')
</script>

<template>
  <div class="chat-sidebar" :class="{ 'hidden-on-mobile': selectedChatId }">
    <div class="sidebar-header">
      <h2 class="sidebar-title">Собеседники</h2>
    </div>

    <div class="chat-list">
      <div v-if="isLoading" class="sidebar-message">Загрузка...</div>
      <div v-else-if="chats.length === 0" class="sidebar-message">Нет активных диалогов</div>

      <div
        v-else
        v-for="chat in chats"
        :key="chat.id"
        class="chat-list-item"
        :class="{ active: selectedChatId === chat.id }"
        @click="$emit('select', chat.id)"
      >
        <div
          class="chat-avatar"
          :class="chat.interlocutor.role === 'Student' ? 'student-bg' : 'company-bg'"
        >
          {{ getInitials(chat.interlocutor.displayName) }}
        </div>

        <div class="chat-preview">
          <div class="preview-header">
            <span
              class="chat-name"
              :class="{ 'text-blocked': chat.isBlockedByMe || chat.isBlockedByInterlocutor }"
            >
              {{ chat.interlocutor.displayName }}
            </span>
            <span class="chat-time">{{ formatChatTime(chat.lastMessageAt) }}</span>
          </div>
          <div class="preview-footer">
            <span
              class="chat-last-msg"
              :class="{ 'text-blocked': chat.isBlockedByMe || chat.isBlockedByInterlocutor }"
            >
              <IconLock
                v-if="chat.isBlockedByMe || chat.isBlockedByInterlocutor"
                class="icon-tiny"
              />
              {{ chat.isBlockedByMe ? 'Вы заблокировали пользователя' : chat.lastMessage }}
            </span>
            <span v-if="chat.unreadCount && chat.unreadCount > 0" class="unread-dot"></span>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
.chat-sidebar {
  width: 320px;
  min-width: 320px;
  border-right: 1px solid var(--gray-border);
  display: flex;
  flex-direction: column;
  background-color: var(--background-page);
}
.sidebar-header {
  height: 72px;
  padding: 0 20px;
  border-bottom: 1px solid var(--gray-border);
  display: flex;
  align-items: center;
  flex-shrink: 0;
}
.sidebar-title {
  margin: 0;
  font-size: 20px;
  font-weight: 600;
  color: var(--dark-text);
}
.sidebar-message {
  padding: 24px;
  text-align: center;
  color: var(--gray-text-focus);
  font-size: 14px;
}
.chat-list {
  flex: 1;
  overflow-y: auto;
}
.chat-list-item {
  display: flex;
  align-items: center;
  gap: 12px;
  padding: 16px 20px;
  cursor: pointer;
  border-bottom: 1px solid #f1f5f9;
  transition: background-color 0.2s;
}
.chat-list-item:hover {
  background-color: #f8fafc;
}
.chat-list-item.active {
  background-color: #e2e8f0;
}
.chat-avatar {
  width: 48px;
  height: 48px;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  font-weight: 600;
  font-size: 18px;
  flex-shrink: 0;
}
.student-bg {
  background-color: #dcfce7;
  color: #166534;
}
.company-bg {
  background-color: #e0e7ff;
  color: #3730a3;
}
.chat-preview {
  flex: 1;
  min-width: 0;
  display: flex;
  flex-direction: column;
  gap: 4px;
}
.preview-header {
  display: flex;
  justify-content: space-between;
  align-items: baseline;
}
.chat-name {
  font-weight: 600;
  font-size: 15px;
  color: var(--dark-text);
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}
.chat-time {
  font-size: 12px;
  color: var(--gray-text-focus);
  flex-shrink: 0;
  margin-left: 8px;
}
.preview-footer {
  display: flex;
  justify-content: space-between;
  align-items: center;
  gap: 8px;
}
.chat-last-msg {
  font-size: 13px;
  color: var(--gray-text);
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
  display: flex;
  align-items: center;
  gap: 4px;
}
.text-blocked {
  color: #94a3b8;
  font-style: italic;
}
.icon-tiny {
  width: 12px;
  height: 12px;
}
.unread-dot {
  width: 10px;
  height: 10px;
  background-color: var(--susu-blue, #2563eb);
  border-radius: 50%;
  display: inline-block;
  flex-shrink: 0;
}
@media (max-width: 768px) {
  .chat-sidebar.hidden-on-mobile {
    display: none !important;
  }
  .chat-sidebar {
    width: 100%;
    border-right: none;
  }
}
</style>
