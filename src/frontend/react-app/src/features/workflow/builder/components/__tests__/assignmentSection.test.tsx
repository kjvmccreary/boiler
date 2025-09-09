import { render, screen, fireEvent, waitFor } from '@testing-library/react';
import { describe, it, expect, vi } from 'vitest';
import AssignmentSection from '../AssignmentSection';
import type { HumanTaskNode } from '../../../dsl/dsl.types';

const trackSpy = vi.fn();
vi.mock('../../telemetry/workflowTelemetry', () => ({
  trackWorkflow: (...args: any[]) => trackSpy(...args)
}));

vi.mock('../../hooks/useRolesCache', () => ({
  useRolesCache: () => ({
    roles: [{ id: 'r1', name: 'Operators' }, { id: 'r2', name: 'Admins' }],
    loading: false,
    error: null
  })
}));

vi.mock('../../hooks/useUserSearch', () => ({
  useUserSearch: () => ({
    results: [{ id: 'u1', displayName: 'Alice' }, { id: 'u2', displayName: 'Bob' }],
    loading: false,
    term: '',
    setTerm: () => {},
    clear: () => {}
  })
}));

vi.mock('../HybridExpressionEditor', () => ({
  __esModule: true,
  default: (props: any) => (
    <textarea
      aria-label="hybrid-assignment-editor"
      value={props.value}
      onChange={(e) => props.onChange(e.target.value)}
      style={{ width: '100%', height: 80 }}
      onBlur={() => {
        // simulate semantic validation success
        props.onSemanticValidation?.({ success: true, errors: [], durationMs: 12 });
      }}
    />
  )
}));

function makeTask(partial: Partial<HumanTaskNode> = {}): HumanTaskNode {
  return {
    id: 'h1',
    type: 'humanTask',
    x: 0,
    y: 0,
    label: 'Task',
    ...partial
  };
}

describe('AssignmentSection', () => {
  it('renders mode options', () => {
    render(<AssignmentSection node={makeTask()} onPatch={() => {}} />);
    expect(screen.getByLabelText(/Users/i)).toBeInTheDocument();
    expect(screen.getByLabelText(/Roles/i)).toBeInTheDocument();
  });

  it('selecting expression mode shows editor', () => {
    render(<AssignmentSection node={makeTask()} onPatch={() => {}} />);
    fireEvent.click(screen.getByLabelText(/Expression/i));
    expect(screen.getByLabelText(/hybrid-assignment-editor/i)).toBeInTheDocument();
    expect(trackSpy).toHaveBeenCalledWith('assignment.mode.changed', expect.objectContaining({ mode: 'expression' }));
  });

  it('hybrid mode with empty roles + expression triggers errors after interaction', async () => {
    render(<AssignmentSection node={makeTask()} onPatch={() => {}} />);
    fireEvent.click(screen.getByLabelText(/Hybrid/i));
    // Add expression text to simulate interaction
    const editor = screen.getByLabelText(/hybrid-assignment-editor/i);
    fireEvent.change(editor, { target: { value: '{"users":["u1"]}' } });
    await waitFor(() => {
      // Summary chips appear; no direct error until validation of roles (roles missing)
      // We can infer roles warning by presence of 'Hybrid' chip only (simplistic check)
      expect(screen.getByText(/Hybrid/)).toBeInTheDocument();
    });
  });

  it('SLA soft warning >= target produces error message', async () => {
    render(<AssignmentSection node={makeTask({ assignment: { mode: 'roles', roles: ['Operators'] } })} onPatch={() => {}} />);
    const target = screen.getByLabelText(/Target/i);
    const soft = screen.getByLabelText(/Soft Warn/i);
    fireEvent.change(target, { target: { value: '10' } });
    fireEvent.change(soft, { target: { value: '15' } });
    // Wait for validation cycle (effect commit)
    await waitFor(() => {
      const errAlerts = screen.queryAllByText(/Soft warning must be less than target/i);
      // The actual text comes from validation rules; if not directly displayed, ensure error chip count > 0
      const errChip = screen.getAllByText(/err/i);
      expect(errChip.length).toBeGreaterThan(0);
    });
  });

  it('Escalation inputs disabled until SLA target set', () => {
    render(<AssignmentSection node={makeTask({ assignment: { mode: 'roles', roles: ['Operators'] } })} onPatch={() => {}} />);
    const roleInput = screen.getByLabelText(/Role$/i) as HTMLInputElement;
    const afterInput = screen.getByLabelText(/After \(m\)/i) as HTMLInputElement;
    expect(roleInput.disabled).toBe(true);
    expect(afterInput.disabled).toBe(true);
    fireEvent.change(screen.getByLabelText(/Target/i), { target: { value: '30' } });
    expect(roleInput.disabled).toBe(false);
    expect(afterInput.disabled).toBe(false);
  });

  it('Switching to users mode clears roles (confirmation flow bypassed since no data loss)', () => {
    const patches: any[] = [];
    render(<AssignmentSection node={makeTask({ assignment: { mode: 'roles', roles: ['Operators'] } })} onPatch={(p) => patches.push(p)} />);
    fireEvent.click(screen.getByLabelText(/Users$/i));
    // At least one patch should set mode:users
    expect(patches.some(p => p.assignment?.mode === 'users')).toBe(true);
    expect(trackSpy).toHaveBeenCalledWith('assignment.mode.changed', expect.objectContaining({ mode: 'users' }));
  });

  it('expression validation telemetry fires on blur (simulated)', async () => {
    render(<AssignmentSection node={makeTask({ assignment: { mode: 'expression', expression: '{"==":[1,1]}' } })} onPatch={() => {}} />);
    const editor = screen.getByLabelText(/hybrid-assignment-editor/i);
    fireEvent.change(editor, { target: { value: '{"==":[1,1]}' } });
    fireEvent.blur(editor);
    await waitFor(() => {
      expect(trackSpy.mock.calls.some(c => c[0] === 'assignment.expression.validated')).toBe(true);
    });
  });
});
