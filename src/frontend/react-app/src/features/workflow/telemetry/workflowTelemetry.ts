// Lightweight telemetry dispatcher.
// Replace console.debug with real sink integration when backend telemetry endpoint available.

interface TelemetryEvent {
  ts: string;
  event: string;
  payload?: any;
}

type TelemetrySink = (e: TelemetryEvent) => void;

let sinks: TelemetrySink[] = [];

/**
 * Register an additional sink (e.g., push to buffer, send to backend).
 */
export function registerWorkflowTelemetrySink(sink: TelemetrySink) {
  sinks.push(sink);
}

/**
 * Core tracking function (non-blocking, try/catch safe).
 */
export function trackWorkflow(event: string, payload?: any) {
  const record: TelemetryEvent = {
    ts: new Date().toISOString(),
    event,
    payload
  };
  // Default console sink (dev)
  // eslint-disable-next-line no-console
  console.debug('[WF][telemetry]', record);
  for (const sink of sinks) {
    try { sink(record); } catch { /* ignore */ }
  }
}

/**
 * Emit a workflow telemetry event.
 * Safe in production: failures in sinks are swallowed.
 */
