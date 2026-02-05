<script setup lang="ts">
import { ref } from 'vue'
import IconTranslate from '@/components/icons/IconTranslate.vue'

const activeRole = ref('students')

const setRole = (role: string) => {
  activeRole.value = role
}
</script>

<template>
  <header class="header-outer">
    <div class="container header-inner">
      <div class="left-group">
        <button class="btn-text btn-text-blue">Челябинск</button>
        <button class="btn-text btn-text-blue">ЮУрГУ</button>
      </div>

      <div class="right-group">
        <nav class="role-nav">
          <button
            class="btn-text"
            :class="activeRole === 'students' ? 'btn-text-blue' : 'btn-text-gray'"
            @click="setRole('students')"
          >
            Студентам
          </button>

          <button
            class="btn-text"
            :class="activeRole === 'employers' ? 'btn-text-blue' : 'btn-text-gray'"
            @click="setRole('employers')"
          >
            Работодателям
          </button>
        </nav>
        <button class="btn-text btn-text-blue btn-lang">
          <IconTranslate class="icon-translate" />
          <span>Русский</span>
        </button>
      </div>
    </div>
  </header>
</template>

<style scoped>
.header-outer {
  width: 100%;
  height: 48px;
  background-color: #ffffff;
  border-bottom: 1px solid #e2e8f0;
  display: flex;
  align-items: center;
  user-select: none;
  -webkit-user-select: none;
}

.header-inner {
  display: flex;
  justify-content: space-between;
  align-items: center;
  width: 100%;
}

.left-group,
.right-group {
  display: flex;
  gap: 20px;
  align-items: center;
}

.role-nav {
  display: flex;
  gap: 20px;
}

.btn-lang {
  gap: 2px;
  font-weight: 400; /* Язык обычно не жирный */
}

.icon-translate {
  width: 18px;
  height: 18px;
  flex-shrink: 0;
}
</style>
