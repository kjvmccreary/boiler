import { vi } from 'vitest';
import { apiClient } from '../../../services/api.client';

// Shape expected by workflow.service unwrap()
interface ApiEnvelope<T> {
  success: boolean;
  data: T;
  message?: string;
  errors?: any;
}

type HttpMethod = 'get' | 'post' | 'put' | 'delete';

interface QueueItem {
  method: HttpMethod;
  response: any;
}

const queue: QueueItem[] = [];

/**
 * Queue a mock envelope (success wrapper) for the next matching apiClient.<method> call.
 * response = raw data you would normally find under data (we auto-wrap success:true).
 */
export function enqueueOk(method: HttpMethod, data: any) {
  queue.push({ method, response: { success: true, data } });
}

/**
 * Queue an axios-style error envelope.
 * We THROW an error shaped like axios would (error.response.data = envelope)
 * so higher-level catch logic (e.g., publishDefinition custom handler) can inspect it.
 */
export function enqueueErr(method: HttpMethod, message: string, errors?: any) {
  queue.push({
    method,
    response: { success: false, message, errors }
  });
}

// Install spies only once
let installed = false;
export function installApiClientMocks() {
  if (installed) return;
  installed = true;

  (['get', 'post', 'put', 'delete'] as HttpMethod[]).forEach(m => {
    vi.spyOn(apiClient, m).mockImplementation(async () => {
      const idx = queue.findIndex(q => q.method === m);
      if (idx === -1) {
        throw new Error(`No mock queued for apiClient.${m}()`);
      }
      const item = queue.splice(idx, 1)[0];
      if (item.response && item.response.success === false) {
        const err: any = new Error(item.response.message || 'Operation failed');
        err.response = { data: item.response, status: 400 };
        throw err;
      }
      return { data: item.response } as any;
    });
  });
}

/**
 * Utility to ensure the queue is empty after a test (optional).
 */
export function assertQueueDrained() {
  if (queue.length > 0) {
    throw new Error(`Undrained apiClient mock queue: ${queue.map(q => q.method).join(', ')}`);
  }
}

/**
 * Clear any pending mocks (use in afterEach if desired).
 */
export function resetApiQueue() {
  queue.splice(0, queue.length);
}
