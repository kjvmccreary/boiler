import { useEffect, useState, useRef } from 'react';

let monacoPromise: Promise<typeof import('monaco-editor')> | null = null;
let monacoRef: typeof import('monaco-editor') | null = null;
let loadError: any = null;

export function loadMonaco(): Promise<typeof import('monaco-editor')> {
  if (monacoRef) return Promise.resolve(monacoRef);
  if (loadError) return Promise.reject(loadError);
  if (!monacoPromise) {
    monacoPromise = import('monaco-editor')
      .then(m => {
        monacoRef = m;
        return m;
      })
      .catch(e => {
        loadError = e;
        throw e;
      });
  }
  return monacoPromise;
}

export function useMonacoLoader() {
  const loadStartRef = useRef<number | null>(performance.now());
  const [state, setState] = useState<{
    monaco: typeof import('monaco-editor') | null;
    loading: boolean;
    error: any;
    loadStartTs: number | null;
  }>({
    monaco: monacoRef,
    loading: !monacoRef && !loadError,
    error: loadError,
    loadStartTs: loadStartRef.current
  });

  useEffect(() => {
    if (monacoRef || loadError) {
      setState(s => ({ ...s, loading: false }));
      return;
    }
    let cancelled = false;
    loadMonaco()
      .then(m => {
        if (!cancelled) setState({ monaco: m, loading: false, error: null, loadStartTs: loadStartRef.current });
      })
      .catch(e => {
        if (!cancelled) setState({ monaco: null, loading: false, error: e, loadStartTs: loadStartRef.current });
      });
    return () => { cancelled = true; };
  }, []);

  return state;
}
