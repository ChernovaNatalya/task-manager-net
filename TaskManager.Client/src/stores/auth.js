import { defineStore } from 'pinia'
import { ref } from 'vue'
import { authApi } from '../api'

export const useAuthStore = defineStore('auth', () => {
  const token = ref(localStorage.getItem('token') || '')
  const email = ref(localStorage.getItem('user') || '')

  const isAuthenticated = () => !!token.value

  async function login(credentials) {
    const { data } = await authApi.login(credentials)
    token.value = data.token
    email.value = data.email
    localStorage.setItem('token', data.token)
    localStorage.setItem('user', data.email)
  }

  async function register(payload) {
    const { data } = await authApi.register(payload)
    token.value = data.token
    email.value = data.email
    localStorage.setItem('token', data.token)
    localStorage.setItem('user', data.email)
  }

  function logout() {
    token.value = ''
    email.value = ''
    localStorage.removeItem('token')
    localStorage.removeItem('user')
  }

  return { token, email, isAuthenticated, login, register, logout }
})