<script setup lang="ts">
import IconTranslate from '@/components/icons/IconTranslate.vue'
</script>

<template>
  <footer class="app-footer">
    <div class="container">
      <div class="footer-actions">
        <button class="btn-main btn-secondary">
          <IconTranslate class="icon-main" />
          <span>Русский</span>
        </button>
        <button class="btn-main btn-secondary">Задать вопрос</button>
        <button class="btn-main btn-secondary">Сообщить об ошибке</button>
      </div>

      <div class="footer-legal">
        <div class="legal-column">
          <p>
            © 1995–2026 Федеральное государственное автономное образовательное учреждение высшего
            образования «Южно-Уральский государственный университет (национальный исследовательский
            университет)» ФГАОУ ВО «ЮУрГУ (НИУ)» Учредитель — Министерство науки и высшего
            образования Российской Федерации.
            <br /><br />
            При использовании информации ссылка на сайт www.susu.ru обязательна.
          </p>
        </div>
        <div class="legal-column">
          <p>
            Россия, 454080 Челябинск, проспект Ленина, 76 Тел./факс: +7 (351) 267-99-00 E-mail:
            info@susu.ru
            <br /><br />
            Управление маркетинга, брендинга и стратегических коммуникаций: press@susu.ru
            Техподдержка сайта: support@susu.ru
          </p>
        </div>
      </div>
    </div>
  </footer>
</template>

<style scoped>
.app-footer {
  padding: 40px 0 60px;
}

.footer-actions {
  display: flex;
  flex-direction: column;
  align-items: flex-start; /* Выравнивание по левому краю */
  gap: 4px; /* Расстояние между кнопками */
}

/* Сетка для правового текста */
.footer-legal {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 40px;
}

.legal-column {
  font-size: 12px;
  color: var(--gray-text); /* Серый цвет текста */
  line-height: 1.6;
}

@media (max-width: 768px) {
  .footer-legal {
    grid-template-columns: 1fr;
    gap: 20px;
  }

  .app-footer {
    padding: 30px 0 40px;
  }
}
</style>
