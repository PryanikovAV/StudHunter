<script setup lang="ts">
import { ref, watch, nextTick } from 'vue'
import type { ChatDto, MessageDto } from '@/types/chat'
import { formatChatTime } from '@/utils/dateUtils'
import IconSend from '@/components/icons/IconSend.vue'
import IconCheck from '@/components/icons/IconCheck.vue'
import IconCheckDouble from '@/components/icons/IconCheckDouble.vue'
import IconBackButton from '@/components/icons/IconBackButton.vue'
import InteractionButtons from '@/components/InteractionButtons.vue'

const props = defineProps<{
  chat?: ChatDto
  messages: MessageDto[]
  currentUserId: string | null
}>()

const emit = defineEmits<{
  (e: 'back'): void
  (e: 'send', content: string): void
}>()

const newMessage = ref('')
const messagesContainer = ref<HTMLElement | null>(null)

const isMyMessage = (senderId: string | null) => {
  if (!senderId || !props.currentUserId) return false
  return senderId.toLowerCase() === props.currentUserId.toLowerCase()
}

const getInitials = (name: string) => (name ? name.charAt(0).toUpperCase() : '?')

const handleSend = () => {
  if (!newMessage.value.trim()) return
  emit('send', newMessage.value)
  newMessage.value = ''
}

const handleUnblock = () => {
  window.alert('В разработке')
}

watch(
  () => props.messages,
  async () => {
    await nextTick()
    if (messagesContainer.value) {
      messagesContainer.value.scrollTop = messagesContainer.value.scrollHeight
    }
  },
  { deep: true },
)
</script>

<template>
  <div class="chat-window" :class="{ 'hidden-on-mobile': !chat }">
    <div v-if="!chat" class="chat-empty-state">
      <div class="empty-icon-wrapper"><span>💬</span></div>
      <p>Выберите диалог слева, чтобы начать общение</p>
    </div>

    <div v-else class="chat-active-area">
      <div class="chat-window-header">
        <button class="mobile-back-btn" @click="$emit('back')">
          <IconBackButton style="width: 24px; height: 24px" />
        </button>

        <div class="header-info">
          <div
            class="chat-avatar small"
            :class="chat.interlocutor.role === 'Student' ? 'student-bg' : 'company-bg'"
          >
            {{ getInitials(chat.interlocutor.displayName) }}
          </div>
          <div class="header-text">
            <h3 class="header-name">{{ chat.interlocutor.displayName }}</h3>
            <span class="header-role">{{
              chat.interlocutor.role === 'Student' ? 'Кандидат' : 'Компания'
            }}</span>
          </div>
        </div>

        <InteractionButtons
          :target-id="chat.interlocutor.id"
          :favorite-type="chat.interlocutor.role === 'Student' ? 'Student' : 'Employer'"
          :initial-is-blocked="chat.isBlockedByMe"
        />
      </div>

      <div class="messages-area" ref="messagesContainer">
        <div v-if="messages.length === 0" class="sidebar-message">
          Здесь пока нет сообщений. Напишите первыми!
        </div>
        <div
          v-else
          v-for="msg in messages"
          :key="msg.id"
          class="message-wrapper"
          :class="{ mine: isMyMessage(msg.senderId) }"
        >
          <div class="message-bubble">
            <p class="message-text">{{ msg.content }}</p>
            <div class="message-meta">
              <span class="msg-time">{{ formatChatTime(msg.sentAt) }}</span>
              <span v-if="isMyMessage(msg.senderId)" class="msg-status">
                <IconCheckDouble v-if="msg.isRead" class="icon-tick double-tick" />
                <IconCheck v-else class="icon-tick" />
              </span>
            </div>
          </div>
        </div>
      </div>

      <div class="chat-input-area">
        <div v-if="chat.isBlockedByMe" class="blocked-notice">
          Вы заблокировали этого пользователя.
          <button class="btn-text" @click="handleUnblock">Разблокировать</button>
        </div>
        <div v-else-if="chat.isBlockedByInterlocutor" class="blocked-notice">
          Вы не можете отправлять сообщения в этот чат.
        </div>
        <form v-else class="input-form" @submit.prevent="handleSend">
          <textarea
            v-model="newMessage"
            class="input-main chat-textarea"
            placeholder="Написать сообщение..."
            rows="1"
            @keydown.enter.prevent="handleSend"
          ></textarea>
          <button type="submit" class="btn-send" :disabled="!newMessage.trim()">
            <IconSend class="icon-send-svg" />
          </button>
        </form>
      </div>
    </div>
  </div>
</template>

<style scoped>
.chat-window {
  flex: 1;
  display: flex;
  flex-direction: column;
  background-color: #ffffff;
  min-width: 0;
}
.chat-empty-state {
  flex: 1;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  color: var(--gray-text-focus);
}
.empty-icon-wrapper {
  font-size: 48px;
  margin-bottom: 16px;
  opacity: 0.5;
}
.chat-active-area {
  flex: 1;
  display: flex;
  flex-direction: column;
  height: 100%;
  min-height: 0;
}
.chat-window-header {
  height: 72px;
  padding: 0 24px;
  border-bottom: 1px solid var(--gray-border);
  display: flex;
  justify-content: space-between;
  align-items: center;
  background-color: var(--background-page);
  flex-shrink: 0;
}
.mobile-back-btn {
  display: none;
  background: none;
  border: none;
  padding: 0;
  margin-right: 16px;
  cursor: pointer;
}
.header-info {
  display: flex;
  align-items: center;
  gap: 12px;
}
.header-text {
  display: flex;
  flex-direction: column;
}
.header-name {
  margin: 0;
  font-size: 16px;
  font-weight: 600;
  color: var(--dark-text);
}
.header-role {
  font-size: 12px;
  color: var(--gray-text-focus);
}
.chat-avatar {
  width: 40px;
  height: 40px;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  font-weight: 600;
  font-size: 16px;
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
.messages-area {
  flex: 1;
  padding: 24px;
  overflow-y: auto;
  display: flex;
  flex-direction: column;
  gap: 16px;
  background-color: #fafafa;
}
.sidebar-message {
  text-align: center;
  color: var(--gray-text-focus);
  font-size: 14px;
  margin-top: 20px;
}
.message-wrapper {
  display: flex;
  width: 100%;
}
.message-wrapper.mine {
  justify-content: flex-end;
}
.message-bubble {
  max-width: 70%;
  padding: 10px 14px;
  border-radius: 12px;
  background-color: #f1f5f9;
  color: var(--dark-text);
  border-bottom-left-radius: 2px;
  display: flex;
  flex-direction: column;
  gap: 4px;
}
.message-wrapper.mine .message-bubble {
  background-color: var(--dark-text, #111827);
  color: #ffffff;
  border-bottom-left-radius: 12px;
  border-bottom-right-radius: 2px;
}
.message-text {
  margin: 0;
  font-size: 15px;
  line-height: 1.4;
  white-space: pre-wrap;
}
.message-meta {
  display: flex;
  justify-content: flex-end;
  align-items: center;
  gap: 4px;
  font-size: 11px;
  opacity: 0.7;
}
.icon-tick {
  width: 14px;
  height: 14px;
}
.chat-input-area {
  padding: 16px 24px;
  border-top: 1px solid var(--gray-border);
  background-color: #ffffff;
  flex-shrink: 0;
}
.input-form {
  display: flex;
  align-items: flex-end;
  gap: 12px;
}
.chat-textarea {
  flex: 1;
  resize: none;
  border-radius: 20px !important;
  padding: 12px 16px !important;
  max-height: 120px;
  min-height: 44px;
  line-height: 1.4;
  overflow-y: auto;
}
.chat-textarea::-webkit-scrollbar {
  width: 6px;
}
.btn-send {
  width: 44px;
  height: 44px;
  border-radius: 50%;
  background-color: var(--dark-text);
  color: white;
  border: none;
  display: flex;
  align-items: center;
  justify-content: center;
  cursor: pointer;
  flex-shrink: 0;
  transition: opacity 0.2s;
}
.btn-send:hover:not(:disabled) {
  opacity: 0.8;
}
.btn-send:disabled {
  background-color: var(--gray-border);
  color: var(--gray-text-focus);
  cursor: not-allowed;
}
.icon-send-svg {
  width: 20px;
  height: 20px;
  transform: translateX(2px);
}
.blocked-notice {
  text-align: center;
  color: var(--gray-text-focus);
  font-size: 14px;
  padding: 12px;
  background-color: #f8fafc;
  border-radius: 8px;
}
.btn-text {
  background: none;
  border: none;
  color: var(--susu-blue);
  font-weight: 500;
  cursor: pointer;
  padding: 0;
  margin-left: 4px;
}
.btn-text:hover {
  text-decoration: underline;
}
@media (max-width: 768px) {
  .chat-window.hidden-on-mobile {
    display: none !important;
  }
  .mobile-back-btn {
    display: block;
  }
  .chat-window-header {
    padding: 0 16px;
  }
  .messages-area {
    padding: 16px;
  }
  .chat-input-area {
    padding: 12px 16px;
  }
}
</style>
