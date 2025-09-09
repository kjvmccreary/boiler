import React, { useEffect, useRef, useState } from 'react';
import ExpressionEditor from '../components/ExpressionEditor';
import { MonacoExpressionEditor } from './MonacoExpressionEditor';
import { workflowService } from '@/services/workflow.service';
import { useExpressionSettings } from '../context/ExpressionSettingsContext';
import { setJsonLogicVariables } from '../monaco/jsonlogicOperators';

interface HybridExpressionEditorProps {
  kind: 'gateway' | 'join';
  value: string;
  onChange: (val: string) => void;
  useMonaco?: boolean;
  semantic?: boolean;
  variableContext?: string[]; // NEW: list of variable names available
}

export const HybridExpressionEditor: React.FC<HybridExpressionEditorProps> = ({
  kind,
  value,
  onChange,
  useMonaco = false,
  semantic = true,
  variableContext
}) => {
  const { semanticEnabled, recordSemanticValidation } = useExpressionSettings();
  const effectiveSemantic = semantic && semanticEnabled;

  // Update Monaco variable context (global provider) when list changes
  useEffect(() => {
    if (variableContext && variableContext.length) {
      setJsonLogicVariables(variableContext);
    }
  }, [variableContext]);

  const [semanticErrors, setSemanticErrors] = useState<string[]>([]);
  const [semanticWarnings, setSemanticWarnings] = useState<string[]>([]);
  const [loadingSemantic, setLoadingSemantic] = useState(false);
  const [semanticVersion, setSemanticVersion] = useState(0);
  const debounceHandleRef = useRef<number | undefined>(undefined);
  const activeRequestVersionRef = useRef<number>(0);

  const clearDebounce = () => {
    if (debounceHandleRef.current) {
      clearTimeout(debounceHandleRef.current);
      debounceHandleRef.current = undefined;
    }
  };

  const resetSemanticState = () => {
    setSemanticErrors([]);
    setSemanticWarnings([]);
    setLoadingSemantic(false);
  };

  const scheduleSemantic = (expr: string) => {
    if (!effectiveSemantic) {
      clearDebounce();
      resetSemanticState();
      return;
    }
    clearDebounce();
    debounceHandleRef.current = window.setTimeout(async () => {
      // Local JSON validity quick check
      try { JSON.parse(expr); } catch { return; }

      // Version increment
      const requestVersion = activeRequestVersionRef.current + 1;
      activeRequestVersionRef.current = requestVersion;
      const started = performance.now();

      setLoadingSemantic(true);
      try {
        const res = await workflowService.validateExpression(kind, expr);
        // Guard: ignore stale responses
        if (requestVersion !== activeRequestVersionRef.current) return;

        setSemanticErrors(res.errors ?? []);
        setSemanticWarnings(res.warnings ?? []);
        setSemanticVersion(requestVersion);
        recordSemanticValidation(
          Math.round(performance.now() - started),
          res.success,
          res.errors?.length ?? 0,
          res.warnings?.length ?? 0
        );
      } finally {
        if (requestVersion === activeRequestVersionRef.current) {
          setLoadingSemantic(false);
        }
      }
    }, 500);
  };

  // If preference flips OFF, immediately clear state
  useEffect(() => {
    if (!effectiveSemantic) {
      clearDebounce();
      resetSemanticState();
    } else {
      // When turning back on, trigger validation for current value
      scheduleSemantic(value);
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [effectiveSemantic]);

  useEffect(() => () => clearDebounce(), []);

  if (useMonaco) {
    return (
      <MonacoExpressionEditor
        kind={kind}
        value={value}
        onChange={val => {
          onChange(val);
          scheduleSemantic(val);
        }}
        onSemanticValidate={scheduleSemantic}
        semanticErrors={semanticErrors}
        semanticWarnings={semanticWarnings}
        loadingSemantic={loadingSemantic}
        semantic={effectiveSemantic}
        semanticVersion={semanticVersion}
      />
    );
  }

  return (
    <ExpressionEditor
      kind={kind}
      value={value}
      onChange={val => {
        onChange(val);
        scheduleSemantic(val);
      }}
      onValidityChange={() => void 0}
    />
  );
};
