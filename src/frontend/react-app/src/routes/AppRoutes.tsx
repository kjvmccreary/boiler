import { Routes, Route } from 'react-router-dom';
import { ErrorBoundary } from '@/components/common/ErrorBoundary.js';
import { LoadingSpinner } from '@/components/common/LoadingStates.js';
import { Suspense, lazy } from 'react';

// Lazy load components for code splitting
const LoginForm = lazy(() => import('@/components/auth/LoginForm.js').then(m => ({ default: m.LoginForm })));
const RegisterForm = lazy(() => import('@/components/auth/RegisterForm.js').then(m => ({ default: m.RegisterForm })));
const ForgotPasswordForm = lazy(() => import('@/components/auth/ForgotPasswordForm.js').then(m => ({ default: m.ForgotPasswordForm })));
const ResetPasswordForm = lazy(() => import('@/components/auth/ResetPasswordForm.js').then(m => ({ default: m.ResetPasswordForm })));
const Dashboard = lazy(() => import('@/pages/Dashboard.js').then(m => ({ default: m.Dashboard })));
const UserList = lazy(() => import('@/components/users/UserList.js').then(m => ({ default: m.UserList })));
const RoleList = lazy(() => import('@/components/roles/RoleList.js').then(m => ({ default: m.RoleList })));

export function AppRoutes() {
  return (
    <ErrorBoundary level="page">
      <Suspense fallback={<LoadingSpinner fullHeight message="Loading application..." />}>
        <Routes>
          {/* Public routes */}
          <Route 
            path="/login" 
            element={
              <ErrorBoundary level="component">
                <LoginForm />
              </ErrorBoundary>
            } 
          />
          <Route 
            path="/register" 
            element={
              <ErrorBoundary level="component">
                <RegisterForm />
              </ErrorBoundary>
            } 
          />
          <Route 
            path="/forgot-password" 
            element={
              <ErrorBoundary level="component">
                <ForgotPasswordForm />
              </ErrorBoundary>
            } 
          />
          <Route 
            path="/reset-password" 
            element={
              <ErrorBoundary level="component">
                <ResetPasswordForm />
              </ErrorBoundary>
            } 
          />

          {/* Protected routes */}
          <Route 
            path="/dashboard" 
            element={
              <ErrorBoundary level="page">
                <Dashboard />
              </ErrorBoundary>
            } 
          />
          <Route 
            path="/users" 
            element={
              <ErrorBoundary level="page">
                <UserList />
              </ErrorBoundary>
            } 
          />
          <Route 
            path="/roles" 
            element={
              <ErrorBoundary level="page">
                <RoleList />
              </ErrorBoundary>
            } 
          />

          {/* Catch-all route */}
          <Route 
            path="*" 
            element={
              <ErrorBoundary level="page">
                <div>404 - Page Not Found</div>
              </ErrorBoundary>
            } 
          />
        </Routes>
      </Suspense>
    </ErrorBoundary>
  );
}
