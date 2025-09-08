import { useEffect, useState, useCallback } from 'react';
import { Box, TextField, Typography, IconButton, Tooltip, Chip, Stack } from '@mui/material';
import RefreshIcon from '@mui/icons-material/Refresh';

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
          isValid
            ? `${kind === 'gateway' ? 'Gateway' : 'Join'} condition parsed OK ${
                lastValidated ? `(@${new Date(lastValidated).toLocaleTimeString()})` : ''
              }`
            : 'Must be valid JSON (JsonLogic shape)'
        }
        FormHelperTextProps={{
          sx: { fontSize: '0.7rem', lineHeight: 1.1 }
        }}
      />
    </Box>
  );
}

export default ExpressionEditor;
