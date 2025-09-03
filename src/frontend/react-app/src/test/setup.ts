import '@testing-library/jest-dom';
import { cleanup } from '@testing-library/react';
import { afterEach, beforeAll, afterAll, vi } from 'vitest';
import { setupServer } from 'msw/node';
import { handlers } from './mocks/handlers';

// -----------------------------------------------------------------------------
// 1. Environment / Globals
// -----------------------------------------------------------------------------
process.env.TZ = 'UTC';

// (Optional console noise filter disabled by default)
// const originalError = console.error;
// console.error = (...args: any[]) => { /* filter if desired */ };

// -----------------------------------------------------------------------------
// 2. MSW Server
// -----------------------------------------------------------------------------
export const server = setupServer(...handlers);

beforeAll(() => {
  server.listen({ onUnhandledRequest: 'warn' });
});

afterEach(() => {
  cleanup();
  server.resetHandlers();
  vi.clearAllMocks();
  localStorage.clear();
  sessionStorage.clear();
});

afterAll(() => {
  server.close();
});

// -----------------------------------------------------------------------------
// 3. Browser API Shims
// -----------------------------------------------------------------------------
if (!('matchMedia' in window)) {
  Object.defineProperty(window, 'matchMedia', {
    writable: true,
    value: vi.fn().mockImplementation((query: string) => ({
      matches: false,
      media: query,
      onchange: null,
      addListener: vi.fn(),          // deprecated
      removeListener: vi.fn(),       // deprecated
      addEventListener: vi.fn(),
      removeEventListener: vi.fn(),
      dispatchEvent: vi.fn()
    }))
  });
}

// Provide lightweight class mocks so TypeScript is satisfied without @ts-expect-error
if (!('ResizeObserver' in globalThis)) {
  class MockResizeObserver {
    observe(): void {}
    unobserve(): void {}
    disconnect(): void {}
  }
  (globalThis as any).ResizeObserver = vi.fn().mockImplementation(() => new MockResizeObserver());
}

if (!('IntersectionObserver' in globalThis)) {
  class MockIntersectionObserver {
    observe(): void {}
    unobserve(): void {}
    disconnect(): void {}
    takeRecords(): any[] { return []; }
    root = null;
    rootMargin = '';
    thresholds: number[] = [];
  }
  (globalThis as any).IntersectionObserver = vi.fn().mockImplementation(
    () => new MockIntersectionObserver()
  );
}

if (!('scrollTo' in window)) {
  Object.defineProperty(window, 'scrollTo', {
    writable: true,
    value: vi.fn()
  });
}

// -----------------------------------------------------------------------------
// 4. In-Memory Storage (stateful + resettable)
// -----------------------------------------------------------------------------
function createMemoryStorage() {
  let store = new Map<string, string>();
  return {
    get length() {
      return store.size;
    },
    clear: vi.fn(() => { store.clear(); }),
    getItem: vi.fn((key: string) => (store.has(key) ? store.get(key)! : null)),
    setItem: vi.fn((key: string, value: string) => { store.set(key, value); }),
    removeItem: vi.fn((key: string) => { store.delete(key); }),
    key: vi.fn((index: number) => Array.from(store.keys())[index] ?? null)
  };
}

const memoryLocalStorage = createMemoryStorage();
const memorySessionStorage = createMemoryStorage();

if (!(globalThis as any)._storageStubbed) {
  vi.stubGlobal('localStorage', memoryLocalStorage);
  vi.stubGlobal('sessionStorage', memorySessionStorage);
  (globalThis as any)._storageStubbed = true;
}

// -----------------------------------------------------------------------------
// 5. Environment Variable Stubs
// -----------------------------------------------------------------------------
vi.stubEnv('VITE_API_BASE_URL', 'http://localhost:5000/api');
vi.stubEnv('VITE_APP_TITLE', 'Test App');
vi.stubEnv('MODE', 'test');
vi.stubEnv('NODE_ENV', 'test');

// -----------------------------------------------------------------------------
// 6. Utility Hooks Placeholder
// (Add per-test bootstrap hooks here if needed.)
// -----------------------------------------------------------------------------
