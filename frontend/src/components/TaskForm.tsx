import { useEffect, useState, type FormEvent } from 'react'
import { TaskStatus, type TaskFormData, type TaskItem } from '../types/task'

const emptyTask: TaskFormData = {
  title: '',
  description: '',
  status: TaskStatus.Todo,
  dueDate: new Date().toISOString().slice(0, 10),
}

export function TaskForm({
  selectedTask,
  onCancel,
  onSubmit,
}: {
  selectedTask: TaskItem | null
  onCancel: () => void
  onSubmit: (input: TaskFormData) => Promise<void>
}) {
  const [form, setForm] = useState<TaskFormData>(emptyTask)
  const [isSubmitting, setIsSubmitting] = useState(false)

  useEffect(() => {
    if (!selectedTask) {
      setForm(emptyTask)
      return
    }

    setForm({
      title: selectedTask.title,
      description: selectedTask.description,
      status: selectedTask.status,
      dueDate: selectedTask.dueDate.slice(0, 10),
    })
  }, [selectedTask])

  async function handleSubmit(event: FormEvent) {
    event.preventDefault()
    setIsSubmitting(true)

    try {
      await onSubmit({
        ...form,
        dueDate: new Date(`${form.dueDate}T12:00:00.000Z`).toISOString(),
      })
      setForm(emptyTask)
    } finally {
      setIsSubmitting(false)
    }
  }

  return (
    <form onSubmit={handleSubmit} className="rounded-3xl border border-white/70 bg-white/90 p-6 shadow-xl shadow-slate-200/70">
      <div className="flex items-start justify-between gap-4">
        <div>
          <p className="text-sm font-bold uppercase tracking-[0.3em] text-teal-700">
            {selectedTask ? 'Edit task' : 'New task'}
          </p>
          <h2 className="mt-2 text-2xl font-black text-slate-950">
            {selectedTask ? 'Refine the details' : 'Capture the next move'}
          </h2>
        </div>
        {selectedTask && (
          <button type="button" onClick={onCancel} className="rounded-full bg-slate-100 px-3 py-1 text-sm font-bold text-slate-700">
            Cancel
          </button>
        )}
      </div>

      <div className="mt-6 space-y-4">
        <label className="block">
          <span className="text-sm font-bold text-slate-700">Title</span>
          <input
            value={form.title}
            onChange={(event) => setForm({ ...form, title: event.target.value })}
            className="mt-2 w-full rounded-2xl border border-slate-200 bg-white px-4 py-3 outline-none transition focus:border-teal-500 focus:ring-4 focus:ring-teal-100"
            required
          />
        </label>
        <label className="block">
          <span className="text-sm font-bold text-slate-700">Description</span>
          <textarea
            value={form.description}
            onChange={(event) => setForm({ ...form, description: event.target.value })}
            className="mt-2 min-h-28 w-full rounded-2xl border border-slate-200 bg-white px-4 py-3 outline-none transition focus:border-teal-500 focus:ring-4 focus:ring-teal-100"
            required
          />
        </label>
        <div className="grid gap-4 sm:grid-cols-2">
          <label className="block">
            <span className="text-sm font-bold text-slate-700">Status</span>
            <select
              value={form.status}
              onChange={(event) => setForm({ ...form, status: Number(event.target.value) as TaskStatus })}
              className="mt-2 w-full rounded-2xl border border-slate-200 bg-white px-4 py-3 outline-none transition focus:border-teal-500 focus:ring-4 focus:ring-teal-100"
            >
              <option value={TaskStatus.Todo}>Todo</option>
              <option value={TaskStatus.InProgress}>In progress</option>
              <option value={TaskStatus.Done}>Done</option>
            </select>
          </label>
          <label className="block">
            <span className="text-sm font-bold text-slate-700">Due date</span>
            <input
              value={form.dueDate}
              onChange={(event) => setForm({ ...form, dueDate: event.target.value })}
              className="mt-2 w-full rounded-2xl border border-slate-200 bg-white px-4 py-3 outline-none transition focus:border-teal-500 focus:ring-4 focus:ring-teal-100"
              type="date"
              required
            />
          </label>
        </div>
        <button
          type="submit"
          disabled={isSubmitting}
          className="w-full rounded-2xl bg-teal-600 px-5 py-3 font-black text-white transition hover:bg-teal-700 disabled:cursor-not-allowed disabled:opacity-60"
        >
          {isSubmitting ? 'Saving...' : selectedTask ? 'Save changes' : 'Create task'}
        </button>
      </div>
    </form>
  )
}
