import React from 'react'
import { describe, it, expect, vi } from 'vitest'
import { render, screen, fireEvent } from '@testing-library/react'
import { ErrorBoundary } from '@/components/common/ErrorBoundary'

interface ThrowErrorProps {
  shouldError?: boolean
}

function ThrowError({ shouldError = false }: ThrowErrorProps) {
  if (shouldError) {
    throw new Error('Test error')
  }
  return <div>No error</div>
}

describe('ErrorBoundary', () => {
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

    // Fix: Look for text that actually appears in the error UI
    expect(screen.getByText(/component error/i)).toBeInTheDocument()
    expect(screen.getByText(/this component encountered an error/i)).toBeInTheDocument()
    expect(screen.getByRole('button', { name: /try again/i })).toBeInTheDocument()
  })

  it('shows page-level error UI when level is page', () => {
    render(
      <ErrorBoundary level="page">
        <ThrowError shouldError={true} />
      </ErrorBoundary>
    )

    // Fix: Look for page-specific error text
    expect(screen.getByText(/page error/i)).toBeInTheDocument()
    expect(screen.getByText(/this page encountered an error/i)).toBeInTheDocument()
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

    const detailsButton = screen.getByRole('button', { name: /show details/i })
    fireEvent.click(detailsButton)

    expect(screen.getByText(/error details/i)).toBeInTheDocument()
    expect(screen.getByText(/error id/i)).toBeInTheDocument()
  })

  it('renders custom fallback when provided', () => {
    const customFallback = <div>Custom error message</div>

    render(
      <ErrorBoundary fallback={customFallback}>
        <ThrowError shouldError={true} />
      </ErrorBoundary>
    )

    expect(screen.getByText('Custom error message')).toBeInTheDocument()
  })
})
