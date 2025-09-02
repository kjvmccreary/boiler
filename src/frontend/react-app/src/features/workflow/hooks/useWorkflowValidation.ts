import { useState } from 'react';
import { workflowService, GraphValidationResult } from '@/services/workflow.service';

export function useWorkflowValidation() {
  const [loading, setLoading] = useState(false);
  const [result, setResult] = useState<GraphValidationResult | null>(null);

  const validateJson = async (json: string) => {
    setLoading(true);
    try {
      const vr = await workflowService.validateDefinitionJson(json);
      setResult(vr);
      return vr;
    } finally {
      setLoading(false);
    }
  };

  const validateById = async (id: number) => {
    setLoading(true);
    try {
      const vr = await workflowService.validateDefinitionById(id);
      setResult(vr);
      return vr;
    } finally {
      setLoading(false);
    }
  };

  return { loading, result, validateJson, validateById, clear: () => setResult(null) };
}
