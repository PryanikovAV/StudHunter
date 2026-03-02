<script setup lang="ts">
import { ref, computed, onMounted, onUnmounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { signalrService } from '@/services/signalrService'
import AppCard from '@/components/AppCard.vue'
import apiClient from '@/api'
import type { ChatDto, MessageDto, ChatParticipantDto } from '@/types/chat'
import ChatSidebar from '@/components/chat/ChatSidebar.vue'
import ChatWindow from '@/components/chat/ChatWindow.vue'

const route = useRoute()
const router = useRouter()

const chats = ref<ChatDto[]>([])
const messages = ref<MessageDto[]>([])
const selectedChatId = ref<string | null>(null)
const isLoadingChats = ref(true)
const activeInvitationId = ref<string | null>(null)

const currentChat = computed(() => chats.value.find((c) => c.id === selectedChatId.value))

const currentUserId = computed(() => {
  const token = localStorage.getItem('token')
  if (!token) return null
  try {
    const parts = token.split('.')
    if (parts.length >= 2 && parts[1]) {
      const payload = JSON.parse(atob(parts[1]))
      return (
        payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'] ||
        payload.sub ||
        payload.nameid ||
        null
      )
    }
  } catch (e) {
    console.error('Ошибка расшифровки токена', e)
  }
  return null
})

const fetchChats = async (isBackground = false) => {
  try {
    if (!isBackground) isLoadingChats.value = true
    const response = await apiClient.get('/chats')
    chats.value = response.data.items || response.data
  } catch (error) {
    console.error('Ошибка загрузки списка чатов:', error)
  } finally {
    if (!isBackground) isLoadingChats.value = false
  }
}

const fetchMessages = async (chatId: string) => {
  if (chatId.startsWith('draft_')) {
    messages.value = []
    return
  }
  try {
    const response = await apiClient.get(`/chats/${chatId}/messages?PageSize=50`)
    const msgs = response.data.items || response.data
    messages.value = msgs.reverse()
  } catch (error) {
    console.error('Ошибка загрузки сообщений:', error)
  }
}

const handleSelectChat = async (id: string) => {
  if (selectedChatId.value && !selectedChatId.value.startsWith('draft_')) {
    await signalrService.leaveChat(selectedChatId.value)
  }
  selectedChatId.value = id
  await fetchMessages(id)

  if (!id.startsWith('draft_')) {
    await signalrService.joinChat(id)
    try {
      await apiClient.patch(`/chats/${id}/read`)
      const chat = chats.value.find((c) => c.id === id)
      if (chat) chat.unreadCount = 0
    } catch (error) {
      console.error('Не удалось отметить сообщения', error)
    }
  }
}

const handleSendMessage = async (content: string) => {
  if (!currentChat.value) return
  const receiverId = currentChat.value.interlocutor.id

  try {
    await apiClient.post('/chats/send', {
      receiverId,
      content,
      invitationId: activeInvitationId.value,
    })
    activeInvitationId.value = null
    await fetchChats(true)
    const updatedChat = chats.value.find((c) => c.interlocutor.id === receiverId)
    if (updatedChat) await handleSelectChat(updatedChat.id)
  } catch (error) {
    console.error('Ошибка отправки:', error)
    window.alert('Не удалось отправить сообщение.')
  }
}

const initChatFromUrl = async () => {
  const queryReceiverId = route.query.receiverId as string
  const queryInvitationId = route.query.invitationId as string

  if (queryInvitationId) activeInvitationId.value = queryInvitationId

  if (queryReceiverId) {
    const existingChat = chats.value.find((c) => c.interlocutor.id === queryReceiverId)
    if (existingChat) {
      await handleSelectChat(existingChat.id)
    } else {
      try {
        const res = await apiClient.get<ChatParticipantDto>(
          `/chats/interlocutor/${queryReceiverId}`,
        )
        const draftChat: ChatDto = {
          id: `draft_${queryReceiverId}`,
          interlocutor: res.data,
          lastMessage: 'Новый диалог',
          lastMessageAt: new Date().toISOString(),
          isBlockedByMe: false,
          isBlockedByInterlocutor: false,
          unreadCount: 0,
        }
        chats.value.unshift(draftChat)
        await handleSelectChat(draftChat.id)
      } catch (error) {
        console.error('Не удалось получить данные собеседника:', error)
      }
    }
    router.replace({ path: route.path })
  }
}

onMounted(async () => {
  await fetchChats()
  await initChatFromUrl()

  const token = localStorage.getItem('token')
  if (token) {
    await signalrService.startConnection(token)

    signalrService.onReceiveMessage((newMessage) => {
      const isMyMsg = newMessage.senderId?.toLowerCase() === currentUserId.value?.toLowerCase()
      const targetChat = chats.value.find((c) => c.interlocutor.id === newMessage.senderId)
      const isCurrentChatOpen =
        currentChat.value && (newMessage.senderId === currentChat.value.interlocutor.id || isMyMsg)

      if (isCurrentChatOpen) {
        messages.value.push(newMessage)
        if (!isMyMsg) {
          apiClient.patch(`/chats/${currentChat.value?.id}/read`).catch((e) => console.error(e))
        }
      } else {
        if (targetChat && !isMyMsg) {
          targetChat.unreadCount = (targetChat.unreadCount || 0) + 1
        }
      }
      if (targetChat) {
        if (!isMyMsg) {
          targetChat.lastMessage = newMessage.content
          targetChat.lastMessageAt = newMessage.sentAt
        }
        chats.value = [targetChat, ...chats.value.filter((c) => c.id !== targetChat.id)]
      }
      setTimeout(() => {
        fetchChats(true)
      }, 500)
    })
  }
})

onUnmounted(() => {
  signalrService.stopConnection()
})
</script>

<template>
  <div class="chat-page-root">
    <h1 class="page-title">Мессенджер</h1>

    <AppCard class="chat-main-card">
      <ChatSidebar
        :chats="chats"
        :selectedChatId="selectedChatId"
        :isLoading="isLoadingChats"
        @select="handleSelectChat"
      />
      <ChatWindow
        :chat="currentChat"
        :messages="messages"
        :currentUserId="currentUserId"
        @send="handleSendMessage"
        @back="selectedChatId = null"
      />
    </AppCard>
  </div>
</template>

<style scoped>
.chat-page-root {
  display: flex;
  flex-direction: column;
  height: calc(100vh - 350px);
  min-height: 500px;
  max-width: 900px;
  width: 100%;
  margin: 0 auto;
}

.page-title {
  margin-bottom: 16px;
  flex-shrink: 0;
}

.chat-main-card {
  flex: 1;
  display: flex !important;
  flex-direction: row !important;
  padding: 0 !important;
  overflow: hidden;
  min-height: 0;
  width: 100%;
}
</style>
