import { memo } from 'react';
import { Handle, Position, NodeProps } from 'reactflow';
import { Box, Typography, Paper } from '@mui/material';
import {
  PlayArrow as StartIcon,
  Stop as EndIcon,
  Person as HumanTaskIcon,
  Settings as AutomaticIcon,
  AccountTree as GatewayIcon,
  Schedule as TimerIcon
} from '@mui/icons-material';
import type { DslNode } from '../../dsl/dsl.types';

const iconMap = {
  start: StartIcon,
  end: EndIcon,
  humanTask: HumanTaskIcon,
  automatic: AutomaticIcon,
  gateway: GatewayIcon,
  timer: TimerIcon
};

const colorMap = {
  start: { bg: '#e3f2fd', border: '#1976d2', text: '#1976d2' },
  end: { bg: '#fce4ec', border: '#c2185b', text: '#c2185b' },
  humanTask: { bg: '#f3e5f5', border: '#7b1fa2', text: '#7b1fa2' },
  automatic: { bg: '#e8f5e8', border: '#388e3c', text: '#388e3c' },
  gateway: { bg: '#ede7f6', border: '#5e35b1', text: '#5e35b1' }, // neutralized
  timer: { bg: '#e1f5fe', border: '#0277bd', text: '#0277bd' }
};

export const WorkflowNode = memo(({ data, selected }: NodeProps<DslNode>) => {
  const IconComponent = iconMap[data.type];
  const colors = colorMap[data.type];
  const isGateway = data.type === 'gateway';

  return (
    <Paper
      elevation={selected ? 8 : 2}
      sx={{
        minWidth: 130,
        minHeight: isGateway ? 80 : 60,
        bgcolor: colors.bg,
        border: 2,
        borderColor: selected ? colors.border : 'transparent',
        borderStyle: 'solid',
        cursor: 'pointer',
        position: 'relative'
      }}
    >
      {/* Input (except start) */}
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
            maxWidth: 110,
            overflow: 'hidden',
            textOverflow: 'ellipsis'
          }}
        >
          {data.label || data.type}
        </Typography>

        {data.type === 'humanTask' && (data as any).assigneeRoles?.length > 0 && (
          <Typography variant="caption" sx={{ color: 'text.secondary', fontSize: '0.6rem' }}>
            {(data as any).assigneeRoles.length} role(s)
          </Typography>
        )}

        {data.type === 'timer' && (data as any).delayMinutes && (
          <Typography variant="caption" sx={{ color: 'text.secondary', fontSize: '0.6rem' }}>
            {(data as any).delayMinutes}m
          </Typography>
        )}
      </Box>

      {/* Generic bottom output removed for gateway to prevent unlabeled edges */}
      {data.type !== 'end' && !isGateway && (
        <Handle
          type="source"
          position={Position.Bottom}
          style={{ background: colors.border, width: 8, height: 8 }}
        />
      )}

      {/* Binary gateway handles */}
      {isGateway && (
        <>
          <Handle
            type="source"
            position={Position.Right}
            id="true"
            style={{ background: '#16a34a', width: 10, height: 10, top: '35%' }}
          />
          <Handle
            type="source"
            position={Position.Right}
            id="false"
            style={{ background: '#dc2626', width: 10, height: 10, top: '70%' }}
          />
        </>
      )}
    </Paper>
  );
});

WorkflowNode.displayName = 'WorkflowNode';
