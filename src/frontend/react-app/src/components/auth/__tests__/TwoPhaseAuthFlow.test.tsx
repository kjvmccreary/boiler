import { describe, it, expect, vi, beforeEach } from 'vitest'
import { render, screen, waitFor } from '@testing-library/react'
import { BrowserRouter } from 'react-router-dom'
import userEvent from '@testing-library/user-event'
import { AuthProvider } from '@/contexts/AuthContext.tsx'
import { TenantProvider } from '@/contexts/TenantContext.tsx'
import { TenantSelector } from '../TenantSelector.tsx'
import { LoginForm } from '../LoginForm.tsx'

// ✅ FIX: Import and create mock service references
import { authService } from '@/services/auth.service.ts'
import { tenantService } from '@/services/tenant.service.ts'
import { tokenManager } from '@/utils/token.manager.ts'

// Mock all services
vi.mock('@/services/auth.service.ts', () => ({
  authService: {
    login: vi.fn(),
    validateToken: vi.fn(),
    refreshToken: vi.fn(),
  }
}))

vi.mock('@/services/tenant.service.ts', () => ({
  tenantService: {
    getUserTenants: vi.fn(),
    selectTenant: vi.fn(),
    switchTenant: vi.fn(),
    getTenantSettings: vi.fn(),
  }
}))

vi.mock('@/utils/token.manager.ts', () => ({
  tokenManager: {
    getToken: vi.fn(),
    getRefreshToken: vi.fn(),
    setTokens: vi.fn(),
    clearTokens: vi.fn(),
    isTokenExpired: vi.fn(),
    getTokenClaims: vi.fn(),
  }
}))

// ✅ FIX: Create mock service references
const mockAuthService = vi.mocked(authService)
const mockTenantService = vi.mocked(tenantService)
const mockTokenManager = vi.mocked(tokenManager)

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
    
    // ✅ FIX: Use the proper mock references
    mockAuthService.login.mockClear()
    mockTenantService.getUserTenants.mockClear()
    mockTenantService.selectTenant.mockClear()
  })

  it('should complete Phase 1: Login without tenant context', async () => {
    const user = userEvent.setup()

    // Mock Phase 1 login response with all required AuthResponse properties
    mockAuthService.login.mockResolvedValue({
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
    mockTokenManager.getTokenClaims.mockReturnValue({
      nameid: '1',
      email: 'admin@tenant1.com',
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
      expect(mockAuthService.login).toHaveBeenCalledWith({
        email: 'admin@tenant1.com',
        password: 'password123'
      })
    })
  })

  it('should complete Phase 2: Tenant selection', async () => {
    const user = userEvent.setup()

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
    mockTenantService.getUserTenants.mockResolvedValue({
      success: true,
      data: mockTenants
    })

    mockTenantService.selectTenant.mockResolvedValue({
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
    }, { timeout: 5000 })

    await user.click(screen.getByText('Tenant 1'))
    await user.click(screen.getByRole('button', { name: /select organization/i }))

    expect(mockOnTenantSelected).toHaveBeenCalledWith('1')
  })

  it('should handle single tenant auto-selection', async () => {
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

    mockTenantService.getUserTenants.mockResolvedValue({
      success: true,
      data: singleTenant
    })

    mockTenantService.selectTenant.mockResolvedValue({
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

    // Wait for loading to complete
    await waitFor(() => {
      expect(screen.queryByRole('progressbar')).not.toBeInTheDocument()
    })

    // Should auto-select and show continue
    await waitFor(() => {
      expect(screen.getByText('Continue')).toBeInTheDocument()
    }, { timeout: 5000 })

    const user = userEvent.setup()
    const continueButton = screen.getByText('Continue')
    await user.click(continueButton)

    expect(mockOnTenantSelected).toHaveBeenCalledWith('1')
  })

  it('should handle authentication errors', async () => {
    mockAuthService.login.mockRejectedValue(
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
    mockTenantService.getUserTenants.mockRejectedValue(
      new Error('Failed to load tenants')
    )

    const mockOnTenantSelected = vi.fn()

    render(
      <TestWrapper>
        <TenantSelector onTenantSelected={mockOnTenantSelected} />
      </TestWrapper>
    )

    await waitFor(() => {
      expect(screen.getByText(/failed to load tenants/i)).toBeInTheDocument()
    }, { timeout: 5000 })
  })
})
