<script setup lang="ts">
import { ref, computed } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import EmployerHero from '@/components/EmployerHero.vue'
import SearchBar from '@/components/SearchBar.vue'

const router = useRouter()
const route = useRoute()
const heroRef = ref<InstanceType<typeof EmployerHero> | null>(null)

const isSearchPage = computed(() => route.name === 'employer-resume-search')

const handleResumeSearch = (query: string) => {
  if (!query.trim()) return

  router.push({
    name: 'employer-resume-search',
    query: { q: query },
  })
}

const handleAdvancedSearch = () => {
  router.push({
    name: 'employer-resume-search',
  })
}

const refreshHero = () => {
  if (heroRef.value) {
    heroRef.value.loadHeroData()
  }
}
</script>

<template>
  <div class="layout-wrapper">
    <template v-if="!isSearchPage">
      <EmployerHero ref="heroRef" />

      <div class="search-bar-wrapper">
        <div class="container">
          <SearchBar
            placeholder="Навык, профессия, ВУЗ или имя студента"
            @search="handleResumeSearch"
            @advanced-search="handleAdvancedSearch"
          />
        </div>
      </div>
    </template>

    <main class="layout-content">
      <router-view @update-hero="refreshHero" />
    </main>
  </div>
</template>
