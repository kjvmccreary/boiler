import { StrictMode } from 'react';
import { createRoot } from 'react-dom/client';
import './index.css';
import App from './App.tsx';

// ✅ ADD: Install MUI Premium License Key
import { LicenseInfo } from '@mui/x-license';

LicenseInfo.setLicenseKey('16a7881b177e895a30a9962d75169d44Tz05OTM1MSxFPTE3NTk2MDgyMjgwMDAsUz1wcmVtaXVtLExNPXN1YnNjcmlwdGlvbixQVj1pbml0aWFsLEtWPTI=');

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
