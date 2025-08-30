import { Edge, Node } from 'reactflow';

export interface WFNodeData {
  label?: string;
  dueInMinutes?: number;
  assigneeRoles?: string[];
  condition?: string;          // for gateway
  action?: any;
}

export interface WFDefinitionEdge {
  id: string;
  from: string;
  to: string;
  fromHandle?: string | null;
  label?: string | null;
}

export interface WFDefinitionNode {
  id: string;
  type: string;
  label?: string;
  x: number;
  y: number;
  // Additional per-node properties (kept generic)
  [key: string]: any;
}

export interface WFDefinitionJson {
  key?: string;
  nodes: WFDefinitionNode[];
  edges: WFDefinitionEdge[];
  // other definition metadata as needed
  [key: string]: any;
}

export type RFNode = Node<WFNodeData>;
export type RFEdge = Edge<{ fromHandle?: string | null }>;
