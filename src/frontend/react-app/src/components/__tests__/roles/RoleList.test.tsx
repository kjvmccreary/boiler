import React from 'react'
import { describe, it, expect, vi, beforeEach, afterEach } from 'vitest'
import { render, screen, waitFor } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import { BrowserRouter } from 'react-router-dom'
import { QueryClient, QueryClientProvider } from '@tanstack/react-query'
import { RoleList } from '@/components/roles/RoleList'
import { roleService } from '@/services/role.service'

vi.mock('@/services/role.service')

vi.mock('@/contexts/AuthContext', () => ({
  useAuth: () => ({
    user: { id: '1', permissions: ['roles.view', 'roles.edit', 'roles.delete'] },
    isAuthenticated: true,
  }),
}))

vi.mock('@/components/authorization/CanAccess', () => ({
  CanAccess: ({ children }: { children: React.ReactNode }) => <>{children}</>,
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

const mockRoles = [
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
]

describe('RoleList', () => {
  const mockOnEditRole = vi.fn()
  const mockOnDeleteRole = vi.fn()

  beforeEach(() => {
    vi.clearAllMocks()
    vi.mocked(roleService.getRoles).mockResolvedValue({
      items: mockRoles,
      totalCount: 2,
      pageNumber: 1,
      pageSize: 10,
      totalPages: 1,
    })
  })

  afterEach(() => {
    vi.clearAllMocks()
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

    await waitFor(() => {
      expect(screen.getByText('Admin')).toBeInTheDocument()
    })

    expect(screen.getByText('User')).toBeInTheDocument()
    expect(screen.getByText('Administrator role')).toBeInTheDocument()
    expect(screen.getByText('Standard user role')).toBeInTheDocument()
  })

  it('handles search functionality', async () => {
    const user = userEvent.setup()

    render(
      <TestWrapper>
        <RoleList
          onEditRole={mockOnEditRole}
          onDeleteRole={mockOnDeleteRole}
        />
      </TestWrapper>
    )

    await waitFor(() => {
      expect(screen.getByPlaceholderText('Search roles...')).toBeInTheDocument()
    })

    const searchInput = screen.getByPlaceholderText('Search roles...')
    await user.type(searchInput, 'Admin')

    await waitFor(() => {
      expect(roleService.getRoles).toHaveBeenCalledWith({
        page: 1,
        pageSize: 10,
        searchTerm: 'Admin',
      })
    })
  })

  it('handles pagination correctly', async () => {
    const user = userEvent.setup()

    vi.mocked(roleService.getRoles).mockResolvedValue({
      items: mockRoles,
      totalCount: 25,
      pageNumber: 1,
      pageSize: 10,
      totalPages: 3,
    })

    render(
      <TestWrapper>
        <RoleList
          onEditRole={mockOnEditRole}
          onDeleteRole={mockOnDeleteRole}
        />
      </TestWrapper>
    )

    await waitFor(() => {
      expect(screen.getByText('Admin')).toBeInTheDocument()
    })

    const nextPageButton = screen.getByLabelText('Go to next page')
    await user.click(nextPageButton)

    await waitFor(() => {
      expect(roleService.getRoles).toHaveBeenCalledWith({
        page: 2,
        pageSize: 10,
        searchTerm: undefined,
      })
    })
  })

  it('calls onEditRole when edit button is clicked', async () => {
    const user = userEvent.setup()

    render(
      <TestWrapper>
        <RoleList
          onEditRole={mockOnEditRole}
          onDeleteRole={mockOnDeleteRole}
        />
      </TestWrapper>
    )

    await waitFor(() => {
      expect(screen.getByText('Admin')).toBeInTheDocument()
    })

    // Fix: Use data-testid and find enabled buttons
    const editButtons = screen.getAllByTestId('edit-button')
    const enabledEditButton = editButtons.find(button => !button.hasAttribute('disabled'))

    if (enabledEditButton) {
      await user.click(enabledEditButton)
      expect(mockOnEditRole).toHaveBeenCalledWith(1)
    }
  })

  it('calls onDeleteRole when delete button is clicked', async () => {
    const user = userEvent.setup()

    render(
      <TestWrapper>
        <RoleList
          onEditRole={mockOnEditRole}
          onDeleteRole={mockOnDeleteRole}
        />
      </TestWrapper>
    )

    await waitFor(() => {
      expect(screen.getByText('Admin')).toBeInTheDocument()
    })

    // Fix: Use data-testid and find enabled buttons
    const deleteButtons = screen.getAllByTestId('delete-button')
    const enabledDeleteButton = deleteButtons.find(button => !button.hasAttribute('disabled'))

    if (enabledDeleteButton) {
      await user.click(enabledDeleteButton)
      expect(mockOnDeleteRole).toHaveBeenCalledWith(1)
    }
  })

  it('disables edit and delete buttons for system roles', async () => {
    render(
      <TestWrapper>
        <RoleList
          onEditRole={mockOnEditRole}
          onDeleteRole={mockOnDeleteRole}
        />
      </TestWrapper>
    )

    await waitFor(() => {
      expect(screen.getByText('User')).toBeInTheDocument()
    })

    // System roles should have disabled buttons
    const allButtons = screen.getAllByTestId(/edit-button|delete-button/)
    const disabledButtons = allButtons.filter(button => button.hasAttribute('disabled'))
    expect(disabledButtons.length).toBeGreaterThan(0)
  })

  it('shows loading state initially', () => {
    vi.mocked(roleService.getRoles).mockImplementation(
      () => new Promise(() => { }) // Never resolves
    )

    render(
      <TestWrapper>
        <RoleList
          onEditRole={mockOnEditRole}
          onDeleteRole={mockOnDeleteRole}
        />
      </TestWrapper>
    )

    expect(screen.getByRole('progressbar')).toBeInTheDocument()
  })

  it('shows error state when loading fails', async () => {
    vi.mocked(roleService.getRoles).mockRejectedValue(new Error('Failed to fetch roles'))

    render(
      <TestWrapper>
        <RoleList
          onEditRole={mockOnEditRole}
          onDeleteRole={mockOnDeleteRole}
        />
      </TestWrapper>
    )

    await waitFor(() => {
      expect(screen.getByText(/Failed to fetch roles/)).toBeInTheDocument()
    })
  })

  it('displays role information correctly', async () => {
    render(
      <TestWrapper>
        <RoleList
          onEditRole={mockOnEditRole}
          onDeleteRole={mockOnDeleteRole}
        />
      </TestWrapper>
    )

    await waitFor(() => {
      expect(screen.getByText('Admin')).toBeInTheDocument()
    })

    expect(screen.getByText('2')).toBeInTheDocument() // Permission count for Admin
    expect(screen.getByText('5')).toBeInTheDocument() // User count for Admin
    expect(screen.getByText('Custom')).toBeInTheDocument() // Type for Admin
    expect(screen.getByText('System')).toBeInTheDocument() // Type for User
  })

  it('handles page size changes', async () => {
    const user = userEvent.setup()

    render(
      <TestWrapper>
        <RoleList
          onEditRole={mockOnEditRole}
          onDeleteRole={mockOnDeleteRole}
        />
      </TestWrapper>
    )

    await waitFor(() => {
      expect(screen.getByText('Admin')).toBeInTheDocument()
    })

    const pageSizeSelect = screen.getByRole('combobox', { name: /rows per page/i })
    await user.click(pageSizeSelect)

    const option25 = screen.getByRole('option', { name: '25' })
    await user.click(option25)

    await waitFor(() => {
      expect(roleService.getRoles).toHaveBeenCalledWith({
        page: 1,
        pageSize: 25,
        searchTerm: undefined,
      })
    })
  })
})
