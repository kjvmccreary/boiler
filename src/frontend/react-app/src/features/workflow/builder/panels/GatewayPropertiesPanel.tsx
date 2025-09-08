import { useCallback } from 'react';
import {
  Box,
  FormControl,
  InputLabel,
  MenuItem,
  Select,
  SelectChangeEvent,
  TextField,
  Typography,
  Alert
} from '@mui/material';
import { GatewayStrategy } from '../../dsl/dsl.types';

export interface GatewayPropertiesPanelProps {
  nodeId: string;
  label?: string;
  strategy?: GatewayStrategy;
  condition?: string;
  onChange: (patch: Record<string, any>) => void;
}

const strategyOptions: { value: GatewayStrategy; label: string; desc: string }[] = [
  { value: 'exclusive', label: 'Exclusive', desc: 'Choose exactly one branch (default)' },
  { value: 'conditional', label: 'Conditional', desc: 'Evaluate expression to select true/false edges' },
  { value: 'parallel', label: 'Parallel', desc: 'Fan out to all outgoing branches (C2 will enable visual enhancements)' }
];

export function GatewayPropertiesPanel({
  nodeId,
  strategy = 'exclusive',
  condition,
  onChange
}: GatewayPropertiesPanelProps) {

  const handleStrategyChange = useCallback((e: SelectChangeEvent<string>) => {
    const next = e.target.value as GatewayStrategy;
    const patch: Record<string, any> = { strategy: next };
    // Clear condition if leaving conditional
    if (strategy === 'conditional' && next !== 'conditional') {
      patch.condition = undefined;
    }
    // Provide starter JsonLogic if entering conditional with no condition
    if (next === 'conditional' && !condition) {
      patch.condition = '{"==":[{"var":"approved"},true]}';
    }
    onChange(patch);
  }, [strategy, condition, onChange]);

  const handleConditionChange = useCallback((e: React.ChangeEvent<HTMLInputElement>) => {
    onChange({ condition: e.target.value });
  }, [onChange]);

  return (
    <Box sx={{ display: 'flex', flexDirection: 'column', gap: 2 }}>
      <Typography variant="subtitle1" fontWeight={600}>
        Gateway
      </Typography>

      <FormControl size="small" fullWidth>
        <InputLabel id={`gw-${nodeId}-strategy-label`}>Strategy</InputLabel>
        <Select
          labelId={`gw-${nodeId}-strategy-label`}
            label="Strategy"
            value={strategy}
            onChange={handleStrategyChange}
        >
          {strategyOptions.map(opt => (
            <MenuItem key={opt.value} value={opt.value}>
              <Box sx={{ display: 'flex', flexDirection: 'column' }}>
                <Typography variant="body2" fontWeight={500}>{opt.label}</Typography>
                <Typography variant="caption" color="text.secondary">{opt.desc}</Typography>
              </Box>
            </MenuItem>
          ))}
        </Select>
      </FormControl>

      {strategy === 'conditional' && (
        <TextField
          label="JsonLogic Condition"
          size="small"
          multiline
          minRows={3}
          value={condition ?? ''}
          onChange={handleConditionChange}
          placeholder='{"==":[{"var":"approved"},true]}'
          helperText="JsonLogic expression. H1 story will add validation."
        />
      )}

      {strategy === 'parallel' && (
        <Alert severity="info" variant="outlined">
          Parallel fan-out active. All outgoing edges will execute. C2 will add visualization & join warnings.
        </Alert>
      )}
    </Box>
  );
}

export default GatewayPropertiesPanel;
