import React from 'react';
import { Switch, FormControlLabel, Chip, Tooltip, Stack } from '@mui/material';

interface Props {
  enabled: boolean;
  onChange: (v: boolean) => void;
  mismatchCount: number;
  durationMs?: number;
}

export const StrictModeToggle: React.FC<Props> = ({
  enabled,
  onChange,
  mismatchCount,
  durationMs
}) => (
  <Stack direction="row" spacing={1} alignItems="center">
    <FormControlLabel
      control={
        <Switch
          size="small"
          checked={enabled}
          onChange={(e) => onChange(e.target.checked)}
        />
      }
      label="Strict Structural"
    />
    <Tooltip
      title={
        mismatchCount
          ? 'Gateways with strict vs heuristic differences'
          : 'No differences between strict and heuristic'
      }
    >
      <Chip
        size="small"
        label={`${mismatchCount} diff`}
        color={mismatchCount ? 'warning' : 'success'}
        variant={mismatchCount ? 'filled' : 'outlined'}
      />
    </Tooltip>
    {enabled && durationMs != null && (
      <Tooltip title="Last strict analysis duration">
        <Chip
          size="small"
          variant="outlined"
          label={`${durationMs.toFixed(1)}ms`}
        />
      </Tooltip>
    )}
  </Stack>
);

export default StrictModeToggle;
