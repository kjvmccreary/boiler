/**
 * monacoLoader (Monaco Bundle Optimization PR1)
 * - Single-flight dynamic import of monaco-editor.
 * - Emits telemetry events for load start/complete/failure.
 * - Provides optional idle prefetch hook (no-op once loaded).
 * - Designed for JSON + custom JsonLogic enhancements only (other languages deferred).
 */
import { trackWorkflow } from '@/features/workflow/telemetry/workflowTelemetry';

// Slim-mode: load only core editor API (no bundled default languages).
// We will explicitly register only JSON (and optional JavaScript if ever needed).
let loadPromise: Promise<typeof import('monaco-editor/esm/vs/editor/editor.api')> | null = null;
let monacoInstance: typeof import('monaco-editor/esm/vs/editor/editor.api') | null = null;
let loadError: any = null;
let firstLoadStartTs: number | null = null;
let themeApplied: string | null = null;

// Editor lifecycle tracking (concurrency + model hygiene)
const activeEditors = new Set<string>();
let concurrencyWarned = false;

const MAX_MODELS_BEFORE_CLEAN = 30;
const CLEAN_TARGET_RETAIN = 10;

function cleanModelsIfExcess(monaco: typeof import('monaco-editor/esm/vs/editor/editor.api')) {
  const models = monaco.editor.getModels();
  if (models.length <= MAX_MODELS_BEFORE_CLEAN) return;
  // Dispose oldest (by version id heuristic) except the first CLEAN_TARGET_RETAIN most recently used
  const sorted = [...models].sort((a, b) => b.getVersionId() - a.getVersionId());
  const toDispose = sorted.slice(CLEAN_TARGET_RETAIN);
  for (const m of toDispose) {
    try { m.dispose(); } catch { /* ignore */ }
  }
  trackWorkflow('monaco.models.cleaned', { before: models.length, disposed: toDispose.length });
}

export function registerEditorInstance(id: string) {
  activeEditors.add(id);
  if (activeEditors.size > 3 && !concurrencyWarned) {
    concurrencyWarned = true;
    trackWorkflow('monaco.concurrent', { count: activeEditors.size });
  }
}
export function unregisterEditorInstance(id: string) {
  activeEditors.delete(id);
  if (monacoInstance) {
    cleanModelsIfExcess(monacoInstance);
  }
}

if (typeof window !== 'undefined') {
  window.addEventListener('beforeunload', () => {
    if (monacoInstance) {
      cleanModelsIfExcess(monacoInstance);
    }
  });
}

export interface EnsureMonacoResult {
  monaco: typeof import('monaco-editor/esm/vs/editor/editor.api');
  firstLoad: boolean;
  durationMs: number;
}

/**
 * Dynamically loads monaco (load-on-first-mount).
 * Subsequent calls resolve synchronously with the cached instance.
 */
export function ensureMonaco(): Promise<EnsureMonacoResult> {
  if (monacoInstance) {
    return Promise.resolve({
      monaco: monacoInstance,
      firstLoad: false,
      durationMs: 0
    });
  }
  if (loadError) {
    return Promise.reject(loadError);
  }
  if (!loadPromise) {
    firstLoadStartTs = performance.now();
    trackWorkflow('monaco.load.start', { cold: true, slim: true });
    // IMPORTANT: editor.api DOES NOT auto-load any language contributions or workers.
    // We selectively import JSON language + JSON worker only.
    loadPromise = import('monaco-editor/esm/vs/editor/editor.api')
      .then(async mod => {
        try {
          // JSON language registration (basic tokenization + language service).
          // Some monaco-editor versions do not expose the basic-languages json path under esm;
          // the language service contribution is sufficient to register JSON.
          try {
            await import('monaco-editor/esm/vs/language/json/monaco.contribution');
          } catch (langErr) {
            trackWorkflow('monaco.slim.registration.error', {
              message: 'json.contribution load failed: ' + String((langErr as any)?.message || langErr)
            });
          }
          // Configure custom worker factory (only editor + json)
          // Using Vite ?worker bundling.
          const EditorWorker = (await import('monaco-editor/esm/vs/editor/editor.worker?worker')).default;
          const JsonWorker = (await import('monaco-editor/esm/vs/language/json/json.worker?worker')).default;
          // Attach environment once
          // eslint-disable-next-line @typescript-eslint/ban-ts-comment
          // @ts-ignore
          self.MonacoEnvironment = {
            getWorker(_: any, label: string) {
              if (label === 'json') return new JsonWorker();
              return new EditorWorker();
            }
          };
        } catch (e) {
          // If any slim import fails, we still proceed (Monaco core loaded; language registration may fail)
          trackWorkflow('monaco.slim.registration.error', { message: String((e as any)?.message || e) });
        }
        monacoInstance = mod;
        const duration = performance.now() - (firstLoadStartTs ?? performance.now());
        trackWorkflow('monaco.load.complete', { cold: true, slim: true, durationMs: Math.round(duration) });
        trackWorkflow('monaco.slim.enabled', { jsonOnly: true });
        return mod;
      })
      .catch(err => {
        loadError = err;
        const duration = performance.now() - (firstLoadStartTs ?? performance.now());
        trackWorkflow('monaco.load.failed', {
          durationMs: Math.round(duration),
          message: String(err?.message || err),
          slim: true
        });
        throw err;
      });
  }
  return loadPromise.then(m => ({
    monaco: m,
    firstLoad: true,
    durationMs: Math.round(performance.now() - (firstLoadStartTs ?? performance.now()))
  }));
}

/**
 * Prefetch monaco on idle (only if not yet loaded).
 * Safe to call multiple times.
 */
export function prefetchMonacoOnIdle(delayMs = 2000, reason: 'idle' | 'heuristic' = 'idle') {
  if (monacoInstance || loadPromise) return;
  const runner = () => {
    if (monacoInstance || loadPromise) return;
    trackWorkflow('monaco.prefetch.trigger', { reason });
    ensureMonaco().catch(() => void 0);
  };
  if ('requestIdleCallback' in window) {
    (window as any).requestIdleCallback(() => {
      setTimeout(runner, delayMs);
    });
  } else {
    setTimeout(runner, delayMs);
  }
}

export function applyThemeIfChanged(monaco: typeof import('monaco-editor/esm/vs/editor/editor.api'), themeName: string) {
  if (themeApplied === themeName) return;
  themeApplied = themeName;
  monaco.editor.setTheme(themeName);
}
