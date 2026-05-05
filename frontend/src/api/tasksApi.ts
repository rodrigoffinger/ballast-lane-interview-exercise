import { apiRequest } from './httpClient'
import type { TaskFormData, TaskItem } from '../types/task'

export function listTasks(token: string) {
  return apiRequest<TaskItem[]>('/api/tasks', {}, token)
}

export function createTask(token: string, input: TaskFormData) {
  return apiRequest<TaskItem>(
    '/api/tasks',
    {
      method: 'POST',
      body: JSON.stringify(input),
    },
    token,
  )
}

export function updateTask(token: string, id: string, input: TaskFormData) {
  return apiRequest<TaskItem>(
    `/api/tasks/${id}`,
    {
      method: 'PUT',
      body: JSON.stringify(input),
    },
    token,
  )
}

export function deleteTask(token: string, id: string) {
  return apiRequest<void>(
    `/api/tasks/${id}`,
    {
      method: 'DELETE',
    },
    token,
  )
}

