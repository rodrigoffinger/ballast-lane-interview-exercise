import { useState, type FormEvent } from 'react'
import { Link, useNavigate } from 'react-router-dom'
import { registerUser } from '../api/authApi'
import { ApiClientError } from '../api/httpClient'
import { ErrorAlert } from '../components/ErrorAlert'

export function RegisterPage() {
  const navigate = useNavigate()
  const [name, setName] = useState('')
  const [email, setEmail] = useState('')
  const [password, setPassword] = useState('')
  const [error, setError] = useState<string | null>(null)
  const [isSubmitting, setIsSubmitting] = useState(false)

  async function handleSubmit(event: FormEvent) {
    event.preventDefault()
    setError(null)
    setIsSubmitting(true)

    try {
      await registerUser({ name, email, password })
      navigate('/login')
    } catch (caught) {
      setError(caught instanceof ApiClientError ? caught.message : 'Registration failed.')
    } finally {
      setIsSubmitting(false)
    }
  }

  return (
    <section className="mx-auto max-w-xl rounded-3xl border border-white/70 bg-white/90 p-8 shadow-xl shadow-slate-200/70">
      <p className="text-sm font-bold uppercase tracking-[0.35em] text-teal-700">Create account</p>
      <h2 className="mt-3 text-3xl font-black tracking-tight text-slate-950">Start with a clean task board.</h2>
      <form onSubmit={handleSubmit} className="mt-7 space-y-5">
        <ErrorAlert message={error} />
        <label className="block">
          <span className="text-sm font-bold text-slate-700">Name</span>
          <input
            value={name}
            onChange={(event) => setName(event.target.value)}
            className="mt-2 w-full rounded-2xl border border-slate-200 bg-white px-4 py-3 outline-none transition focus:border-teal-500 focus:ring-4 focus:ring-teal-100"
            required
          />
        </label>
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
          className="w-full rounded-2xl bg-slate-950 px-5 py-3 font-black text-white transition hover:bg-teal-700 disabled:cursor-not-allowed disabled:opacity-60"
        >
          {isSubmitting ? 'Creating account...' : 'Create account'}
        </button>
        <p className="text-center text-sm text-slate-600">
          Already registered?{' '}
          <Link className="font-bold text-teal-700 hover:text-teal-900" to="/login">
            Sign in
          </Link>
        </p>
      </form>
    </section>
  )
}
