<script setup lang="ts">
import { onMounted, watch } from 'vue'
import { useRoute } from 'vue-router'
import { useToast } from 'vue-toastification'
import AppHeader from '@/components/AppHeader.vue'
import AppDownload from '@/components/AppDownload.vue'
import AppFooter from '@/components/AppFooter.vue'

import { signalrService } from '@/services/signalrService'
import { useNotificationStore } from '@/stores/notifications'

const route = useRoute()
const toast = useToast()
const notificationStore = useNotificationStore()

const initGlobalSockets = async () => {
  const token = localStorage.getItem('token')
  if (!token) return

  await signalrService.startConnection(token)
  await signalrService.startNotificationConnection(token)

  signalrService.onReceiveMessage(() => {
    if (route.name !== 'student-messages' && route.name !== 'employer-messages') {
      notificationStore.incrementMessages()
    }
  })

  signalrService.onReceiveNotification((notif) => {
    const type = notif.type || ''

    switch (type) {
      case 'InvitationIncome':
        notificationStore.incrementInvitations()
        break

      case 'InvitationStatus':
        notificationStore.incrementStatusUpdates()
        break

      case 'System':
        toast.info(notif.message || 'Системное уведомление')
        break

      default:
        break
    }
  })
}

onMounted(initGlobalSockets)

watch(
  () => route.path,
  () => {
    if (localStorage.getItem('token')) {
      initGlobalSockets()
    } else {
      signalrService.stopConnection()
      signalrService.stopNotificationConnection()
      notificationStore.clearAll()
    }
  },
)
</script>

<template>
  <AppHeader v-if="!$route.meta.hideLayout" />

  <main :class="{ 'auth-centered': $route.meta.hideLayout }">
    <router-view />
  </main>

  <AppDownload v-if="!$route.meta.hideLayout" />
  <AppFooter v-if="!$route.meta.hideLayout" />
</template>

<style>
.auth-centered {
  min-height: 100vh;
  display: flex;
  align-items: center;
  justify-content: center;
}
</style>
