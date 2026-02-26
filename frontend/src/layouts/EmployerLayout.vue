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
  <div class="employer-layout-wrapper">
    <EmployerHero ref="heroRef" />

    <div class="employer-search-bar">
      <div class="container" style="margin-top: 16px">
        <SearchBar
          placeholder="Навык, профессия, ВУЗ или имя студента"
          @search="handleResumeSearch"
        />
      </div>
    </div>

    <main class="employer-content">
      <div class="container">
        <router-view @update-hero="refreshHero" />
      </div>
    </main>
  </div>
</template>

<style scoped>
.employer-layout-wrapper {
  display: flex;
  flex-direction: column;
  min-height: 100vh;
}

.employer-content {
  padding: 24px 0 60px 0;
  flex-grow: 1;
}

.employer-search-bar {
  padding: 16px 0;
}
</style>
