import React, { useCallback, useEffect, useMemo, useRef, useState } from 'react';
import {
  Box,
  Card,
  CardContent,
  Typography,
  Chip,
  Stack,
  IconButton,
  Tooltip,
  Button,
  TextField,
  MenuItem,
  Divider,
  CircularProgress,
  Alert
} from '@mui/material';
import RefreshIcon from '@mui/icons-material/Refresh';
import FilterAltIcon from '@mui/icons-material/FilterAlt';
import TimelineIcon from '@mui/icons-material/Timeline';
import ClearAllIcon from '@mui/icons-material/ClearAll';
import type { WorkflowEventDto } from '@/types/workflow';
import { workflowService } from '@/services/workflow.service';
import toast from 'react-hot-toast';

interface InstanceEventTimelineProps {
  instanceId: number;
  initialEvents?: WorkflowEventDto[];
  disabledAuto?: boolean;
}

interface EventPage {
  items: WorkflowEventDto[];
  totalCount?: number;
  page: number;
  pageSize: number;
}

const DEFAULT_PAGESIZE = 50;

export const InstanceEventTimeline: React.FC<InstanceEventTimelineProps> = ({
  instanceId,
  initialEvents,
  disabledAuto
}) => {
  const [events, setEvents] = useState<WorkflowEventDto[]>(() => initialEvents || []);
  const [loading, setLoading] = useState(false);
  const [loadingMore, setLoadingMore] = useState(false);
  const [page, setPage] = useState(1);
  const [totalCount, setTotalCount] = useState<number | undefined>(undefined);
  const [typeFilter, setTypeFilter] = useState<string>('');
  const [search, setSearch] = useState('');
  const [fromDate, setFromDate] = useState('');
  const [toDate, setToDate] = useState('');
  const abortRef = useRef<AbortController | null>(null);

  const canLoadMore = totalCount === undefined
    ? events.length > 0 && events.length % DEFAULT_PAGESIZE === 0
    : events.length < totalCount;

  const distinctTypes = useMemo(
    () => Array.from(new Set(events.map(e => e.eventType))).sort(),
    [events]
  );

  const resetAndReload = () => {
    setPage(1);
    void loadPage(1, false);
  };

  const loadPage = useCallback(async (targetPage: number, append: boolean) => {
    if (!instanceId) return;
    if (abortRef.current) abortRef.current.abort();
    const localAbort = new AbortController();
    abortRef.current = localAbort;
    try {
      targetPage === 1 ? setLoading(true) : setLoadingMore(true);

      let pageData: EventPage | null = null;

      if ((workflowService as any).getInstanceEvents) {
        const res = await (workflowService as any).getInstanceEvents(instanceId, {
          page: targetPage,
          pageSize: DEFAULT_PAGESIZE,
          eventType: typeFilter || undefined,
          search: search || undefined,
          from: fromDate || undefined,
          to: toDate || undefined,
          signal: localAbort.signal
        });
        pageData = {
          items: res.items || res.events || [],
          totalCount: res.totalCount,
          page: targetPage,
          pageSize: DEFAULT_PAGESIZE
        };
      } else {
        // Fallback: get snapshot & locally filter
        const snap = await workflowService.getRuntimeSnapshot(instanceId);
        let evts: WorkflowEventDto[] = snap.events || [];
        evts = applyLocalFilters(evts, { typeFilter, search, fromDate, toDate });
        const start = (targetPage - 1) * DEFAULT_PAGESIZE;
        const slice = evts.slice(start, start + DEFAULT_PAGESIZE);
        pageData = {
          items: slice,
          totalCount: evts.length,
          page: targetPage,
          pageSize: DEFAULT_PAGESIZE
        };
      }

      if (!pageData) return;
      setTotalCount(pageData.totalCount);
      setPage(targetPage);
      setEvents(prev => append ? [...prev, ...pageData!.items] : pageData!.items);
    } catch (e: any) {
      if (e?.name !== 'AbortError') {
        toast.error('Failed to load events');
      }
    } finally {
      setLoading(false);
      setLoadingMore(false);
    }
  }, [instanceId, typeFilter, search, fromDate, toDate]);

  useEffect(() => {
    if (!disabledAuto && events.length === 0) {
      void loadPage(1, false);
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [instanceId]);

  const applyFilters = () => resetAndReload();
  const clearFilters = () => {
    setTypeFilter('');
    setSearch('');
    setFromDate('');
    setToDate('');
    setTimeout(() => resetAndReload(), 0);
  };

  const grouped = useMemo(() => {
    const map = new Map<string, WorkflowEventDto[]>();
    for (const e of events) {
      const day = e.occurredAt?.substring(0, 10) || 'Unknown';
      if (!map.has(day)) map.set(day, []);
      map.get(day)!.push(e);
    }
    return Array.from(map.entries())
      .sort((a, b) => b[0].localeCompare(a[0]))
      .map(([day, list]) => ({
        day,
        list: list.sort((a, b) => b.occurredAt.localeCompare(a.occurredAt))
      }));
  }, [events]);

  const loadMore = () => {
    if (!canLoadMore || loadingMore) return;
    void loadPage(page + 1, true);
  };

  return (
    <Card sx={{ mb: 3 }}>
      <CardContent>
        <Box display="flex" justifyContent="space-between" flexWrap="wrap" gap={2} alignItems="center" mb={2}>
          <Stack direction="row" spacing={1} alignItems="center">
            <TimelineIcon fontSize="small" />
            <Typography variant="h6" component="div">
              Event Timeline
              <Typography variant="caption" color="text.secondary" ml={1}>
                ({events.length}{totalCount !== undefined ? ` / ${totalCount}` : ''})
              </Typography>
            </Typography>
          </Stack>
          <Stack direction="row" spacing={1} alignItems="center">
            <Tooltip title="Refresh (reset & reload first page)">
              <span>
                <IconButton size="small" onClick={() => resetAndReload()} disabled={loading}>
                  <RefreshIcon fontSize="small" />
                </IconButton>
              </span>
            </Tooltip>
          </Stack>
        </Box>

        {/* Filters */}
        <Box
          sx={{
            display: 'grid',
            gap: 1.5,
            gridTemplateColumns: { xs: '1fr', md: 'repeat(5, minmax(120px,1fr))' },
            mb: 2
          }}
        >
          <TextField
            select
            size="small"
            label="Type"
            value={typeFilter}
            onChange={(e) => setTypeFilter(e.target.value)}
          >
            <MenuItem value="">(All)</MenuItem>
            {distinctTypes.map(t => (
              <MenuItem key={t} value={t}>{t}</MenuItem>
            ))}
          </TextField>
          <TextField
            size="small"
            label="Search (name/data)"
            value={search}
            onChange={(e) => setSearch(e.target.value)}
            placeholder="Contains..."
          />
            <TextField
            size="small"
            label="From"
            type="date"
            value={fromDate}
            onChange={(e) => setFromDate(e.target.value)}
            InputLabelProps={{ shrink: true }}
          />
            <TextField
              size="small"
              label="To"
              type="date"
              value={toDate}
              onChange={(e) => setToDate(e.target.value)}
              InputLabelProps={{ shrink: true }}
            />
          <Stack direction="row" spacing={1} justifyContent="flex-start" alignItems="center">
            <Tooltip title="Apply Filters">
              <span>
                <Button
                  size="small"
                  variant="contained"
                  startIcon={<FilterAltIcon />}
                  onClick={applyFilters}
                  disabled={loading}
                >
                  Apply
                </Button>
              </span>
            </Tooltip>
            <Tooltip title="Clear Filters">
              <span>
                <IconButton
                  size="small"
                  color="warning"
                  onClick={clearFilters}
                  disabled={loading && events.length === 0}
                >
                  <ClearAllIcon fontSize="small" />
                </IconButton>
              </span>
            </Tooltip>
          </Stack>
        </Box>

        {loading && events.length === 0 && (
          <Box display="flex" gap={2} alignItems="center" py={4} justifyContent="center">
            <CircularProgress size={20} />
            <Typography variant="body2">Loading events...</Typography>
          </Box>
        )}

        {!loading && events.length === 0 && (
          <Alert severity="info" variant="outlined">No events match the current filters.</Alert>
        )}

        {/* Grouped timeline */}
        <Box>
          {grouped.map(group => (
            <Box key={group.day} mb={2}>
              <Typography variant="subtitle2" color="text.secondary" sx={{ mb: 0.5 }}>
                {group.day}
              </Typography>
              <Divider sx={{ mb: 1 }} />
              <Stack spacing={1.2}>
                {group.list.map(e => (
                  <EventRow key={e.id} evt={e} highlightSearch={search} />
                ))}
              </Stack>
            </Box>
          ))}
        </Box>

        {/* Load More */}
        {events.length > 0 && (
          <Box display="flex" justifyContent="center" mt={2}>
            <Button
              size="small"
              variant="outlined"
              onClick={loadMore}
              disabled={!canLoadMore || loadingMore}
            >
              {loadingMore ? 'Loading...' : canLoadMore ? 'Load More' : 'End of Timeline'}
            </Button>
          </Box>
        )}
      </CardContent>
    </Card>
  );
};

const EventRow: React.FC<{ evt: WorkflowEventDto; highlightSearch?: string }> = ({ evt, highlightSearch }) => {
  const typeColor = mapTypeColor(evt.eventType);
  const occurred = new Date(evt.occurredAt).toLocaleTimeString();

  let dataPreview = '';
  if (evt.data) {
    const raw = JSON.stringify(evt.data);
    dataPreview = raw.length > 200 ? `${raw.substring(0, 200)}…` : raw;
  }

  const highlight = (text: string) => {
    if (!highlightSearch || !text) return text;
    const lower = text.toLowerCase();
    const needle = highlightSearch.toLowerCase();
    const idx = lower.indexOf(needle);
    if (idx === -1) return text;
    return (
      <>
        {text.substring(0, idx)}
        <mark style={{ background: 'rgba(255,215,0,0.4)', padding: 0 }}>
          {text.substring(idx, idx + needle.length)}
        </mark>
        {text.substring(idx + needle.length)}
      </>
    );
  };

  return (
    <Box
      sx={{
        display: 'grid',
        gap: 1,
        gridTemplateColumns: { xs: '80px 1fr', md: '100px 140px 1fr 110px' },
        alignItems: 'flex-start',
        p: 1,
        borderRadius: 1,
        '&:hover': { backgroundColor: 'action.hover' }
      }}
    >
      <Typography variant="caption" sx={{ fontFamily: 'monospace' }}>{occurred}</Typography>
      <Box sx={{ display: { xs: 'none', md: 'flex' } }}>
        <Chip
          size="small"
          label={evt.eventType}
          color={typeColor}
          variant={typeColor === 'default' ? 'outlined' : 'filled'}
          sx={{ height: 22, fontSize: '0.65rem' }}
        />
      </Box>
      <Box>
        <Typography
          variant="body2"
          sx={{ fontFamily: 'monospace', fontSize: '0.75rem', wordBreak: 'break-word' }}
        >
          {highlight(evt.name || '(no name)')}
        </Typography>
        {dataPreview && (
          <Typography
            variant="caption"
            color="text.secondary"
            sx={{ fontFamily: 'monospace', display: 'block', mt: 0.25 }}
          >
            {highlight(dataPreview)}
          </Typography>
        )}
      </Box>
      <Box sx={{ display: { xs: 'none', md: 'flex' }, justifyContent: 'flex-end' }}>
        <Chip
          size="small"
          label={evt.userId ? `User ${evt.userId}` : 'System'}
          variant="outlined"
          sx={{ fontSize: '0.6rem', height: 20 }}
        />
      </Box>
    </Box>
  );
};

function mapTypeColor(type: string):
  'default' | 'primary' | 'secondary' | 'error' | 'info' | 'warning' | 'success' {
  const t = type.toLowerCase();
  if (t.includes('error') || t.includes('fail')) return 'error';
  if (t.includes('task')) return 'primary';
  if (t.includes('node')) return 'info';
  if (t.includes('signal') || t.includes('event')) return 'secondary';
  if (t.includes('timer')) return 'warning';
  if (t.includes('complete') || t.includes('success')) return 'success';
  return 'default';
}

function applyLocalFilters(list: WorkflowEventDto[], filters: {
  typeFilter: string;
  search: string;
  fromDate: string;
  toDate: string;
}): WorkflowEventDto[] {
  return list.filter(e => {
    if (filters.typeFilter && e.eventType !== filters.typeFilter) return false;
    if (filters.search) {
      const s = filters.search.toLowerCase();
      const dataStr = e.data ? JSON.stringify(e.data).toLowerCase() : '';
      if (!(e.name?.toLowerCase().includes(s) || dataStr.includes(s))) return false;
    }
    if (filters.fromDate && e.occurredAt < filters.fromDate) return false;
    if (filters.toDate && e.occurredAt > filters.toDate + 'T23:59:59') return false;
    return true;
  });
}

export default InstanceEventTimeline;
