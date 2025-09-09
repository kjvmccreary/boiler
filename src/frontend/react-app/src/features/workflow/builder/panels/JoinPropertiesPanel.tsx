import { useCallback } from 'react';
import {
  Box,
  Typography,
  FormControl,
  InputLabel,
  Select,
  MenuItem,
  SelectChangeEvent,
  TextField,
  Switch,
  FormControlLabel,
  Alert
} from '@mui/material';
import { JoinMode } from '../../dsl/dsl.types';
import { HybridExpressionEditor } from '../components/HybridExpressionEditor';
import { useExpressionSettings } from '../context/ExpressionSettingsContext';

export interface JoinPropertiesPanelProps {
  nodeId: string;
  mode?: JoinMode;
  thresholdCount?: number;
  thresholdPercent?: number;
  expression?: string;
  cancelRemaining?: boolean;
  onChange: (patch: Record<string, any>) => void;
  useMonaco?: boolean;
}

const modeOptions: { value: JoinMode; label: string; desc: string }[] = [
  { value: 'all', label: 'All', desc: 'Wait for all incoming branches' },
  { value: 'any', label: 'Any', desc: 'Proceed when first branch arrives' },
  { value: 'count', label: 'Count', desc: 'Proceed after a fixed number arrive' },
  { value: 'quorum', label: 'Quorum', desc: 'Proceed when percentage arrive' },
  { value: 'expression', label: 'Expression', desc: 'Custom JsonLogic expression' }
];

export function JoinPropertiesPanel({
  nodeId,
  mode = 'all',
  thresholdCount,
  thresholdPercent,
  expression,
  cancelRemaining = false,
  onChange,
  useMonaco = false
}: JoinPropertiesPanelProps) {
  const { semanticEnabled } = useExpressionSettings();

  const handleMode = useCallback((e: SelectChangeEvent<string>) => {
    const next = e.target.value as JoinMode;
    const patch: Record<string, any> = { mode: next };
    if (next !== 'count') patch.thresholdCount = undefined;
    if (next !== 'quorum') patch.thresholdPercent = undefined;
    if (next !== 'expression') patch.expression = undefined;
    onChange(patch);
  }, [onChange]);

  const showCount = mode === 'count';
  const showQuorum = mode === 'quorum';
  const showExpression = mode === 'expression';

  return (
    <Box sx={{ display: 'flex', flexDirection: 'column', gap: 2 }}>
      <Typography variant="subtitle1" fontWeight={600}>Join</Typography>

      <FormControl size="small" fullWidth>
        <InputLabel id={`join-${nodeId}-mode-label`}>Mode</InputLabel>
        <Select
          labelId={`join-${nodeId}-mode-label`}
          label="Mode"
          value={mode}
          onChange={handleMode}
        >
          {modeOptions.map(opt => (
            <MenuItem key={opt.value} value={opt.value}>
              <Box sx={{ display: 'flex', flexDirection: 'column' }}>
                <Typography variant="body2" fontWeight={500}>{opt.label}</Typography>
                <Typography variant="caption" color="text.secondary">{opt.desc}</Typography>
              </Box>
            </MenuItem>
          ))}
        </Select>
      </FormControl>

      {showCount && (
        <TextField
          size="small"
          label="Threshold Count"
          type="number"
          value={thresholdCount ?? ''}
          onChange={e => onChange({ thresholdCount: e.target.value ? Number(e.target.value) : undefined })}
          placeholder="e.g. 2"
          helperText="Number of branches required"
        />
      )}

      {showQuorum && (
        <TextField
          size="small"
          label="Threshold Percent"
          type="number"
          value={thresholdPercent ?? ''}
          onChange={e => onChange({ thresholdPercent: e.target.value ? Number(e.target.value) : undefined })}
          placeholder="e.g. 60"
          helperText="Percentage (1-100)"
        />
      )}

      {showExpression && (
        <HybridExpressionEditor
          kind="join"
          value={expression ?? ''}
          onChange={val => onChange({ expression: val })}
          useMonaco={useMonaco}
          semantic={semanticEnabled}
          variableContext={[
            'branch.arrivals',
            'branch.totalExpected',
            'instance.id',
            'instance.status',
            'user.id'
          ]}
        />
      )}

      <FormControlLabel
        control={
          <Switch
            size="small"
            checked={!!cancelRemaining}
            onChange={(_, v) => onChange({ cancelRemaining: v })}
          />
        }
        label="Cancel remaining branches when satisfied"
      />

      <Alert severity="info" variant="outlined">
        Expression mode executes a JsonLogic expression referencing branch arrival context (future docs).
      </Alert>
    </Box>
  );
}

export default JoinPropertiesPanel;
