<template>
  <div class="settings-view">
    <h2>Ayarlar</h2>
    <div class="glass-card section">
      <h3>Veritabanı Bağlantıları</h3>
      <p class="placeholder">PostgreSQL ve Barsoft MSSQL bağlantı ayarları.</p>
    </div>
    <div class="glass-card section">
      <h3>API Entegrasyonları</h3>
      <p class="placeholder">TCMB, TÜİK ve diğer API ayarları.</p>
    </div>
  </div>
</template>

<style scoped>
.settings-view { display: flex; flex-direction: column; gap: 20px; }
h2 { font-size: 24px; font-weight: 700; }
.section { padding: 20px; }
.section h3 { font-size: 16px; font-weight: 600; margin-bottom: 12px; }
.placeholder { color: var(--text-secondary); font-size: 13px; }
</style>
