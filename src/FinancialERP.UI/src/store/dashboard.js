import { defineStore } from 'pinia'
import api from '../services/api'

export const useDashboardStore = defineStore('dashboard', {
  state: () => ({
    dashboard: null,
    notifications: [],
    loading: false,
    error: null,
  }),
  actions: {
    async fetchDashboard() {
      this.loading = true
      try {
        const { data } = await api.get('/dashboard')
        this.dashboard = data
        this.notifications = data.notifications || []
      } catch (err) {
        this.error = err.message
      } finally {
        this.loading = false
      }
    },
    async fetchNotifications() {
      try {
        const { data } = await api.get('/notifications/smart')
        this.notifications = data
      } catch (err) {
        this.error = err.message
      }
    },
    async markAsRead(id) {
      await api.post(`/notifications/${id}/read`)
      this.notifications = this.notifications.filter((n) => n.id !== id)
    },
  },
  getters: {
    unreadCount: (state) => state.notifications.length,
    criticalAlerts: (state) => state.notifications.filter((n) => n.severity === 2),
  },
})
