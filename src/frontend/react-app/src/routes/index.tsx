import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import { AppLayout } from '@/components/layout/AppLayout.js';
import { ProtectedRoute } from '@/components/authorization/ProtectedRoute.js';
import { EnhancedLoginForm } from '@/components/auth/EnhancedLoginForm.js';
import { SelfServeRegisterForm } from '@/components/auth/SelfServeRegisterForm.js';
import { ChangePasswordForm } from '@/components/auth/ChangePasswordForm.js';
import { UserList } from '@/components/users/UserList.js';
import { UserProfile } from '@/components/users/UserProfile.js';
import { UserRoleAssignment } from '@/components/users/UserRoleAssignment.js';
import { RoleManagement } from '@/pages/RoleManagement.js';
import { RoleEditor } from '@/components/roles/RoleEditor.js';
import { RoleDetails } from '@/components/roles/RoleDetails.js';
import { Dashboard } from '@/pages/Dashboard.js';
import { CreateUser } from '@/components/users/CreateUser.js';
import { LandingPage } from '@/components/landing/LandingPage.js';
import { UnauthorizedPage } from '@/components/common/UnauthorizedPage.js'; // 🔧 ADD

// Router-free routes component for testing
export function AppRoutesConfig() {
  return (
    <Routes>
      {/* 🔧 FIX: Root path should go to landing page, not AppLayout */}
      <Route path="/" element={<LandingPage />} />
      
      {/* Public routes */}
      <Route path="/welcome" element={<LandingPage />} />
      <Route path="/login" element={<EnhancedLoginForm />} />
      <Route path="/register" element={<SelfServeRegisterForm />} />
      
      {/* 🔧 ADD: Unauthorized page */}
      <Route path="/unauthorized" element={<UnauthorizedPage />} />

      {/* 🔧 FIX: Protected routes now under /app */}
      <Route path="/app" element={<AppLayout />}>
        <Route index element={<Navigate to="/app/dashboard" replace />} />
        
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

        {/* User management routes - 🔧 Enhanced with smart redirect */}
        <Route path="users" element={
          <ProtectedRoute 
            requirePermission="users.view"
            redirectToAccessibleRoute={true} // 🔧 NEW: Enable smart redirect
          >
            <UserList />
          </ProtectedRoute>
        } />
        
        <Route path="users/:userId" element={
          <ProtectedRoute 
            requirePermission="users.view"
            redirectToAccessibleRoute={true} // 🔧 NEW: Enable smart redirect
          >
            <UserProfile />
          </ProtectedRoute>
        } />
        
        <Route path="users/:userId/roles" element={
          <ProtectedRoute 
            requirePermission="users.manage_roles"
            redirectToAccessibleRoute={true} // 🔧 NEW: Enable smart redirect
          >
            <UserRoleAssignment />
          </ProtectedRoute>
        } />

        <Route path="users/new" element={
          <ProtectedRoute 
            requirePermission="users.create"
            redirectToAccessibleRoute={true} // 🔧 NEW: Enable smart redirect
          >
            <CreateUser />
          </ProtectedRoute>
        } />

        {/* Role management routes - 🔧 Enhanced with smart redirect */}
        <Route path="roles" element={
          <ProtectedRoute 
            requirePermission="roles.view"
            redirectToAccessibleRoute={true} // 🔧 NEW: Enable smart redirect
          >
            <RoleManagement />
          </ProtectedRoute>
        } />
        
        <Route path="roles/:id" element={
          <ProtectedRoute 
            requirePermission="roles.view"
            redirectToAccessibleRoute={true} // 🔧 NEW: Enable smart redirect
          >
            <RoleDetails />
          </ProtectedRoute>
        } />
        
        <Route path="roles/:id/edit" element={
          <ProtectedRoute 
            requirePermission="roles.edit"
            redirectToAccessibleRoute={true} // 🔧 NEW: Enable smart redirect
          >
            <RoleEditor />
          </ProtectedRoute>
        } />
        
        <Route path="roles/new" element={
          <ProtectedRoute 
            requirePermission="roles.create"
            redirectToAccessibleRoute={true} // 🔧 NEW: Enable smart redirect
          >
            <RoleEditor />
          </ProtectedRoute>
        } />
      </Route>

      {/* Fallback - redirect unknown routes to landing */}
      <Route path="*" element={<Navigate to="/" replace />} />
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
