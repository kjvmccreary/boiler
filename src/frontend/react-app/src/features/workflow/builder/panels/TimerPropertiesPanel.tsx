import { useCallback, useEffect, useMemo, useRef, useState } from 'react';
import {
  Box,
  Typography,
  RadioGroup,
  FormControlLabel,
  Radio,
  TextField,
  Stack,
  Alert,
  Chip,
  Divider,
  Tooltip,
  IconButton
} from '@mui/material';
import RefreshIcon from '@mui/icons-material/Refresh';

export interface TimerPropertiesPanelProps {
  nodeId: string;
  delayMinutes?: number;
  delaySeconds?: number;
  untilIso?: string;
  onChange: (patch: Record<string, any>) => void;
}

type Mode = 'relative' | 'absolute';

interface ValidationState {
  errors: string[];
  warnings: string[];
}

const MAX_TOTAL_SECONDS = 60 * 60 * 24 * 30; // 30 days cap for relative (sanity guard)
const REL_PRESETS: { label: string; minutes?: number; seconds?: number }[] = [
  { label: '30s', seconds: 30 },
  { label: '5m', minutes: 5 },
  { label: '15m', minutes: 15 },
  { label: '1h', minutes: 60 }
];

function formatDuration(totalSeconds: number): string {
  if (totalSeconds < 60) return `${totalSeconds}s`;
  const m = Math.floor(totalSeconds / 60);
  const s = totalSeconds % 60;
  if (s === 0) return `${m}m`;
  return `${m}m ${s}s`;
}

function useInterval(cb: () => void, ms: number | null) {
  const ref = useRef(cb);
  ref.current = cb;
  useEffect(() => {
    if (ms == null) return;
    const id = setInterval(() => ref.current(), ms);
    return () => clearInterval(id);
  }, [ms]);
}

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
  const [nowTick, setNowTick] = useState<number>(Date.now());

  // Live countdown tick every 1s for absolute preview
  useInterval(() => {
    if (mode === 'absolute') setNowTick(Date.now());
  }, mode === 'absolute' ? 1000 : null);

  // Normalize patch helpers
  const applyRelative = useCallback((mStr: string, sStr: string) => {
    const patch: Record<string, any> = {};
    const m = mStr.trim() === '' ? undefined : Number(mStr);
    const s = sStr.trim() === '' ? undefined : Number(sStr);
    patch.delayMinutes = isNaN(m as any) ? undefined : m;
    patch.delaySeconds = isNaN(s as any) ? undefined : s;
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

  // Validation
  const validation: ValidationState = useMemo(() => {
    const v: ValidationState = { errors: [], warnings: [] };
    if (mode === 'relative') {
      const m = minutes ? Number(minutes) : 0;
      const s = seconds ? Number(seconds) : 0;
      if ((minutes && isNaN(m)) || (seconds && isNaN(s))) {
        v.errors.push('Invalid number input.');
        return v;
      }
      if (m < 0 || s < 0) {
        v.errors.push('Delay values must be >= 0.');
      }
      if (m === 0 && s === 0) {
        v.warnings.push('Provide minutes or seconds greater than zero.');
      }
      const total = m * 60 + s;
      if (total > MAX_TOTAL_SECONDS) {
        v.errors.push('Relative delay exceeds maximum (30 days).');
      } else if (total > 0 && total >= 60 * 60 * 24 * 7) {
        v.warnings.push('Long delay > 7 days. Confirm this is intentional.');
      }
    } else {
      if (!absIso) {
        v.warnings.push('Choose a future timestamp.');
      } else {
        const dt = new Date(absIso);
        if (isNaN(dt.getTime())) {
          v.errors.push('Invalid datetime value.');
        } else if (dt.getTime() <= Date.now()) {
          v.errors.push('Timestamp must be in the future.');
        } else if (dt.getTime() - Date.now() > MAX_TOTAL_SECONDS * 1000) {
          v.warnings.push('Absolute time is more than 30 days in the future.');
        }
      }
    }
    return v;
  }, [mode, minutes, seconds, absIso]);

  const totalRelativeSeconds = useMemo(() => {
    if (mode !== 'relative') return 0;
    const m = minutes ? Number(minutes) : 0;
    const s = seconds ? Number(seconds) : 0;
    if (isNaN(m) || isNaN(s)) return 0;
    return m * 60 + s;
  }, [mode, minutes, seconds]);

  const relativePreview = useMemo(() => {
    if (mode !== 'relative') return '';
    if (totalRelativeSeconds <= 0) return '';
    return `≈ ${formatDuration(totalRelativeSeconds)}`;
  }, [mode, totalRelativeSeconds]);

  const absolutePreview = useMemo(() => {
    if (mode !== 'absolute' || !absIso) return '';
    const dt = new Date(absIso);
    if (isNaN(dt.getTime())) return '';
    const diffMs = dt.getTime() - nowTick;
    if (diffMs <= 0) return 'due now / past';
    const diffSec = Math.floor(diffMs / 1000);
    return `in ${formatDuration(diffSec)}`;
  }, [mode, absIso, nowTick]);

  const anyErrors = validation.errors.length > 0;
  const anyWarnings = validation.warnings.length > 0;

  const applyPreset = (preset: { minutes?: number; seconds?: number }) => {
    const m = preset.minutes ?? 0;
    const s = preset.seconds ?? 0;
    setMinutes(m ? String(m) : '');
    setSeconds(s ? String(s) : '');
    applyRelative(m ? String(m) : '', s ? String(s) : '');
  };

  const resetRelative = () => {
    setMinutes('');
    setSeconds('');
    applyRelative('', '');
  };

  const resetAbsolute = () => {
    setAbsIso('');
    applyAbsolute('');
  };

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
        <Stack direction="column" spacing={1}>
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
              error={anyErrors}
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
              error={anyErrors}
            />
            <Tooltip title="Reset relative values">
              <IconButton
                size="small"
                onClick={resetRelative}
                aria-label="Reset relative delay"
                sx={{ alignSelf: 'center' }}
              >
                <RefreshIcon fontSize="inherit" />
              </IconButton>
            </Tooltip>
          </Stack>
          <Stack direction="row" spacing={1} flexWrap="wrap">
            {REL_PRESETS.map(p => (
              <Chip
                key={p.label}
                size="small"
                label={p.label}
                onClick={() => applyPreset(p)}
                variant="outlined"
              />
            ))}
            {relativePreview && (
              <Chip
                size="small"
                color="primary"
                variant="filled"
                label={relativePreview}
              />
            )}
          </Stack>
        </Stack>
      )}

      {mode === 'absolute' && (
        <Stack spacing={1}>
          <Stack direction="row" spacing={1} alignItems="center">
            <TextField
              size="small"
              label="Execute At (UTC)"
              type="datetime-local"
              value={absIso ? absIso.slice(0, 16) : ''}
              onChange={e => {
                // Browser returns local time (no Z). We interpret as local and convert to ISO.
                const raw = e.target.value;
                if (!raw) {
                  setAbsIso('');
                  applyAbsolute('');
                  return;
                }
                // new Date(raw) treats raw as local time.
                const dt = new Date(raw);
                const iso = dt.toISOString();
                setAbsIso(iso);
                applyAbsolute(iso);
              }}
              helperText="Select a future time (stored UTC)."
              InputLabelProps={{ shrink: true }}
              error={anyErrors}
              sx={{ width: 230 }}
            />
            <Tooltip title="Reset absolute timestamp">
              <IconButton
                size="small"
                onClick={resetAbsolute}
                aria-label="Reset absolute time"
              >
                <RefreshIcon fontSize="inherit" />
              </IconButton>
            </Tooltip>
          </Stack>
          {absolutePreview && (
            <Chip
              size="small"
              variant="outlined"
              color="primary"
              label={absolutePreview}
              sx={{ maxWidth: 'fit-content' }}
            />
          )}
        </Stack>
      )}

      {(anyErrors || anyWarnings) && <Divider />}

      {validation.errors.map((e, i) => (
        <Alert key={`err-${i}`} severity="error" variant="outlined">
          {e}
        </Alert>
      ))}

      {validation.warnings.map((w, i) => (
        <Alert key={`warn-${i}`} severity="warning" variant="outlined">
          {w}
        </Alert>
      ))}

      <Typography variant="caption" color="text.secondary">
        {mode === 'relative'
          ? 'Relative delay converts to a scheduled due time at publish. Large values may be subject to operational limits.'
          : 'Absolute time triggers once when the timer worker detects it is due.'}
      </Typography>
    </Box>
  );
}

export default TimerPropertiesPanel;
