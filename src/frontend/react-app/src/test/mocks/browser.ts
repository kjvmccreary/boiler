import { setupWorker } from 'msw/browser';
import { handlers } from './handlers.js';

// This configures a Service Worker with the given request handlers
export const worker = setupWorker(...handlers);

// Start the worker for development mode
export const startWorker = () => {
  if (import.meta.env.DEV && import.meta.env.VITE_ENABLE_MOCK === 'true') {
    return worker.start({
      onUnhandledRequest: 'warn',
      serviceWorker: {
        url: '/mockServiceWorker.js',
      },
    });
  }
  return Promise.resolve();
};

// Optional: Auto-start for development
if (import.meta.env.DEV && import.meta.env.VITE_ENABLE_MOCK === 'true') {
  startWorker().catch(console.error);
}
