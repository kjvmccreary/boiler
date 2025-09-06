import * as signalR from '@microsoft/signalr';

let connection: signalR.HubConnection | null = null;

export interface InstanceUpdatedEvent {
  instanceId: number;
  status: string;
  currentNodeIds: string;
  completedAt?: string | null;
  errorMessage?: string | null;
}

export interface InstancesChangedEvent {
  tenantId: number;
}

export interface InstanceProgressEvent {
  instanceId: number;
  percentage: number;
  visitedCount: number;
  totalNodes: number;
  status: string;
  activeNodeIds: string[]; // emitted from backend dispatcher
}

type InstanceUpdatedListener = (e: InstanceUpdatedEvent) => void;
type InstancesChangedListener = (e: InstancesChangedEvent) => void;
type InstanceProgressListener = (e: InstanceProgressEvent) => void;

const instanceUpdatedListeners = new Set<InstanceUpdatedListener>();
const instancesChangedListeners = new Set<InstancesChangedListener>();
const instanceProgressListeners = new Set<InstanceProgressListener>();

export function onInstanceUpdated(listener: InstanceUpdatedListener) {
  instanceUpdatedListeners.add(listener);
  return () => instanceUpdatedListeners.delete(listener);
}

export function onInstancesChanged(listener: InstancesChangedListener) {
  instancesChangedListeners.add(listener);
  return () => instancesChangedListeners.delete(listener);
}

export function onInstanceProgress(listener: InstanceProgressListener) {
  instanceProgressListeners.add(listener);
  return () => instanceProgressListeners.delete(listener);
}

export async function startInstanceHub(): Promise<void> {
  if (connection && connection.state === signalR.HubConnectionState.Connected) return;
  if (connection && connection.state === signalR.HubConnectionState.Connecting) return;

  connection = new signalR.HubConnectionBuilder()
    .withUrl('/api/workflow/hubs/instances', {
      transport: signalR.HttpTransportType.WebSockets,
      accessTokenFactory: () => localStorage.getItem('auth_token') || ''
    })
    .withAutomaticReconnect([0, 2000, 5000, 10000, 20000])
    .build();

  connection.on('InstanceUpdated', (payload: InstanceUpdatedEvent) => {
    instanceUpdatedListeners.forEach(l => { try { l(payload); } catch { } });
  });

  connection.on('InstancesChanged', (payload: InstancesChangedEvent) => {
    instancesChangedListeners.forEach(l => { try { l(payload); } catch { } });
  });

  connection.on('InstanceProgress', (payload: InstanceProgressEvent) => {
    instanceProgressListeners.forEach(l => { try { l(payload); } catch { } });
  });

  try {
    await connection.start();
  } catch {
    // silent; automatic reconnect handles retries
  }
}

export async function stopInstanceHub(): Promise<void> {
  if (connection) {
    try { await connection.stop(); } catch { }
    connection = null;
  }
}
