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
import { PropertyPanel } from './panels/PropertyPanel';
import { ExpressionSettingsProvider } from './context/ExpressionSettingsContext';
import { SemanticValidationToggle } from './components/SemanticValidationToggle';
import { ThemeSelector } from './components/ThemeSelector';

import type {
  DslNode,
  NodeType,
  StartNode,
  EndNode,
  HumanTaskNode,
  AutomaticNode,
  GatewayNode,
  TimerNode,
  JoinNode
} from '../dsl/dsl.types';

const nodeTypes = {
  start: WorkflowNode,
  end: WorkflowNode,
  humanTask: WorkflowNode,
  automatic: WorkflowNode,
  gateway: WorkflowNode,
  timer: WorkflowNode,
  join: WorkflowNode
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
  const [activeNode, setActiveNode] = useState<Node | undefined>();

  const handleInit = useCallback((inst: ReactFlowInstance) => {
    setRfInstance(inst);
    (window as any).__RF = inst;
    console.log('[RF][Init] instance exposed as window.__RF'); // eslint-disable-line
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
          case 'gateway':
            return {
              ...baseNode,
              type: 'gateway',
              label: 'Gateway',
              strategy: 'exclusive'
            } as GatewayNode;
          case 'timer': return { ...baseNode, type: 'timer', label: 'Timer', delayMinutes: 5 } as TimerNode;
          case 'join':
            return {
              ...baseNode,
              type: 'join',
              label: 'Join',
              mode: 'all',
              cancelRemaining: false
            } as JoinNode;
          default: return { ...baseNode, type: 'start', label: 'Unknown' } as StartNode; // fixed baseBaseNode -> baseNode
        }
      };

      const newNode: Node = {
        id: newNodeId,
        type,
        position,
        data: getNodeData(type)
      };

      setNodes(nds => nds.concat(newNode));
      setActiveNode(newNode);
    },
    [screenToFlowPosition, setNodes]
  );

  const computeParallelBranchLabel = (sourceId: string): string => {
    const existing = edges.filter(e => e.source === sourceId);
    const index = existing.length + 1;
    return `b${index}`;
  };

  const internalHandleConnect = useCallback(
    (params: Connection) => {
      const sourceNode = nodes.find(n => n.id === params.source);
      let branch: string | undefined =
        params.sourceHandle === 'true' || params.sourceHandle === 'false'
          ? params.sourceHandle
          : undefined;

      let style: Edge['style'] | undefined;
      let label: string | undefined = branch;

      const isParallel = !!sourceNode && (sourceNode.data as any)?.strategy === 'parallel';

      if (isParallel) {
        label = computeParallelBranchLabel(sourceNode!.id);
        style = {
          strokeDasharray: '4 2',
          stroke: '#5e35b1',
          strokeWidth: 2
        };
      }

      setEdges(prev =>
        addEdge(
          {
            ...params,
            id: `e-${params.source}-${params.sourceHandle ?? label ?? 'h'}-${params.target}-${Date.now()}`,
            source: params.source!,
            target: params.target!,
            sourceHandle: params.sourceHandle ?? undefined,
            label,
            data: { fromHandle: branch, parallel: isParallel },
            type: 'default',
            style
          } as Edge,
          prev
        )
      );
    },
    [setEdges, nodes, edges]
  );

  const effectiveConnect = onConnect ?? internalHandleConnect;

  useEffect(() => {
    const gatewayEdgeDebug = edges
      .filter(e =>
        nodes.some(n => n.id === e.source && ((n as any).data?.type === 'gateway' || n.type === 'gateway'))
      );
    if (gatewayEdgeDebug.length) {
      console.log('[RF][GatewayEdges]', gatewayEdgeDebug.map(e => ({
        id: e.id,
        source: e.source,
        target: e.target,
        sourceHandle: e.sourceHandle,
        label: e.label,
        data: e.data
      }))); // eslint-disable-line
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
          if (activeNode && ids.includes(activeNode.id)) setActiveNode(undefined);
        }
        if (selectedEdges.length > 0) {
          const ids = selectedEdges.map(e => e.id);
          setEdges(eds => eds.filter(e => !ids.includes(e.id)));
        }
      }
    },
    [nodes, edges, setNodes, setEdges, activeNode]
  );

  const handleNodeClick = useCallback((evt: React.MouseEvent, node: Node) => {
    onNodeClick(evt, node);
    setActiveNode(node);
  }, [onNodeClick]);

  const updateNode = useCallback((id: string, patch: Record<string, any>) => {
    setNodes(ns =>
      ns.map(n => {
        if (n.id !== id) return n;
        const orig = n.data || {};
        const merged = { ...orig, ...patch };
        return { ...n, data: merged };
      })
    );
    if (activeNode?.id === id) {
      setActiveNode(prev => prev ? { ...prev, data: { ...prev.data, ...patch } } : prev);
    }
  }, [setNodes, activeNode]);

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
        onNodeClick={handleNodeClick}
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

      <PropertyPanel
        open={!!activeNode}
        node={activeNode}
        edges={edges}
        onClose={() => setActiveNode(undefined)}
        updateNode={updateNode}
      />
    </Box>
  );
}

export function BuilderCanvas(props: BuilderCanvasProps) {
  return (
    <ExpressionSettingsProvider>
      <ReactFlowProvider>
        <Box sx={{ position: 'relative', width: '100%', height: '100%' }}>
          <BuilderCanvasInner {...props} />
          <Box
            sx={{
              position: 'absolute',
              top: 8,
              right: 8,
              zIndex: 10,
              display: 'flex',
              flexDirection: 'column',
              gap: 1,
              alignItems: 'flex-end'
            }}
          >
            <ThemeSelector />
            <SemanticValidationToggle />
          </Box>
        </Box>
      </ReactFlowProvider>
    </ExpressionSettingsProvider>
  );
}

export default BuilderCanvas;
