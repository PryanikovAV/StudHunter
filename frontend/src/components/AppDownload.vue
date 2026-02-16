<template>
  <section class="app-download">
    <div class="container download-container">
      <div class="qr-section">
        <div class="qr-box">
          <img src="@/assets/qr-code.svg" alt="QR Code" class="qr-image" />
        </div>
      </div>

      <div class="text-section">
        <h2 class="download-title">Сканируйте QR-код, чтобы скачать приложение</h2>
        <p class="download-subtitle">
          Свежие вакансии, ответы на отклики и доступ к резюме — всегда у вас под рукой
        </p>
      </div>

      <div class="phone-section">
        <img src="@/assets/phone-mockup.png" alt="App Preview" class="phone-img" />
      </div>
    </div>
  </section>
</template>

<style scoped>
.app-download {
  background-color: rgba(39, 88, 134, 0.1);
  width: 100%;
  margin-top: 100px;
  position: relative;
}

.download-container {
  display: flex;
  align-items: center;
  position: relative;
  min-height: 200px;
}

.qr-section {
  flex-shrink: 0;
}

.qr-box {
  background: var(--background-field);
  padding: 10px;
  border-radius: 12px;
  display: flex;
  width: 160px;
  height: 160px;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.05);
}

.qr-image {
  width: 100%;
  height: 100%;
}

.text-section {
  flex: 1;
  padding: 0 60px;
}

.download-title {
  font-size: 24px;
  font-weight: 700;
  color: var(--dark-text);
  margin-bottom: 8px;
  line-height: 1.2;
}

.download-subtitle {
  font-size: 16px;
  color: #475569;
  max-width: 450px;
  line-height: 1.4;
}

.phone-section {
  flex-shrink: 0;
  display: flex;
  align-items: flex-end;
  height: 100%;
  position: relative;
}

.phone-img {
  height: 280px;
  margin-top: -80px;
  display: block;
}

@media (max-width: 992px) {
  .app-download {
    margin-top: 60px;
  }

  .download-container {
    flex-direction: column;
    padding: 40px 20px 0 20px;
    text-align: center;
  }

  .text-section {
    padding: 30px 0;
  }

  .download-subtitle {
    margin: 0 auto;
  }

  .phone-img {
    margin-top: 0;
    height: 280px;
  }

  .qr-box {
    width: 140px;
    height: 140px;
    margin: 0 auto;
  }
}
</style>
