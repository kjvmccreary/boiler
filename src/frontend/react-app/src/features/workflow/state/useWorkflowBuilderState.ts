import { useCallback, useState } from 'react';
import {
  Edge,
  Node,
  applyNodeChanges,
  applyEdgeChanges,
  NodeChange,
  EdgeChange,
  Connection
} from 'reactflow';
import {
  DslDefinition,
  DslNode,
  DslEdge
} from '../dsl/dsl.types';

export interface WorkflowBuilderState {
  nodes: Node[];
  edges: Edge[];
  setNodes: React.Dispatch<React.SetStateAction<Node[]>>;
  setEdges: React.Dispatch<React.SetStateAction<Edge[]>>;
  onNodesChange: (changes: NodeChange[]) => void;
  onEdgesChange: (changes: EdgeChange[]) => void;
  onConnect: (c: Connection) => void;
  activeNodeId?: string;
  setActiveNodeId: (id?: string) => void;
  updateNodeProperties: (id: string, patch: Record<string, any>) => void;
  serialize: () => DslDefinition;
}

export function useWorkflowBuilderState(): WorkflowBuilderState {
  const [nodes, setNodes] = useState<Node[]>([]);
  const [edges, setEdges] = useState<Edge[]>([]);
  const [activeNodeId, setActiveNodeId] = useState<string | undefined>();

  const onNodesChange = useCallback(
    (changes: NodeChange[]) => setNodes(ns => applyNodeChanges(changes, ns)),
    []
  );

  const onEdgesChange = useCallback(
    (changes: EdgeChange[]) => setEdges(es => applyEdgeChanges(changes, es)),
    []
  );

  const onConnect = useCallback(
    (c: Connection) =>
      setEdges(es => {
        const id = `${c.source}-${c.target}-${crypto.randomUUID().slice(0, 8)}`;
        return [
          ...es,
          {
            id,
            source: c.source!,
            target: c.target!,
            label: '', // keep a simple string label to avoid downstream typing issues
            data: {}
          }
        ];
      }),
    []
  );

  const updateNodeProperties = useCallback((id: string, patch: Record<string, any>) => {
    setNodes(ns =>
      ns.map(n => {
        if (n.id !== id) return n;
        const prevProps = (n.data?.properties ?? {});
        const merged = { ...prevProps, ...patch };
        Object.keys(merged).forEach(k => {
          if (merged[k] === undefined) delete merged[k];
        });
        return {
          ...n,
          data: {
            ...n.data,
            properties: merged
          }
        };
      })
    );
  }, []);

  const serialize = useCallback((): DslDefinition => {
    const dslNodes: DslNode[] = nodes.map(n => ({
      id: n.id,
      type: n.type as any,
      label: n.data?.label || n.data?.properties?.label || n.id,
      x: n.position.x,
      y: n.position.y,
      properties: n.data?.properties || {}
    }));

    // Coerce any ReactNode label into a string (DslEdge.label expects string|undefined)
    const dslEdges: DslEdge[] = edges.map(e => {
      let label: string | undefined;
      if (typeof e.label === 'string') {
        label = e.label;
      } else if (typeof e.label === 'number') {
        label = String(e.label);
      } else if (e.data && typeof (e.data as any).label === 'string') {
        // fallback if stored in data
        label = (e.data as any).label;
      } else {
        label = undefined;
      }
      return {
        id: e.id,
        from: e.source,
        to: e.target,
        label
      };
    });

    return {
      key: 'TEMP-KEY', // TODO: supply real key from surrounding context
      nodes: dslNodes,
      edges: dslEdges
    };
  }, [nodes, edges]);

  return {
    nodes,
    edges,
    setNodes,
    setEdges,
    onNodesChange,
    onEdgesChange,
    onConnect,
    activeNodeId,
    setActiveNodeId,
    updateNodeProperties,
    serialize
  };
}
