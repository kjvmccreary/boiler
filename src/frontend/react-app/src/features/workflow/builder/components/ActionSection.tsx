import React, { useCallback, useEffect, useMemo, useState } from 'react';
import {
  Box,
  Typography,
  RadioGroup,
  FormControlLabel,
  Radio,
  TextField,
  Stack,
  IconButton,
  Button,
  Divider,
  Chip,
  Alert,
  Collapse,
  Tooltip,
  Select,
  MenuItem,
  Accordion,
  AccordionSummary,
  AccordionDetails
} from '@mui/material';
import {
  Add as AddIcon,
  Delete as DeleteIcon,
  ExpandMore as ExpandMoreIcon,
  WarningAmber as WarningIcon,
  Http as HttpIcon,
  CloudOff as NoopIcon,
  Replay as RetryIcon
} from '@mui/icons-material';
import type {
  AutomaticNode,
  AutomaticAction,
  AutomaticActionWebhook,
  HttpMethod
} from '../../dsl/dsl.types';
import { validateAutomaticAction } from '../../dsl/automaticActionRules';
import HybridExpressionEditor from './HybridExpressionEditor';
import { trackWorkflow } from '../../telemetry/workflowTelemetry';

export interface ActionSectionProps {
  node: AutomaticNode;
  onPatch: (patch: Partial<AutomaticNode>) => void;
  readonly?: boolean;
}

const HTTP_METHODS: HttpMethod[] = ['GET', 'POST', 'PUT', 'PATCH', 'DELETE'];

interface HeaderRow {
  id: string;
  key: string;
  value: string;
}

const createEmptyWebhook = (): AutomaticActionWebhook => ({
  kind: 'webhook',
  url: '',
  method: 'POST',
  headers: {},
  bodyTemplate: '',
  retryPolicy: { maxAttempts: 1, backoffSeconds: 0 }
});

/**
 * ActionSection
 * Property panel UI for AutomaticNode.action authoring.
 * Features:
 *  - Kind switch (noop/webhook) with discard confirmation.
 *  - Webhook editor: URL, method, headers (dynamic rows), body template (Monaco), retry policy.
 *  - Live validation (validateAutomaticAction) with error/warning surfacing.
 *  - Summary chips for quick at-a-glance configuration state.
 *  - Telemetry events capturing usage & health transitions.
 */
export const ActionSection: React.FC<ActionSectionProps> = ({ node, onPatch, readonly }) => {
  const [expanded, setExpanded] = useState(true);
  const action = node.action;

  const [kind, setKind] = useState<AutomaticAction['kind']>(action?.kind || 'noop');

  // Webhook specific local state (mirrors DSL; commit after each change)
  const webhook = (kind === 'webhook'
    ? (action?.kind === 'webhook' ? action : createEmptyWebhook())
    : undefined) as AutomaticActionWebhook | undefined;

  const [url, setUrl] = useState(webhook?.url || '');
  const [method, setMethod] = useState<HttpMethod>(webhook?.method || 'POST');
  const [body, setBody] = useState(webhook?.bodyTemplate || '');
  const [rawHeaders, setRawHeaders] = useState<HeaderRow[]>(() => {
    if (webhook?.headers) {
      return Object.entries(webhook.headers).map(([k, v]) => ({
        id: crypto.randomUUID(),
        key: k,
        value: v
      }));
    }
    return [];
  });

  const [maxAttempts, setMaxAttempts] = useState<number>(webhook?.retryPolicy?.maxAttempts ?? 1);
  const [backoffSeconds, setBackoffSeconds] = useState<number>(webhook?.retryPolicy?.backoffSeconds ?? 0);

  const [confirmReset, setConfirmReset] = useState(false); // kind switch confirm when discarding webhook config
  const [showHeaders, setShowHeaders] = useState<boolean>(rawHeaders.length > 0);
  const [showAdvanced, setShowAdvanced] = useState<boolean>(!!(webhook?.retryPolicy && webhook.retryPolicy.maxAttempts > 1));

  // Derived header map (filter out empties) MUST precede refs that depend on it
  const headersObject = useMemo(() => {
    const out: Record<string, string> = {};
    rawHeaders.forEach(h => {
      if (h.key.trim()) out[h.key.trim()] = h.value;
    });
    return out;
  }, [rawHeaders]);

  // Validation (placed after headersObject so refs can read stable initial values)
  const { errors, warnings } = useMemo(() => {
    if (!node.action) return { errors: [], warnings: [] };
    return validateAutomaticAction(node as AutomaticNode);
  }, [node]);

  // Telemetry state refs (after errors/headers resolved)
  const prevHeaderCountRef = React.useRef<number>(Object.keys(headersObject).length);
  const prevBodyBucketRef = React.useRef<string>('0');
  const prevErrorStateRef = React.useRef<'none' | 'has'>(errors.length ? 'has' : 'none');
  const prevKindRef = React.useRef<string>(kind);
  const prevRetryActiveRef = React.useRef<boolean>(maxAttempts > 1);

  // Commit patch on every relevant local change
  useEffect(() => {
    if (readonly) return;
    if (kind === 'noop') {
      onPatch({ action: { kind: 'noop' } });
      return;
    }
    const retryPolicy = (maxAttempts && maxAttempts > 1)
      ? { maxAttempts, backoffSeconds: backoffSeconds < 0 ? 0 : backoffSeconds }
      : undefined;
    const cleanHeaders = Object.keys(headersObject).length ? headersObject : undefined;
    const bodyTemplate = body?.trim().length ? body : undefined;

    onPatch({
      action: {
        kind: 'webhook',
        url,
        method,
        headers: cleanHeaders,
        bodyTemplate,
        retryPolicy
      }
    });
  }, [kind, url, method, headersObject, body, maxAttempts, backoffSeconds, onPatch, readonly]);

  // Summary chips
  const summary: string[] = [];
  summary.push(kind);
  if (kind === 'webhook') {
    summary.push(method);
    if (Object.keys(headersObject).length) summary.push(`hdrs:${Object.keys(headersObject).length}`);
    if (body?.trim().length) summary.push(`body:${Math.ceil(body.length / 1024)}KB`);
    if (maxAttempts > 1) summary.push(`retry:${maxAttempts}@${backoffSeconds}s`);
  }
  if (errors.length) summary.push(`${errors.length} err`);
  if (warnings.length) summary.push(`W:${warnings.length}`);

  const changeKind = (next: AutomaticAction['kind']) => {
    if (next === kind) return;
    if (kind === 'webhook') {
      const hasData =
        url.trim() ||
        rawHeaders.some(h => h.key.trim() || h.value.trim()) ||
        (body && body.trim());
      if (hasData) {
        setConfirmReset(true);
      } else {
        setKind(next);
        trackWorkflow('automatic.action.changed', { nodeId: node.id, kind: next });
      }
    } else {
      setKind(next);
      trackWorkflow('automatic.action.changed', { nodeId: node.id, kind: next });
    }
  };

  const confirmSwitch = () => {
    setKind('noop');
    setConfirmReset(false);
    trackWorkflow('automatic.action.discarded', { nodeId: node.id, discarded: 'webhook' });
  };

  const cancelSwitch = () => setConfirmReset(false);

  const addHeaderRow = () =>
    setRawHeaders(h => [...h, { id: crypto.randomUUID(), key: '', value: '' }]);

  const updateHeaderRow = (id: string, field: 'key' | 'value', value: string) =>
    setRawHeaders(rows =>
      rows.map(r => (r.id === id ? { ...r, [field]: value } : r))
    );

  const removeHeaderRow = (id: string) =>
    setRawHeaders(rows => rows.filter(r => r.id !== id));

  const clearWebhook = () => {
    setUrl('');
    setMethod('POST');
    setBody('');
    setRawHeaders([]);
    setMaxAttempts(1);
    setBackoffSeconds(0);
  };

  const bodyLooksJson = body.trim().startsWith('{') || body.trim().startsWith('[');
  let bodyJsonError: string | null = null;
  if (bodyLooksJson && body.trim().length) {
    try { JSON.parse(body); } catch (e: any) { bodyJsonError = e?.message || 'Invalid JSON'; }
  }

  const largeBody = body.length > 10_000; // mirrors validation threshold

  // Telemetry for validation results (fire on changes)
  useEffect(() => {
    if (kind === 'webhook') {
      trackWorkflow('automatic.webhook.validated', {
        nodeId: node.id,
        errorCount: errors.length,
        warningCount: warnings.length
      });
    }
  }, [errors.length, warnings.length, kind, node.id]);

  // Telemetry: header count changes (debounced by render tick)
  useEffect(() => {
    if (kind !== 'webhook') return;
    const count = Object.keys(headersObject).length;
    if (count !== prevHeaderCountRef.current) {
      trackWorkflow('automatic.webhook.headers.changed', {
        nodeId: node.id,
        count
      });
      prevHeaderCountRef.current = count;
    }
  }, [headersObject, kind, node.id]);

  // Telemetry: body size bucket transitions & JSON parse state
  useEffect(() => {
    if (kind !== 'webhook') return;
    const len = body.length;
    const bucket =
      len === 0 ? '0' :
        len <= 1024 ? '1k' :
          len <= 5 * 1024 ? '5k' :
            len <= 10 * 1024 ? '10k' : '>10k';
    if (bucket !== prevBodyBucketRef.current) {
      trackWorkflow('automatic.webhook.body.size.bucket', {
        nodeId: node.id,
        bucket,
        length: len
      });
      prevBodyBucketRef.current = bucket;
    }
    const looksJson = bodyLooksJson;
    const jsonValid = looksJson && !bodyJsonError;
    if (looksJson) {
      trackWorkflow('automatic.webhook.body.jsonState', {
        nodeId: node.id,
        valid: jsonValid,
        length: len
      });
    }
  }, [body, bodyLooksJson, bodyJsonError, kind, node.id]);

  // Telemetry: error state transitions (from errors→none or none→errors)
  useEffect(() => {
    if (kind !== 'webhook') return;
    const state: 'none' | 'has' = errors.length ? 'has' : 'none';
    if (state !== prevErrorStateRef.current) {
      trackWorkflow('automatic.webhook.health.changed', {
        nodeId: node.id,
        hadErrors: prevErrorStateRef.current === 'has',
        hasErrors: state === 'has'
      });
      prevErrorStateRef.current = state;
    }
  }, [errors.length, kind, node.id]);

  // Telemetry: retry activation toggle (maxAttempts >1 considered active)
  useEffect(() => {
    if (kind !== 'webhook') return;
    const active = maxAttempts > 1;
    if (active !== prevRetryActiveRef.current) {
      trackWorkflow('automatic.retry.state.changed', {
        nodeId: node.id,
        active,
        maxAttempts,
        backoffSeconds
      });
      prevRetryActiveRef.current = active;
    }
  }, [maxAttempts, backoffSeconds, kind, node.id]);

  // Telemetry: kind change (guard duplicate initial)
  useEffect(() => {
    if (prevKindRef.current !== kind) {
      trackWorkflow('automatic.action.kind.changed', { nodeId: node.id, from: prevKindRef.current, to: kind });
      prevKindRef.current = kind;
    }
  }, [kind, node.id]);

  return (
    <Box>
      <Box display="flex" justifyContent="space-between" alignItems="center" mb={1}>
        <Typography variant="subtitle2">Action</Typography>
        <IconButton size="small" onClick={() => setExpanded(e => !e)}>
          <ExpandMoreIcon
            style={{
              transform: expanded ? 'rotate(180deg)' : 'rotate(0deg)',
              transition: '0.2s'
            }}
            fontSize="small"
          />
        </IconButton>
      </Box>

      <Stack direction="row" spacing={0.5} flexWrap="wrap" mb={expanded ? 1 : 0}>
        {summary.map(s => {
            const color: 'default' | 'error' | 'warning' | 'primary' =
              s.endsWith('err') ? 'error'
                : s.startsWith('W:') ? 'warning'
                  : s === 'webhook' || HTTP_METHODS.includes(s as HttpMethod) ? 'primary'
                    : 'default';
            return <Chip key={s} size="small" label={s} color={color} variant="outlined" />;
        })}
      </Stack>

      <Collapse in={expanded} unmountOnExit>
        <RadioGroup
          row
          value={kind}
          onChange={(e) => changeKind(e.target.value as AutomaticAction['kind'])}
        >
          <FormControlLabel value="noop" control={<Radio size="small" />} label={<span style={{ display: 'flex', alignItems: 'center', gap: 4 }}><NoopIcon fontSize="inherit" /> Noop</span>} />
          <FormControlLabel value="webhook" control={<Radio size="small" />} label={<span style={{ display: 'flex', alignItems: 'center', gap: 4 }}><HttpIcon fontSize="inherit" /> Webhook</span>} />
        </RadioGroup>

        <Divider sx={{ my: 1 }} />

        {kind === 'webhook' && (
          <Box>
            <TextField
              fullWidth
              size="small"
              label="URL"
              value={url}
              onChange={(e) => setUrl(e.target.value)}
              placeholder="https://example.com/hook"
              sx={{ mb: 1.5 }}
              error={!!url && !/^https?:\/\//i.test(url)}
              helperText={url && !/^https?:\/\//i.test(url) ? 'Must start with http:// or https://' : ' '}
              onBlur={() => {
                if (url) {
                  trackWorkflow('automatic.webhook.url.blur', {
                    nodeId: node.id,
                    validScheme: /^https?:\/\//i.test(url)
                  });
                }
              }}
            />

            <Stack direction="row" spacing={1} sx={{ mb: 1.5 }}>
              <TextField
                select
                size="small"
                label="Method"
                value={method}
                onChange={(e) => {
                  setMethod(e.target.value as HttpMethod);
                  trackWorkflow('automatic.action.changed', { nodeId: node.id, kind, method: e.target.value });
                }}
                sx={{ width: 140 }}
              >
                {HTTP_METHODS.map(m => <MenuItem key={m} value={m}>{m}</MenuItem>)}
              </TextField>

              <Button
                variant="outlined"
                size="small"
                onClick={() => {
                  clearWebhook();
                  trackWorkflow('automatic.action.changed', { nodeId: node.id, kind, reset: true });
                }}
              >
                Reset
              </Button>
              <Button
                variant="outlined"
                size="small"
                color="inherit"
                onClick={() => setShowHeaders(h => !h)}
              >
                {showHeaders ? 'Hide Headers' : 'Headers'}
              </Button>
            </Stack>

            {showHeaders && (
              <Box sx={{
                border: theme => `1px solid ${theme.palette.divider}`,
                borderRadius: 1,
                p: 1,
                mb: 1.5
              }}>
                <Stack direction="row" justifyContent="space-between" alignItems="center" sx={{ mb: 0.5 }}>
                  <Typography variant="caption" fontWeight={600}>Headers</Typography>
                  <IconButton size="small" onClick={addHeaderRow}>
                    <AddIcon fontSize="inherit" />
                  </IconButton>
                </Stack>
                <Stack spacing={0.5}>
                  {rawHeaders.length === 0 && (
                    <Typography variant="caption" color="text.secondary">
                      No headers
                    </Typography>
                  )}
                  {rawHeaders.map(row => (
                    <Stack key={row.id} direction="row" spacing={0.5} alignItems="center">
                      <TextField
                        size="small"
                        placeholder="Key"
                        value={row.key}
                        onChange={(e) => updateHeaderRow(row.id, 'key', e.target.value)}
                        sx={{ flex: 1 }}
                      />
                      <TextField
                        size="small"
                        placeholder="Value"
                        value={row.value}
                        onChange={(e) => updateHeaderRow(row.id, 'value', e.target.value)}
                        sx={{ flex: 1 }}
                      />
                      <IconButton
                        size="small"
                        onClick={() => removeHeaderRow(row.id)}
                      >
                        <DeleteIcon fontSize="inherit" />
                      </IconButton>
                    </Stack>
                  ))}
                </Stack>
              </Box>
            )}

            <Box sx={{ mb: 1.5 }}>
              <Typography variant="caption" fontWeight={600} display="block" sx={{ mb: 0.5 }}>
                Body Template (optional)
              </Typography>
              <HybridExpressionEditor
                value={body}
                onChange={(v) => setBody(v)}
                semantic={false}
                useMonaco
                kind="gateway"
                placeholder='{
   "event": "sample",
   "payload": { "id": "{{task.id}}" }
 }'
                height={180}
                onSemanticValidation={undefined}
              />
              {bodyJsonError && (
                <Typography variant="caption" color="warning.main" sx={{ display: 'block', mt: 0.5 }}>
                  Body not valid JSON (will be sent as raw text)
                </Typography>
              )}
              {largeBody && (
                <Typography variant="caption" color="warning.main" sx={{ display: 'block', mt: 0.5 }}>
                  Body size &gt; 10KB (warning)
                </Typography>
              )}
            </Box>

            <Accordion
              elevation={0}
              disableGutters
              sx={{
                border: theme => `1px solid ${theme.palette.divider}`,
                borderRadius: 1,
                mb: 1.5
              }}
              expanded={showAdvanced}
              onChange={() => setShowAdvanced(a => !a)}
            >
              <AccordionSummary expandIcon={<ExpandMoreIcon />}>
                <Typography variant="caption" fontWeight={600} display="flex" alignItems="center" gap={0.5}>
                  <RetryIcon fontSize="inherit" /> Retry Policy
                </Typography>
              </AccordionSummary>
              <AccordionDetails>
                <Stack direction="row" spacing={1} alignItems="center" sx={{ mb: 1 }}>
                  <TextField
                    size="small"
                    label="Max Attempts"
                    type="number"
                    value={maxAttempts}
                    onChange={(e) => {
                      const v = Number(e.target.value);
                      setMaxAttempts(isNaN(v) ? 1 : v);
                      trackWorkflow('automatic.retry.configured', { nodeId: node.id, maxAttempts: v, backoffSeconds });
                    }}
                    sx={{ width: 140 }}
                    inputProps={{ min: 1 }}
                  />
                  <TextField
                    size="small"
                    label="Backoff (s)"
                    type="number"
                    value={backoffSeconds}
                    onChange={(e) => {
                      const v = Number(e.target.value);
                      setBackoffSeconds(isNaN(v) ? 0 : v);
                      trackWorkflow('automatic.retry.configured', { nodeId: node.id, maxAttempts, backoffSeconds: v });
                    }}
                    sx={{ width: 140 }}
                    disabled={maxAttempts <= 1}
                    inputProps={{ min: 0 }}
                  />
                </Stack>
                <Typography variant="caption" color="text.secondary">
                  Attempts &gt; 1 enables basic linear retry (attemptInterval = backoffSeconds).
                </Typography>
              </AccordionDetails>
            </Accordion>

            {(errors.length > 0 || warnings.length > 0) && (
              <Stack spacing={1} sx={{ mb: 1.5 }}>
                {errors.length > 0 && (
                  <Alert severity="error" variant="outlined" sx={{ p: 1 }}>
                    <Typography variant="caption" component="div">
                      {errors.map((e, i) => <div key={i}>{e}</div>)}
                    </Typography>
                  </Alert>
                )}
                {warnings.length > 0 && (
                  <Alert severity="warning" variant="outlined" sx={{ p: 1 }}>
                    <Typography variant="caption" component="div">
                      {warnings.map((w, i) => <div key={i}>{w}</div>)}
                    </Typography>
                  </Alert>
                )}
              </Stack>
            )}
          </Box>
        )}

        {kind === 'noop' && (
          <Alert severity="info" sx={{ p: 1 }}>
            <Typography variant="caption">
              Noop action performs no external call. Use Webhook for outbound HTTP.
            </Typography>
          </Alert>
        )}

        {/* Confirm discard dialog (simple inline) */}
        {confirmReset && (
          <Alert
            severity="warning"
            icon={<WarningIcon fontSize="small" />}
            sx={{ mt: 2 }}
            action={
              <Stack direction="row" spacing={1}>
                <Button size="small" color="inherit" onClick={cancelSwitch}>Cancel</Button>
                <Button
                  size="small"
                  color="warning"
                  variant="contained"
                  onClick={confirmSwitch}
                >
                  Discard & Switch
                </Button>
              </Stack>
            }
          >
            <Typography variant="caption">
              Switching to Noop will discard current webhook configuration.
            </Typography>
          </Alert>
        )}
      </Collapse>
    </Box>
  );
};

export default ActionSection;
