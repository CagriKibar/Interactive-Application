<template>
  <div class="analysis-view">
    <h2>Gelişmiş Analiz</h2>
    <div class="analysis-grid">
      <div class="glass-card section">
        <h3>📊 Kâr Marjı Analizi</h3>
        <p class="placeholder">Ürün bazlı kâr marjı erimesi ve döviz etkisi detayları.</p>
      </div>
      <div class="glass-card section">
        <h3>📦 Stok Optimizasyonu</h3>
        <p class="placeholder">En çok satanlar, stok gün sayısı ve sipariş önerileri.</p>
      </div>
    </div>
  </div>
</template>

<style scoped>
.analysis-view { display: flex; flex-direction: column; gap: 20px; }
h2 { font-size: 24px; font-weight: 700; }
.analysis-grid { display: grid; grid-template-columns: 1fr 1fr; gap: 16px; }
.section { padding: 20px; min-height: 250px; }
.section h3 { font-size: 16px; font-weight: 600; margin-bottom: 12px; }
.placeholder { color: var(--text-secondary); font-size: 13px; }
</style>
