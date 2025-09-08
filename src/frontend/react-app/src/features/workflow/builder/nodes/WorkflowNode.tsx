import { memo } from 'react';
import { Handle, Position, NodeProps } from 'reactflow';
import { Box, Typography, Paper, Chip } from '@mui/material';
import {
  PlayArrow as StartIcon,
  Stop as EndIcon,
  Person as HumanTaskIcon,
  Settings as AutomaticIcon,
  AccountTree as GatewayIcon,
  Schedule as TimerIcon,
  MergeType as JoinIcon
} from '@mui/icons-material';
import type { DslNode } from '../../dsl/dsl.types';
import { useGatewayHandles } from '../hooks/useGatewayHandles';

const iconMap = {
  start: StartIcon,
  end: EndIcon,
  humanTask: HumanTaskIcon,
  automatic: AutomaticIcon,
  gateway: GatewayIcon,
  timer: TimerIcon,
  join: JoinIcon
};

const colorMap = {
  start: { bg: '#e3f2fd', border: '#1976d2', text: '#1976d2' },
  end: { bg: '#fce4ec', border: '#c2185b', text: '#c2185b' },
  humanTask: { bg: '#f3e5f5', border: '#7b1fa2', text: '#7b1fa2' },
  automatic: { bg: '#e8f5e8', border: '#388e3c', text: '#388e3c' },
  gateway: { bg: '#ede7f6', border: '#5e35b1', text: '#5e35b1' },
  timer: { bg: '#e1f5fe', border: '#0277bd', text: '#0277bd' },
  join: { bg: '#fff8e1', border: '#ff8f00', text: '#ff8f00' }
};

export const WorkflowNode = memo(({ data, selected }: NodeProps<DslNode>) => {
  const IconComponent = iconMap[data.type];
  const colors = colorMap[data.type];
  const isGateway = data.type === 'gateway';
  const isJoin = data.type === 'join';

  const strategy: string | undefined = (data as any).strategy ?? (data as any).properties?.strategy;
  const handles = isGateway ? useGatewayHandles(strategy as any) : null;

  return (
    <Paper
      elevation={selected ? 8 : 2}
      sx={{
        minWidth: 140,
        minHeight: isGateway ? 90 : 60,
        bgcolor: colors.bg,
        border: 2,
        borderColor: selected ? colors.border : 'transparent',
        borderStyle: 'solid',
        cursor: 'pointer',
        position: 'relative'
      }}
    >
      {/* Target handle (all except start) */}
      {data.type !== 'start' && (
        <Handle
          type="target"
            position={Position.Top}
          style={{ background: colors.border, width: 8, height: 8 }}
        />
      )}

      <Box
        sx={{
          p: 1.25,
          display: 'flex',
          flexDirection: 'column',
          alignItems: 'center',
          gap: 0.5
        }}
      >
        <IconComponent sx={{ color: colors.text }} fontSize="small" />
        <Typography
          variant="caption"
          fontWeight="medium"
          sx={{
            color: colors.text,
            textAlign: 'center',
            lineHeight: 1.2,
            maxWidth: 120,
            overflow: 'hidden',
            textOverflow: 'ellipsis'
          }}
        >
          {data.label || data.type}
        </Typography>

        {isGateway && (
          <Chip
            size="small"
            label={(strategy || 'exclusive').slice(0, 9)}
            sx={{
              height: 16,
              fontSize: '0.55rem',
              bgcolor: '#fff',
              border: '1px solid',
              borderColor: colors.border,
              mt: 0.2
            }}
          />
        )}

        {isJoin && (
          <Chip
            size="small"
            label={(data as any).mode || 'all'}
            sx={{
              height: 16,
              fontSize: '0.55rem',
              bgcolor: '#fff',
              border: '1px solid',
              borderColor: colors.border,
              mt: 0.2
            }}
          />
        )}

        {data.type === 'humanTask' && (data as any).assigneeRoles?.length > 0 && (
          <Typography variant="caption" sx={{ color: 'text.secondary', fontSize: '0.6rem' }}>
            {(data as any).assigneeRoles.length} role(s)
          </Typography>
        )}
        {data.type === 'timer' && (data as any).delayMinutes != null && (
          <Typography variant="caption" sx={{ color: 'text.secondary', fontSize: '0.6rem' }}>
            {(data as any).delayMinutes}m
          </Typography>
        )}
      </Box>

      {/* Default bottom source (except end & gateway where custom) */}
      {data.type !== 'end' && !isGateway && !isJoin && (
        <Handle
          type="source"
          position={Position.Bottom}
          style={{ background: colors.border, width: 8, height: 8 }}
        />
      )}

      {/* Join keeps a single bottom source for downstream continuation */}
      {isJoin && (
        <Handle
          type="source"
          position={Position.Bottom}
          id="out"
          style={{ background: colors.border, width: 10, height: 10 }}
        />
      )}

      {/* Gateway handles */}
      {isGateway && handles}
    </Paper>
  );
});

WorkflowNode.displayName = 'WorkflowNode';
