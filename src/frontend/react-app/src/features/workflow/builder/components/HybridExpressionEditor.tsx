// Finished H7: dynamic variable context fetch + manual refresh
import React, { useCallback, useEffect, useRef, useState } from 'react';
import { Box, CircularProgress, IconButton, Stack, TextField, Tooltip, Typography } from '@mui/material';
import { Refresh as RefreshIcon } from '@mui/icons-material';
import { workflowService } from '@/services/workflow.service';
import { setJsonLogicVariables } from '../monaco/jsonlogicOperators';
import { useExpressionSettingsOptional } from '../context/ExpressionSettingsContext';
import { MonacoExpressionEditor } from './MonacoExpressionEditor';
import ExpressionEditor from './ExpressionEditor';

export interface HybridExpressionEditorProps {
  value: string;
  onChange: (val: string) => void;
  kind?: 'gateway' | 'join' | 'task-assignment';
  placeholder?: string;
  height?: number;
  useMonaco?: boolean;
  language?: string;
  semantic?: boolean;
  disableSemanticOnError?: boolean;
  onSemanticValidation?: (res: { success: boolean; errors: string[]; durationMs: number }) => void;
  variableContext?: string[];
  variableDeps?: any[];
  disableDynamicVars?: boolean;
}

type SemanticState = 'idle' | 'validating' | 'ok' | 'err';

const DEFAULT_HEIGHT = 160;

const HybridExpressionEditor: React.FC<HybridExpressionEditorProps> = ({
  value,
  onChange,
  kind = 'gateway',
  placeholder,
  height = DEFAULT_HEIGHT,
  useMonaco = true,
  semantic = true,
  disableSemanticOnError = true,
  onSemanticValidation,
  variableContext,
  variableDeps = [],
  disableDynamicVars = false,
}) => {

  // SAFE: does not throw if provider missing (prevents blank screen)
  const { semanticEnabled, recordSemanticValidation } = useExpressionSettingsOptional();
  const effectiveSemantic = semantic && semanticEnabled;

  const [parseError, setParseError] = useState<string | null>(null);
  const [semanticErrors, setSemanticErrors] = useState<string[]>([]);
  const [semanticWarnings, setSemanticWarnings] = useState<string[]>([]);
  const [semanticState, setSemanticState] = useState<SemanticState>('idle');
  const [loadingSemantic, setLoadingSemantic] = useState(false);
  const [semanticVersion, setSemanticVersion] = useState(0);

  const [loadingVars, setLoadingVars] = useState(false);
  const [varsVersion, setVarsVersion] = useState(0);

  const debounceRef = useRef<number | undefined>(undefined);
  const activeRequestVersionRef = useRef<number>(0);

  /* ---------------- Variable Handling ---------------- */
  const loadVariables = useCallback(async () => {
    if (variableContext?.length) {
      setJsonLogicVariables(variableContext);
      setVarsVersion(v => v + 1);
      return;
    }
    if (disableDynamicVars) return;
    setLoadingVars(true);
    try {
      const vars = await workflowService.getExpressionVariables(
        kind === 'task-assignment' ? 'gateway' : kind
      );
      setJsonLogicVariables(vars);
      setVarsVersion(v => v + 1);
    } finally {
      setLoadingVars(false);
    }
  }, [variableContext, disableDynamicVars, kind]);

  useEffect(() => {
    loadVariables();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [kind, ...variableDeps]);

  const manualRefreshVariables = () => loadVariables();

  /* ---------------- Local Parse Validation ---------------- */
  const validateLocal = (text: string) => {
    if (!text.trim()) {
      setParseError(null);
      return;
    }
    try {
      JSON.parse(text);
      setParseError(null);
    } catch (e: any) {
      setParseError(e?.message || 'Invalid JSON');
    }
  };

  /* ---------------- Semantic Validation (debounced) ---------------- */
  const clearDebounce = () => {
    if (debounceRef.current) {
      clearTimeout(debounceRef.current);
      debounceRef.current = undefined;
    }
  };

  const resetSemantic = () => {
    setSemanticErrors([]);
    setSemanticWarnings([]);
    setSemanticState('idle');
    setLoadingSemantic(false);
  };

  const scheduleSemantic = (expr: string) => {
    if (!effectiveSemantic) {
      clearDebounce();
      resetSemantic();
      return;
    }
    clearDebounce();
    debounceRef.current = window.setTimeout(() => runSemantic(expr), 600);
  };

  const runSemantic = async (expr: string) => {
    if (!effectiveSemantic) return;
    if (!expr.trim()) {
      setSemanticState('idle');
      onSemanticValidation?.({ success: true, errors: [], durationMs: 0 });
      return;
    }
    // Skip if local parse invalid
    if (disableSemanticOnError && parseError) {
      setSemanticState('err');
      onSemanticValidation?.({ success: false, errors: [parseError], durationMs: 0 });
      return;
    }

    const requestVersion = activeRequestVersionRef.current + 1;
    activeRequestVersionRef.current = requestVersion;

    setSemanticState('validating');
    setLoadingSemantic(true);

    const started = performance.now();
    try {
      const res = await workflowService.validateExpression(
        kind === 'task-assignment' ? 'gateway' : kind,
        expr
      );
      const duration = performance.now() - started;

      // Race guard
      if (requestVersion !== activeRequestVersionRef.current) return;

      setSemanticErrors(res.errors ?? []);
      setSemanticWarnings(res.warnings ?? []);
      setSemanticVersion(requestVersion);

      if (res.success && (res.errors?.length ?? 0) === 0) {
        setSemanticState('ok');
        onSemanticValidation?.({ success: true, errors: [], durationMs: duration });
      } else {
        setSemanticState('err');
        onSemanticValidation?.({ success: false, errors: res.errors ?? [], durationMs: duration });
      }

      recordSemanticValidation(
        Math.round(duration),
        res.success,
        res.errors?.length ?? 0,
        res.warnings?.length ?? 0
      );
    } catch (e: any) {
      if (requestVersion !== activeRequestVersionRef.current) return;
      setSemanticState('err');
      onSemanticValidation?.({
        success: false,
        errors: [e?.message || 'Semantic validation failed'],
        durationMs: 0
      });
    } finally {
      if (requestVersion === activeRequestVersionRef.current) {
        setLoadingSemantic(false);
      }
    }
  };

  // Re-run semantic when toggled on
  useEffect(() => {
    if (effectiveSemantic) {
      scheduleSemantic(value);
    } else {
      resetSemantic();
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [effectiveSemantic]);

  // Re-run when vars refreshed
  useEffect(() => {
    if (effectiveSemantic) scheduleSemantic(value);
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [varsVersion]);

  useEffect(() => () => clearDebounce(), []);

  /* ---------------- Render Helpers ---------------- */
  const showSemanticChip = effectiveSemantic && (semanticState !== 'idle' || parseError);

  const SemanticChip = () => {
    if (!showSemanticChip) return null;
    let label = 'Idle';
    let color: 'default' | 'error' | 'success' | 'warning' = 'default';
    if (parseError) {
      label = 'Parse Err';
      color = 'error';
    } else {
      switch (semanticState) {
        case 'validating':
          label = 'Validating…'; color = 'warning'; break;
        case 'ok':
          label = 'Valid'; color = 'success'; break;
        case 'err':
          label = 'Invalid'; color = 'error'; break;
      }
    }
    return (
      <Box
        sx={{
          px: 1,
          py: 0.25,
          borderRadius: 1,
          fontSize: '0.65rem',
          bgcolor: (t) =>
            color === 'success' ? t.palette.success.light :
              color === 'error' ? t.palette.error.light :
                color === 'warning' ? t.palette.warning.light :
                  t.palette.action.hover,
          color: (t) =>
            color === 'success' ? t.palette.success.contrastText :
              color === 'error' ? t.palette.error.contrastText :
                color === 'warning' ? t.palette.warning.contrastText :
                  t.palette.text.secondary
        }}
      >
        {label}
      </Box>
    );
  };

  /* ---------------- Editor Selection ---------------- */
  if (useMonaco) {
    return (
      <Box>
        <Stack direction="row" spacing={1} justifyContent="flex-end" mb={0.5} alignItems="center">
          <Tooltip title="Refresh variables">
            <span>
              <IconButton
                size="small"
                onClick={manualRefreshVariables}
                disabled={loadingVars}
              >
                <RefreshIcon fontSize="inherit" />
              </IconButton>
            </span>
          </Tooltip>
          {loadingVars && <CircularProgress size={14} />}
          <SemanticChip />
        </Stack>
        <MonacoExpressionEditor
          kind={kind === 'task-assignment' ? 'gateway' : (kind as 'gateway' | 'join')}
          value={value}
          onChange={(txt) => {
            onChange(txt);
            validateLocal(txt);
            scheduleSemantic(txt);
          }}
          semanticErrors={semanticErrors}
          semanticWarnings={semanticWarnings}
          loadingSemantic={loadingSemantic || loadingVars}
          semantic={effectiveSemantic}
          semanticVersion={semanticVersion}
          height={height}
        />
        {placeholder && !value.trim() && (
          <Typography variant="caption" color="text.secondary">
            {placeholder}
          </Typography>
        )}
        {parseError && (
          <Typography variant="caption" color="error" sx={{ display: 'block', mt: 0.5 }}>
            {parseError}
          </Typography>
        )}
      </Box>
    );
  }

  // Plain fallback (textarea / legacy editor)
  return (
    <Box>
      <Stack direction="row" spacing={1} justifyContent="flex-end" mb={0.5} alignItems="center">
        <Tooltip title="Refresh variables">
          <span>
            <IconButton
              size="small"
              onClick={manualRefreshVariables}
              disabled={loadingVars}
            >
              <RefreshIcon fontSize="inherit" />
            </IconButton>
          </span>
        </Tooltip>
        {loadingVars && <CircularProgress size={14} />}
        <SemanticChip />
      </Stack>
      <Box sx={{ position: 'relative', border: theme => `1px solid ${theme.palette.divider}`, borderRadius: 1, p: 1, minHeight: height }}>
        <ExpressionEditor
          kind={kind === 'task-assignment' ? 'gateway' : (kind as 'gateway' | 'join')}
          value={value}
          onChange={(txt) => {
            onChange(txt);
            validateLocal(txt);
            scheduleSemantic(txt);
          }}
          onValidityChange={() => void 0}
        />
      </Box>
      {placeholder && !value.trim() && (
        <Typography variant="caption" color="text.secondary">
          {placeholder}
        </Typography>
      )}
      {parseError && (
        <Typography variant="caption" color="error" sx={{ display: 'block', mt: 0.5 }}>
          {parseError}
        </Typography>
      )}
    </Box>
  );
};

export default HybridExpressionEditor;
export { HybridExpressionEditor };
