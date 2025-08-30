import React, { useEffect } from 'react';
import { Handle, Position, useUpdateNodeInternals } from 'reactflow';
import './BinaryGatewayNode.css';

interface BinaryGatewayNodeProps {
  id?: string;
  data: { label?: string; condition?: string };
  selected?: boolean;
}

const BinaryGatewayNode: React.FC<BinaryGatewayNodeProps> = ({ id, data, selected }) => {
  const update = useUpdateNodeInternals();
  useEffect(() => {
    if (!id) return;
    const t1 = setTimeout(() => update(id), 0);
    const t2 = setTimeout(() => update(id), 40);
    return () => { clearTimeout(t1); clearTimeout(t2); };
  }, [id, update]);

  useEffect(() => {
    // eslint-disable-next-line no-console
    console.log('[BinaryGatewayNode Active] id=', id);
  }, [id]);

  return (
    <div className={`wf-bingw-node ${selected ? 'wf-selected' : ''}`}>
      <div className="wf-bingw-header">
        {data.label || 'Gateway'} <span className="wf-bingw-tag">BIN</span>
      </div>
      <div className="wf-bingw-cond">
        {data.condition ? <code>{data.condition}</code> : <span className="wf-muted">no condition</span>}
      </div>
      <Handle id="in" type="target" position={Position.Left} className="gw-h gw-h-in" />
      <Handle id="out_true" type="source" position={Position.Right} className="gw-h gw-h-true" style={{ top: 38 }} />
      <Handle id="out_false" type="source" position={Position.Right} className="gw-h gw-h-false" style={{ top: 88 }} />
    </div>
  );
};

export default BinaryGatewayNode;
