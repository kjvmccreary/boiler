import { HubConnection, HubConnectionBuilder, LogLevel } from "@microsoft/signalr";
import { ActiveTasksCount } from "../types/ActiveTasksCount";

/**
 * Lightweight singleton wrapper for task-related SignalR events.
 * Server is expected to broadcast:
 *  - "ActiveTasksCountChanged" (payload: ActiveTasksCount)
 *  - "TasksChanged" (payload: any) – optional existing event
 */
class TaskHub {
  private connection: HubConnection | null = null;
  private started = false;
  private activeCountHandlers = new Set<(c: ActiveTasksCount) => void>();
  private tasksChangedHandlers = new Set<(payload: any) => void>();
  private startPromise: Promise<void> | null = null;

  // Adjust if backend uses a different hub route (e.g., /hubs/tasks or /taskNotificationsHub)
  private hubUrl = "/hubs/tasks";

  configure(url: string) {
    if (this.started) {
      // ignore reconfig after start
      return;
    }
    this.hubUrl = url;
  }

  private ensureConnection() {
    if (!this.connection) {
      this.connection = new HubConnectionBuilder()
        .withUrl(this.hubUrl)
        .withAutomaticReconnect()
        .configureLogging(LogLevel.Warning)
        .build();

      this.connection.on("ActiveTasksCountChanged", (payload: ActiveTasksCount) => {
        this.activeCountHandlers.forEach(h => {
          try { h(payload); } catch { /* ignore */ }
        });
      });

      this.connection.on("TasksChanged", (payload: any) => {
        this.tasksChangedHandlers.forEach(h => {
          try { h(payload); } catch { /* ignore */ }
        });
      });

      this.connection.onclose(() => {
        this.started = false;
        this.startPromise = null;
      });
    }
  }

  async start(): Promise<void> {
    this.ensureConnection();
    if (this.started) return;
    if (this.startPromise) return this.startPromise;
    this.startPromise = (async () => {
      try {
        await this.connection!.start();
        this.started = true;
      } catch {
        // swallow; will retry lazily on next subscribe
        this.started = false;
        this.startPromise = null;
      }
    })();
    return this.startPromise;
  }

  onActiveCounts(handler: (c: ActiveTasksCount) => void) {
    this.activeCountHandlers.add(handler);
    this.start(); // fire & forget
  }

  offActiveCounts(handler: (c: ActiveTasksCount) => void) {
    this.activeCountHandlers.delete(handler);
  }

  onTasksChanged(handler: (payload: any) => void) {
    this.tasksChangedHandlers.add(handler);
    this.start();
  }

  offTasksChanged(handler: (payload: any) => void) {
    this.tasksChangedHandlers.delete(handler);
  }
}

export const taskHub = new TaskHub();
