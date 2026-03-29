<template>
  <div class="cashflow-card glass-card">
    <h3>Nakit Akışı Sağlığı</h3>

    <!-- Circular Gauge -->
    <div class="gauge-container">
      <svg viewBox="0 0 120 120" class="gauge">
        <circle cx="60" cy="60" r="50" fill="none" stroke="var(--gray-200)" stroke-width="10" />
        <circle cx="60" cy="60" r="50" fill="none" :stroke="gaugeColor" stroke-width="10"
          stroke-linecap="round" :stroke-dasharray="circumference"
          :stroke-dashoffset="dashOffset" class="gauge-fill" />
      </svg>
      <div class="gauge-label">
        <span class="gauge-score">{{ score }}</span>
        <span class="gauge-text">/ 100</span>
      </div>
    </div>

    <div class="metrics">
      <div class="metric"><span class="label">Banka Bakiyesi</span><span class="value green">{{ formatMoney(bankBalance) }}</span></div>
      <div class="metric"><span class="label">KK Borcu</span><span class="value red">{{ formatMoney(creditCardDebt) }}</span></div>
      <div class="metric"><span class="label">Net Pozisyon</span><span class="value" :class="netPosition >= 0 ? 'green' : 'red'">{{ formatMoney(netPosition) }}</span></div>
      <div class="metric"><span class="label">Reel Değer</span><span class="value">{{ formatMoney(realValue) }}</span></div>
    </div>
  </div>
</template>

<script setup>
import { computed } from 'vue'

const props = defineProps({
  score: { type: Number, default: 0 },
  bankBalance: { type: Number, default: 0 },
  creditCardDebt: { type: Number, default: 0 },
  netPosition: { type: Number, default: 0 },
  realValue: { type: Number, default: 0 },
})

const circumference = 2 * Math.PI * 50
const dashOffset = computed(() => circumference - (props.score / 100) * circumference)
const gaugeColor = computed(() => {
  if (props.score >= 70) return 'var(--color-green)'
  if (props.score >= 40) return 'var(--color-orange)'
  return 'var(--color-red)'
})
const formatMoney = (v) => `${(v || 0).toLocaleString('tr-TR', { minimumFractionDigits: 2 })} ₺`
</script>

<style scoped>
.cashflow-card { padding: 20px; }
h3 { font-size: 16px; font-weight: 600; margin-bottom: 16px; }
.gauge-container { position: relative; width: 120px; height: 120px; margin: 0 auto 20px; }
.gauge { transform: rotate(-90deg); }
.gauge-fill { transition: stroke-dashoffset 1s var(--ease-apple); }
.gauge-label {
  position: absolute;
  inset: 0;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
}
.gauge-score { font-size: 28px; font-weight: 700; }
.gauge-text { font-size: 12px; color: var(--text-secondary); }
.metrics { display: flex; flex-direction: column; gap: 10px; }
.metric { display: flex; justify-content: space-between; }
.label { font-size: 13px; color: var(--text-secondary); }
.value { font-size: 14px; font-weight: 600; }
.green { color: var(--color-green); }
.red { color: var(--color-red); }
</style>
