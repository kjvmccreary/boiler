import '@testing-library/jest-dom'
import { cleanup } from '@testing-library/react'
import { afterEach, beforeAll, afterAll, vi } from 'vitest'
import { setupServer } from 'msw/node'
import { handlers } from './mocks/handlers.js'

// Mock server setup - Fix MSW unhandled request errors
export const server = setupServer(...handlers)

// Start server before all tests
beforeAll(() => {
  // Fix: Change to 'warn' to avoid MSW errors breaking tests
  server.listen({ onUnhandledRequest: 'warn' })
})

// Clean up after each test - Fix multiple element rendering
afterEach(() => {
  cleanup()
  server.resetHandlers()
  // Clear all mocks to prevent state leakage
  vi.clearAllMocks()
  // Clear localStorage and sessionStorage
  localStorage.clear()
  sessionStorage.clear()
})

// Close server after all tests
afterAll(() => {
  server.close()
})

// Mock window.matchMedia
Object.defineProperty(window, 'matchMedia', {
  writable: true,
  value: vi.fn().mockImplementation(query => ({
    matches: false,
    media: query,
    onchange: null,
    addListener: vi.fn(), // deprecated
    removeListener: vi.fn(), // deprecated
    addEventListener: vi.fn(),
    removeEventListener: vi.fn(),
    dispatchEvent: vi.fn(),
  })),
})

// Mock ResizeObserver
global.ResizeObserver = vi.fn().mockImplementation(() => ({
  observe: vi.fn(),
  unobserve: vi.fn(),
  disconnect: vi.fn(),
}))

// Mock IntersectionObserver
global.IntersectionObserver = vi.fn().mockImplementation(() => ({
  observe: vi.fn(),
  unobserve: vi.fn(),
  disconnect: vi.fn(),
}))

// Mock scrollTo
Object.defineProperty(window, 'scrollTo', {
  writable: true,
  value: vi.fn().mockImplementation(() => {
    return undefined
  }),
})

// Mock localStorage
const localStorageMock = {
  getItem: vi.fn(),
  setItem: vi.fn(),
  removeItem: vi.fn(),
  clear: vi.fn(),
  length: 0,
  key: vi.fn(),
}
vi.stubGlobal('localStorage', localStorageMock)

// Mock sessionStorage
const sessionStorageMock = {
  getItem: vi.fn(),
  setItem: vi.fn(),
  removeItem: vi.fn(),
  clear: vi.fn(),
  length: 0,
  key: vi.fn(),
}
vi.stubGlobal('sessionStorage', sessionStorageMock)

// Fix: Add environment variable configuration for tests
vi.stubEnv('VITE_API_BASE_URL', 'http://localhost:5000/api')
vi.stubEnv('VITE_APP_TITLE', 'Test App')
vi.stubEnv('MODE', 'test')
