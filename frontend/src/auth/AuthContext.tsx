import { createContext, useContext, useState } from 'react'
import type { LoginResponse } from '../types/auth'

type AuthState = {
  token: string | null
  user: LoginResponse | null
  signIn: (login: LoginResponse) => void
  signOut: () => void
}

const AuthContext = createContext<AuthState | undefined>(undefined)

const tokenStorageKey = 'taskplanner.token'
const userStorageKey = 'taskplanner.user'

export function AuthProvider({ children }: { children: React.ReactNode }) {
  const [token, setToken] = useState<string | null>(() => localStorage.getItem(tokenStorageKey))
  const [user, setUser] = useState<LoginResponse | null>(() => {
    const stored = localStorage.getItem(userStorageKey)
    return stored ? (JSON.parse(stored) as LoginResponse) : null
  })

  function signIn(login: LoginResponse) {
    localStorage.setItem(tokenStorageKey, login.accessToken)
    localStorage.setItem(userStorageKey, JSON.stringify(login))
    setToken(login.accessToken)
    setUser(login)
  }

  function signOut() {
    localStorage.removeItem(tokenStorageKey)
    localStorage.removeItem(userStorageKey)
    setToken(null)
    setUser(null)
  }

  return (
    <AuthContext.Provider value={{ token, user, signIn, signOut }}>
      {children}
    </AuthContext.Provider>
  )
}

export function useAuth() {
  const context = useContext(AuthContext)
  if (!context) {
    throw new Error('useAuth must be used within AuthProvider.')
  }

  return context
}

