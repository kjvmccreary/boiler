import React from 'react';
import { Box, Chip, Tooltip, Typography, Stack } from '@mui/material';
import ErrorOutlineIcon from '@mui/icons-material/ErrorOutline';
import ReportProblemIcon from '@mui/icons-material/ReportProblem';
import type { ParallelGatewayDiagnostics } from '../../dsl/dsl.validate';

export interface ParallelDiagnosticsOverlayProps {
  diag: ParallelGatewayDiagnostics;
  compact?: boolean;
}

const COLORS = ['#1976d2', '#9c27b0', '#2e7d32', '#ed6c02', '#6d4c41', '#00838f'];

export const ParallelDiagnosticsOverlay: React.FC<ParallelDiagnosticsOverlayProps> = ({ diag, compact }) => {
  if (!diag) return null;

  const badge = diag.hasError
    ? <Tooltip title="Parallel convergence error"><ErrorOutlineIcon color="error" fontSize="small" /></Tooltip>
    : (diag.multipleCommon || diag.subsetJoins.length || diag.orphanBranches.length
        ? <Tooltip title="Parallel convergence warnings"><ReportProblemIcon color="warning" fontSize="small" /></Tooltip>
        : null);

  return (
    <Box sx={{
      border: '1px solid',
      borderColor: diag.hasError ? 'error.main' : (diag.multipleCommon ? 'warning.main' : 'divider'),
      borderRadius: 1,
      p: 1,
      mb: 2,
      background: diag.hasError ? 'rgba(211,47,47,0.04)' : 'background.paper'
    }}>
      <Stack direction="row" spacing={1} alignItems="center" sx={{ mb: 1 }}>
        <Typography variant="subtitle2" sx={{ display: 'flex', alignItems: 'center', gap: 0.5 }}>
          Parallel Diagnostics
        </Typography>
        {badge}
        <Chip size="small" label={`${diag.branches.length} branches`} />
        {diag.commonJoins.length > 0 && (
          <Chip
            size="small"
            color={diag.multipleCommon ? 'warning' : 'success'}
            variant={diag.multipleCommon ? 'outlined' : 'filled'}
            label={diag.multipleCommon
              ? `Multiple common joins: ${diag.commonJoins.length}`
              : `Converges at ${diag.commonJoins[0]}`}
          />
        )}
        {diag.orphanBranches.length > 0 && (
          <Chip size="small" color="error" variant="outlined" label="Branch w/o join" />
        )}
        {diag.subsetJoins.length > 0 && (
          <Chip size="small" color="warning" variant="outlined" label="Subset joins" />
        )}
      </Stack>

      {!compact && (
        <Box>
          <Typography variant="caption" color="text.secondary">Branches:</Typography>
          <Stack spacing={0.5} sx={{ mt: 0.5 }}>
            {diag.branches.map((b, idx) => (
              <Box
                key={b.startNodeId}
                sx={{
                  display: 'flex',
                  alignItems: 'center',
                  gap: 1,
                  fontSize: '0.75rem'
                }}
              >
                <Box
                  sx={{
                    width: 12,
                    height: 12,
                    borderRadius: '50%',
                    background: COLORS[idx % COLORS.length],
                    flexShrink: 0
                  }}
                />
                <Typography variant="caption" sx={{ fontFamily: 'monospace' }}>
                  {b.startNodeId}
                </Typography>
                <Typography variant="caption" color="text.secondary">
                  → {b.firstJoins.length
                    ? b.firstJoins.join(', ')
                    : '(no join)'}
                </Typography>
              </Box>
            ))}
          </Stack>
          {diag.subsetJoins.length > 0 && (
            <Typography variant="caption" sx={{ mt: 1 }} color="warning.main">
              Subset convergence joins: {diag.subsetJoins.join(', ')}
            </Typography>
          )}
          {diag.orphanBranches.length > 0 && (
            <Typography variant="caption" sx={{ mt: 0.5 }} color="error.main">
              Orphan branches: {diag.orphanBranches.join(', ')}
            </Typography>
          )}
        </Box>
      )}
    </Box>
  );
};

export default ParallelDiagnosticsOverlay;
