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
import { useMonacoLoader } from '../monaco/useMonacoLoader';
import { registerJsonLogicEnhancements } from '../monaco/registerJsonLogicLanguage';
import ExpressionEditor from './ExpressionEditor';
import { useExpressionSettings } from '../context/ExpressionSettingsContext';

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
  const { monaco, loading, error, loadStartTs } = useMonacoLoader();
  const { effectiveResolvedTheme, recordMonacoLoad } = useExpressionSettings();
  const containerRef = useRef<HTMLDivElement | null>(null);
  const editorRef = useRef<import('monaco-editor').editor.IStandaloneCodeEditor | null>(null);
  const [jsonValid, setJsonValid] = useState<boolean>(true);
  const [lastLocalError, setLastLocalError] = useState<string | undefined>();
  const lastAppliedSemanticVersionRef = useRef<number | undefined>(undefined);
  const loadRecordedRef = useRef(false);

  // Theme application
  useEffect(() => {
    if (!monaco || !editorRef.current) return;
    const themeName =
      effectiveResolvedTheme === 'dark'
        ? 'vs-dark'
        : effectiveResolvedTheme === 'hc'
          ? 'hc-black'
          : 'vs';
    monaco.editor.setTheme(themeName);
  }, [monaco, effectiveResolvedTheme]);

  // Local parse + markers
  const applyLocalValidation = useCallback(
    (current: string) => {
      if (!monaco || !editorRef.current) return;
      const model = editorRef.current.getModel();
      if (!model) return;

      const markers: import('monaco-editor').editor.IMarkerData[] = [];
      if (!current.trim()) {
        setJsonValid(false);
        setLastLocalError('Expression required');
        markers.push({
          severity: monaco.MarkerSeverity.Error,
          message: 'Expression required',
          startLineNumber: 1,
          startColumn: 1,
          endLineNumber: 1,
          endColumn: 1
        });
      } else {
        try {
          JSON.parse(current);
          setJsonValid(true);
          setLastLocalError(undefined);
        } catch (e: any) {
          setJsonValid(false);
          setLastLocalError(e.message);
          markers.push({
            severity: monaco.MarkerSeverity.Error,
            message: e.message,
            startLineNumber: 1,
            startColumn: 1,
            endLineNumber: 1,
            endColumn: 1
          });
        }
      }
      monaco.editor.setModelMarkers(model, 'jsonlogic-local', markers);
    },
    [monaco]
  );

  // Semantic markers
  useEffect(() => {
    if (!monaco || !editorRef.current) return;
    if (semanticVersion == null) return;
    if (
      lastAppliedSemanticVersionRef.current != null &&
      semanticVersion < lastAppliedSemanticVersionRef.current
    ) {
      return;
    }
    lastAppliedSemanticVersionRef.current = semanticVersion;
    const model = editorRef.current.getModel();
    if (!model) return;
    const markers: import('monaco-editor').editor.IMarkerData[] = [];
    for (const err of semanticErrors) {
      markers.push({
        severity: monaco.MarkerSeverity.Error,
        message: err,
        startLineNumber: 1,
        startColumn: 1,
        endLineNumber: 1,
        endColumn: 1
      });
    }
    for (const warn of semanticWarnings) {
      markers.push({
        severity: monaco.MarkerSeverity.Warning,
        message: warn,
        startLineNumber: 1,
        startColumn: 1,
        endLineNumber: 1,
        endColumn: 1
      });
    }
    monaco.editor.setModelMarkers(model, 'jsonlogic-semantic', markers);
  }, [semanticErrors, semanticWarnings, monaco, semanticVersion]);

  // Initialize editor
  useEffect(() => {
    if (!monaco || !containerRef.current || editorRef.current) return;
    registerJsonLogicEnhancements(monaco);
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

    const sub = editor.onDidChangeModelContent(() => {
      const current = editor.getValue();
      onChange(current);
      applyLocalValidation(current);
      if (semantic && onSemanticValidate && jsonValid) {
        onSemanticValidate(current);
      }
    });

    // Theme after create
    const themeName =
      effectiveResolvedTheme === 'dark'
        ? 'vs-dark'
        : effectiveResolvedTheme === 'hc'
          ? 'hc-black'
          : 'vs';
    monaco.editor.setTheme(themeName);

    // Initial markers
    applyLocalValidation(value || '');

    // Load time telemetry
    if (!loadRecordedRef.current && loadStartTs != null) {
      loadRecordedRef.current = true;
      const delta = performance.now() - loadStartTs;
      recordMonacoLoad(Math.round(delta));
    }

    return () => {
      sub.dispose();
      editor.dispose();
      editorRef.current = null;
    };
  }, [
    monaco,
    applyLocalValidation,
    value,
    onChange,
    semantic,
    onSemanticValidate,
    jsonValid,
    readOnly,
    kind,
    ariaLabel,
    effectiveResolvedTheme,
    loadStartTs,
    recordMonacoLoad
  ]);

  // External value sync
  useEffect(() => {
    if (editorRef.current && value !== editorRef.current.getValue()) {
      editorRef.current.setValue(value || '');
      applyLocalValidation(value || '');
    }
  }, [value, applyLocalValidation]);

  const manualValidate = () => {
    if (editorRef.current) {
      const current = editorRef.current.getValue();
      applyLocalValidation(current);
      if (semantic && jsonValid && onSemanticValidate) onSemanticValidate(current);
    }
  };

  const formatDocument = () => {
    if (editorRef.current && monaco) {
      editorRef.current.getAction('editor.action.formatDocument')?.run();
    }
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

  if (loading) {
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

  if (error) {
    return (
      <Box sx={{ display: 'flex', flexDirection: 'column', gap: 1 }}>
        <Typography variant="body2" fontWeight={600}>{label}</Typography>
        <Chip size="small" label="Fallback" color="warning" sx={{ maxWidth: 'fit-content' }} />
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
      <Stack direction="row" spacing={1} alignItems="center">
        <Typography variant="body2" fontWeight={600}>{label}</Typography>
        <Chip
          size="small"
          variant="outlined"
          color={statusChip.color}
          label={statusChip.label}
        />
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
