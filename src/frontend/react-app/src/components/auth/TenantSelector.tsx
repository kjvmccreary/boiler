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

export function TenantSelector({ onTenantSelected }: TenantSelectorProps) {
  const { availableTenants, isLoading, error } = useTenant(); // 🔧 REVERT: Remove switchTenant
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
      // 🔧 REVERT: Just call the prop function - App.tsx will handle the switching logic
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
      <Container component="main" maxWidth="sm">
        <Box sx={{ 
          marginTop: 8, 
          display: 'flex', 
          flexDirection: 'column', 
          alignItems: 'center' 
        }}>
          <CircularProgress />
          <Typography variant="body2" sx={{ mt: 2 }}>
            Loading your organizations...
          </Typography>
        </Box>
      </Container>
    );
  }

  return (
    <Container component="main" maxWidth="sm">
      <Box sx={{
        marginTop: 8,
        display: 'flex',
        flexDirection: 'column',
        alignItems: 'center',
      }}>
        <Card sx={{ width: '100%', maxWidth: 500 }}>
          <CardContent sx={{ p: 4 }}>
            <Typography component="h1" variant="h4" align="center" gutterBottom>
              Select Organization
            </Typography>
            
            <Typography variant="body1" align="center" color="text.secondary" sx={{ mb: 3 }}>
              {availableTenants.length === 1 
                ? 'Connecting to your organization...'
                : 'You have access to multiple organizations. Please select one to continue.'
              }
            </Typography>

            {error && (
              <Alert severity="error" sx={{ mb: 2 }}>
                {error}
              </Alert>
            )}

            {availableTenants.length === 0 && !isLoading && (
              <Alert severity="warning" sx={{ mb: 2 }}>
                You don't have access to any organizations. Please contact your administrator.
              </Alert>
            )}

            <List sx={{ mb: 3 }}>
              {availableTenants.map((tenant) => (
                <ListItem key={tenant.id} disablePadding>
                  <ListItemButton
                    selected={selectedTenantId === tenant.id}
                    onClick={() => handleTenantSelect(tenant)}
                    sx={{
                      borderRadius: 1,
                      mb: 1,
                      border: selectedTenantId === tenant.id ? 2 : 1,
                      borderColor: selectedTenantId === tenant.id 
                        ? 'primary.main' 
                        : 'divider',
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
                          {tenant.name}
                          {tenant.subscriptionPlan && (
                            <Chip 
                              label={tenant.subscriptionPlan} 
                              size="small" 
                              variant="outlined"
                              color={tenant.subscriptionPlan === 'Development' ? 'warning' : 'default'}
                            />
                          )}
                        </Box>
                      }
                      secondary={tenant.domain || 'No domain configured'}
                    />
                    {selectedTenantId === tenant.id && (
                      <CheckCircle color="primary" />
                    )}
                  </ListItemButton>
                </ListItem>
              ))}
            </List>

            <Button
              fullWidth
              variant="contained"
              size="large"
              onClick={handleContinue}
              disabled={!selectedTenantId || isSelecting || availableTenants.length === 0}
              startIcon={isSelecting ? <CircularProgress size={20} /> : undefined}
            >
              {isSelecting ? 'Switching Organization...' : 
               availableTenants.length === 1 ? 'Continue' : 'Select Organization'}
            </Button>
          </CardContent>
        </Card>
      </Box>
    </Container>
  );
}
