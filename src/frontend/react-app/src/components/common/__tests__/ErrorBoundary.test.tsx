import { describe, it, expect, vi, afterEach } from 'vitest'
import { render, screen, fireEvent } from '../../../test/utils/test-utils.js'
import { ErrorBoundary } from '../ErrorBoundary.js'

// Test component that throws an error
function ThrowError({ shouldError }: { shouldError: boolean }) {
  if (shouldError) {
    throw new Error('Test error')
  }
  return <div>No error</div>
}

describe('ErrorBoundary', () => {
  const mockConsoleError = vi.spyOn(console, 'error').mockImplementation(() => {})
  const mockConsoleGroup = vi.spyOn(console, 'group').mockImplementation(() => {})
  const mockConsoleGroupEnd = vi.spyOn(console, 'groupEnd').mockImplementation(() => {})

  afterEach(() => {
    mockConsoleError.mockClear()
    mockConsoleGroup.mockClear()
    mockConsoleGroupEnd.mockClear()
  })

  it('renders children when no error occurs', () => {
    render(
      <ErrorBoundary>
        <ThrowError shouldError={false} />
      </ErrorBoundary>
    )

    expect(screen.getByText('No error')).toBeInTheDocument()
  })

  it('renders error UI when error occurs', () => {
    render(
      <ErrorBoundary>
        <ThrowError shouldError={true} />
      </ErrorBoundary>
    )

    expect(screen.getByText('Component Error')).toBeInTheDocument()
    expect(screen.getByText('This component encountered an error.')).toBeInTheDocument()
    expect(screen.getByRole('button', { name: /try again/i })).toBeInTheDocument()
  })

  it('shows page-level error UI when level is page', () => {
    render(
      <ErrorBoundary level="page">
        <ThrowError shouldError={true} />
      </ErrorBoundary>
    )

    expect(screen.getByText('Page Error')).toBeInTheDocument()
    expect(screen.getByText('This page encountered an error and cannot be displayed.')).toBeInTheDocument()
    expect(screen.getByRole('button', { name: /refresh page/i })).toBeInTheDocument()
  })

  it('calls onError callback when error occurs', () => {
    const onError = vi.fn()
    
    render(
      <ErrorBoundary onError={onError}>
        <ThrowError shouldError={true} />
      </ErrorBoundary>
    )

    expect(onError).toHaveBeenCalledWith(
      expect.any(Error),
      expect.objectContaining({
        componentStack: expect.any(String)
      })
    )
  })

  it('shows error details when details button is clicked', () => {
    render(
      <ErrorBoundary>
        <ThrowError shouldError={true} />
      </ErrorBoundary>
    )

    const showDetailsButton = screen.getByRole('button', { name: /show details/i })
    fireEvent.click(showDetailsButton)

    expect(screen.getByText(/Error ID:/)).toBeInTheDocument()
    expect(screen.getByText(/Error Details:/)).toBeInTheDocument()
  })

  it('renders custom fallback when provided', () => {
    const customFallback = <div>Custom error message</div>

    render(
      <ErrorBoundary fallback={customFallback}>
        <ThrowError shouldError={true} />
      </ErrorBoundary>
    )

    expect(screen.getByText('Custom error message')).toBeInTheDocument()
    expect(screen.queryByText('Component Error')).not.toBeInTheDocument()
  })
})
