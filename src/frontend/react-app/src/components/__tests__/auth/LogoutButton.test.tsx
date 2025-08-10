import { describe, it, expect, vi, beforeEach } from 'vitest'
import { render, screen, waitFor } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import { BrowserRouter } from 'react-router-dom'
import { LogoutButton } from '@/components/auth/LogoutButton.js'
import toast from 'react-hot-toast'

// Mock dependencies
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

// Mock useAuth properly
const mockLogout = vi.fn()
const mockAuthContext = {
  logout: mockLogout,
  login: vi.fn(),
  register: vi.fn(),
  clearError: vi.fn(),
  refreshAuth: vi.fn(),
  user: {
    id: '1',
    email: 'test@example.com',
    firstName: 'Test',
    lastName: 'User',
    fullName: 'Test User',
    phoneNumber: '+1234567890',
    timeZone: 'UTC',
    language: 'en',
    lastLoginAt: '2024-01-01T00:00:00Z',
    emailConfirmed: true,
    isActive: true,
    roles: [],
    tenantId: '1',
    createdAt: '2024-01-01T00:00:00Z',
    updatedAt: '2024-01-01T00:00:00Z'
  },
  permissions: [],
  isAuthenticated: true,
  isLoading: false,
  error: null,
}

vi.mock('@/contexts/AuthContext.js', () => ({
  useAuth: () => mockAuthContext,
}))

// Helper function to render component with router
const renderWithRouter = (props: any = {}) => {
  return render(
    <BrowserRouter>
      <LogoutButton {...props} />
    </BrowserRouter>
  )
}

describe('LogoutButton', () => {
  const user = userEvent.setup()
  
  beforeEach(() => {
    vi.clearAllMocks()
    mockNavigate.mockClear()
    mockLogout.mockClear()
  })

  describe('Button Variants', () => {
    it('renders button variant by default', () => {
      renderWithRouter()
      
      const button = screen.getByRole('button', { name: /logout/i })
      expect(button).toBeInTheDocument()
      expect(button).toHaveClass('MuiButton-contained')
    })

    it('renders icon variant correctly', () => {
      renderWithRouter({ variant: 'icon', tooltip: 'Sign out' })
      
      const iconButton = screen.getByRole('button')
      expect(iconButton).toBeInTheDocument()
    })

    it('renders text variant correctly', () => {
      renderWithRouter({ variant: 'text' })
      
      const button = screen.getByRole('button', { name: /logout/i })
      expect(button).toBeInTheDocument()
      expect(button).toHaveClass('MuiButton-text')
    })

    it('renders custom children text', () => {
      renderWithRouter({ children: 'Sign Out' })
      
      expect(screen.getByRole('button', { name: /sign out/i })).toBeInTheDocument()
    })
  })

  describe('Logout Functionality', () => {
    it('performs logout without confirmation by default', async () => {
      mockLogout.mockResolvedValueOnce(undefined)
      
      renderWithRouter()
      
      await user.click(screen.getByRole('button', { name: /logout/i }))
      
      await waitFor(() => {
        expect(mockLogout).toHaveBeenCalled()
        expect(toast.success).toHaveBeenCalledWith('Logged out successfully')
        expect(mockNavigate).toHaveBeenCalledWith('/login')
      })
    })

    it('shows confirmation dialog when showConfirmation is true', async () => {
      renderWithRouter({ showConfirmation: true })
      
      await user.click(screen.getByRole('button', { name: /logout/i }))
      
      // Confirmation dialog should appear
      expect(screen.getByText('Confirm Logout')).toBeInTheDocument()
      expect(screen.getByText(/are you sure you want to logout/i)).toBeInTheDocument()
      expect(screen.getByRole('button', { name: /cancel/i })).toBeInTheDocument()
      
      // Logout should not be called yet
      expect(mockLogout).not.toHaveBeenCalled()
    })

    it('cancels logout from confirmation dialog', async () => {
      renderWithRouter({ showConfirmation: true })
      
      await user.click(screen.getByRole('button', { name: /logout/i }))
      await user.click(screen.getByRole('button', { name: /cancel/i }))
      
      await waitFor(() => {
        expect(screen.queryByText('Confirm Logout')).not.toBeInTheDocument()
      })
      expect(mockLogout).not.toHaveBeenCalled()
    })

    it('confirms logout from confirmation dialog', async () => {
      mockLogout.mockResolvedValueOnce(undefined)
      
      renderWithRouter({ showConfirmation: true })
      
      await user.click(screen.getByRole('button', { name: /logout/i }))
      
      // Click logout in dialog (find the confirmation logout button)
      const logoutButtons = screen.getAllByRole('button', { name: /logout/i })
      const confirmButton = logoutButtons.find(btn => 
        btn.closest('[role="dialog"]') !== null
      )
      
      await user.click(confirmButton!)
      
      await waitFor(() => {
        expect(mockLogout).toHaveBeenCalled()
        expect(toast.success).toHaveBeenCalledWith('Logged out successfully')
        expect(mockNavigate).toHaveBeenCalledWith('/login')
      })
    })
  })

  describe('Error Handling', () => {
    it('handles logout error gracefully', async () => {
      const error = new Error('Logout failed')
      mockLogout.mockRejectedValueOnce(error)
      
      const onError = vi.fn()
      renderWithRouter({ onError })
      
      await user.click(screen.getByRole('button', { name: /logout/i }))
      
      await waitFor(() => {
        expect(toast.error).toHaveBeenCalledWith('Failed to logout. Please try again.')
        expect(onError).toHaveBeenCalledWith(error)
      })
      
      // Should not navigate on error
      expect(mockNavigate).not.toHaveBeenCalled()
    })

    it('shows loading state during logout', async () => {
      // Create a promise that resolves after delay
      let resolveLogout: () => void
      const logoutPromise = new Promise<void>((resolve) => {
        resolveLogout = resolve
      })
      mockLogout.mockReturnValueOnce(logoutPromise)
      
      renderWithRouter()
      
      const button = screen.getByRole('button', { name: /logout/i })
      
      await user.click(button)
      
      // Check loading state
      await waitFor(() => {
        expect(button).toBeDisabled()
        expect(screen.getByRole('progressbar')).toBeInTheDocument()
      })
      
      // Resolve the logout
      resolveLogout!()
      
      await waitFor(() => {
        expect(mockNavigate).toHaveBeenCalled()
      })
    })
  })

  describe('Accessibility', () => {
    it('has proper ARIA attributes for icon variant', () => {
      renderWithRouter({ variant: 'icon' })
      
      const button = screen.getByRole('button')
      const tooltip = button.closest('[aria-label]')
      expect(tooltip).toBeDefined()
    })

    it('manages dialog accessibility correctly', async () => {
      renderWithRouter({ showConfirmation: true })
      
      await user.click(screen.getByRole('button', { name: /logout/i }))
      
      const dialog = screen.getByRole('dialog')
      expect(dialog).toBeInTheDocument()
      expect(dialog).toHaveAttribute('aria-labelledby', 'logout-dialog-title')
      expect(dialog).toHaveAttribute('aria-describedby', 'logout-dialog-description')
    })
  })
})
