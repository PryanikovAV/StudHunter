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
  <div class="student-layout-wrapper">
    <StudentHero />

    <div class="student-search-bar">
      <div class="container" style="margin-top: 16px">
        <SearchBar placeholder="Профессия, должность или компания" @search="handleStudentSearch" />
      </div>
    </div>

    <main class="student-content">
      <div class="container">
        <router-view />
      </div>
    </main>
  </div>
</template>

<style scoped>
.student-layout-wrapper {
  display: flex;
  flex-direction: column;
  min-height: 100vh;
}

.student-content {
  padding: 24px 0 60px 0;
  flex-grow: 1;
}

.student-search-bar {
  padding: 16px 0;
}
</style>
