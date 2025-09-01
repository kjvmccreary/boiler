import * as signalR from '@microsoft/signalr';

let connection: signalR.HubConnection | null = null;

type Listener = () => void;
const listeners = new Set<Listener>();

export function onTasksChanged(listener: Listener) {
  listeners.add(listener);
  return () => listeners.delete(listener);
}

export async function startTaskHub(): Promise<void> {
  if (connection && connection.state === signalR.HubConnectionState.Connected) return;

  connection = new signalR.HubConnectionBuilder()
    .withUrl('/api/workflow/hubs/tasks', {
      transport: signalR.HttpTransportType.WebSockets,
      accessTokenFactory: () => localStorage.getItem('auth_token') || ''
    })
    .withAutomaticReconnect([0, 2000, 5000, 10000])
    .build();

  connection.on('TasksChanged', () => {
    listeners.forEach(l => l());
  });

  connection.onclose(() => {
    // Optionally log
  });

  try {
    await connection.start();
    // console.log('Task hub connected');
  } catch {
    // swallow; auto-reconnect
  }
}

export async function stopTaskHub(): Promise<void> {
  if (connection) {
    try { await connection.stop(); } catch { /* ignore */ }
    connection = null;
  }
}
