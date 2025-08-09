import 'vitest/globals'
import '@testing-library/jest-dom'

declare global {
  // Ensure vitest globals are available
  const describe: typeof import('vitest').describe
  const it: typeof import('vitest').it
  const test: typeof import('vitest').test
  const expect: typeof import('vitest').expect
  const beforeAll: typeof import('vitest').beforeAll
  const beforeEach: typeof import('vitest').beforeEach
  const afterAll: typeof import('vitest').afterAll
  const afterEach: typeof import('vitest').afterEach
  const vi: typeof import('vitest').vi
}

export {}
