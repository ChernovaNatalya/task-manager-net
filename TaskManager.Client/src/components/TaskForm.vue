<script setup>
import { reactive, ref, watch } from 'vue'

const props = defineProps({
  task: { type: Object, default: null },
  visible: { type: Boolean, default: false },
})

const emit = defineEmits(['save', 'close'])

const form = reactive({
  title: '',
  description: '',
  status: 0,
  priority: 1,
  dueDate: '',
})

const error = ref('')
const saving = ref(false)

watch(
  () => props.visible,
  () => {
    if (props.visible) {
      error.value = ''
      form.title = props.task?.title || ''
      form.description = props.task?.description || ''
      form.status = props.task?.status ?? 0
      form.priority = props.task?.priority ?? 1
      form.dueDate = props.task?.dueDate ? props.task.dueDate.slice(0, 16) : ''
    }
  },
)

async function handleSubmit() {
  error.value = ''
  if (!form.title.trim()) {
    error.value = 'Название обязательно'
    return
  }
  saving.value = true
  try {
    const payload = {
      title: form.title.trim(),
      description: form.description.trim() || null,
      status: Number(form.status),
      priority: Number(form.priority),
      dueDate: form.dueDate ? new Date(form.dueDate).toISOString() : null,
    }
    await emit('save', payload)
  } finally {
    saving.value = false
  }
}
</script>

<template>
  <Teleport to="body">
    <div v-if="visible" class="modal-overlay" @click.self="$emit('close')">
      <form class="modal-card" @submit.prevent="handleSubmit">
        <div class="modal-header">
          <h2 class="modal-title">{{ task ? 'Редактировать задачу' : 'Новая задача' }}</h2>
          <button type="button" class="modal-close" aria-label="Закрыть" @click="$emit('close')">
            &times;
          </button>
        </div>

        <div v-if="error" class="alert alert-error">{{ error }}</div>

        <label class="form-label" for="task-title">Название</label>
        <input
          id="task-title"
          v-model="form.title"
          class="form-input"
          type="text"
          maxlength="200"
          placeholder="Например: Сделать отчёт"
          required
        />

        <label class="form-label" for="task-description">Описание</label>
        <textarea
          id="task-description"
          v-model="form.description"
          class="form-input form-textarea"
          maxlength="2000"
          rows="4"
          placeholder="Детали задачи (необязательно)"
        ></textarea>

        <div class="form-row">
          <div>
            <label class="form-label" for="task-status">Статус</label>
            <select id="task-status" v-model.number="form.status" class="form-input">
              <option :value="0">К выполнению</option>
              <option :value="1">В работе</option>
              <option :value="2">Готово</option>
            </select>
          </div>
          <div>
            <label class="form-label" for="task-priority">Приоритет</label>
            <select id="task-priority" v-model.number="form.priority" class="form-input">
              <option :value="0">Низкий</option>
              <option :value="1">Средний</option>
              <option :value="2">Высокий</option>
            </select>
          </div>
        </div>

        <label class="form-label" for="task-duedate">Срок выполнения</label>
        <input id="task-duedate" v-model="form.dueDate" class="form-input" type="datetime-local" />

        <div class="modal-actions">
          <button type="button" class="btn btn-secondary" @click="$emit('close')">Отмена</button>
          <button type="submit" class="btn btn-primary" :disabled="saving">
            {{ saving ? 'Сохранение...' : task ? 'Сохранить' : 'Создать' }}
          </button>
        </div>
      </form>
    </div>
  </Teleport>
</template>