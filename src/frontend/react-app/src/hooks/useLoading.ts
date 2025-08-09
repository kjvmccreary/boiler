import { useState, useCallback } from 'react';

interface UseLoadingOptions {
  initialLoading?: boolean;
  onStart?: () => void;
  onComplete?: () => void;
  onError?: (error: Error) => void;
}

interface UseLoadingReturn {
  isLoading: boolean;
  error: Error | null;
  startLoading: () => void;
  stopLoading: () => void;
  setError: (error: Error | null) => void;
  withLoading: <T>(asyncFn: () => Promise<T>) => Promise<T>;
}

export function useLoading(options: UseLoadingOptions = {}): UseLoadingReturn {
  const { initialLoading = false, onStart, onComplete, onError } = options;

  const [isLoading, setIsLoading] = useState(initialLoading);
  const [error, setError] = useState<Error | null>(null);

  const startLoading = useCallback(() => {
    setIsLoading(true);
    setError(null);
    onStart?.();
  }, [onStart]);

  const stopLoading = useCallback(() => {
    setIsLoading(false);
    onComplete?.();
  }, [onComplete]);

  const handleError = useCallback((err: Error | null) => {
    setError(err);
    if (err) {
      onError?.(err);
    }
  }, [onError]);

  const withLoading = useCallback(async <T>(asyncFn: () => Promise<T>): Promise<T> => {
    try {
      startLoading();
      const result = await asyncFn();
      return result;
    } catch (err) {
      const error = err instanceof Error ? err : new Error('An unknown error occurred');
      handleError(error);
      throw error;
    } finally {
      stopLoading();
    }
  }, [startLoading, stopLoading, handleError]);

  return {
    isLoading,
    error,
    startLoading,
    stopLoading,
    setError: handleError,
    withLoading,
  };
}
