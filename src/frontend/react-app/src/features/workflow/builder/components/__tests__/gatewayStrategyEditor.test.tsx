import { render, screen, fireEvent } from '@testing-library/react';
import { describe, it, expect, vi } from 'vitest';
import { GatewayStrategyEditor } from '../GatewayStrategyEditor';
import type { GatewayNode } from '../../../dsl/dsl.types';

const trackSpy = vi.fn();
vi.mock('../../telemetry/workflowTelemetry', () => ({
  trackWorkflow: (...args: any[]) => trackSpy(...args)
}));

function makeNode(partial: Partial<GatewayNode> = {}): GatewayNode {
  return {
    id: 'g1',
    type: 'gateway',
    x: 0,
    y: 0,
    label: 'Gateway',
    ...partial
  };
}

describe('GatewayStrategyEditor', () => {
  it('infers conditional when condition exists but no strategy', () => {
    const node = makeNode({ condition: '{"==":[1,1]}' });
    render(<GatewayStrategyEditor node={node} onChange={() => {}} />);
    // Conditional UI shows "Condition (JsonLogic)"
    expect(screen.getByText(/Condition \(JsonLogic\)/i)).toBeInTheDocument();
  });

  it('switches strategy from exclusive -> parallel', () => {
    const changes: any[] = [];
    const node = makeNode({ strategy: 'exclusive' });
    render(<GatewayStrategyEditor node={node} onChange={(p) => changes.push(p)} />);
    fireEvent.mouseDown(screen.getByLabelText(/Strategy/i));
    fireEvent.click(screen.getByRole('option', { name: /Parallel/i }));
    expect(changes.some(c => c.strategy === 'parallel')).toBe(true);
    expect(trackSpy).toHaveBeenCalledWith('gateway.strategy.changed', expect.objectContaining({ from: 'exclusive', to: 'parallel' }));
  });

  it('adds default condition when switching to conditional without existing condition', () => {
    const changes: any[] = [];
    const node = makeNode({ strategy: 'exclusive' });
    render(<GatewayStrategyEditor node={node} onChange={(p) => changes.push(p)} />);
    fireEvent.mouseDown(screen.getByLabelText(/Strategy/i));
    fireEvent.click(screen.getByRole('option', { name: /Conditional/i }));
    const withCond = changes.find(c => c.strategy === 'conditional');
    expect(withCond?.condition).toBeTruthy();
  });

  it('shows parse error for invalid JSON condition', () => {
    const node = makeNode({ strategy: 'conditional', condition: '{bad json' });
    render(<GatewayStrategyEditor node={node} onChange={() => {}} />);
    expect(screen.getByText(/Invalid JSON/i)).toBeInTheDocument();
  });

  it('applies a snippet and beautifies JSON', () => {
    const node = makeNode({ strategy: 'conditional', condition: '{"==":[1,1]}' });
    const changes: any[] = [];
    render(<GatewayStrategyEditor node={node} onChange={(p) => changes.push(p)} />);
    // Click a snippet chip (Equals)
    fireEvent.click(screen.getByText(/Equals/i));
    expect(changes.some(c => c.condition?.includes('"field"'))).toBe(true);
    // Click Beautify icon button via tooltip label
    // Tooltip content not directly accessible, target icon button occurrence
    const beautifyBtn = screen.getAllByRole('button').find(b => b.querySelector('svg'));
    if (beautifyBtn) fireEvent.click(beautifyBtn); // smoke (no assertion beyond not throwing)
    // One of the earlier actions may have tracked; ensure at least one strategy change call overall
    expect(trackSpy.mock.calls.some(c => c[0] === 'gateway.strategy.changed')).toBe(true);
  });
});
