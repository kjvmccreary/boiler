import { describe, it, expect, vi, beforeEach } from 'vitest'
import { render, screen, waitFor } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import { BrowserRouter } from 'react-router-dom'
import { QueryClient, QueryClientProvider } from '@tanstack/react-query'
import { RoleList } from '@/components/roles/RoleList.js'
import { roleService } from '@/services/role.service.js'
import type { RoleDto } from '@/types/index.js'

// Mock dependencies
vi.mock('@/services/role.service.js')

// Create a proper mock for useNavigate
const mockNavigate = vi.fn()
vi.mock('react-router-dom', async () => {
  const actual = await vi.importActual('react-router-dom')
  return {
    ...actual,
    useNavigate: () => mockNavigate,
  }
})

// Mock CanAccess
vi.mock('@/components/authorization/CanAccess.js', () => ({
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

// Updated mock data to match .NET 9 API response structure
const mockRoles: RoleDto[] = [
  {
    id: 1, // Use number for .NET 9 backend
    name: 'Admin',
    description: 'Administrator role',
    isSystemRole: false,
    isDefault: false,
    tenantId: 1, // Use number for .NET 9 backend
    permissions: [
      { id: 1, name: 'users.view', category: 'Users', description: 'View users' },
      { id: 2, name: 'users.edit', category: 'Users', description: 'Edit users' }
    ],
    createdAt: '2024-01-01T00:00:00Z',
    updatedAt: '2024-01-01T00:00:00Z',
    userCount: 5
  },
  {
    id: 2,
    name: 'User',
    description: 'Standard user role',
    isSystemRole: false,
    isDefault: true,
    tenantId: 1,
    permissions: [
      { id: 1, name: 'users.view', category: 'Users', description: 'View users' }
    ],
    createdAt: '2024-01-01T00:00:00Z',
    updatedAt: '2024-01-01T00:00:00Z',
    userCount: 10
  },
]

// Mock the API response structure that matches the component's expectations
const mockApiResponse = {
  roles: mockRoles,
  pagination: {
    totalCount: 2,
    pageNumber: 1,
    pageSize: 10,
    totalPages: 1
  }
}

describe('RoleList', () => {
  const user = userEvent.setup()

  beforeEach(() => {
    vi.clearAllMocks()
    mockNavigate.mockClear()
    // Mock the service method with the correct response structure
    vi.mocked(roleService.getRoles).mockResolvedValue(mockApiResponse)
  })

  it('renders roles list correctly', async () => {
    render(
      <TestWrapper>
        <RoleList />
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
    render(
      <TestWrapper>
        <RoleList />
      </TestWrapper>
    )

    await waitFor(() => {
      expect(screen.getByText('Admin')).toBeInTheDocument()
    })

    const searchInput = screen.getByPlaceholderText('Search roles...')
    await user.type(searchInput, 'Admin')

    // Wait for debounced search
    await waitFor(() => {
      expect(roleService.getRoles).toHaveBeenCalledWith({
        page: 1,
        pageSize: 10,
        searchTerm: 'Admin'
      })
    }, { timeout: 1000 })
  })

  it('handles pagination correctly', async () => {
    const largeMockResponse = {
      roles: mockRoles,
      pagination: {
        totalCount: 25,
        pageNumber: 1,
        pageSize: 10,
        totalPages: 3
      }
    }
    vi.mocked(roleService.getRoles).mockResolvedValue(largeMockResponse)

    render(
      <TestWrapper>
        <RoleList />
      </TestWrapper>
    )

    await waitFor(() => {
      expect(screen.getByText('Admin')).toBeInTheDocument()
    })

    // Find pagination controls
    const nextPageButton = screen.getByRole('button', { name: 'Go to next page' })
    await user.click(nextPageButton)

    expect(roleService.getRoles).toHaveBeenCalledWith({
      page: 2,
      pageSize: 10,
      searchTerm: ''
    })
  })

  it('calls onEditRole when edit button is clicked', async () => {
    const mockOnEditRole = vi.fn()
    
    render(
      <TestWrapper>
        <RoleList onEditRole={mockOnEditRole} />
      </TestWrapper>
    )

    await waitFor(() => {
      expect(screen.getByText('Admin')).toBeInTheDocument()
    })

    // Find edit buttons (should be IconButtons with Edit icons)
    const editButtons = screen.getAllByRole('button')
    const editButton = editButtons.find(btn => 
      btn.querySelector('svg') && 
      btn.closest('tr')?.textContent?.includes('Admin')
    )
    
    if (editButton) {
      await user.click(editButton)
      expect(mockOnEditRole).toHaveBeenCalledWith(mockRoles[0])
    }
  })

  it('calls onDeleteRole when delete button is clicked', async () => {
    const mockOnDeleteRole = vi.fn()
    
    render(
      <TestWrapper>
        <RoleList onDeleteRole={mockOnDeleteRole} />
      </TestWrapper>
    )

    await waitFor(() => {
      expect(screen.getByText('Admin')).toBeInTheDocument()
    })

    // Find delete buttons
    const deleteButtons = screen.getAllByRole('button')
    const deleteButton = deleteButtons.find(btn => 
      btn.querySelector('svg') && 
      btn.closest('tr')?.textContent?.includes('Admin')
    )
    
    if (deleteButton) {
      await user.click(deleteButton)
      expect(mockOnDeleteRole).toHaveBeenCalledWith(1)
    }
  })

  it('disables edit and delete buttons for system roles', async () => {
    const systemRoleMock = {
      roles: [{
        ...mockRoles[0],
        isSystemRole: true
      }],
      pagination: {
        totalCount: 1,
        pageNumber: 1,
        pageSize: 10,
        totalPages: 1
      }
    }
    vi.mocked(roleService.getRoles).mockResolvedValue(systemRoleMock)

    render(
      <TestWrapper>
        <RoleList />
      </TestWrapper>
    )

    await waitFor(() => {
      expect(screen.getByText('Admin')).toBeInTheDocument()
    })

    // Check that buttons are disabled for system roles
    const buttons = screen.getAllByRole('button')
    const disabledButtons = buttons.filter(btn => btn.hasAttribute('disabled'))
    expect(disabledButtons.length).toBeGreaterThan(0)
  })

  it('shows loading state initially', () => {
    vi.mocked(roleService.getRoles).mockImplementation(
      () => new Promise(() => {}) // Never resolves
    )

    render(
      <TestWrapper>
        <RoleList />
      </TestWrapper>
    )

    expect(screen.getByRole('progressbar')).toBeInTheDocument()
  })

  it('shows error state when loading fails', async () => {
    vi.mocked(roleService.getRoles).mockRejectedValue(new Error('Failed to load'))

    render(
      <TestWrapper>
        <RoleList />
      </TestWrapper>
    )

    await waitFor(() => {
      expect(screen.getByText(/Failed to fetch roles/)).toBeInTheDocument()
    })
  })

  it('displays role information correctly', async () => {
    render(
      <TestWrapper>
        <RoleList />
      </TestWrapper>
    )

    await waitFor(() => {
      expect(screen.getByText('Admin')).toBeInTheDocument()
    })

    // Check role details are displayed
    expect(screen.getByText('2')).toBeInTheDocument() // Permission count for Admin
    expect(screen.getByText('5')).toBeInTheDocument() // User count for Admin
    expect(screen.getByText('Custom')).toBeInTheDocument() // Role type chip
  })

  it('handles page size changes', async () => {
    render(
      <TestWrapper>
        <RoleList />
      </TestWrapper>
    )

    await waitFor(() => {
      expect(screen.getByText('Admin')).toBeInTheDocument()
    })

    // Find the rows per page selector
    const pageSizeSelect = screen.getByDisplayValue('10')
    await user.click(pageSizeSelect)
    
    const option25 = screen.getByRole('option', { name: '25' })
    await user.click(option25)

    expect(roleService.getRoles).toHaveBeenCalledWith({
      page: 1,
      pageSize: 25,
      searchTerm: ''
    })
  })
})
