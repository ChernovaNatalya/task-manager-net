<script setup>
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '../stores/auth'
import { getErrorMessage } from '../api'

const router = useRouter()
const auth = useAuthStore()

const email = ref('')
const password = ref('')
const error = ref('')
const loading = ref(false)

async function handleSubmit() {
  error.value = ''
  loading.value = true
  try {
    await auth.login({ email: email.value, password: password.value })
    router.push('/tasks')
  } catch (e) {
    error.value = getErrorMessage(e, 'Неверный email или пароль')
  } finally {
    loading.value = false
  }
}
</script>

<template>
  <div class="auth-page">
    <form class="auth-card" @submit.prevent="handleSubmit">
      <h1 class="auth-title">Вход</h1>
      <p class="auth-subtitle">Добро пожаловать обратно</p>

      <div v-if="error" class="alert alert-error">{{ error }}</div>

      <label class="form-label" for="email">Email</label>
      <input
        id="email"
        v-model="email"
        class="form-input"
        type="email"
        autocomplete="email"
        required
      />

      <label class="form-label" for="password">Пароль</label>
      <input
        id="password"
        v-model="password"
        class="form-input"
        type="password"
        autocomplete="current-password"
        required
      />

      <button class="btn btn-primary btn-block" type="submit" :disabled="loading">
        {{ loading ? 'Вход...' : 'Войти' }}
      </button>

      <p class="auth-switch">
        Нет аккаунта?
        <RouterLink :to="{ name: 'register' }">Зарегистрироваться</RouterLink>
      </p>
    </form>
  </div>
</template>