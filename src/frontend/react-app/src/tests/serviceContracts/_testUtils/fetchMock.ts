/* Simple per-test fetch mock helper.
   Usage:
     const restore = mockFetchOnce(jsonPayload);
     ...call service...
     restore();
*/
export function mockFetchOnce(payload: any, init?: { status?: number }) {
  const original = global.fetch;
  global.fetch = (async () =>
    ({
      ok: (init?.status ?? 200) >= 200 && (init?.status ?? 200) < 300,
      status: init?.status ?? 200,
      json: async () => payload
    }) as any) as any;
  return () => {
    global.fetch = original;
  };
}

export function assertNoEnvelope(result: any) {
  if (result == null || typeof result !== 'object') return;
  if ('success' in result) {
    throw new Error('Result still contains envelope key: success');
  }
  if ('data' in result) {
    // Allow paged results that are already unwrapped (items etc.)
    const val = (result as any).data;
    // If data is an array or object with items we consider this still wrapped
    if (val !== undefined) {
      throw new Error('Result still contains top-level data envelope');
    }
  }
}
