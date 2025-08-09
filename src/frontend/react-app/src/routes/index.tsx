import { createBrowserRouter, Navigate } from 'react-router-dom';
import { AppLayout } from '@/components/layout/AppLayout.js';
import { ProtectedRoute } from '@/components/authorization/ProtectedRoute.js';
import { LoginForm } from '@/components/auth/LoginForm.js';
import { RegisterForm } from '@/components/auth/RegisterForm.js';
import { Dashboard } from '@/pages/Dashboard.js';
import { UserList } from '@/components/users/UserList.js';
import { RoleList } from '@/components/roles/RoleList.js';
import { RoleEditor } from '@/components/roles/RoleEditor.js';
import { ROUTES } from './route.constants.js';
import { UserProfile } from '@/components/users/UserProfile.js';
import { UserRoleAssignment } from '@/components/users/UserRoleAssignment.js';

export const router = createBrowserRouter([
  // Public routes
  {
    path: ROUTES.LOGIN,
    element: <LoginForm />,
  },
  {
    path: ROUTES.REGISTER,
    element: <RegisterForm />,
  },
  {
    path: ROUTES.UNAUTHORIZED,
    element: (
      <div className="flex items-center justify-center min-h-screen">
        <div className="text-center">
          <h1 className="text-2xl font-bold mb-4">Access Denied</h1>
          <p className="text-gray-600">You don't have permission to access this page.</p>
        </div>
      </div>
    ),
  },
  
  // Protected routes with layout
  {
    path: ROUTES.HOME,
    element: (
      <ProtectedRoute>
        <AppLayout />
      </ProtectedRoute>
    ),
    children: [
      {
        index: true,
        element: <Navigate to={ROUTES.DASHBOARD} replace />,
      },
      {
        path: ROUTES.DASHBOARD,
        element: <Dashboard />,
      },
      {
        path: ROUTES.PROFILE,
        element: <UserProfile />,
      },
      {
        path: ROUTES.USERS,
        element: (
          <ProtectedRoute requirePermission="users.view">
            <UserList />
          </ProtectedRoute>
        ),
      },
      {
        path: '/users/:userId',
        element: (
          <ProtectedRoute requirePermission="users.view">
            <UserProfile />
          </ProtectedRoute>
        ),
      },
      {
        path: '/users/:userId/roles',
        element: (
          <ProtectedRoute requirePermission="users.manage_roles">
            <UserRoleAssignment />
          </ProtectedRoute>
        ),
      },
      {
        path: ROUTES.ROLES,
        element: (
          <ProtectedRoute requirePermission="roles.view">
            <RoleList />
          </ProtectedRoute>
        ),
      },
      {
        path: '/roles/new',
        element: (
          <ProtectedRoute requirePermission="roles.create">
            <RoleEditor />
          </ProtectedRoute>
        ),
      },
      {
        path: '/roles/:id/edit',
        element: (
          <ProtectedRoute requirePermission="roles.edit">
            <RoleEditor />
          </ProtectedRoute>
        ),
      },
      {
        path: ROUTES.ROLE_DETAILS,
        element: (
          <ProtectedRoute requirePermission="roles.view">
            <div>
              <h1>Role Details</h1>
              <p>Role details view will be implemented here.</p>
            </div>
          </ProtectedRoute>
        ),
      },
      {
        path: ROUTES.SETTINGS,
        element: (
          <div>
            <h1>Settings</h1>
            <p>Application settings will be implemented here.</p>
          </div>
        ),
      },
    ],
  },
  
  // Catch all route
  {
    path: '*',
    element: <Navigate to={ROUTES.DASHBOARD} replace />,
  },
]);
