export interface NotificationDto {
  id: string
  title: string
  message: string
  isRead: boolean
  createdAt: string
  type: string
  entityId?: string | null
  timeAgo: string
}
