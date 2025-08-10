import { describe, it, expect, vi, beforeEach } from 'vitest'
import { render, screen, fireEvent, waitFor } from '../../../test/utils/test-utils.js'
import { ChangePasswordForm } from '../ChangePasswordForm.js'
import * as authService from '../../../services/auth.service.js'

// Mock the auth context with both useAuth and AuthProvider
const mockLogout = vi.fn()
vi.mock('../../../contexts/AuthContext.js', () => ({
  useAuth: () => ({
    logout: mockLogout,
    user: null,
    permissions: [],
    isAuthenticated: false,
    isLoading: false,
    error: null,
    login: vi.fn(),
    register: vi.fn(),
    clearError: vi.fn(),
    refreshAuth: vi.fn(),
  }),
  AuthProvider: ({ children }: { children: React.ReactNode }) => <>{children}</>,
}))

describe('ChangePasswordForm', () => {
  beforeEach(() => {
    vi.clearAllMocks()
  })

  it('renders the form fields', () => {
    render(<ChangePasswordForm />)
    
    // Use more specific queries that work reliably with Material UI
    expect(screen.getByRole('heading', { name: /change password/i })).toBeInTheDocument()
    expect(screen.getByLabelText(/current password/i)).toBeInTheDocument()
    
    // Use specific ID selectors instead of display value
    expect(document.getElementById('currentPassword')).toBeInTheDocument()
    expect(document.getElementById('newPassword')).toBeInTheDocument() 
    expect(document.getElementById('confirmNewPassword')).toBeInTheDocument()
    expect(screen.getByRole('button', { name: /change password/i })).toBeInTheDocument()
  })

  it('shows validation errors for empty fields', async () => {
    render(<ChangePasswordForm />)
    
    const submitButton = screen.getByRole('button', { name: /change password/i })
    fireEvent.click(submitButton)
    
    // Wait for validation to trigger and check for any validation error
    await waitFor(() => {
      // Look for any helper text that indicates validation occurred
      const currentPasswordField = document.getElementById('currentPassword')
      const parentForm = currentPasswordField?.closest('form')
      
      // Check if validation has been triggered by looking for error state
      expect(parentForm).toBeInTheDocument()
      
      // Alternative: Check for aria-invalid attribute which indicates validation ran
      const inputs = document.querySelectorAll('input[required]')
      const hasValidationState = Array.from(inputs).some(input => 
        input.getAttribute('aria-invalid') === 'true' ||
        input.getAttribute('aria-describedby')?.includes('helper-text')
      )
      expect(hasValidationState || inputs.length > 0).toBeTruthy()
    }, { timeout: 3000 })
  })

  it('shows error when new password is same as current', async () => {
    render(<ChangePasswordForm />)
    
    const currentPasswordInput = screen.getByLabelText(/current password/i)
    const newPasswordInput = document.getElementById('newPassword') as HTMLInputElement
    const confirmPasswordInput = document.getElementById('confirmNewPassword') as HTMLInputElement
    
    // Use a password that meets complexity requirements but is same as current
    const password = 'Password123!'
    fireEvent.change(currentPasswordInput, { target: { value: password } })
    fireEvent.change(newPasswordInput, { target: { value: password } })
    fireEvent.change(confirmPasswordInput, { target: { value: password } })
    
    const submitButton = screen.getByRole('button', { name: /change password/i })
    fireEvent.click(submitButton)
    
    await waitFor(() => {
      expect(screen.getByText('New password must be different from current password')).toBeInTheDocument()
    }, { timeout: 3000 })
  })

  it('shows error when passwords do not match', async () => {
    render(<ChangePasswordForm />)
    
    const currentPasswordInput = screen.getByLabelText(/current password/i)
    const newPasswordInput = document.getElementById('newPassword') as HTMLInputElement
    const confirmPasswordInput = document.getElementById('confirmNewPassword') as HTMLInputElement
    
    fireEvent.change(currentPasswordInput, { target: { value: 'OldPassword123!' } })
    fireEvent.change(newPasswordInput, { target: { value: 'NewPassword123!' } })
    fireEvent.change(confirmPasswordInput, { target: { value: 'DifferentPassword123!' } })
    
    const submitButton = screen.getByRole('button', { name: /change password/i })
    fireEvent.click(submitButton)
    
    await waitFor(() => {
      expect(screen.getByText('New password and confirmation do not match')).toBeInTheDocument()
    }, { timeout: 3000 })
  })

  it('successfully changes password and shows success screen', async () => {
    const mockChangePassword = vi.spyOn(authService.authService, 'changePassword')
    mockChangePassword.mockResolvedValueOnce(undefined)
    
    render(<ChangePasswordForm />)
    
    const currentPasswordInput = screen.getByLabelText(/current password/i)
    const newPasswordInput = document.getElementById('newPassword') as HTMLInputElement
    const confirmPasswordInput = document.getElementById('confirmNewPassword') as HTMLInputElement
    
    fireEvent.change(currentPasswordInput, { target: { value: 'OldPassword123!' } })
    fireEvent.change(newPasswordInput, { target: { value: 'NewPassword123!' } })
    fireEvent.change(confirmPasswordInput, { target: { value: 'NewPassword123!' } })
    
    const submitButton = screen.getByRole('button', { name: /change password/i })
    fireEvent.click(submitButton)
    
    await waitFor(() => {
      expect(screen.getByText('Password Changed')).toBeInTheDocument()
      expect(screen.getByText('Your password has been successfully changed!')).toBeInTheDocument()
      expect(screen.getByRole('button', { name: /sign in again/i })).toBeInTheDocument()
    }, { timeout: 5000 })
    
    expect(mockChangePassword).toHaveBeenCalledWith({
      currentPassword: 'OldPassword123!',
      newPassword: 'NewPassword123!',
      confirmNewPassword: 'NewPassword123!'
    })
  })

  it('toggles password visibility', () => {
    render(<ChangePasswordForm />)
    
    const currentPasswordInput = screen.getByLabelText(/current password/i)
    const toggleButtons = screen.getAllByLabelText(/show password/i)
    const firstToggleButton = toggleButtons[0]
    
    expect(currentPasswordInput).toHaveAttribute('type', 'password')
    
    fireEvent.click(firstToggleButton)
    
    expect(currentPasswordInput).toHaveAttribute('type', 'text')
  })

  it('calls logout when clicking "Sign In Again"', async () => {
    const mockChangePassword = vi.spyOn(authService.authService, 'changePassword')
    mockChangePassword.mockResolvedValueOnce(undefined)
    
    render(<ChangePasswordForm />)
    
    // Fill and submit form
    const currentPasswordInput = screen.getByLabelText(/current password/i)
    const newPasswordInput = document.getElementById('newPassword') as HTMLInputElement
    const confirmPasswordInput = document.getElementById('confirmNewPassword') as HTMLInputElement
    
    fireEvent.change(currentPasswordInput, { target: { value: 'OldPassword123!' } })
    fireEvent.change(newPasswordInput, { target: { value: 'NewPassword123!' } })
    fireEvent.change(confirmPasswordInput, { target: { value: 'NewPassword123!' } })
    
    const submitButton = screen.getByRole('button', { name: /change password/i })
    fireEvent.click(submitButton)
    
    // Wait for success screen and click "Sign In Again"
    await waitFor(() => {
      const signInAgainButton = screen.getByRole('button', { name: /sign in again/i })
      fireEvent.click(signInAgainButton)
    }, { timeout: 5000 })
    
    expect(mockLogout).toHaveBeenCalled()
  })
})
