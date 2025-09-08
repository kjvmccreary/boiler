import { Box, Drawer, IconButton, Typography } from '@mui/material';
import CloseIcon from '@mui/icons-material/Close';
import { Edge, Node } from 'reactflow';
import GatewayPropertiesPanel from './GatewayPropertiesPanel';
import JoinPropertiesPanel from './JoinPropertiesPanel';
import { GatewayStrategy, JoinMode } from '../../dsl/dsl.types';
import type { ReactNode } from 'react';

export interface PropertyPanelProps {
  open: boolean;
  node?: Node;
  edges: Edge[];
  onClose: () => void;
  updateNode: (id: string, patch: Record<string, any>) => void;
}

export function PropertyPanel({ open, node, edges, onClose, updateNode }: PropertyPanelProps) {
  if (!node) {
    return (
      <Drawer anchor="right" open={open} onClose={onClose}>
        <Box sx={{ width: 360, p: 2 }} />
      </Drawer>
    );
  }

  const data = node.data || {};
  const type = node.type;
  const update = (patch: Record<string, any>) => updateNode(node.id, patch);
  let body: ReactNode = null;

  if (type === 'gateway') {
    const props = (data as any).properties || data;
    const strategy: GatewayStrategy | undefined = props.strategy;
    const outgoing = edges.filter(e => e.source === node.id);
    const parallelWarnings: string[] = [];
    if (strategy === 'parallel' && outgoing.length < 2) {
      parallelWarnings.push('Needs at least 2 outgoing branches for a meaningful parallel fan-out.');
    }
    body = (
      <GatewayPropertiesPanel
        nodeId={node.id}
        strategy={strategy}
        condition={props.condition}
        onChange={patch => update(patch)}
        parallelBranchCount={outgoing.length}
        hasJoinCandidate={false}
        parallelWarnings={parallelWarnings}
      />
    );
  } else if (type === 'join') {
    const props = (data as any).properties || data;
    body = (
      <JoinPropertiesPanel
        nodeId={node.id}
        mode={props.mode as JoinMode}
        thresholdCount={props.thresholdCount}
        thresholdPercent={props.thresholdPercent}
        expression={props.expression}
        cancelRemaining={props.cancelRemaining}
        onChange={patch => update(patch)}
      />
    );
  } else {
    body = (
      <Typography variant="body2" color="text.secondary">
        No specialized editor for node type &quot;{type}&quot; yet.
      </Typography>
    );
  }

  return (
    <Drawer anchor="right" open={open} onClose={onClose}>
      <Box sx={{ width: 360, p: 2, display: 'flex', flexDirection: 'column', gap: 2 }}>
        <Box sx={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between' }}>
          <Typography variant="h6" fontSize={16} fontWeight={600} sx={{ pr: 1 }}>
            Node Properties
          </Typography>
          <IconButton size="small" onClick={onClose}>
            <CloseIcon fontSize="small" />
          </IconButton>
        </Box>
        {body}
      </Box>
    </Drawer>
  );
}

export default PropertyPanel;
