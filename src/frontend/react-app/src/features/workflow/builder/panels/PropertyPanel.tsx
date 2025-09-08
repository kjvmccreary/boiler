import { Box, Drawer, IconButton, Typography } from '@mui/material';
import CloseIcon from '@mui/icons-material/Close';
import { Node } from 'reactflow';
import GatewayPropertiesPanel from './GatewayPropertiesPanel';
import { GatewayStrategy } from '../../dsl/dsl.types';

export interface PropertyPanelProps {
  open: boolean;
  node?: Node;
  onClose: () => void;
  updateNode: (id: string, patch: Record<string, any>) => void;
}

export function PropertyPanel({ open, node, onClose, updateNode }: PropertyPanelProps) {
  if (!node) {
    return (
      <Drawer anchor="right" open={open} onClose={onClose}>
        <Box sx={{ width: 360, p: 2 }} />
      </Drawer>
    );
  }

  const data = node.data || {};
  const type = node.type;

  const update = (patch: Record<string, any>) => {
    updateNode(node.id, patch);
  };

  let body: JSX.Element | null = null;

  if (type === 'gateway') {
    const props = data.properties || {};
    const strategy: GatewayStrategy | undefined = props.strategy;
    body = (
      <GatewayPropertiesPanel
        nodeId={node.id}
        strategy={strategy}
        condition={props.condition}
        onChange={(patch) => update(patch)}
      />
    );
  } else {
    body = (
      <Typography variant="body2" color="text.secondary">
        No specialized editor for node type "{type}" yet.
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
