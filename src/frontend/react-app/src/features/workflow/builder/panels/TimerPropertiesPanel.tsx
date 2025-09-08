import { useCallback, useEffect, useState } from 'react';
import {
  Box,
  Typography,
  RadioGroup,
  FormControlLabel,
  Radio,
  TextField,
  Stack,
  Alert
} from '@mui/material';

export interface TimerPropertiesPanelProps {
  nodeId: string;
  delayMinutes?: number;
  delaySeconds?: number;
  untilIso?: string;
  onChange: (patch: Record<string, any>) => void;
}

type Mode = 'relative' | 'absolute';

export function TimerPropertiesPanel({
  nodeId,
  delayMinutes,
  delaySeconds,
  untilIso,
  onChange
}: TimerPropertiesPanelProps) {

  const initialMode: Mode = untilIso ? 'absolute' : 'relative';
  const [mode, setMode] = useState<Mode>(initialMode);
  const [minutes, setMinutes] = useState<string>(delayMinutes != null ? String(delayMinutes) : '');
  const [seconds, setSeconds] = useState<string>(delaySeconds != null ? String(delaySeconds) : '');
  const [absIso, setAbsIso] = useState<string>(untilIso ?? '');

  // Keep mode consistent if user clears absolute
  useEffect(() => {
    if (mode === 'absolute' && !absIso) {
      // no auto-switch; user decides
    }
  }, [mode, absIso]);

  const applyRelative = useCallback((m: string, s: string) => {
    const patch: Record<string, any> = {};
    patch.delayMinutes = m ? Number(m) : undefined;
    patch.delaySeconds = s ? Number(s) : undefined;
    patch.untilIso = undefined;
    onChange(patch);
  }, [onChange]);

  const applyAbsolute = useCallback((iso: string) => {
    onChange({
      untilIso: iso || undefined,
      delayMinutes: undefined,
      delaySeconds: undefined
    });
  }, [onChange]);

  const handleModeChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const next = e.target.value as Mode;
    setMode(next);
    if (next === 'relative') {
      applyRelative(minutes, seconds);
    } else {
      applyAbsolute(absIso);
    }
  };

  const relativeValid = (): boolean => {
    if (mode !== 'relative') return true;
    const m = minutes ? Number(minutes) : 0;
    const s = seconds ? Number(seconds) : 0;
    return m > 0 || s > 0;
  };

  const absoluteValid = (): boolean => {
    if (mode !== 'absolute') return true;
    if (!absIso) return false;
    const d = new Date(absIso);
    return !isNaN(d.getTime()) && d.getTime() > Date.now();
  };

  const showRelativeWarning = mode === 'relative' && !relativeValid();
  const showAbsoluteWarning = mode === 'absolute' && !absoluteValid();

  return (
    <Box sx={{ display: 'flex', flexDirection: 'column', gap: 2 }}>
      <Typography variant="subtitle1" fontWeight={600}>Timer</Typography>

      <RadioGroup
        row
        value={mode}
        onChange={handleModeChange}
        name={`timer-mode-${nodeId}`}
      >
        <FormControlLabel value="relative" control={<Radio size="small" />} label="Relative Delay" />
        <FormControlLabel value="absolute" control={<Radio size="small" />} label="Absolute Time" />
      </RadioGroup>

      {mode === 'relative' && (
        <Stack direction="row" spacing={2}>
          <TextField
            size="small"
            label="Minutes"
            type="number"
            value={minutes}
            onChange={e => {
              const v = e.target.value;
              setMinutes(v);
              applyRelative(v, seconds);
            }}
            inputProps={{ min: 0 }}
            sx={{ width: 120 }}
          />
          <TextField
            size="small"
            label="Seconds"
            type="number"
            value={seconds}
            onChange={e => {
              const v = e.target.value;
              setSeconds(v);
              applyRelative(minutes, v);
            }}
            inputProps={{ min: 0 }}
            sx={{ width: 120 }}
          />
        </Stack>
      )}

      {mode === 'absolute' && (
        <TextField
          size="small"
          label="Execute At (UTC)"
          type="datetime-local"
          value={absIso}
          onChange={e => {
            const raw = e.target.value;
            setAbsIso(raw);
            applyAbsolute(raw ? new Date(raw).toISOString() : '');
          }}
          helperText="Select future time. Stored as ISO UTC."
          InputLabelProps={{ shrink: true }}
        />
      )}

      {showRelativeWarning && (
        <Alert severity="warning" variant="outlined">
          Provide minutes or seconds &gt; 0.
        </Alert>
      )}
      {showAbsoluteWarning && (
        <Alert severity="warning" variant="outlined">
            Provide a future timestamp.
        </Alert>
      )}

      <Typography variant="caption" color="text.secondary">
        {mode === 'relative'
          ? 'Relative delay will convert to schedule after publication.'
          : 'Absolute time triggers once. Worker polls DB timers.'}
      </Typography>
    </Box>
  );
}

export default TimerPropertiesPanel;
