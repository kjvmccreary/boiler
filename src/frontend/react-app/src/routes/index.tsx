import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import { AppLayout } from '@/components/layout/AppLayout.js';
import { ProtectedRoute } from '@/components/authorization/ProtectedRoute.js';
import { LoginForm } from '@/components/auth/LoginForm.js';
import { RegisterForm } from '@/components/auth/RegisterForm.js';
import { ChangePasswordForm } from '@/components/auth/ChangePasswordForm.js';
import { UserList } from '@/components/users/UserList.js';
import { UserProfile } from '@/components/users/UserProfile.js';
import { UserRoleAssignment } from '@/components/users/UserRoleAssignment.js';
import { RoleList } from '@/components/roles/RoleList.js';
import { RoleEditor } from '@/components/roles/RoleEditor.js';
import { Dashboard } from '@/pages/Dashboard.js';

export function AppRoutes() {
  return (
    <BrowserRouter>
      <Routes>
        {/* Public routes */}
        <Route path="/login" element={<LoginForm />} />
        <Route path="/register" element={<RegisterForm />} />

        {/* Protected routes */}
        <Route path="/" element={<AppLayout />}>
          <Route index element={<Navigate to="/dashboard" replace />} />
          
          <Route path="dashboard" element={
            <ProtectedRoute>
              <Dashboard />
            </ProtectedRoute>
          } />

          <Route path="change-password" element={
            <ProtectedRoute>
              <ChangePasswordForm />
            </ProtectedRoute>
          } />

          {/* User management routes */}
          <Route path="users" element={
            <ProtectedRoute requirePermission="users.view">
              <UserList />
            </ProtectedRoute>
          } />
          
          <Route path="users/:userId" element={
            <ProtectedRoute requirePermission="users.view">
              <UserProfile />
            </ProtectedRoute>
          } />
          
          <Route path="users/:userId/roles" element={
            <ProtectedRoute requirePermission="users.manage_roles">
              <UserRoleAssignment />
            </ProtectedRoute>
          } />

          {/* Role management routes */}
          <Route path="roles" element={
            <ProtectedRoute requirePermission="roles.view">
              <RoleList />
            </ProtectedRoute>
          } />
          
          <Route path="roles/:id" element={
            <ProtectedRoute requirePermission="roles.edit">
              <RoleEditor />
            </ProtectedRoute>
          } />
        </Route>

        {/* Fallback */}
        <Route path="*" element={<Navigate to="/dashboard" replace />} />
      </Routes>
    </BrowserRouter>
  );
}
