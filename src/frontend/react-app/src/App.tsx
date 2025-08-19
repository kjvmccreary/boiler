import { ThemeProvider, createTheme } from '@mui/material/styles';
import { CssBaseline } from '@mui/material';
import { Toaster } from 'react-hot-toast';
import { BrowserRouter } from 'react-router-dom';
import { AuthProvider } from '@/contexts/AuthContext.js';
import { PermissionProvider } from '@/contexts/PermissionContext.js';
import { TenantProvider, useTenant } from '@/contexts/TenantContext.js';
import { TenantNavigationHandler } from '@/components/navigation/TenantNavigationHandler.js';
import { TenantSelector } from '@/components/auth/TenantSelector.js';
import { AppRoutesConfig } from '@/routes/index.js';
import { useAuth } from '@/contexts/AuthContext.js';

const theme = createTheme({
  palette: {
    primary: {
      main: '#1976d2',
    },
    secondary: {
      main: '#dc004e',
    },
  },
});

// Component to conditionally render TenantSelector or main app
function AppContent() {
  const { isAuthenticated } = useAuth();
  const { showTenantSelector, currentTenant, switchTenant } = useTenant();

  console.log('🔍 AppContent: State check:', {
    isAuthenticated,
    showTenantSelector,
    currentTenant: currentTenant ? { id: currentTenant.id, name: currentTenant.name } : null
  });

  // Show tenant selector if user is authenticated but needs to select a tenant
  if (isAuthenticated && showTenantSelector && !currentTenant) {
    console.log('🏢 AppContent: Showing TenantSelector');
    return <TenantSelector onTenantSelected={switchTenant} />;
  }

  // Normal app layout - user is authenticated and has a selected tenant
  if (isAuthenticated && currentTenant) {
    console.log('🏢 AppContent: Showing main app for tenant:', currentTenant.name);
    return (
      <>
        <TenantNavigationHandler />
        <AppRoutesConfig />
      </>
    );
  }

  // For unauthenticated users, still show the main app (will show login)
  console.log('🔍 AppContent: Showing app for unauthenticated user');
  return (
    <>
      <TenantNavigationHandler />
      <AppRoutesConfig />
    </>
  );
}

function App() {
  return (
    <ThemeProvider theme={theme}>
      <CssBaseline />
      <AuthProvider>
        <TenantProvider>
          <PermissionProvider>
            <BrowserRouter>
              <AppContent />
            </BrowserRouter>
            <Toaster
              position="top-right"
              toastOptions={{
                duration: 4000,
                style: {
                  background: '#363736',
                  color: '#fff',
                },
              }}
            />
          </PermissionProvider>
        </TenantProvider>
      </AuthProvider>
    </ThemeProvider>
  );
}

export default App;
