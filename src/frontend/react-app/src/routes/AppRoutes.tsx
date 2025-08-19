import { Routes, Route, Navigate } from 'react-router-dom';
import { lazy, Suspense } from 'react';
import { CircularProgress, Box } from '@mui/material';
import { ProtectedRoute } from '@/components/authorization/ProtectedRoute.js'; // 🔧 FIX: Correct path

// Lazy load components
const EnhancedLoginForm = lazy(() => import('@/components/auth/EnhancedLoginForm.js'));
const RegisterForm = lazy(() => import('@/components/auth/RegisterForm.js'));
const Dashboard = lazy(() => import('@/pages/Dashboard.js'));
// ... other imports

const LoadingSpinner = () => (
  <Box display="flex" justifyContent="center" alignItems="center" minHeight="50vh">
    <CircularProgress />
  </Box>
);

export function AppRoutes() {
  return (
    <Suspense fallback={<LoadingSpinner />}>
      <Routes>
        {/* Public routes */}
        <Route path="/login" element={<EnhancedLoginForm />} />
        <Route path="/register" element={<RegisterForm />} />
        
        {/* Protected routes */}
        <Route path="/dashboard" element={
          <ProtectedRoute>
            <Dashboard />
          </ProtectedRoute>
        } />
        
        {/* Default redirect */}
        <Route path="/" element={<Navigate to="/dashboard" replace />} />
      </Routes>
    </Suspense>
  );
}
