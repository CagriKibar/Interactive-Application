<template>
  <div class="notification-overlay" @click.self="$emit('close')">
    <div class="notification-panel glass-card">
      <div class="panel-header">
        <h2>Bildirimler</h2>
        <button class="close-btn" @click="$emit('close')">✕</button>
      </div>

      <div class="notification-list">
        <template v-if="notifications.length > 0">
          <NotificationCard
            v-for="(notif, index) in notifications"
            :key="index"
            :notification="notif"
            :style="{ animationDelay: `${index * 0.08}s` }"
            @dismiss="dismissNotification(index)"
          />
        </template>
        <div v-else class="empty-state">
          <span class="empty-icon">✅</span>
          <p>Yeni bildirim yok</p>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { computed } from 'vue'
import { useDashboardStore } from '../../store/dashboard'
import NotificationCard from './NotificationCard.vue'

defineEmits(['close'])
const store = useDashboardStore()
const notifications = computed(() => store.notifications)

const dismissNotification = (index) => {
  store.notifications.splice(index, 1)
}
</script>

<style scoped>
.notification-overlay {
  position: fixed;
  inset: 0;
  background: rgba(0, 0, 0, 0.2);
  z-index: 200;
  display: flex;
  justify-content: flex-end;
}
.notification-panel {
  width: 400px;
  height: 100vh;
  padding: 20px;
  border-radius: 0;
  backdrop-filter: blur(40px) saturate(200%);
  -webkit-backdrop-filter: blur(40px) saturate(200%);
  overflow-y: auto;
  animation: slideInRight var(--duration-normal) var(--ease-apple);
}
.panel-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 12px 0 20px;
  border-bottom: 1px solid var(--border-color);
  margin-bottom: 16px;
}
.panel-header h2 { font-size: 20px; font-weight: 700; }
.close-btn {
  background: var(--gray-100);
  border: none;
  width: 28px;
  height: 28px;
  border-radius: 50%;
  cursor: pointer;
  font-size: 14px;
  display: flex;
  align-items: center;
  justify-content: center;
}
.notification-list { display: flex; flex-direction: column; gap: 10px; }
.empty-state {
  text-align: center;
  padding: 40px 0;
  color: var(--text-secondary);
}
.empty-icon { font-size: 40px; display: block; margin-bottom: 12px; }
</style>
