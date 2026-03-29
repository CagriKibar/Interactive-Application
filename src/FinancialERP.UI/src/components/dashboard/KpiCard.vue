<template>
  <div class="kpi-card glass-card">
    <div class="kpi-icon">{{ icon }}</div>
    <div class="kpi-value" :class="{ pulse: critical }">{{ formattedValue }}</div>
    <div class="kpi-label">{{ label }}</div>
    <div v-if="change !== undefined" class="kpi-change" :class="changeDirection">
      {{ changeDirection === 'up' ? '↑' : '↓' }} %{{ Math.abs(change).toFixed(1) }}
    </div>
  </div>
</template>

<script setup>
import { computed } from 'vue'
const props = defineProps({
  icon: String,
  label: String,
  value: Number,
  change: Number,
  prefix: { type: String, default: '' },
  suffix: { type: String, default: '' },
  critical: { type: Boolean, default: false },
})
const formattedValue = computed(() =>
  `${props.prefix}${(props.value || 0).toLocaleString('tr-TR', { minimumFractionDigits: 0, maximumFractionDigits: 2 })}${props.suffix}`
)
const changeDirection = computed(() => (props.change >= 0 ? 'up' : 'down'))
</script>

<style scoped>
.kpi-card {
  padding: 20px;
  display: flex;
  flex-direction: column;
  gap: 6px;
  animation: countUp var(--duration-normal) var(--ease-apple) both;
}
.kpi-icon { font-size: 28px; }
.kpi-value { font-size: 28px; font-weight: 700; letter-spacing: -0.5px; }
.kpi-value.pulse { animation: pulse 2s ease-in-out infinite; }
.kpi-label { font-size: 13px; color: var(--text-secondary); }
.kpi-change {
  font-size: 12px;
  font-weight: 600;
  padding: 2px 8px;
  border-radius: 4px;
  width: fit-content;
}
.kpi-change.up { color: var(--color-green); background: rgba(52, 199, 89, 0.1); }
.kpi-change.down { color: var(--color-red); background: rgba(255, 59, 48, 0.1); }
</style>
