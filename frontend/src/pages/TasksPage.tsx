import { useEffect, useState } from 'react'
import { createTask, deleteTask, listTasks, updateTask } from '../api/tasksApi'
import { ApiClientError } from '../api/httpClient'
import { useAuth } from '../auth/AuthContext'
import { ErrorAlert } from '../components/ErrorAlert'
import { TaskForm } from '../components/TaskForm'
import { TaskList } from '../components/TaskList'
import type { TaskFormData, TaskItem } from '../types/task'

export function TasksPage() {
  const { token } = useAuth()
  const [tasks, setTasks] = useState<TaskItem[]>([])
  const [selectedTask, setSelectedTask] = useState<TaskItem | null>(null)
  const [error, setError] = useState<string | null>(null)
  const [isLoading, setIsLoading] = useState(true)

  async function loadTasks() {
    if (!token) {
      return
    }

    setError(null)
    setIsLoading(true)

    try {
      setTasks(await listTasks(token))
    } catch (caught) {
      setError(caught instanceof ApiClientError ? caught.message : 'Could not load tasks.')
    } finally {
      setIsLoading(false)
    }
  }

  useEffect(() => {
    void loadTasks()
  }, [token])

  async function handleSubmit(input: TaskFormData) {
    if (!token) {
      return
    }

    setError(null)
    try {
      if (selectedTask) {
        await updateTask(token, selectedTask.id, input)
        setSelectedTask(null)
      } else {
        await createTask(token, input)
      }
      await loadTasks()
    } catch (caught) {
      setError(caught instanceof ApiClientError ? caught.message : 'Could not save task.')
    }
  }

  async function handleDelete(task: TaskItem) {
    if (!token || !confirm(`Delete "${task.title}"?`)) {
      return
    }

    setError(null)
    try {
      await deleteTask(token, task.id)
      await loadTasks()
    } catch (caught) {
      setError(caught instanceof ApiClientError ? caught.message : 'Could not delete task.')
    }
  }

  return (
    <section className="grid gap-6 lg:grid-cols-[0.9fr_1.1fr]">
      <TaskForm selectedTask={selectedTask} onCancel={() => setSelectedTask(null)} onSubmit={handleSubmit} />

      <div className="rounded-3xl border border-white/70 bg-white/70 p-6 shadow-xl shadow-slate-200/70 backdrop-blur">
        <div className="mb-5 flex items-end justify-between gap-4">
          <div>
            <p className="text-sm font-bold uppercase tracking-[0.3em] text-teal-700">Your tasks</p>
            <h2 className="mt-2 text-2xl font-black text-slate-950">Today&apos;s command center</h2>
          </div>
          <span className="rounded-full bg-slate-950 px-4 py-2 text-sm font-black text-white">
            {tasks.length} total
          </span>
        </div>
        <ErrorAlert message={error} />
        <div className="mt-4">
          {isLoading ? (
            <div className="rounded-3xl bg-white/80 p-8 text-center font-bold text-slate-600">Loading tasks...</div>
          ) : (
            <TaskList tasks={tasks} onEdit={setSelectedTask} onDelete={handleDelete} />
          )}
        </div>
      </div>
    </section>
  )
}

