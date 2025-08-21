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
  TokenManager: {
    getInstance: () => ({
      setTokens: vi.fn(),
      getTokenClaims: vi.fn().mockReturnValue(null),
      clearTokens: vi.fn(),
      getAccessToken: vi.fn(),
      getRefreshToken: vi.fn()
    })
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

function TestComponent() {
  const { currentTenant, availableTenants, isLoading } = useTenant()
  
  if (isLoading) return <div>Loading tenants...</div>
  
  return (
    <div>
      <div data-testid="current-tenant">
        {currentTenant ? currentTenant.name : 'No tenant'}
      </div>
      <div data-testid="available-count">
        {availableTenants.length}
      </div>
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

describe('TenantContext', () => {
  let mockTenantService: any

  beforeEach(async () => {
    localStorage.clear()
    vi.clearAllMocks()
    
    // ✅ FIXED: Get the mocked service
    const tenantModule = await import('@/services/tenant.service')
    mockTenantService = tenantModule.tenantService
    
    // ✅ FIXED: Setup proper mock responses
    mockTenantService.getUserTenants.mockResolvedValue({
      success: true,
      data: [
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
    })
    
    // ✅ FIXED: Return proper token structure for selectTenant
    mockTenantService.selectTenant.mockResolvedValue({
      success: true,
      message: 'Tenant selected successfully',
      data: {
        accessToken: 'valid.jwt.token', // ✅ Use a simple string that won't cause decode errors
        refreshToken: 'valid.refresh.token',
        user: { id: '3', firstName: 'Admin', lastName: 'User', email: 'admin@test.com' },
        tenant: { id: '1', name: 'Test Tenant', domain: 'test.local' }
      }
    })
  })

  it('should load and auto-select single tenant', async () => {
    render(<TestWrapper />)
    
    await waitFor(() => {
      expect(screen.getByTestId('current-tenant')).toHaveTextContent('Test Tenant')
    }, { timeout: 8000 })
    
    expect(mockTenantService.getUserTenants).toHaveBeenCalled()
  }, 15000)
})
