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

import BinaryGatewayNode from './BinaryGatewayNode';
import { toDefinition, toGraph } from '../utils/definitionMapper';
import { enrichDefinition } from '../utils/enrichEdges';
import {
  EditorWorkflowDefinition,
  RFNodeData,
  RFEdgeData
} from '../../../types/workflow';

const nodeTypes = { wfGateway: BinaryGatewayNode };

interface Props {
  initialDefinitionJson?: EditorWorkflowDefinition | null;
  workflowKey: string;
  onSave: (json: EditorWorkflowDefinition) => Promise<void> | void;
}

const WorkflowBuilder: React.FC<Props> = ({ initialDefinitionJson, workflowKey, onSave }) => {
  const [nodes, setNodes] = useState<Node<RFNodeData>[]>([]);
  const [edges, setEdges] = useState<Edge<RFEdgeData>[]>([]);
  const [rfInstance, setRfInstance] = useState<ReactFlowInstance | null>(null);

  useEffect(() => {
    if (!initialDefinitionJson) return;
    const g = toGraph(initialDefinitionJson);
    setNodes(g.nodes);
    requestAnimationFrame(() => setEdges(g.edges));
  }, [initialDefinitionJson]);

  const onConnect = useCallback((params: Connection) => {
    const logical =
      params.sourceHandle === 'out_true' ? 'true'
      : params.sourceHandle === 'out_false' ? 'false'
      : undefined;
    setEdges(eds =>
      addEdge(
        {
          id: `e-${params.sourceHandle ?? 'h'}-${params.source}-${params.target}-${nanoid(4)}`,
          ...params,
          source: params.source,
          target: params.target,
          sourceHandle: params.sourceHandle ?? undefined,
          data: { branch: logical },
          label: logical,
          type: 'straight',
          style: {
            stroke: logical === 'true' ? '#16a34a'
                 : logical === 'false' ? '#dc2626'
                 : '#64748b',
            strokeWidth: 2
          }
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
        for (const c of changes) {
          if (c.type === 'remove') next = next.filter(e => e.id !== c.id);
        }
        return next;
      }),
    []
  );

  const handleSave = useCallback(async () => {
    if (!rfInstance) return;
    const raw = toDefinition(workflowKey, nodes, edges);
    const enriched = enrichDefinition(raw);
    await onSave(enriched);
  }, [rfInstance, nodes, edges, workflowKey, onSave]);

  return (
    <div style={{ width: '100%', height: '100%' }}>
      <div style={{ padding: 4, background: '#0f172a', color: '#e2e8f0', display: 'flex', gap: 8 }}>
        <button onClick={handleSave}>Save</button>
        <span style={{ fontSize: 12, opacity: 0.7 }}>Binary gateway (true / false)</span>
      </div>
      <ReactFlow
        fitView
        nodeTypes={nodeTypes}
        nodes={nodes}
        edges={edges}
        onInit={setRfInstance}
        onConnect={onConnect}
        onNodesChange={onNodesChange}
        onEdgesChange={onEdgesChange}
      >
        <Background />
        <Controls />
      </ReactFlow>
    </div>
  );
};

export default WorkflowBuilder;
