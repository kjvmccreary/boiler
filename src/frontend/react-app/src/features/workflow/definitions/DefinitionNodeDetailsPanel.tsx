import { Drawer, Box, Typography, Divider } from '@mui/material';
import type { DslNode } from '../dsl/dsl.types';

interface Props {
  open: boolean;
  node: DslNode | null;
  onClose: () => void;
}

export function DefinitionNodeDetailsPanel({ open, node, onClose }: Props) {
  return (
    <Drawer
      anchor="right"
      open={open}
      onClose={onClose}
      PaperProps={{ sx: { width: 320, p: 2 } }}
    >
      {!node && (
        <Typography variant="body2" color="text.secondary">
          Select a node to see its properties.
        </Typography>
      )}
      {node && (
        <Box>
          <Typography variant="h6" gutterBottom>
            {node.label || node.type}
          </Typography>
          <Divider sx={{ mb: 2 }} />
          <Info label="ID" value={node.id} />
          <Info label="Type" value={node.type} />
          {node.type === 'humanTask' && (
            <>
              <Info label="Assignee Roles" value={(node.assigneeRoles || []).join(', ') || '—'} />
              <Info label="Due (min)" value={node.dueInMinutes?.toString() ?? '—'} />
            </>
          )}
          {node.type === 'automatic' && (
            <Info label="Action Kind" value={node.action?.kind || 'noop'} />
          )}
          {node.type === 'gateway' && (
            <Info label="Condition" value={node.condition || '—'} multiline />
          )}
          {node.type === 'timer' && (
            <>
              <Info label="Delay (min)" value={node.delayMinutes?.toString() ?? '—'} />
              <Info label="Until ISO" value={(node as any).untilIso || '—'} />
            </>
          )}
        </Box>
      )}
    </Drawer>
  );
}

function Info({ label, value, multiline }: { label: string; value: string; multiline?: boolean }) {
  return (
    <Box sx={{ mb: 1.5 }}>
      <Typography variant="caption" color="text.secondary">{label}</Typography>
      <Typography
        variant="body2"
        sx={{
          fontFamily: multiline ? 'monospace' : undefined,
          whiteSpace: multiline ? 'pre-wrap' : 'normal',
          wordBreak: 'break-word'
        }}
      >
        {value}
      </Typography>
    </Box>
  );
}

export default DefinitionNodeDetailsPanel;
