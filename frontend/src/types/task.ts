export const TaskStatus = {
  Todo: 0,
  InProgress: 1,
  Done: 2,
} as const

export type TaskStatus = (typeof TaskStatus)[keyof typeof TaskStatus]

export type TaskItem = {
  id: string
  userId: string
  title: string
  description: string
  status: TaskStatus
  dueDate: string
  createdAt: string
  updatedAt: string
}

export type TaskFormData = {
  title: string
  description: string
  status: TaskStatus
  dueDate: string
}
