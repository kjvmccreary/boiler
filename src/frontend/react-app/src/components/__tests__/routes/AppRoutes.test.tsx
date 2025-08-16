import { describe, it, expect, vi } from 'vitest'
import { render, screen } from '@testing-library/react'
import { MemoryRouter, Outlet } from 'react-router-dom'
import { QueryClient, QueryClientProvider } from '@tanstack/react-query'
import { AppRoutesConfig } from '@/routes/index.js'

// Mock all the components
vi.mock('@/components/auth/LoginForm.js', () => ({
  LoginForm: () => <div>Login Form</div>,
}))

vi.mock('@/components/roles/RoleDetails.js', () => ({
  RoleDetails: () => <div>Role Details</div>,
}))

vi.mock('@/components/roles/RoleEditor.js', () => ({
  RoleEditor: () => <div>Role Editor</div>,
}))

vi.mock('@/components/roles/RoleList.js', () => ({
  RoleList: () => <div>Role List</div>,
}))

// Mock AppLayout to properly render Outlet (child routes)
vi.mock('@/components/layout/AppLayout.js', () => ({
  AppLayout: () => (
    <div>
      App Layout:
      <Outlet />
    </div>
  ),
}))

vi.mock('@/components/authorization/ProtectedRoute.js', () => ({
  ProtectedRoute: ({ children }: { children: React.ReactNode }) => (
    <div>Protected: {children}</div>
  ),
}))

vi.mock('@/pages/Dashboard.js', () => ({
  Dashboard: () => <div>Dashboard</div>,
}))

// Mock auth context
vi.mock('@/contexts/AuthContext.js', () => ({
  useAuth: () => ({
    user: { permissions: ['roles.view', 'roles.edit', 'roles.create'] },
    isAuthenticated: true,
  }),
  AuthProvider: ({ children }: { children: React.ReactNode }) => <>{children}</>,
}))

// Mock permission context
vi.mock('@/contexts/PermissionContext.js', () => ({
  usePermission: () => ({
    hasPermission: () => true,
    hasAnyPermission: () => true,
    hasAllPermissions: () => true,
  }),
  PermissionProvider: ({ children }: { children: React.ReactNode }) => <>{children}</>,
}))

// Mock CanAccess component
vi.mock('@/components/authorization/CanAccess.js', () => ({
  CanAccess: ({ children }: { children: React.ReactNode }) => <>{children}</>,
}))

// Test wrapper that provides router and query client
function TestWrapper({ children, initialEntries }: { children: React.ReactNode; initialEntries: string[] }) {
  const queryClient = new QueryClient({
    defaultOptions: {
      queries: { retry: false, gcTime: 0 },
    },
  })

  return (
    <QueryClientProvider client={queryClient}>
      <MemoryRouter initialEntries={initialEntries}>
        {children}
      </MemoryRouter>
    </QueryClientProvider>
  )
}

describe('AppRoutes', () => {
  it('renders role details on /roles/:id route', () => {
    render(
      <TestWrapper initialEntries={['/roles/1']}>
        <AppRoutesConfig />
      </TestWrapper>
    )

    expect(screen.getByText('Role Details')).toBeInTheDocument()
  })

  it('renders role editor on /roles/:id/edit route', () => {
    render(
      <TestWrapper initialEntries={['/roles/1/edit']}>
        <AppRoutesConfig />
      </TestWrapper>
    )

    expect(screen.getByText('Role Editor')).toBeInTheDocument()
  })

  it('renders role editor on /roles/new route', () => {
    render(
      <TestWrapper initialEntries={['/roles/new']}>
        <AppRoutesConfig />
      </TestWrapper>
    )

    expect(screen.getByText('Role Editor')).toBeInTheDocument()
  })

  it('renders role list on /roles route', () => {
    render(
      <TestWrapper initialEntries={['/roles']}>
        <AppRoutesConfig />
      </TestWrapper>
    )

    expect(screen.getByText('Role List')).toBeInTheDocument()
  })

  it('redirects root to dashboard', () => {
    render(
      <TestWrapper initialEntries={['/']}>
        <AppRoutesConfig />
      </TestWrapper>
    )

    expect(screen.getByText('Dashboard')).toBeInTheDocument()
  })

  it('renders login form on /login route', () => {
    render(
      <TestWrapper initialEntries={['/login']}>
        <AppRoutesConfig />
      </TestWrapper>
    )

    expect(screen.getByText('Login Form')).toBeInTheDocument()
  })
})
