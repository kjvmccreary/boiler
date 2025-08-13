import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import { AppLayout } from '@/components/layout/AppLayout.js';
import { ProtectedRoute } from '@/components/authorization/ProtectedRoute.js';
import { LoginForm } from '@/components/auth/LoginForm.js';
import { RegisterForm } from '@/components/auth/RegisterForm.js';
import { ChangePasswordForm } from '@/components/auth/ChangePasswordForm.js';
import { UserList } from '@/components/users/UserList.js';
import { UserProfile } from '@/components/users/UserProfile.js';
import { UserRoleAssignment } from '@/components/users/UserRoleAssignment.js';
import { RoleManagement } from '@/pages/RoleManagement.js'; // ✅ ADD: Import RoleManagement
import { RoleEditor } from '@/components/roles/RoleEditor.js';
import { RoleDetails } from '@/components/roles/RoleDetails.js';
import { Dashboard } from '@/pages/Dashboard.js';
import { CreateUser } from '@/components/users/CreateUser.js';

// Router-free routes component for testing
export function AppRoutesConfig() {
  return (
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

        <Route path="/users/new" element={
          <ProtectedRoute requirePermission="users.create">
            <CreateUser />
          </ProtectedRoute>
        } />

        {/* ✅ UPDATED: Role management routes */}
        <Route path="roles" element={
          <ProtectedRoute requirePermission="roles.view">
            <RoleManagement />
          </ProtectedRoute>
        } />
        
        <Route path="roles/:id" element={
          <ProtectedRoute requirePermission="roles.view">
            <RoleDetails />
          </ProtectedRoute>
        } />
        
        <Route path="roles/:id/edit" element={
          <ProtectedRoute requirePermission="roles.edit">
            <RoleEditor />
          </ProtectedRoute>
        } />
        
        <Route path="roles/new" element={
          <ProtectedRoute requirePermission="roles.create">
            <RoleEditor />
          </ProtectedRoute>
        } />
      </Route>

      {/* Fallback */}
      <Route path="*" element={<Navigate to="/dashboard" replace />} />
    </Routes>
  );
}

// Main app routes component with router
export function AppRoutes() {
  return (
    <BrowserRouter>
      <AppRoutesConfig />
    </BrowserRouter>
  );
}
