import { describe, it, expect, vi, beforeEach } from 'vitest'
import { render, screen, waitFor } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import { BrowserRouter } from 'react-router-dom'
import { QueryClient, QueryClientProvider } from '@tanstack/react-query'
import { RoleDetails } from '@/components/roles/RoleDetails.js'
import { roleService } from '@/services/role.service.js'
import toast from 'react-hot-toast'
import type { Role, User } from '@/types/index.js'

// Mock dependencies
vi.mock('@/services/role.service.js')
vi.mock('react-hot-toast')

// Create a proper mock for useNavigate
const mockNavigate = vi.fn()
vi.mock('react-router-dom', async () => {
  const actual = await vi.importActual('react-router-dom')
  return {
    ...actual,
    useParams: () => ({ id: '1' }),
    useNavigate: () => mockNavigate,
  }
})

// Mock the auth context
vi.mock('@/contexts/AuthContext.js', () => ({
  useAuth: () => ({
    user: { id: '1', permissions: ['roles.view', 'roles.edit', 'roles.delete'] },
    isAuthenticated: true,
  }),
  AuthProvider: ({ children }: { children: React.ReactNode }) => <>{children}</>,
}))

// Mock CanAccess to always render children for testing
vi.mock('@/components/authorization/CanAccess.js', () => ({
  CanAccess: ({ children }: { children: React.ReactNode }) => <>{children}</>,
}))

// Test wrapper
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

// Mock data with correct types
const mockRole: Role = {
  id: '1',
  name: 'Admin',
  description: 'Administrator role with full access',
  isSystemRole: false,
  isDefault: false,
  tenantId: 'tenant-1',
  permissions: [
    {
      id: '1',
      name: 'users.view',
      category: 'Users',
      description: 'View users',
    },
    {
      id: '2',
      name: 'users.edit',
      category: 'Users', 
      description: 'Edit users',
    },
    {
      id: '3',
      name: 'roles.view',
      category: 'Roles',
      description: 'View roles',
    },
  ],
  createdAt: '2024-01-01T00:00:00Z',
  updatedAt: '2024-01-01T00:00:00Z',
}

const mockUsers: User[] = [
  {
    id: '1',
    firstName: 'John',
    lastName: 'Doe',
    fullName: 'John Doe',
    email: 'john@example.com',
    emailConfirmed: true,
    isActive: true,
    roles: [],
    tenantId: 'tenant-1',
    createdAt: '2024-01-01T00:00:00Z',
    updatedAt: '2024-01-01T00:00:00Z',
  },
  {
    id: '2',
    firstName: 'Jane',
    lastName: 'Smith',
    fullName: 'Jane Smith',
    email: 'jane@example.com',
    emailConfirmed: false,
    isActive: true,
    roles: [],
    tenantId: 'tenant-1',
    createdAt: '2024-01-01T00:00:00Z',
    updatedAt: '2024-01-01T00:00:00Z',
  },
]

describe('RoleDetails', () => {
  const user = userEvent.setup()

  beforeEach(() => {
    vi.clearAllMocks()
    mockNavigate.mockClear()
  })

  it('renders role information correctly', async () => {
    vi.mocked(roleService.getRoleById).mockResolvedValue(mockRole)
    vi.mocked(roleService.getRoleUsers).mockResolvedValue(mockUsers)

    render(
      <TestWrapper>
        <RoleDetails />
      </TestWrapper>
    )

    await waitFor(() => {
      expect(screen.getByText('Admin')).toBeInTheDocument()
    })

    expect(screen.getByText('Administrator role with full access')).toBeInTheDocument()
    expect(screen.getByText('3 Permissions')).toBeInTheDocument()
    expect(screen.getByText('Users (2)')).toBeInTheDocument()
  })

  it('displays permissions grouped by category', async () => {
    vi.mocked(roleService.getRoleById).mockResolvedValue(mockRole)
    vi.mocked(roleService.getRoleUsers).mockResolvedValue([])

    render(
      <TestWrapper>
        <RoleDetails />
      </TestWrapper>
    )

    await waitFor(() => {
      expect(screen.getByText('Users')).toBeInTheDocument()
    })

    expect(screen.getByText('Roles')).toBeInTheDocument()
    expect(screen.getByText('users.view')).toBeInTheDocument()
    expect(screen.getByText('users.edit')).toBeInTheDocument()
    expect(screen.getByText('roles.view')).toBeInTheDocument()
  })

  it('shows assigned users with correct status chips', async () => {
    vi.mocked(roleService.getRoleById).mockResolvedValue(mockRole)
    vi.mocked(roleService.getRoleUsers).mockResolvedValue(mockUsers)

    render(
      <TestWrapper>
        <RoleDetails />
      </TestWrapper>
    )

    await waitFor(() => {
      expect(screen.getByText('John Doe')).toBeInTheDocument()
    })

    expect(screen.getByText('Jane Smith')).toBeInTheDocument()
    expect(screen.getByText('john@example.com')).toBeInTheDocument()
    expect(screen.getByText('jane@example.com')).toBeInTheDocument()

    // Check status chips
    const activeChips = screen.getAllByText('Active')
    const pendingChips = screen.getAllByText('Pending')
    expect(activeChips).toHaveLength(1)
    expect(pendingChips).toHaveLength(1)
  })

  it('handles edit button click for non-system roles', async () => {
    vi.mocked(roleService.getRoleById).mockResolvedValue(mockRole)
    vi.mocked(roleService.getRoleUsers).mockResolvedValue([])

    render(
      <TestWrapper>
        <RoleDetails />
      </TestWrapper>
    )

    await waitFor(() => {
      expect(screen.getByRole('button', { name: /edit/i })).toBeInTheDocument()
    })

    const editButton = screen.getByRole('button', { name: /edit/i })
    await user.click(editButton)

    expect(mockNavigate).toHaveBeenCalledWith('/roles/1/edit')
  })

  it('disables edit and delete buttons for system roles', async () => {
    const systemRole: Role = { ...mockRole, isSystemRole: true }
    vi.mocked(roleService.getRoleById).mockResolvedValue(systemRole)
    vi.mocked(roleService.getRoleUsers).mockResolvedValue([])

    render(
      <TestWrapper>
        <RoleDetails />
      </TestWrapper>
    )

    await waitFor(() => {
      expect(screen.getByRole('button', { name: /edit/i })).toBeDisabled()
    })

    expect(screen.getByRole('button', { name: /delete/i })).toBeDisabled()
    expect(screen.getByText('System Role')).toBeInTheDocument()
  })

  it('handles delete role with confirmation', async () => {
    vi.mocked(roleService.getRoleById).mockResolvedValue(mockRole)
    vi.mocked(roleService.getRoleUsers).mockResolvedValue([])
    vi.mocked(roleService.deleteRole).mockResolvedValue(true)

    // Mock window.confirm
    const confirmSpy = vi.spyOn(window, 'confirm').mockReturnValue(true)

    render(
      <TestWrapper>
        <RoleDetails />
      </TestWrapper>
    )

    await waitFor(() => {
      expect(screen.getByRole('button', { name: /delete/i })).toBeInTheDocument()
    })

    const deleteButton = screen.getByRole('button', { name: /delete/i })
    await user.click(deleteButton)

    expect(confirmSpy).toHaveBeenCalledWith(
      'Are you sure you want to delete the role "Admin"? This action cannot be undone.'
    )
    expect(roleService.deleteRole).toHaveBeenCalledWith('1')
    expect(toast.success).toHaveBeenCalledWith('Role deleted successfully')
    expect(mockNavigate).toHaveBeenCalledWith('/roles')

    confirmSpy.mockRestore()
  })

  it('handles role not found error', async () => {
    vi.mocked(roleService.getRoleById).mockRejectedValue(new Error('Role not found'))

    render(
      <TestWrapper>
        <RoleDetails />
      </TestWrapper>
    )

    await waitFor(() => {
      expect(screen.getByText('Role not found')).toBeInTheDocument()
    })

    expect(screen.getByRole('button', { name: /back to roles/i })).toBeInTheDocument()
  })

  it('shows loading state initially', () => {
    vi.mocked(roleService.getRoleById).mockImplementation(
      () => new Promise(() => {}) // Never resolves
    )

    render(
      <TestWrapper>
        <RoleDetails />
      </TestWrapper>
    )

    expect(screen.getByRole('progressbar')).toBeInTheDocument()
  })

  it('handles empty permissions list', async () => {
    const roleWithoutPermissions: Role = { ...mockRole, permissions: [] }
    vi.mocked(roleService.getRoleById).mockResolvedValue(roleWithoutPermissions)
    vi.mocked(roleService.getRoleUsers).mockResolvedValue([])

    render(
      <TestWrapper>
        <RoleDetails />
      </TestWrapper>
    )

    await waitFor(() => {
      expect(screen.getByText('This role has no permissions assigned.')).toBeInTheDocument()
    })
  })

  it('handles empty users list', async () => {
    vi.mocked(roleService.getRoleById).mockResolvedValue(mockRole)
    vi.mocked(roleService.getRoleUsers).mockResolvedValue([])

    render(
      <TestWrapper>
        <RoleDetails />
      </TestWrapper>
    )

    await waitFor(() => {
      expect(screen.getByText('No users assigned to this role')).toBeInTheDocument()
    })
  })
})
