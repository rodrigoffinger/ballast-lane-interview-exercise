import { apiRequest } from './httpClient'
import type { LoginResponse, RegisterResponse } from '../types/auth'

export function registerUser(input: {
  name: string
  email: string
  password: string
}) {
  return apiRequest<RegisterResponse>('/api/auth/register', {
    method: 'POST',
    body: JSON.stringify(input),
  })
}

export function login(input: { email: string; password: string }) {
  return apiRequest<LoginResponse>('/api/auth/login', {
    method: 'POST',
    body: JSON.stringify(input),
  })
}

