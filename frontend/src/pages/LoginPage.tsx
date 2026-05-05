import { useState, type FormEvent } from 'react'
import { Link, useNavigate } from 'react-router-dom'
import { login } from '../api/authApi'
import { ApiClientError } from '../api/httpClient'
import { useAuth } from '../auth/AuthContext'
import { ErrorAlert } from '../components/ErrorAlert'

export function LoginPage() {
  const navigate = useNavigate()
  const { signIn } = useAuth()
  const [email, setEmail] = useState('demo@ballastlane.local')
  const [password, setPassword] = useState('Demo123!')
  const [error, setError] = useState<string | null>(null)
  const [isSubmitting, setIsSubmitting] = useState(false)

  async function handleSubmit(event: FormEvent) {
    event.preventDefault()
    setError(null)
    setIsSubmitting(true)

    try {
      const result = await login({ email, password })
      signIn(result)
      navigate('/tasks')
    } catch (caught) {
      setError(caught instanceof ApiClientError ? caught.message : 'Login failed.')
    } finally {
      setIsSubmitting(false)
    }
  }

  return (
    <section className="mx-auto grid max-w-5xl gap-6 lg:grid-cols-[1fr_0.9fr]">
      <div className="rounded-3xl bg-slate-950 p-8 text-white shadow-xl">
        <p className="text-sm font-bold uppercase tracking-[0.35em] text-teal-300">
          Welcome back
        </p>
        <h2 className="mt-4 text-4xl font-black tracking-tight">
          Pick up the thread without hunting through chaos.
        </h2>
        <p className="mt-4 text-slate-300">
          Demo credentials are prefilled so the interview panel can immediately explore the secured task flow.
        </p>
      </div>

      <form onSubmit={handleSubmit} className="rounded-3xl border border-white/70 bg-white/90 p-8 shadow-xl shadow-slate-200/70">
        <div className="space-y-5">
          <ErrorAlert message={error} />
          <label className="block">
            <span className="text-sm font-bold text-slate-700">Email</span>
            <input
              value={email}
              onChange={(event) => setEmail(event.target.value)}
              className="mt-2 w-full rounded-2xl border border-slate-200 bg-white px-4 py-3 outline-none transition focus:border-teal-500 focus:ring-4 focus:ring-teal-100"
              type="email"
              required
            />
          </label>
          <label className="block">
            <span className="text-sm font-bold text-slate-700">Password</span>
            <input
              value={password}
              onChange={(event) => setPassword(event.target.value)}
              className="mt-2 w-full rounded-2xl border border-slate-200 bg-white px-4 py-3 outline-none transition focus:border-teal-500 focus:ring-4 focus:ring-teal-100"
              type="password"
              required
            />
          </label>
          <button
            type="submit"
            disabled={isSubmitting}
            className="w-full rounded-2xl bg-teal-600 px-5 py-3 font-black text-white transition hover:bg-teal-700 disabled:cursor-not-allowed disabled:opacity-60"
          >
            {isSubmitting ? 'Signing in...' : 'Sign in'}
          </button>
          <p className="text-center text-sm text-slate-600">
            Need another account?{' '}
            <Link className="font-bold text-teal-700 hover:text-teal-900" to="/register">
              Register here
            </Link>
          </p>
        </div>
      </form>
    </section>
  )
}
