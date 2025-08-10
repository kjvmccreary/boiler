import { type ReactElement } from 'react'
import { render, type RenderOptions } from '@testing-library/react'
import { BrowserRouter, MemoryRouter } from 'react-router-dom'
import { QueryClient, QueryClientProvider } from '@tanstack/react-query'
import { AuthProvider } from '@/contexts/AuthContext.js'

// Create a test query client
const createTestQueryClient = () => new QueryClient({
  defaultOptions: {
    queries: {
      retry: false,
      gcTime: 0,
    },
  },
})

// Wrapper component for tests
interface AllTheProvidersProps {
  children: React.ReactNode
  queryClient?: QueryClient
  initialEntries?: string[]
}

function AllTheProviders({ 
  children, 
  queryClient = createTestQueryClient(),
  initialEntries = ['/']
}: AllTheProvidersProps) {
  const RouterComponent = initialEntries.length > 1 || initialEntries[0] !== '/' 
    ? ({ children }: { children: React.ReactNode }) => (
        <MemoryRouter initialEntries={initialEntries}>
          {children}
        </MemoryRouter>
      )
    : BrowserRouter

  return (
    <QueryClientProvider client={queryClient}>
      <RouterComponent>
        <AuthProvider>
          {children}
        </AuthProvider>
      </RouterComponent>
    </QueryClientProvider>
  )
}

// Custom render function
const customRender = (
  ui: ReactElement,
  options?: Omit<RenderOptions, 'wrapper'> & {
    queryClient?: QueryClient
    initialEntries?: string[]
  }
) => {
  const { queryClient, initialEntries, ...renderOptions } = options || {}
  
  return render(ui, {
    wrapper: ({ children }) => (
      <AllTheProviders 
        queryClient={queryClient}
        initialEntries={initialEntries}
      >
        {children}
      </AllTheProviders>
    ),
    ...renderOptions,
  })
}

// Mock user data for tests
export const mockUsers = {
  admin: {
    id: '1',
    email: 'admin@test.com',
    firstName: 'Admin',
    lastName: 'User',
    fullName: 'Admin User',
    phoneNumber: '+1234567890',
    timeZone: 'UTC',
    language: 'en',
    lastLoginAt: '2024-01-01T00:00:00Z',
    emailConfirmed: true,
    isActive: true,
    roles: [],
    tenantId: '1',
    createdAt: '2024-01-01T00:00:00Z',
    updatedAt: '2024-01-01T00:00:00Z'
  },
  user: {
    id: '2',
    email: 'user@test.com',
    firstName: 'Regular',
    lastName: 'User',
    fullName: 'Regular User',
    phoneNumber: '+1234567890',
    timeZone: 'UTC',
    language: 'en',
    lastLoginAt: '2024-01-01T00:00:00Z',
    emailConfirmed: true,
    isActive: true,
    roles: [],
    tenantId: '1',
    createdAt: '2024-01-01T00:00:00Z',
    updatedAt: '2024-01-01T00:00:00Z'
  }
}

// Mock auth responses
export const mockAuthResponses = {
  loginSuccess: {
    accessToken: 'mock-access-token',
    refreshToken: 'mock-refresh-token',
    expiresAt: new Date(Date.now() + 3600000).toISOString(),
    tokenType: 'Bearer',
    user: mockUsers.admin,
    tenant: {
      id: '1',
      name: 'Test Tenant',
      isActive: true,
      createdAt: '2024-01-01T00:00:00Z',
      updatedAt: '2024-01-01T00:00:00Z'
    }
  },
  registerSuccess: {
    accessToken: 'mock-access-token',
    refreshToken: 'mock-refresh-token',
    expiresAt: new Date(Date.now() + 3600000).toISOString(),
    tokenType: 'Bearer',
    user: mockUsers.user,
    tenant: {
      id: '1',
      name: 'Test Tenant',
      isActive: true,
      createdAt: '2024-01-01T00:00:00Z',
      updatedAt: '2024-01-01T00:00:00Z'
    }
  }
}

// Mock permissions for tests
export const mockPermissions = {
  Users: [
    { id: 1, name: 'users.view', category: 'Users', description: 'View users', isActive: true },
    { id: 2, name: 'users.create', category: 'Users', description: 'Create users', isActive: true },
    { id: 3, name: 'users.edit', category: 'Users', description: 'Edit users', isActive: true },
    { id: 4, name: 'users.delete', category: 'Users', description: 'Delete users', isActive: true }
  ],
  Roles: [
    { id: 5, name: 'roles.view', category: 'Roles', description: 'View roles', isActive: true },
    { id: 6, name: 'roles.create', category: 'Roles', description: 'Create roles', isActive: true },
    { id: 7, name: 'roles.edit', category: 'Roles', description: 'Edit roles', isActive: true },
    { id: 8, name: 'roles.delete', category: 'Roles', description: 'Delete roles', isActive: true }
  ]
}

// Utility functions for common test scenarios
export const waitForLoadingToFinish = async () => {
  const { screen } = await import('@testing-library/react')
  try {
    await screen.findByText('loading', { exact: false }, { timeout: 100 })
  } catch {
    // Loading finished
  }
}

export const expectToastMessage = (toast: any, type: 'success' | 'error', message: string) => {
  expect(toast[type]).toHaveBeenCalledWith(message)
}

// Re-export everything from React Testing Library
export * from '@testing-library/react'

// Override render method with our custom render
export { customRender as render }
