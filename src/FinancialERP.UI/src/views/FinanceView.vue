<template>
  <div class="finance-view">
    <h2>Finans Yönetimi</h2>
    <div class="finance-grid">
      <div class="glass-card section">
        <h3>💳 Kredi Kartları</h3>
        <p class="placeholder">Kredi kartı listesi ve ekstre takibi burada görünecek.</p>
      </div>
      <div class="glass-card section">
        <h3>🏦 Banka Hesapları</h3>
        <p class="placeholder">Banka bakiyeleri ve faiz bilgileri burada görünecek.</p>
      </div>
      <div class="glass-card section">
        <h3>📋 Sabit Giderler</h3>
        <p class="placeholder">Personel, kira, faturalar ve diğer giderler burada görünecek.</p>
      </div>
    </div>
  </div>
</template>

<style scoped>
.finance-view { display: flex; flex-direction: column; gap: 20px; }
h2 { font-size: 24px; font-weight: 700; }
.finance-grid { display: grid; grid-template-columns: repeat(3, 1fr); gap: 16px; }
.section { padding: 20px; min-height: 200px; }
.section h3 { font-size: 16px; font-weight: 600; margin-bottom: 12px; }
.placeholder { color: var(--text-secondary); font-size: 13px; }
</style>
