import { Component } from 'react';
import type { ErrorInfo, ReactNode } from 'react';
import {
  Box,
  Typography,
  Button,
  Card,
  CardContent,
  Alert,
  Collapse,
} from '@mui/material';
import {
  Refresh as RefreshIcon,
  ExpandMore as ExpandMoreIcon,
  ExpandLess as ExpandLessIcon,
  BugReport as BugReportIcon,
} from '@mui/icons-material';

interface Props {
  children: ReactNode;
  fallback?: ReactNode;
  onError?: (error: Error, errorInfo: ErrorInfo) => void;
  level?: 'page' | 'component' | 'section';
  resetOnPropsChange?: boolean;
  resetKeys?: Array<string | number>;
}

interface State {
  hasError: boolean;
  error: Error | null;
  errorInfo: ErrorInfo | null;
  showDetails: boolean;
  errorId: string;
}

export class ErrorBoundary extends Component<Props, State> {
  private resetTimeoutId: number | null = null;

  constructor(props: Props) {
    super(props);

    this.state = {
      hasError: false,
      error: null,
      errorInfo: null,
      showDetails: false,
      errorId: '',
    };
  }

  static getDerivedStateFromError(error: Error): Partial<State> {
    // Update state so the next render will show the fallback UI
    return {
      hasError: true,
      error,
      errorId: `error_${Date.now()}_${Math.random().toString(36).substring(2, 11)}`, // Fixed deprecated substr
    };
  }

  componentDidCatch(error: Error, errorInfo: ErrorInfo) {
    // Log the error
    console.error('ErrorBoundary caught an error:', error, errorInfo);

    // Update state with error details
    this.setState({
      error,
      errorInfo,
    });

    // Call custom error handler if provided
    if (this.props.onError) {
      this.props.onError(error, errorInfo);
    }

    // Report to error tracking service (add your service here)
    this.reportError(error, errorInfo);
  }

  componentDidUpdate(prevProps: Props) {
    const { resetKeys, resetOnPropsChange } = this.props;
    const { hasError } = this.state;

    // Reset error boundary when resetKeys change
    if (hasError && resetKeys) {
      const hasResetKeyChanged = resetKeys.some(
        (key, index) => key !== prevProps.resetKeys?.[index]
      );

      if (hasResetKeyChanged) {
        this.resetErrorBoundary();
      }
    }

    // Reset error boundary when props change (if enabled)
    if (hasError && resetOnPropsChange && prevProps.children !== this.props.children) {
      this.resetErrorBoundary();
    }
  }

  private reportError = (error: Error, errorInfo: ErrorInfo) => {
    // TODO: Send error to your error reporting service
    // Example: Sentry, LogRocket, etc.
    /*
    Sentry.withScope((scope) => {
      scope.setTag('errorBoundary', true);
      scope.setContext('componentStack', {
        componentStack: errorInfo.componentStack,
      });
      Sentry.captureException(error);
    });
    */

    // For now, just log to console
    console.group('🚨 Error Boundary Report');
    console.error('Error:', error);
    console.error('Error Info:', errorInfo);
    console.error('Component Stack:', errorInfo.componentStack);
    console.groupEnd();
  };

  private resetErrorBoundary = () => {
    if (this.resetTimeoutId) {
      clearTimeout(this.resetTimeoutId);
    }

    this.resetTimeoutId = window.setTimeout(() => {
      this.setState({
        hasError: false,
        error: null,
        errorInfo: null,
        showDetails: false,
        errorId: '',
      });
    }, 100);
  };

  private handleRetry = () => {
    this.resetErrorBoundary();
  };

  private handleRefresh = () => {
    window.location.reload();
  };

  private toggleDetails = () => {
    this.setState(prev => ({
      showDetails: !prev.showDetails,
    }));
  };

  private getErrorMessage = (): string => {
    const { error } = this.state;
    const { level = 'component' } = this.props;

    if (!error) return 'An unexpected error occurred';

    // Customize error messages based on error type
    if (error.name === 'ChunkLoadError') {
      return 'Failed to load application resources. Please refresh the page.';
    }

    if (error.message.includes('Network Error')) {
      return 'Network connection error. Please check your internet connection.';
    }

    if (error.message.includes('Permission denied')) {
      return 'You don\'t have permission to access this resource.';
    }

    // Level-specific messages
    switch (level) {
      case 'page':
        return 'This page encountered an error and cannot be displayed.';
      case 'section':
        return 'This section encountered an error.';
      default:
        return 'This component encountered an error.';
    }
  };

  private getErrorTitle = (): string => {
    const { level = 'component' } = this.props;

    switch (level) {
      case 'page':
        return 'Page Error';
      case 'section':
        return 'Section Error';
      default:
        return 'Component Error';
    }
  };

  render() {
    if (this.state.hasError) {
      // Custom fallback UI if provided
      if (this.props.fallback) {
        return this.props.fallback;
      }

      // Default error UI
      const { level = 'component' } = this.props;
      const { error, errorInfo, showDetails, errorId } = this.state;

      return (
        <Card 
          sx={{ 
            m: level === 'page' ? 2 : 1,
            border: '2px solid',
            borderColor: 'error.main',
            backgroundColor: 'error.50',
          }}
        >
          <CardContent>
            <Box sx={{ display: 'flex', alignItems: 'center', mb: 2 }}>
              <BugReportIcon color="error" sx={{ mr: 1 }} />
              <Typography variant={level === 'page' ? 'h5' : 'h6'} color="error">
                {this.getErrorTitle()}
              </Typography>
            </Box>

            <Alert severity="error" sx={{ mb: 2 }}>
              {this.getErrorMessage()}
            </Alert>

            <Box sx={{ display: 'flex', gap: 1, mb: 2 }}>
              <Button
                variant="contained"
                color="primary"
                onClick={this.handleRetry}
                startIcon={<RefreshIcon />}
                size="small"
              >
                Try Again
              </Button>

              {level === 'page' && (
                <Button
                  variant="outlined"
                  onClick={this.handleRefresh}
                  size="small"
                >
                  Refresh Page
                </Button>
              )}

              <Button
                variant="text"
                onClick={this.toggleDetails}
                endIcon={showDetails ? <ExpandLessIcon /> : <ExpandMoreIcon />}
                size="small"
              >
                {showDetails ? 'Hide' : 'Show'} Details
              </Button>
            </Box>

            <Collapse in={showDetails}>
              <Box sx={{ mt: 2 }}>
                <Typography variant="subtitle2" gutterBottom>
                  Error ID: {errorId}
                </Typography>

                {error && (
                  <Box sx={{ mb: 2 }}>
                    <Typography variant="subtitle2" color="error" gutterBottom>
                      Error Details:
                    </Typography>
                    <Box
                      component="pre"
                      sx={{
                        backgroundColor: 'grey.100',
                        p: 1,
                        borderRadius: 1,
                        fontSize: '0.75rem',
                        overflow: 'auto',
                        maxHeight: 200,
                      }}
                    >
                      {error.toString()}
                    </Box>
                  </Box>
                )}

                {errorInfo && (
                  <Box>
                    <Typography variant="subtitle2" color="error" gutterBottom>
                      Component Stack:
                    </Typography>
                    <Box
                      component="pre"
                      sx={{
                        backgroundColor: 'grey.100',
                        p: 1,
                        borderRadius: 1,
                        fontSize: '0.75rem',
                        overflow: 'auto',
                        maxHeight: 200,
                      }}
                    >
                      {errorInfo.componentStack}
                    </Box>
                  </Box>
                )}
              </Box>
            </Collapse>
          </CardContent>
        </Card>
      );
    }

    return this.props.children;
  }
}

// Higher-order component for easier usage
export function withErrorBoundary<P extends object>(
  Component: React.ComponentType<P>,
  errorBoundaryProps?: Omit<Props, 'children'>
) {
  return function WrappedComponent(props: P) {
    return (
      <ErrorBoundary {...errorBoundaryProps}>
        <Component {...props} />
      </ErrorBoundary>
    );
  };
}

// Hook for programmatic error handling
export function useErrorHandler() {
  return (error: Error, errorInfo?: string) => {
    console.error('Manual error report:', error, errorInfo);
    
    // You can also throw the error to trigger the nearest error boundary
    throw error;
  };
}
