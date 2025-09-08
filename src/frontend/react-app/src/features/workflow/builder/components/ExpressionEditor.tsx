import { useEffect, useState, useCallback } from 'react';
import { Box, TextField, Typography, IconButton, Tooltip, Chip, Stack, CircularProgress } from '@mui/material';
import RefreshIcon from '@mui/icons-material/Refresh';
import { workflowService } from '@/services/workflow.service';

export interface ExpressionEditorProps {
  label?: string;
  placeholder?: string;
  value?: string;
  onChange: (val: string) => void;
  onValidityChange?: (valid: boolean) => void;
  disabled?: boolean;
  compact?: boolean;
  kind?: 'gateway' | 'join';
}

/**
 * Lightweight JsonLogic expression editor.
 * Currently performs only JSON parse validation.
 * Future (backend) validation can be plugged in where marked.
 */
export function ExpressionEditor({
  label = 'Expression (JsonLogic)',
  placeholder = '{"==":[{"var":"approved"},true]}',
  value,
  onChange,
  onValidityChange,
  disabled,
  compact,
  kind = 'gateway'
}: ExpressionEditorProps) {
  const [raw, setRaw] = useState(value ?? '');
  const [isValid, setIsValid] = useState(true);
  const [lastValidated, setLastValidated] = useState<number>(0);
  const [semanticLoading, setSemanticLoading] = useState(false);
  const [semanticErrors, setSemanticErrors] = useState<string[]>([]);
  const [semanticWarnings, setSemanticWarnings] = useState<string[]>([]);

  const validate = useCallback((text: string) => {
    if (!text || !text.trim()) {
      setIsValid(false);
      onValidityChange?.(false);
      return;
    }
    try {
      JSON.parse(text);
      setIsValid(true);
      onValidityChange?.(true);
    } catch {
      setIsValid(false);
      onValidityChange?.(false);
    }
    setLastValidated(Date.now());
  }, [onValidityChange]);

  useEffect(() => {
    setRaw(value ?? '');
    // Re-validate when external value changes
    if (value !== undefined) validate(value);
  }, [value, validate]);

  // Debounced semantic validate (basic 500ms after last change)
  useEffect(() => {
    const handle = setTimeout(async () => {
      if (!raw.trim()) {
        setSemanticErrors([]);
        setSemanticWarnings([]);
        return;
      }
      // Only run if local JSON is syntactically valid
      try { JSON.parse(raw); } catch { return; }

      setSemanticLoading(true);
      try {
        const res = await workflowService.validateExpression(kind, raw);
        setSemanticErrors(res.errors);
        setSemanticWarnings(res.warnings);
      } finally {
        setSemanticLoading(false);
      }
    }, 500);
    return () => clearTimeout(handle);
  }, [raw, kind]);

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const next = e.target.value;
    setRaw(next);
    onChange(next);
  };

  const handleManualValidate = () => validate(raw);

  return (
    <Box sx={{ display: 'flex', flexDirection: 'column', gap: compact ? 0.5 : 1 }}>
      <Stack direction="row" spacing={1} alignItems="center">
        <Typography variant="body2" fontWeight={600}>
          {label}
        </Typography>
        <Chip
          size="small"
          label={isValid ? 'Valid JSON' : 'Invalid'}
          color={isValid ? 'success' : 'error'}
          variant="outlined"
        />
        <Tooltip title="Re-validate now">
          <IconButton size="small" onClick={handleManualValidate} disabled={disabled}>
            <RefreshIcon fontSize="inherit" />
          </IconButton>
        </Tooltip>
      </Stack>
      <TextField
        multiline
        minRows={compact ? 2 : 4}
        size="small"
        value={raw}
        onChange={handleChange}
        onBlur={() => validate(raw)}
        placeholder={placeholder}
        disabled={disabled}
        error={!isValid}
        helperText={
          !isValid
            ? 'Must be valid JSON (JsonLogic shape)'
            : semanticErrors.length
            ? semanticErrors[0]
            : semanticWarnings.length
            ? semanticWarnings[0]
            : `${kind === 'gateway' ? 'Gateway' : 'Join'} condition parsed OK`
        }
        FormHelperTextProps={{
          sx: { fontSize: '0.7rem', lineHeight: 1.1 }
        }}
      />
      {semanticLoading && (
        <Stack direction="row" spacing={1} sx={{ mt: 0.5 }}>
          <CircularProgress size={14} />
          <Typography variant="caption" color="text.secondary">Checking semantics...</Typography>
        </Stack>
      )}
      {!semanticLoading && semanticWarnings.length > 1 && (
        <Typography variant="caption" color="warning.main">
          {semanticWarnings.length} warnings
        </Typography>
      )}
    </Box>
  );
}

export default ExpressionEditor;
