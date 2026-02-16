<template>
  <div class="app-card">
    <slot />
  </div>
</template>

<style scoped>
.app-card {
  background-color: var(--background-field);
  border: 1px solid var(--gray-border);
  border-radius: 16px;
  padding: 24px;
  width: 100%;
  transition:
    transform 0.1s ease,
    box-shadow 0.1s ease;
  display: flex;
  flex-direction: column;
}

.app-card:hover {
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.03);
}
</style>
