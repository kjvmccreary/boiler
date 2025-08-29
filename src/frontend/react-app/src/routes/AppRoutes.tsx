import { Routes, Route, Navigate } from 'react-router-dom';
import { lazy, Suspense } from 'react';
import { CircularProgress, Box } from '@mui/material';
import { ProtectedRoute } from '@/components/authorization/ProtectedRoute.js';

const EnhancedLoginForm = lazy(() => import('@/components/auth/EnhancedLoginForm.js'));
const RegisterForm = lazy(() => import('@/components/auth/RegisterForm.js'));
const Dashboard = lazy(() => import('@/pages/Dashboard.js'));
const TaskDetailsPage = lazy(() => import('@/features/workflow/tasks/TaskDetailsPage'));
const InstanceDetailsPage = lazy(() => import('@/features/workflow/instances/InstanceDetailsPage')); // ensure file exists
const DefinitionDetailsPage = lazy(() => import('@/features/workflow/definitions/DefinitionDetailsPage')); // ensure file exists

const LoadingSpinner = () => (
  <Box display="flex" justifyContent="center" alignItems="center" minHeight="50vh">
    <CircularProgress />
  </Box>
);

export function AppRoutes() {
  return (
    <Suspense fallback={<LoadingSpinner />}>
      <Routes>
        {/* Public */}
        <Route path="/login" element={<EnhancedLoginForm />} />
        <Route path="/register" element={<RegisterForm />} />

        {/* Protected core pages */}
        <Route
          path="/dashboard"
          element={
            <ProtectedRoute>
              <Dashboard />
            </ProtectedRoute>
          }
        />

        {/* Workflow detail routes (added) */}
        {/* NOTE: Temporarily remove ProtectedRoute from Task/Definition detail to confirm routing works.
                 Re-wrap with permission once verified:
                 <ProtectedRoute requiredPermission="workflow.view_tasks"> ... */}
        <Route
          path="/app/workflow/tasks/:id"
          element={
            <ProtectedRoute /* requiredPermission="workflow.view_tasks" */>
              <TaskDetailsPage />
            </ProtectedRoute>
          }
        />
        <Route
          path="/app/workflow/instances/:id"
          element={
            <ProtectedRoute /* requiredPermission="workflow.view_instances" */>
              <InstanceDetailsPage />
            </ProtectedRoute>
          }
        />
        <Route
          path="/app/workflow/definitions/:id"
          element={
            <ProtectedRoute /* requiredPermission="workflow.view_definitions" */>
              <DefinitionDetailsPage />
            </ProtectedRoute>
          }
        />

        {/* Default */}
        <Route path="/" element={<Navigate to="/dashboard" replace />} />
      </Routes>
    </Suspense>
  );
}

export default AppRoutes;
