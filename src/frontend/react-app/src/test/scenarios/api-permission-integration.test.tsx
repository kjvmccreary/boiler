// @ts-config-path ../../../tsconfig.vitest.json

import * as React from 'react'
import { describe, it, expect, beforeEach, afterEach } from 'vitest'
import { rbacRender, rbacUserEvent } from '../utils/rbac-test-utils.js'
import { screen, waitFor, cleanup, act } from '@testing-library/react'
import { server } from '../setup.js'
import { http, HttpResponse } from 'msw'

describe('API Permission Integration Scenarios', () => {

  beforeEach(() => {
    cleanup()
  })

  afterEach(() => {
    cleanup()
    server.resetHandlers()
  })

  describe('API Call Authorization', () => {
    const UserManagementComponent = () => {
      const [message, setMessage] = React.useState('')

      const handleCreateUser = async () => {
        try {
          const response = await fetch('/api/users', {
            method: 'POST',
            headers: {
              'Content-Type': 'application/json',
              'Authorization': 'Bearer mock-admin-token' // Fix: Add auth header
            },
            body: JSON.stringify({ name: 'New User' })
          })

          if (response.ok) {
            setMessage('success')
          } else if (response.status === 403) {
            setMessage('permission-denied')
          } else {
            setMessage('error')
          }
        } catch (error) {
          console.error('API call failed:', error)
          setMessage('error')
        }
      }

      return (
        <div>
          <button data-testid="create-user-btn" onClick={handleCreateUser}>
            Create User
          </button>
          {message === 'success' && (
            <div data-testid="success-message">User created successfully</div>
          )}
          {message === 'permission-denied' && (
            <div data-testid="permission-error">Permission denied</div>
          )}
          {message === 'error' && (
            <div data-testid="error-message">An error occurred</div>
          )}
        </div>
      )
    }

    it('should allow API calls for users with correct permissions', async () => {
      // Fix: Add specific MSW handler
      server.use(
        http.post('/api/users', ({ request }) => {
          const auth = request.headers.get('Authorization')
          if (auth?.includes('admin')) {
            return HttpResponse.json({
              success: true,
              data: { id: '1', name: 'New User' }
            })
          }
          return HttpResponse.json(
            { success: false, message: 'Unauthorized' },
            { status: 401 }
          )
        })
      )

      const { user } = rbacUserEvent.setupForRole('admin')

      rbacRender.asAdmin(<UserManagementComponent />)

      const createButton = screen.getByTestId('create-user-btn')

      await act(async () => {
        await user.click(createButton)
      })

      await waitFor(() => {
        expect(screen.getByTestId('success-message')).toBeInTheDocument()
      }, { timeout: 10000 })
    })

    it('should reject API calls for users without permissions', async () => {
      // Fix: Add specific MSW handler for viewer
      server.use(
        http.post('/api/users', ({ request }) => {
          const auth = request.headers.get('Authorization')
          if (auth?.includes('viewer')) {
            return HttpResponse.json(
              { success: false, message: 'Insufficient permissions' },
              { status: 403 }
            )
          }
          return HttpResponse.json({ success: true })
        })
      )

      const ViewerComponent = () => {
        const [message, setMessage] = React.useState('')

        const handleCreateUser = async () => {
          try {
            const response = await fetch('/api/users', {
              method: 'POST',
              headers: {
                'Content-Type': 'application/json',
                'Authorization': 'Bearer mock-viewer-token' // Fix: Add viewer auth
              },
              body: JSON.stringify({ name: 'New User' })
            })

            if (response.status === 403) {
              setMessage('permission-denied')
            } else if (response.ok) {
              setMessage('success')
            } else {
              setMessage('error')
            }
          } catch (error) {
            setMessage('error')
          }
        }

        return (
          <div>
            <button data-testid="create-user-btn" onClick={handleCreateUser}>
              Create User
            </button>
            {message === 'permission-denied' && (
              <div data-testid="permission-error">Permission denied</div>
            )}
          </div>
        )
      }

      const { user } = rbacUserEvent.setupForRole('viewer')

      rbacRender.asViewer(<ViewerComponent />)

      const createButton = screen.getByTestId('create-user-btn')

      await act(async () => {
        await user.click(createButton)
      })

      await waitFor(() => {
        expect(screen.getByTestId('permission-error')).toBeInTheDocument()
      }, { timeout: 10000 })
    })
  })

  describe('Role-Based Data Filtering', () => {
    const UserListComponent = ({ role }: { role: string }) => {
      const [users, setUsers] = React.useState([])
      const [loading, setLoading] = React.useState(true)

      React.useEffect(() => {
        const fetchUsers = async () => {
          try {
            const response = await fetch('/api/users', {
              headers: {
                'Authorization': `Bearer mock-${role}-token`
              }
            })
            const data = await response.json()
            setUsers(data.data?.items || [])
          } catch (error) {
            console.error('Failed to fetch users:', error)
          } finally {
            setLoading(false)
          }
        }

        fetchUsers()
      }, [role])

      if (loading) return <div data-testid="loading">Loading...</div>

      return (
        <div>
          <div data-testid="user-count">Users: {users.length}</div>
          {users.map((user: any, index: number) => (
            <div key={index} data-testid={`user-${index}`}>
              {user.name}
            </div>
          ))}
        </div>
      )
    }

    it('should filter user data based on role permissions', async () => {
      // Fix: Add proper MSW handler
      server.use(
        http.get('/api/users', ({ request }) => {
          const auth = request.headers.get('Authorization')
          if (auth?.includes('admin')) {
            return HttpResponse.json({
              success: true,
              data: {
                items: [
                  { id: '1', name: 'John Doe' },
                  { id: '2', name: 'Jane Smith' }
                ],
                totalCount: 2
              }
            })
          }
          return HttpResponse.json({
            success: true,
            data: { items: [], totalCount: 0 }
          })
        })
      )

      rbacRender.asAdmin(<UserListComponent role="admin" />)

      await waitFor(() => {
        expect(screen.queryByTestId('loading')).not.toBeInTheDocument()
      }, { timeout: 10000 })

      const userCount = screen.getByTestId('user-count')
      expect(userCount).toHaveTextContent(/Users: [1-9]/)
    })

    it('should limit data for lower privilege users', async () => {
      rbacRender.asViewer(<UserListComponent role="viewer" />)

      await waitFor(() => {
        expect(screen.queryByTestId('loading')).not.toBeInTheDocument()
      }, { timeout: 10000 })

      const userCount = screen.getByTestId('user-count')
      expect(userCount).toHaveTextContent('Users: 0')
    })
  })

  describe('Permission-Based Error Handling', () => {
    const ProtectedActionComponent = ({ role }: { role: string }) => {
      const [error, setError] = React.useState('')
      const [success, setSuccess] = React.useState('')

      const handleDeleteUser = async () => {
        try {
          const response = await fetch('/api/users/1', {
            method: 'DELETE',
            headers: {
              'Authorization': `Bearer mock-${role}-token`
            }
          })

          if (response.ok) {
            setSuccess('User deleted successfully')
            setError('')
          } else if (response.status === 403) {
            setError('You do not have permission to delete users')
            setSuccess('')
          }
        } catch (err) {
          setError('An unexpected error occurred')
          setSuccess('')
        }
      }

      return (
        <div>
          <button data-testid="delete-btn" onClick={handleDeleteUser}>
            Delete User
          </button>
          {error && <div data-testid="error-message">{error}</div>}
          {success && <div data-testid="success-message">{success}</div>}
        </div>
      )
    }

    it('should handle 403 Forbidden responses gracefully', async () => {
      server.use(
        http.delete('/api/users/:id', ({ request }) => {
          const auth = request.headers.get('Authorization')
          if (!auth?.includes('admin')) {
            return HttpResponse.json(
              { success: false, message: 'Insufficient permissions' },
              { status: 403 }
            )
          }
          return HttpResponse.json({ success: true })
        })
      )

      const { user } = rbacUserEvent.setupForRole('user')

      rbacRender.asUser(<ProtectedActionComponent role="user" />)

      const deleteButton = screen.getByTestId('delete-btn')

      await act(async () => {
        await user.click(deleteButton)
      })

      await waitFor(() => {
        expect(screen.getByTestId('error-message')).toHaveTextContent(
          'You do not have permission to delete users'
        )
      })
    })

    it('should handle successful operations for authorized users', async () => {
      const { user } = rbacUserEvent.setupForRole('admin')

      rbacRender.asAdmin(<ProtectedActionComponent role="admin" />)

      const deleteButton = screen.getByTestId('delete-btn')

      await act(async () => {
        await user.click(deleteButton)
      })

      await waitFor(() => {
        expect(screen.getByTestId('success-message')).toHaveTextContent(
          'User deleted successfully'
        )
      })
    })
  })
})
