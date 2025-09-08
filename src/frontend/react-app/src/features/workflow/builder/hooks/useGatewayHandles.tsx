import { ReactNode } from 'react';
import { Handle, Position } from 'reactflow';
import { GatewayStrategy } from '../../dsl/dsl.types';

/**
 * Returns the correct handle set for a gateway based on strategy.
 *  - exclusive / conditional: true & false handles (binary decision)
 *  - parallel: single bottom handle (fan-out — C2 will add richer UI)
 */
export function useGatewayHandles(strategy: GatewayStrategy | undefined): ReactNode {
  if (strategy === 'parallel') {
    return (
      <Handle
        type="source"
        position={Position.Bottom}
        id="parallel"
        style={{ background: '#5e35b1', width: 12, height: 12 }}
      />
    );
  }

  return (
    <>
      <Handle
        type="source"
        position={Position.Right}
        id="true"
        style={{ background: '#16a34a', width: 10, height: 10, top: '35%' }}
      />
      <Handle
        type="source"
        position={Position.Right}
        id="false"
        style={{ background: '#dc2626', width: 10, height: 10, top: '70%' }}
      />
    </>
  );
}

export default useGatewayHandles;
