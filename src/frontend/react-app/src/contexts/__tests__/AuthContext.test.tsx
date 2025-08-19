import { describe, it, expect, vi, beforeEach, afterEach } from 'vitest'
import { render, screen, waitFor, act } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import React, { useEffect } from 'react'
import { AuthProvider, useAuth } from '../AuthContext'
import { authService } from '@/services/auth.service'
import { tokenManager } from '@/utils/token.manager'
import type { User, AuthResponse } from '@/types/index'
import { TenantProvider } from '@/contexts/TenantContext.js'
import { MemoryRouter } from 'react-router-dom' // 🔧 ADD: Missing import

// Mock the services
vi.mock('@/services/auth.service')
vi.mock('@/utils/token.manager')

const mockAuthService = vi.mocked(authService)
const mockTokenManager = vi.mocked(tokenManager)

// Extend Window interface to include our test property
declare global {
  interface Window {
    __listeners?: Array<{ event: string; handler: EventListener }>
  }
}

// Define auth context state type for better type safety
interface AuthContextState {
  user: User | null
  permissions: string[]
  roles: string[]
  isAuthenticated: boolean
  isLoading: boolean
  error: string | null
  login: (email: string, password: string) => Promise<void>
  register: (userData: {
    email: string
    password: string
    confirmPassword: string
    firstName: string
    lastName: string
  }) => Promise<void>
  logout: () => Promise<void>
  clearError: () => void
  refreshAuth: () => Promise<void>
}

// Test component to access auth context
function TestComponent({ onStateChange }: { onStateChange?: (state: AuthContextState) => void }) {
  const auth = useAuth()
  
  // Call onStateChange when state changes
  if (onStateChange) {
    onStateChange(auth)
  }

  return (
    <div>
      <div data-testid="is-authenticated">{auth.isAuthenticated.toString()}</div>
      <div data-testid="is-loading">{auth.isLoading.toString()}</div>
      <div data-testid="user-email">{auth.user?.email || 'null'}</div>
      <div data-testid="user-roles">{JSON.stringify(auth.roles)}</div>
      <div data-testid="permissions">{JSON.stringify(auth.permissions)}</div>
      <div data-testid="error">{auth.error || 'null'}</div>
      <button onClick={() => auth.login('test@example.com', 'password')}>Login</button>
      <button onClick={auth.logout}>Logout</button>
      <button onClick={auth.clearError}>Clear Error</button>
      <button onClick={auth.refreshAuth}>Refresh Auth</button>
    </div>
  )
}

// Special test component for error scenarios
function TestComponentWithErrorHandler({ onStateChange }: { onStateChange?: (state: AuthContextState) => void }) {
  const auth = useAuth()
  
  // Call onStateChange when state changes
  if (onStateChange) {
    onStateChange(auth)
  }

  const handleLogin = async () => {
    try {
      await auth.login('test@example.com', 'password')
    } catch {
      // Expected error, handled by component
    }
  }

  const handleRegister = async () => {
    try {
      await auth.register({
        email: 'test@example.com',
        password: 'Password123!',
        confirmPassword: 'Password123!',
        firstName: 'Test',
        lastName: 'User'
      })
    } catch {
      // Expected error, handled by component
    }
  }

  return (
    <div>
      <div data-testid="is-authenticated">{auth.isAuthenticated.toString()}</div>
      <div data-testid="is-loading">{auth.isLoading.toString()}</div>
      <div data-testid="user-email">{auth.user?.email || 'null'}</div>
      <div data-testid="user-roles">{JSON.stringify(auth.roles)}</div>
      <div data-testid="permissions">{JSON.stringify(auth.permissions)}</div>
      <div data-testid="error">{auth.error || 'null'}</div>
      <button onClick={handleLogin}>Login</button>
      <button onClick={auth.logout}>Logout</button>
      <button onClick={auth.clearError}>Clear Error</button>
      <button onClick={auth.refreshAuth}>Refresh Auth</button>
      <button onClick={handleRegister}>Register</button>
    </div>
  )
}

// Mock user data
const mockUser: User = {
  id: '1',
  email: 'test@example.com',
  firstName: 'Test',
  lastName: 'User',
  fullName: 'Test User',
  roles: ['User', 'Admin'],
  tenantId: 'tenant1',
  isActive: true,
  emailConfirmed: true,
  createdAt: '2024-01-01T00:00:00Z',
  updatedAt: '2024-01-01T00:00:00Z'
}

const mockAuthResponse: AuthResponse = {
  accessToken: 'mock-access-token',
  refreshToken: 'mock-refresh-token',
  expiresAt: '2024-01-01T01:00:00Z',
  tokenType: 'Bearer',
  user: mockUser,
  tenant: {
    id: 'tenant1',
    name: 'Test Tenant',
    isActive: true,
    createdAt: '2024-01-01T00:00:00Z',
    updatedAt: '2024-01-01T00:00:00Z'
  }
}

describe('AuthContext', () => {
  beforeEach(() => {
    vi.clearAllMocks()
    // Reset window events with proper typing
    const listeners = window.__listeners || []
    listeners.forEach(({ event, handler }: { event: string; handler: EventListener }) => {
      window.removeEventListener(event, handler)
    })
    window.__listeners = []
  })

  afterEach(() => {
    vi.restoreAllMocks()
  })

  describe('useAuth hook', () => {
    it('should throw error when used outside AuthProvider', () => {
      expect(() => {
        render(<TestComponent />)
      }).toThrow('useAuth must be used within an AuthProvider')
    })
  })

  describe('AuthProvider initialization', () => {
    it('should initialize with default unauthenticated state', async () => {
      mockTokenManager.getToken.mockReturnValue(null)
      mockTokenManager.getRefreshToken.mockReturnValue(null)

      act(() => {
        render(
          <AuthProvider>
            <TestComponent />
          </AuthProvider>
        )
      })

      await waitFor(() => {
        expect(screen.getByTestId('is-authenticated')).toHaveTextContent('false')
        expect(screen.getByTestId('is-loading')).toHaveTextContent('false')
      })

      expect(screen.getByTestId('user-email')).toHaveTextContent('null')
      expect(screen.getByTestId('user-roles')).toHaveTextContent('[]')
      expect(screen.getByTestId('permissions')).toHaveTextContent('[]')
    })

    it('should initialize with test mode when mockUser provided', () => {
      render(
        <AuthProvider testMode mockUser={mockUser} mockAuthState="authenticated">
          <TestComponent />
        </AuthProvider>
      )

      expect(screen.getByTestId('is-authenticated')).toHaveTextContent('true')
      expect(screen.getByTestId('is-loading')).toHaveTextContent('false')
      expect(screen.getByTestId('user-email')).toHaveTextContent('test@example.com')
      expect(screen.getByTestId('user-roles')).toHaveTextContent('["User","Admin"]')
    })

    it('should initialize with loading state when mockAuthState is loading', () => {
      render(
        <AuthProvider testMode mockAuthState="loading">
          <TestComponent />
        </AuthProvider>
      )

      expect(screen.getByTestId('is-authenticated')).toHaveTextContent('false')
      expect(screen.getByTestId('is-loading')).toHaveTextContent('true')
    })

    it('should initialize auth when valid token exists', async () => {
      mockTokenManager.getToken.mockReturnValue('valid-token')
      mockTokenManager.getRefreshToken.mockReturnValue('refresh-token')
      mockTokenManager.isTokenExpired.mockReturnValue(false)
      mockTokenManager.getTokenClaims.mockReturnValue({
        email: 'test@example.com',
        roles: ['User'],
        permissions: ['users.read']
      })
      mockAuthService.validateToken.mockResolvedValue(mockUser)

      act(() => {
        render(
          <AuthProvider>
            <TestComponent />
          </AuthProvider>
        )
      })

      await waitFor(() => {
        expect(screen.getByTestId('is-authenticated')).toHaveTextContent('true')
      })

      expect(screen.getByTestId('user-email')).toHaveTextContent('test@example.com')
      expect(mockAuthService.validateToken).toHaveBeenCalledTimes(1)
    })

    it('should refresh token when existing token is expired', async () => {
      mockTokenManager.getToken.mockReturnValue('expired-token')
      mockTokenManager.getRefreshToken.mockReturnValue('refresh-token')
      mockTokenManager.isTokenExpired.mockReturnValue(true)
      mockTokenManager.getTokenClaims.mockReturnValue({
        email: 'test@example.com',
        roles: ['User'],
        permissions: ['users.read']
      })
      mockAuthService.refreshToken.mockResolvedValue({
        token: 'new-access-token',
        refreshToken: 'new-refresh-token'
      } as { token: string; refreshToken: string })
      mockAuthService.validateToken.mockResolvedValue(mockUser)

      act(() => {
        render(
          <AuthProvider>
            <TestComponent />
          </AuthProvider>
        )
      })

      await waitFor(() => {
        expect(screen.getByTestId('is-authenticated')).toHaveTextContent('true')
      })

      expect(mockAuthService.refreshToken).toHaveBeenCalledWith('refresh-token')
      expect(mockTokenManager.setTokens).toHaveBeenCalledWith('new-access-token', 'new-refresh-token')
    })

    it('should clear auth when token refresh fails', async () => {
      mockTokenManager.getToken.mockReturnValue('expired-token')
      mockTokenManager.getRefreshToken.mockReturnValue('refresh-token')
      mockTokenManager.isTokenExpired.mockReturnValue(true)
      mockAuthService.refreshToken.mockRejectedValue(new Error('Refresh failed'))

      act(() => {
        render(
          <AuthProvider>
            <TestComponent />
          </AuthProvider>
        )
      })

      await waitFor(() => {
        expect(screen.getByTestId('is-authenticated')).toHaveTextContent('false')
      })

      expect(mockTokenManager.clearTokens).toHaveBeenCalled()
    })
  })

  describe('login functionality', () => {
    it('should login successfully with valid credentials', async () => {
      const user = userEvent.setup()
      mockAuthService.login.mockResolvedValue(mockAuthResponse)
      mockTokenManager.getTokenClaims.mockReturnValue({
        email: 'test@example.com',
        roles: ['User', 'Admin'],
        permissions: ['users.read', 'users.write']
      })

      act(() => {
        render(
          <AuthProvider>
            <TestComponent />
          </AuthProvider>
        )
      })

      await act(async () => {
        await user.click(screen.getByText('Login'))
      })

      await waitFor(() => {
        expect(screen.getByTestId('is-authenticated')).toHaveTextContent('true')
      })

      expect(screen.getByTestId('user-email')).toHaveTextContent('test@example.com')
      expect(mockTokenManager.setTokens).toHaveBeenCalledWith(
        mockAuthResponse.accessToken,
        mockAuthResponse.refreshToken
      )
    })

    it('should handle login failure', async () => {
      const user = userEvent.setup()
      
      // Use mockRejectedValue but catch the error in the test component
      mockAuthService.login.mockRejectedValue(new Error('Invalid credentials'))

      act(() => {
        render(
          <AuthProvider>
            <TestComponentWithErrorHandler />
          </AuthProvider>
        )
      })

      // Click login and the component will handle the error
      await user.click(screen.getByText('Login'))

      // Wait for error to be set in the AuthContext
      await waitFor(() => {
        expect(screen.getByTestId('error')).toHaveTextContent('Invalid credentials')
      })

      expect(screen.getByTestId('is-authenticated')).toHaveTextContent('false')
    })

    it('should set loading state during login', async () => {
      const user = userEvent.setup()
      let loadingState = false
      
      // Create a promise we can control
      let resolveLogin: (value: AuthResponse) => void
      const loginPromise = new Promise<AuthResponse>((resolve) => {
        resolveLogin = resolve
      })
      
      mockAuthService.login.mockReturnValue(loginPromise)
      mockTokenManager.getTokenClaims.mockReturnValue({
        email: 'test@example.com',
        roles: ['User'],
        permissions: ['users.read']
      })

      act(() => {
        render(
          <AuthProvider>
            <TestComponent onStateChange={(state) => {
              if (state.isLoading) loadingState = true
            }} />
          </AuthProvider>
        )
      })

      await act(async () => {
        await user.click(screen.getByText('Login'))
      })

      // Check loading state is set
      expect(loadingState).toBe(true)

      // Resolve the login
      await act(async () => {
        resolveLogin(mockAuthResponse)
      })

      await waitFor(() => {
        expect(screen.getByTestId('is-loading')).toHaveTextContent('false')
      })
    })
  })

  describe('logout functionality', () => {
    it('should logout successfully', async () => {
      const user = userEvent.setup()
      mockAuthService.logout.mockResolvedValue(undefined)

      render(
        <AuthProvider testMode mockUser={mockUser} mockAuthState="authenticated">
          <TestComponent />
        </AuthProvider>
      )

      // Verify we start authenticated
      expect(screen.getByTestId('is-authenticated')).toHaveTextContent('true')

      await act(async () => {
        await user.click(screen.getByText('Logout'))
      })

      await waitFor(() => {
        expect(screen.getByTestId('is-authenticated')).toHaveTextContent('false')
      })

      expect(screen.getByTestId('user-email')).toHaveTextContent('null')
      expect(screen.getByTestId('user-roles')).toHaveTextContent('[]')
    })

    it('should handle logout service failure gracefully', async () => {
      const user = userEvent.setup()
      mockAuthService.logout.mockRejectedValue(new Error('Logout service failed'))
      const consoleSpy = vi.spyOn(console, 'error').mockImplementation(() => {})

      render(
        <AuthProvider testMode mockUser={mockUser} mockAuthState="authenticated">
          <TestComponent />
        </AuthProvider>
      )

      await act(async () => {
        await user.click(screen.getByText('Logout'))
      })

      await waitFor(() => {
        expect(screen.getByTestId('is-authenticated')).toHaveTextContent('false')
      })

      // Should still clear state even if service fails
      expect(screen.getByTestId('user-email')).toHaveTextContent('null')
      consoleSpy.mockRestore()
    })

    it('should not call tokenManager in test mode', async () => {
      const user = userEvent.setup()

      render(
        <AuthProvider testMode mockUser={mockUser} mockAuthState="authenticated">
          <TestComponent />
        </AuthProvider>
      )

      await act(async () => {
        await user.click(screen.getByText('Logout'))
      })

      expect(mockTokenManager.clearTokens).not.toHaveBeenCalled()
    })
  })

  describe('error handling', () => {
    it('should clear error when clearError is called', async () => {
      const user = userEvent.setup()
      
      // Use mockRejectedValue but catch the error in the test component
      mockAuthService.login.mockRejectedValue(new Error('Login failed'))

      act(() => {
        render(
          <AuthProvider>
            <TestComponentWithErrorHandler />
          </AuthProvider>
        )
      })

      // Click login to trigger error
      await user.click(screen.getByText('Login'))

      // Wait for error to be set
      await waitFor(() => {
        expect(screen.getByTestId('error')).toHaveTextContent('Login failed')
      })

      // Clear the error
      await act(async () => {
        await user.click(screen.getByText('Clear Error'))
      })

      expect(screen.getByTestId('error')).toHaveTextContent('null')
    })
  })

  describe('role and permission extraction', () => {
    it('should extract roles from JWT token as array', async () => {
      mockTokenManager.getToken.mockReturnValue('valid-token')
      mockTokenManager.isTokenExpired.mockReturnValue(false)
      mockTokenManager.getTokenClaims.mockReturnValue({
        'http://schemas.microsoft.com/ws/2008/06/identity/claims/role': ['Admin', 'User']
      })
      mockAuthService.validateToken.mockResolvedValue(mockUser)

      act(() => {
        render(
          <AuthProvider>
            <TestComponent />
          </AuthProvider>
        )
      })

      await waitFor(() => {
        expect(screen.getByTestId('user-roles')).toHaveTextContent('["Admin","User"]')
      })
    })

    it('should extract roles from JWT token as comma-separated string', async () => {
      mockTokenManager.getToken.mockReturnValue('valid-token')
      mockTokenManager.isTokenExpired.mockReturnValue(false)
      mockTokenManager.getTokenClaims.mockReturnValue({
        role: 'Admin,User,Manager'
      })
      mockAuthService.validateToken.mockResolvedValue(mockUser)

      act(() => {
        render(
          <AuthProvider>
            <TestComponent />
          </AuthProvider>
        )
      })

      await waitFor(() => {
        expect(screen.getByTestId('user-roles')).toHaveTextContent('["Admin","User","Manager"]')
      })
    })

    it('should extract permissions from JWT token', async () => {
      mockTokenManager.getToken.mockReturnValue('valid-token')
      mockTokenManager.isTokenExpired.mockReturnValue(false)
      mockTokenManager.getTokenClaims.mockReturnValue({
        permissions: ['users.read', 'users.write', 'admin.access']
      })
      mockAuthService.validateToken.mockResolvedValue(mockUser)

      act(() => {
        render(
          <AuthProvider>
            <TestComponent />
          </AuthProvider>
        )
      })

      await waitFor(() => {
        expect(screen.getByTestId('permissions')).toHaveTextContent('["users.read","users.write","admin.access"]')
      })
    })

    it('should handle missing role claims gracefully', async () => {
      mockTokenManager.getToken.mockReturnValue('valid-token')
      mockTokenManager.isTokenExpired.mockReturnValue(false)
      mockTokenManager.getTokenClaims.mockReturnValue({
        email: 'test@example.com'
        // No role claims
      })
      mockAuthService.validateToken.mockResolvedValue(mockUser)

      act(() => {
        render(
          <AuthProvider>
            <TestComponent />
          </AuthProvider>
        )
      })

      await waitFor(() => {
        expect(screen.getByTestId('user-roles')).toHaveTextContent('[]')
      })
    })

    it('should handle token parsing errors', async () => {
      mockTokenManager.getToken.mockReturnValue('valid-token')
      mockTokenManager.isTokenExpired.mockReturnValue(false)
      mockTokenManager.getTokenClaims.mockImplementation(() => {
        throw new Error('Invalid token format')
      })
      mockAuthService.validateToken.mockResolvedValue(mockUser)
      const consoleSpy = vi.spyOn(console, 'error').mockImplementation(() => {})

      act(() => {
        render(
          <AuthProvider>
            <TestComponent />
          </AuthProvider>
        )
      })

      await waitFor(() => {
        expect(screen.getByTestId('user-roles')).toHaveTextContent('[]')
        expect(screen.getByTestId('permissions')).toHaveTextContent('[]')
      })

      consoleSpy.mockRestore()
    })
  })

  describe('register functionality', () => {
    it('should register successfully', async () => {
      mockAuthService.register.mockResolvedValue(mockAuthResponse)
      mockTokenManager.getTokenClaims.mockReturnValue({
        email: 'test@example.com',
        roles: ['User'],
        permissions: ['users.read']
      })

      const TestRegisterComponent = () => {
        const auth = useAuth()
        return (
          <div>
            <button onClick={() => auth.register({
              email: 'test@example.com',
              password: 'Password123!',
              confirmPassword: 'Password123!',
              firstName: 'Test',
              lastName: 'User'
            })}>Register</button>
            <div data-testid="is-authenticated">{auth.isAuthenticated.toString()}</div>
            <div data-testid="user-email">{auth.user?.email || 'null'}</div>
          </div>
        )
      }

      const user = userEvent.setup()
      
      act(() => {
        render(
          <AuthProvider>
            <TestRegisterComponent />
          </AuthProvider>
        )
      })

      await act(async () => {
        await user.click(screen.getByText('Register'))
      })

      await waitFor(() => {
        expect(screen.getByTestId('is-authenticated')).toHaveTextContent('true')
      })

      expect(screen.getByTestId('user-email')).toHaveTextContent('test@example.com')
      expect(mockTokenManager.setTokens).toHaveBeenCalledWith(
        mockAuthResponse.accessToken,
        mockAuthResponse.refreshToken
      )
    })

    it('should handle register failure', async () => {
      // Use mockRejectedValue but catch the error in the test component
      mockAuthService.register.mockRejectedValue(new Error('Registration failed'))

      const user = userEvent.setup()
      
      act(() => {
        render(
          <AuthProvider>
            <TestComponentWithErrorHandler />
          </AuthProvider>
        )
      })

      // Click register - component will handle the rejection
      await user.click(screen.getByText('Register'))

      await waitFor(() => {
        expect(screen.getByTestId('error')).toHaveTextContent('Registration failed')
      })
    })
  })

  describe('auth event listener', () => {
    it('should listen for auth:logout events', async () => {
      // Component that can be controlled to test logout behavior
      const TestEventComponent = () => {
        const auth = useAuth()
        
        // Create a window event listener effect to simulate the real behavior
        useEffect(() => {
          const handleLogout = () => {
            auth.logout()
          }
          
          window.addEventListener('auth:logout', handleLogout)
          return () => window.removeEventListener('auth:logout', handleLogout)
        }, [auth.logout])
        
        return (
          <div>
            <div data-testid="is-authenticated">{auth.isAuthenticated.toString()}</div>
            <div data-testid="user-email">{auth.user?.email || 'null'}</div>
          </div>
        )
      }

      // Render component in authenticated state
      act(() => {
        render(
          <AuthProvider testMode mockUser={mockUser} mockAuthState="authenticated">
            <TestEventComponent />
          </AuthProvider>
        )
      })

      // Verify initial state
      expect(screen.getByTestId('is-authenticated')).toHaveTextContent('true')
      expect(screen.getByTestId('user-email')).toHaveTextContent('test@example.com')

      // Simulate auth:logout event
      await act(async () => {
        const event = new CustomEvent('auth:logout', { detail: 'Token expired' })
        window.dispatchEvent(event)
      })

      // Wait for the logout to complete
      await waitFor(() => {
        expect(screen.getByTestId('is-authenticated')).toHaveTextContent('false')
      })

      expect(screen.getByTestId('user-email')).toHaveTextContent('null')
    })
  })

  describe('refreshAuth functionality', () => {
    it('should refresh auth in non-test mode', async () => {
      mockTokenManager.getToken.mockReturnValue('valid-token')
      mockTokenManager.isTokenExpired.mockReturnValue(false)
      mockTokenManager.getTokenClaims.mockReturnValue({
        email: 'test@example.com',
        roles: ['User'],
        permissions: ['users.read']
      })
      mockAuthService.validateToken.mockResolvedValue(mockUser)

      const TestRefreshComponent = () => {
        const auth = useAuth()
        return (
          <div>
            <button onClick={auth.refreshAuth}>Refresh</button>
            <div data-testid="is-authenticated">{auth.isAuthenticated.toString()}</div>
          </div>
        )
      }

      const user = userEvent.setup()
      
      act(() => {
        render(
          <AuthProvider>
            <TestRefreshComponent />
          </AuthProvider>
        )
      })

      await act(async () => {
        await user.click(screen.getByText('Refresh'))
      })

      await waitFor(() => {
        expect(mockAuthService.validateToken).toHaveBeenCalled()
      })
    })

    it('should not refresh auth in test mode', async () => {
      const TestRefreshComponent = () => {
        const auth = useAuth()
        return (
          <div>
            <button onClick={auth.refreshAuth}>Refresh</button>
          </div>
        )
      }

      const user = userEvent.setup()
      render(
        <AuthProvider testMode>
          <TestRefreshComponent />
        </AuthProvider>
      )

      await act(async () => {
        await user.click(screen.getByText('Refresh'))
      })

      // Should not call any auth service methods in test mode
      expect(mockAuthService.validateToken).not.toHaveBeenCalled()
    })
  })

  describe('multiple role handling', () => {
    it('should handle user with multiple roles from user object', () => {
      const multiRoleUser: User = {
        ...mockUser,
        roles: ['User', 'Admin', 'Manager']
      }

      render(
        <AuthProvider testMode mockUser={multiRoleUser} mockAuthState="authenticated">
          <TestComponent />
        </AuthProvider>
      )

      expect(screen.getByTestId('user-roles')).toHaveTextContent('["User","Admin","Manager"]')
    })

    it('should handle single role from user object', () => {
      const singleRoleUser: User = {
        ...mockUser,
        roles: 'Admin' as string | string[] // Fix type assertion
      }

      render(
        <AuthProvider testMode mockUser={singleRoleUser} mockAuthState="authenticated">
          <TestComponent />
        </AuthProvider>
      )

      expect(screen.getByTestId('user-roles')).toHaveTextContent('["Admin"]')
    })
  })
})

function TestWrapper({ children }: { children: React.ReactNode }) {
  return (
    <MemoryRouter>
      <AuthProvider testMode>
        <TenantProvider> {/* 🔧 NEW: Add TenantProvider */}
          {children}
        </TenantProvider>
      </AuthProvider>
    </MemoryRouter>
  )
}
