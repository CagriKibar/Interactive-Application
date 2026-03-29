<template>
  <div class="dashboard">
    <!-- KPI Row -->
    <div class="kpi-grid">
      <KpiCard icon="💵" label="Günlük Ciro" :value="d.todayRevenue" prefix="" suffix=" ₺" />
      <KpiCard icon="📈" label="Günlük Kâr" :value="d.todayProfit" suffix=" ₺" />
      <KpiCard icon="💲" label="USD/TRY" :value="d.usdRate" :change="d.usdChangePercent" />
      <KpiCard icon="💶" label="EUR/TRY" :value="d.eurRate" :change="d.eurChangePercent" />
    </div>

    <!-- Middle Row -->
    <div class="middle-grid">
      <MacroWidget
        :usdRate="d.usdRate"
        :eurRate="d.eurRate"
        :usdChange="d.usdChangePercent"
        :eurChange="d.eurChangePercent"
        :monthlyInflation="d.monthlyInflationRate"
        :annualInflation="d.annualInflationRate"
      />
      <CashFlowHealthCard
        :score="cashFlowScore"
        :bankBalance="d.cashFlow?.totalBankBalance"
        :creditCardDebt="d.cashFlow?.totalCreditCardDebt"
        :netPosition="d.cashFlow?.netCashPosition"
        :realValue="d.cashFlow?.inflationAdjustedValue"
      />
    </div>

    <!-- Margin Alerts -->
    <div v-if="d.erodedMargins?.length" class="alerts-section glass-card">
      <h3>⚠️ Kâr Marjı Uyarıları</h3>
      <div class="alert-list">
        <div v-for="m in d.erodedMargins" :key="m.productCode" class="alert-row">
          <span class="product-name">{{ m.productName }}</span>
          <span class="erosion red">-%{{ m.marginChangePercent.toFixed(1) }}</span>
          <span class="suggested">Önerilen: {{ m.suggestedNewPrice.toLocaleString('tr-TR') }} ₺</span>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { computed, onMounted } from 'vue'
import { useDashboardStore } from '../store/dashboard'
import KpiCard from '../components/dashboard/KpiCard.vue'
import MacroWidget from '../components/dashboard/MacroWidget.vue'
import CashFlowHealthCard from '../components/finance/CashFlowHealthCard.vue'

const store = useDashboardStore()
onMounted(() => store.fetchDashboard())

const d = computed(() => store.dashboard || {})
const cashFlowScore = computed(() => {
  const h = d.value.cashFlow?.healthStatus
  if (h === 1) return 85
  if (h === 2) return 60
  if (h === 3) return 35
  return 15
})
</script>

<style scoped>
.dashboard { display: flex; flex-direction: column; gap: 20px; }
.kpi-grid { display: grid; grid-template-columns: repeat(4, 1fr); gap: 16px; }
.middle-grid { display: grid; grid-template-columns: 1fr 1fr; gap: 16px; }
.alerts-section { padding: 20px; }
.alerts-section h3 { font-size: 16px; font-weight: 600; margin-bottom: 12px; }
.alert-list { display: flex; flex-direction: column; gap: 8px; }
.alert-row {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 10px;
  background: var(--gray-50);
  border-radius: var(--radius-sm);
  font-size: 13px;
}
.product-name { font-weight: 600; flex: 1; }
.erosion { font-weight: 700; margin: 0 16px; }
.red { color: var(--color-red); }
.suggested { color: var(--text-secondary); font-size: 12px; }
</style>
