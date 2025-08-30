import React, { useCallback, useEffect, useState } from 'react';
import ReactFlow, {
  Background,
  Controls,
  addEdge,
  Connection,
  Edge,
  Node,
  ReactFlowInstance
} from 'reactflow';
import { nanoid } from 'nanoid';
import 'reactflow/dist/style.css';

import GatewayNode from './GatewayNode';
import { toDefinition, toGraph } from '../utils/definitionMapper';
import { enrichDefinition } from '../utils/enrichEdges';
import {
  EditorWorkflowDefinition,
  RFNodeData,
  RFEdgeData
} from '../../../types/workflow';

const nodeTypes = { gateway: GatewayNode };

interface Props {
  initialDefinitionJson?: EditorWorkflowDefinition | null;
  workflowKey: string;
  onSave: (json: EditorWorkflowDefinition) => Promise<void> | void; // expects enriched def now
}

const WorkflowBuilder: React.FC<Props> = ({ initialDefinitionJson, workflowKey, onSave }) => {
  const [nodes, setNodes] = useState<Node<RFNodeData>[]>([]);
  const [edges, setEdges] = useState<Edge<RFEdgeData>[]>([]);
  const [rfInstance, setRfInstance] = useState<ReactFlowInstance | null>(null);

  useEffect(() => {
    if (initialDefinitionJson) {
      const g = toGraph(initialDefinitionJson);
      setNodes(g.nodes);
      setEdges(g.edges);
    }
  }, [initialDefinitionJson]);

  const onConnect = useCallback((params: Connection) => {
    setEdges(eds =>
      addEdge(
        {
          id: `e-${params.sourceHandle ?? 'h'}-${params.source}-${params.target}-${nanoid(4)}`,
          ...params,
          source: params.source,
            target: params.target,
          sourceHandle: params.sourceHandle ?? undefined,
          data: { fromHandle: params.sourceHandle },
          label: params.sourceHandle ?? undefined,
          type: 'default'
        } as Edge<RFEdgeData>,
        eds
      )
    );
  }, []);

  const onNodesChange = useCallback(
    (changes: any) =>
      setNodes(nds =>
        nds.map(n => {
          const change = changes.find((c: any) => c.id === n.id);
          if (!change) return n;
          if (change.type === 'position' && change.position)
            return { ...n, position: change.position };
          return n;
        })
      ),
    []
  );

  const onEdgesChange = useCallback(
    (changes: any) =>
      setEdges(es => {
        let next = [...es];
        changes.forEach((c: any) => {
          if (c.type === 'remove') next = next.filter(e => e.id !== c.id);
        });
        return next;
      }),
    []
  );

  const handleSave = useCallback(async () => {
    if (!rfInstance) return;
    const raw = toDefinition(workflowKey, nodes, edges);
    const enriched = enrichDefinition(raw);
    console.debug('[WF] Builder save enriched edges',
      enriched.edges.map(e => ({ id: e.id, fromHandle: e.fromHandle, label: e.label }))
    );
    await onSave(enriched);
  }, [rfInstance, nodes, edges, workflowKey, onSave]);

  return (
    <div style={{ width: '100%', height: '100%' }}>
      <div style={{ padding: 4, background: '#0f172a', color: '#e2e8f0', display: 'flex', gap: 8 }}>
        <button onClick={handleSave}>Save</button>
        <span style={{ fontSize: 12, opacity: 0.65 }}>Gateway edges retain true / false / else</span>
      </div>
      <ReactFlow
        nodeTypes={nodeTypes}
        nodes={nodes}
        edges={edges}
        onConnect={onConnect}
        onNodesChange={onNodesChange}
        onEdgesChange={onEdgesChange}
        onInit={setRfInstance}
        fitView
      >
        <Background />
        <Controls />
      </ReactFlow>
    </div>
  );
};

export default WorkflowBuilder;
