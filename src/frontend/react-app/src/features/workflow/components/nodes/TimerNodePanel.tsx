import React, { useEffect, useState } from 'react';

export interface TimerNodePanelProps {
  properties: Record<string, any>;
  onChange: (patch: Record<string, any>) => void;
  readOnly?: boolean;
}

type Mode = 'seconds' | 'minutes';

const MAX_SECONDS = 7 * 24 * 3600;
const MAX_MINUTES = MAX_SECONDS / 60;

export const TimerNodePanel: React.FC<TimerNodePanelProps> = ({
  properties,
  onChange,
  readOnly = false
}) => {
  const initialMode: Mode =
    properties.delaySeconds != null
      ? 'seconds'
      : 'minutes';

  const [mode, setMode] = useState<Mode>(initialMode);
  const [delaySeconds, setDelaySeconds] = useState<string>(
    properties.delaySeconds != null ? String(properties.delaySeconds) : ''
  );
  const [delayMinutes, setDelayMinutes] = useState<string>(
    properties.delayMinutes != null ? String(properties.delayMinutes) : ''
  );
  const [untilIso, setUntilIso] = useState<string>(properties.untilIso || properties.dueDate || '');

  useEffect(() => {
    if (properties.delaySeconds != null && mode !== 'seconds') {
      setMode('seconds');
      setDelaySeconds(String(properties.delaySeconds));
    }
    if (properties.delayMinutes != null && mode !== 'minutes') {
      setMode('minutes');
      setDelayMinutes(String(properties.delayMinutes));
    }
    if (properties.untilIso && properties.untilIso !== untilIso) {
      setUntilIso(properties.untilIso);
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [properties]);

  const commitDelaySeconds = (raw: string) => {
    setDelaySeconds(raw);
    if (readOnly) return;
    if (raw.trim() === '') {
      onChange({ delaySeconds: undefined });
      return;
    }
    const n = Number(raw);
    if (!Number.isNaN(n)) {
      const clamped = clamp(n, 0, MAX_SECONDS);
      onChange({
        delaySeconds: clamped,
        delayMinutes: undefined,
        untilIso: undefined,
        dueDate: undefined
      });
    }
  };

  const commitDelayMinutes = (raw: string) => {
    setDelayMinutes(raw);
    if (readOnly) return;
    if (raw.trim() === '') {
      onChange({ delayMinutes: undefined });
      return;
    }
    const n = Number(raw);
    if (!Number.isNaN(n)) {
      const clamped = clamp(n, 0, MAX_MINUTES);
      onChange({
        delayMinutes: clamped,
        delaySeconds: undefined,
        untilIso: undefined,
        dueDate: undefined
      });
    }
  };

  const commitUntilIso = (localValue: string) => {
    setUntilIso(localValue);
    if (readOnly) return;
    if (!localValue) {
      onChange({ untilIso: undefined, dueDate: undefined });
      return;
    }
    const dt = new Date(localValue);
    const iso = dt.toISOString();
    onChange({
      untilIso: iso,
      delaySeconds: undefined,
      delayMinutes: undefined
    });
  };

  const switchMode = (next: Mode) => {
    if (readOnly) return;
    if (next === mode) return;
    if (next === 'seconds' && delayMinutes) {
      const converted = Number(delayMinutes) * 60;
      setDelaySeconds(String(round(converted, 3)));
      commitDelaySeconds(String(round(converted, 3)));
      setDelayMinutes('');
    } else if (next === 'minutes' && delaySeconds) {
      const converted = Number(delaySeconds) / 60;
      setDelayMinutes(String(round(converted, 4)));
      commitDelayMinutes(String(round(converted, 4)));
      setDelaySeconds('');
    }
    setMode(next);
  };

  return (
    <div style={{ fontSize: 14, lineHeight: 1.4 }}>
      <h4 style={{ margin: '4px 0 8px' }}>Timer Configuration</h4>

      <label style={labelStyle}>
        Mode
        <select
          disabled={readOnly}
          value={mode}
          onChange={e => switchMode(e.target.value as Mode)}
          style={selectStyle}
        >
          <option value="seconds">delaySeconds</option>
          <option value="minutes">delayMinutes</option>
        </select>
      </label>

      {mode === 'seconds' && (
        <label style={labelStyle}>
          Delay Seconds (float ≥ 0)
          <input
            disabled={readOnly || !!untilIso}
            type="number"
            min={0}
            step="0.1"
            value={delaySeconds}
            onChange={e => commitDelaySeconds(e.target.value)}
            placeholder="e.g. 5 or 0.5"
            style={inputStyle}
          />
        </label>
      )}

      {mode === 'minutes' && (
        <label style={labelStyle}>
          Delay Minutes (float ≥ 0)
          <input
            disabled={readOnly || !!untilIso}
            type="number"
            min={0}
            step="0.01"
            value={delayMinutes}
            onChange={e => commitDelayMinutes(e.target.value)}
            placeholder="e.g. 0.1 ≈ 6s"
            style={inputStyle}
          />
        </label>
      )}

      <div style={{ margin: '8px 0 4px', fontWeight: 500 }}>
        Absolute Time (UTC) – overrides delay
      </div>
      <input
        disabled={readOnly}
        type="datetime-local"
        value={untilIso ? toLocalInput(untilIso) : ''}
        onChange={e => commitUntilIso(e.target.value)}
        style={inputStyle}
      />

      <div style={noteStyle}>
        Priority: <code>untilIso</code> &gt; <code>delaySeconds</code> &gt; <code>delayMinutes</code>. 0 = fire ASAP (next worker scan). Values capped at 7 days.
      </div>

      {preview(properties)}
    </div>
  );
};

/* ---------- Helpers ---------- */
function clamp(n: number, min: number, max: number) {
  return Math.min(Math.max(n, min), max);
}
function round(n: number, places: number) {
  const p = Math.pow(10, places);
  return Math.round(n * p) / p;
}
function toLocalInput(iso: string): string {
  if (!iso) return '';
  const d = new Date(iso);
  const pad = (x: number) => x.toString().padStart(2, '0');
  return `${d.getFullYear()}-${pad(d.getMonth() + 1)}-${pad(d.getDate())}T${pad(d.getHours())}:${pad(d.getMinutes())}`;
}
function preview(props: Record<string, any>) {
  const untilIso = props.untilIso;
  const ds = props.delaySeconds;
  const dm = props.delayMinutes;
  let label = 'Effective: ';
  if (untilIso) label += `untilIso ${untilIso}`;
  else if (ds != null) label += `delaySeconds ${ds}`;
  else if (dm != null) label += `delayMinutes ${dm}`;
  else label += 'default 1 minute';
  return <div style={previewStyle}>{label}</div>;
}

/* ---------- Styles ---------- */
const labelStyle: React.CSSProperties = { display: 'block', marginTop: 8, fontWeight: 500 };
const inputStyle: React.CSSProperties = { width: '100%', padding: '4px 6px', marginTop: 4, boxSizing: 'border-box' };
const selectStyle: React.CSSProperties = { marginLeft: 8 };
const noteStyle: React.CSSProperties = { marginTop: 10, fontSize: 12, opacity: 0.75 };
const previewStyle: React.CSSProperties = { marginTop: 10, fontSize: 12, fontStyle: 'italic', opacity: 0.8 };

export default TimerNodePanel;
