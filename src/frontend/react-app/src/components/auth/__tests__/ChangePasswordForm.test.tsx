import { describe, it, expect, vi } from 'vitest'
import { render, screen, fireEvent, waitFor } from '../../../test/utils/test-utils.js'
import { ChangePasswordForm } from '../ChangePasswordForm.js'
import * as authService from '../../../services/auth.service.js'

// Mock the auth context
const mockLogout = vi.fn()
vi.mock('../../../contexts/AuthContext.js', () => ({
  useAuth: () => ({
    logout: mockLogout,
  })
}))

describe('ChangePasswordForm', () => {
  beforeEach(() => {
    vi.clearAllMocks()
  })

  it('renders the form fields', () => {
    render(<ChangePasswordForm />)
    
    expect(screen.getByText('Change Password')).toBeInTheDocument()
    expect(screen.getByLabelText(/current password/i)).toBeInTheDocument()
    expect(screen.getByLabelText(/new password/i)).toBeInTheDocument()
    expect(screen.getByLabelText(/confirm new password/i)).toBeInTheDocument()
    expect(screen.getByRole('button', { name: /change password/i })).toBeInTheDocument()
  })

  it('shows validation errors for empty fields', async () => {
    render(<ChangePasswordForm />)
    
    const submitButton = screen.getByRole('button', { name: /change password/i })
    fireEvent.click(submitButton)
    
    await waitFor(() => {
      expect(screen.getByText('Current password is required')).toBeInTheDocument()
      expect(screen.getByText('Password is required')).toBeInTheDocument()
      expect(screen.getByText('Please confirm your new password')).toBeInTheDocument()
    })
  })

  it('shows error when new password is same as current', async () => {
    render(<ChangePasswordForm />)
    
    const currentPasswordInput = screen.getByLabelText(/current password/i)
    const newPasswordInput = screen.getByLabelText(/new password/i)
    const confirmPasswordInput = screen.getByLabelText(/confirm new password/i)
    
    fireEvent.change(currentPasswordInput, { target: { value: 'Password123!' } })
    fireEvent.change(newPasswordInput, { target: { value: 'Password123!' } })
    fireEvent.change(confirmPasswordInput, { target: { value: 'Password123!' } })
    
    const submitButton = screen.getByRole('button', { name: /change password/i })
    fireEvent.click(submitButton)
    
    await waitFor(() => {
      expect(screen.getByText('New password must be different from current password')).toBeInTheDocument()
    })
  })

  it('shows error when passwords do not match', async () => {
    render(<ChangePasswordForm />)
    
    const currentPasswordInput = screen.getByLabelText(/current password/i)
    const newPasswordInput = screen.getByLabelText(/new password/i)
    const confirmPasswordInput = screen.getByLabelText(/confirm new password/i)
    
    fireEvent.change(currentPasswordInput, { target: { value: 'OldPassword123!' } })
    fireEvent.change(newPasswordInput, { target: { value: 'NewPassword123!' } })
    fireEvent.change(confirmPasswordInput, { target: { value: 'DifferentPassword123!' } })
    
    const submitButton = screen.getByRole('button', { name: /change password/i })
    fireEvent.click(submitButton)
    
    await waitFor(() => {
      expect(screen.getByText('New password and confirmation do not match')).toBeInTheDocument()
    })
  })

  it('successfully changes password and shows success screen', async () => {
    const mockChangePassword = vi.spyOn(authService.authService, 'changePassword')
    mockChangePassword.mockResolvedValueOnce()
    
    render(<ChangePasswordForm />)
    
    const currentPasswordInput = screen.getByLabelText(/current password/i)
    const newPasswordInput = screen.getByLabelText(/new password/i)
    const confirmPasswordInput = screen.getByLabelText(/confirm new password/i)
    
    fireEvent.change(currentPasswordInput, { target: { value: 'OldPassword123!' } })
    fireEvent.change(newPasswordInput, { target: { value: 'NewPassword123!' } })
    fireEvent.change(confirmPasswordInput, { target: { value: 'NewPassword123!' } })
    
    const submitButton = screen.getByRole('button', { name: /change password/i })
    fireEvent.click(submitButton)
    
    await waitFor(() => {
      expect(screen.getByText('Password Changed')).toBeInTheDocument()
      expect(screen.getByText('Your password has been successfully changed!')).toBeInTheDocument()
      expect(screen.getByRole('button', { name: /sign in again/i })).toBeInTheDocument()
    })
    
    expect(mockChangePassword).toHaveBeenCalledWith({
      currentPassword: 'OldPassword123!',
      newPassword: 'NewPassword123!',
      confirmNewPassword: 'NewPassword123!'
    })
  })

  it('toggles password visibility', () => {
    render(<ChangePasswordForm />)
    
    const currentPasswordInput = screen.getByLabelText(/current password/i)
    const toggleButton = screen.getAllByLabelText(/show password/i)[0]
    
    expect(currentPasswordInput).toHaveAttribute('type', 'password')
    
    fireEvent.click(toggleButton)
    
    expect(currentPasswordInput).toHaveAttribute('type', 'text')
  })

  it('calls logout when clicking "Sign In Again"', async () => {
    const mockChangePassword = vi.spyOn(authService.authService, 'changePassword')
    mockChangePassword.mockResolvedValueOnce()
    
    render(<ChangePasswordForm />)
    
    // Fill and submit form
    const currentPasswordInput = screen.getByLabelText(/current password/i)
    const newPasswordInput = screen.getByLabelText(/new password/i)
    const confirmPasswordInput = screen.getByLabelText(/confirm new password/i)
    
    fireEvent.change(currentPasswordInput, { target: { value: 'OldPassword123!' } })
    fireEvent.change(newPasswordInput, { target: { value: 'NewPassword123!' } })
    fireEvent.change(confirmPasswordInput, { target: { value: 'NewPassword123!' } })
    
    const submitButton = screen.getByRole('button', { name: /change password/i })
    fireEvent.click(submitButton)
    
    // Wait for success screen and click "Sign In Again"
    await waitFor(() => {
      const signInAgainButton = screen.getByRole('button', { name: /sign in again/i })
      fireEvent.click(signInAgainButton)
    })
    
    expect(mockLogout).toHaveBeenCalled()
  })
})
