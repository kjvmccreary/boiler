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
import { UnauthorizedPage } from '@/components/common/UnauthorizedPage.js';

// Workflow pages
import { DefinitionsPage } from '@/features/workflow/definitions/DefinitionsPage.js';
import { DefinitionDetailsPage } from '@/features/workflow/definitions/DefinitionDetailsPage.js'; // ✅ ADDED
import { InstanceDetailsPage } from '@/features/workflow/instances/InstanceDetailsPage.js';
import { InstancesListPage } from '@/features/workflow/instances/InstancesListPage.js';
import { MyTasksPage } from '@/features/workflow/tasks/MyTasksPage.js';
import { TaskDetailsPage } from '@/features/workflow/tasks/TaskDetailsPage.js'; // ✅ ADDED
import { BuilderPage } from '@/features/workflow/builder/BuilderPage.js';

// Router-free routes component for testing
export function AppRoutesConfig() {
  return (
    <Routes>
      <Route path="/" element={<LandingPage />} />
      <Route path="/welcome" element={<LandingPage />} />
      <Route path="/login" element={<EnhancedLoginForm />} />
      <Route path="/register" element={<SelfServeRegisterForm />} />
      <Route path="/unauthorized" element={<UnauthorizedPage />} />

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

        {/* Users */}
        <Route path="users" element={
          <ProtectedRoute requirePermission="users.view" redirectToAccessibleRoute>
            <UserList />
          </ProtectedRoute>
        } />
        <Route path="users/new" element={
          <ProtectedRoute requirePermission="users.create" redirectToAccessibleRoute>
            <CreateUser />
          </ProtectedRoute>
        } />
        <Route path="users/:userId" element={
          <ProtectedRoute requirePermission="users.view" redirectToAccessibleRoute>
            <UserProfile />
          </ProtectedRoute>
        } />
        <Route path="users/:userId/roles" element={
          <ProtectedRoute requirePermission="users.manage_roles" redirectToAccessibleRoute>
            <UserRoleAssignment />
          </ProtectedRoute>
        } />

        {/* Roles */}
        <Route path="roles" element={
          <ProtectedRoute requirePermission="roles.view" redirectToAccessibleRoute>
            <RoleManagement />
          </ProtectedRoute>
        } />
        <Route path="roles/new" element={
          <ProtectedRoute requirePermission="roles.create" redirectToAccessibleRoute>
            <RoleEditor />
          </ProtectedRoute>
        } />
        <Route path="roles/:id" element={
          <ProtectedRoute requirePermission="roles.view" redirectToAccessibleRoute>
            <RoleDetails />
          </ProtectedRoute>
        } />
        <Route path="roles/:id/edit" element={
          <ProtectedRoute requirePermission="roles.edit" redirectToAccessibleRoute>
            <RoleEditor />
          </ProtectedRoute>
        } />

        {/* Workflow */}
        <Route path="workflow/definitions" element={
          <ProtectedRoute requirePermission="workflow.read" redirectToAccessibleRoute>
            <DefinitionsPage />
          </ProtectedRoute>
        } />
        <Route path="workflow/definitions/:id" element={   /* ✅ NEW DETAIL ROUTE */
          <ProtectedRoute requirePermission="workflow.read" redirectToAccessibleRoute>
            <DefinitionDetailsPage />
          </ProtectedRoute>
        } />

        <Route path="workflow/instances" element={
          <ProtectedRoute requirePermission="workflow.read" redirectToAccessibleRoute>
            <InstancesListPage />
          </ProtectedRoute>
        } />
        <Route path="workflow/instances/:id" element={
          <ProtectedRoute requirePermission="workflow.read" redirectToAccessibleRoute>
            <InstanceDetailsPage />
          </ProtectedRoute>
        } />

        <Route path="workflow/tasks/mine" element={
          <ProtectedRoute requirePermission="workflow.read" redirectToAccessibleRoute>
            <MyTasksPage />
          </ProtectedRoute>
        } />
        <Route path="workflow/tasks/:id" element={        /* ✅ NEW DETAIL ROUTE */
          <ProtectedRoute requirePermission="workflow.read" redirectToAccessibleRoute>
            <TaskDetailsPage />
          </ProtectedRoute>
        } />

        <Route path="workflow/builder/:id?" element={
          <ProtectedRoute requirePermission="workflow.write" redirectToAccessibleRoute>
            <BuilderPage />
          </ProtectedRoute>
        } />
      </Route>

      <Route path="*" element={<Navigate to="/" replace />} />
    </Routes>
  );
}

// Main app routes (kept for completeness if used elsewhere)
export function AppRoutes() {
  return (
    <BrowserRouter>
      <AppRoutesConfig />
    </BrowserRouter>
  );
}
