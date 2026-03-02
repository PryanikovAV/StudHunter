<script setup lang="ts">
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import EmployerHero from '@/components/EmployerHero.vue'
import SearchBar from '@/components/SearchBar.vue'

const router = useRouter()
const heroRef = ref<InstanceType<typeof EmployerHero> | null>(null)

const handleResumeSearch = (query: string) => {
  if (!query.trim()) return

  router.push({
    name: 'employer-resume-search',
    query: { q: query },
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
    <EmployerHero ref="heroRef" />

    <div class="search-bar-wrapper">
      <div class="container" style="margin-top: 16px">
        <SearchBar
          placeholder="Навык, профессия, ВУЗ или имя студента"
          @search="handleResumeSearch"
        />
      </div>
    </div>

    <main class="layout-content">
      <router-view @update-hero="refreshHero" />
    </main>
  </div>
</template>
