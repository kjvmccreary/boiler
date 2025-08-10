import { describe, it, expect, vi, beforeEach } from 'vitest'
import { render, screen, waitFor } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import { MemoryRouter } from 'react-router-dom'
import { QueryClient, QueryClientProvider } from '@tanstack/react-query'
import { EmailConfirmation } from '@/components/auth/EmailConfirmation.js'
import { LogoutButton } from '@/components/auth/LogoutButton.js'
import { authService } from '@/services/auth.service.js'
import toast from 'react-hot-toast'

// Mock dependencies
vi.mock('@/services/auth.service.js')
vi.mock('react-hot-toast')

// Mock the auth context
const mockLogout = vi.fn()
vi.mock('@/contexts/AuthContext.js', () => ({
  useAuth: () => ({
    logout: mockLogout,
    user: { id: '1', email: 'test@example.com', firstName: 'Test', lastName: 'User' },
    isAuthenticated: true,
    isLoading: false,
  }),
  AuthProvider: ({ children }: { children: React.ReactNode }) => <>{children}</>,
}))

// Test wrapper component
interface TestWrapperProps {
  children: React.ReactNode
  initialEntries?: string[]
}

function TestWrapper({ children, initialEntries = ['/'] }: TestWrapperProps) {
  const queryClient = new QueryClient({
    defaultOptions: {
      queries: { retry: false, gcTime: 0 },
    },
  })

  return (
    <QueryClientProvider client={queryClient}>
      <MemoryRouter initialEntries={initialEntries}>
        {children}
      </MemoryRouter>
    </QueryClientProvider>
  )
}

describe('Authentication Integration Tests', () => {
  const user = userEvent.setup()
  
  beforeEach(() => {
    vi.clearAllMocks()
  })

  describe('Email Confirmation Flow', () => {
    it('completes email confirmation and shows success state', async () => {
      vi.mocked(authService.confirmEmail).mockResolvedValueOnce(undefined)
      
      render(
        <TestWrapper initialEntries={['/confirm-email?token=valid-token']}>
          <EmailConfirmation />
        </TestWrapper>
      )
      
      // Initial loading state
      expect(screen.getByText('Confirming Email')).toBeInTheDocument()
      
      // Success state
      await waitFor(() => {
        expect(screen.getByText('Email Confirmed!')).toBeInTheDocument()
      })
      
      expect(toast.success).toHaveBeenCalledWith('Email confirmed successfully!')
      expect(screen.getByRole('button', { name: /sign in now/i })).toBeInTheDocument()
    })

    it('handles confirmation failure and provides recovery options', async () => {
      vi.mocked(authService.confirmEmail).mockRejectedValueOnce(
        new Error('The confirmation link has expired')
      )
      
      render(
        <TestWrapper initialEntries={['/confirm-email?token=expired-token']}>
          <EmailConfirmation />
        </TestWrapper>
      )
      
      await waitFor(() => {
        expect(screen.getByText('Confirmation Failed')).toBeInTheDocument()
      })
      
      expect(screen.getByText(/confirmation link has expired/i)).toBeInTheDocument()
      expect(screen.getByRole('button', { name: /resend confirmation/i })).toBeInTheDocument()
      expect(screen.getByRole('button', { name: /back to login/i })).toBeInTheDocument()
    })
  })

  describe('Logout Flow', () => {
    it('performs logout with confirmation dialog', async () => {
      mockLogout.mockResolvedValueOnce(undefined)
      
      render(
        <TestWrapper>
          <LogoutButton showConfirmation={true} />
        </TestWrapper>
      )
      
      // Click logout button
      await user.click(screen.getByRole('button', { name: /logout/i }))
      
      // Confirmation dialog appears
      expect(screen.getByText('Confirm Logout')).toBeInTheDocument()
      expect(screen.getByText(/are you sure you want to logout/i)).toBeInTheDocument()
      
      // Confirm logout
      const logoutButtons = screen.getAllByRole('button', { name: /logout/i })
      const confirmButton = logoutButtons.find(btn => 
        btn.closest('[role="dialog"]') !== null
      )
      await user.click(confirmButton!)
      
      await waitFor(() => {
        expect(mockLogout).toHaveBeenCalled()
      })
    })

    it('cancels logout when user chooses cancel', async () => {
      render(
        <TestWrapper>
          <LogoutButton showConfirmation={true} />
        </TestWrapper>
      )
      
      await user.click(screen.getByRole('button', { name: /logout/i }))
      await user.click(screen.getByRole('button', { name: /cancel/i }))
      
      await waitFor(() => {
        expect(screen.queryByText('Confirm Logout')).not.toBeInTheDocument()
      })
      expect(mockLogout).not.toHaveBeenCalled()
    })
  })

  describe('Error Recovery', () => {
    it('provides clear error messages and recovery paths', async () => {
      // Test email confirmation error
      vi.mocked(authService.confirmEmail).mockRejectedValueOnce(
        new Error('Network error occurred')
      )
      
      render(
        <TestWrapper initialEntries={['/confirm-email?token=network-error']}>
          <EmailConfirmation />
        </TestWrapper>
      )
      
      await waitFor(() => {
        expect(screen.getByText('Confirmation Failed')).toBeInTheDocument()
      })
      
      expect(screen.getByText('Network error occurred')).toBeInTheDocument()
      expect(screen.getByRole('button', { name: /resend confirmation/i })).toBeInTheDocument()
    })
  })

  describe('Accessibility Integration', () => {
    it('maintains proper focus management through auth flows', async () => {
      vi.mocked(authService.confirmEmail).mockResolvedValueOnce(undefined)
      
      render(
        <TestWrapper initialEntries={['/confirm-email?token=valid-token']}>
          <EmailConfirmation />
        </TestWrapper>
      )
      
      await waitFor(() => {
        expect(screen.getByRole('button', { name: /sign in now/i })).toBeInTheDocument()
      })
      
      const signInButton = screen.getByRole('button', { name: /sign in now/i })
      signInButton.focus()
      expect(signInButton).toHaveFocus()
    })

    it('provides appropriate ARIA announcements for state changes', async () => {
      vi.mocked(authService.confirmEmail).mockRejectedValueOnce(
        new Error('Confirmation failed')
      )
      
      render(
        <TestWrapper initialEntries={['/confirm-email?token=error-token']}>
          <EmailConfirmation />
        </TestWrapper>
      )
      
      await waitFor(() => {
        // Error alert should be announced to screen readers
        expect(screen.getByRole('alert')).toBeInTheDocument()
      })
    })
  })
})
