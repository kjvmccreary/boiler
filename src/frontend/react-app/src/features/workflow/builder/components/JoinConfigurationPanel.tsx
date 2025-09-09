import React, { useEffect, useMemo, useState } from 'react';
import {
  Box,
  FormControl,
  InputLabel,
  Select,
  MenuItem,
  TextField,
  Switch,
  FormControlLabel,
  Typography,
  Stack,
  Alert,
  Chip
} from '@mui/material';
import type { JoinNode } from '../../dsl/dsl.types';
import type { JoinDiagnostics } from '../../dsl/dsl.validate';

export interface JoinConfigurationPanelProps {
  node: JoinNode;
  diagnostics?: JoinDiagnostics;
  onChange: (patch: Partial<JoinNode>) => void;
  disabled?: boolean;
}

const MODES: { value: JoinNode['mode']; label: string; desc: string }[] = [
  { value: 'all', label: 'All', desc: 'Wait for every incoming branch' },
  { value: 'any', label: 'Any', desc: 'Continue when the first branch arrives' },
  { value: 'count', label: 'Count', desc: 'Wait for a specific number of branches' },
  { value: 'quorum', label: 'Quorum %', desc: 'Wait for percentage of branches' },
  { value: 'expression', label: 'Expression', desc: 'Custom JsonLogic threshold' }
];

export const JoinConfigurationPanel: React.FC<JoinConfigurationPanelProps> = ({
  node,
  diagnostics,
  onChange,
  disabled
}) => {
  const mode = node.mode || 'all';
  const [expressionDraft, setExpressionDraft] = useState(node.expression || '');
  const [exprError, setExprError] = useState<string | null>(null);

  useEffect(() => {
    if (mode !== 'expression') {
      setExprError(null);
      return;
    }
    if (!expressionDraft.trim()) {
      setExprError('Expression required');
      return;
    }
    try {
      JSON.parse(expressionDraft);
      setExprError(null);
    } catch (e: any) {
      setExprError(e?.message || 'Invalid JSON');
    }
  }, [mode, expressionDraft]);

  const modeMeta = useMemo(() => MODES.find(m => m.value === mode), [mode]);

  return (
    <Box>
      <FormControl fullWidth size="small" margin="dense" disabled={disabled}>
        <InputLabel>Join Mode</InputLabel>
        <Select
          label="Join Mode"
          value={mode}
          onChange={(e) => {
            const val = e.target.value as JoinNode['mode'];
            const patch: Partial<JoinNode> = { mode: val };
            if (val !== 'expression') patch.expression = undefined;
            if (val !== 'count') patch.thresholdCount = undefined;
            if (val !== 'quorum') patch.thresholdPercent = undefined;
            onChange(patch);
          }}
        >
          {MODES.map(m => (
            <MenuItem key={m.value} value={m.value}>{m.label}</MenuItem>
          ))}
        </Select>
      </FormControl>
      {modeMeta && (
        <Typography variant="caption" color="text.secondary">
          {modeMeta.desc}
        </Typography>
      )}

      {mode === 'count' && (
        <TextField
          label="Threshold Count"
          type="number"
          size="small"
          margin="dense"
          fullWidth
          value={(node.thresholdCount ?? '')}
          onChange={(e) => onChange({ thresholdCount: e.target.value === '' ? undefined : Number(e.target.value) })}
          inputProps={{ min: 1 }}
          disabled={disabled}
        />
      )}

      {mode === 'quorum' && (
        <TextField
          label="Threshold Percent"
          type="number"
          size="small"
          margin="dense"
          fullWidth
          value={(node.thresholdPercent ?? '')}
          onChange={(e) => onChange({ thresholdPercent: e.target.value === '' ? undefined : Number(e.target.value) })}
          inputProps={{ min: 1, max: 100 }}
          helperText="1–100"
          disabled={disabled}
        />
      )}

      {mode === 'expression' && (
        <Box mt={1}>
          <TextField
            multiline
            minRows={4}
            maxRows={12}
            fullWidth
            size="small"
            label="JsonLogic Expression"
            value={expressionDraft}
            onChange={(e) => {
              setExpressionDraft(e.target.value);
              onChange({ expression: e.target.value });
            }}
            error={!!exprError}
            helperText={exprError || 'Provide a valid JsonLogic object'}
            InputProps={{ sx: { fontFamily: 'monospace', fontSize: '0.75rem' } }}
            disabled={disabled}
          />
        </Box>
      )}

      <FormControlLabel
        sx={{ mt: 1 }}
        control={
          <Switch
            size="small"
            checked={!!node.cancelRemaining}
            disabled={disabled}
            onChange={(e) => onChange({ cancelRemaining: e.target.checked })}
          />
        }
        label={
          <Typography variant="caption">
            Cancel remaining branches when satisfied
          </Typography>
        }
      />

      {diagnostics && (
        <Box mt={1}>
          <Stack direction="row" spacing={1} flexWrap="wrap">
            <Chip
              size="small"
              label={`${diagnostics.incomingCount} incoming`}
              color={diagnostics.incomingCount < 2 ? 'error' : 'default'}
              variant={diagnostics.incomingCount < 2 ? 'filled' : 'outlined'}
            />
            {diagnostics.satisfiedThresholdDescription && (
              <Chip size="small" variant="outlined" label={diagnostics.satisfiedThresholdDescription} />
            )}
            {diagnostics.issues.length > 0 && (
              <Chip size="small" color="error" variant="filled" label={`${diagnostics.issues.length} issue(s)`} />
            )}
            {diagnostics.warnings.length > 0 && (
              <Chip size="small" color="warning" variant="outlined" label={`${diagnostics.warnings.length} warn`} />
            )}
          </Stack>
          {(diagnostics.issues.length > 0 || diagnostics.warnings.length > 0) && (
            <Alert
              severity={diagnostics.issues.length > 0 ? 'error' : 'warning'}
              sx={{ mt: 1, p: 1 }}
            >
              <Typography variant="caption" component="div" sx={{ whiteSpace: 'pre-line' }}>
                { [
                  ...diagnostics.issues.map(i => `• ${i}`),
                  ...diagnostics.warnings.map(w => `• ${w}`)
                ].join('\n') }
              </Typography>
            </Alert>
          )}
        </Box>
      )}
    </Box>
  );
};

export default JoinConfigurationPanel;
