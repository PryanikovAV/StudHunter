<template>
  <footer class="app-footer">
    <div class="container">
      <div class="footer-actions">
        <button class="btn-text btn-text-gray footer-action-btn">Задать вопрос</button>
        <button class="btn-text btn-text-gray footer-action-btn">Сообщить об ошибке</button>
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
  background-color: #ffffff; /* Белый фон сайта */
  padding: 40px 0 60px; /* Отступы: сверху 40, снизу 60 */
  border-top: 1px solid #f1f5f9; /* Тонкая линия отделения от контента */
}

/* Группировка кнопок */
.footer-actions {
  display: flex;
  flex-direction: column; /* Одна над другой */
  align-items: flex-start; /* Выравнивание по левому краю */
  gap: 4px; /* Расстояние между кнопками */
  margin-bottom: 24px; /* Отступ до текста */
}

/* Локальная правка для кнопок в футере */
.footer-action-btn {
  padding: 4px 0; /* Делаем их чуть компактнее */
  font-size: 14px; /* Как в кнопке "я ищу сотрудника" */
}

/* Сетка для правового текста */
.footer-legal {
  display: grid;
  grid-template-columns: 1fr 1fr; /* 2 одинаковые колонки */
  gap: 40px; /* Расстояние между колонками */
}

.legal-column p {
  font-size: 12px; /* По ТЗ */
  color: #94a3b8; /* Серый цвет текста */
  line-height: 1.6;
  margin: 0;
}

/* --- АДАПТИВНОСТЬ --- */

@media (max-width: 768px) {
  .footer-legal {
    grid-template-columns: 1fr; /* В один столбец на мобильных */
    gap: 20px;
  }

  .app-footer {
    padding: 30px 0 40px;
  }
}
</style>
