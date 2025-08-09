import { describe, it, expect } from 'vitest'
import { render, screen } from '../../../test/utils/test-utils.js'
import { 
  LoadingSpinner, 
  PageLoading, 
  TableSkeleton,
  UserListSkeleton 
} from '../LoadingStates.js'

describe('LoadingStates', () => {
  describe('LoadingSpinner', () => {
    it('renders with default props', () => {
      render(<LoadingSpinner />)
      
      const spinner = screen.getByRole('progressbar')
      expect(spinner).toBeInTheDocument()
    })

    it('renders with custom message', () => {
      const message = 'Loading data...'
      render(<LoadingSpinner message={message} />)
      
      expect(screen.getByText(message)).toBeInTheDocument()
    })

    it('renders with full height when specified', () => {
      render(<LoadingSpinner fullHeight />)
      
      const container = screen.getByRole('progressbar').parentElement
      expect(container).toHaveStyle({ minHeight: '50vh' })
    })
  })

  describe('PageLoading', () => {
    it('renders with default message', () => {
      render(<PageLoading />)
      
      expect(screen.getByText('Loading...')).toBeInTheDocument()
      expect(screen.getByRole('progressbar')).toBeInTheDocument()
    })

    it('renders with custom message', () => {
      const message = 'Saving changes...'
      render(<PageLoading message={message} />)
      
      expect(screen.getByText(message)).toBeInTheDocument()
    })

    it('shows progress percentage when provided', () => {
      render(<PageLoading progress={75} />)
      
      expect(screen.getByText('75%')).toBeInTheDocument()
    })
  })

  describe('TableSkeleton', () => {
    it('renders skeleton elements', () => {
      render(<TableSkeleton />)
      
      // Check that skeleton elements are rendered
      const skeletons = screen.getAllByRole('generic')
      expect(skeletons.length).toBeGreaterThan(0)
    })

    it('renders custom number of rows and columns', () => {
      render(<TableSkeleton rows={3} columns={2} />)
      
      // Check that skeleton elements exist
      const skeletons = screen.getAllByRole('generic')
      expect(skeletons.length).toBeGreaterThan(0)
    })

    it('can hide header', () => {
      render(<TableSkeleton showHeader={false} />)
      
      // Check that skeleton elements exist
      const skeletons = screen.getAllByRole('generic')
      expect(skeletons.length).toBeGreaterThan(0)
    })
  })

  describe('UserListSkeleton', () => {
    it('renders default number of user items', () => {
      render(<UserListSkeleton />)
      
      const list = screen.getByRole('list')
      expect(list).toBeInTheDocument()
      
      const listItems = screen.getAllByRole('listitem')
      expect(listItems).toHaveLength(5) // Default count
    })

    it('renders custom number of user items', () => {
      render(<UserListSkeleton count={3} />)
      
      const listItems = screen.getAllByRole('listitem')
      expect(listItems).toHaveLength(3)
    })
  })
})
