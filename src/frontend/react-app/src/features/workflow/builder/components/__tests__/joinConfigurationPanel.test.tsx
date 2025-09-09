import { render, screen, fireEvent } from '@testing-library/react';
import { describe, it, expect } from 'vitest';
import JoinConfigurationPanel from '../JoinConfigurationPanel';
import type { JoinNode } from '../../../dsl/dsl.types';
import { vi } from 'vitest';

const trackSpy = vi.fn();
vi.mock('../../telemetry/workflowTelemetry', () => ({
  trackWorkflow: (...args: any[]) => trackSpy(...args)
}));

function makeJoin(partial: Partial<JoinNode> = {}): JoinNode {
  return {
    id: 'j1',
    type: 'join',
    x: 0,
    y: 0,
    label: 'Join',
    mode: 'all',
    ...partial
  };
}

describe('JoinConfigurationPanel', () => {
  it('renders default mode description', () => {
    render(<JoinConfigurationPanel node={makeJoin()} onChange={() => {}} />);
    expect(screen.getByText(/Wait for every incoming branch/i)).toBeInTheDocument();
  });

  it('switches to count mode and shows threshold input', () => {
    const changes: any[] = [];
    render(<JoinConfigurationPanel node={makeJoin()} onChange={(p) => changes.push(p)} />);
    fireEvent.mouseDown(screen.getByLabelText(/Join Mode/i));
    fireEvent.click(screen.getByRole('option', { name: /Count/i }));
    expect(screen.getByLabelText(/Threshold Count/i)).toBeInTheDocument();
    expect(changes.some(c => c.mode === 'count')).toBe(true);
    expect(trackSpy).toHaveBeenCalledWith('join.mode.changed', expect.objectContaining({ from: 'all', to: 'count' }));
  });

  it('switches to expression mode and validates empty expression', () => {
    render(<JoinConfigurationPanel node={makeJoin()} onChange={() => {}} />);
    fireEvent.mouseDown(screen.getByLabelText(/Join Mode/i));
    fireEvent.click(screen.getByRole('option', { name: /Expression/i }));
    expect(screen.getByText(/Expression required/i)).toBeInTheDocument();
  });

  it('expression mode invalid JSON shows Invalid JSON', () => {
    const changes: any[] = [];
    render(<JoinConfigurationPanel node={makeJoin()} onChange={(p) => changes.push(p)} />);
    fireEvent.mouseDown(screen.getByLabelText(/Join Mode/i));
    fireEvent.click(screen.getByRole('option', { name: /Expression/i }));
    const area = screen.getByLabelText(/JsonLogic Expression/i);
    fireEvent.change(area, { target: { value: '{bad' } });
    expect(screen.getByText(/Invalid JSON/i)).toBeInTheDocument();
  });

  it('quorum mode shows percent helper and handles input', () => {
    const changes: any[] = [];
    render(<JoinConfigurationPanel node={makeJoin()} onChange={(p) => changes.push(p)} />);
    fireEvent.mouseDown(screen.getByLabelText(/Join Mode/i));
    fireEvent.click(screen.getByRole('option', { name: /Quorum/i }));
    const percent = screen.getByLabelText(/Threshold Percent/i);
    fireEvent.change(percent, { target: { value: '60' } });
    expect((percent as HTMLInputElement).value).toBe('60');
    expect(changes.some(c => c.mode === 'quorum')).toBe(true);
    expect(trackSpy.mock.calls.some(c => c[0] === 'join.mode.changed' && c[1].to === 'quorum')).toBe(true);
  });
});
