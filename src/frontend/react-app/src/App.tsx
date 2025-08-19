import { ThemeProvider, createTheme } from '@mui/material/styles';
import { CssBaseline } from '@mui/material';
import { Toaster } from 'react-hot-toast';
import { BrowserRouter } from 'react-router-dom';
import { AuthProvider } from '@/contexts/AuthContext.js';
import { PermissionProvider } from '@/contexts/PermissionContext.js';
import { TenantProvider } from '@/contexts/TenantContext.js';
import { TenantNavigationHandler } from '@/components/navigation/TenantNavigationHandler.js';
import { AppRoutesConfig } from '@/routes/index.js';

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
        <TenantProvider>
          <PermissionProvider>
            <BrowserRouter>
              <TenantNavigationHandler />
              <AppRoutesConfig />
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
