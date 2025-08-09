import {
  Box,
  CircularProgress,
  LinearProgress,
  Skeleton,
  Card,
  CardContent,
  Typography,
  List,
  ListItem,
  ListItemAvatar,
  ListItemText,
} from '@mui/material';

// Basic loading spinner
interface LoadingSpinnerProps {
  size?: number;
  message?: string;
  fullHeight?: boolean;
}

export function LoadingSpinner({ size = 40, message, fullHeight = false }: LoadingSpinnerProps) {
  return (
    <Box
      sx={{
        display: 'flex',
        flexDirection: 'column',
        alignItems: 'center',
        justifyContent: 'center',
        minHeight: fullHeight ? '50vh' : 100,
        gap: 2,
      }}
    >
      <CircularProgress size={size} />
      {message && (
        <Typography variant="body2" color="text.secondary">
          {message}
        </Typography>
      )}
    </Box>
  );
}

// Page loading with progress bar
interface PageLoadingProps {
  message?: string;
  progress?: number;
}

export function PageLoading({ message = 'Loading...', progress }: PageLoadingProps) {
  return (
    <Box sx={{ width: '100%', mt: 2 }}>
      <Box sx={{ display: 'flex', alignItems: 'center', mb: 1 }}>
        <Box sx={{ minWidth: 35 }}>
          <Typography variant="body2" color="text.secondary">
            {progress !== undefined ? `${Math.round(progress)}%` : ''}
          </Typography>
        </Box>
      </Box>
      <LinearProgress 
        variant={progress !== undefined ? 'determinate' : 'indeterminate'} 
        value={progress}
      />
      <Typography variant="body2" color="text.secondary" sx={{ mt: 1 }}>
        {message}
      </Typography>
    </Box>
  );
}

// Table skeleton loader
interface TableSkeletonProps {
  rows?: number;
  columns?: number;
  showHeader?: boolean;
}

export function TableSkeleton({ rows = 5, columns = 4, showHeader = true }: TableSkeletonProps) {
  return (
    <Box>
      {showHeader && (
        <Box sx={{ display: 'flex', gap: 2, mb: 2 }}>
          {Array.from({ length: columns }).map((_, index) => (
            <Skeleton
              key={`header-${index}`}
              variant="text"
              width="100%"
              height={40}
              sx={{ flex: 1 }}
            />
          ))}
        </Box>
      )}
      {Array.from({ length: rows }).map((_, rowIndex) => (
        <Box key={`row-${rowIndex}`} sx={{ display: 'flex', gap: 2, mb: 1 }}>
          {Array.from({ length: columns }).map((_, colIndex) => (
            <Skeleton
              key={`cell-${rowIndex}-${colIndex}`}
              variant="text"
              width="100%"
              height={32}
              sx={{ flex: 1 }}
            />
          ))}
        </Box>
      ))}
    </Box>
  );
}

// User list skeleton
interface UserListSkeletonProps {
  count?: number;
}

export function UserListSkeleton({ count = 5 }: UserListSkeletonProps) {
  return (
    <List>
      {Array.from({ length: count }).map((_, index) => (
        <ListItem key={`user-skeleton-${index}`} sx={{ mb: 1 }}>
          <ListItemAvatar>
            <Skeleton variant="circular" width={40} height={40} />
          </ListItemAvatar>
          <ListItemText
            primary={<Skeleton variant="text" width="60%" />}
            secondary={<Skeleton variant="text" width="40%" />}
          />
          <Box sx={{ ml: 2 }}>
            <Skeleton variant="rectangular" width={60} height={24} sx={{ borderRadius: 1 }} />
          </Box>
        </ListItem>
      ))}
    </List>
  );
}

// Role card skeleton
interface RoleCardSkeletonProps {
  count?: number;
}

export function RoleCardSkeleton({ count = 3 }: RoleCardSkeletonProps) {
  return (
    <Box
      sx={{
        display: 'grid',
        gridTemplateColumns: {
          xs: '1fr',
          sm: 'repeat(2, 1fr)',
          md: 'repeat(3, 1fr)',
        },
        gap: 2,
      }}
    >
      {Array.from({ length: count }).map((_, index) => (
        <Card key={`role-skeleton-${index}`}>
          <CardContent>
            <Box sx={{ display: 'flex', alignItems: 'center', mb: 2 }}>
              <Skeleton variant="circular" width={32} height={32} sx={{ mr: 2 }} />
              <Skeleton variant="text" width="70%" height={28} />
            </Box>
            <Skeleton variant="text" width="100%" height={20} sx={{ mb: 1 }} />
            <Skeleton variant="text" width="80%" height={20} sx={{ mb: 2 }} />
            <Box sx={{ display: 'flex', gap: 1 }}>
              <Skeleton variant="rectangular" width={60} height={24} sx={{ borderRadius: 1 }} />
              <Skeleton variant="rectangular" width={80} height={24} sx={{ borderRadius: 1 }} />
            </Box>
          </CardContent>
        </Card>
      ))}
    </Box>
  );
}

// Form skeleton
interface FormSkeletonProps {
  fields?: number;
}

export function FormSkeleton({ fields = 5 }: FormSkeletonProps) {
  return (
    <Box sx={{ width: '100%', maxWidth: 600 }}>
      {Array.from({ length: fields }).map((_, index) => (
        <Box key={`form-field-${index}`} sx={{ mb: 3 }}>
          <Skeleton variant="text" width="30%" height={20} sx={{ mb: 1 }} />
          <Skeleton variant="rectangular" width="100%" height={56} sx={{ borderRadius: 1 }} />
        </Box>
      ))}
      <Box sx={{ display: 'flex', gap: 2, mt: 4 }}>
        <Skeleton variant="rectangular" width={100} height={36} sx={{ borderRadius: 1 }} />
        <Skeleton variant="rectangular" width={80} height={36} sx={{ borderRadius: 1 }} />
      </Box>
    </Box>
  );
}

// Dashboard skeleton
export function DashboardSkeleton() {
  return (
    <Box>
      {/* Header */}
      <Box sx={{ mb: 4 }}>
        <Skeleton variant="text" width="40%" height={40} sx={{ mb: 1 }} />
        <Skeleton variant="text" width="60%" height={24} />
      </Box>

      {/* Stats cards */}
      <Box
        sx={{
          display: 'grid',
          gridTemplateColumns: {
            xs: '1fr',
            sm: 'repeat(2, 1fr)',
            md: 'repeat(4, 1fr)',
          },
          gap: 3,
          mb: 4,
        }}
      >
        {Array.from({ length: 4 }).map((_, index) => (
          <Card key={`stat-${index}`}>
            <CardContent>
              <Box sx={{ display: 'flex', alignItems: 'center', mb: 2 }}>
                <Skeleton variant="circular" width={48} height={48} sx={{ mr: 2 }} />
                <Box sx={{ flexGrow: 1 }}>
                  <Skeleton variant="text" width="80%" height={24} />
                  <Skeleton variant="text" width="60%" height={20} />
                </Box>
              </Box>
              <Skeleton variant="text" width="40%" height={32} />
            </CardContent>
          </Card>
        ))}
      </Box>

      {/* Content area */}
      <Box
        sx={{
          display: 'grid',
          gridTemplateColumns: {
            xs: '1fr',
            md: '2fr 1fr',
          },
          gap: 3,
        }}
      >
        <Card>
          <CardContent>
            <Skeleton variant="text" width="30%" height={28} sx={{ mb: 2 }} />
            <TableSkeleton rows={6} columns={3} />
          </CardContent>
        </Card>
        <Card>
          <CardContent>
            <Skeleton variant="text" width="50%" height={28} sx={{ mb: 2 }} />
            <UserListSkeleton count={4} />
          </CardContent>
        </Card>
      </Box>
    </Box>
  );
}

// Permission selector skeleton
export function PermissionSelectorSkeleton() {
  return (
    <Box>
      <Box sx={{ display: 'flex', alignItems: 'center', mb: 2 }}>
        <Skeleton variant="text" width="30%" height={24} sx={{ mr: 2 }} />
        <Skeleton variant="rectangular" width={80} height={24} sx={{ borderRadius: 1 }} />
      </Box>
      
      <Box sx={{ mb: 2 }}>
        <Skeleton variant="rectangular" width="100%" height={40} sx={{ borderRadius: 1 }} />
      </Box>

      {Array.from({ length: 4 }).map((_, categoryIndex) => (
        <Card key={`category-${categoryIndex}`} sx={{ mb: 1 }}>
          <Box sx={{ p: 2, display: 'flex', alignItems: 'center' }}>
            <Skeleton variant="rectangular" width={24} height={24} sx={{ mr: 2 }} />
            <Skeleton variant="text" width="30%" height={24} sx={{ mr: 2 }} />
            <Skeleton variant="rectangular" width={50} height={20} sx={{ borderRadius: 1 }} />
          </Box>
          <Box sx={{ px: 2, pb: 2 }}>
            {Array.from({ length: 3 }).map((_, permIndex) => (
              <Box key={`perm-${permIndex}`} sx={{ display: 'flex', alignItems: 'center', mb: 1, ml: 4 }}>
                <Skeleton variant="rectangular" width={20} height={20} sx={{ mr: 2 }} />
                <Skeleton variant="text" width="60%" height={20} />
              </Box>
            ))}
          </Box>
        </Card>
      ))}
    </Box>
  );
}

// Generic content skeleton
interface ContentSkeletonProps {
  variant?: 'card' | 'list' | 'grid';
  count?: number;
}

export function ContentSkeleton({ variant = 'card', count = 3 }: ContentSkeletonProps) {
  if (variant === 'list') {
    return <UserListSkeleton count={count} />;
  }

  if (variant === 'grid') {
    return <RoleCardSkeleton count={count} />;
  }

  // Default card variant
  return (
    <Box sx={{ display: 'flex', flexDirection: 'column', gap: 2 }}>
      {Array.from({ length: count }).map((_, index) => (
        <Card key={`content-skeleton-${index}`}>
          <CardContent>
            <Skeleton variant="text" width="70%" height={28} sx={{ mb: 1 }} />
            <Skeleton variant="text" width="100%" height={20} sx={{ mb: 1 }} />
            <Skeleton variant="text" width="80%" height={20} />
          </CardContent>
        </Card>
      ))}
    </Box>
  );
}
