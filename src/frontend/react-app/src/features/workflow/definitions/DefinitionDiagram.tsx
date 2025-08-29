import { useEffect, useState, useCallback, useMemo } from 'react';
import { Box, Alert, CircularProgress, Chip, Stack, Tooltip } from '@mui/material';
import ReactFlow, {
  Background,
  Controls,
  MiniMap,
  Node,
  Edge,
  NodeMouseHandler
} from 'reactflow';
import 'reactflow/dist/style.css';
import { deserializeFromDsl } from '../dsl/dsl.serialize';
import type { DslDefinition, DslNode } from '../dsl/dsl.types';

type NodeRuntimeStatus =
  | 'active'
  | 'completed'
  | 'overdue-active'
  | 'overdue-pending'
  | 'due-soon'
  | 'pending';

interface TaskRuntime {
  nodeId: string;
  status: string;
  dueDate?: string;
}

interface DefinitionDiagramProps {
  jsonDefinition?: string | null;
  onNodeSelect?: (node: DslNode | null) => void;
  currentNodeIds?: string[] | string | null;
  tasks?: TaskRuntime[];
  traversedEdgeIds?: string[];
  visitedNodeIds?: string[];
  dueSoonMinutes?: number;
}

const DEFAULT_DUE_SOON_MIN = 15;

export function DefinitionDiagram({
  jsonDefinition,
  onNodeSelect,
  currentNodeIds,
  tasks,
  traversedEdgeIds,
  visitedNodeIds,
  dueSoonMinutes = DEFAULT_DUE_SOON_MIN
}: DefinitionDiagramProps) {
  // Raw parsed (base) nodes/edges – never augmented in state to avoid loops
  const [baseNodes, setBaseNodes] = useState<Node[]>([]);
  const [baseEdges, setBaseEdges] = useState<Edge[]>([]);
  const [error, setError] = useState<string | null>(null);
  const [loading, setLoading] = useState(false);

  // Normalize current node IDs
  const currentSet = useMemo(() => {
    if (!currentNodeIds) return new Set<string>();
    if (Array.isArray(currentNodeIds)) return new Set(currentNodeIds);
    const raw = currentNodeIds.trim();
    if (raw.startsWith('[')) {
      try {
        const arr = JSON.parse(raw);
        return new Set((Array.isArray(arr) ? arr : []).filter(Boolean));
      } catch {
        return new Set<string>();
      }
    }
    return new Set(raw.split(',').map(s => s.trim()).filter(Boolean));
  }, [currentNodeIds]);

  // Task lookups
  const taskByNode = useMemo(() => {
    const map = new Map<string, TaskRuntime>();
    (tasks || []).forEach(t => map.set(t.nodeId, t));
    return map;
  }, [tasks]);

  const traversedSet = useMemo(() => new Set(traversedEdgeIds || []), [traversedEdgeIds]);
  const visitedSet = useMemo(() => new Set(visitedNodeIds || []), [visitedNodeIds]);
  const now = Date.now();

  // Parse definition (once per jsonDefinition change)
  useEffect(() => {
    setError(null);
    setLoading(true);
    try {
      if (!jsonDefinition) {
        setBaseNodes([]);
        setBaseEdges([]);
        setError('No JSON definition found.');
        return;
      }
      const parsed: DslDefinition = JSON.parse(jsonDefinition);
      const { nodes: flowNodes, edges: flowEdges } = deserializeFromDsl(parsed);
      const roNodes = flowNodes.map(n => ({
        ...n,
        draggable: false,
        selectable: true
      }));
      setBaseNodes(roNodes);
      setBaseEdges(flowEdges);
    } catch (e: any) {
      setError(`Failed to parse workflow definition: ${e.message}`);
    } finally {
      setLoading(false);
    }
  }, [jsonDefinition]);

  // Node runtime status map (derived)
  const nodeStatusMap = useMemo<Record<string, NodeRuntimeStatus>>(() => {
    if (!baseNodes.length) return {};
    const map: Record<string, NodeRuntimeStatus> = {};
    baseNodes.forEach(n => {
      const id = n.id;
      const task = taskByNode.get(id);
      const isActive = currentSet.has(id);
      const visited = visitedSet.size > 0 ? visitedSet.has(id) : !!task;
      const dueDate = task?.dueDate ? Date.parse(task.dueDate) : undefined;
      const overdue = dueDate ? dueDate < now : false;
      const dueSoon = !overdue && dueDate ? (dueDate - now) / 60000 <= dueSoonMinutes : false;

      if (isActive && overdue) map[id] = 'overdue-active';
      else if (isActive) map[id] = 'active';
      else if (overdue && visited) map[id] = 'overdue-pending';
      else if (visited && !isActive) map[id] = 'completed';
      else if (dueSoon && !isActive) map[id] = 'due-soon';
      else map[id] = 'pending';
    });
    return map;
  }, [baseNodes, taskByNode, currentSet, visitedSet, now, dueSoonMinutes]);

  // Styling helpers
  const getNodeStyle = (status: NodeRuntimeStatus, type?: string): React.CSSProperties => {
    const baseBg =
      type === 'start' ? '#E3F2FD' :
        type === 'end' ? '#FCE4EC' :
          type === 'humanTask' ? '#E8F5E9' :
            type === 'automatic' ? '#FFF8E1' :
              type === 'gateway' ? '#EDE7F6' :
                type === 'timer' ? '#E0F7FA' : '#FAFAFA';

    const style: React.CSSProperties = {
      borderWidth: 2,
      borderStyle: 'solid',
      borderColor: '#CFD8DC',
      background: baseBg,
      fontSize: 12,
      padding: 4,
      transition: 'box-shadow 120ms, border-color 120ms'
    };

    switch (status) {
      case 'active':
        style.borderColor = '#1976d2';
        style.boxShadow = '0 0 0 3px rgba(25,118,210,0.25)';
        break;
      case 'overdue-active':
        style.borderColor = '#d32f2f';
        style.boxShadow = '0 0 0 4px rgba(211,47,47,0.35)';
        break;
      case 'overdue-pending':
        style.borderColor = '#d32f2f';
        style.background = '#FFEBEE';
        break;
      case 'completed':
        style.borderColor = '#66BB6A';
        style.background = '#E8F5E9';
        break;
      case 'due-soon':
        style.borderColor = '#FB8C00';
        style.borderStyle = 'dashed';
        break;
      case 'pending':
      default:
        break;
    }
    return style;
  };

  // Derive augmented nodes (memoized, no setState)
  const displayNodes = useMemo<Node[]>(() => {
    if (!baseNodes.length) return [];
    return baseNodes.map(n => {
      const status = nodeStatusMap[n.id];
      return {
        ...n,
        style: {
          ...(n.style || {}),
          ...getNodeStyle(status, (n.data as any)?.type || n.type)
        },
        data: {
          ...n.data,
          runtimeStatus: status
        }
      };
    });
  }, [baseNodes, nodeStatusMap]);

  // Derive augmented edges
  const displayEdges = useMemo<Edge[]>(() => {
    if (!baseEdges.length) return [];
    return baseEdges.map(e => {
      const traversed = traversedSet.has(e.id);
      return {
        ...e,
        style: {
          ...(e.style || {}),
          stroke: traversed ? '#1976d2' : '#B0BEC5',
          strokeWidth: traversed ? 2 : 1.5,
          strokeDasharray: traversed ? undefined : '3 3'
        },
        animated: traversed
      };
    });
  }, [baseEdges, traversedSet]);

  const handleNodeClick: NodeMouseHandler = useCallback((_, node: Node) => {
    const dslNode = (node.data as DslNode) || null;
    onNodeSelect?.(dslNode);
  }, [onNodeSelect]);

  if (loading) {
    return (
      <Box sx={{ p: 2, display: 'flex', gap: 2, alignItems: 'center' }}>
        <CircularProgress size={20} />
        Loading diagram...
      </Box>
    );
  }

  if (error) {
    return <Alert severity="error" sx={{ mb: 2 }}>{error}</Alert>;
  }

  if (!displayNodes.length) {
    return <Alert severity="info">No nodes to display.</Alert>;
  }

  return (
    <Box sx={{ position: 'relative' }}>
      <Box sx={{ height: 500, border: 1, borderColor: 'divider', borderRadius: 1, position: 'relative' }}>
        <ReactFlow
          nodes={displayNodes}
          edges={displayEdges}
          onNodeClick={handleNodeClick}
          fitView
          nodesDraggable={false}
          nodesConnectable={false}
          elementsSelectable
          panOnDrag
          panOnScroll
          zoomOnScroll
          zoomOnPinch
          selectionOnDrag={false}
          deleteKeyCode={null}
          multiSelectionKeyCode={null}
        >
          <Background gap={16} size={1} />
          <MiniMap pannable zoomable />
          <Controls showInteractive={false} />
        </ReactFlow>
      </Box>
      <RuntimeLegend />
    </Box>
  );
}

// Legend component
function RuntimeLegend() {
  const item = (label: string, color: string, variant: 'filled' | 'outlined' = 'filled') => (
    <Tooltip title={label}>
      <Chip
        size="small"
        label={label}
        variant={variant}
        sx={{
          borderColor: variant === 'outlined' ? color : undefined,
          backgroundColor: variant === 'filled' ? color : 'transparent',
          color: variant === 'filled' ? '#fff' : 'inherit'
        }}
      />
    </Tooltip>
  );

  return (
    <Box
      sx={{
        position: 'absolute',
        top: 8,
        right: 8,
        p: 1,
        bgcolor: 'background.paper',
        border: 1,
        borderColor: 'divider',
        borderRadius: 1,
        boxShadow: 1,
        maxWidth: 260
      }}
    >
      <Stack direction="row" flexWrap="wrap" gap={1}>
        {item('Active', '#1976d2')}
        {item('Overdue Active', '#d32f2f')}
        {item('Overdue (Inactive)', '#d32f2f', 'outlined')}
        {item('Completed', '#66BB6A')}
        {item('Due Soon', '#FB8C00', 'outlined')}
        {item('Pending', '#B0BEC5', 'outlined')}
        <Tooltip title="Traversed Edge">
          <Chip size="small" label="Edge Traversed" sx={{ background: '#1976d2', color: '#fff' }} />
        </Tooltip>
        <Tooltip title="Edge Pending">
          <Chip size="small" label="Edge Pending" variant="outlined" sx={{ borderColor: '#B0BEC5' }} />
        </Tooltip>
      </Stack>
    </Box>
  );
}

export default DefinitionDiagram;
