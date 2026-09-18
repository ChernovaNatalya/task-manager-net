<script setup>
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '../stores/auth'
import { getErrorMessage } from '../api'

const router = useRouter()
const auth = useAuthStore()

const email = ref('')
const password = ref('')
const confirmPassword = ref('')
const error = ref('')
const loading = ref(false)

async function handleSubmit() {
  error.value = ''
  if (password.value !== confirmPassword.value) {
    error.value = 'Пароли не совпадают'
    return
  }
  loading.value = true
  try {
    await auth.register({ email: email.value, password: password.value })
    router.push('/tasks')
  } catch (e) {
    error.value = getErrorMessage(e, 'Не удалось зарегистрироваться')
  } finally {
    loading.value = false
  }
}
</script>

<template>
  <div class="auth-page">
    <form class="auth-card" @submit.prevent="handleSubmit">
      <h1 class="auth-title">Регистрация</h1>
      <p class="auth-subtitle">Создайте новый аккаунт</p>

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
        autocomplete="new-password"
        minlength="6"
        required
      />

      <label class="form-label" for="confirm-password">Подтвердите пароль</label>
      <input
        id="confirm-password"
        v-model="confirmPassword"
        class="form-input"
        type="password"
        autocomplete="new-password"
        minlength="6"
        required
      />

      <button class="btn btn-primary btn-block" type="submit" :disabled="loading">
        {{ loading ? 'Регистрация...' : 'Зарегистрироваться' }}
      </button>

      <p class="auth-switch">
        Уже есть аккаунт?
        <RouterLink :to="{ name: 'login' }">Войти</RouterLink>
      </p>
    </form>
  </div>
</template>