import * as signalR from '@microsoft/signalr'

export interface MessageDto {
  id: string
  senderId: string | null
  content: string
  sentAt: string
  invitationId: string | null
  isRead: boolean
}

class SignalRService {
  private connection: signalR.HubConnection | null = null

  public async startConnection(token: string) {
    if (this.connection?.state === signalR.HubConnectionState.Connected) {
      return
    }
    const backendBaseUrl = 'http://localhost:5000'

    this.connection = new signalR.HubConnectionBuilder()
      .withUrl(`${backendBaseUrl}/hubs/chat`, {
        accessTokenFactory: () => token,
      })
      .withAutomaticReconnect()
      .build()

    try {
      await this.connection.start()
      console.log('SignalR: Успешно подключено к чату!')
    } catch (err) {
      console.error('SignalR: Ошибка подключения', err)
    }
  }

  public stopConnection() {
    if (this.connection) {
      this.connection.stop()
      console.log('SignalR: Отключено')
    }
  }

  public onReceiveMessage(callback: (message: MessageDto) => void) {
    if (this.connection) {
      this.connection.off('ReceiveMessage')
      this.connection.on('ReceiveMessage', callback)
    }
  }

  public async joinChat(chatId: string) {
    if (this.connection?.state === signalR.HubConnectionState.Connected) {
      try {
        await this.connection.invoke('JoinChat', chatId)
        console.log(`SignalR: Вошли в чат ${chatId}`)
      } catch (err) {
        console.error('SignalR: Ошибка входа в чат', err)
      }
    }
  }

  public async leaveChat(chatId: string) {
    if (this.connection?.state === signalR.HubConnectionState.Connected) {
      try {
        await this.connection.invoke('LeaveChat', chatId)
        console.log(`SignalR: Покинули чат ${chatId}`)
      } catch (err) {
        console.error('SignalR: Ошибка выхода из чата', err)
      }
    }
  }
}

export const signalrService = new SignalRService()
