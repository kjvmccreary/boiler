import React, { useEffect, useState } from 'react';
import { Box, Stack, Typography, Alert, Button, Tooltip } from '@mui/material';
import HybridExpressionEditor from './HybridExpressionEditor';

interface FormSchemaSectionProps {
  value: unknown;
  onChange: (parsed: unknown, source: string) => void;
  collapsible?: boolean;
}

type ParseState = 'idle' | 'valid' | 'error';

const MAX_SIZE = 25_000; // bytes warning threshold

export const FormSchemaSection: React.FC<FormSchemaSectionProps> = ({
  value,
  onChange
}) => {
  const [source, setSource] = useState<string>(() => {
    if (!value) return '{\n  "type": "object",\n  "properties": {}\n}';
    try {
      return typeof value === 'string' ? value : JSON.stringify(value, null, 2);
    } catch {
      return '// invalid form schema';
    }
  });
  const [state, setState] = useState<ParseState>('idle');
  const [error, setError] = useState<string | null>(null);
  const [sizeWarn, setSizeWarn] = useState<boolean>(false);

  useEffect(() => {
    // recompute size warning
    const bytes = new TextEncoder().encode(source).length;
    setSizeWarn(bytes > MAX_SIZE);
  }, [source]);

  const attemptParse = () => {
    if (!source.trim()) {
      setState('idle');
      setError(null);
      onChange(undefined, '');
      return;
    }
    try {
      const parsed = JSON.parse(source);
      setState('valid');
      setError(null);
      onChange(parsed, source);
    } catch (e: any) {
      setState('error');
      setError(e?.message || 'Invalid JSON');
    }
  };

  return (
    <Box>
      <Stack direction="row" justifyContent="space-between" alignItems="center" mb={0.5}>
        <Typography variant="subtitle2">Form Schema (JSON)</Typography>
        <Stack direction="row" spacing={1}>
          <Tooltip title="Validate / Parse JSON">
            <span>
              <Button
                size="small"
                variant="outlined"
                onClick={attemptParse}
                color={state === 'error' ? 'error' : 'primary'}
              >
                {state === 'error' ? 'Retry Parse' : 'Parse'}
              </Button>
            </span>
          </Tooltip>
        </Stack>
      </Stack>
      <HybridExpressionEditor
        kind="gateway" // reuses json Monaco; semantic not required here
        value={source}
        onChange={(v) => {
          setSource(v);
          setState('idle');
        }}
        useMonaco
        semantic={false}
        placeholder='{
  "type": "object",
  "properties": {
    "field1": { "type": "string" }
  }
}'
        height={220}
      />
      {error && (
        <Alert severity="error" sx={{ mt: 1, py: 0.5 }}>
          {error}
        </Alert>
      )}
      {state === 'valid' && !error && (
        <Alert severity="success" sx={{ mt: 1, py: 0.5 }}>
          Parsed OK
        </Alert>
      )}
      {sizeWarn && (
        <Alert severity="warning" sx={{ mt: 1, py: 0.5 }}>
          Schema size exceeds {Math.round(MAX_SIZE / 1024)}KB. Consider trimming.
        </Alert>
      )}
    </Box>
  );
};

export default FormSchemaSection;
