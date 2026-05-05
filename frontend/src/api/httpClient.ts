import type { ApiResponse } from '../types/api'

export class ApiClientError extends Error {
  public readonly errors: { code: string; message: string }[]

  constructor(errors: { code: string; message: string }[]) {
    super(errors.map((error) => error.message).join('\n'))
    this.errors = errors
  }
}

export async function apiRequest<T>(
  path: string,
  options: RequestInit = {},
  token?: string | null,
): Promise<T> {
  const response = await fetch(path, {
    ...options,
    headers: {
      'Content-Type': 'application/json',
      ...(token ? { Authorization: `Bearer ${token}` } : {}),
      ...options.headers,
    },
  })

  if (response.status === 204) {
    return undefined as T
  }

  const body = (await response.json()) as ApiResponse<T>

  if (!response.ok || !body.success) {
    throw new ApiClientError(body.errors)
  }

  return body.data as T
}
