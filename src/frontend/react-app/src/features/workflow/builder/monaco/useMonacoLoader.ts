// DEPRECATED: Prefer ensureMonaco() from monacoLoader.ts
import { useEffect, useState } from 'react';
import { ensureMonaco } from './monacoLoader';

export function useMonacoLoader() {
  const [state, setState] = useState({
    monaco: null as typeof import('monaco-editor') | null,
    loading: true,
    error: null as any,
    loadStartTs: performance.now()
  });

  useEffect(() => {
    let cancelled = false;
    ensureMonaco()
      .then(r => {
        if (cancelled) return;
        setState(s => ({ ...s, monaco: r.monaco, loading: false, error: null }));
      })
      .catch(e => {
        if (cancelled) return;
        setState(s => ({ ...s, loading: false, error: e }));
      });
    return () => { cancelled = true; };
  }, []);

  return state;
}
