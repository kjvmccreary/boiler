import { ActiveTasksCount, emptyActiveTasksCount } from "../types/ActiveTasksCount";

interface ApiEnvelope<T> {
  success: boolean;
  data: T;
  message?: string;
  errors?: { code: string; message: string; field?: string; value?: unknown }[];
}

const ENDPOINT = "/api/workflow/tasks/active-counts";

/**
 * Fetch active task counts for the current user (scoped to tenant).
 */
export async function getActiveTasksCount(signal?: AbortSignal): Promise<ActiveTasksCount> {
  const resp = await fetch(ENDPOINT, {
    method: "GET",
    headers: {
      "Accept": "application/json"
    },
    signal
  });

  if (!resp.ok) {
    // Non-success HTTP: return empty baseline (caller can decide to surface error)
    return emptyActiveTasksCount;
  }

  let json: ApiEnvelope<ActiveTasksCount> | undefined;
  try {
    json = await resp.json();
  } catch {
    return emptyActiveTasksCount;
  }

  if (!json || !json.success || !json.data) {
    return emptyActiveTasksCount;
  }

  return json.data;
}
