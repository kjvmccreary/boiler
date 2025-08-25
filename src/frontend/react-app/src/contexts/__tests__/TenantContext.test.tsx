import { describe, it, expect, vi, beforeEach } from 'vitest'
import { render, screen, waitFor } from '@testing-library/react'
import { TenantProvider, useTenant } from '../TenantContext.js'
import { AuthProvider } from '../AuthContext.js'

vi.mock('@/services/tenant.service', () => ({
  tenantService: {
    getUserTenants: vi.fn(),
    getTenantSettings: vi.fn(),
    selectTenant: vi.fn(),
    switchTenant: vi.fn()
  }
}))

// ✅ FIXED: Mock utils that are causing token decode errors
vi.mock('@/utils/token.manager', () => ({
  tokenManager: {
    setTokens: vi.fn(),
    getTokenClaims: vi.fn().mockReturnValue(null),
    clearTokens: vi.fn(),
    getToken: vi.fn(),
    getRefreshToken: vi.fn()
  }
}))

// ✅ FIXED: Add refreshAuth function to AuthContext mock
vi.mock('@/contexts/AuthContext.js', () => ({
  useAuth: () => ({
    user: { id: '3', firstName: 'Admin', lastName: 'User', email: 'admin@test.com' },
    isAuthenticated: true,
    refreshAuth: vi.fn().mockResolvedValue(true) // ✅ ADD: Missing refreshAuth function
  }),
  AuthProvider: ({ children, testMode }: any) => <div data-testid="auth-provider">{children}</div>
}))

vi.mock('@/services/api.client', () => ({
  apiClient: {
    setCurrentTenant: vi.fn(),
  }
}))

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

function TestWrapper() {
  return (
    <AuthProvider testMode>
      <TenantProvider>
        <TestComponent />
      </TenantProvider>
    </AuthProvider>
  )
}

// ✅ ENHANCED: Mock data for different scenarios
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
  let mockTenantService: any

  beforeEach(async () => {
    localStorage.clear()
    sessionStorage.clear()
    vi.clearAllMocks()
    
    // ✅ FIXED: Get the mocked service
    const tenantModule = await import('@/services/tenant.service')
    mockTenantService = tenantModule.tenantService
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
      render(<TestWrapper />)
      
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

      render(<TestWrapper />)
      
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
      render(<TestWrapper />)
      
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
      render(<TestWrapper />)
      
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
      mockTenantService.getUserTenants.mockRejectedValue(
        new Error('Failed to load tenants')
      )

      render(<TestWrapper />)
      
      await waitFor(() => {
        expect(screen.getByTestId('error')).toHaveTextContent('Failed to load tenants')
      }, { timeout: 8000 })
    }, 15000)

    it('should handle selectTenant API failure', async () => {
      mockTenantService.getUserTenants.mockResolvedValue({
        success: true,
        data: mockSingleTenant
      })

      mockTenantService.selectTenant.mockRejectedValue(
        new Error('Failed to select tenant')
      )

      render(<TestWrapper />)
      
      await waitFor(() => {
        expect(screen.getByTestId('error')).toHaveTextContent('Failed to select tenant')
      }, { timeout: 8000 })
    }, 15000)

    it('should handle switchTenant API failure', async () => {
      mockTenantService.getUserTenants.mockResolvedValue({
        success: true,
        data: mockMultipleTenants
      })

      mockTenantService.switchTenant.mockRejectedValue(
        new Error('Failed to switch tenant')
      )

      render(<TestWrapper />)
      
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
      expect(screen.getByText('Loading tenants...')).toBeInTheDocument()
      
      // Then should show tenant after loading
      await waitFor(() => {
        expect(screen.getByTestId('current-tenant')).toHaveTextContent('Test Tenant')
      }, { timeout: 8000 })
    }, 15000)
  })

  describe('JWT Token Integration', () => {
    it('should handle page refresh with existing JWT tenant', async () => {
      // Mock JWT with tenant_id already present (page refresh scenario)
      const { tokenManager } = await import('@/utils/token.manager')
      vi.mocked(tokenManager.getTokenClaims).mockReturnValue({
        tenant_id: '1',
        nameid: '3',
        email: 'admin@test.com'
      })
      
      localStorage.setItem('auth_token', 'existing.jwt.token')

      mockTenantService.getUserTenants.mockResolvedValue({
        success: true,
        data: mockMultipleTenants
      })

      render(<TestWrapper />)
      
      await waitFor(() => {
        expect(screen.getByTestId('available-count')).toHaveTextContent('2')
      })

      // Should not call selectTenant for page refresh
      expect(mockTenantService.selectTenant).not.toHaveBeenCalled()
    }, 15000)
  })

  describe('No Tenants Scenario', () => {
    it('should handle user with no tenant access', async () => {
      mockTenantService.getUserTenants.mockResolvedValue({
        success: true,
        data: []
      })

      render(<TestWrapper />)
      
      await waitFor(() => {
        expect(screen.getByTestId('available-count')).toHaveTextContent('0')
        expect(screen.getByTestId('error')).toHaveTextContent('No tenants found for user')
      }, { timeout: 8000 })
    }, 15000)
  })
})
