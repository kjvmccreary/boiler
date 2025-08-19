import { useState } from 'react';
import {
  Box,
  Button,
  Dialog,
  DialogTitle,
  DialogContent,
  List,
  ListItem,
  ListItemButton,
  ListItemText,
  ListItemIcon,
  Typography,
  IconButton,
  Chip,
  Avatar,
  Divider,
  Alert,
  CircularProgress,
} from '@mui/material';
import {
  Business as BusinessIcon,
  SwapHoriz as SwapHorizIcon,
  Close as CloseIcon,
  CheckCircle as CheckCircleIcon,
} from '@mui/icons-material';
import { useTenant } from '@/contexts/TenantContext.js';
import { useAuth } from '@/contexts/AuthContext.js';
import toast from 'react-hot-toast';

interface TenantSwitcherProps {
  variant?: 'button' | 'menu-item';
  onClose?: () => void;
}

export function TenantSwitcher({ variant = 'button', onClose }: TenantSwitcherProps) {
  const [open, setOpen] = useState(false);
  const [switching, setSwitching] = useState<string | null>(null);
  const { currentTenant, availableTenants, switchTenant } = useTenant();
  const { user } = useAuth();

  const handleOpen = () => {
    setOpen(true);
  };

  const handleClose = () => {
    setOpen(false);
    onClose?.();
  };

  const handleSwitchTenant = async (tenantId: string) => {
    if (tenantId === currentTenant?.id) {
      handleClose();
      return;
    }

    try {
      setSwitching(tenantId);
      await switchTenant(tenantId);
      toast.success('Organization switched successfully');
      handleClose();
    } catch (error) {
      console.error('Failed to switch tenant:', error);
      toast.error('Failed to switch organization');
    } finally {
      setSwitching(null);
    }
  };

  const getTenantInitials = (name: string) => {
    return name
      .split(' ')
      .map(word => word.charAt(0))
      .join('')
      .toUpperCase()
      .substring(0, 2);
  };

  // Don't show if user has only one tenant
  if (availableTenants.length <= 1) {
    return null;
  }

  const TriggerComponent = variant === 'button' ? (
    <Button
      startIcon={<SwapHorizIcon />}
      onClick={handleOpen}
      variant="outlined"
      size="small"
    >
      Switch Organization
    </Button>
  ) : (
    <ListItemButton onClick={handleOpen}>
      <ListItemIcon>
        <SwapHorizIcon fontSize="small" />
      </ListItemIcon>
      <ListItemText primary="Switch Organization" />
    </ListItemButton>
  );

  return (
    <>
      {TriggerComponent}

      <Dialog
        open={open}
        onClose={handleClose}
        maxWidth="sm"
        fullWidth
        PaperProps={{
          sx: { minHeight: 400 }
        }}
      >
        <DialogTitle>
          <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
            <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
              <BusinessIcon color="primary" />
              <Typography variant="h6">Switch Organization</Typography>
            </Box>
            <IconButton onClick={handleClose} size="small">
              <CloseIcon />
            </IconButton>
          </Box>
        </DialogTitle>

        <DialogContent sx={{ px: 0 }}>
          {currentTenant && (
            <Box sx={{ px: 3, mb: 2 }}>
              <Alert severity="info" sx={{ mb: 2 }}>
                Currently signed in as <strong>{user?.firstName} {user?.lastName}</strong>
              </Alert>
              
              <Typography variant="body2" color="text.secondary" gutterBottom>
                Current Organization
              </Typography>
              <Box sx={{ display: 'flex', alignItems: 'center', gap: 2, mb: 2 }}>
                <Avatar sx={{ bgcolor: 'primary.main' }}>
                  {getTenantInitials(currentTenant.name)}
                </Avatar>
                <Box>
                  <Typography variant="subtitle1" fontWeight="medium">
                    {currentTenant.name}
                  </Typography>
                  <Chip label="Active" size="small" color="success" />
                </Box>
              </Box>
            </Box>
          )}

          <Divider sx={{ mb: 1 }} />

          <Box sx={{ px: 3, mb: 1 }}>
            <Typography variant="body2" color="text.secondary">
              Available Organizations ({availableTenants.length})
            </Typography>
          </Box>

          <List dense>
            {availableTenants.map((tenant) => {
              const isCurrentTenant = tenant.id === currentTenant?.id;
              const isSwitching = switching === tenant.id;

              return (
                <ListItem key={tenant.id} disablePadding>
                  <ListItemButton
                    onClick={() => handleSwitchTenant(tenant.id)}
                    disabled={isSwitching}
                    selected={isCurrentTenant}
                  >
                    <ListItemIcon>
                      {isSwitching ? (
                        <CircularProgress size={24} />
                      ) : (
                        <Avatar sx={{ 
                          bgcolor: isCurrentTenant ? 'success.main' : 'primary.main',
                          width: 32,
                          height: 32
                        }}>
                          {getTenantInitials(tenant.name)}
                        </Avatar>
                      )}
                    </ListItemIcon>

                    <ListItemText
                      primary={
                        <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
                          <Typography
                            variant="subtitle2"
                            sx={{ 
                              fontWeight: isCurrentTenant ? 'bold' : 'normal'
                            }}
                          >
                            {tenant.name}
                          </Typography>
                          {isCurrentTenant && (
                            <CheckCircleIcon color="success" fontSize="small" />
                          )}
                        </Box>
                      }
                      secondary={
                        <Box>
                          <Typography variant="caption" color="text.secondary">
                            {tenant.domain || 'No domain set'}
                          </Typography>
                          <br />
                          <Chip 
                            label={tenant.subscriptionPlan || 'Basic'} 
                            size="small" 
                            variant="outlined"
                            sx={{ mt: 0.5 }}
                          />
                        </Box>
                      }
                    />

                    {isSwitching && (
                      <Box sx={{ ml: 1 }}>
                        <Typography variant="caption" color="primary">
                          Switching...
                        </Typography>
                      </Box>
                    )}
                  </ListItemButton>
                </ListItem>
              );
            })}
          </List>

          <Box sx={{ px: 3, mt: 2 }}>
            <Alert severity="info">
              <Typography variant="body2">
                💡 <strong>Tip:</strong> You have access to {availableTenants.length} organizations. 
                Switching will reload your permissions and data for the selected organization.
              </Typography>
            </Alert>
          </Box>
        </DialogContent>
      </Dialog>
    </>
  );
}

export default TenantSwitcher;
