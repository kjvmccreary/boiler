import { ThemeProvider, createTheme } from '@mui/material/styles';
import { CssBaseline } from '@mui/material';
import { Toaster } from 'react-hot-toast';
import { AuthProvider } from '@/contexts/AuthContext.js';
import { PermissionProvider } from '@/contexts/PermissionContext.js';
import { TenantProvider } from '@/contexts/TenantContext.js'; // 🔧 NEW: Add TenantProvider
import { AppRoutes } from '@/routes/index.js';

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

function App() {
  return (
    <ThemeProvider theme={theme}>
      <CssBaseline />
      <AuthProvider>
        <TenantProvider> {/* 🔧 NEW: Wrap PermissionProvider with TenantProvider */}
          <PermissionProvider>
            <AppRoutes />
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
