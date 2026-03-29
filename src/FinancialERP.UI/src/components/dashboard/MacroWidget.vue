<template>
  <div class="macro-widget glass-card">
    <h3 class="widget-title">Döviz & Enflasyon</h3>
    <div class="rates-row">
      <div class="rate-item">
        <span class="rate-label">USD/TRY</span>
        <span class="rate-value">{{ usdRate.toFixed(4) }}</span>
        <span class="rate-change" :class="usdChange >= 0 ? 'up' : 'down'">
          {{ usdChange >= 0 ? '▲' : '▼' }} %{{ Math.abs(usdChange).toFixed(2) }}
        </span>
      </div>
      <div class="rate-item">
        <span class="rate-label">EUR/TRY</span>
        <span class="rate-value">{{ eurRate.toFixed(4) }}</span>
        <span class="rate-change" :class="eurChange >= 0 ? 'up' : 'down'">
          {{ eurChange >= 0 ? '▲' : '▼' }} %{{ Math.abs(eurChange).toFixed(2) }}
        </span>
      </div>
    </div>
    <div class="inflation-row">
      <div class="inflation-item">
        <span class="inf-label">Aylık Enflasyon</span>
        <span class="inf-value">%{{ monthlyInflation.toFixed(1) }}</span>
      </div>
      <div class="inflation-item">
        <span class="inf-label">Yıllık Enflasyon</span>
        <span class="inf-value">%{{ annualInflation.toFixed(1) }}</span>
      </div>
    </div>
  </div>
</template>

<script setup>
defineProps({
  usdRate: { type: Number, default: 0 },
  eurRate: { type: Number, default: 0 },
  usdChange: { type: Number, default: 0 },
  eurChange: { type: Number, default: 0 },
  monthlyInflation: { type: Number, default: 0 },
  annualInflation: { type: Number, default: 0 },
})
</script>

<style scoped>
.macro-widget { padding: 20px; }
.widget-title { font-size: 16px; font-weight: 600; margin-bottom: 16px; }
.rates-row { display: flex; gap: 20px; margin-bottom: 16px; }
.rate-item { flex: 1; }
.rate-label { font-size: 12px; color: var(--text-secondary); display: block; margin-bottom: 4px; }
.rate-value { font-size: 22px; font-weight: 700; display: block; }
.rate-change { font-size: 12px; font-weight: 600; }
.rate-change.up { color: var(--color-red); }
.rate-change.down { color: var(--color-green); }
.inflation-row { display: flex; gap: 20px; padding-top: 16px; border-top: 1px solid var(--border-color); }
.inflation-item { flex: 1; }
.inf-label { font-size: 12px; color: var(--text-secondary); display: block; margin-bottom: 4px; }
.inf-value { font-size: 18px; font-weight: 600; }
</style>
