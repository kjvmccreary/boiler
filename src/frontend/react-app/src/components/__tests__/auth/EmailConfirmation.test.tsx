import { describe, it, expect, vi, beforeEach } from 'vitest'
import { render, screen, waitFor } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import { MemoryRouter } from 'react-router-dom'
import { EmailConfirmation } from '@/components/auth/EmailConfirmation.js'
import { authService } from '@/services/auth.service.js'
import toast from 'react-hot-toast'

// Mock dependencies
vi.mock('@/services/auth.service.js')
vi.mock('react-hot-toast')

// Mock useNavigate
const mockNavigate = vi.fn()
vi.mock('react-router-dom', async () => {
  const actual = await vi.importActual('react-router-dom')
  return {
    ...actual,
    useNavigate: () => mockNavigate,
  }
})

// Helper function to render component with router and query params
const renderWithRouter = (initialEntries: string[] = ['/confirm-email']) => {
  return render(
    <MemoryRouter initialEntries={initialEntries}>
      <EmailConfirmation />
    </MemoryRouter>
  )
}

describe('EmailConfirmation', () => {
  const user = userEvent.setup()
  
  beforeEach(() => {
    vi.clearAllMocks()
    mockNavigate.mockClear()
  })

  describe('Token Validation', () => {
    it('displays invalid token message when no token is provided', async () => {
      renderWithRouter(['/confirm-email'])
      
      // Wait for component to process the missing token
      await waitFor(() => {
        expect(screen.getByText('Confirmation Failed')).toBeInTheDocument()
      })
      
      expect(screen.getByText(/invalid confirmation link/i)).toBeInTheDocument()
      expect(screen.getByRole('button', { name: /resend confirmation/i })).toBeInTheDocument()
      expect(screen.getByRole('button', { name: /back to login/i })).toBeInTheDocument()
    })

    it('shows loading state initially when token is present', async () => {
      vi.mocked(authService.confirmEmail).mockImplementation(() => {
        return new Promise(resolve => setTimeout(resolve, 100))
      })

      renderWithRouter(['/confirm-email?token=valid-token'])
      
      // Should immediately show loading state
      expect(screen.getByText('Confirming Email')).toBeInTheDocument()
      expect(screen.getByText(/confirming your email address/i)).toBeInTheDocument()
      expect(screen.getByRole('progressbar')).toBeInTheDocument()
    })
  })

  describe('Successful Confirmation', () => {
    it('displays success message when confirmation succeeds', async () => {
      vi.mocked(authService.confirmEmail).mockResolvedValueOnce(undefined)
      
      renderWithRouter(['/confirm-email?token=valid-token'])
      
      await waitFor(() => {
        expect(screen.getByText('Email Confirmed!')).toBeInTheDocument()
      })
      
      expect(screen.getByText(/your email address has been successfully confirmed/i)).toBeInTheDocument()
      expect(screen.getByRole('button', { name: /sign in now/i })).toBeInTheDocument()
      expect(toast.success).toHaveBeenCalledWith('Email confirmed successfully!')
    })

    it('navigates to login when Sign In Now button is clicked', async () => {
      vi.mocked(authService.confirmEmail).mockResolvedValueOnce(undefined)
      
      renderWithRouter(['/confirm-email?token=valid-token'])
      
      await waitFor(() => {
        expect(screen.getByRole('button', { name: /sign in now/i })).toBeInTheDocument()
      })
      
      await user.click(screen.getByRole('button', { name: /sign in now/i }))
      
      expect(mockNavigate).toHaveBeenCalledWith('/login')
    })
  })

  describe('Error Handling', () => {
    it('displays error message when confirmation fails', async () => {
      const errorMessage = 'Confirmation failed'
      vi.mocked(authService.confirmEmail).mockRejectedValueOnce(new Error(errorMessage))
      
      renderWithRouter(['/confirm-email?token=invalid-token'])
      
      await waitFor(() => {
        expect(screen.getByText('Confirmation Failed')).toBeInTheDocument()
      })
      
      expect(screen.getByText(errorMessage)).toBeInTheDocument()
      expect(toast.error).toHaveBeenCalledWith('Email confirmation failed')
    })

    it('handles expired token error specifically', async () => {
      const expiredError = new Error('The confirmation link has expired')
      vi.mocked(authService.confirmEmail).mockRejectedValueOnce(expiredError)
      
      renderWithRouter(['/confirm-email?token=expired-token'])
      
      await waitFor(() => {
        expect(screen.getByText(/confirmation link has expired/i)).toBeInTheDocument()
      })
    })

    it('handles invalid token error specifically', async () => {
      const invalidError = new Error('The confirmation link is invalid')
      vi.mocked(authService.confirmEmail).mockRejectedValueOnce(invalidError)
      
      renderWithRouter(['/confirm-email?token=invalid-token'])
      
      await waitFor(() => {
        expect(screen.getByText(/confirmation link is invalid/i)).toBeInTheDocument()
      })
    })

    it('handles not found error specifically', async () => {
      const notFoundError = new Error('Token not found')
      vi.mocked(authService.confirmEmail).mockRejectedValueOnce(notFoundError)
      
      renderWithRouter(['/confirm-email?token=missing-token'])
      
      await waitFor(() => {
        expect(screen.getByText(/confirmation token was not found/i)).toBeInTheDocument()
      })
    })
  })

  describe('User Actions', () => {
    it('handles resend confirmation button click', async () => {
      renderWithRouter(['/confirm-email'])
      
      await waitFor(() => {
        expect(screen.getByRole('button', { name: /resend confirmation/i })).toBeInTheDocument()
      })
      
      await user.click(screen.getByRole('button', { name: /resend confirmation/i }))
      
      expect(toast).toHaveBeenCalledWith(
        'Please log in and request a new confirmation email from your profile.',
        expect.objectContaining({
          duration: 4000,
          icon: '📧',
        })
      )
      expect(mockNavigate).toHaveBeenCalledWith('/login')
    })

    it('handles back to login button click', async () => {
      renderWithRouter(['/confirm-email'])
      
      await waitFor(() => {
        expect(screen.getByRole('button', { name: /back to login/i })).toBeInTheDocument()
      })
      
      await user.click(screen.getByRole('button', { name: /back to login/i }))
      
      expect(mockNavigate).toHaveBeenCalledWith('/login')
    })
  })

  describe('API Integration', () => {
    it('calls authService.confirmEmail with correct token', async () => {
      const token = 'test-confirmation-token'
      vi.mocked(authService.confirmEmail).mockResolvedValueOnce(undefined)
      
      renderWithRouter([`/confirm-email?token=${token}`])
      
      await waitFor(() => {
        expect(authService.confirmEmail).toHaveBeenCalledWith(token)
      })
    })

    it('handles network errors gracefully', async () => {
      const networkError = new Error('Network error')
      vi.mocked(authService.confirmEmail).mockRejectedValueOnce(networkError)
      
      renderWithRouter(['/confirm-email?token=network-error-token'])
      
      await waitFor(() => {
        expect(screen.getByText('Confirmation Failed')).toBeInTheDocument()
      })
      
      expect(screen.getByText('Network error')).toBeInTheDocument()
    })
  })

  describe('Accessibility', () => {
    it('provides appropriate error announcements', async () => {
      vi.mocked(authService.confirmEmail).mockRejectedValueOnce(new Error('Test error'))
      
      renderWithRouter(['/confirm-email?token=error-token'])
      
      await waitFor(() => {
        // Error alert should be present for screen readers
        expect(screen.getByRole('alert')).toBeInTheDocument()
      })
    })
  })
})
