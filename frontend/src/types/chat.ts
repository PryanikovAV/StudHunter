export interface ChatParticipantDto {
  id: string
  displayName: string
  role: string
}

export interface ChatDto {
  id: string
  interlocutor: ChatParticipantDto
  lastMessage: string
  lastMessageAt: string | null
  isBlockedByMe: boolean
  isBlockedByInterlocutor: boolean
  unreadCount?: number
}

export interface MessageDto {
  id: string
  senderId: string | null
  content: string
  sentAt: string
  invitationId: string | null
  isRead: boolean
}
