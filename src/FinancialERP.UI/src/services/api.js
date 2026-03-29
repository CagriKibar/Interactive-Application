import axios from 'axios'

const api = axios.create({
  baseURL: 'http://localhost:5000/api',
  timeout: 15000,
  headers: { 'Content-Type': 'application/json' },
})

api.interceptors.response.use(
  (res) => res,
  (err) => {
    console.error('API Hatası:', err.response?.data || err.message)
    return Promise.reject(err)
  }
)

export default api
