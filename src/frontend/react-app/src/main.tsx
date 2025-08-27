import { StrictMode } from 'react';
import { createRoot } from 'react-dom/client';
import './index.css';
import App from './App.tsx';

// ✅ ADD: Install MUI Premium License Key
//import { LicenseInfo } from '@mui/x-license';

//const licenseKey = import.meta.env.VITE_MUI_LICENSE_KEY;
//if (licenseKey) {
//  LicenseInfo.setLicenseKey(licenseKey);
//} else {
//  console.warn('⚠️ MUI Premium license key not found. Add VITE_MUI_LICENSE_KEY to your .env file.');
//}
// Enable MSW in development for testing API integration
async function enableMocking() {
  if (import.meta.env.DEV && import.meta.env.VITE_ENABLE_MOCK === 'true') {
    const { startWorker } = await import('./test/mocks/browser.js');
    return startWorker();
  }
  return Promise.resolve();
}

enableMocking().then(() => {
  createRoot(document.getElementById('root')!).render(
    <StrictMode>
      <App />
    </StrictMode>,
  );
});
