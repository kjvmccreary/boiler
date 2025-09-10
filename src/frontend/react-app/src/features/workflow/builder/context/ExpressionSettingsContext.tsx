import React, {
  createContext,
  useCallback,
  useContext,
  useEffect,
  useMemo,
  useRef,
  useState
} from 'react';

export type MonacoThemePreference = 'system' | 'light' | 'dark' | 'hc';

export interface MonacoTelemetrySnapshot {
  loads: number;
  lastLoadMs?: number;
  avgLoadMs?: number;
  semanticCalls: number;
  semanticErrors: number;
  semanticWarnings: number;
  lastSemanticDurationMs?: number;
  avgSemanticDurationMs?: number;
}

export interface ExpressionSettingsContextValue {
  semanticEnabled: boolean;
  theme: MonacoThemePreference;
  effectiveResolvedTheme: 'light' | 'dark' | 'hc';
  toggleSemantic: () => void;
  setSemantic: (enabled: boolean) => void;
  setTheme: (pref: MonacoThemePreference) => void;
  telemetry: MonacoTelemetrySnapshot;
  recordMonacoLoad: (ms: number) => void;
  recordSemanticValidation: (
    durationMs: number,
    success: boolean,
    errorCount: number,
    warningCount: number
  ) => void;
}

const ExpressionSettingsContext = createContext<ExpressionSettingsContextValue | undefined>(undefined);

const STORAGE_KEY_SEMANTIC = 'wf.semanticValidation';
const STORAGE_KEY_THEME = 'wf.monacoTheme';

export const ExpressionSettingsProvider: React.FC<{ children: React.ReactNode }> = ({ children }) => {
  const [semanticEnabled, setSemanticEnabled] = useState<boolean>(true);
  const [theme, setThemeState] = useState<MonacoThemePreference>('system');

  const loadsRef = useRef(0);
  const totalLoadMsRef = useRef(0);
  const lastLoadMsRef = useRef<number | undefined>(undefined);

  const semanticCallsRef = useRef(0);
  const semanticErrorsRef = useRef(0);
  const semanticWarningsRef = useRef(0);
  const lastSemanticDurationRef = useRef<number | undefined>(undefined);
  const totalSemanticDurationRef = useRef(0);

  useEffect(() => {
    try {
      const raw = localStorage.getItem(STORAGE_KEY_SEMANTIC);
      if (raw === 'off') setSemanticEnabled(false);
      const t = localStorage.getItem(STORAGE_KEY_THEME) as MonacoThemePreference | null;
      if (t && ['system', 'light', 'dark', 'hc'].includes(t)) setThemeState(t);
    } catch {
      /* ignore */
    }
  }, []);

  const persistSemantic = (enabled: boolean) => {
    try { localStorage.setItem(STORAGE_KEY_SEMANTIC, enabled ? 'on' : 'off'); } catch { /* ignore */ }
  };
  const persistTheme = (pref: MonacoThemePreference) => {
    try { localStorage.setItem(STORAGE_KEY_THEME, pref); } catch { /* ignore */ }
  };

  const setSemantic = useCallback((enabled: boolean) => {
    setSemanticEnabled(enabled);
    persistSemantic(enabled);
  }, []);

  const toggleSemantic = useCallback(() => {
    setSemanticEnabled(prev => {
      const next = !prev;
      persistSemantic(next);
      return next;
    });
  }, []);

  const setTheme = useCallback((pref: MonacoThemePreference) => {
    setThemeState(pref);
    persistTheme(pref);
  }, []);

  const effectiveResolvedTheme = useMemo<'light' | 'dark' | 'hc'>(() => {
    if (theme === 'hc') return 'hc';
    if (theme === 'light') return 'light';
    if (theme === 'dark') return 'dark';
    try {
      const prefersDark = window.matchMedia?.('(prefers-color-scheme: dark)')?.matches;
      return prefersDark ? 'dark' : 'light';
    } catch {
      return 'light';
    }
  }, [theme]);

  const recordMonacoLoad = useCallback((ms: number) => {
    loadsRef.current += 1;
    totalLoadMsRef.current += ms;
    lastLoadMsRef.current = ms;
    // eslint-disable-next-line no-console
    console.log('[Monaco][Telemetry] load ms:', ms);
  }, []);

  const recordSemanticValidation = useCallback((durationMs: number, success: boolean, errorCount: number, warningCount: number) => {
    semanticCallsRef.current += 1;
    totalSemanticDurationRef.current += durationMs;
    lastSemanticDurationRef.current = durationMs;
    if (!success) semanticErrorsRef.current += errorCount;
    if (warningCount) semanticWarningsRef.current += warningCount;
    // eslint-disable-next-line no-console
    console.log('[Monaco][Telemetry] semantic ms:', durationMs, 'errors:', errorCount, 'warnings:', warningCount);
  }, []);

  const telemetry: MonacoTelemetrySnapshot = useMemo(() => {
    const loads = loadsRef.current;
    const semanticCalls = semanticCallsRef.current;
    return {
      loads,
      lastLoadMs: lastLoadMsRef.current,
      avgLoadMs: loads > 0 ? Math.round((totalLoadMsRef.current / loads) * 10) / 10 : undefined,
      semanticCalls,
      semanticErrors: semanticErrorsRef.current,
      semanticWarnings: semanticWarningsRef.current,
      lastSemanticDurationMs: lastSemanticDurationRef.current,
      avgSemanticDurationMs: semanticCalls > 0
        ? Math.round((totalSemanticDurationRef.current / semanticCalls) * 10) / 10
        : undefined
    };
  }, [semanticEnabled, theme, effectiveResolvedTheme]);

  return (
    <ExpressionSettingsContext.Provider
      value={{
        semanticEnabled,
        toggleSemantic,
        setSemantic,
        theme,
        setTheme,
        effectiveResolvedTheme,
        telemetry,
        recordMonacoLoad,
        recordSemanticValidation
      }}
    >
      {children}
    </ExpressionSettingsContext.Provider>
  );
};

/**
 * Strict hook – throws if provider absent (original behavior).
 */
export function useExpressionSettings(): ExpressionSettingsContextValue {
  const ctx = useContext(ExpressionSettingsContext);
  if (!ctx) throw new Error('useExpressionSettings must be used within ExpressionSettingsProvider');
  return ctx;
}

/**
 * Lenient hook – returns fallback defaults if provider not mounted.
 * Prevents hard crash while you integrate Provider at the builder root.
 */
export function useExpressionSettingsOptional(): ExpressionSettingsContextValue {
  const ctx = useContext(ExpressionSettingsContext);
  if (ctx) return ctx;
  // Single noisy warning only.
  if (typeof window !== 'undefined' && !(window as any).__exprSettingsWarned) {
    (window as any).__exprSettingsWarned = true;
    // eslint-disable-next-line no-console
    console.warn('[ExpressionSettings] Provider missing. Using fallback defaults (semantic ON, light theme).');
  }
  return {
    semanticEnabled: true,
    theme: 'light',
    effectiveResolvedTheme: 'light',
    toggleSemantic: () => void 0,
    setSemantic: () => void 0,
    setTheme: () => void 0,
    telemetry: {
      loads: 0,
      semanticCalls: 0,
      semanticErrors: 0,
      semanticWarnings: 0
    },
    recordMonacoLoad: () => void 0,
    recordSemanticValidation: () => void 0
  };
}

export { ExpressionSettingsContext };
