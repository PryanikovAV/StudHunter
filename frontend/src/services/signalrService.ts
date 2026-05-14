import * as signalR from '@microsoft/signalr'
import type { MessageDto } from '@/types/chat'
import type { NotificationDto } from '@/types/notification.ts'

class SignalRService {
  private chatConnection: signalR.HubConnection | null = null
  private notifConnection: signalR.HubConnection | null = null

  public async startConnection(token: string) {
    if (this.chatConnection?.state === signalR.HubConnectionState.Connected) return

    this.chatConnection = new signalR.HubConnectionBuilder()
      .withUrl('/hubs/chat', { accessTokenFactory: () => token })
      .withAutomaticReconnect()
      .build()

    try {
      await this.chatConnection.start()
      console.log('SignalR: Успешно подключено к чату!')
    } catch (err) {
      console.error('SignalR: Ошибка подключения к чату', err)
    }
  }

  public stopConnection() {
    this.chatConnection?.stop()
  }

  public onReceiveMessage(callback: (message: MessageDto) => void) {
    if (this.chatConnection) {
      this.chatConnection.on('ReceiveMessage', callback)
    }
  }

  public offReceiveMessage(callback: (message: MessageDto) => void) {
    if (this.chatConnection) {
      this.chatConnection.off('ReceiveMessage', callback)
    }
  }

  public async joinChat(chatId: string) {
    if (this.chatConnection?.state === signalR.HubConnectionState.Connected) {
      await this.chatConnection.invoke('JoinChat', chatId).catch(console.error)
    }
  }

  public async leaveChat(chatId: string) {
    if (this.chatConnection?.state === signalR.HubConnectionState.Connected) {
      await this.chatConnection.invoke('LeaveChat', chatId).catch(console.error)
    }
  }

  public async startNotificationConnection(token: string) {
    if (this.notifConnection?.state === signalR.HubConnectionState.Connected) return

    this.notifConnection = new signalR.HubConnectionBuilder()
      .withUrl('/hubs/notifications', { accessTokenFactory: () => token })
      .withAutomaticReconnect()
      .build()

    try {
      await this.notifConnection.start()
      console.log('SignalR: Успешно подключено к уведомлениям!')
    } catch (err) {
      console.error('SignalR: Ошибка подключения к уведомлениям', err)
    }
  }

  public stopNotificationConnection() {
    if (this.notifConnection) {
      this.notifConnection.stop()
      console.log('SignalR: Отключено от уведомлений')
    }
  }

  public onReceiveNotification(callback: (notification: NotificationDto) => void) {
    if (this.notifConnection) {
      this.notifConnection.off('ReceiveNotification')
      this.notifConnection.on('ReceiveNotification', callback)
    }
  }
}

export const signalrService = new SignalRService()
