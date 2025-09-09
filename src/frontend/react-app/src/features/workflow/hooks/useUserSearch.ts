import { useEffect, useState } from 'react';
import { userService } from '@/services/user.service';

export interface UseUserSearchResult {
  results: { id: string; displayName: string; email?: string }[];
  loading: boolean;
  term: string;
  setTerm: (v: string) => void;
  clear: () => void;
}

export function useUserSearch(debounceMs = 300, active: boolean): UseUserSearchResult {
  const [term, setTerm] = useState('');
  const [results, setResults] = useState<UseUserSearchResult['results']>([]);
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    if (!active) {
      setResults([]);
      return;
    }
    if (!term.trim()) {
      setResults([]);
      return;
    }
    const handle = setTimeout(async () => {
      setLoading(true);
      try {
        const r = await userService.search(term);
        setResults(r);
      } catch {
        setResults([]);
      } finally {
        setLoading(false);
      }
    }, debounceMs);
    return () => clearTimeout(handle);
  }, [term, debounceMs, active]);

  return {
    results,
    loading,
    term,
    setTerm,
    clear: () => { setTerm(''); setResults([]); }
  };
}
