import { render, screen, fireEvent } from '@testing-library/react';
import { describe, it, expect, vi } from 'vitest';
import ActionSection from '../ActionSection';
import type { AutomaticNode } from '../../../dsl/dsl.types';

const trackSpy = vi.fn();
vi.mock('../../telemetry/workflowTelemetry', () => ({
  trackWorkflow: (...args: any[]) => trackSpy(...args)
}));

vi.mock('../HybridExpressionEditor', () => ({
  __esModule: true,
  default: (props: any) => (
    <textarea
      aria-label="hybrid-auto-editor"
      value={props.value}
      onChange={(e) => props.onChange(e.target.value)}
      style={{ width: '100%', height: 60 }}
    />
  )
}));

function makeAuto(partial: Partial<AutomaticNode> = {}): AutomaticNode {
  return {
    id: 'auto1',
    type: 'automatic',
    x: 0,
    y: 0,
    label: 'Auto',
    ...partial
  };
}

describe('ActionSection', () => {
  it('defaults to noop kind', () => {
    render(<ActionSection node={makeAuto()} onPatch={() => {}} />);
    expect(screen.getByText(/Noop action performs no external call/i)).toBeInTheDocument();
  });

  it('switches to webhook and validates URL scheme', () => {
    const patches: any[] = [];
    render(<ActionSection node={makeAuto()} onPatch={(p) => patches.push(p)} />);
    fireEvent.click(screen.getByLabelText(/Webhook/i));
    const url = screen.getByLabelText(/^URL$/i);
    fireEvent.change(url, { target: { value: 'ftp://bad' } });
    fireEvent.blur(url);
    expect(screen.getByText(/Must start with http/)).toBeInTheDocument();
    fireEvent.change(url, { target: { value: 'https://good.com' } });
    fireEvent.blur(url);
    expect(screen.queryByText(/Must start with http/)).toBeNull();
    expect(patches.some(p => p.action?.url === 'https://good.com')).toBe(true);
    // Kind change and URL blur telemetry
    expect(trackSpy.mock.calls.some(c => c[0] === 'automatic.action.changed' && c[1].kind === 'webhook')).toBe(true);
    expect(trackSpy.mock.calls.some(c => c[0] === 'automatic.webhook.url.blur')).toBe(true);
  });

  it('enables retry policy when maxAttempts >1', () => {
    render(<ActionSection node={makeAuto()} onPatch={() => {}} />);
    fireEvent.click(screen.getByLabelText(/Webhook/i));
    const maxAttempts = screen.getByLabelText(/Max Attempts/i);
    fireEvent.change(maxAttempts, { target: { value: '3' } });
    const backoff = screen.getByLabelText(/Backoff \(s\)/i) as HTMLInputElement;
    expect(backoff.disabled).toBe(false);
    expect(trackSpy.mock.calls.some(c => c[0] === 'automatic.retry.configured')).toBe(true);
  });

  it('adds header row and shows hdrs summary chip', () => {
    render(<ActionSection node={makeAuto()} onPatch={() => {}} />);
    fireEvent.click(screen.getByLabelText(/Webhook/i));
    // Show headers panel
    fireEvent.click(screen.getByText(/Headers/i));
    // Click add header icon (first button with svg inside headers section)
    const addBtn = screen.getAllByRole('button').find(b => b.querySelector('svg'));
    if (addBtn) fireEvent.click(addBtn);
    // Enter header key/value
    const inputs = screen.getAllByPlaceholderText(/Key|Value/);
    if (inputs.length >= 2) {
      fireEvent.change(inputs[0], { target: { value: 'X-Test' } });
      fireEvent.change(inputs[1], { target: { value: '123' } });
    }
    // Summary chip should include hdrs:1
    expect(screen.getByText(/hdrs:1/i)).toBeInTheDocument();
  });

  it('body size warning appears for >10KB body', () => {
    render(<ActionSection node={makeAuto()} onPatch={() => {}} />);
    fireEvent.click(screen.getByLabelText(/Webhook/i));
    const editor = screen.getByLabelText(/hybrid-auto-editor/i);
    fireEvent.change(editor, { target: { value: 'a'.repeat(11000) } });
    expect(screen.getByText(/Body size > 10KB/i)).toBeInTheDocument();
  });

  it('discard confirmation when switching webhook -> noop with data', () => {
    render(<ActionSection node={makeAuto()} onPatch={() => {}} />);
    fireEvent.click(screen.getByLabelText(/Webhook/i));
    const url = screen.getByLabelText(/^URL$/i);
    fireEvent.change(url, { target: { value: 'https://example.com' } });
    fireEvent.click(screen.getByLabelText(/Noop/i));
    expect(screen.getByText(/discard current webhook configuration/i)).toBeInTheDocument();
    fireEvent.click(screen.getByText(/Discard & Switch/i));
    expect(screen.getByText(/Noop action performs no external call/i)).toBeInTheDocument();
    expect(trackSpy.mock.calls.some(c => c[0] === 'automatic.action.discarded')).toBe(true);
  });
});
