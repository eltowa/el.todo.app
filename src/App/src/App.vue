<script setup lang="ts">
import { ref, onMounted } from 'vue'

interface Todo {
  id: number
  title: string
  isComplete: boolean
}

const apiBase = import.meta.env.API_HTTP ?? ''
const todos = ref<Todo[]>([])
const loading = ref(false)
const error = ref<string | null>(null)
const newTodoTitle = ref('')

async function loadTodos() {
  loading.value = true
  error.value = null
  try {
    const response = await fetch(`${apiBase}/api/todos`, {
      signal: AbortSignal.timeout(5000),
    })
    if (!response.ok) throw new Error(`Failed to load todos: ${response.status}`)
    todos.value = await response.json()
  } catch (err) {
    error.value = err instanceof Error ? err.message : 'Unknown error'
  } finally {
    loading.value = false
  }
}

async function addTodo() {
  const title = newTodoTitle.value.trim()
  if (!title) return
  loading.value = true
  error.value = null
  try {
    const response = await fetch(`${apiBase}/api/todos`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ title }),
    })
    if (!response.ok) throw new Error(`Failed to add todo: ${response.status}`)
    await loadTodos()
    newTodoTitle.value = ''
  } catch (err) {
    error.value = err instanceof Error ? err.message : 'Unknown error'
  } finally {
    loading.value = false
  }
}

async function toggleTodo(todo: Todo) {
  loading.value = true
  error.value = null
  try {
    const response = await fetch(`${apiBase}/api/todos/${todo.id}`, {
      method: 'PUT',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ ...todo, isComplete: !todo.isComplete }),
    })
    if (!response.ok) throw new Error(`Failed to update todo: ${response.status}`)
    await loadTodos()
  } catch (err) {
    error.value = err instanceof Error ? err.message : 'Unknown error'
  } finally {
    loading.value = false
  }
}

async function deleteTodo(id: number) {
  loading.value = true
  error.value = null
  try {
    const response = await fetch(`${apiBase}/api/todos/${id}`, {
      method: 'DELETE',
    })
    if (!response.ok) throw new Error(`Failed to delete todo: ${response.status}`)
    await loadTodos()
  } catch (err) {
    error.value = err instanceof Error ? err.message : 'Unknown error'
  } finally {
    loading.value = false
  }
}

onMounted(() => {
  loadTodos()
})
</script>

<template>
  <div class="todo-app">
    <h1>Todo App</h1>

    <div v-if="error" class="error">{{ error }}</div>

    <div v-if="loading" class="loading">Loading...</div>

    <form @submit.prevent="addTodo" class="add-todo">
      <input
        v-model="newTodoTitle"
        type="text"
        placeholder="What needs to be done?"
        class="todo-input"
      />
      <button type="submit" class="add-button">Add</button>
    </form>

    <ul class="todo-list">
      <li
        v-for="todo in todos"
        :key="todo.id"
        class="todo-item"
        :class="{ completed: todo.isComplete }"
      >
        <input
          type="checkbox"
          :checked="todo.isComplete"
          @change="toggleTodo(todo)"
        />
        <span>{{ todo.title }}</span>
        <button @click="deleteTodo(todo.id)" class="delete-button">Delete</button>
      </li>
    </ul>

    <p v-if="todos.length === 0 && !loading && !error" class="empty">
      No todos yet. Add one above!
    </p>
  </div>
</template>

<style scoped>
.todo-app {
  max-width: 600px;
  margin: 2rem auto;
  font-family: system-ui, -apple-system, sans-serif;
  padding: 0 1rem;
}

h1 {
  text-align: center;
  color: #333;
}

.error {
  color: #dc2626;
  background: #fee2e2;
  padding: 0.75rem;
  border-radius: 0.375rem;
  margin-bottom: 1rem;
}

.loading {
  text-align: center;
  color: #666;
  margin: 2rem 0;
}

.add-todo {
  display: flex;
  gap: 0.5rem;
  margin-bottom: 1.5rem;
}

.todo-input {
  flex: 1;
  padding: 0.5rem 0.75rem;
  border: 1px solid #d1d5db;
  border-radius: 0.375rem;
  font-size: 1rem;
}

.add-button {
  padding: 0.5rem 1rem;
  background: #2563eb;
  color: white;
  border: none;
  border-radius: 0.375rem;
  cursor: pointer;
  font-size: 1rem;
}

.add-button:hover {
  background: #1d4ed8;
}

.todo-list {
  list-style: none;
  padding: 0;
  margin: 0;
}

.todo-item {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 0.75rem;
  border: 1px solid #e5e7eb;
  border-radius: 0.375rem;
  margin-bottom: 0.5rem;
}

.todo-item.completed span {
  text-decoration: line-through;
  color: #9ca3af;
}

.todo-item input[type="checkbox"] {
  margin-right: 0.75rem;
}

.delete-button {
  background: #dc2626;
  color: white;
  border: none;
  border-radius: 0.25rem;
  padding: 0.25rem 0.5rem;
  cursor: pointer;
}

.delete-button:hover {
  background: #b91c1c;
}

.empty {
  text-align: center;
  color: #666;
  margin-top: 2rem;
}
</style>
