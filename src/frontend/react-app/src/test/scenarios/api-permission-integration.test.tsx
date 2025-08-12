// @ts-config-path ../../../tsconfig.vitest.json

import * as React from 'react' // ✅ FIX: Use * as React import
//import { describe, it, expect, beforeEach } from 'vitest'
import { rbacRender, rbacUserEvent } from '../utils/rbac-test-utils.js'
import { screen, waitFor } from '@testing-library/react'
import { server } from '../setup.js' // 🔧 FIX: Use your existing server
import { http, HttpResponse } from 'msw'

// 🔧 .NET 9 RBAC: API Permission Integration Testing
describe('API Permission Integration Scenarios', () => {

  // 🔧 SCENARIO 1: API Call Authorization
  describe('API Call Authorization', () => {
    const UserManagementComponent = () => {
      const handleCreateUser = async () => {
        try {
          const response = await fetch('http://localhost:5000/api/users', {method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ name: 'New User' })
          })
          
          if (response.ok) {
            // Success handling
            const successDiv = document.createElement('div')
            successDiv.setAttribute('data-testid', 'success-message')
            successDiv.textContent = 'User created successfully'
            document.body.appendChild(successDiv)
          } else if (response.status === 403) {
            // Permission denied
            const errorDiv = document.createElement('div')
            errorDiv.setAttribute('data-testid', 'permission-error')
            errorDiv.textContent = 'Permission denied'
            document.body.appendChild(errorDiv)
          }
        } catch (error) {
          console.error('API call failed:', error)
        }
      }

      return (
        <div>
          <button data-testid="create-user-btn" onClick={handleCreateUser}>
            Create User
          </button>
        </div>
      )
    }

    beforeEach(() => {
      // Clear any existing DOM modifications
      document.body.innerHTML = ''
    })

    it('should allow API calls for users with correct permissions', async () => {
      const { user } = rbacUserEvent.setupForRole('admin')
      
      rbacRender.asAdmin(<UserManagementComponent />)
      
      const createButton = screen.getByTestId('create-user-btn')
      await user.click(createButton)
      
      await waitFor(() => {
        expect(screen.getByTestId('success-message')).toBeInTheDocument()
      })
    })

    it('should reject API calls for users without permissions', async () => {
      // Override MSW handler to return 403 for viewer
      server.use(
        http.post('/api/users', ({ request }) => {
          const auth = request.headers.get('Authorization')
          if (auth?.includes('viewer')) {
            return HttpResponse.json(
              { success: false, message: 'Insufficient permissions' },
              { status: 403 }
            )
          }
          return HttpResponse.json({ success: true, data: {} })
        })
      )

      const { user } = rbacUserEvent.setupForRole('viewer')
      
      rbacRender.asViewer(<UserManagementComponent />)
      
      const createButton = screen.getByTestId('create-user-btn')
      await user.click(createButton)
      
      await waitFor(() => {
        expect(screen.getByTestId('permission-error')).toBeInTheDocument()
      })
    })
  })

  // 🔧 SCENARIO 2: Role-Based Data Filtering
  describe('Role-Based Data Filtering', () => {
    const UserListComponent = () => {
      const [users, setUsers] = React.useState([])
      const [loading, setLoading] = React.useState(true)

      React.useEffect(() => {
        fetch('/api/users')
          .then(res => res.json())
          .then(data => {
            setUsers(data.data?.items || [])
            setLoading(false)
          })
      }, [])

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
      // Admin should see all users
      rbacRender.asAdmin(<UserListComponent />)
      
      await waitFor(() => {
        expect(screen.queryByTestId('loading')).not.toBeInTheDocument()
      })
      
      // Should see multiple users (based on MSW mock data)
      const userCount = screen.getByTestId('user-count')
      expect(userCount).toHaveTextContent(/Users: [1-9]/)
    })

    it('should limit data for lower privilege users', async () => {
      // Override MSW to return limited data for viewers
      server.use(
        http.get('/api/users', ({ request }) => {
          const auth = request.headers.get('Authorization')
          if (auth?.includes('viewer')) {
            return HttpResponse.json({
              success: true,
              data: {
                items: [],
                totalCount: 0,
                pageNumber: 1,
                pageSize: 10,
                totalPages: 0
              }
            })
          }
          // Return default response for other roles
          return HttpResponse.json({
            success: true,
            data: {
              items: [
                { id: '1', name: 'John Doe' },
                { id: '2', name: 'Jane Smith' }
              ],
              totalCount: 2,
              pageNumber: 1,
              pageSize: 10,
              totalPages: 1
            }
          })
        })
      )

      rbacRender.asViewer(<UserListComponent />)
      
      await waitFor(() => {
        expect(screen.queryByTestId('loading')).not.toBeInTheDocument()
      })
      
      // Viewer should see no users due to permission restrictions
      const userCount = screen.getByTestId('user-count')
      expect(userCount).toHaveTextContent('Users: 0')
    })
  })

  // 🔧 SCENARIO 3: Permission-Based Error Handling
  describe('Permission-Based Error Handling', () => {
    const ProtectedActionComponent = () => {
      const [error, setError] = React.useState('')
      const [success, setSuccess] = React.useState('')

      const handleDeleteUser = async () => {
        try {
          const response = await fetch('/api/users/1', { method: 'DELETE' })
          
          if (response.ok) {
            setSuccess('User deleted successfully')
            setError('')
          } else if (response.status === 403) {
            setError('You do not have permission to delete users')
            setSuccess('')
          } else if (response.status === 401) {
            setError('Please log in to continue')
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
      // Override MSW to return 403 for non-admin users
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
      
      rbacRender.asUser(<ProtectedActionComponent />)
      
      const deleteButton = screen.getByTestId('delete-btn')
      await user.click(deleteButton)
      
      await waitFor(() => {
        expect(screen.getByTestId('error-message')).toHaveTextContent(
          'You do not have permission to delete users'
        )
      })
    })

    it('should handle successful operations for authorized users', async () => {
      const { user } = rbacUserEvent.setupForRole('admin')
      
      rbacRender.asAdmin(<ProtectedActionComponent />)
      
      const deleteButton = screen.getByTestId('delete-btn')
      await user.click(deleteButton)
      
      await waitFor(() => {
        expect(screen.getByTestId('success-message')).toHaveTextContent(
          'User deleted successfully'
        )
      })
    })
  })
})
