import React, { useEffect, useState, useCallback } from 'react';
import { BuilderCanvas } from '../builder/BuilderCanvas';
import type { Node, Edge } from 'reactflow';
import { toGraph, toDefinition } from '../utils/definitionMapper';
import { enrichDefinition } from '../utils/enrichEdges';
import type { EditorWorkflowDefinition } from '../../../types/workflow';

interface Props {
  initialDefinitionJson?: EditorWorkflowDefinition | null; // parsed JSON object of saved workflow
  workflowKey: string;
  onSave: (def: EditorWorkflowDefinition) => Promise<void> | void;
}

function rehydrateEdges(edges: Edge[]): Edge[] {
  return edges.map(e => {
    if (e.sourceHandle === 'true' || e.sourceHandle === 'false') return e;
    const h =
      (e as any).data?.fromHandle ||
      (typeof e.label === 'string' ? e.label : undefined);
    if (h === 'true' || h === 'false') {
      return {
        ...e,
        sourceHandle: h,
        label: h,
        data: { ...(e.data || {}), fromHandle: h }
      };
    }
    return e;
  });
}

export const WorkflowBuilder: React.FC<Props> = ({
  initialDefinitionJson,
  workflowKey,
  onSave
}) => {
  const [nodes, setNodes] = useState<Node[]>([]);
  const [edges, setEdges] = useState<Edge[]>([]);

  // Load + rehydrate on mount / change
  useEffect(() => {
    if (!initialDefinitionJson) return;
    const g = toGraph(initialDefinitionJson);
    setNodes(g.nodes);
    setEdges(rehydrateEdges(g.edges));
  }, [initialDefinitionJson]);

  const handleSave = useCallback(() => {
    const def = toDefinition(workflowKey, nodes, edges);
    const enriched = enrichDefinition(def); // ensure branch labels still enforced
    onSave(enriched);
  }, [workflowKey, nodes, edges, onSave]);

  return (
    <div style={{ display: 'flex', height: '100%', width: '100%' }}>
      <div style={{ flex: 1, position: 'relative' }}>
        <BuilderCanvas
          nodes={nodes}
          edges={edges}
          onNodesChange={changes => setNodes(chs => [...chs])} // delegate to internal apply in real code
          onEdgesChange={changes => setEdges(chs => [...chs])}
          onNodeClick={() => { }}
          setNodes={setNodes}
          setEdges={setEdges}
        />
        <div style={{ position: 'absolute', top: 8, left: 8, zIndex: 10 }}>
          <button onClick={handleSave}>Save</button>
        </div>
      </div>
    </div>
  );
};

export default WorkflowBuilder;
