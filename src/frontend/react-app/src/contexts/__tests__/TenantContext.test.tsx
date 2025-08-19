import { describe, it, expect, vi, beforeEach, afterEach } from 'vitest'
import { render, screen, waitFor, act } from '@testing-library/react'
import { TenantProvider, useTenant } from '../TenantContext.js'
import { AuthProvider } from '../AuthContext.js'
import { mockUsers } from '@/test/utils/test-utils.js'

// Mock the tenant service
vi.mock('@/services/tenant.service.js', () => ({
  tenantService: {
    getUserTenants: vi.fn().mockResolvedValue({
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
    }),
    getTenantSettings: vi.fn().mockResolvedValue({
      success: true,
      data: { theme: { primaryColor: '#1976d2' } }
    })
  }
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
    <AuthProvider mockUser={mockUsers.admin} mockAuthState="authenticated" testMode>
      <TenantProvider>
        <TestComponent />
      </TenantProvider>
    </AuthProvider>
  )
}

describe('TenantContext', () => {
  it('should load and auto-select single tenant', async () => {
    render(<TestWrapper />)
    
    await waitFor(() => {
      expect(screen.getByTestId('current-tenant')).toHaveTextContent('Test Tenant')
      expect(screen.getByTestId('available-count')).toHaveTextContent('1')
    })
  })
})
