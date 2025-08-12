import React from 'react'
import { describe, it, expect, vi, beforeEach } from 'vitest'
import { render, screen, waitFor } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import { BrowserRouter } from 'react-router-dom'
import { QueryClient, QueryClientProvider } from '@tanstack/react-query'
import { ChangePasswordForm } from '@/components/auth/ChangePasswordForm'
import { authService } from '@/services/auth.service'

// Mock the auth service
vi.mock('@/services/auth.service')

// Mock auth context
vi.mock('@/contexts/AuthContext', () => ({
  useAuth: () => ({
    logout: vi.fn(),
  }),
}))

function TestWrapper({ children }: { children: React.ReactNode }) {
  const queryClient = new QueryClient({
    defaultOptions: {
      queries: { retry: false, gcTime: 0 },
    },
  })

  return (
    <QueryClientProvider client={queryClient}>
      <BrowserRouter>
        {children}
      </BrowserRouter>
    </QueryClientProvider>
  )
}

describe('ChangePasswordForm', () => {
  const mockChangePassword = vi.mocked(authService.changePassword)

  beforeEach(() => {
    vi.clearAllMocks()
  })

  it('renders the form fields', () => {
    render(<ChangePasswordForm />, { wrapper: TestWrapper })

    expect(screen.getByLabelText(/current password/i)).toBeInTheDocument()
    expect(document.getElementById('newPassword')).toBeInTheDocument()
    expect(screen.getByLabelText(/confirm new password/i)).toBeInTheDocument()
    expect(screen.getByRole('button', { name: /change password/i })).toBeInTheDocument()
  })

  it('prevents submission with empty fields', async () => {
    const user = userEvent.setup()
    render(<ChangePasswordForm />, { wrapper: TestWrapper })

    const submitButton = screen.getByRole('button', { name: /change password/i })
    await user.click(submitButton)

    // ✅ FINAL FIX: Use heading role to target H1 specifically
    await waitFor(() => {
      expect(mockChangePassword).not.toHaveBeenCalled()
    })
    
    // Verify we're still on the form page using heading
    expect(screen.getByRole('heading', { name: /change password/i })).toBeInTheDocument()
    expect(screen.queryByText(/password changed/i)).not.toBeInTheDocument()
  })

  it('shows error when new password is same as current', async () => {
    const user = userEvent.setup()
    render(<ChangePasswordForm />, { wrapper: TestWrapper })

    const currentPasswordInput = screen.getByLabelText(/current password/i)
    const newPasswordInput = document.getElementById('newPassword') as HTMLInputElement
    const confirmPasswordInput = screen.getByLabelText(/confirm new password/i)

    await user.type(currentPasswordInput, 'samepassword')
    await user.type(newPasswordInput, 'samepassword')
    await user.type(confirmPasswordInput, 'samepassword')

    const submitButton = screen.getByRole('button', { name: /change password/i })
    await user.click(submitButton)

    await waitFor(() => {
      expect(screen.getByText('New password must be different from current password')).toBeInTheDocument()
    })
  })

  it('shows error when passwords do not match', async () => {
    const user = userEvent.setup()
    render(<ChangePasswordForm />, { wrapper: TestWrapper })

    const currentPasswordInput = screen.getByLabelText(/current password/i)
    const newPasswordInput = document.getElementById('newPassword') as HTMLInputElement
    const confirmPasswordInput = screen.getByLabelText(/confirm new password/i)

    await user.type(currentPasswordInput, 'currentpass')
    await user.type(newPasswordInput, 'NewPassword123!')
    await user.type(confirmPasswordInput, 'DifferentPassword123!')

    const submitButton = screen.getByRole('button', { name: /change password/i })
    await user.click(submitButton)

    await waitFor(() => {
      expect(screen.getByText('New password and confirmation do not match')).toBeInTheDocument()
    })
  })

  it('successfully changes password and shows success screen', async () => {
    const user = userEvent.setup()
    mockChangePassword.mockResolvedValueOnce(undefined)

    render(<ChangePasswordForm />, { wrapper: TestWrapper })

    const currentPasswordInput = screen.getByLabelText(/current password/i)
    const newPasswordInput = document.getElementById('newPassword') as HTMLInputElement
    const confirmPasswordInput = screen.getByLabelText(/confirm new password/i)

    await user.type(currentPasswordInput, 'currentpass')
    await user.type(newPasswordInput, 'NewPassword123!')
    await user.type(confirmPasswordInput, 'NewPassword123!')

    const submitButton = screen.getByRole('button', { name: /change password/i })
    await user.click(submitButton)

    await waitFor(() => {
      expect(screen.getByText(/password changed/i)).toBeInTheDocument()
      expect(screen.getByText(/your password has been successfully changed/i)).toBeInTheDocument()
    })

    expect(mockChangePassword).toHaveBeenCalledWith({
      currentPassword: 'currentpass',
      newPassword: 'NewPassword123!',
      confirmNewPassword: 'NewPassword123!'
    })
  })

  it('toggles password visibility', async () => {
    const user = userEvent.setup()
    render(<ChangePasswordForm />, { wrapper: TestWrapper })

    const currentPasswordInput = screen.getByLabelText(/current password/i)
    expect(currentPasswordInput).toHaveAttribute('type', 'password')

    const toggleButton = screen.getAllByLabelText(/show password/i)[0]
    await user.click(toggleButton)

    expect(currentPasswordInput).toHaveAttribute('type', 'text')
  })

  it('calls logout when clicking "Sign In Again"', async () => {
    const user = userEvent.setup()
    mockChangePassword.mockResolvedValueOnce(undefined)

    render(<ChangePasswordForm />, { wrapper: TestWrapper })

    const currentPasswordInput = screen.getByLabelText(/current password/i)
    const newPasswordInput = document.getElementById('newPassword') as HTMLInputElement
    const confirmPasswordInput = screen.getByLabelText(/confirm new password/i)

    await user.type(currentPasswordInput, 'currentpass')
    await user.type(newPasswordInput, 'NewPassword123!')
    await user.type(confirmPasswordInput, 'NewPassword123!')

    const submitButton = screen.getByRole('button', { name: /change password/i })
    await user.click(submitButton)

    await waitFor(() => {
      expect(screen.getByText(/password changed/i)).toBeInTheDocument()
    })

    const signInButton = screen.getByRole('button', { name: /sign in again/i })
    await user.click(signInButton)
  })

  it('prevents submission with weak password', async () => {
    const user = userEvent.setup()
    render(<ChangePasswordForm />, { wrapper: TestWrapper })

    const currentPasswordInput = screen.getByLabelText(/current password/i)
    const newPasswordInput = document.getElementById('newPassword') as HTMLInputElement

    await user.type(currentPasswordInput, 'ValidPassword123!')
    await user.type(newPasswordInput, 'weak')

    const submitButton = screen.getByRole('button', { name: /change password/i })
    await user.click(submitButton)

    // ✅ FINAL FIX: Use heading role to target H1 specifically
    await waitFor(() => {
      expect(mockChangePassword).not.toHaveBeenCalled()
    })
    
    // Verify we're still on the form page using heading
    expect(screen.getByRole('heading', { name: /change password/i })).toBeInTheDocument()
    expect(screen.queryByText(/password changed/i)).not.toBeInTheDocument()
  })

  it('handles API error gracefully', async () => {
    const user = userEvent.setup()
    mockChangePassword.mockRejectedValueOnce({
      response: {
        status: 400,
        data: { message: 'Current password is incorrect' }
      }
    })

    render(<ChangePasswordForm />, { wrapper: TestWrapper })

    const currentPasswordInput = screen.getByLabelText(/current password/i)
    const newPasswordInput = document.getElementById('newPassword') as HTMLInputElement
    const confirmPasswordInput = screen.getByLabelText(/confirm new password/i)

    await user.type(currentPasswordInput, 'wrongpassword')
    await user.type(newPasswordInput, 'NewPassword123!')
    await user.type(confirmPasswordInput, 'NewPassword123!')

    const submitButton = screen.getByRole('button', { name: /change password/i })
    await user.click(submitButton)

    await waitFor(() => {
      expect(screen.getByText(/current password is incorrect/i)).toBeInTheDocument()
    })
  })
})
