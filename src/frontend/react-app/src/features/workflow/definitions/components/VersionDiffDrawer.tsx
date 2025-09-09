import React from 'react';
import {
  Drawer,
  Box,
  Typography,
  IconButton,
  Chip,
  Divider,
  Collapse,
  Tooltip,
  Stack
} from '@mui/material';
import CloseIcon from '@mui/icons-material/Close';
import type { DiffResult, NodeModification } from '../utils/diffWorkflowDefinitions';
import { trackWorkflow } from '@/features/workflow/telemetry/workflowTelemetry';

export interface VersionDiffDrawerProps {
  open: boolean;
  onClose: () => void;
  currentVersion: number;
  previousVersion: number;
  diff: DiffResult | null;
  loading?: boolean;
  name: string;
}

const Section: React.FC<{
  title: string;
  count: number;
  children: React.ReactNode;
  defaultOpen?: boolean;
}> = ({ title, count, children, defaultOpen }) => {
  const [open, setOpen] = React.useState(!!defaultOpen);
  return (
    <Box>
      <Box display="flex" alignItems="center" justifyContent="space-between" sx={{ cursor: 'pointer' }} onClick={() => setOpen(o => !o)}>
        <Typography variant="subtitle2">{title}</Typography>
        <Chip size="small" label={count} variant="outlined" />
      </Box>
      <Collapse in={open}>
        <Box mt={1}>
          {count === 0 ? (
            <Typography variant="caption" color="text.secondary">None</Typography>
          ) : children}
        </Box>
      </Collapse>
      <Divider sx={{ my: 1.5 }} />
    </Box>
  );
};

export const VersionDiffDrawer: React.FC<VersionDiffDrawerProps> = ({
  open,
  onClose,
  diff,
  currentVersion,
  previousVersion,
  loading,
  name
}) => {
  return (
    <Drawer
      anchor="right"
      open={open}
      onClose={onClose}
      PaperProps={{ sx: { width: { xs: '100%', sm: 500 }, p: 2, display: 'flex', flexDirection: 'column' } }}
    >
      <Box display="flex" alignItems="center" justifyContent="space-between" mb={1.5}>
        <Typography variant="h6" sx={{ fontSize: '1rem' }}>
          Diff v{currentVersion} ↔ v{previousVersion}
        </Typography>
        <IconButton size="small" onClick={onClose}>
          <CloseIcon fontSize="small" />
        </IconButton>
      </Box>
      <Typography variant="caption" color="text.secondary" sx={{ mb: 1 }}>
        {name}
      </Typography>

      {loading && (
        <Typography variant="body2">Loading previous version…</Typography>
      )}

      {!loading && diff && diff.error && (
        <Typography variant="body2" color="error">
          {diff.error}
        </Typography>
      )}

      {!loading && diff && !diff.error && (
        <Box sx={{ flex: 1, overflowY: 'auto' }}>
          <Stack direction="row" spacing={1} flexWrap="wrap" mb={2}>
            <Chip label={`+${diff.summary.addedNodes} nodes`} size="small" color={diff.summary.addedNodes ? 'success' : 'default'} variant="outlined" />
            <Chip label={`-${diff.summary.removedNodes} nodes`} size="small" color={diff.summary.removedNodes ? 'error' : 'default'} variant="outlined" />
            <Chip label={`Δ${diff.summary.modifiedNodes} nodes`} size="small" color={diff.summary.modifiedNodes ? 'warning' : 'default'} variant="outlined" />
            <Chip label={`+${diff.summary.addedEdges} edges`} size="small" color={diff.summary.addedEdges ? 'success' : 'default'} variant="outlined" />
            <Chip label={`-${diff.summary.removedEdges} edges`} size="small" color={diff.summary.removedEdges ? 'error' : 'default'} variant="outlined" />
          </Stack>

          <Section title="Added Nodes" count={diff.addedNodes.length} defaultOpen={diff.addedNodes.length > 0}>
            <Stack spacing={0.5}>
              {diff.addedNodes.map(n => (
                <Typography key={n.id} variant="caption" sx={{ fontFamily: 'monospace' }}>
                  {n.id} ({n.type}) {n.label ? `– ${n.label}` : ''}
                </Typography>
              ))}
            </Stack>
          </Section>

          <Section title="Removed Nodes" count={diff.removedNodes.length}>
            <Stack spacing={0.5}>
              {diff.removedNodes.map(n => (
                <Typography key={n.id} variant="caption" sx={{ fontFamily: 'monospace' }}>
                  {n.id} ({n.type}) {n.label ? `– ${n.label}` : ''}
                </Typography>
              ))}
            </Stack>
          </Section>

            <Section title="Modified Nodes" count={diff.modifiedNodes.length}>
            <Stack spacing={0.75}>
              {diff.modifiedNodes.map(m => (
                <ModifiedNodeRow key={m.id} mod={m} />
              ))}
            </Stack>
          </Section>

          <Section title="Added Edges" count={diff.addedEdges.length}>
            <Stack spacing={0.5}>
              {diff.addedEdges.map((e, i) => {
                const id = e.id || `${e.from || e.source}->${e.to || e.target}`;
                return (
                  <Typography key={id + i} variant="caption" sx={{ fontFamily: 'monospace' }}>
                    {id}
                  </Typography>
                );
              })}
            </Stack>
          </Section>

          <Section title="Removed Edges" count={diff.removedEdges.length}>
            <Stack spacing={0.5}>
              {diff.removedEdges.map((e, i) => {
                const id = e.id || `${e.from || e.source}->${e.to || e.target}`;
                return (
                  <Typography key={id + i} variant="caption" sx={{ fontFamily: 'monospace' }}>
                    {id}
                  </Typography>
                );
              })}
            </Stack>
          </Section>

          {diff.summary.addedNodes === 0 &&
            diff.summary.removedNodes === 0 &&
            diff.summary.modifiedNodes === 0 &&
            diff.summary.addedEdges === 0 &&
            diff.summary.removedEdges === 0 && (
              <Typography variant="caption" color="text.secondary">
                No differences detected.
              </Typography>
            )}
        </Box>
      )}

      <Typography variant="caption" color="text.secondary" sx={{ mt: 1 }}>
        PR1 baseline – field-level diff; visual overlay planned in PR3.
      </Typography>
    </Drawer>
  );
};

const ModifiedNodeRow: React.FC<{ mod: NodeModification }> = ({ mod }) => {
  const [open, setOpen] = React.useState(false);

  const toggle = () => {
    setOpen(o => {
      const next = !o;
      if (next) {
        trackWorkflow('diff.viewer.modified.expanded', {
          nodeId: mod.id,
          changedCount: mod.changedFields?.length || mod.changedKeys.length
        });
      }
      return next;
    });
  };

  return (
    <Box>
      <Box
        display="flex"
        alignItems="center"
        justifyContent="space-between"
        sx={{ cursor: 'pointer' }}
        onClick={toggle}
      >
        <Typography variant="caption" sx={{ fontFamily: 'monospace' }}>
          {mod.id} ({mod.type}) {mod.label ? `– ${mod.label}` : ''}
        </Typography>
        <Stack direction="row" spacing={0.5}>
          {(mod.changedFields?.length ? mod.changedFields : mod.changedKeys.map(k => ({ field: k } as any)))
            .slice(0, 4)
            .map(cf => (
              <Chip
                key={cf.field}
                label={cf.field}
                size="small"
                variant="outlined"
                color="warning"
              />
            ))}
          {(mod.changedFields?.length || mod.changedKeys.length) > 4 && (
            <Chip
              label={`+${(mod.changedFields?.length || mod.changedKeys.length) - 4}`}
              size="small"
              variant="outlined"
              color="warning"
            />
          )}
        </Stack>
      </Box>
      <Collapse in={open}>
        <Box mt={0.5} ml={0.5}>
          {mod.changedFields?.length
            ? (
              <Stack spacing={0.5}>
                {mod.changedFields.map(f => (
                  <Typography
                    key={f.field}
                    variant="caption"
                    sx={{ fontFamily: 'monospace', display: 'block' }}
                  >
                    {f.field}:{" "}
                    <span style={{ color: '#b71c1c' }}>{formatValue(f.before)}</span>
                    {" -> "}
                    <span style={{ color: '#1b5e20' }}>{formatValue(f.after)}</span>
                  </Typography>
                ))}
              </Stack>
            )
            : (
              <>
                <Typography variant="caption" sx={{ fontFamily: 'monospace', display: 'block', opacity: 0.8 }}>
                  - {JSON.stringify(mod.before)}
                </Typography>
                <Typography variant="caption" sx={{ fontFamily: 'monospace', display: 'block', opacity: 0.8 }}>
                  + {JSON.stringify(mod.after)}
                </Typography>
              </>
            )}
        </Box>
      </Collapse>
    </Box>
  );
};

function formatValue(v: any) {
  if (v === undefined) return '∅';
  if (v === null) return 'null';
  if (typeof v === 'string') {
    if (v.length > 40) return `"${v.slice(0, 37)}..."`;
    return JSON.stringify(v);
  }
  if (typeof v === 'object') return JSON.stringify(v);
  return String(v);
}

export default VersionDiffDrawer;
