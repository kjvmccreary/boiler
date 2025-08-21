import { vi } from 'vitest'

// ✅ CENTRALIZED MOCKS: Create all service mocks in one place to avoid hoisting issues
export const mockTenantService = {
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
    data: {
      theme: { primaryColor: '#1976d2', companyName: 'Test Tenant' },
      features: { multiUser: true },
      subscriptionPlan: 'Development'
    }
  }),
  selectTenant: vi.fn().mockResolvedValue({
    success: true,
    message: 'Tenant selected successfully',
    data: {
      accessToken: 'mock_access_token',
      refreshToken: 'mock_refresh_token',
      user: { id: '3', firstName: 'Admin', lastName: 'User', email: 'admin@test.com' },
      tenant: { id: '1', name: 'Test Tenant', domain: 'test.local' }
    }
  }),
  switchTenant: vi.fn().mockResolvedValue({
    success: true,
    data: { accessToken: 'mock_token', refreshToken: 'mock_refresh' }
  })
}

export const mockRoleService = {
  getRoles: vi.fn(),
  getRoleById: vi.fn(),
  createRole: vi.fn(),
  updateRole: vi.fn(),
  deleteRole: vi.fn(),
  getRolePermissions: vi.fn(),
  updateRolePermissions: vi.fn()
}

export const mockAuthService = {
  login: vi.fn(),
  register: vi.fn(),
  logout: vi.fn(),
  refreshToken: vi.fn(),
  getCurrentUser: vi.fn()
}
