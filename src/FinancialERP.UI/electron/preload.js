const { contextBridge, ipcRenderer } = require('electron')

contextBridge.exposeInMainWorld('electronAPI', {
  onThemeChanged: (callback) => ipcRenderer.on('theme-changed', (_, isDark) => callback(isDark)),
})
