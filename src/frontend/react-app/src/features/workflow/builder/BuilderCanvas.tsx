import { useCallback, useEffect, useRef, useState } from 'react';
import {
  ReactFlow,
  Background,
  Controls,
  MiniMap,
  Node,
  Edge,
  OnNodesChange,
  OnEdgesChange,
  ReactFlowProvider,
  useReactFlow,
  BackgroundVariant,
  addEdge,
  Connection,
  OnConnect,
  ReactFlowInstance
} from 'reactflow';
import { Box } from '@mui/material';
import { WorkflowNode } from './nodes/WorkflowNode';
import type {
  DslNode,
  NodeType,
  StartNode,
  EndNode,
  HumanTaskNode,
  AutomaticNode,
  GatewayNode,
  TimerNode
} from '../dsl/dsl.types';

const nodeTypes = {
  start: WorkflowNode,
  end: WorkflowNode,
  humanTask: WorkflowNode,
  automatic: WorkflowNode,
  gateway: WorkflowNode,
  timer: WorkflowNode
};

export interface BuilderCanvasProps {
  nodes: Node[];
  edges: Edge[];
  onNodesChange: OnNodesChange;
  onEdgesChange: OnEdgesChange;
  onNodeClick: (event: React.MouseEvent, node: Node) => void;
  setNodes: React.Dispatch<React.SetStateAction<Node[]>>;
  setEdges: React.Dispatch<React.SetStateAction<Edge[]>>;
  onConnect?: OnConnect;
}

function BuilderCanvasInner({
  nodes,
  edges,
  onNodesChange,
  onEdgesChange,
  onNodeClick,
  setNodes,
  setEdges,
  onConnect
}: BuilderCanvasProps) {
  const reactFlowWrapper = useRef<HTMLDivElement>(null);
  const { screenToFlowPosition } = useReactFlow();
  const [rfInstance, setRfInstance] = useState<ReactFlowInstance | null>(null);

  // Expose instance globally for console diagnostics
  const handleInit = useCallback((inst: ReactFlowInstance) => {
    setRfInstance(inst);
    (window as any).__RF = inst;
    // eslint-disable-next-line no-console
    console.log('[RF][Init] instance exposed as window.__RF');
  }, []);

  const onDragOver = useCallback((event: React.DragEvent) => {
    event.preventDefault();
    event.dataTransfer.dropEffect = 'move';
  }, []);

  const onDrop = useCallback(
    (event: React.DragEvent) => {
      event.preventDefault();
      const type = event.dataTransfer.getData('application/reactflow') as NodeType;
      if (!type) return;

      const position = screenToFlowPosition({ x: event.clientX, y: event.clientY });
      const newNodeId = `${type}-${Date.now()}`;

      const getNodeData = (nodeType: NodeType): DslNode => {
        const baseNode = { id: newNodeId, x: position.x, y: position.y };
        switch (nodeType) {
          case 'start': return { ...baseNode, type: 'start', label: 'Start' } as StartNode;
          case 'end': return { ...baseNode, type: 'end', label: 'End' } as EndNode;
          case 'humanTask': return { ...baseNode, type: 'humanTask', label: 'Human Task', assigneeRoles: [] } as HumanTaskNode;
          case 'automatic': return { ...baseNode, type: 'automatic', label: 'Automatic Task', action: { kind: 'noop' } } as AutomaticNode;
          case 'gateway': return { ...baseNode, type: 'gateway', label: 'Gateway', condition: '{"==": [{"var": "approved"}, true]}' } as GatewayNode;
          case 'timer': return { ...baseNode, type: 'timer', label: 'Timer', delayMinutes: 5 } as TimerNode;
          default: return { ...baseNode, type: 'start', label: 'Unknown' } as StartNode;
        }
      };

      const newNode: Node = {
        id: newNodeId,
        type,
        position,
        data: getNodeData(type)
      };

      setNodes(nds => nds.concat(newNode));
    },
    [screenToFlowPosition, setNodes]
  );

  const internalHandleConnect = useCallback(
    (params: Connection) => {
      const branch =
        params.sourceHandle === 'true' || params.sourceHandle === 'false'
          ? params.sourceHandle
          : undefined;

      setEdges(prev =>
        addEdge(
          {
            ...params,
            id: `e-${params.source}-${params.sourceHandle ?? 'h'}-${params.target}-${Date.now()}`,
            source: params.source,
            target: params.target,
            sourceHandle: params.sourceHandle ?? undefined,
            label: branch,
            data: { fromHandle: branch },
            type: 'default'
          } as Edge,
          prev
        )
      );
    },
    [setEdges]
  );

  const effectiveConnect = onConnect ?? internalHandleConnect;

  // Debug: log only gateway edges whenever edges change
  useEffect(() => {
    const gatewayEdgeDebug = edges
      .filter(e => nodes.some(n => n.id === e.source && (n as any).data?.type === 'gateway') || nodes.some(n => n.id === e.source && n.type === 'gateway'));
    if (gatewayEdgeDebug.length) {
      // eslint-disable-next-line no-console
      console.log('[RF][GatewayEdges]', gatewayEdgeDebug.map(e => ({
        id: e.id,
        source: e.source,
        target: e.target,
        sourceHandle: e.sourceHandle,
        label: e.label,
        data: e.data
      })));
    }
  }, [edges, nodes]);

  const onKeyDown = useCallback(
    (event: React.KeyboardEvent) => {
      if (event.key === 'Delete' || event.key === 'Backspace') {
        const selectedNodes = nodes.filter(n => n.selected);
        const selectedEdges = edges.filter(e => e.selected);

        if (selectedNodes.length > 0) {
          const ids = selectedNodes.map(n => n.id);
          setNodes(nds => nds.filter(n => !ids.includes(n.id)));
          setEdges(eds => eds.filter(e => !ids.includes(e.source) && !ids.includes(e.target)));
        }
        if (selectedEdges.length > 0) {
          const ids = selectedEdges.map(e => e.id);
          setEdges(eds => eds.filter(e => !ids.includes(e.id)));
        }
      }
    },
    [nodes, edges, setNodes, setEdges]
  );

  return (
    <Box
      ref={reactFlowWrapper}
      sx={{ width: '100%', height: '100%' }}
      onKeyDown={onKeyDown}
      tabIndex={0}
    >
      <ReactFlow
        nodes={nodes}
        edges={edges}
        onNodesChange={onNodesChange}
        onEdgesChange={onEdgesChange}
        onConnect={effectiveConnect}
        onNodeClick={onNodeClick}
        onDrop={onDrop}
        onDragOver={onDragOver}
        nodeTypes={nodeTypes}
        fitView
        onInit={handleInit}
      >
        <Background variant={BackgroundVariant.Dots} gap={12} size={1} />
        <Controls />
        <MiniMap
          nodeStrokeColor={n => {
            if (n.type === 'start') return '#0041d0';
            if (n.type === 'end') return '#ff0072';
            return '#999';
          }}
          nodeColor={n => {
            if (n.type === 'start') return '#4dabf7';
            if (n.type === 'end') return '#ff6b9d';
            return '#fff';
          }}
          nodeBorderRadius={2}
          pannable
          zoomable
          position="bottom-right"
        />
      </ReactFlow>
    </Box>
  );
}

export function BuilderCanvas(props: BuilderCanvasProps) {
  return (
    <ReactFlowProvider>
      <BuilderCanvasInner {...props} />
    </ReactFlowProvider>
  );
}

export default BuilderCanvas;
