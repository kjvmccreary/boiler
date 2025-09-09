import { useCallback } from 'react';
import {
  Box,
  FormControl,
  InputLabel,
  MenuItem,
  Select,
  SelectChangeEvent,
  Typography,
  Alert,
  Chip,
  Stack
} from '@mui/material';
import { GatewayStrategy } from '../../dsl/dsl.types';
import { HybridExpressionEditor } from '../components/HybridExpressionEditor';
import { useExpressionSettings } from '../context/ExpressionSettingsContext';

export interface GatewayPropertiesPanelProps {
  nodeId: string;
  label?: string;
  strategy?: GatewayStrategy;
  condition?: string;
  onChange: (patch: Record<string, any>) => void;
  parallelBranchCount?: number;
  hasJoinCandidate?: boolean;
  parallelWarnings?: string[];
  useMonaco?: boolean;
}

const strategyOptions: { value: GatewayStrategy; label: string; desc: string }[] = [
  { value: 'exclusive', label: 'Exclusive', desc: 'Choose exactly one branch (default)' },
  { value: 'conditional', label: 'Conditional', desc: 'Evaluate expression to select true/false edges' },
  { value: 'parallel', label: 'Parallel', desc: 'Fan out to all outgoing branches' }
];

export function GatewayPropertiesPanel({
  nodeId,
  strategy = 'exclusive',
  condition,
  onChange,
  parallelBranchCount,
  hasJoinCandidate,
  parallelWarnings,
  useMonaco = false
}: GatewayPropertiesPanelProps) {
  const { semanticEnabled } = useExpressionSettings();

  const handleStrategyChange = useCallback((e: SelectChangeEvent<string>) => {
    const next = e.target.value as GatewayStrategy;
    const patch: Record<string, any> = { strategy: next };
    if (strategy === 'conditional' && next !== 'conditional') {
      patch.condition = undefined;
    }
    if (next === 'conditional' && !condition) {
      patch.condition = '{"==":[{"var":"approved"},true]}';
    }
    onChange(patch);
  }, [strategy, condition, onChange]);

  const showParallelPanel = strategy === 'parallel';

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
        <HybridExpressionEditor
          kind="gateway"
          value={condition ?? ''}
          onChange={val => onChange({ condition: val })}
          useMonaco={useMonaco}
          variableContext={[
            'instance.id',
            'instance.status',
            'user.id',
            'user.roles',
            'input.payload'
          ]}
          semantic={semanticEnabled}
        />
      )}

      {showParallelPanel && (
        <Box sx={{ display: 'flex', flexDirection: 'column', gap: 1 }}>
          <Stack direction="row" spacing={1} alignItems="center">
            <Chip
              size="small"
              color="primary"
              variant="outlined"
              label={`Branches: ${parallelBranchCount ?? 0}`}
            />
            <Chip
              size="small"
              color={hasJoinCandidate ? 'success' : 'warning'}
              variant="outlined"
              label={hasJoinCandidate ? 'Join Found' : 'No Join Node'}
            />
          </Stack>
          <Alert severity="info" variant="outlined">
            Parallel: all outgoing edges execute. A downstream Join node will synchronize branches.
          </Alert>
          {(parallelWarnings || []).map((w, i) => (
            <Alert key={i} severity="warning" variant="standard">{w}</Alert>
          ))}
        </Box>
      )}
    </Box>
  );
}

export default GatewayPropertiesPanel;
