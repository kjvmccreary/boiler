import React, { useEffect, useRef, useState, useCallback, useMemo } from 'react';
import {
  Box,
  CircularProgress,
  Typography,
  Stack,
  IconButton,
  Tooltip,
  Chip
} from '@mui/material';
import RefreshIcon from '@mui/icons-material/Refresh';
import FormatAlignLeftIcon from '@mui/icons-material/FormatAlignLeft';
// Replaced useMonacoLoader with optimized loader (PR1)
import {
  ensureMonaco,
  prefetchMonacoOnIdle,
  registerEditorInstance,
  unregisterEditorInstance,
  applyThemeIfChanged
} from '../monaco/monacoLoader';
import { registerJsonLogicEnhancements } from '../monaco/registerJsonLogicLanguage';
import ExpressionEditor from './ExpressionEditor';
import { useExpressionSettings } from '../context/ExpressionSettingsContext';
import { trackWorkflow } from '@/features/workflow/telemetry/workflowTelemetry';

export interface MonacoExpressionEditorProps {
  value: string;
  onChange: (val: string) => void;
  onSemanticValidate?: (value: string) => void;
  semanticErrors?: string[];
  semanticWarnings?: string[];
  loadingSemantic?: boolean;
  label?: string;
  kind?: 'gateway' | 'join';
  height?: number;
  readOnly?: boolean;
  semantic?: boolean;
  ariaLabel?: string;
  semanticVersion?: number;
}

export const MonacoExpressionEditor: React.FC<MonacoExpressionEditorProps> = ({
  value,
  onChange,
  onSemanticValidate,
  semanticErrors = [],
  semanticWarnings = [],
  loadingSemantic,
  label = 'Expression (JsonLogic)',
  kind = 'gateway',
  height = 220,
  readOnly,
  semantic = true,
  ariaLabel,
  semanticVersion
}) => {
  // Stable editor instance id
  const editorIdRef = useRef<string>(`ed_${Math.random().toString(36).slice(2)}`);
  const [monacoState, setMonacoState] = useState<{
    monaco: typeof import('monaco-editor') | null;
    loading: boolean;
    error: any;
    loadStartTs: number | null;
    firstLoad: boolean;
    deferred: boolean;
    loadAttempted: boolean;
  }>({ monaco: null, loading: false, error: null, loadStartTs: null, firstLoad: false, deferred: true, loadAttempted: false });
  const { effectiveResolvedTheme, recordMonacoLoad, semanticEnabled } = useExpressionSettings();
  const containerRef = useRef<HTMLDivElement | null>(null);
  const editorRef = useRef<import('monaco-editor').editor.IStandaloneCodeEditor | null>(null);
  const [jsonValid, setJsonValid] = useState<boolean>(true);
  const [lastLocalError, setLastLocalError] = useState<string | undefined>();
  const [pendingValue, setPendingValue] = useState<string>(value || '');
  const parseDebounceRef = useRef<number | undefined>(undefined);
  const lastParsedValueRef = useRef<string>('');
  const lastAppliedSemanticVersionRef = useRef<number | undefined>(undefined);
  const loadRecordedRef = useRef(false);

  // Theme
  useEffect(() => {
    const monaco = monacoState.monaco;
    if (!monaco || !editorRef.current) return;
    const themeName =
      effectiveResolvedTheme === 'dark'
        ? 'vs-dark'
        : effectiveResolvedTheme === 'hc'
          ? 'hc-black'
          : 'vs';
    applyThemeIfChanged(monaco, themeName);
  }, [monacoState.monaco, effectiveResolvedTheme]);

  // Bucket helper for parse telemetry
  function bucketMs(ms: number): string {
    if (ms < 1) return '<1';
    if (ms < 2) return '<2';
    if (ms < 5) return '<5';
    if (ms < 10) return '<10';
    if (ms < 25) return '<25';
    if (ms < 50) return '<50';
    return '50+';
  }

  const runLocalParse = useCallback((current: string) => {
    if (!monacoState.monaco || !editorRef.current) return;
    if (current === lastParsedValueRef.current) return; // skip duplicate
    lastParsedValueRef.current = current;
    const model = editorRef.current.getModel();
    if (!model) return;
    const markers: import('monaco-editor').editor.IMarkerData[] = [];
    if (!current.trim()) {
      setJsonValid(false);
      setLastLocalError('Expression required');
      markers.push({
        severity: monacoState.monaco.MarkerSeverity.Error,
        message: 'Expression required',
        startLineNumber: 1,
        startColumn: 1,
        endLineNumber: 1,
        endColumn: 1
      });
      monacoState.monaco.editor.setModelMarkers(model, 'jsonlogic-local', markers);
      return;
    }

    const t0 = performance.now();
    try {
      JSON.parse(current);
      const elapsed = performance.now() - t0;
      trackWorkflow('monaco.local.parse.ms', { ms: Math.round(elapsed), bucket: bucketMs(elapsed) });
      setJsonValid(true);
      setLastLocalError(undefined);
    } catch (e: any) {
      const elapsed = performance.now() - t0;
      trackWorkflow('monaco.local.parse.ms', { ms: Math.round(elapsed), bucket: bucketMs(elapsed), error: true });
      setJsonValid(false);
      setLastLocalError(e.message);
      markers.push({
        severity: monacoState.monaco.MarkerSeverity.Error,
        message: e.message,
        startLineNumber: 1,
        startColumn: 1,
        endLineNumber: 1,
        endColumn: 1
      });
    }
    monacoState.monaco.editor.setModelMarkers(model, 'jsonlogic-local', markers);
  }, [monacoState.monaco]);

  const scheduleLocalParse = useCallback((nextVal: string) => {
    setPendingValue(nextVal);
    if (parseDebounceRef.current) {
      clearTimeout(parseDebounceRef.current);
    }
    // Debounce 150ms
    parseDebounceRef.current = window.setTimeout(() => {
      runLocalParse(nextVal);
    }, 150);
  }, [runLocalParse]);

  // Semantic markers
  useEffect(() => {
    if (!monacoState.monaco || !editorRef.current) return;
    if (semanticVersion == null) return;
    if (
      lastAppliedSemanticVersionRef.current != null &&
      semanticVersion < lastAppliedSemanticVersionRef.current
    ) return;

    lastAppliedSemanticVersionRef.current = semanticVersion;
    const model = editorRef.current.getModel();
    if (!model) return;
    const markers: import('monaco-editor').editor.IMarkerData[] = [];

    for (const err of semanticErrors) {
      markers.push({
        severity: monacoState.monaco.MarkerSeverity.Error,
        message: err,
        startLineNumber: 1,
        startColumn: 1,
        endLineNumber: 1,
        endColumn: 1
      });
    }
    for (const warn of semanticWarnings) {
      markers.push({
        severity: monacoState.monaco.MarkerSeverity.Warning,
        message: warn,
        startLineNumber: 1,
        startColumn: 1,
        endLineNumber: 1,
        endColumn: 1
      });
    }
    monacoState.monaco.editor.setModelMarkers(model, 'jsonlogic-semantic', markers);
  }, [semanticErrors, semanticWarnings, monacoState.monaco, semanticVersion]);

  // Heuristic prefetch if user does not focus after 5s
  useEffect(() => {
    const t = setTimeout(() => {
      if (!monacoState.monaco && monacoState.deferred && !monacoState.loading) {
        prefetchMonacoOnIdle(0, 'heuristic');
      }
    }, 5000);
    return () => clearTimeout(t);
  }, [monacoState.monaco, monacoState.deferred, monacoState.loading]);

  const beginLoad = useCallback((trigger: 'focus' | 'button' | 'retry') => {
    if (monacoState.loading || monacoState.monaco) return;
    setMonacoState(s => ({ ...s, loading: true, loadStartTs: performance.now(), loadAttempted: true }));
    if (trigger === 'focus') {
      trackWorkflow('monaco.defer.used', { trigger });
    } else if (trigger === 'retry') {
      trackWorkflow('monaco.reload.attempt', {});
    }
    ensureMonaco()
      .then(r => {
        setMonacoState(s => ({
          ...s,
          monaco: r.monaco,
          loading: false,
          error: null,
          firstLoad: r.firstLoad,
          deferred: false
        }));
        prefetchMonacoOnIdle(); // idle warm (noop if loaded)
        if (trigger === 'retry') {
          trackWorkflow('monaco.reload.success', { durationMs: r.durationMs });
        }
      })
      .catch(e => {
        setMonacoState(s => ({ ...s, loading: false, error: e }));
        if (trigger === 'retry') {
          trackWorkflow('monaco.reload.failed', { message: String(e?.message || e) });
        }
      });
  }, [monacoState.loading, monacoState.monaco]);

  // Init
  useEffect(() => {
    const monaco = monacoState.monaco;
    if (!monaco || !containerRef.current || editorRef.current) return;
    try {
      registerJsonLogicEnhancements(monaco);
    } catch {
      // Non-fatal; enhancements optional
    }
    const editor = monaco.editor.create(containerRef.current, {
      value: value || '',
      language: 'json',
      automaticLayout: true,
      readOnly: !!readOnly,
      minimap: { enabled: false },
      lineNumbers: 'on',
      scrollbar: { vertical: 'auto' },
      ariaLabel: ariaLabel ?? `${kind} jsonlogic expression editor`
    });
    editorRef.current = editor;
    registerEditorInstance(editorIdRef.current);

    const sub = editor.onDidChangeModelContent(() => {
      const current = editor.getValue();
      onChange(current);
      scheduleLocalParse(current);
      // Semantic validation will run after parse completes (guarded in separate effect)
    });

    // Theme
    const themeName =
      effectiveResolvedTheme === 'dark'
        ? 'vs-dark'
        : effectiveResolvedTheme === 'hc'
          ? 'hc-black'
          : 'vs';
    monaco.editor.setTheme(themeName);

    // Initial
    scheduleLocalParse(value || '');
    if (!loadRecordedRef.current && monacoState.loadStartTs != null && monacoState.firstLoad) {
      loadRecordedRef.current = true;
      const delta = performance.now() - monacoState.loadStartTs;
      recordMonacoLoad(Math.round(delta));
    }

    return () => {
      sub.dispose();
      try { editor.dispose(); } catch { /* ignore */ }
      unregisterEditorInstance(editorIdRef.current);
      editorRef.current = null;
    };
  }, [
    monacoState.monaco,
    scheduleLocalParse,
    value,
    onChange,
    semantic,
    semanticEnabled,
    onSemanticValidate,
    jsonValid,
    readOnly,
    kind,
    ariaLabel,
    effectiveResolvedTheme,
    monacoState.loadStartTs,
    recordMonacoLoad,
    monacoState.firstLoad
  ]);

  // External value change
  useEffect(() => {
    if (editorRef.current && value !== editorRef.current.getValue()) {
      editorRef.current.setValue(value || '');
      scheduleLocalParse(value || '');
    }
  }, [value, scheduleLocalParse]);

  // Trigger semantic validation after (debounced) local parse success
  useEffect(() => {
    if (!semantic || !semanticEnabled) return;
    if (!jsonValid) return;
    if (!onSemanticValidate) return;
    // Only validate when pendingValue matches last parsed (ensures parse ran)
    if (pendingValue === lastParsedValueRef.current && pendingValue.trim()) {
      onSemanticValidate(pendingValue);
    }
  }, [semantic, semanticEnabled, jsonValid, pendingValue, onSemanticValidate]);

  const manualValidate = () => {
    if (!editorRef.current) return;
    const current = editorRef.current.getValue();
    runLocalParse(current);
    if (semantic && semanticEnabled && jsonValid && onSemanticValidate) {
      onSemanticValidate(current);
    }
  };

  const formatDocument = () => {
    editorRef.current?.getAction('editor.action.formatDocument')?.run();
  };

  const statusChip = useMemo(() => {
    if (!jsonValid) return { color: 'error' as const, label: 'Invalid JSON' };
    if (semanticErrors.length) return { color: 'error' as const, label: `${semanticErrors.length} error(s)` };
    if (semanticWarnings.length) return { color: 'warning' as const, label: `${semanticWarnings.length} warn` };
    return { color: 'success' as const, label: 'OK' };
  }, [jsonValid, semanticErrors, semanticWarnings]);

  const a11yMessage = useMemo(() => {
    if (!jsonValid && lastLocalError) return `JSON invalid: ${lastLocalError}`;
    if (semanticErrors.length) return `Semantic error: ${semanticErrors[0]}`;
    if (semanticWarnings.length) return `Semantic warning: ${semanticWarnings[0]}`;
    return 'Expression valid';
  }, [jsonValid, lastLocalError, semanticErrors, semanticWarnings]);

  const semanticChip = (
    <Tooltip title={semanticEnabled ? 'Semantic validation enabled' : 'Semantic validation disabled'}>
      <Chip
        size="small"
        variant="outlined"
        color={semanticEnabled ? 'primary' : 'default'}
        label={semanticEnabled ? 'Sem ON' : 'Sem OFF'}
        sx={{ ml: 0.5 }}
      />
    </Tooltip>
  );

  // Deferred shell (before load triggered)
  if (monacoState.deferred && !monacoState.monaco && !monacoState.loading && !monacoState.error) {
    return (
      <Box
        sx={{
          border: '1px dashed',
          borderColor: 'divider',
          borderRadius: 1,
          p: 1,
          height,
          position: 'relative',
          display: 'flex',
          flexDirection: 'column'
        }}
        onFocus={() => beginLoad('focus')}
        onClick={() => beginLoad('focus')}
        tabIndex={0}
      >
        <Typography variant="caption" color="text.secondary" sx={{ mb: 1 }}>
          Advanced editor deferred. Click or focus to load Monaco.
        </Typography>
        <ExpressionEditor
          value={value}
          kind={kind}
          onChange={(txt) => {
            onChange(txt);
            // If user starts typing, attempt load
            if (!monacoState.loading && !monacoState.monaco) beginLoad('focus');
          }}
          onValidityChange={() => void 0}
        />
      </Box>
    );
  }

  if (monacoState.loading) {
    return (
      <Box sx={{ display: 'flex', flexDirection: 'column', gap: 1 }}>
        <Typography variant="body2" fontWeight={600}>{label}</Typography>
        <Box
          sx={{
            height,
            border: '1px solid',
            borderColor: 'divider',
            borderRadius: 1,
            display: 'flex',
            alignItems: 'center',
            justifyContent: 'center'
          }}
        >
          <Stack alignItems="center" spacing={1}>
            <CircularProgress size={22} />
            <Typography variant="caption" color="text.secondary">
              Loading editor...
            </Typography>
          </Stack>
        </Box>
      </Box>
    );
  }

  if (monacoState.error) {
    return (
      <Box sx={{ display: 'flex', flexDirection: 'column', gap: 1 }}>
        <Typography variant="body2" fontWeight={600}>{label}</Typography>
        <Chip size="small" label="Fallback" color="warning" sx={{ maxWidth: 'fit-content' }} />
        <Box sx={{ display: 'flex', gap: 1, mb: 1 }}>
          <Chip
            size="small"
            label="Retry load editor"
            color="primary"
            onClick={() => beginLoad('retry')}
            variant="outlined"
          />
        </Box>
        <ExpressionEditor
          value={value}
          onChange={onChange}
          kind={kind}
          onValidityChange={() => void 0}
        />
      </Box>
    );
  }

  return (
    <Box sx={{ display: 'flex', flexDirection: 'column', gap: 0.5, position: 'relative' }}>
      <Box
        aria-live="polite"
        sx={{
          position: 'absolute',
          width: 1,
          height: 1,
          overflow: 'hidden',
          clip: 'rect(0 0 0 0)',
          whiteSpace: 'nowrap'
        }}
      >
        {a11yMessage}
      </Box>
      <Stack direction="row" spacing={1} alignItems="center" flexWrap="wrap">
        <Typography variant="body2" fontWeight={600}>{label}</Typography>
        <Chip
          size="small"
          variant="outlined"
          color={statusChip.color}
          label={statusChip.label}
        />
        {semanticChip}
        <Tooltip title="Re-validate">
          <IconButton size="small" onClick={manualValidate}>
            <RefreshIcon fontSize="inherit" />
          </IconButton>
        </Tooltip>
        <Tooltip title="Format (Ctrl/Cmd+Shift+F)">
          <IconButton size="small" onClick={formatDocument}>
            <FormatAlignLeftIcon fontSize="inherit" />
          </IconButton>
        </Tooltip>
        {loadingSemantic && (
          <Typography variant="caption" color="text.secondary">
            semantic...
          </Typography>
        )}
      </Stack>
      <Box
        ref={containerRef}
        sx={{
          border: '1px solid',
          borderColor: jsonValid ? 'divider' : 'error.main',
          borderRadius: 1,
          height,
          overflow: 'hidden',
          '& .monaco-editor': {
            fontSize: 13
          }
        }}
      />
      {lastLocalError && (
        <Typography variant="caption" color="error.main">
          {lastLocalError}
        </Typography>
      )}
      {!lastLocalError && semanticErrors.length > 0 && (
        <Typography variant="caption" color="error.main">
          {semanticErrors[0]}
        </Typography>
      )}
      {semanticErrors.length === 0 && semanticWarnings.length > 0 && (
        <Typography variant="caption" color="warning.main">
          {semanticWarnings[0]}
        </Typography>
      )}
    </Box>
  );
};
