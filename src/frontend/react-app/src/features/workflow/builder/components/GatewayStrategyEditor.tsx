import React, { useCallback, useEffect, useMemo, useState } from 'react';
import {
  Box,
  FormControl,
  InputLabel,
  Select,
  MenuItem,
  TextField,
  Typography,
  Stack,
  Chip,
  Tooltip,
  IconButton,
  Alert
} from '@mui/material';
import HelpOutlineIcon from '@mui/icons-material/HelpOutline';
import AutoFixHighIcon from '@mui/icons-material/AutoFixHigh';
import type { GatewayNode } from '../../dsl/dsl.types';
import { trackWorkflow } from '../../telemetry/workflowTelemetry';

export interface GatewayStrategyEditorProps {
  node: GatewayNode;
  onChange: (partial: Partial<GatewayNode>) => void;
  disabled?: boolean;
}

const CONDITIONAL_SNIPPETS: { label: string; json: string }[] = [
  { label: 'Equals', json: '{"==":[{"var":"field"}, 10]}' },
  { label: 'Not Equals', json: '{"!=":[{"var":"field"}, 0]}' },
  { label: 'Greater Than', json: '{">":[{"var":"amount"}, 100]}' },
  { label: 'In Set', json: '{"in":[{"var":"status"}, ["Open","Pending"]]}' }
];

export const GatewayStrategyEditor: React.FC<GatewayStrategyEditorProps> = ({
  node,
  onChange,
  disabled
}) => {
  const inferred = useMemo(() => node.strategy || (node.condition ? 'conditional' : 'exclusive'),
    [node.strategy, node.condition]);
  const [conditionDraft, setConditionDraft] = useState(node.condition || '');
  const [parseError, setParseError] = useState<string | null>(null);

  useEffect(() => { if (inferred !== 'conditional') setParseError(null); }, [inferred]);

  useEffect(() => {
    if (inferred !== 'conditional') return;
    if (!conditionDraft.trim()) { setParseError('Condition is required'); return; }
    try { JSON.parse(conditionDraft); setParseError(null); }
    catch (e: any) { setParseError(e?.message || 'Invalid JSON'); }
  }, [inferred, conditionDraft]);

  const handleStrategyChange = (val: string) => {
    const prev = inferred;
    if (val === 'conditional' && !node.condition) {
      const defaultCond = '{"==":[{"var":"field"}, true]}';
      onChange({ strategy: 'conditional', condition: defaultCond });
      setConditionDraft(defaultCond);
      trackWorkflow('gateway.strategy.changed', { nodeId: node.id, from: prev, to: 'conditional' });
      return;
    }
    if (val !== 'conditional') {
      onChange({ strategy: val as any, condition: undefined });
      trackWorkflow('gateway.strategy.changed', { nodeId: node.id, from: prev, to: val });
    } else {
      onChange({ strategy: 'conditional' });
      trackWorkflow('gateway.strategy.changed', { nodeId: node.id, from: prev, to: 'conditional' });
    }
  };

  const applySnippet = (json: string) => {
    setConditionDraft(json);
    onChange({ condition: json });
  };

  const autoBeautify = useCallback(() => {
    try {
      const parsed = JSON.parse(conditionDraft);
      const pretty = JSON.stringify(parsed, null, 2);
      setConditionDraft(pretty);
      onChange({ condition: pretty });
    } catch { /* ignore */ }
  }, [conditionDraft, onChange]);

  return (
    <Box>
      <FormControl fullWidth size="small" margin="dense" disabled={disabled}>
        <InputLabel>Strategy</InputLabel>
        <Select
          label="Strategy"
          value={inferred}
          onChange={(e) => handleStrategyChange(e.target.value)}
        >
          <MenuItem value="exclusive">Exclusive</MenuItem>
          <MenuItem value="conditional">Conditional</MenuItem>
          <MenuItem value="parallel">Parallel</MenuItem>
        </Select>
      </FormControl>

      {inferred === 'exclusive' && (
        <Typography variant="caption" color="text.secondary">
          Exclusive: first matching branch proceeds; last branch acts as default if unmatched.
        </Typography>
      )}

      {inferred === 'parallel' && (
        <Typography variant="caption" color="text.secondary">
          Parallel: all outgoing branches activate concurrently, ensure downstream join(s).
        </Typography>
      )}

      {inferred === 'conditional' && (
        <Box sx={{ mt: 1 }}>
          <Stack direction="row" alignItems="center" spacing={1}>
            <Typography variant="subtitle2">Condition (JsonLogic)</Typography>
            <Tooltip title="JsonLogic quick reference">
              <HelpOutlineIcon fontSize="small" color="action" />
            </Tooltip>
            <Tooltip title="Beautify">
              <IconButton size="small" onClick={autoBeautify}>
                <AutoFixHighIcon fontSize="small" />
              </IconButton>
            </Tooltip>
          </Stack>
          <TextField
            multiline
            minRows={4}
            maxRows={14}
            fullWidth
            size="small"
            value={conditionDraft}
            onChange={(e) => {
              setConditionDraft(e.target.value);
              onChange({ condition: e.target.value });
            }}
            error={!!parseError}
            helperText={parseError || 'Valid JsonLogic object required'}
            sx={{ fontFamily: 'monospace', mt: 1 }}
            disabled={disabled}
          />
          <Stack direction="row" spacing={1} sx={{ mt: 1, flexWrap: 'wrap' }}>
            {CONDITIONAL_SNIPPETS.map(s => (
              <Chip
                key={s.label}
                label={s.label}
                size="small"
                variant="outlined"
                onClick={() => applySnippet(s.json)}
              />
            ))}
          </Stack>
          {parseError && (
            <Alert severity="error" sx={{ mt: 1, p: 1 }}>
              {parseError}
            </Alert>
          )}
        </Box>
      )}
    </Box>
  );
};

export default GatewayStrategyEditor;
