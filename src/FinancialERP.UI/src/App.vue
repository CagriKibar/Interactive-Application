<template>
  <div class="app-layout">
    <!-- macOS Titlebar Spacer -->
    <div class="titlebar-spacer"></div>

    <!-- Sidebar -->
    <nav class="sidebar glass-card">
      <div class="sidebar-logo">
        <span class="logo-icon">📊</span>
        <span class="logo-text">ERP AI</span>
      </div>

      <div class="nav-items">
        <router-link to="/" class="nav-item" active-class="active">
          <span class="nav-icon">🏠</span>
          <span>Dashboard</span>
        </router-link>
        <router-link to="/finance" class="nav-item" active-class="active">
          <span class="nav-icon">💰</span>
          <span>Finans</span>
        </router-link>
        <router-link to="/analysis" class="nav-item" active-class="active">
          <span class="nav-icon">📈</span>
          <span>Analiz</span>
        </router-link>
        <router-link to="/settings" class="nav-item" active-class="active">
          <span class="nav-icon">⚙️</span>
          <span>Ayarlar</span>
        </router-link>
      </div>
    </nav>

    <!-- Main Content -->
    <main class="main-content">
      <header class="top-bar">
        <h1 class="page-title">{{ $route.name }}</h1>
        <button class="notification-bell" @click="showNotifications = !showNotifications">
          🔔
          <span v-if="unreadCount > 0" class="badge">{{ unreadCount }}</span>
        </button>
      </header>

      <router-view v-slot="{ Component }">
        <transition name="page" mode="out-in">
          <component :is="Component" />
        </transition>
      </router-view>
    </main>

    <!-- Notification Panel -->
    <NotificationPanel
      v-if="showNotifications"
      @close="showNotifications = false"
    />
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import { useDashboardStore } from './store/dashboard'
import NotificationPanel from './components/notifications/NotificationPanel.vue'

const store = useDashboardStore()
const showNotifications = ref(false)
const unreadCount = computed(() => store.unreadCount)

onMounted(() => { store.fetchDashboard() })
</script>

<style scoped>
.app-layout {
  display: flex;
  height: 100vh;
  overflow: hidden;
}
.titlebar-spacer {
  position: fixed;
  top: 0; left: 0; right: 0;
  height: 38px;
  -webkit-app-region: drag;
  z-index: 100;
}
.sidebar {
  width: 220px;
  padding: 52px 12px 12px;
  display: flex;
  flex-direction: column;
  gap: 8px;
  border-radius: 0;
  border-right: var(--glass-border);
  flex-shrink: 0;
}
.sidebar-logo {
  display: flex;
  align-items: center;
  gap: 8px;
  padding: 8px 12px;
  margin-bottom: 16px;
}
.logo-icon { font-size: 24px; }
.logo-text { font-size: 18px; font-weight: 700; }
.nav-items { display: flex; flex-direction: column; gap: 4px; }
.nav-item {
  display: flex;
  align-items: center;
  gap: 10px;
  padding: 10px 12px;
  border-radius: var(--radius-sm);
  color: var(--text-secondary);
  text-decoration: none;
  font-size: 14px;
  font-weight: 500;
  transition: all var(--duration-fast) var(--ease-apple);
}
.nav-item:hover { background: var(--border-color); color: var(--text-primary); }
.nav-item.active {
  background: var(--color-blue);
  color: #fff;
  box-shadow: 0 2px 8px rgba(0, 122, 255, 0.3);
}
.nav-icon { font-size: 18px; }

.main-content {
  flex: 1;
  padding: 52px 24px 24px;
  overflow-y: auto;
}
.top-bar {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 24px;
}
.page-title { font-size: 28px; font-weight: 700; }
.notification-bell {
  position: relative;
  background: none;
  border: none;
  font-size: 22px;
  cursor: pointer;
  padding: 8px;
  border-radius: var(--radius-sm);
  transition: background var(--duration-fast) var(--ease-apple);
}
.notification-bell:hover { background: var(--border-color); }
.badge {
  position: absolute;
  top: 2px; right: 2px;
  background: var(--color-red);
  color: #fff;
  font-size: 10px;
  font-weight: 700;
  min-width: 18px;
  height: 18px;
  border-radius: 9px;
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 0 4px;
}
</style>
