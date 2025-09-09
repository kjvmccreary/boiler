import { render, screen, fireEvent, waitFor } from '@testing-library/react';
import { describe, it, expect, vi, beforeEach } from 'vitest';
import DefinitionsPage from '../DefinitionsPage';
import React from 'react';

// Telemetry mock
const trackSpy = vi.fn();
vi.mock('../../../builder/telemetry/workflowTelemetry', () => ({
  trackWorkflow: (...args: any[]) => trackSpy(...args)
}));

// Tenant + auth mocks
vi.mock('@/contexts/TenantContext', () => ({
  useTenant: () => ({ currentTenant: { id: 1, name: 'TestTenant' } })
}));
vi.mock('@/components/authorization/CanAccess', () => ({
  CanAccess: (p: any) => <>{p.children}</>
}));

// workflowService mock
const getDefinitionsMock = vi.fn();
vi.mock('@/services/workflow.service', () => ({
  workflowService: {
    getDefinitions: (...args: any[]) => getDefinitionsMock(...args)
  }
}));

describe('DefinitionsPage Tag Filters', () => {
  beforeEach(() => {
    trackSpy.mockReset();
    getDefinitionsMock.mockReset();
  });

  it('surfaces validation errors from TagFilterValidationError (422)', async () => {
    // First successful load (initial)
    getDefinitionsMock.mockResolvedValueOnce([]);

    // Second call => validation error (simulate 422)
    const err: any = new Error('anyTags: too many tags (max 12)');
    err.name = 'TagFilterValidationError';
    err.errors = ['anyTags: too many tags (max 12)', 'allTags: tag "SuperLongTagNameExceedingLimit" exceeds 40 characters'];
    getDefinitionsMock.mockRejectedValueOnce(err);

    render(<DefinitionsPage />);

    // initial load
    await waitFor(() => expect(getDefinitionsMock).toHaveBeenCalledTimes(1));

    // Enter invalid filters
    fireEvent.change(screen.getByLabelText(/Any Tags/i), { target: { value: 'a,b,c,d,e,f,g,h,i,j,k,l,m' } });
    fireEvent.change(screen.getByLabelText(/All Tags/i), { target: { value: 'SuperLongTagNameExceedingLimit' } });

    fireEvent.click(screen.getByRole('button', { name: /Apply Filters/i }));

    await waitFor(() => {
      expect(screen.getByText(/anyTags: too many tags/i)).toBeInTheDocument();
      expect(screen.getByText(/SuperLongTagNameExceedingLimit/i)).toBeInTheDocument();
    });

    // Telemetry for validation failure expected
    expect(trackSpy.mock.calls.some(c => c[0] === 'tags.filter.validation.failed')).toBe(true);
  });

  it('tracks successful apply', async () => {
    getDefinitionsMock.mockResolvedValueOnce([]); // initial
    getDefinitionsMock.mockResolvedValueOnce([]); // after apply

    render(<DefinitionsPage />);

    await waitFor(() => expect(getDefinitionsMock).toHaveBeenCalledTimes(1));

    fireEvent.change(screen.getByLabelText(/Any Tags/i), { target: { value: 'alpha,beta' } });
    fireEvent.click(screen.getByRole('button', { name: /Apply Filters/i }));

    await waitFor(() => expect(getDefinitionsMock).toHaveBeenCalledTimes(2));
    expect(trackSpy.mock.calls.some(c => c[0] === 'tags.filter.applied')).toBe(true);
  });
});
