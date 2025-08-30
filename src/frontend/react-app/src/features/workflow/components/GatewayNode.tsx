import React from 'react';
import { Handle, Position } from 'reactflow';
import './GatewayNode.css';

const GatewayNode: React.FC<{ data: any; selected?: boolean }> = ({ data, selected }) => {
  return (
    <div className={`wf-gateway-node ${selected ? 'wf-selected' : ''}`}>
      <div className="wf-gateway-header">{data.label || 'Gateway'}</div>
      <div className="wf-gateway-cond">
        {data.condition ? <code>{data.condition}</code> : <span className="wf-muted">no condition</span>}
      </div>

      <Handle id="in" type="target" position={Position.Left} style={{ background: '#64748b' }} />

      <Handle
        id="true"
        type="source"
        position={Position.Right}
        style={{ top: 20, background: '#16a34a' }}
        title="True branch"
      />
      <Handle
        id="false"
        type="source"
        position={Position.Right}
        style={{ top: 54, background: '#dc2626' }}
        title="False branch"
      />
      <Handle
        id="else"
        type="source"
        position={Position.Bottom}
        style={{ background: '#f97316' }}
        title="Else / fallback"
      />
    </div>
  );
};

export default GatewayNode;
