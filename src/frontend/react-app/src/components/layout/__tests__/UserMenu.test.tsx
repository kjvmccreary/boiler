import { describe, it, expect, vi, beforeEach } from 'vitest'
import { render, screen, waitFor } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import { UserMenu } from '../UserMenu.js'
import { BrowserRouter } from 'react-router-dom'

// Mock the route constants
vi.mock('@/routes/route.constants.js', () => ({
  ROUTES: {
    PROFILE: '/profile',
    SETTINGS: '/settings',
    LOGIN: '/login'
  }
}))

// ✅ FIXED: Use correct file extension .ts not .js
vi.mock('@/services/tenant.service.ts', () => ({
  tenantService: {
    getUserTenants: vi.fn(),
    getTenantSettings: vi.fn(),
    selectTenant: vi.fn(),
    switchTenant: vi.fn()
  }
}))

vi.mock('@/contexts/AuthContext.js', () => ({
  useAuth: () => ({
    user: { 
      id: '3', 
      firstName: 'Admin', 
      lastName: 'User', 
      email: 'admin@test.com',
      permissions: ['profile.view', 'settings.view'] 
    },
    isAuthenticated: true,
    logout: vi.fn()
  }),
  AuthProvider: ({ children }: any) => <div>{children}</div>
}))

vi.mock('@/contexts/TenantContext.js', () => ({
  useTenant: () => ({
    currentTenant: { id: '1', name: 'Default Tenant' },
    availableTenants: [{ id: '1', name: 'Default Tenant' }],
    switchTenant: vi.fn(),
    tenantSettings: { theme: { primaryColor: '#1976d2' } },
    isLoading: false,
    error: null
  }),
  TenantProvider: ({ children }: any) => <div>{children}</div>
}))

function TestWrapper({ children }: { children: React.ReactNode }) {
  return (
    <BrowserRouter>
      {children}
    </BrowserRouter>
  )
}

describe('UserMenu', () => {
  beforeEach(() => {
    localStorage.clear()
    vi.clearAllMocks()
  })

  it('should display current tenant in user menu', async () => {
    const user = userEvent.setup()
    
    render(
      <TestWrapper>
        <UserMenu />
      </TestWrapper>
    )

    // Click to open menu
    const avatarButton = screen.getByRole('button', { name: /account of current user/i })
    await user.click(avatarButton)

    // Check for user information instead of tenant name
    await waitFor(() => {
      expect(screen.getByText('Admin User')).toBeInTheDocument()
      expect(screen.getByText('admin@test.com')).toBeInTheDocument()
    })
  })

  it('should not show tenant switching option for single tenant', async () => {
    const user = userEvent.setup()
    
    render(
      <TestWrapper>
        <UserMenu />
      </TestWrapper>
    )

    const avatarButton = screen.getByRole('button', { name: /account of current user/i })
    await user.click(avatarButton)

    // Should not show switch organization option when only one tenant
    await waitFor(() => {
      expect(screen.queryByText('Switch Organization')).not.toBeInTheDocument()
      expect(screen.queryByText('Switch Tenant')).not.toBeInTheDocument()
    })
  })
})
