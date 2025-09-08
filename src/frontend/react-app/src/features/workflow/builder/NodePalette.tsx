import { Box, Typography, Paper, Tooltip } from '@mui/material';
import {
  PlayArrow as StartIcon,
  Stop as EndIcon,
  Person as HumanTaskIcon,
  Settings as AutomaticIcon,
  AccountTree as GatewayIcon,
  Schedule as TimerIcon,
  MergeType as JoinIcon
} from '@mui/icons-material';
import type { NodeType, NodePaletteItem } from '../dsl/dsl.types';

const nodeTypes: NodePaletteItem[] = [
  {
    type: 'start',
    label: 'Start',
    icon: 'PlayArrow',
    description: 'Starting point of the workflow',
    defaultData: { id: '', type: 'start', label: 'Start', x: 0, y: 0 }
  },
  {
    type: 'end',
    label: 'End',
    icon: 'Stop',
    description: 'End point of the workflow',
    defaultData: { id: '', type: 'end', label: 'End', x: 0, y: 0 }
  },
  {
    type: 'humanTask',
    label: 'Human Task',
    icon: 'Person',
    description: 'Task requiring human interaction',
    defaultData: {
      id: '',
      type: 'humanTask',
      label: 'Human Task',
      x: 0,
      y: 0,
      assigneeRoles: []
    }
  },
  {
    type: 'automatic',
    label: 'Automatic',
    icon: 'Settings',
    description: 'Automated system task',
    defaultData: {
      id: '',
      type: 'automatic',
      label: 'Automatic Task',
      x: 0,
      y: 0,
      action: { kind: 'noop' }
    }
  },
  {
    type: 'gateway',
    label: 'Gateway',
    icon: 'AccountTree',
    description: 'Branching logic (select strategy in properties)',
    defaultData: {
      id: '',
      type: 'gateway',
      label: 'Gateway',
      x: 0,
      y: 0,
      strategy: 'exclusive'
    }
  },
  {
    type: 'timer',
    label: 'Timer',
    icon: 'Schedule',
    description: 'Time-based delay',
    defaultData: {
      id: '',
      type: 'timer',
      label: 'Timer',
      x: 0,
      y: 0,
      delayMinutes: 5
    }
  },
  {
    type: 'join',
    label: 'Join',
    icon: 'MergeType',
    description: 'Synchronize parallel branches (base)',
    defaultData: {
      id: '',
      type: 'join',
      label: 'Join',
      x: 0,
      y: 0,
      mode: 'all',
      cancelRemaining: false
    } as any
  }
];

const iconMap = {
  PlayArrow: StartIcon,
  Stop: EndIcon,
  Person: HumanTaskIcon,
  Settings: AutomaticIcon,
  AccountTree: GatewayIcon,
  Schedule: TimerIcon,
  MergeType: JoinIcon
};

export function NodePalette() {
  const onDragStart = (event: React.DragEvent, nodeType: NodeType) => {
    event.dataTransfer.setData('application/reactflow', nodeType);
    event.dataTransfer.effectAllowed = 'move';
  };

  return (
    <Box
      sx={{
        width: 200,
        borderRight: 1,
        borderColor: 'divider',
        bgcolor: 'background.paper',
        p: 2,
        overflow: 'auto'
      }}
    >
      <Typography variant="h6" gutterBottom>
        Node Palette
      </Typography>
      <Typography variant="body2" color="text.secondary" sx={{ mb: 2 }}>
        Drag nodes to the canvas
      </Typography>

      <Box sx={{ display: 'flex', flexDirection: 'column', gap: 1 }}>
        {nodeTypes.map(nodeType => {
          const IconComponent = iconMap[nodeType.icon as keyof typeof iconMap];
          return (
            <Tooltip key={nodeType.type} title={nodeType.description} placement="right">
              <Paper
                sx={{
                  p: 2,
                  cursor: 'grab',
                  display: 'flex',
                  alignItems: 'center',
                  gap: 1,
                  '&:hover': { bgcolor: 'action.hover' },
                  '&:active': { cursor: 'grabbing' }
                }}
                draggable
                onDragStart={event => onDragStart(event, nodeType.type)}
              >
                <IconComponent fontSize="small" color="primary" />
                <Typography variant="body2" fontWeight="medium">
                  {nodeType.label}
                </Typography>
              </Paper>
            </Tooltip>
          );
        })}
      </Box>

      <Box sx={{ mt: 3 }}>
        <Typography variant="body2" color="text.secondary">
          <strong>Instructions:</strong>
        </Typography>
        <Typography variant="caption" color="text.secondary">
          • Drag nodes to canvas<br />
          • Connect nodes via handles<br />
          • Click nodes to edit properties<br />
          • Delete with Del/Backspace key
        </Typography>
      </Box>
    </Box>
  );
}
