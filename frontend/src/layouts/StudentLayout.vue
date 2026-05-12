<script setup lang="ts">
import { computed } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import StudentHero from '@/components/StudentHero.vue'
import SearchBar from '@/components/SearchBar.vue'

const router = useRouter()
const route = useRoute()

const isSearchPage = computed(() => route.name === 'student-vacancy-search')

const handleStudentSearch = (query: string) => {
  if (!query.trim()) return

  router.push({
    name: 'student-vacancy-search',
    query: { q: query },
  })
}

const handleAdvancedSearch = () => {
  router.push({
    name: 'student-vacancy-search',
  })
}
</script>

<template>
  <div class="layout-wrapper">
    <template v-if="!isSearchPage">
      <StudentHero />

      <div class="search-bar-wrapper">
        <div class="container">
          <SearchBar
            placeholder="Профессия, должность или компания"
            @search="handleStudentSearch"
            @advanced-search="handleAdvancedSearch"
          />
        </div>
      </div>
    </template>

    <main class="layout-content">
      <router-view />
    </main>
  </div>
</template>
