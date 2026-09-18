<script setup>
import { onMounted, ref, watch } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '../stores/auth'
import { tasksApi } from '../api'
import TaskForm from '../components/TaskForm.vue'

const router = useRouter()
const auth = useAuthStore()

const tasks = ref([])
const counts = ref({ all: 0, todo: 0, progress: 0, done: 0 })
const loading = ref(true)
const loadError = ref('')
const filter = ref('all')
const modalVisible = ref(false)
const editingTask = ref(null)

const statusLabels = ['К выполнению', 'В работе', 'Готово']
const statusClasses = ['status-todo', 'status-progress', 'status-done']
const priorityLabels = ['Низкий', 'Средний', 'Высокий']
const priorityClasses = ['priority-low', 'priority-medium', 'priority-high']

watch(filter, () => loadTasks())

onMounted(() => {
  loadTasks()
  loadCounts()
})

async function loadTasks({ silent = false } = {}) {
  if (!silent) {
    loading.value = true
    loadError.value = ''
  }
  try {
    const params = filter.value === 'all' ? {} : { status: filter.value }
    const { data } = await tasksApi.getAll(params)
    tasks.value = data
  } catch {
    if (!silent) loadError.value = 'Не удалось загрузить задачи'
  } finally {
    loading.value = false
  }
}

async function loadCounts() {
  try {
    const { data } = await tasksApi.getCounts()
    counts.value = data
  } catch {
    // ошибка загрузки уже показана в loadTasks
  }
}

function openCreate() {
  editingTask.value = null
  modalVisible.value = true
}

function openEdit(task) {
  editingTask.value = task
  modalVisible.value = true
}

async function handleSave(payload) {
  try {
    if (editingTask.value) {
      await tasksApi.update(editingTask.value.id, payload)
    } else {
      await tasksApi.create(payload)
    }
    modalVisible.value = false
    await Promise.all([loadTasks(), loadCounts()])
  } catch {
    loadError.value = editingTask.value
      ? 'Не удалось обновить задачу'
      : 'Не удалось создать задачу'
  }
}

async function handleDelete(task) {
  if (!confirm(`Удалить задачу «${task.title}»?`)) return
  try {
    await tasksApi.remove(task.id)
    await Promise.all([loadTasks(), loadCounts()])
  } catch {
    loadError.value = 'Не удалось удалить задачу'
  }
}

async function toggleStatus(task) {
  const status = task.status === 2 ? 0 : task.status + 1
  await updateStatus(task, status)
}

async function updateStatus(task, status) {
  const payload = {
    title: task.title,
    description: task.description,
    status,
    priority: task.priority,
    dueDate: task.dueDate,
  }
  try {
    await tasksApi.update(task.id, payload)
  } catch {
    loadError.value = 'Не удалось обновить статус'
    return
  }
  await Promise.all([loadTasks({ silent: true }), loadCounts()])
}

function formatDate(value) {
  if (!value) return ''
  return new Date(value).toLocaleString('ru-RU', {
    day: '2-digit',
    month: '2-digit',
    year: 'numeric',
    hour: '2-digit',
    minute: '2-digit',
  })
}

function handleLogout() {
  auth.logout()
  router.push('/login')
}
</script>

<template>
  <div class="app">
    <header class="header">
      <div class="header-inner">
        <h1 class="logo">
          <span class="logo-check">&#10003;</span> Задачи
        </h1>
        <div class="header-actions">
          <span class="header-user">{{ auth.email }}</span>
          <button class="btn btn-secondary" @click="handleLogout">Выйти</button>
        </div>
      </div>
    </header>

    <main class="container">
      <div class="toolbar">
        <div class="filters">
          <button
            class="filter-btn"
            :class="{ active: filter === 'all' }"
            @click="filter = 'all'"
          >
            Все <span class="badge">{{ counts.all }}</span>
          </button>
          <button
            class="filter-btn"
            :class="{ active: filter === '0' }"
            @click="filter = '0'"
          >
            К выполнению <span class="badge">{{ counts.todo }}</span>
          </button>
          <button
            class="filter-btn"
            :class="{ active: filter === '1' }"
            @click="filter = '1'"
          >
            В работе <span class="badge">{{ counts.progress }}</span>
          </button>
          <button
            class="filter-btn"
            :class="{ active: filter === '2' }"
            @click="filter = '2'"
          >
            Готово <span class="badge">{{ counts.done }}</span>
          </button>
        </div>
        <button class="btn btn-primary" @click="openCreate">+ Новая задача</button>
      </div>

      <div v-if="loadError" class="alert alert-error">{{ loadError }}</div>

      <div v-if="loading" class="state-box">Загрузка...</div>

      <div v-else-if="tasks.length === 0" class="state-box">
        {{ filter === 'all' ? 'Задач нет. Нажмите «+ Новая задача», чтобы добавить.' : 'Задач с таким статусом нет.' }}
      </div>

      <ul v-else class="task-list">
        <li v-for="task in tasks" :key="task.id">
          <div class="task-card" :class="{ 'task-done': task.status === 2 }">
            <div class="task-main">
              <button
                class="task-checkbox"
                :class="{ checked: task.status === 2 }"
                :aria-label="task.status === 2 ? 'Открыть задачу' : 'Завершить задачу'"
                @click="toggleStatus(task)"
              ></button>
              <div class="task-content">
                <h3 class="task-title" @click="openEdit(task)">{{ task.title }}</h3>
                <p v-if="task.description" class="task-description">{{ task.description }}</p>
                <div class="task-meta">
                  <span class="badge" :class="statusClasses[task.status]">
                    {{ statusLabels[task.status] }}
                  </span>
                  <span class="badge" :class="priorityClasses[task.priority]">
                    Приоритет: {{ priorityLabels[task.priority] }}
                  </span>
                  <span v-if="task.dueDate" class="task-date">&#9200; {{ formatDate(task.dueDate) }}</span>
                </div>
              </div>
            </div>
            <div class="task-actions">
              <button class="icon-btn" title="Редактировать" @click="openEdit(task)">&#9998;</button>
              <button class="icon-btn icon-btn-danger" title="Удалить" @click="handleDelete(task)">
                &#128465;
              </button>
            </div>
          </div>
        </li>
      </ul>
    </main>

    <TaskForm
      :task="editingTask"
      :visible="modalVisible"
      @save="handleSave"
      @close="modalVisible = false"
    />
  </div>
</template>