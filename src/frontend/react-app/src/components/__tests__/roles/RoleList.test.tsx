import React from 'react'
import { describe, it, expect, vi, beforeEach } from 'vitest'
import { render, screen, waitFor } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import { BrowserRouter } from 'react-router-dom'
import { QueryClient, QueryClientProvider } from '@tanstack/react-query'
import { RoleList } from '@/components/roles/RoleList'

// ✅ FIXED: Use the correct absolute path that matches the project structure
vi.mock('@/services/role.service', () => ({
  roleService: {
    getRoles: vi.fn(),
    getRoleById: vi.fn(),
    getRoleUsers: vi.fn(),
    createRole: vi.fn(),
    updateRole: vi.fn(),
    deleteRole: vi.fn(),
    getRolePermissions: vi.fn(),
    assignRoleToUser: vi.fn(),
    removeRoleFromUser: vi.fn(),
    getUserPermissions: vi.fn(),
    getUserRoles: vi.fn(),
    getRoleByName: vi.fn()
  }
}))

vi.mock('@/services/tenant.service')

vi.mock('@/contexts/AuthContext.js', () => ({
  useAuth: () => ({
    user: { 
      id: '3', 
      firstName: 'Admin', 
      lastName: 'User', 
      email: 'admin@test.com',
      permissions: ['roles.view', 'roles.edit', 'roles.delete'] 
    },
    isAuthenticated: true,
  }),
  AuthProvider: ({ children }: any) => <div data-testid="auth-provider">{children}</div>
}))

vi.mock('@/contexts/TenantContext.js', () => ({
  useTenant: () => ({
    currentTenant: { id: '1', name: 'Test Tenant' },
    availableTenants: [{ id: '1', name: 'Test Tenant' }],
    switchTenant: vi.fn(),
    tenantSettings: { theme: { primaryColor: '#1976d2' } },
    isLoading: false,
    error: null
  }),
  TenantProvider: ({ children }: any) => <div data-testid="tenant-provider">{children}</div>
}))

vi.mock('@/components/authorization/CanAccess.js', () => ({
  CanAccess: ({ children }: { children: React.ReactNode }) => <div data-testid="can-access">{children}</div>,
}))

function TestWrapper({ children }: { children: React.ReactNode }) {
  const queryClient = new QueryClient({
    defaultOptions: {
      queries: { retry: false, gcTime: 0 },
    },
  })

  return (
    <QueryClientProvider client={queryClient}>
      <BrowserRouter>
        {children}
      </BrowserRouter>
    </QueryClientProvider>
  )
}

const mockRolesResponse = {
  roles: [
    {
      id: 1,
      name: 'Admin',
      description: 'Administrator role',
      isSystemRole: false,
      isDefault: false,
      tenantId: 1,
      permissions: [
        { id: '1', name: 'users.view', category: 'Users', description: 'View users' },
        { id: '2', name: 'users.edit', category: 'Users', description: 'Edit users' },
      ],
      userCount: 5,
      createdAt: '2024-01-01T00:00:00Z',
      updatedAt: '2024-01-01T00:00:00Z',
    },
    {
      id: 2,
      name: 'User',
      description: 'Standard user role',
      isSystemRole: true,
      isDefault: true,
      tenantId: 1,
      permissions: [
        { id: '1', name: 'users.view', category: 'Users', description: 'View users' },
      ],
      userCount: 10,
      createdAt: '2024-01-01T00:00:00Z',
      updatedAt: '2024-01-01T00:00:00Z',
    },
  ],
  pagination: {
    totalCount: 2,
    pageNumber: 1,
    pageSize: 10,
    totalPages: 1,
  }
}

describe('RoleList', () => {
  const mockOnEditRole = vi.fn()
  const mockOnDeleteRole = vi.fn()

  beforeEach(async () => {
    localStorage.clear()
    vi.clearAllMocks()
    
    // ✅ FIXED: Import using the correct absolute path
    const roleServiceModule = await import('@/services/role.service')
    const mockRoleService = roleServiceModule.roleService as any
    
    // Setup the mock
    mockRoleService.getRoles.mockResolvedValue(mockRolesResponse)
    mockRoleService.getRoleUsers.mockResolvedValue([
      { id: 1, firstName: 'John', lastName: 'Doe', email: 'john@test.com', fullName: 'John Doe', isActive: true }
    ])
  })

  it('renders roles list correctly', async () => {
    render(
      <TestWrapper>
        <RoleList
          onEditRole={mockOnEditRole}
          onDeleteRole={mockOnDeleteRole}
        />
      </TestWrapper>
    )

    // ✅ Wait for the roles to load and render
    await waitFor(() => {
      expect(screen.getByText('Admin')).toBeInTheDocument()
    }, { timeout: 5000 })

    expect(screen.getByText('User')).toBeInTheDocument()
    expect(screen.getByText('Administrator role')).toBeInTheDocument()
    expect(screen.getByText('Standard user role')).toBeInTheDocument()
  })

  it('shows loading state initially', async () => {
    // ✅ Mock a never-resolving promise to test loading state
    const roleServiceModule = await import('@/services/role.service')
    const mockRoleService = roleServiceModule.roleService as any
    mockRoleService.getRoles.mockImplementation(() => new Promise(() => {}))

    render(
      <TestWrapper>
        <RoleList
          onEditRole={mockOnEditRole}
          onDeleteRole={mockOnDeleteRole}
        />
      </TestWrapper>
    )

    expect(screen.getByRole('progressbar')).toBeInTheDocument()
    expect(screen.getByText('Loading roles for Test Tenant...')).toBeInTheDocument()
  })
})
