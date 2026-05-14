import { defineStore } from 'pinia'

export const useNotificationStore = defineStore('notifications', {
  state: () => ({
    unreadMessagesCount: 0,
    newInvitationsCount: 0,
    statusUpdatesCount: 0,
  }),
  getters: {
    hasUnreadMessages: (state) => state.unreadMessagesCount > 0,
    hasAnyInvitationAlert: (state) => state.newInvitationsCount > 0 || state.statusUpdatesCount > 0,
  },
  actions: {
    incrementMessages() {
      this.unreadMessagesCount++
    },
    incrementInvitations() {
      this.newInvitationsCount++
    },
    incrementStatusUpdates() {
      this.statusUpdatesCount++
    },

    clearMessages() {
      this.unreadMessagesCount = 0
    },
    clearInvitations() {
      this.newInvitationsCount = 0
      this.statusUpdatesCount = 0
    },
    clearAll() {
      this.unreadMessagesCount = 0
      this.newInvitationsCount = 0
      this.statusUpdatesCount = 0
    },
  },
})
