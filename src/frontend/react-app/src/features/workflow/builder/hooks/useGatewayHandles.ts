import { Handle, Position } from 'reactflow';
import { GatewayStrategy } from '../../dsl/dsl.types';
import { ReactNode } from 'react';

/**
 * Returns the correct handle set for a gateway based on strategy.
 * For now:
 *  - exclusive / conditional: binary true/false
 *  - parallel: single unlabeled bottom handle (fan-out edges can be added manually)
 * C2 may extend to dynamic branch creation UI or multi vertical handles.
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
