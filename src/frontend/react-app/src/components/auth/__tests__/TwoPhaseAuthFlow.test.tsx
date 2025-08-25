import { describe, it, expect, vi, beforeEach } from 'vitest'
import { render, screen, waitFor } from '@testing-library/react'
import { BrowserRouter } from 'react-router-dom'
import userEvent from '@testing-library/user-event'
import { AuthProvider } from '@/contexts/AuthContext'
import { TenantProvider } from '@/contexts/TenantContext'
import { TenantSelector } from '../TenantSelector'
import { LoginForm } from '../LoginForm'

// Mock all services
vi.mock('@/services/auth.service', () => ({
  authService: {
    login: vi.fn(),
    validateToken: vi.fn(),
    refreshToken: vi.fn(),
  }
}))

vi.mock('@/services/tenant.service', () => ({
  tenantService: {
    getUserTenants: vi.fn(),
    selectTenant: vi.fn(),
    switchTenant: vi.fn(),
    getTenantSettings: vi.fn(),
  }
}))

vi.mock('@/utils/token.manager', () => ({
  tokenManager: {
    getToken: vi.fn(),
    getRefreshToken: vi.fn(),
    setTokens: vi.fn(),
    clearTokens: vi.fn(),
    isTokenExpired: vi.fn(),
    getTokenClaims: vi.fn(),
  }
}))

const TestWrapper = ({ children }: { children: React.ReactNode }) => (
  <BrowserRouter>
    <AuthProvider>
      <TenantProvider>
        {children}
      </TenantProvider>
    </AuthProvider>
  </BrowserRouter>
)

describe('Two-Phase Authentication Flow', () => {
  beforeEach(() => {
    vi.clearAllMocks()
    localStorage.clear()
    sessionStorage.clear()
  })

  it('should complete Phase 1: Login without tenant context', async () => {
    const user = userEvent.setup()
    const { authService } = await import('@/services/auth.service')
    const { tokenManager } = await import('@/utils/token.manager')

    // Mock Phase 1 login response with all required AuthResponse properties
    vi.mocked(authService.login).mockResolvedValue({
      accessToken: 'phase1-token-no-tenant',
      refreshToken: 'refresh-token',
      expiresAt: new Date(Date.now() + 3600000).toISOString(),
      tokenType: 'Bearer',
      user: {
        id: '1',
        email: 'admin@tenant1.com',
        firstName: 'Admin',
        lastName: 'User',
        fullName: 'Admin User',
        phoneNumber: undefined,
        timeZone: 'UTC',
        language: 'en',
        lastLoginAt: undefined,
        emailConfirmed: true,
        isActive: true,
        roles: ['Admin'],
        tenantId: '1',
        createdAt: new Date().toISOString(),
        updatedAt: new Date().toISOString(),
        preferences: {
          theme: 'light',
          language: 'en',
          timeZone: 'UTC',
          notifications: {
            email: true,
            push: true,
            sms: false
          }
        }
      },
      tenant: {
        id: '1',
        name: 'Default Tenant',
        domain: 'default.local',
        subscriptionPlan: 'Development',
        isActive: true,
        createdAt: new Date().toISOString(),
        updatedAt: new Date().toISOString(),
      }
    })

    // Mock token claims without tenant
    vi.mocked(tokenManager.getTokenClaims).mockReturnValue({
      nameid: '1',
      email: 'admin@tenant1.com',
      // No tenant_id or permissions
    })

    render(
      <TestWrapper>
        <LoginForm />
      </TestWrapper>
    )

    // Fill login form
    await user.type(screen.getByLabelText(/email/i), 'admin@tenant1.com')
    await user.type(screen.getByLabelText(/password/i), 'password123')
    await user.click(screen.getByRole('button', { name: /sign in/i }))

    // Should complete Phase 1
    await waitFor(() => {
      expect(authService.login).toHaveBeenCalledWith({
        email: 'admin@tenant1.com',
        password: 'password123'
      })
    })
  })

  it('should complete Phase 2: Tenant selection', async () => {
    const user = userEvent.setup()
    const { tenantService } = await import('@/services/tenant.service')
    const { tokenManager } = await import('@/utils/token.manager')

    const mockTenants = [
      {
        id: '1',
        name: 'Tenant 1',
        domain: 'tenant1.com',
        subscriptionPlan: 'Premium',
        isActive: true,
        createdAt: new Date().toISOString(),
        updatedAt: new Date().toISOString(),
      },
      {
        id: '2',
        name: 'Tenant 2',
        domain: 'tenant2.com',
        subscriptionPlan: 'Basic',
        isActive: true,
        createdAt: new Date().toISOString(),
        updatedAt: new Date().toISOString(),
      }
    ]

    // Mock get user tenants
    vi.mocked(tenantService.getUserTenants).mockResolvedValue({
      success: true,
      data: mockTenants
    })

    // 🔧 FIXED: Mock Phase 2 tenant selection with correct structure (no expiresAt/tokenType)
    vi.mocked(tenantService.selectTenant).mockResolvedValue({
      success: true,
      data: {
        accessToken: 'phase2-token-with-tenant',
        refreshToken: 'new-refresh-token',
        user: {
          id: '1',
          email: 'admin@tenant1.com',
          firstName: 'Admin',
          lastName: 'User',
          fullName: 'Admin User',
          phoneNumber: undefined,
          timeZone: 'UTC',
          language: 'en',
          lastLoginAt: undefined,
          emailConfirmed: true,
          isActive: true,
          roles: ['Admin'],
          tenantId: '1',
          createdAt: new Date().toISOString(),
          updatedAt: new Date().toISOString(),
          preferences: {
            theme: 'light',
            language: 'en',
            timeZone: 'UTC',
            notifications: {
              email: true,
              push: true,
              sms: false
            }
          }
        },
        tenant: {
          id: '1',
          name: 'Tenant 1',
          domain: 'tenant1.com',
          subscriptionPlan: 'Premium',
          isActive: true,
          createdAt: new Date().toISOString(),
          updatedAt: new Date().toISOString(),
        }
      }
    })

    const mockOnTenantSelected = vi.fn()

    render(
      <TestWrapper>
        <TenantSelector onTenantSelected={mockOnTenantSelected} />
      </TestWrapper>
    )

    // Wait for tenants to load and select one
    await waitFor(() => {
      expect(screen.getByText('Tenant 1')).toBeInTheDocument()
    })

    await user.click(screen.getByText('Tenant 1'))
    await user.click(screen.getByRole('button', { name: /select organization/i }))

    expect(mockOnTenantSelected).toHaveBeenCalledWith('1')
  })

  it('should handle single tenant auto-selection', async () => {
    const { tenantService } = await import('@/services/tenant.service')

    const singleTenant = [
      {
        id: '1',
        name: 'Single Tenant',
        domain: 'single.com',
        subscriptionPlan: 'Premium',
        isActive: true,
        createdAt: new Date().toISOString(),
        updatedAt: new Date().toISOString(),
      }
    ]

    vi.mocked(tenantService.getUserTenants).mockResolvedValue({
      success: true,
      data: singleTenant
    })

    // 🔧 FIXED: Correct structure for selectTenant response (no expiresAt/tokenType)
    vi.mocked(tenantService.selectTenant).mockResolvedValue({
      success: true,
      data: {
        accessToken: 'auto-selected-token',
        refreshToken: 'auto-refresh',
        user: {
          id: '1',
          email: 'admin@tenant1.com',
          firstName: 'Admin',
          lastName: 'User',
          fullName: 'Admin User',
          phoneNumber: undefined,
          timeZone: 'UTC',
          language: 'en',
          lastLoginAt: undefined,
          emailConfirmed: true,
          isActive: true,
          roles: ['Admin'],
          tenantId: '1',
          createdAt: new Date().toISOString(),
          updatedAt: new Date().toISOString(),
          preferences: {
            theme: 'light',
            language: 'en',
            timeZone: 'UTC',
            notifications: {
              email: true,
              push: true,
              sms: false
            }
          }
        },
        tenant: singleTenant[0]
      }
    })

    const mockOnTenantSelected = vi.fn()

    render(
      <TestWrapper>
        <TenantSelector onTenantSelected={mockOnTenantSelected} />
      </TestWrapper>
    )

    // Should auto-select and show continue
    await waitFor(() => {
      expect(screen.getByText('Continue')).toBeInTheDocument()
    })
  })

  it('should handle authentication errors', async () => {
    const { authService } = await import('@/services/auth.service')

    vi.mocked(authService.login).mockRejectedValue(
      new Error('Invalid credentials')
    )

    render(
      <TestWrapper>
        <LoginForm />
      </TestWrapper>
    )

    const user = userEvent.setup()
    
    await user.type(screen.getByLabelText(/email/i), 'wrong@email.com')
    await user.type(screen.getByLabelText(/password/i), 'wrongpassword')
    await user.click(screen.getByRole('button', { name: /sign in/i }))

    await waitFor(() => {
      expect(screen.getByText(/invalid credentials/i)).toBeInTheDocument()
    })
  })

  it('should handle tenant loading errors', async () => {
    const { tenantService } = await import('@/services/tenant.service')

    vi.mocked(tenantService.getUserTenants).mockRejectedValue(
      new Error('Failed to load tenants')
    )

    const mockOnTenantSelected = vi.fn()

    render(
      <TestWrapper>
        <TenantSelector onTenantSelected={mockOnTenantSelected} />
      </TestWrapper>
    )

    await waitFor(() => {
      expect(screen.getByText('Failed to load tenants')).toBeInTheDocument()
    })
  })
})
