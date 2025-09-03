import { useState, useEffect } from 'react';
import {
  Box,
  Card,
  CardContent,
  Typography,
  List,
  ListItem,
  ListItemButton,
  ListItemText,
  ListItemAvatar,
  Avatar,
  Button,
  CircularProgress,
  Alert,
  Container,
  Chip,
} from '@mui/material';
import { Business, CheckCircle, Domain } from '@mui/icons-material';
import type { Tenant } from '@/types/index.js';
import { useTenant } from '@/contexts/TenantContext.js';

interface TenantSelectorProps {
  onTenantSelected: (tenantId: string) => void;
}

/**
 * Tenant selection screen.
 * Recently adjusted to expose stable test hooks + clearer accessibility.
 */
export function TenantSelector({ onTenantSelected }: TenantSelectorProps) {
  const { availableTenants, isLoading, error } = useTenant();
  const [selectedTenantId, setSelectedTenantId] = useState<string | null>(null);
  const [isSelecting, setIsSelecting] = useState(false);

  // Auto-select if only one tenant
  useEffect(() => {
    if (availableTenants.length === 1 && !selectedTenantId) {
      setSelectedTenantId(availableTenants[0].id);
    }
  }, [availableTenants, selectedTenantId]);

  const handleTenantSelect = (tenant: Tenant) => {
    setSelectedTenantId(tenant.id);
  };

  const handleContinue = async () => {
    if (!selectedTenantId) return;
    setIsSelecting(true);
    try {
      // Delegates actual switching to caller
      // (App / context layer)
      console.log('🏢 TenantSelector: Tenant selected:', selectedTenantId);
      await onTenantSelected(selectedTenantId);
    } catch (err) {
      console.error('❌ TenantSelector: Failed to select tenant:', err);
    } finally {
      setIsSelecting(false);
    }
  };

  if (isLoading) {
    return (
      <Container component="main" maxWidth="sm" data-testid="tenant-selector-loading">
        <Box sx={{
          marginTop: 8,
          display: 'flex',
          flexDirection: 'column',
          alignItems: 'center'
        }}>
          <CircularProgress role="progressbar" />
          <Typography variant="body2" sx={{ mt: 2 }}>
            Loading your organizations...
          </Typography>
        </Box>
      </Container>
    );
  }

  return (
    <Container component="main" maxWidth="sm" data-testid="tenant-selector-root">
      <Box sx={{
        marginTop: 8,
        display: 'flex',
        flexDirection: 'column',
        alignItems: 'center',
      }}>
        <Card sx={{ width: '100%', maxWidth: 500 }}>
          <CardContent sx={{ p: 4 }}>
            <Typography
              component="h1"
              variant="h4"
              align="center"
              gutterBottom
              data-testid="tenant-selector-title"
            >
              Select Organization
            </Typography>

            <Typography
              variant="body1"
              align="center"
              color="text.secondary"
              sx={{ mb: 3 }}
              data-testid="tenant-selector-subtitle"
            >
              {availableTenants.length === 1
                ? 'Connecting to your organization...'
                : 'You have access to multiple organizations. Please select one to continue.'}
            </Typography>

            {error && (
              <Alert severity="error" sx={{ mb: 2 }} data-testid="tenant-selector-error">
                {error}
              </Alert>
            )}

            {availableTenants.length === 0 && !isLoading && (
              <Alert severity="warning" sx={{ mb: 2 }} data-testid="tenant-selector-empty">
                You don't have access to any organizations. Please contact your administrator.
              </Alert>
            )}

            <List
              sx={{ mb: 3 }}
              data-testid="tenant-list"
              aria-label="Available organizations"
            >
              {availableTenants.map((tenant) => {
                const selected = selectedTenantId === tenant.id;
                return (
                  <ListItem
                    key={tenant.id}
                    disablePadding
                    data-testid={`tenant-item-wrapper-${tenant.id}`}
                  >
                    <ListItemButton
                      component="button"
                      role="button"
                      aria-selected={selected ? 'true' : 'false'}
                      data-testid={`tenant-item-${tenant.id}`}
                      selected={selected}
                      onClick={() => handleTenantSelect(tenant)}
                      sx={{
                        borderRadius: 1,
                        mb: 1,
                        border: selected ? 2 : 1,
                        borderColor: selected ? 'primary.main' : 'divider',
                        outline: 0,
                        '&[aria-selected="true"]': {
                          backgroundColor: 'action.hover'
                        }
                      }}
                    >
                      <ListItemAvatar>
                        <Avatar sx={{ bgcolor: 'primary.main' }}>
                          {tenant.domain ? <Domain /> : <Business />}
                        </Avatar>
                      </ListItemAvatar>
                      <ListItemText
                        primary={
                          <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
                            <span data-testid={`tenant-name-${tenant.id}`}>{tenant.name}</span>
                            {tenant.subscriptionPlan && (
                              <Chip
                                label={tenant.subscriptionPlan}
                                size="small"
                                variant="outlined"
                                data-testid={`tenant-plan-${tenant.id}`}
                                color={tenant.subscriptionPlan === 'Development' ? 'warning' : 'default'}
                              />
                            )}
                          </Box>
                        }
                        secondary={tenant.domain || 'No domain configured'}
                      />
                      {selected && (
                        <CheckCircle color="primary" data-testid="tenant-selected-icon" />
                      )}
                    </ListItemButton>
                  </ListItem>
                );
              })}
            </List>

            <Button
              fullWidth
              variant="contained"
              size="large"
              onClick={handleContinue}
              data-testid="tenant-continue-button"
              aria-label="Continue with selected organization"
              disabled={!selectedTenantId || isSelecting || availableTenants.length === 0}
              startIcon={isSelecting ? <CircularProgress size={20} /> : undefined}
            >
              {isSelecting
                ? 'Switching Organization...'
                : availableTenants.length === 1
                  ? 'Continue'
                  : 'Select Organization'}
            </Button>
          </CardContent>
        </Card>
      </Box>
    </Container>
  );
}

export default TenantSelector;
