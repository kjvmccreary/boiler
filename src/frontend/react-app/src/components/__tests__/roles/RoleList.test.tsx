import { describe, it, expect, vi, beforeEach } from 'vitest'
import { render, screen, waitFor } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import { BrowserRouter } from 'react-router-dom'
import { QueryClient, QueryClientProvider } from '@tanstack/react-query'
import { RoleList } from '@/components/roles/RoleList.js'
import { roleService } from '@/services/role.service.js'
import type { Role } from '@/types/index.js'

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

const mockRoles: Role[] = [
  {
    id: '1',
    name: 'Admin',
    description: 'Administrator role',
    isSystemRole: false,
    isDefault: false,
    tenantId: 'tenant-1',
    permissions: [
      { id: '1', name: 'users.view', category: 'Users' },
      { id: '2', name: 'users.edit', category: 'Users' }
    ],
    createdAt: '2024-01-01T00:00:00Z',
    updatedAt: '2024-01-01T00:00:00Z',
  },
  {
    id: '2',
    name: 'User',
    description: 'Standard user role',
    isSystemRole: false,
    isDefault: true,
    tenantId: 'tenant-1',
    permissions: [
      { id: '1', name: 'users.view', category: 'Users' }
    ],
    createdAt: '2024-01-01T00:00:00Z',
    updatedAt: '2024-01-01T00:00:00Z',
  },
]

describe('RoleList Navigation', () => {
  const user = userEvent.setup()

  beforeEach(() => {
    vi.clearAllMocks()
    mockNavigate.mockClear()
    vi.mocked(roleService.getRoles).mockResolvedValue(mockRoles)
  })

  it('navigates to role details when "View Details" is clicked', async () => {
    render(
      <TestWrapper>
        <RoleList />
      </TestWrapper>
    )

    await waitFor(() => {
      expect(screen.getByText('Admin')).toBeInTheDocument()
    })

    // Click the actions menu for the first role
    const menuButtons = screen.getAllByRole('button', { name: '' })
    const actionsButton = menuButtons.find(btn => 
      btn.querySelector('svg') && 
      btn.closest('tr')?.textContent?.includes('Admin')
    )
    
    if (actionsButton) {
      await user.click(actionsButton)
      
      // Click "View Details"
      const viewDetailsOption = screen.getByText('View Details')
      await user.click(viewDetailsOption)
      
      expect(mockNavigate).toHaveBeenCalledWith('/roles/1')
    }
  })

  it('navigates to role editor when "Edit Role" is clicked', async () => {
    render(
      <TestWrapper>
        <RoleList />
      </TestWrapper>
    )

    await waitFor(() => {
      expect(screen.getByText('Admin')).toBeInTheDocument()
    })

    // Click the actions menu and then edit
    const menuButtons = screen.getAllByRole('button', { name: '' })
    const actionsButton = menuButtons.find(btn => 
      btn.querySelector('svg') && 
      btn.closest('tr')?.textContent?.includes('Admin')
    )
    
    if (actionsButton) {
      await user.click(actionsButton)
      
      const editRoleOption = screen.getByText('Edit Role')
      await user.click(editRoleOption)
      
      expect(mockNavigate).toHaveBeenCalledWith('/roles/1/edit')
    }
  })

  it('navigates to create role when "Create Role" button is clicked', async () => {
    render(
      <TestWrapper>
        <RoleList />
      </TestWrapper>
    )

    await waitFor(() => {
      expect(screen.getByRole('button', { name: /create role/i })).toBeInTheDocument()
    })

    const createButton = screen.getByRole('button', { name: /create role/i })
    await user.click(createButton)

    expect(mockNavigate).toHaveBeenCalledWith('/roles/new')
  })
})
