import { memo } from 'react';
import { Handle, Position, NodeProps } from 'reactflow';
import { Box, Typography, Paper } from '@mui/material';
import {
  PlayArrow as StartIcon,
  Stop as EndIcon,
  Person as HumanTaskIcon,
  Settings as AutomaticIcon,
  AccountTree as GatewayIcon,
  Schedule as TimerIcon,
} from '@mui/icons-material';
import type { DslNode } from '../../dsl/dsl.types';

const iconMap = {
  start: StartIcon,
  end: EndIcon,
  humanTask: HumanTaskIcon,
  automatic: AutomaticIcon,
  gateway: GatewayIcon,
  timer: TimerIcon,
};

const colorMap = {
  start: { bg: '#e3f2fd', border: '#1976d2', text: '#1976d2' },
  end: { bg: '#fce4ec', border: '#c2185b', text: '#c2185b' },
  humanTask: { bg: '#f3e5f5', border: '#7b1fa2', text: '#7b1fa2' },
  automatic: { bg: '#e8f5e8', border: '#388e3c', text: '#388e3c' },
  gateway: { bg: '#fff3e0', border: '#f57c00', text: '#f57c00' },
  timer: { bg: '#e1f5fe', border: '#0277bd', text: '#0277bd' },
};

export const WorkflowNode = memo(({ data, selected }: NodeProps<DslNode>) => {
  const IconComponent = iconMap[data.type];
  const colors = colorMap[data.type];

  return (
    <Paper
      elevation={selected ? 8 : 2}
      sx={{
        minWidth: 120,
        minHeight: 60,
        bgcolor: colors.bg,
        border: 2,
        borderColor: selected ? colors.border : 'transparent',
        borderStyle: 'solid',
        cursor: 'pointer',
        '&:hover': {
          elevation: 4,
        },
      }}
    >
      {/* Input Handle */}
      {data.type !== 'start' && (
        <Handle
          type="target"
          position={Position.Top}
          style={{
            background: colors.border,
            width: 8,
            height: 8,
          }}
        />
      )}

      {/* Node Content */}
      <Box
        sx={{
          p: 1.5,
          display: 'flex',
          flexDirection: 'column',
          alignItems: 'center',
          gap: 0.5,
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
            maxWidth: 100,
            overflow: 'hidden',
            textOverflow: 'ellipsis',
          }}
        >
          {data.label || data.type}
        </Typography>
        
        {/* Additional info for specific node types */}
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

      {/* Output Handle */}
      {data.type !== 'end' && (
        <Handle
          type="source"
          position={Position.Bottom}
          style={{
            background: colors.border,
            width: 8,
            height: 8,
          }}
        />
      )}
      
      {/* Gateway has side handles for true/false paths */}
      {data.type === 'gateway' && (
        <>
          <Handle
            type="source"
            position={Position.Right}
            id="true"
            style={{
              background: '#4caf50',
              width: 8,
              height: 8,
              top: '30%',
            }}
          />
          <Handle
            type="source"
            position={Position.Right}
            id="false"
            style={{
              background: '#f44336',
              width: 8,
              height: 8,
              top: '70%',
            }}
          />
        </>
      )}
    </Paper>
  );
});

WorkflowNode.displayName = 'WorkflowNode';
