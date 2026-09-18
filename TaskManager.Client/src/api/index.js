import axios from 'axios'

const api = axios.create({
  baseURL: 'http://localhost:7000/api',
})

api.interceptors.request.use((config) => {
  const token = localStorage.getItem('token')
  if (token) {
    config.headers.Authorization = `Bearer ${token}`
  }
  return config
})

api.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response?.status === 401) {
      localStorage.removeItem('token')
      localStorage.removeItem('user')
    }
    return Promise.reject(error)
  },
)

export const authApi = {
  register: (payload) => api.post('/auth/register', payload),
  login: (payload) => api.post('/auth/login', payload),
}

export const tasksApi = {
  getAll: (params) => api.get('/tasks', { params }),
  getCounts: () => api.get('/tasks/counts'),
  getById: (id) => api.get(`/tasks/${id}`),
  create: (payload) => api.post('/tasks', payload),
  update: (id, payload) => api.put(`/tasks/${id}`, payload),
  remove: (id) => api.delete(`/tasks/${id}`),
}

export function getErrorMessage(error, fallback) {
  const data = error.response?.data
  if (data?.message) return data.message
  if (data?.title) return data.title
  if (data?.errors) {
    for (const key of Object.keys(data.errors)) {
      return data.errors[key][0]
    }
  }
  if (!error.response) return 'Нет соединения с сервером. Проверьте, что API запущен.'
  return fallback
}

export default api