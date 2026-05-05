import { TaskStatus, type TaskItem } from '../types/task'

const statusLabels = {
  [TaskStatus.Todo]: 'Todo',
  [TaskStatus.InProgress]: 'In progress',
  [TaskStatus.Done]: 'Done',
}

const statusClasses = {
  [TaskStatus.Todo]: 'bg-slate-100 text-slate-700',
  [TaskStatus.InProgress]: 'bg-amber-100 text-amber-800',
  [TaskStatus.Done]: 'bg-teal-100 text-teal-800',
}

export function TaskList({
  tasks,
  onEdit,
  onDelete,
}: {
  tasks: TaskItem[]
  onEdit: (task: TaskItem) => void
  onDelete: (task: TaskItem) => void
}) {
  if (tasks.length === 0) {
    return (
      <div className="rounded-3xl border border-dashed border-slate-300 bg-white/70 p-8 text-center">
        <p className="font-bold text-slate-700">No tasks yet.</p>
        <p className="mt-1 text-sm text-slate-500">Create the first one and we have lift-off.</p>
      </div>
    )
  }

  return (
    <div className="space-y-4">
      {tasks.map((task) => (
        <article key={task.id} className="rounded-3xl border border-white/70 bg-white/90 p-5 shadow-lg shadow-slate-200/60">
          <div className="flex flex-col gap-4 sm:flex-row sm:items-start sm:justify-between">
            <div>
              <span className={`inline-flex rounded-full px-3 py-1 text-xs font-black uppercase tracking-wide ${statusClasses[task.status]}`}>
                {statusLabels[task.status]}
              </span>
              <h3 className="mt-3 text-xl font-black text-slate-950">{task.title}</h3>
              <p className="mt-2 text-sm leading-6 text-slate-600">{task.description}</p>
              <p className="mt-3 text-sm font-bold text-slate-500">
                Due {new Date(task.dueDate).toLocaleDateString()}
              </p>
            </div>
            <div className="flex gap-2">
              <button
                type="button"
                onClick={() => onEdit(task)}
                className="rounded-full bg-slate-100 px-4 py-2 text-sm font-bold text-slate-700 transition hover:bg-slate-200"
              >
                Edit
              </button>
              <button
                type="button"
                onClick={() => onDelete(task)}
                className="rounded-full bg-red-50 px-4 py-2 text-sm font-bold text-red-700 transition hover:bg-red-100"
              >
                Delete
              </button>
            </div>
          </div>
        </article>
      ))}
    </div>
  )
}

