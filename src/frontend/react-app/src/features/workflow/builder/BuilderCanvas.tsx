import { useCallback, useRef } from 'react';
import {
  ReactFlow,
  Background,
  Controls,
  MiniMap,
  Node,
  Edge,
  OnNodesChange,
  OnEdgesChange,
  OnConnect,
  ReactFlowProvider,
  useReactFlow,
  BackgroundVariant,
} from 'reactflow';
import { Box } from '@mui/material';
import { WorkflowNode } from './nodes/WorkflowNode';
import type { DslNode, NodeType, StartNode, EndNode, HumanTaskNode, AutomaticNode, GatewayNode, TimerNode } from '../dsl/dsl.types';

// Custom node types for ReactFlow
const nodeTypes = {
  start: WorkflowNode,
  end: WorkflowNode,
  humanTask: WorkflowNode,
  automatic: WorkflowNode,
  gateway: WorkflowNode,
  timer: WorkflowNode,
};

interface BuilderCanvasProps {
  nodes: Node[];
  edges: Edge[];
  onNodesChange: OnNodesChange;
  onEdgesChange: OnEdgesChange;
  onConnect: OnConnect;
  onNodeClick: (event: React.MouseEvent, node: Node) => void;
  setNodes: React.Dispatch<React.SetStateAction<Node[]>>;
  setEdges: React.Dispatch<React.SetStateAction<Edge[]>>;
}

function BuilderCanvasInner({
  nodes,
  edges,
  onNodesChange,
  onEdgesChange,
  onConnect,
  onNodeClick,
  setNodes,
  setEdges,
}: BuilderCanvasProps) {
  const reactFlowWrapper = useRef<HTMLDivElement>(null);
  const { screenToFlowPosition } = useReactFlow();

  const onDragOver = useCallback((event: React.DragEvent) => {
    event.preventDefault();
    event.dataTransfer.dropEffect = 'move';
  }, []);

  const onDrop = useCallback(
    (event: React.DragEvent) => {
      event.preventDefault();

      const type = event.dataTransfer.getData('application/reactflow') as NodeType;

      if (typeof type === 'undefined' || !type) {
        return;
      }

      const position = screenToFlowPosition({
        x: event.clientX,
        y: event.clientY,
      });

      const newNodeId = `${type}-${Date.now()}`;
      
      // ✅ FIX: Create strongly typed node data with type assertion
      const getNodeData = (nodeType: NodeType): DslNode => {
        const baseNode = {
          id: newNodeId,
          x: position.x,
          y: position.y,
        };

        switch (nodeType) {
          case 'start':
            return { ...baseNode, type: 'start', label: 'Start' } as StartNode;
          case 'end':
            return { ...baseNode, type: 'end', label: 'End' } as EndNode;
          case 'humanTask':
            return { 
              ...baseNode, 
              type: 'humanTask',
              label: 'Human Task',
              assigneeRoles: [],
              dueInMinutes: undefined,
              formSchema: undefined,
            } as HumanTaskNode;
          case 'automatic':
            return { 
              ...baseNode, 
              type: 'automatic',
              label: 'Automatic Task',
              action: { kind: 'noop' },
            } as AutomaticNode;
          case 'gateway':
            return { 
              ...baseNode, 
              type: 'gateway',
              label: 'Gateway',
              condition: '{"==": [{"var": "approved"}, true]}',
            } as GatewayNode;
          case 'timer':
            return { 
              ...baseNode, 
              type: 'timer',
              label: 'Timer',
              delayMinutes: 5,
            } as TimerNode;
          default:
            return { ...baseNode, type: 'start', label: 'Unknown' } as StartNode;
        }
      };

      const newNode: Node = {
        id: newNodeId,
        type,
        position,
        data: getNodeData(type),
      };

      setNodes((nds) => nds.concat(newNode));
    },
    [screenToFlowPosition, setNodes]
  );

  const onKeyDown = useCallback(
    (event: React.KeyboardEvent) => {
      if (event.key === 'Delete' || event.key === 'Backspace') {
        const selectedNodes = nodes.filter((node) => node.selected);
        const selectedEdges = edges.filter((edge) => edge.selected);

        if (selectedNodes.length > 0) {
          const nodeIds = selectedNodes.map((node) => node.id);
          setNodes((nds) => nds.filter((node) => !nodeIds.includes(node.id)));
          setEdges((eds) => eds.filter((edge) => 
            !nodeIds.includes(edge.source) && !nodeIds.includes(edge.target)
          ));
        }

        if (selectedEdges.length > 0) {
          const edgeIds = selectedEdges.map((edge) => edge.id);
          setEdges((eds) => eds.filter((edge) => !edgeIds.includes(edge.id)));
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
        onConnect={onConnect}
        onNodeClick={onNodeClick}
        onDrop={onDrop}
        onDragOver={onDragOver}
        nodeTypes={nodeTypes}
        fitView
        attributionPosition="bottom-left"
        deleteKeyCode={['Delete', 'Backspace']}
        multiSelectionKeyCode={['Meta', 'Ctrl']}
        selectionKeyCode={['Shift']}
      >
        <Background variant={BackgroundVariant.Dots} gap={12} size={1} />
        <Controls />
        <MiniMap
          nodeStrokeColor={(n) => {
            if (n.type === 'start') return '#0041d0';
            if (n.type === 'end') return '#ff0072';
            return '#eee';
          }}
          nodeColor={(n) => {
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
