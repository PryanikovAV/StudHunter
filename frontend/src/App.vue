<script setup lang="ts">
import AppHeader from '@/components/AppHeader.vue'
import AppDownload from '@/components/AppDownload.vue'
import AppFooter from '@/components/AppFooter.vue'
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
