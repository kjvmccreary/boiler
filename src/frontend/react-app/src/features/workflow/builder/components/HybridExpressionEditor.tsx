// Finished H7: dynamic variable context fetch + manual refresh
import React, { useEffect, useRef, useState, useCallback } from 'react';
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
  variableContext?: string[];   // explicit override wins
  variableDeps?: any[];         // external deps that should trigger variable reload (e.g. selected nodes)
  disableDynamicVars?: boolean; // force skip dynamic fetch
}

export const HybridExpressionEditor: React.FC<HybridExpressionEditorProps> = ({
  kind,
  value,
  onChange,
  useMonaco = false,
  semantic = true,
  variableContext,
  variableDeps = [],
  disableDynamicVars = false
}) => {
  const { semanticEnabled, recordSemanticValidation } = useExpressionSettings();
  const effectiveSemantic = semantic && semanticEnabled;

  const [semanticErrors, setSemanticErrors] = useState<string[]>([]);
  const [semanticWarnings, setSemanticWarnings] = useState<string[]>([]);
  const [loadingSemantic, setLoadingSemantic] = useState(false);
  const [semanticVersion, setSemanticVersion] = useState(0);
  const [varsVersion, setVarsVersion] = useState(0);
  const [loadingVars, setLoadingVars] = useState(false);

  const debounceHandleRef = useRef<number | undefined>(undefined);
  const activeRequestVersionRef = useRef<number>(0);

  // ---- Variable Context Handling (Dynamic) ----
  const loadVariables = useCallback(async () => {
    if (variableContext && variableContext.length) {
      setJsonLogicVariables(variableContext);
      setVarsVersion(v => v + 1);
      return;
    }
    if (disableDynamicVars) return;
    setLoadingVars(true);
    try {
      const vars = await workflowService.getExpressionVariables(kind);
      setJsonLogicVariables(vars);
      setVarsVersion(v => v + 1);
    } finally {
      setLoadingVars(false);
    }
  }, [kind, variableContext, disableDynamicVars]);

  useEffect(() => {
    loadVariables();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [kind, ...variableDeps]);

  const manualRefreshVariables = () => {
    loadVariables();
  };

  // ---- Semantic Validation ----
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
      try { JSON.parse(expr); } catch { return; }
      const requestVersion = activeRequestVersionRef.current + 1;
      activeRequestVersionRef.current = requestVersion;
      const started = performance.now();
      setLoadingSemantic(true);
      try {
        const res = await workflowService.validateExpression(kind, expr);
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

  useEffect(() => {
    if (!effectiveSemantic) {
      clearDebounce();
      resetSemanticState();
    } else {
      scheduleSemantic(value);
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [effectiveSemantic]);

  useEffect(() => () => clearDebounce(), []);

  // Pass-through: we piggyback variable refresh (varsVersion) into editor by forcing semantic re-run when set
  useEffect(() => {
    if (effectiveSemantic) {
      scheduleSemantic(value);
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [varsVersion]);

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
        loadingSemantic={loadingSemantic || loadingVars}
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
