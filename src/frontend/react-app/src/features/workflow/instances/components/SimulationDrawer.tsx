import React, { useEffect, useState, useCallback } from 'react';
import {
  Drawer,
  Box,
  Typography,
  IconButton,
  TextField,
  Button,
  Divider,
  Chip,
  Tooltip,
  FormControlLabel,
  Switch
} from '@mui/material';
import CloseIcon from '@mui/icons-material/Close';
import PlayArrowIcon from '@mui/icons-material/PlayArrow';
import RestartAltIcon from '@mui/icons-material/RestartAlt';
import { simulateWorkflow, SimulationResult } from '../utils/simulateWorkflow';
import { trackWorkflow } from '@/features/workflow/telemetry/workflowTelemetry';

export interface SimulationDrawerProps {
  open: boolean;
  onClose: () => void;
  definitionJson: string | null;
  onHighlightPath: (nodeIds: string[] | null) => void;
}

export const SimulationDrawer: React.FC<SimulationDrawerProps> = ({
  open,
  onClose,
  definitionJson,
  onHighlightPath
}) => {
  const [contextInput, setContextInput] = useState<string>('{}');
  const [contextError, setContextError] = useState<string | null>(null);
  const [result, setResult] = useState<SimulationResult | null>(null);
  const [running, setRunning] = useState(false);
  const [selectedPathId, setSelectedPathId] = useState<string | null>(null);
  const [maxPaths, setMaxPaths] = useState<number>(200);
  const [maxDepth, setMaxDepth] = useState<number>(250);
  const [parallelExplode, setParallelExplode] = useState<boolean>(true);
  const [maxCartesian, setMaxCartesian] = useState<number>(64);

  const parseContext = (): Record<string, any> | null => {
    if (!contextInput.trim()) return {};
    try {
      return JSON.parse(contextInput);
    } catch (e: any) {
      setContextError(e.message);
      return null;
    }
  };

  const runSim = useCallback(() => {
    const ctx = parseContext();
    if (ctx == null) return;
    setContextError(null);
    setRunning(true);
    trackWorkflow('simulation.run.start', {
      hasContext: Object.keys(ctx).length > 0
    });
    setTimeout(() => {
      const sim = simulateWorkflow(definitionJson, {
        context: ctx,
        maxPaths,
        maxDepth,
        parallelExplode,
        maxBranchCartesian: maxCartesian
      });
      setResult(sim);
      trackWorkflow('simulation.run.complete', {
        pathCount: sim.paths.length,
        truncated: sim.truncated,
        maxLength: sim.paths.reduce((m, p) => Math.max(m, p.length), 0),
        hasContext: Object.keys(ctx).length > 0,
        cartesianCapped: !!sim.cartesianCapped
      });
      if (sim.truncated) {
        trackWorkflow('simulation.truncated', {
          reasons: sim.reasons
        });
      }
      setRunning(false);
    }, 0);
  }, [definitionJson, contextInput, maxPaths, maxDepth, parallelExplode, maxCartesian]);

  useEffect(() => {
    if (!open) {
      setResult(null);
      setSelectedPathId(null);
      onHighlightPath(null);
    }
  }, [open, onHighlightPath]);

  return (
    <Drawer anchor="right" open={open} onClose={onClose}>
      <Box sx={{ width: 420, display: 'flex', flexDirection: 'column', height: '100%' }}>
        <Box sx={{ display: 'flex', alignItems: 'center', p: 2, pb: 1 }}>
          <Typography variant="h6" sx={{ flex: 1 }}>Simulation</Typography>
          <IconButton onClick={onClose}><CloseIcon /></IconButton>
        </Box>
        <Divider />
        <Box sx={{ p: 2, display: 'flex', flexDirection: 'column', gap: 2, flex: 1, overflow: 'hidden' }}>
          <Typography variant="body2">
            Enumerate possible terminal paths. Conditional gateways evaluate with provided context.
            Unknown / errors branch both ways.
          </Typography>
          <Box sx={{ display: 'grid', gridTemplateColumns: 'repeat(2,1fr)', gap: 1 }}>
            <TextField
              label="Max Paths"
              size="small"
              type="number"
              value={maxPaths}
              onChange={e => setMaxPaths(Math.max(1, Number(e.target.value)))}
            />
            <TextField
              label="Max Depth"
              size="small"
              type="number"
              value={maxDepth}
              onChange={e => setMaxDepth(Math.max(1, Number(e.target.value)))}
            />
            <TextField
              label="Max Cartesian"
              size="small"
              type="number"
              value={maxCartesian}
              onChange={e => setMaxCartesian(Math.max(1, Number(e.target.value)))}
            />
            <Box sx={{ display: 'flex', alignItems: 'center', pl: 0.5 }}>
              <Tooltip title="Disable to linearize parallel gateways (perf shortcut)">
                <FormControlLabel
                  control={
                    <Switch
                      size="small"
                      checked={parallelExplode}
                      onChange={(e) => setParallelExplode(e.target.checked)}
                    />
                  }
                  label={<Typography variant="caption">Explode Parallels</Typography>}
                />
              </Tooltip>
            </Box>
          </Box>
          <TextField
            label="Context (JSON)"
            value={contextInput}
            onChange={e => setContextInput(e.target.value)}
            multiline
            minRows={3}
            error={!!contextError}
            helperText={contextError || ' '}
            size="small"
          />
          <Box sx={{ display: 'flex', gap: 1 }}>
            <Button
              variant="contained"
              startIcon={<PlayArrowIcon />}
              onClick={runSim}
              disabled={running || !definitionJson}
            >
              {running ? 'Running...' : 'Run Simulation'}
            </Button>
            <Button
              variant="outlined"
              startIcon={<RestartAltIcon />}
              disabled={running}
              onClick={() => {
                setContextInput('{}');
                setContextError(null);
                setResult(null);
                setSelectedPathId(null);
                onHighlightPath(null);
              }}
            >
              Reset
            </Button>
          </Box>

          {result && (
            <Box sx={{ flex: 1, overflowY: 'auto', mt: 1 }}>
              <Box sx={{ display: 'flex', gap: 1, flexWrap: 'wrap', mb: 1 }}>
                <Chip label={`Paths: ${result.paths.length}`} size="small" />
                <Chip label={`Max Len: ${result.paths.reduce((m, p) => Math.max(m, p.length), 0)}`} size="small" />
                {result.truncated && <Chip label="Truncated" color="warning" size="small" />}
                {result.cartesianCapped && <Chip label="Cartesian Cap" color="warning" size="small" />}
                {result.reasons.map(r => (
                  <Chip key={r} label={r} size="small" variant="outlined" />
                ))}
              </Box>
              {result.paths.length === 0 && (
                <Typography variant="caption" color="text.secondary">
                  No terminal paths discovered.
                </Typography>
              )}
              {result.paths.map(p => {
                const selected = p.id === selectedPathId;
                return (
                  <Box
                    key={p.id}
                    sx={{
                      p: 1,
                      mb: 0.75,
                      border: theme => `1px solid ${selected ? theme.palette.primary.main : theme.palette.divider}`,
                      borderRadius: 1,
                      bgcolor: selected ? 'primary.light' : 'background.paper',
                      cursor: 'pointer',
                      transition: 'background 120ms'
                    }}
                    onClick={() => {
                      setSelectedPathId(p.id);
                      onHighlightPath(p.nodes);
                      trackWorkflow('simulation.path.select', {
                        pathId: p.id,
                        length: p.length
                      });
                    }}
                  >
                    <Tooltip title={p.nodes.join(' → ')}>
                      <Typography
                        variant="caption"
                        sx={{
                          fontFamily: 'monospace',
                          whiteSpace: 'nowrap',
                          overflow: 'hidden',
                          textOverflow: 'ellipsis',
                          display: 'block'
                        }}
                      >
                        {p.id}: {p.nodes.join(' → ')}
                      </Typography>
                    </Tooltip>
                    {p.meta?.mergedAtJoinId && (
                      <Typography variant="caption" color="success.main" sx={{ display: 'block' }}>
                        Merged @ {p.meta.mergedAtJoinId}
                      </Typography>
                    )}
                    {p.meta?.partialParallelMerge && (
                      <Typography variant="caption" color="warning.main" sx={{ display: 'block' }}>
                        Partial parallel merge
                      </Typography>
                    )}
                    <Typography variant="caption" color="text.secondary">
                      Terminal: {p.terminalNodeId} • Len {p.length}
                    </Typography>
                  </Box>
                );
              })}
            </Box>
          )}
        </Box>
        <Divider />
        <Box sx={{ p: 1.5 }}>
          <Typography variant="caption" color="text.secondary">
            PR2: Parallel cartesian (capped), join merge detection, config caps. Future: edge condition evaluation per-edge, join mode semantics, probabilities.
          </Typography>
        </Box>
      </Box>
    </Drawer>
  );
};

export default SimulationDrawer;
