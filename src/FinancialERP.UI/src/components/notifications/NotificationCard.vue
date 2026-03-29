<template>
  <div class="notification-card glass-card" :class="severityClass" style="animation: slideInRight var(--duration-normal) var(--ease-apple) both">
    <div class="severity-bar"></div>
    <div class="card-content">
      <div class="card-header">
        <span class="severity-icon">{{ severityIcon }}</span>
        <span class="card-title">{{ notification.title }}</span>
      </div>
      <p class="card-message">{{ notification.message }}</p>

      <div v-if="expanded && notification.actionItems?.length" class="action-items">
        <button
          v-for="(action, i) in notification.actionItems"
          :key="i"
          class="action-btn"
        >{{ action }}</button>
      </div>

      <div class="card-footer">
        <span class="timestamp">{{ timeAgo }}</span>
        <div class="card-actions">
          <button class="btn-expand" @click="expanded = !expanded">
            {{ expanded ? 'Kapat' : 'Detay' }}
          </button>
          <button class="btn-dismiss" @click="$emit('dismiss')">Kaldır</button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed } from 'vue'

const props = defineProps({ notification: Object })
defineEmits(['dismiss'])
const expanded = ref(false)

const severityClass = computed(() => {
  const s = props.notification.severity
  if (s === 2) return 'critical'
  if (s === 1) return 'warning'
  if (s === 3) return 'opportunity'
  return 'info'
})

const severityIcon = computed(() => {
  const s = props.notification.severity
  if (s === 2) return '🔴'
  if (s === 1) return '🟠'
  if (s === 3) return '🟢'
  return '🔵'
})

const timeAgo = computed(() => {
  const diff = Date.now() - new Date(props.notification.generatedAt || Date.now()).getTime()
  const min = Math.floor(diff / 60000)
  if (min < 1) return 'Şimdi'
  if (min < 60) return `${min} dakika önce`
  const hr = Math.floor(min / 60)
  if (hr < 24) return `${hr} saat önce`
  return `${Math.floor(hr / 24)} gün önce`
})
</script>

<style scoped>
.notification-card {
  display: flex;
  overflow: hidden;
  cursor: pointer;
  transition: transform var(--duration-fast) var(--ease-apple);
}
.notification-card:hover { transform: translateY(-2px); }
.severity-bar { width: 4px; flex-shrink: 0; border-radius: 4px 0 0 4px; }
.critical .severity-bar { background: var(--color-red); }
.warning .severity-bar { background: var(--color-orange); }
.opportunity .severity-bar { background: var(--color-green); }
.info .severity-bar { background: var(--color-blue); }
.card-content { padding: 12px; flex: 1; }
.card-header { display: flex; align-items: center; gap: 8px; margin-bottom: 6px; }
.severity-icon { font-size: 14px; }
.card-title { font-size: 13px; font-weight: 600; }
.card-message { font-size: 12px; color: var(--text-secondary); line-height: 1.5; }
.action-items {
  display: flex;
  flex-wrap: wrap;
  gap: 6px;
  margin-top: 10px;
  animation: fadeIn var(--duration-fast) var(--ease-apple);
}
.action-btn {
  padding: 4px 10px;
  font-size: 11px;
  border: 1px solid var(--color-blue);
  color: var(--color-blue);
  background: transparent;
  border-radius: var(--radius-sm);
  cursor: pointer;
  transition: all var(--duration-fast);
}
.action-btn:hover { background: var(--color-blue); color: #fff; }
.card-footer {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-top: 10px;
}
.timestamp { font-size: 11px; color: var(--gray-400); }
.card-actions { display: flex; gap: 6px; }
.btn-expand, .btn-dismiss {
  font-size: 11px;
  padding: 3px 8px;
  border: none;
  border-radius: 4px;
  cursor: pointer;
  background: var(--gray-100);
  color: var(--text-secondary);
}
.btn-expand:hover { background: var(--color-blue); color: #fff; }
.btn-dismiss:hover { background: var(--color-red); color: #fff; }
</style>
