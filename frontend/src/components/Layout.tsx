import { Link, useLocation, useNavigate } from 'react-router-dom'
import { useAuth } from '../auth/AuthContext'

export function Layout({ children }: { children: React.ReactNode }) {
  const { token, user, signOut } = useAuth()
  const navigate = useNavigate()
  const location = useLocation()

  function handleSignOut() {
    signOut()
    navigate('/login')
  }

  return (
    <main className="min-h-screen px-4 py-6 sm:px-6 lg:px-8">
      <div className="mx-auto max-w-6xl">
        <header className="mb-8 flex flex-col gap-4 rounded-3xl border border-white/70 bg-white/80 p-5 shadow-xl shadow-slate-200/70 backdrop-blur md:flex-row md:items-center md:justify-between">
          <Link to="/tasks" className="group">
            <p className="text-xs font-bold uppercase tracking-[0.35em] text-teal-700">
              TaskPlanner
            </p>
            <h1 className="mt-1 text-3xl font-black tracking-tight text-slate-950">
              Calm task control
            </h1>
          </Link>

          <nav className="flex flex-wrap items-center gap-3">
            {token ? (
              <>
                <span className="rounded-full bg-slate-100 px-4 py-2 text-sm font-semibold text-slate-700">
                  {user?.email}
                </span>
                <button
                  type="button"
                  onClick={handleSignOut}
                  className="rounded-full bg-slate-950 px-4 py-2 text-sm font-bold text-white transition hover:bg-teal-700"
                >
                  Sign out
                </button>
              </>
            ) : (
              <>
                <Link
                  to="/login"
                  className={`rounded-full px-4 py-2 text-sm font-bold transition ${
                    location.pathname === '/login'
                      ? 'bg-slate-950 text-white'
                      : 'bg-white text-slate-700 hover:bg-slate-100'
                  }`}
                >
                  Login
                </Link>
                <Link
                  to="/register"
                  className={`rounded-full px-4 py-2 text-sm font-bold transition ${
                    location.pathname === '/register'
                      ? 'bg-slate-950 text-white'
                      : 'bg-white text-slate-700 hover:bg-slate-100'
                  }`}
                >
                  Register
                </Link>
              </>
            )}
          </nav>
        </header>

        {children}
      </div>
    </main>
  )
}

