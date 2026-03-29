import { createRouter, createWebHashHistory } from 'vue-router'

const routes = [
  { path: '/', name: 'Dashboard', component: () => import('../views/DashboardView.vue') },
  { path: '/finance', name: 'Finance', component: () => import('../views/FinanceView.vue') },
  { path: '/analysis', name: 'Analysis', component: () => import('../views/AnalysisView.vue') },
  { path: '/settings', name: 'Settings', component: () => import('../views/SettingsView.vue') },
]

export default createRouter({
  history: createWebHashHistory(),
  routes,
})
