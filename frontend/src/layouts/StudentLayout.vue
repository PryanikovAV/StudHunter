<script setup lang="ts">
import { useRouter } from 'vue-router'
import StudentHero from '@/components/StudentHero.vue'
import SearchBar from '@/components/SearchBar.vue'

const router = useRouter()

const handleStudentSearch = (query: string) => {
  if (!query.trim()) return

  router.push({
    name: 'student-search',
    query: { q: query },
  })
}
</script>

<template>
  <div class="layout-wrapper">
    <StudentHero />

    <div class="search-bar-wrapper">
      <div class="container">
        <SearchBar placeholder="Профессия, должность или компания" @search="handleStudentSearch" />
      </div>
    </div>

    <main class="layout-content">
      <router-view />
    </main>
  </div>
</template>
