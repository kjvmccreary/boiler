import { describe, it, expect, vi, beforeEach } from 'vitest'
import { render, screen, waitFor, cleanup } from '@testing-library/react'
import { TenantProvider, useTenant } from '../TenantContext.tsx'
import { AuthProvider } from '../AuthContext.tsx'
import type { User } from '@/types/index.ts' // ✅ Import User type

// ✅ FIX: Import and mock services properly
import { tenantService } from '@/services/tenant.service.ts'
import { tokenManager } from '@/utils/token.manager.ts'

vi.mock('@/services/tenant.service.ts', () => ({
  tenantService: {
    getUserTenants: vi.fn(),
    getTenantSettings: vi.fn(),
    selectTenant: vi.fn(),
    switchTenant: vi.fn()
  }
}))

vi.mock('@/utils/token.manager.ts', () => ({
  tokenManager: {
    setTokens: vi.fn(),
    getTokenClaims: vi.fn().mockReturnValue(null),
    clearTokens: vi.fn(),
    getToken: vi.fn(),
    getRefreshToken: vi.fn()
  }
}))

vi.mock('@/contexts/AuthContext.tsx', () => ({
  useAuth: () => ({
    user: { id: '3', firstName: 'Admin', lastName: 'User', email: 'admin@test.com' },
    isAuthenticated: true,
    refreshAuth: vi.fn().mockResolvedValue(true)
  }),
  AuthProvider: ({ children, testMode }: any) => <div data-testid="auth-provider">{children}</div>
}))

vi.mock('@/services/api.client.ts', () => ({
  apiClient: {
    setCurrentTenant: vi.fn(),
  }
}))

// ✅ FIX: Create mock references
const mockTenantService = vi.mocked(tenantService)
const mockTokenManager = vi.mocked(tokenManager)

// ✅ FIX: Define mock data
const mockTenants = [
  {
    id: '1',
    name: 'Test Tenant',
    domain: 'test.local',
    subscriptionPlan: 'Development',
    isActive: true,
    createdAt: new Date().toISOString(),
    updatedAt: new Date().toISOString(),
  }
]

// ✅ FIX: Complete mockUser object with all required User properties
const mockUser: User = {
  id: '3',
  email: 'admin@test.com',
  firstName: 'Admin',
  lastName: 'User',
  fullName: 'Admin User', // ✅ REQUIRED
  phoneNumber: undefined,
  timeZone: 'UTC',
  language: 'en',
  lastLoginAt: undefined,
  emailConfirmed: true, // ✅ REQUIRED
  isActive: true, // ✅ REQUIRED
  roles: ['Admin'], // ✅ REQUIRED
  tenantId: '1', // ✅ REQUIRED
  createdAt: new Date().toISOString(), // ✅ REQUIRED
  updatedAt: new Date().toISOString(), // ✅ REQUIRED
  preferences: { // ✅ REQUIRED (optional but good to include)
    theme: 'light',
    language: 'en',
    timeZone: 'UTC',
    notifications: {
      email: true,
      push: false,
      sms: false
    }
  }
}

// ✅ FIX: Simplified TestComponent that only renders once
function TestComponent() {
  const { 
    currentTenant, 
    availableTenants, 
    isLoading, 
    error,
    switchTenant,
    completeTenantSelection,
    showTenantSelector,
    shouldRedirectToDashboard 
  } = useTenant()
  
  if (isLoading) return <div>Loading tenants...</div>
  if (error) return <div data-testid="error">{error}</div>
  
  return (
    <div>
      <div data-testid="current-tenant">
        {currentTenant ? currentTenant.name : 'No tenant'}
      </div>
      <div data-testid="available-count">
        {availableTenants.length}
      </div>
      <div data-testid="show-selector">
        {showTenantSelector ? 'true' : 'false'}
      </div>
      <div data-testid="should-redirect">
        {shouldRedirectToDashboard ? 'true' : 'false'}
      </div>
      {showTenantSelector && <div data-testid="tenant-selector">Selector shown</div>}
      <button 
        data-testid="switch-tenant" 
        onClick={() => switchTenant('2')}
      >
        Switch Tenant
      </button>
      <button 
        data-testid="complete-selection" 
        onClick={() => completeTenantSelection('1')}
      >
        Complete Selection
      </button>
    </div>
  )
}

// ✅ FIX: Simplified TestWrapper that doesn't duplicate components
const TestWrapper = ({ 
  mockUserTenants = mockTenants,
  mockError = null,
  mockLoading = false 
}: { 
  mockUserTenants?: any[]
  mockError?: string | null
  mockLoading?: boolean
}) => {
  // ✅ FIX: Enhanced mock service setup
  const enhancedMockTenantService = {
    ...mockTenantService,
    getUserTenants: vi.fn().mockImplementation(() => {
      if (mockError) {
        return Promise.reject(new Error(mockError))
      }
      if (mockLoading) {
        return new Promise(() => {}) // Never resolves to simulate loading
      }
      return Promise.resolve({
        success: true,
        data: mockUserTenants,
        message: 'Tenants loaded successfully'
      })
    })
  }

  // Override the mock before rendering
  mockTenantService.getUserTenants.mockImplementation(
    enhancedMockTenantService.getUserTenants
  )

  return (
    <AuthProvider mockUser={mockUser} mockAuthState="authenticated" testMode>
      <TenantProvider>
        <TestComponent />
      </TenantProvider>
    </AuthProvider>
  )
}

// ✅ FIX: Enhanced mock data for different scenarios
const mockSingleTenant = [
  {
    id: '1',
    name: 'Test Tenant',
    domain: 'test.local',
    subscriptionPlan: 'Development',
    isActive: true,
    createdAt: new Date().toISOString(),
    updatedAt: new Date().toISOString(),
  }
]

const mockMultipleTenants = [
  {
    id: '1',
    name: 'Test Tenant 1',
    domain: 'test1.local',
    subscriptionPlan: 'Development',
    isActive: true,
    createdAt: new Date().toISOString(),
    updatedAt: new Date().toISOString(),
  },
  {
    id: '2',
    name: 'Test Tenant 2',
    domain: 'test2.local',
    subscriptionPlan: 'Premium',
    isActive: true,
    createdAt: new Date().toISOString(),
    updatedAt: new Date().toISOString(),
  }
]

describe('TenantContext - Enhanced Tests', () => {
  beforeEach(() => {
    cleanup() // ✅ FIX: Add cleanup between tests
    localStorage.clear()
    sessionStorage.clear()
    vi.clearAllMocks()
  })

  describe('Single Tenant Auto-Selection', () => {
    beforeEach(() => {
      mockTenantService.getUserTenants.mockResolvedValue({
        success: true,
        data: mockSingleTenant
      })
      
      mockTenantService.selectTenant.mockResolvedValue({
        success: true,
        message: 'Tenant selected successfully',
        data: {
          accessToken: 'valid.jwt.token',
          refreshToken: 'valid.refresh.token',
          user: { id: '3', email: 'admin@test.com' },
          tenant: mockSingleTenant[0]
        }
      })
    })

    it('should load and auto-select single tenant', async () => {
      render(<TestWrapper />)
      
      await waitFor(() => {
        expect(screen.getByTestId('current-tenant')).toHaveTextContent('Test Tenant')
      }, { timeout: 8000 })
      
      expect(mockTenantService.getUserTenants).toHaveBeenCalled()
      expect(screen.getByTestId('available-count')).toHaveTextContent('1')
    }, 15000)

    it('should call selectTenant API for single tenant', async () => {
      render(<TestWrapper />)
      
      await waitFor(() => {
        expect(mockTenantService.selectTenant).toHaveBeenCalledWith('1')
      }, { timeout: 8000 })
    }, 15000)
  })

  describe('Multiple Tenants', () => {
    beforeEach(() => {
      mockTenantService.getUserTenants.mockResolvedValue({
        success: true,
        data: mockMultipleTenants
      })
    })

    it('should show tenant selector for multiple tenants', async () => {
      render(<TestWrapper mockUserTenants={mockMultipleTenants} />)
      
      await waitFor(() => {
        expect(screen.getByTestId('available-count')).toHaveTextContent('2')
        expect(screen.getByTestId('show-selector')).toHaveTextContent('true')
      }, { timeout: 8000 })
      
      expect(mockTenantService.selectTenant).not.toHaveBeenCalled()
    }, 15000)

    it('should complete tenant selection for multiple tenants', async () => {
      mockTenantService.selectTenant.mockResolvedValue({
        success: true,
        data: {
          accessToken: 'selected.jwt.token',
          refreshToken: 'selected.refresh.token',
          user: { id: '3', email: 'admin@test.com' },
          tenant: mockMultipleTenants[0]
        }
      })

      render(<TestWrapper mockUserTenants={mockMultipleTenants} />)
      
      await waitFor(() => {
        expect(screen.getByTestId('available-count')).toHaveTextContent('2')
      })

      // Complete selection
      const completeButton = screen.getByTestId('complete-selection')
      completeButton.click()

      await waitFor(() => {
        expect(mockTenantService.selectTenant).toHaveBeenCalledWith('1')
      })
    }, 15000)
  })

  describe('Tenant Switching', () => {
    beforeEach(() => {
      mockTenantService.getUserTenants.mockResolvedValue({
        success: true,
        data: mockMultipleTenants
      })

      mockTenantService.switchTenant.mockResolvedValue({
        success: true,
        data: {
          accessToken: 'switched.jwt.token',
          refreshToken: 'switched.refresh.token',
          user: { id: '3', email: 'admin@test.com' },
          tenant: mockMultipleTenants[1]
        }
      })
    })

    it('should switch between tenants', async () => {
      render(<TestWrapper mockUserTenants={mockMultipleTenants} />)
      
      await waitFor(() => {
        expect(screen.getByTestId('available-count')).toHaveTextContent('2')
      })

      // Switch tenant
      const switchButton = screen.getByTestId('switch-tenant')
      switchButton.click()

      await waitFor(() => {
        expect(mockTenantService.switchTenant).toHaveBeenCalledWith('2')
      })
    }, 15000)

    it('should set redirect flag after successful switch', async () => {
      render(<TestWrapper mockUserTenants={mockMultipleTenants} />)
      
      await waitFor(() => {
        expect(screen.getByTestId('available-count')).toHaveTextContent('2')
      })

      const switchButton = screen.getByTestId('switch-tenant')
      switchButton.click()

      await waitFor(() => {
        expect(screen.getByTestId('should-redirect')).toHaveTextContent('true')
      })
    }, 15000)
  })

  describe('Error Handling', () => {
    it('should handle getUserTenants API failure', async () => {
      render(<TestWrapper mockError="Failed to load tenants" />)
      
      await waitFor(() => {
        expect(screen.getByTestId('error')).toHaveTextContent('Failed to load tenants')
      }, { timeout: 8000 })
    }, 15000)

    it('should handle selectTenant API failure', async () => {
      mockTenantService.selectTenant.mockRejectedValue(
        new Error('Failed to select tenant')
      )

      render(<TestWrapper />)
      
      await waitFor(() => {
        expect(screen.getByTestId('error')).toHaveTextContent('Failed to select tenant')
      }, { timeout: 8000 })
    }, 15000)

    it('should handle switchTenant API failure', async () => {
      mockTenantService.switchTenant.mockRejectedValue(
        new Error('Failed to switch tenant')
      )

      render(<TestWrapper mockUserTenants={mockMultipleTenants} />)
      
      await waitFor(() => {
        expect(screen.getByTestId('available-count')).toHaveTextContent('2')
      })

      const switchButton = screen.getByTestId('switch-tenant')
      switchButton.click()

      await waitFor(() => {
        expect(screen.getByTestId('error')).toHaveTextContent('Failed to switch tenant')
      })
    }, 15000)
  })

  describe('Loading States', () => {
    it('should show loading state while fetching tenants', async () => {
      // Mock a slow response
      mockTenantService.getUserTenants.mockImplementation(
        () => new Promise(resolve => setTimeout(() => resolve({
          success: true,
          data: mockSingleTenant
        }), 1000))
      )

      render(<TestWrapper />)
      
      // Should show loading initially
      expect(screen.getAllByText('Loading tenants...')[0]).toBeInTheDocument()
      
      // Then should show tenant after loading
      await waitFor(() => {
        expect(screen.getByTestId('current-tenant')).toHaveTextContent('Test Tenant')
      }, { timeout: 8000 })
    }, 15000)
  })

  describe('JWT Token Integration', () => {
    it('should handle page refresh with existing JWT tenant', async () => {
      // Mock JWT with tenant_id already present (page refresh scenario)
      mockTokenManager.getTokenClaims.mockReturnValue({
        tenant_id: '1',
        nameid: '3',
        email: 'admin@test.com'
      })
      
      localStorage.setItem('auth_token', 'existing.jwt.token')

      render(<TestWrapper mockUserTenants={mockMultipleTenants} />)
      
      await waitFor(() => {
        expect(screen.getByTestId('available-count')).toHaveTextContent('2')
      })

      // Should not call selectTenant for page refresh
      expect(mockTenantService.selectTenant).not.toHaveBeenCalled()
    }, 15000)
  })

  describe('No Tenants Scenario', () => {
    it('should handle user with no tenant access', async () => {
      render(<TestWrapper mockUserTenants={[]} />)
      
      await waitFor(() => {
        expect(screen.getByTestId('available-count')).toHaveTextContent('0')
        expect(screen.getByTestId('error')).toHaveTextContent('No tenants found for user')
      }, { timeout: 8000 })
    }, 15000)
  })
})
