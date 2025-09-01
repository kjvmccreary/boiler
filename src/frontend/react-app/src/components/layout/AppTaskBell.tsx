import { useEffect, useState } from 'react';
import {
  IconButton,
  Badge,
  Popover,
  Box,
  Typography,
  Divider,
  LinearProgress,
  Button,
  Stack
} from '@mui/material';
import NotificationsIcon from '@mui/icons-material/Notifications';
import { getMyTaskSummary, TaskCountsDto } from '@/services/workflow.service';
import { useNavigate } from 'react-router-dom';

export function AppTaskBell() {
  const [anchorEl, setAnchorEl] = useState<HTMLElement | null>(null);
  const [summary, setSummary] = useState<TaskCountsDto | null>(null);
  const [loading, setLoading] = useState(false);
  const navigate = useNavigate();

  const load = async () => {
    try {
      setLoading(true);
      const s = await getMyTaskSummary();
      setSummary(s);
    } catch {
      // optionally toast
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    load();
    const id = setInterval(load, 60000);
    return () => clearInterval(id);
  }, []);

  const open = Boolean(anchorEl);
  const handleOpen = (e: React.MouseEvent<HTMLElement>) => {
    setAnchorEl(e.currentTarget);
    load();
  };
  const handleClose = () => setAnchorEl(null);

  const actionable = summary?.totalActionable ?? 0;

  const navigateWith = (query: string) => {
    navigate(`/app/workflow/tasks${query}`);
    handleClose();
  };

  const handleRowClick = (key: string, value: number) => {
    if (value === 0) return;
    switch (key) {
      case 'Available (claimable)':
        // Created (claimable)
        navigateWith('?status=Created');
        break;
      case 'Assigned to me':
        navigateWith('?status=Assigned');
        break;
      case 'Assigned to my roles':
        // Also Assigned – but user can claim; same status filter.
        navigateWith('?status=Assigned');
        break;
      case 'Claimed':
        navigateWith('?status=Claimed');
        break;
      case 'In Progress':
        navigateWith('?status=InProgress');
        break;
      case 'Overdue':
        navigateWith('?overdue=true');
        break;
      case 'Completed today':
        navigateWith('?status=Completed');
        break;
      case 'Failed':
        navigateWith('?status=Failed');
        break;
      case 'Total Actionable':
        navigateWith('');
        break;
      default:
        navigateWith('');
        break;
    }
  };

  return (
    <>
      <IconButton color="inherit" onClick={handleOpen} size="large">
        <Badge badgeContent={actionable} color={actionable > 0 ? 'error' : 'default'}>
          <NotificationsIcon />
        </Badge>
      </IconButton>
      <Popover
        open={open}
        anchorEl={anchorEl}
        onClose={handleClose}
        anchorOrigin={{ vertical: 'bottom', horizontal: 'right' }}
        transformOrigin={{ vertical: 'top', horizontal: 'right' }}
        slotProps={{ paper: { sx: { width: 340, p: 1 } } }}
      >
        <Box sx={{ px: 1.5, py: 1 }}>
          <Typography variant="subtitle1">Workflow Tasks</Typography>
          <Typography variant="caption" color="text.secondary">
            {summary ? 'Status snapshot' : 'Loading...'}
          </Typography>
        </Box>
        <Divider />
        {loading && <LinearProgress />}
        {summary && (
          <Stack spacing={1.1} sx={{ p: 1.5 }}>
            <Row label="Available (claimable)" value={summary.available} onClick={handleRowClick} highlight />
            <Row label="Assigned to me" value={summary.assignedToMe} onClick={handleRowClick} highlight />
            <Row label="Assigned to my roles" value={summary.assignedToMyRoles} onClick={handleRowClick} />
            <Row label="Claimed" value={summary.claimed} onClick={handleRowClick} />
            <Row label="In Progress" value={summary.inProgress} onClick={handleRowClick} />
            <Row label="Overdue" value={summary.overdue} onClick={handleRowClick} color={summary.overdue > 0 ? 'error.main' : undefined} />
            <Row label="Completed today" value={summary.completedToday} onClick={handleRowClick} />
            {summary.failed > 0 && <Row label="Failed" value={summary.failed} onClick={handleRowClick} color="error.main" />}
            <Divider />
            <Row label="Total Actionable" value={summary.totalActionable} onClick={handleRowClick} strong />
            <Button
              size="small"
              variant="contained"
              onClick={() => {
                navigateWith('');
              }}
            >
              Go to My Tasks
            </Button>
          </Stack>
        )}
      </Popover>
    </>
  );
}

interface RowProps {
  label: string;
  value: number;
  strong?: boolean;
  highlight?: boolean;
  color?: string;
  onClick?: (label: string, value: number) => void;
}

function Row({ label, value, strong, highlight, color, onClick }: RowProps) {
  const clickable = !!onClick && value > 0;
  return (
    <Box
      onClick={() => clickable && onClick(label, value)}
      sx={{
        display: 'flex',
        justifyContent: 'space-between',
        typography: 'body2',
        fontWeight: strong ? 600 : 400,
        bgcolor: highlight && value > 0 ? 'action.hover' : 'transparent',
        px: 1,
        py: 0.5,
        borderRadius: 1,
        color,
        cursor: clickable ? 'pointer' : 'default',
        transition: 'background-color .15s',
        '&:hover': clickable
          ? {
            backgroundColor: 'action.selected'
          }
          : undefined
      }}
    >
      <span>{label}</span>
      <span>{value}</span>
    </Box>
  );
}

export default AppTaskBell;
